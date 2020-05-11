using Google.Protobuf.WellKnownTypes;
using KafkaProducer.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace KafkaProducer.Persistence.Context
{
    public class ProducerDbContext : DbContext
    {
        public ProducerDbContext(DbContextOptions<ProducerDbContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<PaymentEntity>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<PaymentEntity>()
                .HasOne(p => p.Sender)
                .WithMany(u => u.Payments);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
        }

        public DbSet<PaymentEntity> Payments { get; set; }
        public DbSet<UserEntity> Users { get; set; }
    }
}
