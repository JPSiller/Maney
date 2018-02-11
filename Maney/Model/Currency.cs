using SQLite.Net.Attributes;
using System.Collections.Generic;

namespace Capuchinos.Maney.Model
{
    [Table("Currency")]
    public class Currency
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Symbol { get; set; }


        public static IEnumerable<Currency> GetCurrencies()
        {
            return new List<Currency>
            {
                new Currency
                {
                    Name = "USD",
                    Symbol = "$"
                }
                ,
                new Currency
                {
                    Name = "EUR",
                    Symbol = "€"
                },
                new Currency
                {
                    Name = "GBP",
                    Symbol = "£"
                },
                new Currency
                {
                    Name = "MXN",
                    Symbol = "$"
                },
                new Currency
                {
                    Name = "JPY",
                    Symbol = "¥"
                },
                new Currency
                {

                    Name = "AUD",
                    Symbol = "A$"
                },
                new Currency
                {
                    Name = "CAD",
                    Symbol = "C$"
                },
                new Currency
                {
                    Name = "CHF",
                    Symbol = "Fr"
                },
                new Currency
                {
                    Name = "CNY",
                    Symbol = "元"
                },
                new Currency
                {
                    Name = "SEK",
                    Symbol = "kr"
                },
                new Currency
                {
                    Name = "NZD",
                    Symbol = "NZ$"
                },
                new Currency
                {
                    Name = "SGD	",
                    Symbol = "S$"
                },
                new Currency
                {
                    Name = "HKD",
                    Symbol = "HK$"
                },
                new Currency
                {
                    Name = "NOK",
                    Symbol = "kr"
                },
                new Currency
                {
                    Name = "KRW",
                    Symbol = "₩"
                },
                new Currency
                {
                    Name = "TRY",
                    Symbol = "₺"
                },
                new Currency
                {
                    Name = "RUB",
                    Symbol = "₽"
                },
                new Currency
                {
                    Name = "INR",
                    Symbol = "₹"
                },
                new Currency
                {
                    Name = "BRL",
                    Symbol = "R$"
                },
                new Currency
                {
                    Name = "ZAR",
                    Symbol = "R"
                }
            };
        }
    }
}
