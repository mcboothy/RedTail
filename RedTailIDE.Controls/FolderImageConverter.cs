using System;
using System.Globalization;
using System.Windows.Data;

namespace RedTailIDE.Controls
{
    public class FolderImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                return @"Images\folder_" + ((bool)value ? "open" : "closed") + ".png";
            }
            
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
