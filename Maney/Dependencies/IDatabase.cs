namespace Capuchinos.Maney.Dependencies
{
    public interface IDatabase
    {
        SQLite.Net.SQLiteConnection GetConnection();
    }
}
