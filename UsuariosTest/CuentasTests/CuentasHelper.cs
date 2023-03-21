using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;
using WebApplication1;
using WebApplication1.Controllers;
using WebApplication1.Models;

namespace UsuariosTest.UsuariosTests
{
   public class CuentasHelper
   {
      private readonly ApplicationDbContext _applicationDbContext;
      private readonly UserManager<Usuario> _userManager;
      private readonly SignInManager<Usuario> _signInManager;
      private readonly RoleManager<Rol> _roleManager;

      public CuentasHelper()
      {
         Guid dbInstanceId = Guid.NewGuid();
         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
             .UseInMemoryDatabase(databaseName: $"DBTest-{dbInstanceId}")
             .Options;

         _applicationDbContext = new ApplicationDbContext(options);

         var roleStore = new RoleStore<Rol>(_applicationDbContext);
         _roleManager = BuildRoleManager(roleStore);

         var miUserStore = new UserStore<Usuario>(_applicationDbContext);
         _userManager = BuildUserManager(miUserStore);

         var httpContext = new DefaultHttpContext();
         MockAuth(httpContext);

         _signInManager = SetupSignInManager(_userManager, httpContext);

         CargarBaseDeDatosEnMemoria();
      }

      public CuentasController InMemoryCuentasController()
      {
         return new CuentasController(_userManager, _signInManager, _roleManager);
      }

      private UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
      {
         store = store ?? new Mock<IUserStore<TUser>>().Object;
         var options = new Mock<IOptions<IdentityOptions>>();
         var idOptions = new IdentityOptions();
         idOptions.Lockout.AllowedForNewUsers = false;

         options.Setup(o => o.Value).Returns(idOptions);

         var userValidators = new List<IUserValidator<TUser>>();

         var validator = new Mock<IUserValidator<TUser>>();
         userValidators.Add(validator.Object);
         var pwdValidators = new List<PasswordValidator<TUser>>();
         pwdValidators.Add(new PasswordValidator<TUser>());

         var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
             userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
             new IdentityErrorDescriber(), null,
             new Mock<ILogger<UserManager<TUser>>>().Object);

         validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
             .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

         return userManager;
      }

      private RoleManager<TRole> BuildRoleManager<TRole>(IRoleStore<TRole> store = null) where TRole : class
      {
         store = store ?? new Mock<IRoleStore<TRole>>().Object;
         var roles = new List<IRoleValidator<TRole>>();
         roles.Add(new RoleValidator<TRole>());
         return new RoleManager<TRole>(store, roles,
             MockLookupNormalizer(),
             new IdentityErrorDescriber(),
             null);
      }

      private ILookupNormalizer MockLookupNormalizer()
      {
         var normalizerFunc = new Func<string, string>(i =>
         {
            if (i == null)
            {
               return null;
            }
            else
            {
               return Convert.ToBase64String(Encoding.UTF8.GetBytes(i)).ToUpperInvariant();
            }
         });
         var lookupNormalizer = new Mock<ILookupNormalizer>();
         lookupNormalizer.Setup(i => i.NormalizeName(It.IsAny<string>())).Returns(normalizerFunc);
         lookupNormalizer.Setup(i => i.NormalizeEmail(It.IsAny<string>())).Returns(normalizerFunc);
         return lookupNormalizer.Object;
      }

      private static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
            HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
            IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
      {
         var contextAccessor = new Mock<IHttpContextAccessor>();
         contextAccessor.Setup(a => a.HttpContext).Returns(context);
         identityOptions = identityOptions ?? new IdentityOptions();
         var options = new Mock<IOptions<IdentityOptions>>();
         options.Setup(a => a.Value).Returns(identityOptions);
         var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager, options.Object);
         schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
         var sm = new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, schemeProvider, new DefaultUserConfirmation<TUser>());
         sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
         return sm;
      }

      private Mock<IAuthenticationService> MockAuth(HttpContext context)
      {
         var auth = new Mock<IAuthenticationService>();
         context.RequestServices = new ServiceCollection().AddSingleton(auth.Object)
            .BuildServiceProvider();
         return auth;
      }

      public async void CargarBaseDeDatosEnMemoria()
      {
         List<Rol> roles = new()
         {
            new Rol { Name = "ADMIN" },
            new Rol { Name = "CLIENTE"}
         };

         foreach (var rol in roles)
         {
            _roleManager.CreateAsync(rol).Wait();
         }

         List<Usuario> usuarios = new()
         {
            new Usuario { UserName = "user1@test.com", Email = "user1@test.com" },
            new Usuario { UserName = "user2@test.com", Email = "user2@test.com" }
         };

         foreach (var user in usuarios)
         {
            await _userManager.CreateAsync(user, "Admin123*");
         }
      }
   }
}
