using System;
using Windows.UI.Xaml;

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

    public sealed partial class ConfirmationBar
    {
        /// <summary>
        /// Confirmed event fired when user confirms the tool placement.
        /// </summary>
        private event EventHandler _confirmed;
        
        /// <summary>
        /// Confirmed event fired when user confirms the tool placement.
        /// </summary>
        public event EventHandler Confirmed { add { _confirmed += value; } remove { _confirmed -= value; } }

        /// <summary>
        /// Declined event fired when user declines the tool placement.
        /// </summary>
        private event EventHandler _declined;
        
        /// <summary>
        /// Declined event fired when user declines the tool placement.
        /// </summary>
        public event EventHandler Declined { add { _declined += value; } remove { _declined -= value; } }

        /// <summary>
        /// Uped event fired when user moves tool placement one tile up on map.
        /// </summary>
        private event EventHandler _uped;

        /// <summary>
        /// Uped event fired when user moves tool placement one tile up on map.
        /// </summary>
        public event EventHandler Uped { add { _uped += value; } remove { _uped -= value; } }

        /// <summary>
        /// Downed event fired when user moves tool placement one tile down on map.
        /// </summary>
        private event EventHandler _downed;

        /// <summary>
        /// Downed event fired when user moves tool placement one tile down on map.
        /// </summary>
        public event EventHandler Downed { add { _downed += value; } remove { _downed -= value; } }

        /// <summary>
        /// Lefted event fired when user moves tool placement one tile left on map.
        /// </summary>
        private event EventHandler _lefted;
        
        /// <summary>
        /// Lefted event fired when user moves tool placement one tile left on map.
        /// </summary>
        public event EventHandler Lefted { add { _lefted += value; } remove { _lefted -= value; } }

        /// <summary>
        /// Righted event fired when user moves tool placement one tile right on map.
        /// </summary>
        private event EventHandler _righted;
        
        /// <summary>
        /// Righted event fired when user moves tool placement one tile right on map.
        /// </summary>
        public event EventHandler Righted { add { _righted += value; } remove { _righted -= value; } }

        /// <summary>
        /// Fires uped event.
        /// </summary>
        private void OnUped()
        {
            if (_uped != null)
            {
                _uped(this, null);
            }
        }

        /// <summary>
        /// Fires downed event.
        /// </summary>
        private void OnDowned()
        {
            if (_downed != null)
            {
                _downed(this, null);
            }
        }

        /// <summary>
        /// Fires lefted event.
        /// </summary>
        private void OnLefted()
        {
            if (_lefted != null)
            {
                _lefted(this, null);
            }
        }

        /// <summary>
        /// Fires righted event.
        /// </summary>
        private void OnRighted()
        {
            if (_righted != null)
            {
                _righted(this, null);
            }
        }
        
        /// <summary>
        /// Fires declined event.
        /// </summary>
        private void OnDeclined()
        {
            if (_declined != null)
            {
                _declined(this, null);
            }
        }

        /// <summary>
        /// Fires confirmed event.
        /// </summary>
        private void OnConfirmed()
        {
            if (_confirmed != null)
            {
                _confirmed(this, null);
            }
        }

        /// <summary>
        /// Initiates a new instance of the ConfirmationBar class.
        /// </summary>
        public ConfirmationBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// User clicked confirm button to confirm tool placement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            OnConfirmed();
        }

        /// <summary>
        /// User clicked decline button to decline tool placement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Decline_OnClick(object sender, RoutedEventArgs e)
        {
            OnDeclined();
        }

        /// <summary>
        /// User clicked on up button to move tool placement one tile up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Up_OnClick(object sender, RoutedEventArgs e)
        {
            OnUped();
        }

        /// <summary>
        /// User clicked on down button to move tool placement one tile down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Down_OnClick(object sender, RoutedEventArgs e)
        {
            OnDowned();
        }

        /// <summary>
        /// User clicked on left button to move tool placement one tile left.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Left_OnClick(object sender, RoutedEventArgs e)
        {
            OnLefted();
        }

        /// <summary>
        /// User clicked on right button to move tool placement one tile right.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Right_OnClick(object sender, RoutedEventArgs e)
        {
            OnRighted();
        }
    }
}
