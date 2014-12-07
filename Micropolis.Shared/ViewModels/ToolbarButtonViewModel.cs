using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media;
using Micropolis.Common;

namespace Micropolis.ViewModels
{
    public class ToolbarButtonViewModel : BindableBase
    {

        private ImageBrush _uncheckedStateImageBrush;
        public ImageBrush UncheckedStateImageBrush
        {
            get { return _uncheckedStateImageBrush; }
            set
            {
                SetProperty(ref _uncheckedStateImageBrush, value);
                if (!IsChecked)
                {
                    CurrentStateImageBrush = UncheckedStateImageBrush;
                }
            }
        }

        private ImageBrush _checkedStateImageBrush;
        public ImageBrush CheckedStateImageBrush
        {
            get { return _checkedStateImageBrush; }
            set
            {
                SetProperty(ref _checkedStateImageBrush, value);

                if (IsChecked)
                {
                    CurrentStateImageBrush = CheckedStateImageBrush;
                }
            }
        }

        private ImageBrush _currentStateImageBrush = null;
        public ImageBrush CurrentStateImageBrush { get { return _currentStateImageBrush; } set { SetProperty(ref _currentStateImageBrush, value); } }

        private string _text;
        public string Text { get { return _text; } set { SetProperty(ref _text, value); } }

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
            CurrentStateImageBrush = this.UncheckedStateImageBrush;
        }

        public void Check()
        {
            CurrentStateImageBrush = this.CheckedStateImageBrush;
        }
    }
}
