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

namespace Micropolis.ViewModels
{
    public class ToolbarViewModel : BindableBase
    {
        private MainGamePageViewModel _mainPageViewModel;
        private Dictionary<MicropolisTool, ToolbarButtonViewModel> _toolBtns;

        private ToolBarMode _mode;

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
            ExpandCommand = new DelegateCommand(Expand);
            ToolBarItems=new ObservableCollection<ToolbarButtonViewModel>();
            _mode = ToolBarMode.NORMAL;
            ToolRowHeight = ToolRowHeight = new GridLength(1, GridUnitType.Star);
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


        private bool _toolBarExpandButtonIsVisible;
        public bool ToolBarExpandButtonIsVisible { get { return _toolBarExpandButtonIsVisible; } set { SetProperty(ref _toolBarExpandButtonIsVisible, value); } }

        private bool _toolBarIsVisible;
        public bool ToolBarIsVisible { get { return _toolBarIsVisible; } set { SetProperty(ref _toolBarIsVisible, value); } }


        private GridLength _toolRowHeight;
        public GridLength ToolRowHeight { get { return _toolRowHeight; } set { SetProperty(ref _toolRowHeight, value); } }


        /// <summary>
        ///     Makes the toolbar with tools to use on map.
        /// </summary>
        /// <returns>the toolbar as stackpanel</returns>
        private void MakeToolbar()
        {
            _toolBtns.Clear();
            ToolBarItems.Clear();
            ToolRowHeight = new GridLength(1, GridUnitType.Star);
            if (_mode == ToolBarMode.NORMAL || _mode == ToolBarMode.WIDE)
            {
                ToolBarExpandButtonIsVisible = false;
                ToolBarIsVisible = true;
            }
            else if (_mode == ToolBarMode.FLYOUT)
            {
                ToolBarExpandButtonIsVisible = true;
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
                ? "ms-appx:///resources/images/GameUI" + Strings.GetString("tool." + tool.Name + ".icon")
                : "ms-appx:///graphics/tools/" + tool.Name.ToLower() + ".png";
            String iconSelectedName = Strings.ContainsKey("tool." + tool.Name + ".selected_icon")
                ? "ms-appx:///resources/images/GameUI" + Strings.GetString("tool." + tool.Name + ".selected_icon")
                : iconName;
            String tipText = Strings.ContainsKey("tool." + tool.Name + ".tip")
                ? Strings.GetString("tool." + tool.Name + ".tip")
                : tool.Name;


            ToolbarButtonViewModel btn;

            if (_mode == ToolBarMode.NORMAL)
            {
                btn = new ToolbarButtonViewModel { Width = 32, Height = 32 };
            }
            else if (_mode == ToolBarMode.FLYOUT || _mode == ToolBarMode.WIDE)
            {
                btn = new ToolbarButtonViewModel { Width = 64, Height = 64 };
            }
            else
            {
                btn = new ToolbarButtonViewModel { Width = 32, Height = 32 };
            }

            

            var imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(iconName, UriKind.RelativeOrAbsolute))
            };

            btn.UncheckedStateImageBrush = imageBrush;

            var imageBrushSelected = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(iconSelectedName, UriKind.RelativeOrAbsolute))
            };
            btn.CheckedStateImageBrush = imageBrushSelected;

           

            //var tip = new ToolTip {Content = tipText};
            //ToolTipService.SetToolTip(btn, tip);
            //ToDo: re-add ToolTip

            btn.ClickCommand = new DelegateCommand(() =>
            {
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
                ToolBarExpandButtonText = "▼";
                ToolRowHeight = new GridLength(0,GridUnitType.Auto);
            }
            else
            {
                ToolBarIsVisible = true;
                ToolBarExpandButtonText = "▲";
                ToolRowHeight = new GridLength(1, GridUnitType.Star);

            }
             
        }

        private DelegateCommand _expandCommand;
        public DelegateCommand ExpandCommand { get { return _expandCommand; } set { SetProperty(ref _expandCommand, value); } }


        private string _toolBarExpandButtonText;
        public string ToolBarExpandButtonText { get { return _toolBarExpandButtonText; } set { SetProperty(ref _toolBarExpandButtonText, value); } }

    }
}
