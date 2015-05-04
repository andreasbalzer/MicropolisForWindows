using System;
using System.Threading.Tasks;
using Windows.System;
using Micropolis.Common;
using Microsoft.ApplicationInsights;

namespace Micropolis.ViewModels
{
    public class ReviewBarViewModel : BindableBase
    {
        private readonly TelemetryClient _telemetry;
        private readonly string DONEFEEDBACK = "disabled";
        private readonly string INITIALTIMEUNTILFEEDBACK = "5";
        private readonly string POSTPONETIMEUNTILFEEDBACK = "10";
        private readonly string SHOWFEEDBACK = "1";
        private bool _feedbackIsVisible;
        private string _feedbackMessageText;
        private string _rateText;
        private string _sendFeedbackText;

        public ReviewBarViewModel()
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
            FeedbackMessageText = Strings.GetString("feedback.feedbackMessageText");
            RateCommand = new DelegateCommand(OpenStoreRatingPage);
            SendFeedbackCommand = new DelegateCommand(SendFeedback);
            CheckForPreviousFeedback();
        }

        public string SendFeedbackText
        {
            get { return _sendFeedbackText; }
            set { SetProperty(ref _sendFeedbackText, value); }
        }

        public bool FeedbackIsVisible
        {
            get { return _feedbackIsVisible; }
            set
            {
                SetProperty(ref _feedbackIsVisible, value);
                if (value)
                {
                    try
                    {
                        _telemetry.TrackEvent("ReviewShowReviewBar");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
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
                _telemetry.TrackEvent("ReviewOpenRatingPage");
            }
            catch (Exception)
            {
            }

            Launcher.LaunchUriAsync(
                new Uri("ms-windows-store:review?PFN=62155AndreasBalzer.MicropolisforWindows_rqaffv28461by",
                    UriKind.Absolute));
            Disable();
        }

        private void SendFeedback()
        {
            try
            {
                _telemetry.TrackEvent("ReviewSendFeedback");
            }
            catch (Exception)
            {
            }

            var feedbackTitle = Strings.GetString("feedback.title");
            var feedbackBody = Strings.GetString("feedback.body");
            Launcher.LaunchUriAsync(
                new Uri("mailto:micropolis@andreas-balzer.de?subject=" + feedbackTitle + "&body=" + feedbackBody,
                    UriKind.Absolute));
            Disable();
        }

        private async Task CheckForPreviousFeedback()
        {
            var feedbackSent = Prefs.ContainsKey("feedbackSent");
            if (!feedbackSent)
            {
                FeedbackIsVisible = false;
                CreateFeedbackSetting();
            }
            else
            {
                ShowFeedbackOrDecrementCounter();
            }
        }

        private async Task CreateFeedbackSetting()
        {
            Prefs.PutString("feedbackSent", INITIALTIMEUNTILFEEDBACK);
        }

        private async Task ShowFeedbackOrDecrementCounter()
        {
            bool showFeedback;
            var content = Prefs.GetString("feedbackSent", INITIALTIMEUNTILFEEDBACK);
            showFeedback = content == SHOWFEEDBACK;
            if (showFeedback)
            {
                FeedbackIsVisible = true;
                Postpone();
            }
            else
            {
                if (content != DONEFEEDBACK)
                {
                    var newContent = (Int32.Parse(content) - 1).ToString();
                    Prefs.PutString("feedbackSent", newContent);
                }
                FeedbackIsVisible = false;
            }
        }

        private async Task Disable()
        {
            Prefs.PutString("feedbacksent", DONEFEEDBACK);
        }

        private async Task Postpone()
        {
            Prefs.PutString("feedbackSent", POSTPONETIMEUNTILFEEDBACK);
        }
    }
}