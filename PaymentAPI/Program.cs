using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Services;

namespace PaymentAPI // Adicionando o namespace
{
    public class Program // Tornando a classe p�blica
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona servi�os ao container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configura��o do banco de dados
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Habilita CORS para permitir chamadas do frontend React
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    policy => policy.WithOrigins("http://localhost:3000") // Ajuste conforme necess�rio
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });

            // Adiciona o servi�o de processamento de pagamentos em segundo plano
            builder.Services.AddHostedService<PaymentProcessingService>();

            var app = builder.Build();

            // Configura��o do pipeline de requisi��es HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigins"); // Aplica��o da pol�tica CORS

            app.UseAuthentication(); // Caso v� usar JWT futuramente
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
