using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Capuchinos.Maney.Model;
using Capuchinos.Maney.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
// ReSharper disable ExplicitCallerInfoArgument

namespace Capuchinos.Maney.ViewModel
{
    public class CategoriesViewModel : ViewModelBase
    {
        public bool IsPurchased => !DataRepository.GetSettings().Purchased;
        public List<Category> Categories => DataRepository.GetCategories().ToList();
        public RelayCommand<string> DeleteCategoryCommand { get; }
        public RelayCommand<string> SaveCategoryCommand { get; }

        public CategoriesViewModel()
        {
            DeleteCategoryCommand = new RelayCommand<string>(DeleteCategory);
            SaveCategoryCommand = new RelayCommand<string>(SaveCategory);
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

        private void DeleteCategory(string categoryName)
        {
            if (DataRepository.GetCategories().Count() > 1)
            {
                DataRepository.DeleteCategory(categoryName);
                Messenger.Default.Send(new NotificationMessage(this, ""), "ReloadCategories");
            }
            else
            {
                UserDialogs.Instance.Alert(ManeyResources.MustHaveAtLeastOneCategory, ManeyResources.Category,
                    ManeyResources.Accept);
            }
        }

        private void SaveCategory(string categoryName)
        {
            Messenger.Default.Send(new NotificationMessage(this, ""), "CategorySave");
            Messenger.Default.Send(new NotificationMessage<string>(this, categoryName, ""), "CategorySaveData");
        }
    }
}
