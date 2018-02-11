using System;

namespace Capuchinos.Maney.Model
{
    public class ExchangeRate
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
    }
}
