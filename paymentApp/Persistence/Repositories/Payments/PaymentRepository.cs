using KafkaProducer.Common;
using KafkaProducer.Persistence.Context;
using KafkaProducer.Persistence.Entities;
using KafkaProducer.Persistence.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Persistence.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ProducerDbContext _context;
        private readonly IUserRepository _userRepository;

        public PaymentRepository(ProducerDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<DataResponseObject<PaymentEntity>> CreatePayment(PaymentEntity payment)
        {
            var sender = await _context.Users.Where(u => u.Id == payment.SenderId).FirstAsync();
            if (sender == null)
                return new DataResponseObject<PaymentEntity>($"Sender with id {payment.SenderId} doesn't exist");

            payment.Sender = sender;
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return new DataResponseObject<PaymentEntity>(payment);
        }

        public async Task<DataResponseObject<PaymentEntity>> DeletePayment(Guid id)
        {
            var foundPayment = await _context.Payments.FindAsync(id);
            if(foundPayment == null)
            {
                return new DataResponseObject<PaymentEntity>("Payment could not be found");
            }
            _context.Payments.Remove(foundPayment);
            await _context.SaveChangesAsync();
            return new DataResponseObject<PaymentEntity>(true);
        }

        public async Task<DataResponseObject<PaymentEntity>> Pay(Guid id)
        {
            var foundPayment = await _context.Payments.FindAsync(id);
            if (foundPayment == null)
                return new DataResponseObject<PaymentEntity>("Payment not found");

            var foundSender = await _userRepository.GetUserById(foundPayment.SenderId);
            if (!foundSender.Success)
                return new DataResponseObject<PaymentEntity>(foundSender.Message);

            var foundReceiver = await _userRepository.GetUserById(foundPayment.ReceiverId);
            if (!foundReceiver.Success)
                return new DataResponseObject<PaymentEntity>(foundReceiver.Message);

            var sender = foundSender.Data;
            var receiver = foundReceiver.Data;

            receiver.Balance -= foundPayment.Amount;
            sender.Balance += foundPayment.Amount;
            foundPayment.Sender = sender;

            _context.Entry(receiver).CurrentValues.SetValues(receiver);
            _context.Entry(sender).CurrentValues.SetValues(sender);

            foundPayment.Payed = true;
            _context.Entry(foundPayment).CurrentValues.SetValues(foundPayment);
            await _context.SaveChangesAsync();
            return new DataResponseObject<PaymentEntity>(foundPayment);
        }

        public async Task<DataResponseObject<IEnumerable<PaymentEntity>>> GetPaymentsFromUser(Guid id)
        {
            var foundPayments = await _context.Payments.Where(p => p.SenderId == id).ToListAsync();
            if (foundPayments == null)
                return new DataResponseObject<IEnumerable<PaymentEntity>>("No Payments found");

            return new DataResponseObject<IEnumerable<PaymentEntity>>(foundPayments);
        }
    }
}
