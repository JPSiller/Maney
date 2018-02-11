using SQLite.Net.Attributes;

namespace Capuchinos.Maney.Model
{
    [Table("Settings")]
    public class Settings
    {
        [PrimaryKey]
        public short Id { get; set; }
        public string DefaultLanguage { get; set; }
        public string DefaultCurrency { get; set; }
        public bool Purchased { get; set; }
    }
}
