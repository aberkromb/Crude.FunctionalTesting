using System;
using System.Threading.Tasks;

namespace Crude.FunctionalTesting.Dependencies.Postgres
{
    /// <summary>
    ///     Зависимость postgres
    /// </summary>
    public class PostgresDependency : IDependency
    {
        public Task Execute() => throw new NotImplementedException();
    }
}