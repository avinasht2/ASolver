using Microsoft.VisualStudio.TestTools.UnitTesting;
using PriceTicker.Service;
using System.Linq;

namespace PriceTicker.Tests
{
    [TestClass]
    public class PriceServiceUpdatesUnitTests
    {
        private PricesListViewModel pricesListViewModel;
        private IPriceService priceService;
        private double oldAskPx;
        private double oldBidPx;

        [TestInitialize]
        public void Initialize()
        {
            this.priceService = new RandomWalkPriceService();
            this.pricesListViewModel = new PricesListViewModel(this.priceService);
        }

        [TestMethod]
        public void PricesListViewModelTests_VerifyDataTest()
        {
            this.priceService.NewPricesArrived += this.VerifyPriceServiceNewPricesArrivedUpdate;
            this.priceService.Start();

            if (this.priceService.IsStarted)
            {
                this.priceService.Stop();
            }
        }

        [TestMethod]
        public void PricesListViewModelTests_VerifyColorChangeAlgorithm()
        {
            this.priceService.Start();
            this.oldAskPx = this.pricesListViewModel.InstrumentViewModelCollection[0].AskPx;
            this.oldBidPx = this.pricesListViewModel.InstrumentViewModelCollection[0].BidPx;
            if (this.priceService.IsStarted)
            {
                this.priceService.Stop();
            }
            this.priceService.NewPricesArrived += this.VerifyPriceServiceColorChangeUpdate;
            this.priceService.Start();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="instrumentID"></param>
        /// <param name="prices"></param>
        public void VerifyPriceServiceColorChangeUpdate(IPriceService sender, uint instrumentID, IPrices prices)
        {
            InstrumentViewModel newInstrumentViewModel = this.pricesListViewModel.InstrumentViewModelCollection[0];
            if (newInstrumentViewModel.InstrumentId == instrumentID)
            {
                if (prices.AskPx > this.oldAskPx)
                {
                    Assert.AreEqual(PriceValueEnum.Higher, newInstrumentViewModel.AskPriceValueEnum);
                }
                else if (prices.AskPx < this.oldAskPx)
                {
                    Assert.AreEqual(PriceValueEnum.Lower, newInstrumentViewModel.AskPriceValueEnum);
                }
                else
                {
                    Assert.AreEqual(PriceValueEnum.Neutral, newInstrumentViewModel.AskPriceValueEnum);
                }

                if (prices.BidPx > this.oldBidPx)
                {
                    Assert.AreEqual(PriceValueEnum.Higher, newInstrumentViewModel.BidPriceValueEnum);
                }
                else if (prices.BidPx < this.oldBidPx)
                {
                    Assert.AreEqual(PriceValueEnum.Lower, newInstrumentViewModel.BidPriceValueEnum);
                }
                else
                {
                    Assert.AreEqual(PriceValueEnum.Neutral, newInstrumentViewModel.BidPriceValueEnum);
                }

                this.priceService.NewPricesArrived -= this.VerifyPriceServiceColorChangeUpdate;

                if (this.priceService.IsStarted)
                {
                    this.priceService.Stop();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="instrumentID"></param>
        /// <param name="prices"></param>
        public void VerifyPriceServiceNewPricesArrivedUpdate(IPriceService sender, uint instrumentID, IPrices prices)
        {
            this.priceService.NewPricesArrived -= this.VerifyPriceServiceNewPricesArrivedUpdate;
            InstrumentViewModel instrumentViewModel = this.pricesListViewModel.InstrumentViewModelCollection.FirstOrDefault((ivm) => ivm.InstrumentId == instrumentID);
            Assert.IsNotNull(instrumentViewModel);
            Assert.IsTrue(instrumentViewModel.AskPx == prices.AskPx);
            Assert.IsTrue(instrumentViewModel.AskQty == prices.AskQty);
            Assert.IsTrue(instrumentViewModel.BidPx == prices.BidPx);
            Assert.IsTrue(instrumentViewModel.BidQty == prices.BidQty);
            Assert.IsTrue(instrumentViewModel.Volume == prices.Volume);
        }
    }
}