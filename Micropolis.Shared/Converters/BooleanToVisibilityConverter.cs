using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Micropolis.Converters
{
    /// <summary>
    /// The boolean to visibility converter.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || (value is bool) == false)
            {
                throw new NotSupportedException("The value must be of type bool.");
            }

            var boolVal = (bool)value;
            
            if (parameter != null && parameter.ToString().Equals("inverted"))
            {
                return (boolVal) ? Visibility.Collapsed : Visibility.Visible;
            }

            return (boolVal) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
