using System;
using System.Collections.Generic;
using System.Linq;
using Capuchinos.Maney.Helpers;
using Capuchinos.Maney.Model;
using Capuchinos.Maney.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
// ReSharper disable RedundantCheckBeforeAssignment
// ReSharper disable ExplicitCallerInfoArgument

namespace Capuchinos.Maney.ViewModel
{
    public class BalanceViewModel : ViewModelBase
    {
        public bool IsPurchased => !DataRepository.GetSettings().Purchased;
        public List<SortByOption> SortByOptions => SortByOption.GetBalanceOptions();
        public RelayCommand AppearingCommand { get; }

        public BalanceViewModel()
        {
            AppearingCommand = new RelayCommand(OnAppearing);
            PropertyChanged += BalanceViewModel_PropertyChanged;
            Messenger.Default.Register<NotificationMessage>(this, "RemoveAdvertisement", RemoveAdvertisement);
        }

        private void RemoveAdvertisement(NotificationMessage message)
        {
            RaisePropertyChanged(nameof(IsPurchased));
        }

        private void BalanceViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Date):
                    if (SortBy != null && SortBy.SortType != (short)SortByType.All)
                        LoadBalance();
                    break;
                case nameof(SortByIndex):
                    SortBy = SortByOptions[_mSortByIndex];
                    break;
                case nameof(SortBy):
                    IsDateEnabled = SortBy.SortType != (short)SortByType.All;
                    LoadBalance();
                    break;
            }
        }

        private void OnAppearing()
        {
            SortByIndex = SortByOptions.FindIndex(s => s.SortType == (short)SortByType.Day);
            Date = DateTime.Today;
            LoadBalance();
        }

        private void LoadBalance()
        {
            Income = DataRepository.GetTotalIncome(Date, (SortByType)SortBy.SortType);
            Outcome = DataRepository.GetTotalOutcome(Date, (SortByType)SortBy.SortType);
            Balance = DataRepository.GetBalance(Date, (SortByType)SortBy.SortType);
            BuildCategoryPieChart();
            BuildCategoryIncomePieChart();
            BuildCategoryOutcomePieChart();
            BuildIncomeOutcomeChart();
        }

        private void BuildCategoryPieChart()
        {
            Dictionary<string, decimal> categoryValuePair = DataRepository.GetTotalPerCategory(Date, (SortByType)SortBy.SortType);

            var model = new PlotModel();
            var seriesP1 = new PieSeries
            {
                InsideLabelColor = OxyColors.White,
                OutsideLabelFormat = "",
                InsideLabelFormat = "{1} {2:0}%",
                TickHorizontalLength = 0.00,
                TickRadialLength = 0.00
            };

            foreach (var pair in categoryValuePair)
            {
                var slice = new PieSlice(pair.Key, (double)pair.Value)
                {
                    IsExploded = true,
                    Fill = OxyColor.Parse("#3AA7F9")
                };
                seriesP1.Slices.Add(slice);
            }
            model.Series.Add(seriesP1);
            CategoryPieChart = model;
        }

        private void BuildCategoryIncomePieChart()
        {
            Dictionary<string, decimal> categoryValuePair = DataRepository.GetTotalIncomePerCategory(Date, (SortByType)SortBy.SortType);

            var model = new PlotModel();
            var seriesP1 = new PieSeries
            {
                InsideLabelColor = OxyColors.White,
                OutsideLabelFormat = "",
                InsideLabelFormat = "{1} {2:0}%",
                TickHorizontalLength = 0.00,
                TickRadialLength = 0.00
            };

            foreach (var pair in categoryValuePair)
            {
                var slice = new PieSlice(pair.Key, (double)pair.Value)
                {
                    IsExploded = true,
                    Fill = OxyColor.Parse("#30B55C")
                };
                seriesP1.Slices.Add(slice);
            }
            model.Series.Add(seriesP1);
            CategoryIncomePieChart = model;
        }

        private void BuildCategoryOutcomePieChart()
        {
            Dictionary<string, decimal> categoryValuePair = DataRepository.GetTotalOutcomePerCategory(Date, (SortByType)SortBy.SortType);

            var model = new PlotModel();
            var seriesP1 = new PieSeries
            {
                InsideLabelColor = OxyColors.White,
                OutsideLabelFormat = "",
                InsideLabelFormat = "{1} {2:0}%",
                TickHorizontalLength = 0.00,
                TickRadialLength = 0.00
            };
            foreach (var pair in categoryValuePair)
            {
                var slice = new PieSlice(pair.Key, (double)pair.Value)
                {
                    IsExploded = true,
                    Fill = OxyColor.Parse("#F55255")
                };
                seriesP1.Slices.Add(slice);
            }
            model.Series.Add(seriesP1);
            CategoryOutcomePieChart = model;
        }

        private void BuildIncomeOutcomeChart()
        {
            List<decimal> incomePerMonth = DataRepository.GetTotalIncomePerMonth();
            List<decimal> outcomePerMonth = DataRepository.GetTotalOutcomePerMonth();
            double max = (double)incomePerMonth.Concat(outcomePerMonth).Max();
            max = max + max;
            max = max <= 0 ? 1 : max;
            double majorStep = max / 2;
            double minorStep = majorStep / 2;
            var model = new PlotModel
            {
                PlotAreaBorderColor = OxyColor.Parse("#706652"),
                TextColor = OxyColor.Parse("#706652")
            };

            var bottomAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                TicklineColor = OxyColor.Parse("#706652"),
                MinorTicklineColor = OxyColor.Parse("#706652"),
                IsZoomEnabled = false,
                IsPanEnabled = false
            };

            bottomAxis.ActualLabels.Add(ManeyResources.January);
            bottomAxis.ActualLabels.Add(ManeyResources.February);
            bottomAxis.ActualLabels.Add(ManeyResources.March);
            bottomAxis.ActualLabels.Add(ManeyResources.April);
            bottomAxis.ActualLabels.Add(ManeyResources.May);
            bottomAxis.ActualLabels.Add(ManeyResources.June);
            bottomAxis.ActualLabels.Add(ManeyResources.July);
            bottomAxis.ActualLabels.Add(ManeyResources.August);
            bottomAxis.ActualLabels.Add(ManeyResources.September);
            bottomAxis.ActualLabels.Add(ManeyResources.October);
            bottomAxis.ActualLabels.Add(ManeyResources.November);
            bottomAxis.ActualLabels.Add(ManeyResources.December);
            model.Axes.Add(bottomAxis);
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = max,
                Minimum = 0,
                MajorStep = majorStep,
                MinorStep = minorStep,
                TicklineColor = OxyColor.Parse("#706652"),
                MinorTicklineColor = OxyColor.Parse("#706652"),
                IsZoomEnabled = false,
                IsPanEnabled = false
            });

            var incomePerMonthSeries = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColors.White,
                Color = OxyColor.FromRgb(48, 181, 92)
                
            };

            var outcomePerMonthSeries = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColors.White,
                Color = OxyColor.FromRgb(245, 82, 85)
            };

            for (double i = 0.0; i < 12.0; i++)
            {
                incomePerMonthSeries.Points.Add(new DataPoint(i, (double)incomePerMonth[Convert.ToInt16(i)]));
            }

            for (double i = 0.0; i < 12.0; i++)
            {
                outcomePerMonthSeries.Points.Add(new DataPoint(i, (double)outcomePerMonth[Convert.ToInt16(i)]));
            }

            model.Series.Add(incomePerMonthSeries);
            model.Series.Add(outcomePerMonthSeries);
            IncomeOutcomeChart = model;
        }

        private bool _mIsDateEnabled;
        public bool IsDateEnabled
        {
            get => _mIsDateEnabled;
            set
            {
                if (_mIsDateEnabled != value)
                {
                    _mIsDateEnabled = value;
                }
                RaisePropertyChanged();
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

        private decimal _mIncome;
        public decimal Income
        {
            get => _mIncome;
            set
            {
                if (_mIncome != value)
                {
                    _mIncome = value;
                }
                RaisePropertyChanged();
            }
        }
        private decimal _mOutcome;
        public decimal Outcome
        {
            get => _mOutcome;
            set
            {
                if (_mOutcome != value)
                {
                    _mOutcome = value;
                }
                RaisePropertyChanged();
            }
        }
        private decimal _mBalance;
        public decimal Balance
        {
            get => _mBalance;
            set
            {
                if (_mBalance != value)
                {
                    _mBalance = value;
                }
                RaisePropertyChanged();
            }
        }

        private PlotModel _mCategoryPieChart;
        public PlotModel CategoryPieChart
        {
            get => _mCategoryPieChart;
            set
            {
                if (_mCategoryPieChart != value)
                {
                    _mCategoryPieChart = value;
                }
                RaisePropertyChanged();
            }
        }

        private PlotModel _mCategoryIncomePieChart;
        public PlotModel CategoryIncomePieChart
        {
            get => _mCategoryIncomePieChart;
            set
            {
                if (_mCategoryIncomePieChart != value)
                {
                    _mCategoryIncomePieChart = value;
                }
                RaisePropertyChanged();
            }
        }

        private PlotModel _mCategoryOutcomePieChart;
        public PlotModel CategoryOutcomePieChart
        {
            get => _mCategoryOutcomePieChart;
            set
            {
                if (_mCategoryOutcomePieChart != value)
                {
                    _mCategoryOutcomePieChart = value;
                }
                RaisePropertyChanged();
            }
        }

        private PlotModel _mIncomeOutcomeChart;
        public PlotModel IncomeOutcomeChart
        {
            get => _mIncomeOutcomeChart;
            set
            {
                if (_mIncomeOutcomeChart != value)
                {
                    _mIncomeOutcomeChart = value;
                }
                RaisePropertyChanged();
            }
        }
    }
}
