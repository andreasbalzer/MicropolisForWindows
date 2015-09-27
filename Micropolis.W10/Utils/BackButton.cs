using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Micropolis.Utils
{
    public static class BackButton
    {
        public static void RegisterBackButton(Frame frame)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                if (frame.CanGoBack)
                {
                    frame.GoBack();
                    a.Handled = true;
                }
            };
        }
    }
}
