using System;
using JetBrains.Annotations;

namespace Plugging
{
    public interface IPluggableRegistry<out T>
        where T : class
    {
        [CanBeNull]
        T GetService([NotNull] string moduleName);

        bool Supports<TOperation>([NotNull] string moduleName, [CanBeNull] TOperation operation)
            where TOperation : Enum;
    }
}