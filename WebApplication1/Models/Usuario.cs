using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
   public class Usuario : IdentityUser
   {
      public ICollection<UsuarioRol> UsuariosRoles { get; set; }
   }
}
