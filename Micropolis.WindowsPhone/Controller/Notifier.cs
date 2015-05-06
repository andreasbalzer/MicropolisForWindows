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
        private const string taskName = "Notifier Background Task";
        public static void RegisterNotifier()
        {
            UnregisterTask();

            RegisterNewTask();
        }

        private static void RegisterNewTask()
        {
            TimeTrigger trigger23hours = new TimeTrigger(30, true); //1380
            BackgroundExecutionManager.RequestAccessAsync();

            var builder = new BackgroundTaskBuilder();

            builder.Name = taskName;
            builder.TaskEntryPoint = "Micropolis.Controller.NotifierTask";
            builder.SetTrigger(trigger23hours);
            BackgroundTaskRegistration task = builder.Register();
        }

        private static void UnregisterTask()
        {
            var taskRegistered = false;

            foreach (var curTask in BackgroundTaskRegistration.AllTasks)
            {
                if (curTask.Value.Name == taskName)
                {
                    curTask.Value.Unregister(true);
                    break;
                }
            }
        }
    }
}
