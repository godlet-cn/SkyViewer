
using System;
using System.Windows;
using System.Windows.Data;

namespace SkyViewerServer
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Visibility Convert(object value, object parameter)
        {
            string valueString = "true";
            string compareString = "true";
            bool bvalue = true;

            if (value != null && bool.TryParse(value.ToString(), out bvalue))
            {
                valueString = bvalue.ToString().ToLower();
            }

            //默认不占位置隐藏
            Visibility oneState = Visibility.Collapsed;
            //判断是否 不占位置隐藏、占位置隐藏、bool值与字符串比较相同，则显示，不同这隐藏
            if (parameter != null)
            {
                string paramString = System.Convert.ToString(parameter).ToLower();
                string[] arr = paramString.Split('|');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].ToLower() == "false")
                    {
                        compareString = "false";
                    }
                    else if (arr[i].ToLower() == "true")
                    {
                        compareString = "true";
                    }
                    else if (arr[i].ToLower() == "hidden")
                    {
                        oneState = Visibility.Hidden;
                    }
                }
            }
            if (!string.IsNullOrEmpty(compareString))
            {
                return valueString == compareString ? Visibility.Visible : oneState;
            }
            else
            {
                return bvalue ? Visibility.Visible : oneState;
            }
        }

    }
}
