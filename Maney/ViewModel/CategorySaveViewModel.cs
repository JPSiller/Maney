using Capuchinos.Maney.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Rg.Plugins.Popup.Services;
// ReSharper disable RedundantCheckBeforeAssignment

namespace Capuchinos.Maney.ViewModel
{
    public class CategorySaveViewModel : ViewModelBase
    {
        public string OldName { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public CategorySaveViewModel()
        {
            Messenger.Default.Register<NotificationMessage<string>>(this, "CategorySaveData", LoadData);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private async void Cancel()
        {
            await PopupNavigation.PopAllAsync();
        }

        private void LoadData(NotificationMessage<string> oldCategory)
        {
            OldName = oldCategory.Content;
            Name = oldCategory.Content;
        }

        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await PopupNavigation.PopAllAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(OldName))
                {
                    int id = DataRepository.GetCategoryId(Name);
                    DataRepository.UpsertCategory(new Category
                    {
                        Id = id,
                        Name = Name,
                        Description = Name
                    });
                }
                else
                {
                    int id = DataRepository.GetCategoryId(OldName);
                    DataRepository.UpsertCategory(new Category
                    {
                        Id = id,
                        Name = Name,
                        Description = Name
                    });
                    foreach (var t in DataRepository.GetTransactionWithCategoryName(OldName))
                    {
                        t.Category = Name;
                        DataRepository.UpsertTransaction(t);
                    }
                }
                Messenger.Default.Send(new NotificationMessage(this, ""), "ReloadCategories");
                await PopupNavigation.PopAllAsync();
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
    }
}
