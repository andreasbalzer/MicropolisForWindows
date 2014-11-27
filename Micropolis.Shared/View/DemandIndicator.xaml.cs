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
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Engine;

    /// <summary>
    ///     Demand indicator visualizing demand for residential, commercial and industrial zones.
    /// </summary>
    public sealed partial class DemandIndicator : Engine.IListener
    {
        private const int UpperEdge = 19;
        private const int LowerEdge = 28;
        private const int MaxLength = 16;
        private const int ResLeft = 8;
        private const int ComLeft = 17;
        private const int IndLeft = 26;
        private const int BarWidth = 6;
        private Micropolis _engine;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DemandIndicator" /> class.
        /// </summary>
        public DemandIndicator()
        {
            InitializeComponent();
            Loaded += DemandIndicator_Loaded;
        }


        /// <summary>
        ///     Fired whenever resValve, comValve, or indValve changes. (Twice a month in game.)
        /// </summary>
        /// <remarks>implements Micropolis.IListener</remarks>
        public void DemandChanged()
        {
            Repaint();
        }


        /// <summary>
        ///     Cities the message.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="p">The p.</param>
        /// <remarks>implements Micropolis.IListener</remarks>
        public void CityMessage(MicropolisMessage m, CityLocation p)
        {
        }

        /// <summary>
        ///     Cities the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="p">The p.</param>
        public void CitySound(Sound sound, CityLocation p)
        {
        }

        /// <summary>
        ///     Fired whenever the "census" is taken, and the various historical counters have been updated. (Once a month in
        ///     game.)
        /// </summary>
        public void CensusChanged()
        {
        }

        /// <summary>
        ///     Fired whenever the city evaluation is recalculated. (Once a year.)
        /// </summary>
        public void EvaluationChanged()
        {
        }

        /// <summary>
        ///     Fired whenever the mayor's money changes.
        /// </summary>
        public void FundsChanged()
        {
        }

        /// <summary>
        ///     Fired whenever autoBulldoze, autoBudget, noDisasters, or simSpeed change.
        /// </summary>
        public void OptionsChanged()
        {
        }

        /// <summary>
        ///     Handles the Loaded event of the DemandIndicator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void DemandIndicator_Loaded(object sender, RoutedEventArgs e)
        {
            PaintComponent();
        }

        /// <summary>
        ///     Sets the engine.
        /// </summary>
        /// <param name="newEngine">The new engine.</param>
        public void SetEngine(Micropolis newEngine)
        {
            if (_engine != null)
            {
                //old engine
                _engine.RemoveListener(this);
            }

            _engine = newEngine;

            if (_engine != null)
            {
                //new engine
                _engine.AddListener(this);
            }
        }

        /// <summary>
        ///     Paints the component.
        /// </summary>
        public void PaintComponent()
        {
            if (_engine == null)
                return;

            int resValve = _engine.GetResValve();
            int ry0 = resValve <= 0 ? LowerEdge : UpperEdge;
            int ry1 = ry0 - resValve/100;

            if (ry1 - ry0 > MaxLength)
            {
                ry1 = ry0 + MaxLength;
            }
            if (ry1 - ry0 < -MaxLength)
            {
                ry1 = ry0 - MaxLength;
            }

            int comValve = _engine.GetComValve();
            int cy0 = comValve <= 0 ? LowerEdge : UpperEdge;
            int cy1 = cy0 - comValve/100;

            int indValve = _engine.GetIndValve();
            int iy0 = indValve <= 0 ? LowerEdge : UpperEdge;
            int iy1 = iy0 - indValve/100;

            if (ry0 != ry1)
            {
                if (ry1 < 0)
                {
                    ry1 = 0;
                }

                var transform = new TranslateTransform();
                transform.X = ResLeft;
                transform.Y = Math.Min(ry0, ry1);
                ResRectangle.Width = BarWidth;
                ResRectangle.Height = Math.Abs(ry1 - ry0);
                ResRectangle.RenderTransform = transform;
                ResRectangle.Visibility = Visibility.Visible;
            }

            if (cy0 != cy1)
            {
                if (cy1 < 0)
                {
                    cy1 = 0;
                }

                var transform = new TranslateTransform();
                transform.X = ComLeft;
                transform.Y = Math.Min(cy0, cy1);
                ComRectangle.Width = BarWidth;
                ComRectangle.Height = Math.Abs(cy1 - cy0);
                ComRectangle.RenderTransform = transform;
                ComRectangle.Visibility = Visibility.Visible;
            }

            if (iy0 != iy1)
            {
                if (iy1 < 0)
                {
                    iy1 = 0;
                }

                var transform = new TranslateTransform();
                transform.X = IndLeft;
                transform.Y = Math.Min(iy0, iy1);
                IndRectangle.Width = BarWidth;
                IndRectangle.Height = Math.Abs(iy1 - iy0);
                IndRectangle.RenderTransform = transform;
                IndRectangle.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     Repaints this instance.
        /// </summary>
        private void Repaint()
        {
            PaintComponent();
        }
    }
}