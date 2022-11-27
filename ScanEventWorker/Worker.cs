using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using ScanEventWorker.Stores;
using System.Collections.Generic;
using System.Data.Common;

namespace ScanEventWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTime startTime = DateTime.Now;
            _logger.LogInformation("Worker running at: {time}", startTime.ToUniversalTime());

            try
            {
                //Initialization
                DatabaseAccessHelper.InitializeDatabase();
                WorkerConfigStore.GetWorkerConfig();
                LogTypeStore.InitializeLogTypeId();
                WorkerLogStore.LogWithLocalTime(ConstantHelper.INFO_LOG_TYPE_ID, "Worker started", startTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            try
            {
                int fromEventId = ConstantHelper.INIT_EVENT_ID;
                while (!stoppingToken.IsCancellationRequested)
                {// until cancellation, do the followings
                    _logger.LogInformation($"From: {fromEventId} take: {ConstantHelper.LIMIT}");
                    
                    //fetch scanEvents via API call with pagination
                    var events = await ApiHelper.AsyncGetScanEvents(fromEventId, ConstantHelper.LIMIT);
                    if (events?.ScanEvents?.Any() == true)
                    {// response list not empty
                        WorkerLogStore.Log(ConstantHelper.INFO_LOG_TYPE_ID, $"Fetching ScanEvents from: {fromEventId} take: {ConstantHelper.LIMIT} received: {events?.ScanEvents?.Count} ");
                        foreach (var e in events.ScanEvents ?? Enumerable.Empty<ScanEvent>())
                        {//process events one by one

                            ProcessScanEvent(e);

                            if (fromEventId < e.EventId + 1)
                            {
                                fromEventId = e.EventId + 1;
                            }
                            // update last process Event Id 
                            WorkerConfigStore.UpdateLastProcessedEventId(e.EventId);
                        }
                    }
                    else
                    {// empty response list
                        WorkerLogStore.Log(ConstantHelper.INFO_LOG_TYPE_ID, $"Fetching ScanEvents from: {fromEventId} take: {ConstantHelper.LIMIT} received: 0 ");
                        await Task.Delay(ConstantHelper.API_TIME, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                WorkerLogStore.Log(ConstantHelper.ERROR_LOG_TYPE_ID, ex.Message + Environment.NewLine + ex.StackTrace);
            }
            
        }

        private void ProcessScanEvent(ScanEvent scanEvent)
        {
            // To Improve: validate input scanEvent

            // Check if parcelId with a newer created datetime exists in Parcel table
            if (!ScanEventStore.CheckParcelIdWithLaterDateExist(scanEvent.ParcelId, scanEvent.CreatedDateTimeUtc))
            {// not exists, add/replace event into Parcel table
                ScanEventStore.RemoveScanEventByParcelId(scanEvent.ParcelId);
                ScanEventStore.InsertScanEvent(scanEvent);
            }
            //add Event History
            ParcelEventHistoryStore.InsertParcelEventHistory(scanEvent.ParcelId, scanEvent.Type, scanEvent.CreatedDateTimeUtc);
        }
        
    }
}