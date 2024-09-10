using loginClase.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace loginClase.Controllers
{
	[ApiController]
	[Route("api/[controller]")] //dominio/api/cuentas/metodo. se puede hardkodear
	public class CuentasController : Controller
	{
		private readonly UserManager<IdentityUser> userManager;
		private readonly SignInManager<IdentityUser> signInManager;
		private readonly IConfiguration configuration;

		public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.configuration = configuration;
		}

		//endpoints registrar, login, renovartoken 

		//registrar
		[HttpPost("Registrar")]
		public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
		{
			var usuario = new IdentityUser
			{
				UserName = credencialesUsuario.Email,
				Email = credencialesUsuario.Email
			};

			//creando el usuario y lo almacenamos en esta variable
			var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Contrasena);

			if (resultado.Succeeded)
				return await ConstruirToken(credencialesUsuario);

			return BadRequest(resultado.Errors);
		}

		//con post pa q los datos vayan ocultos.
		[HttpPost("Login")]
		public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
		{
			var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,credencialesUsuario.Contrasena,
																	isPersistent: false, lockoutOnFailure: false);
			if (resultado.Succeeded)
				return await ConstruirToken(credencialesUsuario);

			//para que no sea obvio que es lo que esta mal.
			var error = new MensajeError()
			{ 
				Error = "Login incorrecto"
			};

			return BadRequest(error);
		}

		//renovartoken 
		[HttpGet("RenovarToken")]
		//solo la gente logueada puede acceder a este endpoint con jwt. se pueden poner roles pero aun na
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult<RespuestaAutenticacion>> RenovarT()
		{
			var emailClaim = HttpContext.User.Claims.Where(x => x.Type == "email").FirstOrDefault();
			var credencialesUsuario = new CredencialesUsuario()
			{
				Email = emailClaim!.Value
			};
			return await ConstruirToken(credencialesUsuario);
		}

		//metodo construirtoken q no es endpoint. se mandara llamar con los 3 metodos de arriba.
		private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
		{
			var claims = new List<Claim>()
			{
				new Claim("email", credencialesUsuario.Email)
			};

			//consulta a bd pa q nos traiga al usuario x email (selec * from usuarios)
			var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
			var claimsRoles = await userManager.GetClaimsAsync(usuario!); //para q confie que no viene nula la variable

			//a la lista va a agregar lo nuevo junto a lo q ya tiene.
			claims.AddRange(claimsRoles);

			//traer la llave jwt
			var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["LlaveJWT"]!));

			//encriptamos la info
			var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

			//fecha de hoy con un dia mas.
			var expiracion = DateTime.Now.AddDays(1);

			//ya se puede crear el toquen
			var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);


			return new RespuestaAutenticacion 
			{ 
				Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
				Expiracion = expiracion
			};
		}
	}
}
