namespace PriceTicker.Service
{
    public delegate void PriceUpdateDelegate(IPriceService sender, uint instrumentID, IPrices prices);

    public interface IPriceService
    {
        void Start();

        void Stop();

        bool IsStarted { get; }

        event PriceUpdateDelegate NewPricesArrived;
    }
}