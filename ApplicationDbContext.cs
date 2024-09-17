using loginClase.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace loginClase
{
	public class ApplicationDbContext : IdentityDbContext
	{
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			//AQUI VAN LLAVES PRIMARIAS Y FORANEAS

		}

		//Van los modelos que se convertiran en tablas. Modelo va en singular, pero a bd sera en plural.
		//ej: Cliente, se manda como Clientes.
		//esto es pa q entity framework pueda mapear el modelo y mandarlo a la bd
		public DbSet<Departamento> Departamentos { get; set; }


	}
}
