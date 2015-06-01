
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Popups;
using Micropolis.Common;
using Microsoft.ApplicationInsights;

namespace Micropolis.ViewModels
{
    public class RatingFlyoutViewModel : BindableBase
    {
        private readonly TelemetryClient _telemetry;
        private string _feedbackMessageText;
        private string _rateText;
        private string _sendFeedbackText;

        public RatingFlyoutViewModel()
        {
            try
            {
                _telemetry = new TelemetryClient();
            }
            catch (Exception)
            {
            }

            SendFeedbackText = Strings.GetString("feedback.sendFeedbackText");
            RateText = Strings.GetString("feedback.rateText");
            FeedbackMessageText = Strings.GetString("feedback.feedbackRateMessageText");
            RateCommand = new DelegateCommand(OpenStoreRatingPage);
            SendFeedbackCommand = new DelegateCommand(SendFeedback);
        }

        public string SendFeedbackText
        {
            get { return _sendFeedbackText; }
            set { SetProperty(ref _sendFeedbackText, value); }
        }



        public string RateText
        {
            get { return _rateText; }
            set { SetProperty(ref _rateText, value); }
        }

        public string FeedbackMessageText
        {
            get { return _feedbackMessageText; }
            set { SetProperty(ref _feedbackMessageText, value); }
        }

        public DelegateCommand RateCommand { get; private set; }
        public DelegateCommand SendFeedbackCommand { get; private set; }

        private void OpenStoreRatingPage()
        {
            try
            {
                _telemetry.TrackEvent("RatingFlyoutOpenRatingPage");
            }
            catch (Exception)
            {
            }

#if WINDOWS_PHONE_APP
            Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
#else
            Launcher.LaunchUriAsync(
                new Uri("ms-windows-store:review?PFN=62155AndreasBalzer.MicropolisforWindows_rqaffv28461by",
                    UriKind.Absolute));
#endif
        }

        private void SendFeedback()
        {
            try
            {
                _telemetry.TrackEvent("RatingFlyoutSendFeedback");
            }
            catch (Exception)
            {
            }

            var feedbackTitle = Strings.GetString("feedback.title");
            var feedbackBody = Strings.GetString("feedback.body");
            Launcher.LaunchUriAsync(
                new Uri("mailto:micropolis@andreas-balzer.de?subject=" + feedbackTitle + "&body=" + feedbackBody,
                    UriKind.Absolute));
        }
    }
}


