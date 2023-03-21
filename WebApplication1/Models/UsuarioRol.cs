using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
   public class UsuarioRol : IdentityUserRole<string>
   {
      public Usuario Usuario { get; set; }
      public Rol Rol { get; set; }
   }
}
