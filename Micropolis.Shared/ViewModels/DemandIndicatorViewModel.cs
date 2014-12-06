using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Engine;
using Micropolis.Common;

namespace Micropolis.ViewModels
{
    public class DemandIndicatorViewModel : BindableBase, Engine.IListener
    {
        private const int UpperEdge = 19;
        private const int LowerEdge = 28;
        private const int MaxLength = 16;
        private const int ResLeft = 8;
        private const int ComLeft = 17;
        private const int IndLeft = 26;
        private const int BarWidth = 6;
        private Engine.Micropolis _engine;

        public DemandIndicatorViewModel()
        {
            PaintComponent();
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
        ///     Sets the engine.
        /// </summary>
        /// <param name="newEngine">The new engine.</param>
        public void SetEngine(Engine.Micropolis newEngine)
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
            Repaint();
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

                
                ResRectangleX = ResLeft;
                ResRectangleY = Math.Min(ry0, ry1);
                ResRectangleWidth = BarWidth;
                ResRectangleHeight = Math.Abs(ry1 - ry0);
                ResRectangleIsVisible = true;
            }

            if (cy0 != cy1)
            {
                if (cy1 < 0)
                {
                    cy1 = 0;
                }

                ComRectangleX = ComLeft;
                ComRectangleY = Math.Min(cy0, cy1);
                ComRectangleWidth = BarWidth;
                ComRectangleHeight = Math.Abs(cy1 - cy0);
                ComRectangleIsVisible = true;
            }

            if (iy0 != iy1)
            {
                if (iy1 < 0)
                {
                    iy1 = 0;
                }

                IndRectangleX = IndLeft;
                IndRectangleY = Math.Min(iy0, iy1);
                IndRectangleWidth = BarWidth;
                IndRectangleHeight = Math.Abs(iy1 - iy0);
                IndRectangleIsVisible = true;
            }
        }

        /// <summary>
        ///     Repaints this instance.
        /// </summary>
        private void Repaint()
        {
            PaintComponent();
        }

        private double _resRectangleX;
        private double _resRectangleY;
        private double _resRectangleHeight;
        private double _resRectangleWidth;
        private bool _resRectangleIsVisible;

        private double _comRectangleX;
        private double _comRectangleY;
        private double _comRectangleHeight;
        private double _comRectangleWidth;
        private bool _comRectangleIsVisible;

        private double _indRectangleX;
        private double _indRectangleY;
        private double _indRectangleHeight;
        private double _indRectangleWidth;
        private bool _indRectangleIsVisible;

        public double ResRectangleX
        {
            get
            {
                return this._resRectangleX;
            }
            set
            {
                this.SetProperty(ref this._resRectangleX, value);

            }
        }
        public double ResRectangleY
        {
            get
            {
                return this._resRectangleY;
            }
            set
            {
                this.SetProperty(ref this._resRectangleY, value);

            }
        }
        public double ResRectangleHeight
        {
            get
            {
                return this._resRectangleHeight;
            }
            set
            {
                this.SetProperty(ref this._resRectangleHeight, value);

            }
        }
        public double ResRectangleWidth
        {
            get
            {
                return this._resRectangleWidth;
            }
            set
            {
                this.SetProperty(ref this._resRectangleWidth, value);

            }
        }
        public bool ResRectangleIsVisible
        {
            get
            {
                return this._resRectangleIsVisible;
            }
            set
            {
                this.SetProperty(ref this._resRectangleIsVisible, value);

            }
        }

        public double ComRectangleX
        {
            get
            {
                return this._comRectangleX;
            }
            set
            {
                this.SetProperty(ref this._comRectangleX, value);

            }
        }
        public double ComRectangleY
        {
            get
            {
                return this._comRectangleY;
            }
            set
            {
                this.SetProperty(ref this._comRectangleY, value);

            }
        }
        public double ComRectangleHeight
        {
            get
            {
                return this._comRectangleHeight;
            }
            set
            {
                this.SetProperty(ref this._comRectangleHeight, value);

            }
        }
        public double ComRectangleWidth
        {
            get
            {
                return this._comRectangleWidth;
            }
            set
            {
                this.SetProperty(ref this._comRectangleWidth, value);

            }
        }
        public bool ComRectangleIsVisible
        {
            get
            {
                return this._comRectangleIsVisible;
            }
            set
            {
                this.SetProperty(ref this._comRectangleIsVisible, value);

            }
        }

        public double IndRectangleX
        {
            get
            {
                return this._indRectangleX;
            }
            set
            {
                this.SetProperty(ref this._indRectangleX, value);

            }
        }
        public double IndRectangleY
        {
            get
            {
                return this._indRectangleY;
            }
            set
            {
                this.SetProperty(ref this._indRectangleY, value);

            }
        }
        public double IndRectangleHeight
        {
            get
            {
                return this._indRectangleHeight;
            }
            set
            {
                this.SetProperty(ref this._indRectangleHeight, value);

            }
        }
        public double IndRectangleWidth
        {
            get
            {
                return this._indRectangleWidth;
            }
            set
            {
                this.SetProperty(ref this._indRectangleWidth, value);

            }
        }
        public bool IndRectangleIsVisible
        {
            get
            {
                return this._indRectangleIsVisible;
            }
            set
            {
                this.SetProperty(ref this._indRectangleIsVisible, value);

            }
        }
    }
}
