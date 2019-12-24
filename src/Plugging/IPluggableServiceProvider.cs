using JetBrains.Annotations;

namespace Plugging
{
    public interface IPluggableServiceProvider
    {
        [CanBeNull]
        T GetService<T>([NotNull] string moduleName)
            where T : class;
    }
}
