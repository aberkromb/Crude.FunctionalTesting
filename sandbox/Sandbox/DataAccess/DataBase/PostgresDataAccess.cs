using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sandbox.DataAccess.DataBase
{
    public class PostgresDataAccess : IDataAccess
    {
        private readonly PostgresDbContext _dbContext;

        public PostgresDataAccess(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Strings> Get(int id, CancellationToken cancellationToken)
        {
            return _dbContext.Strings.FirstOrDefaultAsync(str => str.Id == id,cancellationToken: cancellationToken);
        }

        public async Task<Strings> Add(Strings value, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(value, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return value;
        }
    }

    public interface IDataAccess
    {
        Task<Strings> Get(int id, CancellationToken cancellationToken);

        Task<Strings> Add(Strings value, CancellationToken cancellationToken);
    }
}