namespace PriceTicker
{
    public class InstrumentViewModel : NotifyPropertyChangedBase
    {
        private double bidPx;

        private uint bidQty;

        private double askPx;

        private uint askQty;

        private uint volume;

        private uint instrumentId;

        private PriceValueEnum askPriceValueEnum;

        private PriceValueEnum bidPriceValueEnum;

        public PriceValueEnum AskPriceValueEnum
        {
            get { return this.askPriceValueEnum; }
            set { this.SetAndNotify(ref this.askPriceValueEnum, value, "AskPriceValueEnum"); }
        }

        public PriceValueEnum BidPriceValueEnum
        {
            get { return this.bidPriceValueEnum; }
            set { this.SetAndNotify(ref this.bidPriceValueEnum, value, "BidPriceValueEnum"); }
        }

        public uint InstrumentId { get; set; }

        public double BidPx
        {
            get { return this.bidPx; }
            set { this.SetAndNotify(ref this.bidPx, value, "BidPx"); }
        }

        public uint BidQty
        {
            get { return this.bidQty; }
            set { this.SetAndNotify(ref this.bidQty, value, "BidQty"); }
        }

        public double AskPx
        {
            get { return this.askPx; }
            set { this.SetAndNotify(ref this.askPx, value, "AskPx"); }
        }

        public uint AskQty
        {
            get { return this.askQty; }
            set { this.SetAndNotify(ref this.askQty, value, "AskQty"); }
        }

        public uint Volume
        {
            get { return this.volume; }
            set { this.SetAndNotify(ref this.volume, value, "Volume"); }
        }
    }
}