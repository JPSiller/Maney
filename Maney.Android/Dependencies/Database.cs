using System.IO;
using Capuchinos.Maney.Droid.Dependencies;
using Xamarin.Forms;
using Capuchinos.Maney.Dependencies;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;

[assembly: Dependency(typeof(Database))]
namespace Capuchinos.Maney.Droid.Dependencies
{
    public class Database : IDatabase
    {
        const string FileName = "Maney.db3";
        public SQLiteConnection GetConnection()
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, FileName);

            var platform = new SQLitePlatformAndroid();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }
    }
}