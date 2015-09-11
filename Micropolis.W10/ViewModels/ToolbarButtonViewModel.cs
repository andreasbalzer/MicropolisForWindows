using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media;
using Micropolis.Common;

namespace Micropolis.ViewModels
{
    public class ToolbarButtonViewModel : BindableBase
    {

        private ImageSource _uncheckedStateImageSource;
        public ImageSource UncheckedStateImageSource
        {
            get { return _uncheckedStateImageSource; }
            set
            {
                SetProperty(ref _uncheckedStateImageSource, value);
                if (!IsChecked)
                {
                    CurrentStateImageSource = UncheckedStateImageSource;
                }
            }
        }

        private ImageSource _checkedStateImageSource;
        public ImageSource CheckedStateImageSource
        {
            get { return _checkedStateImageSource; }
            set
            {
                SetProperty(ref _checkedStateImageSource, value);

                if (IsChecked)
                {
                    CurrentStateImageSource = CheckedStateImageSource;
                }
            }
        }

        private ImageSource _currentStateImageBrush = null;
        public ImageSource CurrentStateImageSource { get { return _currentStateImageBrush; } set { SetProperty(ref _currentStateImageBrush, value); } }

        private string _text="";
        public string Text { get { return _text; } set { SetProperty(ref _text, value); } }

        private string _toolTip;
        public string ToolTip { get { return _toolTip; } set { SetProperty(ref _toolTip, value); } }

        private double _height;
        public double Height { get { return _height; } set { SetProperty(ref _height, value); } }

        private double _width;
        public double Width { get { return _width; } set { SetProperty(ref _width, value); } }



        private DelegateCommand _clickCommand;
        public DelegateCommand ClickCommand { get { return _clickCommand; } set { SetProperty(ref _clickCommand, value); } }

        private bool _isChecked;
        public bool IsChecked { get { return _isChecked; } set { SetProperty(ref _isChecked, value); if (_isChecked) { Check(); } else { Uncheck(); } } }


        public void Uncheck()
        {
            CurrentStateImageSource = this.UncheckedStateImageSource;
        }

        public void Check()
        {
            CurrentStateImageSource = this.CheckedStateImageSource;
        }
    }
}
