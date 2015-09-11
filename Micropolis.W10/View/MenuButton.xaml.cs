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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Micropolis.View
{
    public sealed partial class MenuButton : Button
    {
        public MenuButton() : base()
        {

        }
        
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(MenuButton), new PropertyMetadata(null));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MenuButton), new PropertyMetadata(String.Empty));



        public bool IsTopArrowVisible
        {
            get { return (bool)GetValue(IsTopArrowVisibleProperty); }
            set { SetValue(IsTopArrowVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTopArrowVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTopArrowVisibleProperty =
            DependencyProperty.Register("IsTopArrowVisible", typeof(bool), typeof(MenuButton), new PropertyMetadata(false));



        public bool IsBottomArrowVisible
        {
            get { return (bool)GetValue(IsBottomArrowVisibleProperty); }
            set { SetValue(IsBottomArrowVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBottomArrowVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBottomArrowVisibleProperty =
            DependencyProperty.Register("IsBottomArrowVisible", typeof(bool), typeof(MenuButton), new PropertyMetadata(false));



        public bool IsImageVisible
        {
            get { return (bool)GetValue(IsImageVisibleProperty); }
            set { SetValue(IsImageVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsImageVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsImageVisibleProperty =
            DependencyProperty.Register("IsImageVisible", typeof(bool), typeof(MenuButton), new PropertyMetadata(false));



        public bool IsTextVisible
        {
            get { return (bool)GetValue(IsTextVisibleProperty); }
            set { SetValue(IsTextVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTextVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextVisibleProperty =
            DependencyProperty.Register("IsTextVisible", typeof(bool), typeof(MenuButton), new PropertyMetadata(true));


    }

}
