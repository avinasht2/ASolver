namespace PriceTicker.Service
{
    public interface IPrices
    {
        double BidPx { get; }
        uint BidQty { get; }
        double AskPx { get; }
        uint AskQty { get; }
        uint Volume { get; }
    }
}