using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Capuchinos.Maney.Dependencies;
using Capuchinos.Maney.Helpers;
using ModernHttpClient;
using Newtonsoft.Json.Linq;
using SQLite.Net;
using Xamarin.Forms;

namespace Capuchinos.Maney.Model
{
    public static class DataRepository
    {
        private static readonly SQLiteConnection Connection;
        private static readonly HttpClient Client = new HttpClient(new NativeMessageHandler());

        public static void Init() { }

        static DataRepository()
        {
            Connection = DependencyService.Get<IDatabase>().GetConnection();
            Connection.CreateTable<Category>();
            Connection.CreateTable<Currency>();
            Connection.CreateTable<Language>();
            Connection.CreateTable<Settings>();
            Connection.CreateTable<Transaction>();
            if (!Connection.Table<Currency>().Any())
            {
                InsertCurrencies();
            }
            if (!Connection.Table<Language>().Any())
            {
                InsertLanguages();
            }
        }

        #region Category
        public static void UpsertCategory(Category category)
        {
            if (category.Id != 0)
            {
                Connection.Update(category);
            }
            else
            {
                Connection.Insert(category);
            }
        }

        public static void DeleteCategory(string name)
        {
            Connection.Delete<Category>(GetCategoryId(name));
        }

        public static IEnumerable<Category> GetCategories()
        {
            return Connection.Table<Category>();
        }

        public static int GetCategoryId(string name)
        {
            return Connection
                .Table<Category>().FirstOrDefault(c => c.Name == name)?.Id ?? 0;
        }

        public static void InsertDefaultCategories()
        {
            if (Connection.Table<Category>().Any())
                return;

            foreach (var category in Category.GetDefaultCategories())
            {
                Connection.Insert(category);
            }
        }
        #endregion Category

        #region Currency
        private static void InsertCurrencies()
        {
            foreach (var currency in Currency.GetCurrencies())
            {
                Connection.Insert(currency);
            }
        }
        public static IEnumerable<Currency> GetCurrencies()
        {
            return Connection.Table<Currency>();
        }
        public static Currency GetCurrency(string name)
        {
            return Connection
                .Table<Currency>().FirstOrDefault(c => c.Name == name);
        }
        public static string GetCurrencySymbol(string currencyName)
        {
            return Connection.Table<Currency>().FirstOrDefault(c => c.Name == currencyName)?.Symbol;
        }
        public static async Task<bool> ChangeBaseCurrency(Currency currency)
        {
            List<Transaction> transactionsToPersist = new List<Transaction>();
            foreach (Transaction t in Connection.Table<Transaction>().ToList())
            {
                ExchangeRate rate = await GetExchangeRate(currency.Name, t.SelectedCurrency, t.CurrencyRateDate);
                if (rate != null)
                {
                    string oldCurrency = t.BaseCurrency;
                    t.BaseCurrency = currency.Name;
                    t.ToCurrency = rate.ToCurrency;
                    t.ExchangeRate = rate.Rate;

                    if (rate.Rate == 1)
                        t.RealValue = t.Quantity;
                    else if (oldCurrency.Equals(rate.ToCurrency))
                        t.RealValue = t.RealValue / rate.Rate;
                    else
                        t.RealValue = t.Quantity / rate.Rate;

                    transactionsToPersist.Add(t);
                }
            }

            foreach (Transaction t in transactionsToPersist)
            {
                Connection.Update(t);
            }

            return true;
        }
        public static async Task<ExchangeRate> GetExchangeRate(string from, string to, DateTime dateOfCurrency)
        {
            if (from.Equals(to))
            {
                return new ExchangeRate
                {
                    FromCurrency = from,
                    ToCurrency = to,
                    Date = dateOfCurrency,
                    Rate = 1
                };
            }

            ExchangeRate rate = null;
            string exchangeDate = dateOfCurrency.ToString("yyyy-MM-dd");

            HttpResponseMessage result = await Client.GetAsync(new Uri($"http://api.fixer.io/{exchangeDate}?base={from}&symbols={from},{to}"));
            if (result.IsSuccessStatusCode)
            {
                string json = await result.Content.ReadAsStringAsync();
                var jobject = JObject.Parse(json);

                DateTime date = (DateTime)jobject.GetValue("date");
                decimal exchangeRate = jobject.GetValue("rates").SelectToken(to).Value<decimal>();

                rate = new ExchangeRate
                {
                    FromCurrency = from,
                    ToCurrency = to,
                    Date = date,
                    Rate = exchangeRate
                };
            }

            return rate;
        }
        #endregion Currency

        #region Language
        private static void InsertLanguages()
        {
            foreach (var language in Language.GetLanguages())
            {
                Connection.Insert(language);
            }
        }
        public static Language GetLanguage(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return Connection
                .Table<Language>().FirstOrDefault(l => l.Name == name);
        }
        public static IEnumerable<Language> GetLanguages()
        {
            return Connection.Table<Language>();
        }
        #endregion Language

        #region Transaction
        public static void UpsertTransaction(Transaction t)
        {
            if (t.Id > 0)
            {
                Connection.Update(t);
            }
            else
            {
                Connection.Insert(t);
            }
        }

        public static void EditTransaction(Transaction t)
        {
            Connection.Update(t);
        }

        public static void DeleteTransaction(int id)
        {
            Connection.Delete<Transaction>(id);
        }

        public static decimal GetBalance(DateTime date, SortByType sortByType = SortByType.All)
        {
            return GetTotalIncome(date, sortByType) - GetTotalOutcome(date, sortByType);
        }

        public static IEnumerable<Transaction> GetTransactions()
        {
            return Connection.Table<Transaction>();
        }

        public static IEnumerable<Transaction> GetIncomeTransactions()
        {
            return Connection.Table<Transaction>()
                .Where(t => t.TransactionType == (short)TransactionType.Income);
        }

        public static IEnumerable<Transaction> GetOutcomeTransactions()
        {
            return Connection.Table<Transaction>()
                .Where(t => t.TransactionType == (short)TransactionType.Outcome);
        }

        public static List<Transaction> GetTransactionWithCategoryName(string categoryName)
        {
            if (!Connection.Table<Transaction>().Any())
                return new List<Transaction>();

            List<Transaction> result = Connection.Table<Transaction>().Where(t => t.Category == categoryName)?.ToList();

            return result;
        }

        public static decimal GetTotalIncome(DateTime date, SortByType sortByType = SortByType.All)
        {
            decimal result = 0;
            switch (sortByType)
            {
                case SortByType.All:
                    result = Connection
                        .Table<Transaction>().ToList().
                        Where(t => t.TransactionType == (short)TransactionType.Income).
                        Sum(t => t.RealValue);
                    break;
                case SortByType.Day:
                    result = Connection
                        .Table<Transaction>().ToList().
                        Where(t => t.TransactionType == (short)TransactionType.Income &&
                                   t.DateOfTransaction.ToLocalTime().Year == date.Year &&
                                   t.DateOfTransaction.ToLocalTime().Month == date.Month &&
                                   t.DateOfTransaction.ToLocalTime().Day == date.Day).
                        Sum(t => t.RealValue);
                    break;
                case SortByType.Month:
                    result = Connection
                        .Table<Transaction>().ToList().
                        Where(t => t.TransactionType == (short)TransactionType.Income &&
                                   t.DateOfTransaction.ToLocalTime().Year == date.Year &&
                                   t.DateOfTransaction.ToLocalTime().Month == date.Month).
                        Sum(t => t.RealValue);
                    break;
                case SortByType.Year:
                    result = Connection
                        .Table<Transaction>().ToList().
                        Where(t => t.TransactionType == (short)TransactionType.Income &&
                                   t.DateOfTransaction.ToLocalTime().Year == date.Year).
                        Sum(t => t.RealValue);
                    break;
            }

            return result;
        }

        public static decimal GetTotalOutcome(DateTime date, SortByType sortByType = SortByType.All)
        {
            decimal result = 0;
            switch (sortByType)
            {
                case SortByType.All:
                    result = Connection
                        .Table<Transaction>().ToList().
                        Where(t => t.TransactionType == (short)TransactionType.Outcome).
                        Sum(t => t.RealValue);
                    break;
                case SortByType.Day:
                    result = Connection
                        .Table<Transaction>().ToList().
                        Where(t => t.TransactionType == (short)TransactionType.Outcome &&
                                   t.DateOfTransaction.ToLocalTime().Year == date.Year &&
                                   t.DateOfTransaction.ToLocalTime().Month == date.Month &&
                                   t.DateOfTransaction.ToLocalTime().Day == date.Day).
                        Sum(t => t.RealValue);
                    break;
                case SortByType.Month:
                    result = Connection
                        .Table<Transaction>().ToList().
                        Where(t => t.TransactionType == (short)TransactionType.Outcome &&
                                   t.DateOfTransaction.ToLocalTime().Month == date.Month &&
                                   t.DateOfTransaction.ToLocalTime().Month == date.Month).
                        Sum(t => t.RealValue);
                    break;
                case SortByType.Year:
                    result = Connection
                        .Table<Transaction>().ToList().
                        Where(t => t.TransactionType == (short)TransactionType.Outcome &&
                                   t.DateOfTransaction.ToLocalTime().Year == date.Year).
                        Sum(t => t.RealValue);
                    break;
            }

            return result;
        }

        public static List<decimal> GetTotalIncomePerMonth()
        {
            List<decimal> result = new List<decimal>
            {
                0,0,0,0,0,0,0,0,0,0,0,0
            };
            var months = Connection
                .Table<Transaction>().ToList().Where(t => t.TransactionType == (short)TransactionType.Income)
                .GroupBy(t => new { t.DateOfTransaction.ToLocalTime().Year, t.DateOfTransaction.ToLocalTime().Month })
                .Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) });
            foreach (var pair in
                months)
            {
                result[pair.Name.Month - 1] = pair.Total;
            }

            return result;
        }
        public static List<decimal> GetTotalOutcomePerMonth()
        {
            List<decimal> result = new List<decimal>
            {
                0,0,0,0,0,0,0,0,0,0,0,0
            };
            var months = Connection
                .Table<Transaction>().ToList().Where(t => t.TransactionType == (short)TransactionType.Outcome)
                .GroupBy(t => new { t.DateOfTransaction.ToLocalTime().Year, t.DateOfTransaction.ToLocalTime().Month })
                .Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) });
            foreach (var pair in
                months)
            {
                result[pair.Name.Month - 1] = pair.Total;
            }

            return result;
        }

        public static Dictionary<string, decimal> GetTotalPerCategory(DateTime date, SortByType sortByType = SortByType.All)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();

            switch (sortByType)
            {
                case SortByType.All:
                    foreach (var pair in Connection.Table<Transaction>().ToList().
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
                case SortByType.Day:
                    foreach (var pair in Connection.Table<Transaction>().ToList().
                        Where(
                            t => t.DateOfTransaction.ToLocalTime().Year == date.Year &&
                                 t.DateOfTransaction.ToLocalTime().Month == date.Month &&
                                 t.DateOfTransaction.ToLocalTime().Day == date.Day).
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
                case SortByType.Month:
                    foreach (var pair in Connection.Table<Transaction>().ToList().
                        Where(
                            t => t.DateOfTransaction.ToLocalTime().Year == date.Year &&
                                 t.DateOfTransaction.ToLocalTime().Month == date.Month).
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
                case SortByType.Year:
                    foreach (var pair in Connection.Table<Transaction>().ToList().
                        Where(t => t.DateOfTransaction.ToLocalTime().Year == date.Year).
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
            }

            return result;
        }
        public static Dictionary<string, decimal> GetTotalIncomePerCategory(DateTime date, SortByType sortByType = SortByType.All)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();

            switch (sortByType)
            {
                case SortByType.All:
                    foreach (var pair in Connection.Table<Transaction>().
                        Where(t => t.TransactionType == (short)TransactionType.Income).
                        ToList().
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
                case SortByType.Day:
                    foreach (var pair in Connection.Table<Transaction>().
                        Where(t => t.TransactionType == (short)TransactionType.Income).
                        ToList().
                        Where(
                            t => t.DateOfTransaction.ToLocalTime().Year == date.Year &&
                                 t.DateOfTransaction.ToLocalTime().Month == date.Month &&
                                 t.DateOfTransaction.ToLocalTime().Day == date.Day).
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
                case SortByType.Month:
                    foreach (var pair in Connection.Table<Transaction>().
                        Where(t => t.TransactionType == (short)TransactionType.Income).
                        ToList().
                        Where(
                            t => t.DateOfTransaction.ToLocalTime().Year == date.Year &&
                                 t.DateOfTransaction.ToLocalTime().Month == date.Month).
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
                case SortByType.Year:
                    foreach (var pair in Connection.Table<Transaction>().
                        Where(t => t.TransactionType == (short)TransactionType.Income).
                        ToList().
                        Where(t => t.DateOfTransaction.ToLocalTime().Year == date.Year).
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
            }

            return result;
        }
        public static Dictionary<string, decimal> GetTotalOutcomePerCategory(DateTime date, SortByType sortByType = SortByType.All)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();

            switch (sortByType)
            {
                case SortByType.All:
                    foreach (var pair in Connection.Table<Transaction>().
                        Where(t => t.TransactionType == (short)TransactionType.Outcome).
                        ToList().
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
                case SortByType.Day:
                    foreach (var pair in Connection.Table<Transaction>().
                        Where(t => t.TransactionType == (short)TransactionType.Outcome).
                        ToList().
                        Where(
                            t => t.DateOfTransaction.ToLocalTime().Year == date.Year &&
                                 t.DateOfTransaction.ToLocalTime().Month == date.Month &&
                                 t.DateOfTransaction.ToLocalTime().Day == date.Day).
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
                case SortByType.Month:
                    foreach (var pair in Connection.Table<Transaction>().
                        Where(t => t.TransactionType == (short)TransactionType.Outcome).
                        ToList().
                        Where(
                            t => t.DateOfTransaction.ToLocalTime().Year == date.Year &&
                                 t.DateOfTransaction.ToLocalTime().Month == date.Month).
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
                case SortByType.Year:
                    foreach (var pair in Connection.Table<Transaction>().
                        Where(t => t.TransactionType == (short)TransactionType.Outcome).
                        ToList().
                        Where(t => t.DateOfTransaction.ToLocalTime().Year == date.Year).
                        GroupBy(t => t.Category).
                        Select(a => new { Name = a.Key, Total = a.Sum(b => b.RealValue) }))
                    {
                        result.Add(pair.Name, pair.Total);
                    }
                    break;
            }

            return result;
        }
        #endregion Transaction

        #region Settings
        public static void UpsertSettings(Settings settings)
        {
            if (Connection.Table<Settings>().Any())
            {
                settings.Id = 1;
                Connection.Update(settings);
            }
            else
            {
                settings.Id = 1;
                Connection.Insert(settings);
            }
        }

        public static Settings GetSettings()
        {
            return Connection.Table<Settings>().FirstOrDefault();
        }
        #endregion Settings
    }
}
