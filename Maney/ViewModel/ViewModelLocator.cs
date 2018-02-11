using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;

namespace Capuchinos.Maney.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<HistoryViewModel>();
            SimpleIoc.Default.Register<BalanceViewModel>();
            SimpleIoc.Default.Register<IncomeViewModel>();
            SimpleIoc.Default.Register<OutcomeViewModel>();
            SimpleIoc.Default.Register<CategoriesViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<InitialSettingsViewModel>();
            SimpleIoc.Default.Register<CategorySaveViewModel>();
            SimpleIoc.Default.Register<HelpViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public BalanceViewModel Balance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BalanceViewModel>();
            }
        }

        public HistoryViewModel History
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HistoryViewModel>();
            }
        }

        public IncomeViewModel Income
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IncomeViewModel>();
            }
        }

        public OutcomeViewModel Outcome
        {
            get
            {
                return ServiceLocator.Current.GetInstance<OutcomeViewModel>();
            }
        }

        public CategoriesViewModel Category
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CategoriesViewModel>();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public InitialSettingsViewModel InitialSettings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<InitialSettingsViewModel>();
            }
        }

        public CategorySaveViewModel CategorySave
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CategorySaveViewModel>();
            }
        }

        public HelpViewModel Help
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HelpViewModel>();
            }
        }

        public static void Cleanup()
        {

        }
    }
}