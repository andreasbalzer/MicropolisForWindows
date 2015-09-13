using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Engine;
using Micropolis.Common;
using Microsoft.ApplicationInsights;

namespace Micropolis.ViewModels
{
    public class ToolbarViewModel : BindableBase
    {
        private MainGamePageViewModel _mainPageViewModel;
        private Dictionary<MicropolisTool, ToolbarButtonViewModel> _toolBtns;

        private double _toolHeight;
        private double _toolWidth;

        private ToolBarMode _mode;

        /// <summary>
        /// Height
        /// </summary>
        public double ToolHeight
        {
            get { return _toolHeight; }
            set
            {
                SetProperty(ref _toolHeight, value);
            }
        }

        /// <summary>
        /// Width
        /// </summary>
        public double ToolWidth
        {
            get { return _toolWidth; }
            set
            {
                SetProperty(ref _toolWidth, value);
            }
        }

        /// <summary>
        /// Mode of the toolbar.
        /// </summary>
        public ToolBarMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                MakeToolbar();
            }
        }

        /// <summary>
        /// Initiates a new instance of this Toolbar control.
        /// </summary>
        public ToolbarViewModel()
        {
            ToolBarItems=new ObservableCollection<ToolbarButtonViewModel>();
            _mode = ToolBarMode.NORMAL;
            _toolBtns = new Dictionary<MicropolisTool, ToolbarButtonViewModel>();
        }


        /// <summary>
        /// Sets up this instance after basic initalization.
        /// </summary>
        /// <param name="mainPageViewModel">Reference to main page</param>
        public void SetUpAfterBasicInit(MainGamePageViewModel mainPageViewModel)
        {
            _mainPageViewModel = mainPageViewModel;
            MakeToolbar();
        }

        public ObservableCollection<ToolbarButtonViewModel> ToolBarItems { get; set; }


        private bool _toolBarIsVisible;
        public bool ToolBarIsVisible { get { return _toolBarIsVisible; } set { SetProperty(ref _toolBarIsVisible, value); } }

        /// <summary>
        ///     Makes the toolbar with tools to use on map.
        /// </summary>
        /// <returns>the toolbar as stackpanel</returns>
        private void MakeToolbar()
        {
            _toolBtns.Clear();
            ToolBarItems.Clear();
            if (_mode == ToolBarMode.NORMAL || _mode == ToolBarMode.WIDE)
            {
                ToolBarIsVisible = true;
            }
            else if (_mode == ToolBarMode.FLYOUT)
            {
                ToolBarIsVisible = false;
            }
            

            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["EMPTY"]));

            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["BULLDOZER"]));
            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["WIRE"]));
            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["PARK"]));

            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["ROADS"]));
            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["RAIL"]));

            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["RESIDENTIAL"]));
            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["COMMERCIAL"]));
            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["INDUSTRIAL"]));

            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["FIRE"]));
            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["QUERY"]));
            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["POLICE"]));

            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["POWERPLANT"]));
            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["NUCLEAR"]));

            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["STADIUM"]));
            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["SEAPORT"]));

            ToolBarItems.Add(MakeToolBtn(MicropolisTools.MicropolisTool["AIRPORT"]));
        }

        /// <summary>
        ///     Makes a toolbar button for the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>the RadioButton</returns>
        private ToolbarButtonViewModel MakeToolBtn(MicropolisTool tool)
        {
            String iconName = Strings.ContainsKey("tool." + tool.Name + ".icon")
                ? "ms-appx:///Assets/resources/images/GameUI" + Strings.GetString("tool." + tool.Name + ".icon")
                : "ms-appx:///Assets/graphics/tools/" + tool.Name.ToLower() + ".png";
            String iconSelectedName = Strings.ContainsKey("tool." + tool.Name + ".selected_icon")
                ? "ms-appx:///Assets/resources/images/GameUI" + Strings.GetString("tool." + tool.Name + ".selected_icon")
                : iconName;
            String tipText = Strings.ContainsKey("tool." + tool.Name + ".tip")
                ? Strings.GetString("tool." + tool.Name + ".tip")
                : tool.Name;


            ToolbarButtonViewModel btn;

            if (_mode == ToolBarMode.NORMAL)
            {
                btn = new ToolbarButtonViewModel { Width = 32, Height = 32 };
                ToolWidth = Double.NaN;
                ToolHeight = Double.NaN;
            }
            else if (_mode == ToolBarMode.FLYOUT || _mode == ToolBarMode.WIDE)
            {
                btn = new ToolbarButtonViewModel { Width = 64, Height = 64 };
                ToolWidth = Double.NaN;
                ToolHeight = Double.NaN;
            }
            else
            {
                btn = new ToolbarButtonViewModel { Width = 32, Height = 32 };
                ToolWidth = Double.NaN;
                ToolHeight = Double.NaN;
            }



            var imageSource = new BitmapImage(new Uri(iconName, UriKind.RelativeOrAbsolute));
            
            btn.UncheckedStateImageSource = imageSource;

            var imageSourceSelected = new BitmapImage(new Uri(iconSelectedName, UriKind.RelativeOrAbsolute));
            
            btn.CheckedStateImageSource = imageSourceSelected;

            btn.ToolTip = tipText;  
            
            btn.ClickCommand = new DelegateCommand(() =>
            {
                try { 
                    TelemetryClient telemetry = new TelemetryClient();

                    telemetry.TrackEvent("ToolbarToolUsed"+tool.Name);
                }
                catch (Exception) { }

                SelectTool(tool); 
                if (_mode == ToolBarMode.FLYOUT)
                {
                    ToolBarIsVisible = false;
                }
            });
            _toolBtns.Add(tool, btn);
            return btn;
        }

        

        /// <summary>
        ///     Selects the tool specified.
        /// </summary>
        /// <param name="newTool">The new tool.</param>
        private void SelectTool(MicropolisTool newTool)
        {
            _toolBtns[newTool].IsChecked = true;
            if (newTool == _mainPageViewModel.CurrentTool)
            {
                return;
            }

            if (_mainPageViewModel.CurrentTool != null)
            {
                _toolBtns[_mainPageViewModel.CurrentTool].IsChecked = false;
            }

            _mainPageViewModel.SelectTool(newTool);
        }

        /// <summary>
        /// Fired when toolbar expand button clicked.
        /// </summary>
        private void Expand()
        {
            if (ToolBarIsVisible == true)
            {
                ToolBarIsVisible = false;
            }
            else
            {
                ToolBarIsVisible = true;
            }
             
        }
    }
}
