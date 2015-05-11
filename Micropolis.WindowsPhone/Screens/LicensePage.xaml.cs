using System;
using System.Linq;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;
using Micropolis.Model.Entities;
// Die Elementvorlage "Standardseite" ist unter http://go.microsoft.com/fwlink/?LinkId=234237 dokumentiert.

namespace Micropolis.Screens
{
    /// <summary>
    ///     Eine Standardseite mit Eigenschaften, die die meisten Anwendungen aufweisen.
    /// </summary>
    public sealed partial class LicensePage : Page
    {
        public LicensePage()
        {
            InitializeComponent();
            LicenseTextTB.Inlines.Clear();
            LicenseTextTB.Inlines.Add(new Run {Text = Strings.GetString("license.P1")});
            B1.Content = Strings.GetString("license.B1");
            pageTitle.Text = Strings.GetString("license.Title");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Frame.BackStack.Count >= 2)
            {
                Frame.BackStack.RemoveAt(Frame.BackStack.Count - 2);
                    // removes the page before this, e.g. main menu page
            }
        }

        private async void GnuGplButton_Clicked(object sender, RoutedEventArgs e)
        {
            // The URI to launch
            var uriToLaunch = @"http://www.gnu.org/licenses";

            // Create a Uri object from a URI string 
            var uri = new Uri(uriToLaunch);

            // Launch the URI

            // Launch the URI
            var success = await Launcher.LaunchUriAsync(uri);

            if (success)
            {
                // URI launched
            }
            else
            {
                // URI launch failed
                var dialog = new MessageDialog(Strings.GetString("license.InternetError"));
                await dialog.ShowAsync();
            }
        }

        private async void B1_OnClick(object sender, RoutedEventArgs e)
        {
            var appFolder = ApplicationData.Current.LocalFolder;
            await appFolder.CreateFileAsync("licenseAccepted.txt",
                CreationCollisionOption.ReplaceExisting);

            var current = (ISupportsAppCommands) Application.Current;
            var skipCommand = current.AppCommands.FirstOrDefault(s => s.Instruction == AppCommands.SKIPMENU);

            var skipMenu = skipCommand != null;
            if (skipMenu)
            {
                current.AppCommands.Remove(skipCommand);
                Frame.Navigate(typeof (MainGamePage));
            }
            else
            {
                Frame.Navigate(typeof (MainMenuPage));
            }
        }
    }
}