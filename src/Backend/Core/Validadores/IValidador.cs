namespace Core.Validadores
{
    public interface IValidador
    {
        void Validar<T>(T modelo);
    }
}
