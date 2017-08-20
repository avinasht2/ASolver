using PriceTicker.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace PriceTicker
{
    public class PricesListViewModel : NotifyPropertyChangedBase
    {
        private RelayCommand startCommand;
        private RelayCommand stopCommand;
        private ObservableCollection<InstrumentViewModel> instrumentViewModelCollection;
        private List<double> updateFrequencies;

        /// <summary>
        ///
        /// </summary>
        public IPriceService PriceService
        {
            get;
            private set;
        }

        /// <summary>
        ///
        /// </summary>
        public List<double> UpdateFrequencies
        {
            get { return this.updateFrequencies; }
            set { this.SetAndNotify(ref this.updateFrequencies, value, "UpdateFrequencies"); }
        }

        public ListCollectionView UpdateFrequenciesView
        {
            get { return CollectionViewSource.GetDefaultView(this.updateFrequencies) as ListCollectionView; }
        }

        /// <summary>
        ///
        /// </summary>
        public RelayCommand StartCommand
        {
            get { return this.startCommand; }
            set { this.SetAndNotify(ref this.startCommand, value, "StartCommand"); }
        }

        /// <summary>
        ///
        /// </summary>
        public RelayCommand StopCommand
        {
            get { return this.stopCommand; }
            set { this.SetAndNotify(ref this.stopCommand, value, "StopCommand"); }
        }

        /// <summary>
        ///
        /// </summary>
        public ObservableCollection<InstrumentViewModel> InstrumentViewModelCollection
        {
            get { return this.instrumentViewModelCollection; }
            set { this.SetAndNotify(ref this.instrumentViewModelCollection, value, "InstrumentViewModelCollection"); }
        }

        /// <summary>
        ///
        /// </summary>
        public PricesListViewModel(IPriceService priceService)
        {
            this.PriceService = priceService;
            PriceService.NewPricesArrived += this.InstrumentPricesUpdateHandler;
            this.StartCommand = new RelayCommand(this.ExecuteStartCommand, this.CanExecuteStartCommand);
            this.StopCommand = new RelayCommand(this.ExecuteStopCommand, this.CanExecuteStopCommand);
            this.InstrumentViewModelCollection = new ObservableCollection<InstrumentViewModel>();
            this.UpdateFrequencies = new List<double> { 150, 250, 500, 1000 };
            this.UpdateFrequenciesView.CurrentChanged += this.OnUpdateFrequenciesViewCurrentChanged;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpdateFrequenciesViewCurrentChanged(object sender, EventArgs e)
        {
            var selectedUpdateFrequency = (double)this.UpdateFrequenciesView.CurrentItem;
            var randomWalkPriceService = this.PriceService as RandomWalkPriceService;
            if (randomWalkPriceService != null)
            {
                // Interval is not present on IPriceService
                randomWalkPriceService.Interval = selectedUpdateFrequency;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteStopCommand(object obj)
        {
            this.PriceService.Stop();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanExecuteStopCommand(object obj)
        {
            return this.PriceService.IsStarted;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteStartCommand(object obj)
        {
            this.PriceService.Start();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanExecuteStartCommand(object obj)
        {
            return !this.PriceService.IsStarted;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="price1"></param>
        /// <param name="price2"></param>
        /// <returns></returns>
        private PriceValueEnum GetPriceValueEnum(double price1, double price2)
        {
            if (price1 > price2)
            {
                return PriceValueEnum.Higher;
            }
            else if (price1 < price2)
            {
                return PriceValueEnum.Lower;
            }

            return PriceValueEnum.Neutral;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="instrumentID"></param>
        /// <param name="prices"></param>
        private void InstrumentPricesUpdateHandler(IPriceService sender, uint instrumentID, IPrices prices)
        {
            InstrumentViewModel instrumentViewModel = this.InstrumentViewModelCollection.FirstOrDefault((ivm) => ivm.InstrumentId == instrumentID);
            if (instrumentViewModel != null)
            {
                instrumentViewModel.Volume = prices.Volume;
                instrumentViewModel.AskQty = prices.AskQty;
                instrumentViewModel.BidQty = prices.BidQty;
                instrumentViewModel.AskPriceValueEnum = this.GetPriceValueEnum(prices.AskPx, instrumentViewModel.AskPx);
                instrumentViewModel.BidPriceValueEnum = this.GetPriceValueEnum(prices.BidPx, instrumentViewModel.BidPx);
                instrumentViewModel.AskPx = prices.AskPx;
                instrumentViewModel.BidPx = prices.BidPx;
            }
            else
            {
                this.InstrumentViewModelCollection.Add(
                    new InstrumentViewModel
                    {
                        InstrumentId = instrumentID,
                        AskPriceValueEnum = PriceValueEnum.Neutral,
                        AskPx = prices.AskPx,
                        AskQty = prices.AskQty,
                        BidPriceValueEnum = PriceValueEnum.Neutral,
                        BidPx = prices.BidPx,
                        BidQty = prices.BidQty,
                        Volume = prices.Volume
                    });
            }
        }
    }
}