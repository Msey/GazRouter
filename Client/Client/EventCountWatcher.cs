using System;
using System.ServiceModel;
using System.Windows.Threading;
using GazRouter.Application;
using GazRouter.Common.Events;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog.EventRecipient;
using GazRouter.DTO.Infrastructure.Faults;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Client
{
    public static class EventCountWatcher
    {
        private static DispatcherTimer _timer;
        private static NonAckEventCountDTO _nonAckEventCount = new NonAckEventCountDTO();

        public static void Run()
        {
            if (_timer != null)
                throw new Exception("Таймер уже запущен");
            
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(20)};
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        public static void Stop()
        {
            if (_timer == null) return;
            _timer.Tick -= TimerTick;
            _timer.Stop();
            _timer = null;
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            UpdateNonEventAck();
        }

        private static async void UpdateNonEventAck()
        {
            try
            {
                if (UserProfile.Current?.Site == null) return;

                var count =
                    await
                        new EventLogServiceProxy().GetNotAckEventCountAsync(UserProfile.Current.Site.Id)
                            .ConfigureAwait(false);

                if (_nonAckEventCount.LastEventDate != count.LastEventDate || _nonAckEventCount.Count != count.Count)
                {
                    ServiceLocator.Current.GetInstance<IEventAggregator>()
                        .GetEvent<NotAckEventCountChangedEvent>()
                        .Publish(count);

                    _nonAckEventCount = count;
                }
            }
            catch (FaultException<FaultDetail>)
            {
                Stop();
                throw;
            }
        }
    }
}