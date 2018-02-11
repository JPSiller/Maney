using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Capuchinos.Maney.Helpers;
using Capuchinos.Maney.Model;
using Capuchinos.Maney.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Plugin.Connectivity;
// ReSharper disable RedundantCheckBeforeAssignment
// ReSharper disable ExplicitCallerInfoArgument

namespace Capuchinos.Maney.ViewModel
{
    public class OutcomeViewModel : ViewModelBase
    {
        private int _mEditId;
        public bool IsPurchased => !DataRepository.GetSettings().Purchased;
        public List<Category> Categories => DataRepository.GetCategories().ToList();
        public List<Currency> Currencies => DataRepository.GetCurrencies().ToList();
        public RelayCommand AppearingCommand { get; }
        public RelayCommand DisappearingCommand { get; }
        public RelayCommand InsertCommand { get; }

        public OutcomeViewModel()
        {
            AppearingCommand = new RelayCommand(OnAppearing);
            DisappearingCommand = new RelayCommand(OnDisappearing);
            InsertCommand = new RelayCommand(Save);
            IsNameValid = true;
            IsQuantityValid = true;
            Messenger.Default.Register<NotificationMessage<Transaction>>(this, "EditOutcome", EditOutcome);
            Messenger.Default.Register<NotificationMessage>(this, "ReloadCategories", ReloadCategories);
            Messenger.Default.Register<NotificationMessage>(this, "RemoveAdvertisement", RemoveAdvertisement);
        }

        private void RemoveAdvertisement(NotificationMessage message)
        {
            RaisePropertyChanged(nameof(IsPurchased));
        }

        private void ReloadCategories(NotificationMessage message)
        {
            RaisePropertyChanged(nameof(Categories));
        }

        private void EditOutcome(NotificationMessage<Transaction> transaction)
        {
            Transaction t = transaction.Content;
            _mEditId = t.Id;
            Name = t.Name;
            Quantity = t.Quantity;
            CategoryIndex = Categories.FindIndex(c => c.Name == t.Category);
            CurrencyIndex = Currencies.FindIndex(c => c.Name == t.SelectedCurrency);
            Date = t.DateOfTransaction;
            Time = t.TimeOfTransaction;
        }

        private void OnDisappearing()
        {
            CleanUp();
        }

        public void CleanUp()
        {
            Name = string.Empty;
            Quantity = 0;
            _mEditId = 0;
        }

        private bool Validate()
        {
            bool result = true;
            if (string.IsNullOrWhiteSpace(Name))
            {
                IsNameValid = false;
                result = false;
            }
            if (Quantity == 0)
            {
                IsQuantityValid = false;
                result = false;
            }

            return result;
        }

        private async void Save()
        {
            if (!Validate())
                return;

            var settings = DataRepository.GetSettings();
            ExchangeRate exchangeRate;
            if (Currency.Name.Equals(settings.DefaultCurrency))
            {
                exchangeRate = new ExchangeRate
                {
                    FromCurrency = Currency.Name,
                    ToCurrency = settings.DefaultCurrency,
                    Date = Date.ToUniversalTime(),
                    Rate = 1
                };
            }
            else
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    exchangeRate = await DataRepository.GetExchangeRate(settings.DefaultCurrency, Currency.Name, Date.ToUniversalTime());
                }
                else
                {
                    AlertConfig ac = new AlertConfig
                    {
                        Message = ManeyResources.NeedActiveConnection,
                        OkText = ManeyResources.Accept
                    };
                    UserDialogs.Instance.Alert(ac);
                    return;
                }
            }
            var t = new Transaction
            {
                Id = _mEditId,
                Name = Name,
                Quantity = Quantity,
                Category = Category?.Name,
                BaseCurrency = settings.DefaultCurrency,
                SelectedCurrency = Currency?.Name,
                DateOfTransaction = Date,
                TimeOfTransaction = Time,
                TransactionType = (short)TransactionType.Outcome,
                ToCurrency = Currency?.Name,
                CurrencyRateDate = exchangeRate.Date,
                ExchangeRate = exchangeRate.Rate,
                RealValue = Quantity / exchangeRate.Rate
            };

            DataRepository.UpsertTransaction(t);
            _mEditId = 0;
            Messenger.Default.Send(new NotificationMessage<short>(this, (short)PageIndex.History, ""), "NavigateToTab");
        }

        private void OnAppearing()
        {
            if (_mEditId <= 0)
            {
                Date = DateTime.UtcNow.ToLocalTime();
                Time = Date.TimeOfDay;
                IsQuantityValid = true;
                IsNameValid = true;
                Settings settings = DataRepository.GetSettings();
                if (settings != null)
                {
                    CurrencyIndex = Currencies.FindIndex(c => c.Name == settings.DefaultCurrency);
                }
                CategoryIndex = 0;
            }
        }

        private DateTime _mDate;
        public DateTime Date
        {
            get => _mDate;
            set
            {
                if (_mDate != value)
                {
                    _mDate = value;
                }
                RaisePropertyChanged();
            }
        }

        private TimeSpan _mTime;
        public TimeSpan Time
        {
            get => _mTime;
            set
            {
                if (_mTime != value)
                {
                    _mTime = value;
                }
                RaisePropertyChanged();
            }
        }

        private bool _mIsQuantityValid;
        public bool IsQuantityValid
        {
            get => _mIsQuantityValid;
            set
            {
                if (_mIsQuantityValid != value)
                {
                    _mIsQuantityValid = value;
                }
                RaisePropertyChanged();
            }
        }

        private bool _mIsNameValid;
        public bool IsNameValid
        {
            get => _mIsNameValid;
            set
            {
                if (_mIsNameValid != value)
                {
                    _mIsNameValid = value;
                }
                RaisePropertyChanged();
            }
        }

        private string _mName;
        public string Name
        {
            get => _mName;
            set
            {
                if (_mName != value)
                {
                    _mName = value;
                }
                RaisePropertyChanged();
            }
        }
        private decimal _mQuantity;
        public decimal Quantity
        {
            get => _mQuantity;
            set
            {
                if (_mQuantity != value)
                {
                    _mQuantity = value;
                }
                RaisePropertyChanged();
            }
        }
        private Category _mCategory;
        public Category Category
        {
            get => _mCategory;
            set
            {
                if (_mCategory != value)
                {
                    _mCategory = value;
                }
                RaisePropertyChanged();
            }
        }
        private int _mCategoryIndex;
        public int CategoryIndex
        {
            get => _mCategoryIndex;
            set
            {
                if (_mCategoryIndex != value)
                {
                    if (value < 0)
                        value = 0;

                    _mCategoryIndex = value;
                }
                RaisePropertyChanged();
                Category = Categories[_mCategoryIndex];
            }
        }
        private int _mCurrencyIndex;
        public int CurrencyIndex
        {
            get => _mCurrencyIndex;
            set
            {
                if (_mCurrencyIndex != value)
                {
                    _mCurrencyIndex = value;
                }
                RaisePropertyChanged();
                Currency = Currencies[_mCurrencyIndex];
            }
        }
        private Currency _mCurrency;
        public Currency Currency
        {
            get => _mCurrency;
            set
            {
                if (_mCurrency != value)
                {
                    _mCurrency = value;
                }
                RaisePropertyChanged();
            }
        }
    }
}
