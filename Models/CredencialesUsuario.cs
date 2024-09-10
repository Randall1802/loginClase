using System.ComponentModel.DataAnnotations;

namespace loginClase.Models
{
	public class CredencialesUsuario
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Contrasena { get; set; }
	}
}
