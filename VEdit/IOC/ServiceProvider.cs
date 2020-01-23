using Ninject;
using VEdit.Common;

namespace VEdit
{
    public class ServiceProvider : IServiceProvider
    {
        private IKernel _kernel;

        public ServiceProvider(IKernel kernel) => _kernel = kernel;

        public T Get<T>() => _kernel.Get<T>();
    }
}
