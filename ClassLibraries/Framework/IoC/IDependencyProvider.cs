using Unity;

namespace Framework.IoC
{
    public interface IDependencyProvider
    {
        IUnityContainer RegisterDependencies(IUnityContainer container);
    }
}
