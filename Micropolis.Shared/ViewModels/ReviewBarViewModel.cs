using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Micropolis.Common;

namespace Micropolis.ViewModels
{
    public class ReviewBarViewModel : BindableBase
    {
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
            set { SetProperty(ref _feedbackIsVisible, value); }
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
        }

        private void OpenStoreRatingPage()
        {
            Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp"));
            Disable();
        }

        private void SendFeedback()
        {
            var feedbackTitle = Strings.GetString("feedback.title");
            var feedbackBody = Strings.GetString("feedback.body");
            Launcher.LaunchUriAsync(
                new Uri("mailto:micropolis@andreas-balzer.de?subject=" + feedbackTitle + "&body=" + feedbackBody));
            Postpone();
        }

        private async Task CheckForPreviousFeedback()
        {
            var folder = ApplicationData.Current.RoamingFolder;

#if WINDOWS_PHONE_APP
            try
            {
                var previousFeedbackFile = await folder.GetFileAsync("feedbackSent.txt");
                ShowFeedbackOrDecrementCounter(previousFeedbackFile);
            }
            catch {
                FeedbackIsVisible = false;
                CreateFeedbackFile();
            }
#else
            var previousFeedbackFile = await folder.TryGetItemAsync("feedbackSent.txt");
            var fileExists = previousFeedbackFile != null;
            if (!fileExists)
            {
                FeedbackIsVisible = false;
                CreateFeedbackFile();
            }
            else
            {
                ShowFeedbackOrDecrementCounter(previousFeedbackFile);
            }
#endif
        }

        private async Task CreateFeedbackFile()
        {
            var folder = ApplicationData.Current.RoamingFolder;
            var file = await folder.CreateFileAsync("feedbackSent.txt");
            FileIO.WriteTextAsync(file, INITIALTIMEUNTILFEEDBACK);
        }

        private async Task ShowFeedbackOrDecrementCounter(IStorageItem previousFeedbackFile)
        {
            bool previousFeedback;
            var content = await FileIO.ReadTextAsync((IStorageFile) previousFeedbackFile);
            previousFeedback = content == SHOWFEEDBACK;
            if (!previousFeedback)
            {
                FeedbackIsVisible = true;
            }
            else
            {
                if (content != DONEFEEDBACK)
                {
                    var newContent = (Int32.Parse(content) - 1).ToString();
                    FileIO.WriteTextAsync((IStorageFile) previousFeedbackFile, newContent);
                }
                FeedbackIsVisible = false;
            }
        }

        private async Task Disable()
        {
            var folder = ApplicationData.Current.RoamingFolder;
            var file = await folder.GetFileAsync("feedbackSent.txt");
            FileIO.WriteTextAsync(file, DONEFEEDBACK);
        }

        private async Task Postpone()
        {
            var folder = ApplicationData.Current.RoamingFolder;
            var file = await folder.GetFileAsync("feedbackSent.txt");
            FileIO.WriteTextAsync(file, POSTPONETIMEUNTILFEEDBACK);
        }
    }
}