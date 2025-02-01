using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentAPI.Data;
using PaymentAPI.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentAPI.Services
{
    public class PaymentProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentProcessingService> _logger;

        public PaymentProcessingService(IServiceProvider serviceProvider, ILogger<PaymentProcessingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de processamento de pagamentos iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Busca pagamentos pendentes
                    var pendingPayments = dbContext.Payments
                        .Where(p => p.Status == "Pending")
                        .ToList();

                    foreach (var payment in pendingPayments)
                    {
                        _logger.LogInformation($"Processando pagamento ID: {payment.Id}");

                        // Simula um tempo de processamento
                        await Task.Delay(2000, stoppingToken);

                        // Atualiza o status do pagamento
                        payment.Status = "Completed";
                        dbContext.SaveChanges();

                        _logger.LogInformation($"Pagamento ID {payment.Id} concluído.");
                    }
                }

                // Aguarda antes de verificar novamente (5 segundos)
                await Task.Delay(5000, stoppingToken);
            }

            _logger.LogInformation("Serviço de processamento de pagamentos finalizado.");
        }
    }
}
