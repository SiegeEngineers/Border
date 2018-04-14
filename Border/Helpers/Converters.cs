using System;
using System.Globalization;
using System.Windows.Data;
namespace Border.Helpers
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean.");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(System.Windows.Visibility))
                throw new InvalidOperationException("The target must be a Windows.Visibility.");
            var visible = (bool)value;
            if (visible)
            {
                return (System.Windows.Visibility.Visible);
            }
            else
            {
                return (System.Windows.Visibility.Hidden);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class InverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(System.Windows.Visibility))
                throw new InvalidOperationException("The target must be a Windows.Visibility.");
            var hidden = (bool)value;
            if (hidden)
            {
                return (System.Windows.Visibility.Hidden);
            }
            else
            {
                return (System.Windows.Visibility.Visible);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    [ValueConversion(typeof(long), typeof(string))]
    public class SecondToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string.");
            long totalSeconds = (long)value;
            long seconds = totalSeconds % 60;
            long minutes = totalSeconds/60;
            return string.Format("{0}:{1}", minutes, seconds.ToString("00"));
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(long))
                throw new InvalidOperationException("The target must be an int.");
            string[] s = ((string)value).Split(':');

            return long.Parse(s[0]) *60 + long.Parse(s[1]);
        }
    }

    public class AreEqualConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (values[0] as Border.Model.BuildOrder).Queue == values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[]{ null, false};
        }
    }
}