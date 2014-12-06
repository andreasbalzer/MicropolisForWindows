using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Engine;
using Micropolis.Common;

namespace Micropolis.ViewModels
{
    public class MessagesPaneViewModel : BindableBase
    {

        public ObservableCollection<string> Messages { get; set; }


        public MessagesPaneViewModel()
        {
            Messages=new ObservableCollection<string>();
        }
        /// <summary>
        /// Adds a new message to the message pane.
        /// </summary>
        /// <param name="message">Message to add</param>
        public void AppendCityMessage(MicropolisMessage message)
        {
            AppendMessageText(Strings.GetString(message.Name));
        }

        /// <summary>
        /// Adds a new message to the message pane
        /// </summary>
        /// <param name="messageText">Message to add</param>
        private void AppendMessageText(String messageText)
        {
            Messages.Insert(0,messageText);

            if (Messages.Count > 6)
            {
                for (int pos = 6; pos < Messages.Count; pos++)
                {
                    Messages.RemoveAt(pos);
                }
            }
        }
    }
}
