// DeSTRoi.Libraries.IO.FileSizeValueConverter
using System;
using System.Globalization;
using System.Windows.Data;

namespace DeSTRoi.Libraries.IO
{
	public class FileSizeValueConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return FileSizeConverter.ConvertToFileSize(Convert.ToInt64(value));
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}