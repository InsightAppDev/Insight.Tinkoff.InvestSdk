namespace Insight.Tinkoff.InvestSdk.Dto
{
    public sealed class PlacedOrder
    {
        public string OrderId { get; set; }

        public OperationType Operation { get; set; }

        public OrderStatus Status { get; set; }

        public string RejectReason { get; set; }

        public int RequestedLots { get; set; }

        public int ExecutedLots { get; set; }

        public MoneyAmount Commission { get; set; }
    }
}