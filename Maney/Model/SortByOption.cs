using System.Collections.Generic;
using Capuchinos.Maney.Helpers;
using Capuchinos.Maney.Resources;

namespace Capuchinos.Maney.Model
{
    public class SortByOption
    {
        public short SortType { get; set; }
        public string Name { get; set; }

        public static List<SortByOption> GetHistoryOptions()
        {
            return new List<SortByOption>
            {
                new SortByOption
                {
                    SortType = (short)SortByType.Recent,
                    Name = ManeyResources.Recent
                },
                new SortByOption
                {
                    SortType = (short)SortByType.Oldest,
                    Name = ManeyResources.Oldest
                },
                new SortByOption
                {
                    SortType = (short)SortByType.Largest,
                    Name = ManeyResources.Highest
                },
                new SortByOption
                {
                    SortType = (short)SortByType.Lowest,
                    Name = ManeyResources.Lowest
                },
                new SortByOption
                {
                    SortType = (short)SortByType.Category,
                    Name = ManeyResources.CategoryOption
                }
            };
        }
        public static List<SortByOption> GetBalanceOptions()
        {
            return new List<SortByOption>
            {
                new SortByOption
                {
                    SortType = (short)SortByType.All,
                    Name = ManeyResources.All
                },
                new SortByOption
                {
                    SortType = (short)SortByType.Day,
                    Name = ManeyResources.Day
                },
                new SortByOption
                {
                    SortType = (short)SortByType.Month,
                    Name = ManeyResources.Month
                },
                new SortByOption
                {
                    SortType = (short)SortByType.Year,
                    Name = ManeyResources.Year
                }
            };
        }
    }
}
