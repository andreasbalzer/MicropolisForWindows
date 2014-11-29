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

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Micropolis.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Micropolis.Common;

    public class MainGamePageViewModel : BindableBase
    {
        private bool _isMessagesVisible;

        private bool _isMiniMapVisible;

        private bool _isSpeedPause;
        private bool _isSpeedSlow;
        private bool _isSpeedNormal;
        private bool _isSpeedFast;
        private bool _isSpeedSuperFast;

        public MainGamePageViewModel()
        {
            ToggleMiniMapCommand = new DelegateCommand(ToggleMiniMap);
            ToggleMessagesCommand = new DelegateCommand(ToggleMessages);
            SpeedPauseCommand = new DelegateCommand(SpeedPause);
            SpeedSlowCommand = new DelegateCommand(SpeedSlow);
            SpeedNormalCommand = new DelegateCommand(SpeedNormal);
            SpeedFastCommand = new DelegateCommand(SpeedFast);
            SpeedSuperFastCommand = new DelegateCommand(SpeedSuperFast);
        }

        public bool IsMessagesVisible
        {
            get
            {
                return this._isMessagesVisible;
            }
            set
            {
                this.SetProperty(ref this._isMessagesVisible, value);
                
            }
        }

        public bool IsMiniMapVisible
        {
            get
            {
                return this._isMiniMapVisible;
            }

            set
            {
                this.SetProperty(ref this._isMiniMapVisible, value);
            }
        }

        public bool IsSpeedPause
        {
            get { return _isSpeedPause; }
            set
            {
                this.SetProperty(ref this._isSpeedPause, value);
            }
        }

        public bool IsSpeedSlow
        {
            get { return _isSpeedSlow; }
            set
            {
                this.SetProperty(ref this._isSpeedSlow, value);
            }
        }

        public bool IsSpeedNormal
        {
            get { return _isSpeedNormal; }
            set
            {
                this.SetProperty(ref this._isSpeedNormal, value);
            }
        }

        public bool IsSpeedFast
        {
            get { return _isSpeedFast; }
            set
            {
                this.SetProperty(ref this._isSpeedFast, value);
            }
        }

        public bool IsSpeedSuperFast
        {
            get { return _isSpeedSuperFast; }
            set
            {
                this.SetProperty(ref this._isSpeedSuperFast, value);
            }
        }

        public DelegateCommand ToggleMiniMapCommand { get; private set; }
        public DelegateCommand ToggleMessagesCommand { get; private set; }
        public DelegateCommand SpeedPauseCommand { get; private set; }
        public DelegateCommand SpeedSlowCommand { get; private set; }
        public DelegateCommand SpeedNormalCommand { get; private set; }
        public DelegateCommand SpeedFastCommand { get; private set; }
        public DelegateCommand SpeedSuperFastCommand { get; private set; }
        private void ToggleMiniMap()
        {
            IsMiniMapVisible = !IsMiniMapVisible;
        }

        private void ToggleMessages()
        {
            IsMessagesVisible = !IsMessagesVisible;
        }


        private void SpeedPause()
        {
            IsSpeedSlow = false;
            IsSpeedNormal = false;
            IsSpeedFast = false;
            IsSpeedSuperFast = false;
            IsSpeedPause = true;
        }

        private void SpeedSlow()
        {
            IsSpeedPause = false;
            IsSpeedNormal = false;
            IsSpeedFast = false;
            IsSpeedSuperFast = false;
            IsSpeedSlow = true;
        }

        private void SpeedNormal()
        {
            IsSpeedPause = false;
            IsSpeedSlow = false;
            IsSpeedFast = false;
            IsSpeedSuperFast = false;
            IsSpeedNormal = true;
        }

        private void SpeedFast()
        {
            IsSpeedPause = false;
            IsSpeedSlow = false;
            IsSpeedNormal = false;
            IsSpeedSuperFast = false;
            IsSpeedFast = true;
        }

        private void SpeedSuperFast()
        {
            IsSpeedPause = false;
            IsSpeedSlow = false;
            IsSpeedNormal = false;
            IsSpeedFast = false;
            IsSpeedSuperFast = true;
        }
    }
}