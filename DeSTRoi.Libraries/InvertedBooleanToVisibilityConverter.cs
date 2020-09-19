// DeSTRoi.Libraries.InvertedBooleanToVisibilityConverter
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DeSTRoi.Libraries
{
  public class InvertedBooleanToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((bool)value)
      {
        return Visibility.Collapsed;
      }
      
      return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch ((Visibility)value)
      {
        case Visibility.Visible:
          return false;
        case Visibility.Hidden:
        case Visibility.Collapsed:
          return true;
        default:
          return true;
      }
    }
  }
}