using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Capuchinos.Maney.Dependencies;
using Capuchinos.Maney.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.Forms;
// ReSharper disable RedundantCheckBeforeAssignment

namespace Capuchinos.Maney.ViewModel
{
    public class InitialSettingsViewModel : ViewModelBase
    {
        public RelayCommand AppearingCommand { get; }
        public RelayCommand StartCommand { get; }
        public List<Language> Languages => DataRepository.GetLanguages().ToList();
        public List<Currency> Currencies => DataRepository.GetCurrencies().ToList();
        private Language _mDefaultLanguage;

        public InitialSettingsViewModel()
        {
            AppearingCommand = new RelayCommand(OnAppearing);
            StartCommand = new RelayCommand(Start);
        }

        private void Start()
        {
            if (DefaultLanguage != null && DefaultCurrency != null)
            {
                var settings = new Settings
                {
                    Id = 1,
                    DefaultLanguage = DefaultLanguage.Name,
                    DefaultCurrency = DefaultCurrency.Name,
                    Purchased = false
                };
                DependencyService.Get<ILocalization>().SetLocale(new CultureInfo(DefaultLanguage.Culture));
                DataRepository.UpsertSettings(settings);
                DataRepository.InsertDefaultCategories();
                Messenger.Default.Send(new NotificationMessage(this, ""), "Start");
            }
        }

        private void OnAppearing()
        {
            if (DataRepository.GetSettings() != null)
            {
                Messenger.Default.Send(new NotificationMessage(this, ""), "Start");
            }
        }

        public Language DefaultLanguage
        {
            get => _mDefaultLanguage;
            set
            {
                if (_mDefaultLanguage != value)
                {
                    _mDefaultLanguage = value;
                }
                RaisePropertyChanged();
            }
        }

        private Currency _mDefaultCurrency;
        public Currency DefaultCurrency
        {
            get => _mDefaultCurrency;
            set
            {
                if (_mDefaultCurrency != value)
                {
                    _mDefaultCurrency = value;
                }
                RaisePropertyChanged();
            }
        }
    }
}
