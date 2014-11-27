namespace Micropolis
{
    // This file is part of Micropolis for WinRT.
    // Copyright (C) 2014 Andreas Balzer, Felix Dietrich, Florian Thurnwald and Ivo Vutov
    // Portions Copyright (C) MicropolisJ by Jason Long
    // Portions Copyright (C) Micropolis Don Hopkins
    // Portions Copyright (C) 1989-2007 Electronic Arts Inc.
    //
    // Micropolis for WinRT is free software; you can redistribute it and/or modify
    // it under the terms of the GNU GPLv3, with Additional terms.
    // See the README file, included in this distribution, for details.
    // Project website: http://code.google.com/p/micropolis/

    using System;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Engine;

    public sealed partial class MessagesPane
    {
        /// <summary>
        /// Initiates a new instance of the MessagesPane control.
        /// </summary>
        public MessagesPane()
        {
            InitializeComponent();
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
            var tb = new TextBlock
            {
                Text = messageText,
                Style = Application.Current.Resources["BodyTextBlockStyle"] as Style,
                Foreground = new SolidColorBrush(Colors.Black)
            };
            messagesSP.Children.Insert(0, tb);

            if (messagesSP.Children.Count > 6)
            {
                for (int pos = 6; pos < messagesSP.Children.Count; pos++)
                {
                    messagesSP.Children.RemoveAt(pos);
                }
            }
        }
    }
}