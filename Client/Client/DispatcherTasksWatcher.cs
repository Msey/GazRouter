using GazRouter.Application;
using GazRouter.Common.Events;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;
using System.ServiceModel;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.Infrastructure.Faults;

namespace GazRouter.Client
{
    public class DispatcherTasksWatcher
    {
        private static DispatcherTimer _timer;
        private static List<TaskRecordPdsDTO> _nonAckEvents = new List<TaskRecordPdsDTO>();

        public static void Run()
        {
            if (_timer != null)
                throw new Exception("Таймер уже запущен");

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(20) };
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

        public static async void UpdateNonEventAck()
        {
            try
            {
                if (UserProfile.Current?.Site == null) return;

                var count = await new DispatcherTaskServiceProxy().GetNotAckedTaskListAsync(
                    new GetTaskListParameterSet
                    {
                        SiteId = UserProfile.Current.Site.Id,
                        IsEnterprise = UserProfile.Current.Site.IsEnterprise
                    }).ConfigureAwait(false);


                bool update = false;
                if (count.Count != _nonAckEvents.Count)
                    update = true;
                else
                foreach (var t in count)
                {
                    if (!_nonAckEvents.Contains(t))
                        {
                            update = true;
                            break;
                        }
                }

                if (update)
                {
                    ServiceLocator.Current.GetInstance<IEventAggregator>()
                        .GetEvent<NotAckDispatherTaskListUpdatedEvent>()
                        .Publish(count);

                    _nonAckEvents = count;
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
