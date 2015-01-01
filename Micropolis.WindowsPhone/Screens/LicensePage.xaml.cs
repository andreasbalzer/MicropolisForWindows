using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Documents;
using Micropolis.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Standardseite" ist unter http://go.microsoft.com/fwlink/?LinkId=234237 dokumentiert.
using Micropolis.Model.Entities;

namespace Micropolis.Screens
{
    /// <summary>
    /// Eine Standardseite mit Eigenschaften, die die meisten Anwendungen aufweisen.
    /// </summary>
    public sealed partial class LicensePage : Page
    {

      

        public LicensePage()
        {
            this.InitializeComponent();
            LicenseTextTB.Inlines.Clear();
            LicenseTextTB.Inlines.Add(new Run(){Text=Strings.GetString("license.P1")});
            B1.Content = Strings.GetString("license.B1");
            pageTitle.Text = Strings.GetString("license.Title");
        }


      

        private async void GnuGplButton_Clicked(object sender, RoutedEventArgs e)
        {
            // The URI to launch
            string uriToLaunch = @"http://www.gnu.org/licenses";

            // Create a Uri object from a URI string 
            var uri = new Uri(uriToLaunch);

            // Launch the URI

            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);

            if (success)
            {
                // URI launched
            }
            else
            {
                // URI launch failed
                MessageDialog dialog = new MessageDialog(Strings.GetString("license.InternetError"));
                await dialog.ShowAsync();
            }


        }

        private async void B1_OnClick(object sender, RoutedEventArgs e)
        {
            StorageFolder appFolder = ApplicationData.Current.LocalFolder;
            await appFolder.CreateFileAsync("licenseAccepted.txt",
                CreationCollisionOption.ReplaceExisting);

            ISupportsAppCommands current = (ISupportsAppCommands)App.Current;
            AppCommand skipCommand = current.AppCommands.FirstOrDefault(s => s.Instruction == AppCommands.SKIPMENU);

            bool skipMenu = skipCommand != null;
            if (skipMenu)
            {
                current.AppCommands.Remove(skipCommand);
                Frame.Navigate(typeof(MainGamePage));
            }
            else
            {
                Frame.Navigate(typeof(MainMenuPage));
            }
        }
    }
}
