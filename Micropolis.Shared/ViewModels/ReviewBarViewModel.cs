using System;
using System.Threading.Tasks;
using Windows.Storage;
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
        private readonly string POSTPONETIMEUNTILFEEDBACK = "20";
        private readonly string SHOWFEEDBACK = "0";
        private bool _feedbackIsVisible;
        private string _sendFeedbackText;
        private bool _sorryMessageIsVisible;
        private string _sorryMessageText;

        public ReviewBarViewModel()
        {
            try { 
            _telemetry = new TelemetryClient();
            }
            catch (Exception) { }

            SendFeedbackText = Strings.GetString("feedback.sendFeedbackText");
            SorryMessageText = Strings.GetString("feedback.sorryMessageText");
            Star1Command = new DelegateCommand(ShowFeedback);
            Star2Command = new DelegateCommand(ShowFeedback);
            Star3Command = new DelegateCommand(ShowFeedback);
            Star4Command = new DelegateCommand(OpenStoreRatingPage);
            Star5Command = new DelegateCommand(OpenStoreRatingPage);
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
                    try { 
                    _telemetry.TrackEvent("ReviewShowReviewBar");
                    }
                    catch (Exception) { }
                }
            }
        }

        public string SorryMessageText
        {
            get { return _sorryMessageText; }
            set { SetProperty(ref _sorryMessageText, value); }
        }

        public bool SorryMessageIsVisible
        {
            get { return _sorryMessageIsVisible; }
            set { SetProperty(ref _sorryMessageIsVisible, value); }
        }

        public DelegateCommand Star1Command { get; private set; }
        public DelegateCommand Star2Command { get; private set; }
        public DelegateCommand Star3Command { get; private set; }
        public DelegateCommand Star4Command { get; private set; }
        public DelegateCommand Star5Command { get; private set; }
        public DelegateCommand SendFeedbackCommand { get; private set; }

        private void ShowFeedback()
        {
            SorryMessageIsVisible = true;
            try { 
            _telemetry.TrackEvent("ReviewShowFeedbackWithSorryMessage");
            }
            catch (Exception) { }
        }

        private void OpenStoreRatingPage()
        {
            try { 
            _telemetry.TrackEvent("ReviewOpenRatingPage");
            }
            catch (Exception) { }

            Launcher.LaunchUriAsync(
                new Uri("ms-windows-store:review?PFN=62155AndreasBalzer.MicropolisforWindows_rqaffv28461by",
                    UriKind.Absolute));
            Disable();
        }

        private void SendFeedback()
        {
            try { 
            _telemetry.TrackEvent("ReviewSendFeedback");
            }
            catch (Exception) { }

            var feedbackTitle = Strings.GetString("feedback.title");
            var feedbackBody = Strings.GetString("feedback.body");
            Launcher.LaunchUriAsync(
                new Uri("mailto:micropolis@andreas-balzer.de?subject=" + feedbackTitle + "&body=" + feedbackBody,
                    UriKind.Absolute));
            Postpone();
        }

        private async Task CheckForPreviousFeedback()
        {
            var folder = ApplicationData.Current.RoamingFolder;

#if WINDOWS_PHONE_APP
            try
            {
                var previousFeedbackFile = await folder.GetFileAsync("feedbackSent.txt");
                ShowFeedbackOrDecrementCounter();
            }
            catch {
                FeedbackIsVisible = false;
                CreateFeedbackSetting();
            }
#else

            var fileExists = Prefs.ContainsKey("feedbackSent");
            if (!fileExists)
            {
                FeedbackIsVisible = false;
                CreateFeedbackSetting();
            }
            else
            {
                ShowFeedbackOrDecrementCounter();
            }
#endif
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
            if (showFeedback && false)
            {
                FeedbackIsVisible = true;
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