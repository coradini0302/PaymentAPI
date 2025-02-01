using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Services;

namespace PaymentAPI // Adicionando o namespace
{
    public class Program // Tornando a classe pública
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona serviços ao container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configuração do banco de dados
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Habilita CORS para permitir chamadas do frontend React
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    policy => policy.WithOrigins("http://localhost:3000") // Ajuste conforme necessário
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });

            // Adiciona o serviço de processamento de pagamentos em segundo plano
            builder.Services.AddHostedService<PaymentProcessingService>();

            var app = builder.Build();

            // Configuração do pipeline de requisições HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigins"); // Aplicação da política CORS

            app.UseAuthentication(); // Caso vá usar JWT futuramente
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
