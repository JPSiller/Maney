using System.Collections.Generic;
using Capuchinos.Maney.Helpers;
using SQLite.Net.Attributes;

namespace Capuchinos.Maney.Model
{
    [Table("Language")]
    public class Language
    {
        [PrimaryKey]
        public short Id { get; set; }
        public string Name { get; set; }
        public string Culture { get; set; }

        public static IEnumerable<Language> GetLanguages()
        {
            return new List<Language>
            {
                new Language
                {
                    Id = (short)LanguageName.English,
                    Name = "English",
                    Culture = "en"
                },
                new Language
                {
                    Id = (short)LanguageName.Spanish,
                    Name = "Español",
                    Culture = "es-MX"
                },
                new Language
                {
                    Id = (short)LanguageName.French,
                    Name = "Français",
                    Culture = "fr-FR"
                },
                new Language
                {
                    Id = (short)LanguageName.Chinese,
                    Name = "中文",
                    Culture = "zh-CHS"
                },
                new Language
                {
                    Id = (short)LanguageName.Japanese,
                    Name = "日本語",
                    Culture = "ja-JP"
                },
                new Language
                {
                    Id = (short)LanguageName.Portuguese,
                    Name = "Português",
                    Culture = "pt-BR"
                },
            };
        }
    }
}
