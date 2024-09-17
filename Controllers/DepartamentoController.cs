using loginClase;
using loginClase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace loginClase.Controllers
{
	[ApiController]
	[Route("api/[controller]")] //dominio/api/departamento/metodo. se puede hardkodear
	public class DepartamentoController : Controller
	{
		private readonly ApplicationDbContext context;
		
		public DepartamentoController(ApplicationDbContext context) 
		{
			this.context = context;
		}

		[HttpPost]
		public async Task<ActionResult> Post(Departamento departamento)
		{
			//consulta pa ver si existe el nombre
			var deptoExiste = await context.Departamentos.AnyAsync(x => x.Nombre == departamento.Nombre);

			if (deptoExiste)
				return BadRequest($"Departamento {departamento.Nombre} duplicado");

			//contexto de datos
			context.Add(departamento);
			//manda y agrega a bd
			await context.SaveChangesAsync();

			return Ok();
		}

		[HttpGet("{id:int}", Name = "ObtenerDepartamento")]
		public async Task<ActionResult<Departamento>> GetObtenerDepartamento(int id)
		{
			//busqueda de depto el primero o el default
			var depto = await context.Departamentos.FirstOrDefaultAsync(x => x.Id == id);

			if (depto == null)
				return NotFound();

			return Ok(depto);
		}

		//hacer endpoint que regrese todos los deptos
		[HttpGet("ListarDepartamentos")]
		public async Task<ActionResult<List<Departamento>>> ListarDepartamentos()
		{
			var deptos = await context.Departamentos.ToListAsync();
			return Ok(deptos);
		}

		[HttpPut("ModificarDepto/{id:int}")]
		public async Task<ActionResult> ModificaDepto(int id, Departamento departamento)
		{
			var deptoExiste = await context.Departamentos.FirstOrDefaultAsync(x => x.Id == id);

			if(deptoExiste == null)
				return NotFound();

			deptoExiste.Nombre = departamento.Nombre;
			deptoExiste.Descripcion = departamento.Descripcion;

			context.Update(deptoExiste);

			await context.SaveChangesAsync();
			return Ok(departamento);
		}

		[HttpDelete("{id:int}")]
		public async Task <ActionResult> Eliminar(int id)
		{
			var deptoExiste = await context.Departamentos.FirstOrDefaultAsync(x => x.Id == id);

			if(deptoExiste == null)
				return NotFound();

			context.Remove(deptoExiste);
			await context.SaveChangesAsync();
			return Ok();
		}
	}
}
