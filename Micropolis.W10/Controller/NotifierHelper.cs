using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Micropolis.Controller
{
    public static class NotifierHelper
    {
        private const string entryPoint = "BackgroundTasks.NotifierTask";
        private const int minutesUntilToast = 1380; // 23 hours

        public static void RegisterNotifier()
        {
            UnregisterTask();

            RegisterNewTask();
        }

        private static void RegisterNewTask()
        {
            TimeTrigger trigger23hours = new TimeTrigger(minutesUntilToast, true);
            BackgroundExecutionManager.RequestAccessAsync();

            var builder = new BackgroundTaskBuilder();

            builder.Name = Strings.GetString("NotifierMessage");
            builder.TaskEntryPoint = entryPoint;
            builder.SetTrigger(trigger23hours);
            BackgroundTaskRegistration task = builder.Register();
        }

        private static void UnregisterTask()
        {
            var taskRegistered = false;

            foreach (var curTask in BackgroundTaskRegistration.AllTasks)
            {
                if (curTask.Value.Name == Strings.GetString("NotifierMessage"))
                {
                    curTask.Value.Unregister(true);
                    break;
                }
            }
        }
    }
}
