using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media;
using Micropolis.Common;

namespace Micropolis.ViewModels
{
    public class LevelButtonViewModel : BindableBase
    {
       
        private string _text;
        public string Text { get { return _text; } set { SetProperty(ref _text, value); } }

        private DelegateCommand _clickCommand;
        public DelegateCommand ClickCommand { get { return _clickCommand; } set { SetProperty(ref _clickCommand, value); } }

        private bool _isChecked;
        public bool IsChecked { get { return _isChecked; } set { SetProperty(ref _isChecked, value); } }

    }
}
