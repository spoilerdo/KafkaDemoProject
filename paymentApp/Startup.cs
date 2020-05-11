using AutoMapper;
using Confluent.Kafka;
using KafkaProducer.Persistence.Context;
using KafkaProducer.Persistence.Repositories.Payments;
using KafkaProducer.Persistence.Repositories.Users;
using KafkaProducer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace KafkaProducer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddAutoMapper(typeof(Startup));

            var configuration = new MapperConfiguration(cfg => {
                cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
            });
            configuration.CompileMappings();

            services.AddDbContext<ProducerDbContext>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ProcessPaymentService>();

            var producerConfig = new ProducerConfig();
            Configuration.Bind("ProducerConfig", producerConfig);
            services.AddSingleton(producerConfig);

            var consumerConfig = new ConsumerConfig();
            Configuration.Bind("ConsumerConfig", consumerConfig);
            consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
            services.AddSingleton(consumerConfig);

            services.AddSingleton<IHostedService, ProcessPaymentService.ProcessPayment>();
            services.AddSingleton<IHostedService, ProcessPaymentService.ProcessPaymentFailed>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<PaymentService>();
                endpoints.MapGrpcService<UserService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
