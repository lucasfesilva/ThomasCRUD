using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ThomasCRUD.Models;

namespace ThomasCRUD.Data
{
    // Representa o contexto de banco de dados da aplicação, estendendo IdentityDbContext para suporte à autenticação e autorização
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        

        // Construtor que recebe as opções de configuração do context
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // Conjunto de dados que representa as ordens armazenadas no banco de dados.
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Logradouro> Logradouros { get; set; }

        //Mapeamento de "Um para Muitos" (onde 1 cliente pode ter vários endereços)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Logradouros)
                .WithOne(l => l.Cliente)
                .HasForeignKey(l => l.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
