﻿namespace StockPriceMonitoringAndAlerts.Services
{
    public interface INotificationService
    {
        Task SendAsync(string message);
    }
}