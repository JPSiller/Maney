using System;
using System.IO;
using Capuchinos.Maney.Dependencies;
using Capuchinos.Maney.iOS.Dependencies;
using SQLite.Net;
using SQLite.Net.Platform.XamarinIOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(Database))]
namespace Capuchinos.Maney.iOS.Dependencies
{
    public class Database : IDatabase
    {
        const string FileName = "Maney.db3";
        public SQLiteConnection GetConnection()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, FileName);

            var platform = new SQLitePlatformIOS();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }
    }
}
