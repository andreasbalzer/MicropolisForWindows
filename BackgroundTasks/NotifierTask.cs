using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTasks
{
    public sealed class NotifierTask :IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            ShowToastNotification(taskInstance.Task.Name);
        }

        private void ShowToastNotification(String message)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            // Set Text
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("Micropolis"));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(message));

            // toast duration
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "short");

            // toast navigation
            var toastNavigationUriString = "launchFromToast";
            var toastElement = ((XmlElement)toastXml.SelectSingleNode("/toast"));
            toastElement.SetAttribute("launch", toastNavigationUriString);

            // delete old toast
            var history = typeof (ToastNotificationManager).GetRuntimeProperties().Single(x => x.Name == "History").GetValue(typeof (ToastNotificationManager));
            history.GetType().GetRuntimeMethod("Remove", new[] {typeof (String)}).Invoke(history, new Object[] {"M1"});
            
            // Create the toast notification based on the XML content you've specified.
            ToastNotification toast = new ToastNotification(toastXml);
            toast.GetType().GetRuntimeProperties().Single(x => x.Name == "Tag").SetValue(toast, "M1");
            
            // Send your toast notification.
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    }
}
