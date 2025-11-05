namespace Observatorio.Repo.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;

        protected BaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}