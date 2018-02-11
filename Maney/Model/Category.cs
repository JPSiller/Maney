using SQLite.Net.Attributes;
using System.Collections.Generic;
using Capuchinos.Maney.Resources;

namespace Capuchinos.Maney.Model
{
    [Table("Category")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static IEnumerable<Category> GetDefaultCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    Name = ManeyResources.Work,
                    Description = ManeyResources.Work
                },
                new Category
                {
                    Name = ManeyResources.Bills,
                    Description = ManeyResources.Bills
                }
            };
        }
    }
}
