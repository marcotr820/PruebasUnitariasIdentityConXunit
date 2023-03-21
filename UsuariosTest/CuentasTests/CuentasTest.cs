using WebApplication1.DTOs;

namespace UsuariosTest.UsuariosTests
{
   public class CuentasTest
   {
      [Fact]
      public async Task CrearUsuario_ReturnSuccess()
      {
         var helper = new CuentasHelper();

         var controller = helper.InMemoryCuentasController();

         var usuario = new CrearUsuarioDto
         {
            UserName = "test",
            Email = "test@gmail.com",
            Password = "Admin123*"
         };

         var result = await controller.PostUsuario(usuario);

         Assert.True(result);
      }

      [Fact]
      public async Task GetUsuarios_ReturnAllUsers()
      {
         var helper = new CuentasHelper();

         var controller = helper.InMemoryCuentasController();

         var result = await controller.GetUsuarios();

         Assert.Equal(2, result.Count());
      }

      [Fact]
      public async Task Login_Usuario_Success()
      {
         var helper = new CuentasHelper();

         var controller = helper.InMemoryCuentasController();

         LoginDTO login = new() { usuario = "user1@test.com", password = "Admin123*" };

         var result = await controller.login(login);

         Assert.True(result);
      }

      [Fact]
      public async Task GetRoles_ReturnAllRoles()
      {
         var helper = new CuentasHelper();

         var controller = helper.InMemoryCuentasController();

         var result = await controller.GetRoles();

         Assert.Equal(2, result.Count());
      }

      [Fact]
      public async Task CrearRol()
      {
         var helper = new CuentasHelper();

         var controller = helper.InMemoryCuentasController();

         var result = await controller.PostRol("RolPrueba");

         Assert.True(result);
      }
   }
}
