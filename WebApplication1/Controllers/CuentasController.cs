using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
   [ApiController]
   [Route("[controller]")]

   public class CuentasController : ControllerBase
   {
      //private ApplicationDbContext _db;
      private SignInManager<Usuario> _signInManager;
      private UserManager<Usuario> _userManager;
      private RoleManager<Rol> _roleManager;

      public CuentasController(UserManager<Usuario> userManager, SignInManager<Usuario> singInManager, 
         RoleManager<Rol> roleManager)
      {
         //_db = db;
         _userManager = userManager;
         _signInManager = singInManager;
         _roleManager = roleManager;
      }

      [HttpPost("login")]
      public async Task<bool> login(LoginDTO loginDTO)
      {
         var usuario = await _userManager.FindByNameAsync(loginDTO.usuario);
         if (usuario == null) { return false; }
         var result = await _signInManager.PasswordSignInAsync(loginDTO.usuario, loginDTO.password,
            isPersistent: false, lockoutOnFailure: false);
         if (result.Succeeded)
         {
            return true;
         }
         return false;
      }

      [HttpGet("GetRoles")]
      public async Task<IEnumerable<Rol>> GetRoles()
      {
         var roles = await _roleManager.Roles.CountAsync();
         return await _roleManager.Roles.ToListAsync();
      }

      [HttpPost("CrearRol")]
      public async Task<bool> PostRol(string name)
      {
         var rol = new Rol
         {
            Name = name,
         };
         var res = await _roleManager.CreateAsync(rol);
         if (!res.Succeeded) { return false; }
         return true;
      }

      [HttpPost("CrearUsuario")]
      public async Task<bool> PostUsuario(CrearUsuarioDto crearUsuarioDto)
      {
         var usuario = new Usuario
         {
            UserName = crearUsuarioDto.UserName,
            Email = crearUsuarioDto.Email,
         };
         var res = await _userManager.CreateAsync(usuario, crearUsuarioDto.Password);

         if (!res.Succeeded) { return false; }

         var resultAgregarRol = await _userManager.AddToRoleAsync(usuario, "ADMIN");

         if (!resultAgregarRol.Succeeded)
         {
            return false;
         }

         return true;
      }

      [HttpGet("GetUsuarios")]
      public async Task<IEnumerable<Usuario>> GetUsuarios()
      {
         var usuarios = await _userManager.Users.ToListAsync();
         return usuarios;

      }
   }
}