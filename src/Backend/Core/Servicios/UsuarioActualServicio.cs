using Core.Models;
using Core.Models.Seguridad;
using Core.Models.Sso;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios
{
    public class UsuarioActualServicio : IUsuarioActualServicio
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsuarioActualServicio(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public UsuarioSsoModelo? Get()
        {
            if (_httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity identity)
            {

                var claimTypeCustom = "NumeroNit";
                var claimTypeCustomStatus = "Activo";
                var claim = identity.FindFirst(claimTypeCustom);
                var Nit = claim == null ? string.Empty : claim.Value;

                if (identity.Claims.Any())
                {
                    var userClaims = identity.Claims;
                    return new UsuarioSsoModelo
                    {
                        Nombre = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value ?? "",
                        Correo = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value ?? "",
                        Nit = userClaims.FirstOrDefault(claim => claim.Type == claimTypeCustom)?.Value ?? string.Empty,
                        Activo = userClaims.FirstOrDefault(claim => claim.Type == claimTypeCustomStatus)?.Value == "1" ? true : false
                    };
                }
            }
            return null;
        }
    }
}
