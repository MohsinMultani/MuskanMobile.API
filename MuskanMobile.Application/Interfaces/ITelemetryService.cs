using System;

namespace MuskanMobile.Application.Interfaces
{
    public interface ITelemetryService
    {
        void TrackEvent(string eventName, string? property = null, string? value = null);
        void TrackOrderCreated(int orderId, decimal amount, string customerName);
        void TrackProductSold(int productId, string productName, int quantity, decimal revenue);
        void TrackException(Exception ex, string context);
    }
}