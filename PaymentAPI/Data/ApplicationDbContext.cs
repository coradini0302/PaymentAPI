using Microsoft.EntityFrameworkCore;
using PaymentAPI.Models;

namespace PaymentAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; }

        // Construtor padrão (necessário para o EF Core funcionar com injeção de dependência)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) // Passa as opções para a classe base
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Chama a implementação base

            // Configuração de precisão para a propriedade 'Amount'
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2); // Define precisão de 18 e escala de 2
        }
    }
}