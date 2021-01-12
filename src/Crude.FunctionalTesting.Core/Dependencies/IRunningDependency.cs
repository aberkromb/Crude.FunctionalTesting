using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Core.Dependencies
{
    public interface IRunningDependency: IDisposable
    {
        /// <summary>
        ///     Устанавливает необходимые настройки после запуска зависимости
        /// </summary>
        void ConfigureService(IConfiguration configuration, IServiceCollection services);

        /// <summary>
        ///     Выполняется после того как зависимость запустилась
        /// </summary>
        Task<IDependency> AfterDependencyStart(CancellationToken cancellationToken);
    }
}