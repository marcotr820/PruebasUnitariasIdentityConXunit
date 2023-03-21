using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class CrearUsuarioDto
    {
        [Required]
        [MinLength(5)]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
