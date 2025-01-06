using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecargaApi.middlewares
{
    public class RoleBasedAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RoleBasedAuthorizationMiddleware> _logger;

        public RoleBasedAuthorizationMiddleware(RequestDelegate next, ILogger<RoleBasedAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Verificar si el usuario está autenticado
            Console.WriteLine("User is authenticated: " + context.User.Identity.IsAuthenticated);
            /*if (!context.User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("User is not authenticated.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }*/

            // Obtener los roles del usuario desde las claims del JWT
            var roles = context.User.FindAll("groups").Select(c => c.Value).ToList();

            // Agregar los roles como claims de tipo Role
            foreach (var role in roles)
            {
                context.User.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, role) }));
            }

            // Continuar con la solicitud
            await _next(context);
        }
    }

}
