using Microsoft.ApplicationInsights;
using MuskanMobile.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace MuskanMobile.Application.Services
{
    public class TelemetryService : ITelemetryService
    {
        private readonly TelemetryClient _telemetryClient;

        public TelemetryService(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void TrackEvent(string eventName, string? property = null, string? value = null)
        {
            var properties = new Dictionary<string, string>();
            if (property != null && value != null)
            {
                properties[property] = value;
            }

            _telemetryClient.TrackEvent(eventName, properties);
        }

        public void TrackOrderCreated(int orderId, decimal amount, string customerName)
        {
            var properties = new Dictionary<string, string>
            {
                { "OrderId", orderId.ToString() },
                { "CustomerName", customerName },
                { "Currency", "INR" },
                { "Amount", amount.ToString() } // Amount as string property
            };

            _telemetryClient.TrackEvent("OrderCreated", properties);

            // Alternatively, track metrics separately if needed
            _telemetryClient.TrackMetric("OrderAmount", (double)amount);
        }

        public void TrackProductSold(int productId, string productName, int quantity, decimal revenue)
        {
            var properties = new Dictionary<string, string>
            {
                { "ProductId", productId.ToString() },
                { "ProductName", productName },
                { "Quantity", quantity.ToString() },
                { "Revenue", revenue.ToString() }
            };

            _telemetryClient.TrackEvent("ProductSold", properties);

            // Track metrics separately
            _telemetryClient.TrackMetric("ProductQuantity", quantity);
            _telemetryClient.TrackMetric("ProductRevenue", (double)revenue);
        }

        public void TrackException(Exception ex, string context)
        {
            var properties = new Dictionary<string, string>
            {
                { "Context", context }
            };

            _telemetryClient.TrackException(ex, properties);
        }
    }
}