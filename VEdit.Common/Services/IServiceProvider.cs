namespace VEdit.Common
{
    public interface IServiceProvider
    {
        T Get<T>();
    }
}
