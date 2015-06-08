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



        public bool IsShowTopArrow
        {
            get { return (bool)GetValue(IsShowTopArrowProperty); }
            set { SetValue(IsShowTopArrowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowTopArrow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowTopArrowProperty =
            DependencyProperty.Register("IsShowTopArrow", typeof(bool), typeof(MenuButton), new PropertyMetadata(false));



        public bool IsShowBottomArrow
        {
            get { return (bool)GetValue(IsShowBottomArrowProperty); }
            set { SetValue(IsShowBottomArrowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowBottomArrow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowBottomArrowProperty =
            DependencyProperty.Register("IsShowBottomArrow", typeof(bool), typeof(MenuButton), new PropertyMetadata(false));



        public bool IsShowImage
        {
            get { return (bool)GetValue(IsShowImageProperty); }
            set { SetValue(IsShowImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowImageProperty =
            DependencyProperty.Register("IsShowImage", typeof(bool), typeof(MenuButton), new PropertyMetadata(false));



        public bool IsShowText
        {
            get { return (bool)GetValue(IsShowTextProperty); }
            set { SetValue(IsShowTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowTextProperty =
            DependencyProperty.Register("IsShowText", typeof(bool), typeof(MenuButton), new PropertyMetadata(true));


    }

}
