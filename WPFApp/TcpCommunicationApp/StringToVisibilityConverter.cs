using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TcpCommunicationApp
{
    /// <summary>
    /// 字串轉換器
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            return string.IsNullOrEmpty(input) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
