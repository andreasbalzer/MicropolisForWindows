using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Popups;
using Micropolis.Common;
using Microsoft.ApplicationInsights;

namespace Micropolis.ViewModels
{
    public class ReviewBarViewModel : BindableBase
    {
        private readonly TelemetryClient _telemetry;
        private readonly string DONEFEEDBACK = "disabled";
        private readonly string INITIALTIMEUNTILFEEDBACK = "5";
        private readonly string POSTPONETIMEUNTILFEEDBACK = "11";
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
            FeedbackMessageText = Strings.GetString("feedback.feedbackRateMessageText");
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

            if (content != DONEFEEDBACK)
            {
                content = (Int32.Parse(content) - 1).ToString();
                Prefs.PutString("feedbackSent", content);
            }

            showFeedback = content == SHOWFEEDBACK;
            if (showFeedback)
            {
                FeedbackIsVisible = true;

                Postpone();
                return; // disable dialogs for now.

                var dlg = new MessageDialog(Strings.GetString("feedback.rateMessageText"));
                dlg.Commands.Add(new UICommand(Strings.GetString("feedback.rateText"), null, "rate"));
                dlg.Commands.Add(new UICommand(Strings.GetString("feedback.cancel"), null, "cancel"));

                var result = "";

                try
                {
                    result = (string) (await dlg.ShowAsync()).Id;
                    if (result == "rate")
                    {
                        OpenStoreRatingPage();
                    }
                    if (result == "rate" || result == "cancel")
                    {
                        dlg = new MessageDialog(Strings.GetString("feedback.feedbackMessageText"));
                        dlg.Commands.Add(new UICommand(Strings.GetString("feedback.sendFeedbackText"), null, "feedback"));
                        //dlg.Commands.Add(new UICommand(SendFeedbackText, null, "feedback"));
                        dlg.Commands.Add(new UICommand(Strings.GetString("feedback.cancel"), null, "cancel"));

                        result = "";


                        result = (string) (await dlg.ShowAsync()).Id;
                        if (result == "feedback")
                        {
                            SendFeedback();
                        }
                        else if (result == "cancel")
                        {

                        }
                    }
                }
                catch (Exception)
                {
                    //	this may happen if any other modal window is shown at the moment (ie, Windows query about running application background task)
                }

                Postpone();
            }
            else
            {
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