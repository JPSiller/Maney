using System;
using System.Collections.Generic;
using System.Linq;
using Capuchinos.Maney.Helpers;
using Capuchinos.Maney.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
// ReSharper disable RedundantCheckBeforeAssignment
// ReSharper disable ExplicitCallerInfoArgument

namespace Capuchinos.Maney.ViewModel
{
    public class HistoryViewModel : ViewModelBase
    {
        public bool IsPurchased => !DataRepository.GetSettings().Purchased;
        public RelayCommand AppearingCommand { get; }
        public RelayCommand SortByCommand { get; }
        public RelayCommand<Transaction> DeleteCommand { get; }
        public RelayCommand<Transaction> EditCommand { get; }
        public List<SortByOption> SortByOptions => SortByOption.GetHistoryOptions();

        public HistoryViewModel()
        {
            SortByCommand = new RelayCommand(Sort);
            AppearingCommand = new RelayCommand(OnAppearing);
            DeleteCommand = new RelayCommand<Transaction>(Delete);
            EditCommand = new RelayCommand<Transaction>(Edit);
            Messenger.Default.Register<NotificationMessage>(this, "RemoveAdvertisement", RemoveAdvertisement);
        }

        private void RemoveAdvertisement(NotificationMessage message)
        {
            RaisePropertyChanged(nameof(IsPurchased));
        }

        private void OnAppearing()
        {
            SortByIndex = SortByOptions.FindIndex(s => s.SortType == (short)SortByType.Recent);
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            Transactions = DataRepository.GetTransactions().ToList();
            Sort();
        }

        private void Delete(Transaction t)
        {
            DataRepository.DeleteTransaction(t.Id);
            LoadTransactions();
        }

        private void Edit(Transaction t)
        {
            if (t.TransactionType == (short)TransactionType.Income)
            {
                Messenger.Default.Send(new NotificationMessage<short>(this, (short)PageIndex.Income, ""), "NavigateToTab");
                Messenger.Default.Send(new NotificationMessage<Transaction>(this, t, ""), "EditIncome");
                Messenger.Default.Send(new NotificationMessage<Transaction>(this, t, ""), "EditIncomePageBehind");
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<short>(this, (short)PageIndex.Outcome, ""), "NavigateToTab");
                Messenger.Default.Send(new NotificationMessage<Transaction>(this, t, ""), "EditOutcome");
                Messenger.Default.Send(new NotificationMessage<Transaction>(this, t, ""), "EditOutcomePageBehind");
            }
        }

        private void Sort()
        {
            switch (SortBy.SortType)
            {
                case (short)SortByType.Recent:
                    Transactions = Transactions.OrderByDescending(t => t.TimeOfTransaction).
                        ThenByDescending(t => t.DateOfTransaction.ToLocalTime())
                        .ToList();
                    break;
                case (short)SortByType.Oldest:
                    Transactions = Transactions.OrderBy(t => t.TimeOfTransaction).
                        ThenBy(t => t.DateOfTransaction.ToLocalTime())
                        .ToList();
                    break;
                case (short)SortByType.Largest:
                    Transactions = Transactions.OrderByDescending(t => t.RealValue).ToList();
                    break;
                case (short)SortByType.Lowest:
                    Transactions = Transactions.OrderBy(t => t.RealValue).ToList();
                    break;
                case (short)SortByType.Category:
                    Transactions = Transactions.OrderBy(t => t.Category).ToList();
                    break;
            }
        }

        private int _mSortByIndex;
        public int SortByIndex
        {
            get => _mSortByIndex;
            set
            {
                if (_mSortByIndex != value)
                {
                    _mSortByIndex = value;
                }
                RaisePropertyChanged();
                SortBy = SortByOptions[_mSortByIndex];
                LoadTransactions();
            }
        }

        private SortByOption _mSortBy;
        public SortByOption SortBy
        {
            get => _mSortBy;
            set
            {
                if (_mSortBy != value)
                {
                    _mSortBy = value;
                }
                RaisePropertyChanged();
            }
        }
        private List<Transaction> _mTransactions;
        public List<Transaction> Transactions
        {
            get => _mTransactions;
            set
            {
                if (_mTransactions != value)
                {
                    _mTransactions = value;
                }
                RaisePropertyChanged();
            }
        }
    }
}
