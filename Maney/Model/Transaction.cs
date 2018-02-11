using System;
using SQLite.Net.Attributes;

namespace Capuchinos.Maney.Model
{
    [Table("Transaction")]
    public class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal RealValue { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public TimeSpan TimeOfTransaction { get; set; }
        public string Category { get; set; }
        public short TransactionType { get; set; }
        public string BaseCurrency { get; set; }
        public string SelectedCurrency { get; set; }
        public string ToCurrency { get; set; }
        public DateTime CurrencyRateDate { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
