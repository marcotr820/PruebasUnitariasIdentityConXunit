using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
   public class Rol : IdentityRole
   {
      public ICollection<UsuarioRol> UsuariosRoles { get; set; }
   }
}
