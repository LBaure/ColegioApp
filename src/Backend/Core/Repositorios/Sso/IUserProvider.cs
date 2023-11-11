using Core.Models.Seguridad;
using Core.Models.Sso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositorios.Sso
{
    public interface IUserProvider
    { /// <summary>
      /// Verifica la autenticidad del usuario en base al ticket proporcionado
      /// </summary>
      /// <param name="privateKeyXml">Llave privada de identificación de la aplicación</param>
      /// <param name="ticketAut">Ticket de autorización generado para el usuario</param>
      /// <returns>UsuarioSsoModelo con datos del usuario autorizado</returns>
      /// <exception cref="Excepciones.SsoException">
      /// Lanzada cuando algún parámetro de autenticación no es provisto 
      /// o si el proceso de comunicación con el SSO genera algún error
      /// </exception>
        UsuarioSsoModelo Autenticar(SsoAutModelo SSOReq);
        /// <summary>
        /// Registra el usuario en la base de datos del SSO
        /// </summary>
        /// <param name="usuario">Objeto UsuarioSsoModelo con datos del Usuario a registrar</param>
        /// <param name="privateKeyXml">Llave privada que identifica la aplicación</param>
        /// <returns>
        /// Enum ResultadoRegistro que indica si el proceso se completó
        /// correctamente asignando los permisos a la aplicación o si
        /// el usuario ya se encontraba registrado en el SSO, sólamente le
        /// asigna los permisos sobre la aplicación
        /// </returns>
        /// <exception cref="Excepciones.DatosRequeridosException">
        /// Lanzada cuando no se envían todos los datos requeridos para registrar el usuario
        /// </exception>
        /// <exception cref="Excepciones.NitInvalidoException">
        /// Lanzada cuando el nit proporcionado es inválido
        /// </exception>
        /// <exception cref="Excepciones.SsoException">
        /// Lanzada en caso la llave privada no sea proporcionada
        /// </exception>
        ResultadoRegistro RegistrarUsuario(SSORegistrar SSOReq);
        /// <summary>
        /// Otorga o revoca el acceso del usuario al SRBM
        /// </summary>
        /// <param name="privateKeyXml">Llave privada que identifica a la aplicación</param>
        /// <param name="nit">Nit del usuario a activar o inactivar</param>
        /// <param name="otorgar">
        /// Indica si el permiso sobre el acceso a la aplicación debe ser otorgado o revocado.
        /// True para otorgar o false para revocar.
        /// </param>
        /// <returns>El nuevo estado otorgado al usuario. True para activo o False para inactivo</returns>
        bool CambiarAcceso(SSOAcceso SSOReq);

        string ObtenerCorreoUsuarioSso(SSOObtenerCorreo SSOReq);

        bool ConsultaAccesoSitio(SSOAcceso SSOReq);

        UsuarioModelo ObtenerInfoUsuarioSso(SSOObtenerCorreo SSOReq);


    }

    public enum ResultadoRegistro
    {
        Ok = 0,
        PermisosAsignados = 1,
    }
}
