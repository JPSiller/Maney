using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Capuchinos.Maney.Model;
using Capuchinos.Maney.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Plugin.Connectivity;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Xamarin.Forms;
// ReSharper disable RedundantCheckBeforeAssignment
// ReSharper disable ExplicitCallerInfoArgument

namespace Capuchinos.Maney.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public bool IsPurchased => !DataRepository.GetSettings().Purchased;
        public List<Language> Languages => DataRepository.GetLanguages().ToList();
        public List<Currency> Currencies => DataRepository.GetCurrencies().ToList();

        public RelayCommand AppearingCommand { get; }
        public RelayCommand DisappearingCommand { get; }
        public RelayCommand HelpCommand { get; }
        public RelayCommand PurchaseCommand { get; }
        public RelayCommand RestorePurchaseCommand { get; }

        public SettingsViewModel()
        {
            AppearingCommand = new RelayCommand(OnAppearing);
            DisappearingCommand = new RelayCommand(OnDisappearing);
            HelpCommand = new RelayCommand(Help);
            PurchaseCommand = new RelayCommand(Purchase);
            RestorePurchaseCommand = new RelayCommand(RestorePurchase);
            SetDefault();
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
            Messenger.Default.Register<NotificationMessage>(this, "RemoveAdvertisement", RemoveAdvertisement);
            Messenger.Default.Register<PropertyChangedMessage<Currency>>(
                this,
                (args) =>
                {
                    if (args.OldValue != null && args.OldValue.Name != args.NewValue.Name)
                    {
                        PromptCurrencyDialog();
                        SaveSettings();
                    }
                }
            );
            Messenger.Default.Register<PropertyChangedMessage<Language>>(
                this,
                (args) =>
                {
                    if (args.OldValue != null && args.OldValue.Name != args.NewValue.Name)
                    {
                        SaveSettings();
                        Messenger.Default.Send(new NotificationMessage(""), "CleanUpView");
                        Messenger.Default.Send(new NotificationMessage(""), "ReloadApp");
                    }
                }
            );
        }

        private async void RestorePurchase()
        {
            try
            {
                string productId = "maneypro";

                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        productId = "maneypro";
                        break;
                    case Device.iOS:
                        productId = "capuchinos.maney.ios.maneypro";
                        break;
                }

                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                {
                    UserDialogs.Instance.Alert(ManeyResources.ServiceUnavailable);
                    return;
                }

                IEnumerable<InAppBillingPurchase> purchases = null;
                try
                {
                    purchases = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.InAppPurchase);
                }
                catch (Exception)
                {
                    
                }
                if (purchases?.Any(p => p.ProductId == productId) ?? false)
                {
                    UserDialogs.Instance.Alert(ManeyResources.RestoreSuccess);
                    Settings settings = DataRepository.GetSettings();
                    settings.Purchased = true;
                    DataRepository.UpsertSettings(settings);
                    Messenger.Default.Send(new NotificationMessage(this, ""), "RemoveAdvertisement");
                }
                else
                {
                    UserDialogs.Instance.Alert(ManeyResources.NoPurchaseFound);
                }
            }
            catch (Exception)
            {
                UserDialogs.Instance.Alert(ManeyResources.PurchaseProcessFailed);
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

        private void RemoveAdvertisement(NotificationMessage message)
        {
            RaisePropertyChanged(nameof(IsPurchased));
        }

        private async void Purchase()
        {
            try
            {
                string productId = "maneypro";

                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        productId = "maneypro";
                        break;
                    case Device.iOS:
                        productId = "capuchinos.maney.ios.maneypro";
                        break;
                }

                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                {
                    UserDialogs.Instance.Alert(ManeyResources.ServiceUnavailable);
                    return;
                }

                var purchase = await CrossInAppBilling.Current.PurchaseAsync(productId, ItemType.InAppPurchase, "apppayload");
                if (purchase == null)
                {
                    UserDialogs.Instance.Alert(ManeyResources.PurchaseProcessFailed);
                }
                else
                {
                    Settings settings = DataRepository.GetSettings();
                    settings.Purchased = true;
                    DataRepository.UpsertSettings(settings);
                    Messenger.Default.Send(new NotificationMessage(this, ""), "RemoveAdvertisement");
                }
            }
            catch (Exception)
            {
                UserDialogs.Instance.Alert(ManeyResources.PurchaseProcessFailed);
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

        private void Help()
        {
            Messenger.Default.Send(new NotificationMessage(this, ""), "Help");
        }

        private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            IsCurrencyEnabled = e.IsConnected;
        }

        private void SetDefault()
        {
            Settings settings = DataRepository.GetSettings();
            if (settings != null)
            {
                if (settings.DefaultCurrency != null)
                    CurrencyIndex = Currencies.FindIndex(c => c.Name == settings.DefaultCurrency);
                if (settings.DefaultLanguage != null)
                    LanguageIndex = Languages.FindIndex(l => l.Name == settings.DefaultLanguage);
            }
            IsCurrencyEnabled = CrossConnectivity.Current.IsConnected;
        }

        private void PromptCurrencyDialog()
        {
            AlertConfig ac = new AlertConfig
            {
                Message = ManeyResources.ChangeCurrencyAlert,
                OkText = ManeyResources.Accept,
                OnAction = ChangeCurrency
            };
            UserDialogs.Instance.Alert(ac);
        }

        private async void ChangeCurrency()
        {
            UserDialogs.Instance.ShowLoading(ManeyResources.Processing);
            await DataRepository.ChangeBaseCurrency(DefaultCurrency);
            UserDialogs.Instance.HideLoading();
        }

        private void OnDisappearing()
        {
            SaveSettings();
        }

        private void OnAppearing()
        {
            SetDefault();
        }

        private void SaveSettings()
        {
            Settings settings = DataRepository.GetSettings();
            settings.DefaultLanguage = DefaultLanguage?.Name;
            settings.DefaultCurrency = DefaultCurrency?.Name;
            DataRepository.UpsertSettings(settings);
        }

        private bool _mIsCurrencyEnabled;
        public bool IsCurrencyEnabled
        {
            get => _mIsCurrencyEnabled;
            set
            {
                if (_mIsCurrencyEnabled != value)
                {
                    _mIsCurrencyEnabled = value;
                }
                RaisePropertyChanged();
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
                DefaultCurrency = Currencies[_mCurrencyIndex];
            }
        }

        private int _mlanguageIndex;
        public int LanguageIndex
        {
            get => _mlanguageIndex;
            set
            {
                if (_mlanguageIndex != value)
                {
                    _mlanguageIndex = value;
                }
                RaisePropertyChanged();
                DefaultLanguage = Languages[_mlanguageIndex];
            }
        }

        private Language _mDefaultLanguage;
        public Language DefaultLanguage
        {
            get => _mDefaultLanguage;
            set
            {
                var oldValue = _mDefaultLanguage;
                if (_mDefaultLanguage != value)
                {
                    _mDefaultLanguage = value;
                }
                RaisePropertyChanged(nameof(DefaultLanguage), oldValue, _mDefaultLanguage, true);
            }
        }

        private Currency _mDefaultCurrency;
        public Currency DefaultCurrency
        {
            get => _mDefaultCurrency;
            set
            {
                var oldValue = _mDefaultCurrency;
                if (_mDefaultCurrency != value)
                {
                    _mDefaultCurrency = value;
                }
                RaisePropertyChanged(nameof(DefaultCurrency), oldValue, _mDefaultCurrency, true);
            }
        }
    }
}
