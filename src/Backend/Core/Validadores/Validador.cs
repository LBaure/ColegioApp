
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Validadores
{
    public class Validador : IValidador
    {
        private readonly IServiceProvider _services;
        public Validador(IServiceProvider services)
        {
            _services = services;
        }

        public void Validar<T>(T modelo)
        {
            var validator = _services.GetService<IValidator<T>>();
            validator?.ValidateAndThrow(modelo);
        }
    }
}
