using System;
using Windows.UI.Xaml.Controls;
using Micropolis.ViewModels;

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

    /// <summary>
    /// A connected view connects the MapOverlay control to the MicropolisDrawingArea control thus allows for synchronized zooming and scrolling.
    /// </summary>
    [Obsolete]
    public class ConnectedView // ChangeListener
    {
        /// <summary>
        /// The scrollpane surrounding the MicropolisDrawingArea.
        /// </summary>
        public ScrollViewer ScrollPane;
        
        /// <summary>
        /// The MicropolisDrawingArea with game interface
        /// </summary>
        public MicropolisDrawingAreaViewModel ViewModel;

        /// <summary>
        /// Initiates a new instance of the ConnectedView class.
        /// </summary>
        /// <param name="view">The MicropolisDrawingArea with game interface.</param>
        /// <param name="scrollPane">The scrollpane surrounding the MicropolisDrawingArea</param>
        public ConnectedView(MicropolisDrawingAreaViewModel viewModel, ScrollViewer scrollPane)
        {
            ViewModel = viewModel;
            ScrollPane = scrollPane;
            scrollPane.ViewChanged += scrollPane_ViewChanged;
            //    .getViewport().addChangeListener(this);
        }

        /// <summary>
        /// Has been updating the MicropolisDrawingArea. Replaced with game loop to increase FPS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void scrollPane_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // todo: this was removed as it took too long to update the view every time
            //view.Repaint();
        }
    }
}