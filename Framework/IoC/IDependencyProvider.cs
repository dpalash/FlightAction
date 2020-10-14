using Unity;

namespace Framework.IoC
{
    public interface IDependencyProvider
    {
        void RegisterDependencies(IUnityContainer container);
    }
}
