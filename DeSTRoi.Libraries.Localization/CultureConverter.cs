using System;
using System.Globalization;
using System.Windows.Data;
namespace DeSTRoi.Libraries.Localization
{
	public class CultureConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.GetType() == typeof(TimeSpan))
			{
				return ((TimeSpan)value).ToString("c", culture);
			}
			return System.Convert.ChangeType(value, targetType, CultureInfo.CurrentCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ChangeType(value, targetType, CultureInfo.CurrentCulture);
		}
	}
}