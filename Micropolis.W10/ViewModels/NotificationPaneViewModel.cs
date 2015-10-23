using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Engine;
using Micropolis.Common;
using Microsoft.ApplicationInsights;

namespace Micropolis.ViewModels
{
    public class NotificationPaneViewModel : BindableBase
    {
        public NotificationPaneViewModel(MicropolisDrawingAreaViewModel drawingAreaViewModel, ScrollViewer drawingAreaScrollViewer)
        {
            Messages=new ObservableCollection<string>();
            _drawingAreaViewModel = drawingAreaViewModel;
            _drawingAreaScrollViewer = drawingAreaScrollViewer;
            try { 
            _telemetry = new TelemetryClient();
            }
            catch (Exception) { }
        }

        private static readonly Size VIEWPORT_SIZE = new Size(180, 180);
        private static readonly SolidColorBrush QUERY_COLOR = new SolidColorBrush(Color.FromArgb(255, 255, 165, 0));
        private MainGamePageViewModel _mainPageViewModel;

        /// <summary>
        /// Sets up this instance after basic initialization.
        /// </summary>
        /// <param name="mainPage"></param>
        public void SetUpAfterBasicInit(MainGamePageViewModel mainPageViewModel)
        {
            _mainPageViewModel = mainPageViewModel;
            DismissButtonText = Strings.GetString("notification.dismiss");
            DismissCommand = new DelegateCommand(() => { OnDismissClicked(); });

            GoButtonText = Strings.GetString("notification.go_btn");
            GoCommand = new DelegateCommand(() => { OnGoClicked(); });

            _drawingAreaViewModel.SetUpAfterBasicInit(_mainPageViewModel.Engine, _mainPageViewModel);
            _drawingAreaViewModel.RepaintNow();
        }

        private string _dismissButtonText;
        private string _goButtonText;
        public string DismissButtonText { get { return _dismissButtonText; } set { SetProperty(ref _dismissButtonText, value); } }
        public string GoButtonText { get { return _goButtonText; } set { SetProperty(ref _goButtonText, value); } }

        private DelegateCommand _dismissCommand;
        private DelegateCommand _goCommand;
        public DelegateCommand DismissCommand { get { return _dismissCommand; } set { SetProperty(ref _dismissCommand, value); } }
        public DelegateCommand GoCommand { get { return _goCommand; } set { SetProperty(ref _goCommand, value); } }


        /// <summary>
        /// Called when user clicked the dismiss button to close the pane.
        /// </summary>
        private void OnDismissClicked()
        {
            try { 
            _telemetry.TrackEvent("NotificationPaneDismissClicked");
            }
            catch (Exception) { }

            _mainPageViewModel.HideNotificationPanel();
            ImageIsVisible = true;
            _location = null;
        }


        /// <summary>
        /// Called when user clicked the go button to navigate to the source of notification on the map.
        /// </summary>
        private void OnGoClicked()
        {
            try
            {
                _telemetry.TrackEvent("NotificationPaneGoClicked");
            }
            catch (Exception) { }

            if (_location != null)
            {
                _mainPageViewModel.Centering(_location);
            }
        }

        private MicropolisDrawingAreaViewModel _drawingAreaViewModel;

        private ScrollViewer _drawingAreaScrollViewer;

        void SetPicture(Engine.Micropolis engine, int xpos, int ypos)
        {
            _drawingAreaViewModel.SetEngine(engine);
            Rect r = _drawingAreaViewModel.GetTileBoundsAsRect(xpos, ypos);

            _drawingAreaViewModel.Clip = new Rect(r.X + r.Width / 2 - VIEWPORT_SIZE.Width / 2,
                                                    r.Y + r.Height / 2 - VIEWPORT_SIZE.Height / 2,
                                                    VIEWPORT_SIZE.Width,
                                                    VIEWPORT_SIZE.Height
                                                    );
            _drawingAreaViewModel.RepaintNow();
            App.MainPageReference.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _drawingAreaScrollViewer.ScrollToHorizontalOffset(r.X - (VIEWPORT_SIZE.Width / 2));
                _drawingAreaScrollViewer.ScrollToVerticalOffset(r.Y - (VIEWPORT_SIZE.Height / 2));
            });

        }


        /// <summary>
        /// Shows the specified message for the specified map coordinates.
        /// </summary>
        /// <param name="msg">message to show</param>
        /// <param name="xpos">xpos in map</param>
        /// <param name="ypos">ypos in map</param>
        public void ShowMessage(MicropolisMessage msg, int xpos, int ypos)
        {
            GoButtonIsVisible = true;
            _location = new CityLocation(xpos, ypos);
            SetPicture(_mainPageViewModel.Engine, xpos, ypos);
            ShowMessage(msg, false);
        }

        /// <summary>
        /// Shows the specified message for the specified map coordinates.
        /// </summary>
        /// <param name="msg">message to show</param>
        /// <param name="xpos">xpos in map</param>
        /// <param name="ypos">ypos in map</param>
        public void ShowMessage(MicropolisMessage msg, bool hideImage = true)
        {
            try
            {
                _telemetry.TrackEvent("NotificationPaneMessageShown" + msg.Name);
            }
            catch (Exception) { }

            if (hideImage)
            {
                ImageIsVisible = false;
            }

            if (InfoPaneIsVisible == true)
            {
                InfoPaneIsVisible = false;
            }

            HeaderTextBlockText = Strings.GetString(msg.Name + ".title");
            HeaderSPBackground = new SolidColorBrush(ColorParser.ParseColor(Strings.GetString(msg.Name + ".color")));

            Messages.Clear();
            string[] messages = Strings.GetString(msg.Name + ".detail").Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string message in messages)
            {
                Messages.Add(message);
            }

            DetailPaneIsVisible = true;
        }

        private SolidColorBrush _headerSPBackground;
        public SolidColorBrush HeaderSPBackground { get { return _headerSPBackground; } set { SetProperty(ref _headerSPBackground, value); } }

        private string _headerTextBlockText;
        public string HeaderTextBlockText { get { return _headerTextBlockText; } set { SetProperty(ref _headerTextBlockText, value); } }

        private bool _infoPaneIsVisible;
        public bool InfoPaneIsVisible { get { return _infoPaneIsVisible; } set { SetProperty(ref _infoPaneIsVisible, value); } }

        private bool _goButtonIsVisible;
        public bool GoButtonIsVisible { get { return _goButtonIsVisible; } set { SetProperty(ref _goButtonIsVisible, value); } }


        private bool _detailPaneIsVisible;
        public bool DetailPaneIsVisible { get { return _detailPaneIsVisible; } set { SetProperty(ref _detailPaneIsVisible, value); } }

        public ObservableCollection<string> Messages { get; set; } 

        /// <summary>
        /// Show zone status for specified coordinates and zonestatus
        /// </summary>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        /// <param name="zone"></param>
        public void ShowZoneStatus(int xpos, int ypos, ZoneStatus zone)
        {
            DetailPaneIsVisible = false;

            HeaderTextBlockText = Strings.GetString("notification.query_hdr");
            HeaderSPBackground = QUERY_COLOR;

            String buildingStr = zone.Building != -1 ? Strings.GetString("zone." + zone.Building) : "";
            String popDensityStr = Strings.GetString("status." + zone.PopDensity);
            String landValueStr = Strings.GetString("status." + zone.LandValue);
            String crimeLevelStr = Strings.GetString("status." + zone.CrimeLevel);
            String pollutionStr = Strings.GetString("status." + zone.Pollution);
            String growthRateStr = Strings.GetString("status." + zone.GrowthRate);

            SetPicture(_mainPageViewModel.Engine, xpos, ypos);
            InfoPaneIsVisible = true;
            GoButtonIsVisible = false;

            T1TextBlockText = Strings.GetString("notification.zone_lbl");
            BuildStrTextBlockText = buildingStr;
            NotificationDensityTextBlockText = Strings.GetString("notification.density_lbl");
            PopDensityStrTextBlockText = popDensityStr;
            NotificationValueTextBlockText = Strings.GetString("notification.value_lbl");
            LandValueStrTextBlockText = landValueStr;
            NotificationCrimeTextBlockText = Strings.GetString("notification.crime_lbl");
            CrimeLevelStrTextBlockText = crimeLevelStr;
            NotificationPollutionTextBlockText = Strings.GetString("notification.pollution_lbl");
            PollutionStrTextBlockText = pollutionStr;
            NotificationGrowthTextBlockText = Strings.GetString("notification.growth_lbl");
            GrowthRateStrTextBlockText = growthRateStr;
            _mainPageViewModel.ShowNotificationPanel();
        }

        private string _t1TextBlockText;
        public string T1TextBlockText { get { return _t1TextBlockText; } set { SetProperty(ref _t1TextBlockText, value); } }


        private string _buildStrTextBlockText;
        public string BuildStrTextBlockText { get { return _buildStrTextBlockText; } set { SetProperty(ref _buildStrTextBlockText, value); } }


        private string _popDensityStrTextBlockText;
        public string PopDensityStrTextBlockText { get { return _popDensityStrTextBlockText; } set { SetProperty(ref _popDensityStrTextBlockText, value); } }

        private string _notificationValueTextBlockText;
        public string NotificationValueTextBlockText { get { return _notificationValueTextBlockText; } set { SetProperty(ref _notificationValueTextBlockText, value); } }


        private string _notificationDensityTextBlockText;
        public string NotificationDensityTextBlockText { get { return _notificationDensityTextBlockText; } set { SetProperty(ref _notificationDensityTextBlockText, value); } }


        private string _landValueStrTextBlockText;
        public string LandValueStrTextBlockText { get { return _landValueStrTextBlockText; } set { SetProperty(ref _landValueStrTextBlockText, value); } }

        private string _notificationCrimeTextBlockText;
        public string NotificationCrimeTextBlockText { get { return _notificationCrimeTextBlockText; } set { SetProperty(ref _notificationCrimeTextBlockText, value); } }

        private string _crimeLevelStrTextBlockText;
        public string CrimeLevelStrTextBlockText { get { return _crimeLevelStrTextBlockText; } set { SetProperty(ref _crimeLevelStrTextBlockText, value); } }

        private string _notificationPollutionTextBlockText;
        public string NotificationPollutionTextBlockText { get { return _notificationPollutionTextBlockText; } set { SetProperty(ref _notificationPollutionTextBlockText, value); } }

        private string _pollutionStrTextBlockText;
        public string PollutionStrTextBlockText { get { return _pollutionStrTextBlockText; } set { SetProperty(ref _pollutionStrTextBlockText, value); } }

        private string _notificationGrowthTextBlockText;
        public string NotificationGrowthTextBlockText { get { return _notificationGrowthTextBlockText; } set { SetProperty(ref _notificationGrowthTextBlockText, value); } }

        private string _growthRateStrTextBlockText;
        private TelemetryClient _telemetry;
        private bool _imageIsVisible = true;
        private CityLocation _location;

        public string GrowthRateStrTextBlockText { get { return _growthRateStrTextBlockText; } set { SetProperty(ref _growthRateStrTextBlockText, value); } }

        public bool ImageIsVisible { get { return _imageIsVisible; } set { SetProperty(ref _imageIsVisible, value); } }
    }
}
