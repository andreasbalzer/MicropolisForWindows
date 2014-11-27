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

namespace Micropolis.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Micropolis.Common;

    public class MainGamePageViewModel : BindableBase
    {
        private bool messagesVisible;

        private bool miniMapVisible;

        public MainGamePageViewModel()
        {
            ToggleMiniMapCommand = new DelegateCommand(ToggleMiniMap);
        }

        public bool MessagesVisible
        {
            get
            {
                return this.messagesVisible;
            }

            set
            {
                this.SetProperty(ref this.messagesVisible, value);
                if (value == true && MiniMapVisible)
                {
                    MiniMapVisible = false;
                }
            }
        }

        public bool MiniMapVisible
        {
            get
            {
                return this.miniMapVisible;
            }

            set
            {
                this.SetProperty(ref this.miniMapVisible, value);
                if (value == true && MessagesVisible)
                {
                    MessagesVisible = false;
                }
            }
        }

        public DelegateCommand ToggleMiniMapCommand { get; private set; }

        private void ToggleMiniMap()
        {
            MiniMapVisible = !MiniMapVisible;
        }
    }
}