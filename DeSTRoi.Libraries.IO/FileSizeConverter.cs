// DeSTRoi.Libraries.IO.FileSizeConverter
using System;
namespace DeSTRoi.Libraries.IO
{
	public static class FileSizeConverter
	{
		public static string ConvertToFileSize(int source)
		{
			return ConvertToFileSize(Convert.ToInt64(source));
		}

		public static string ConvertToFileSize(long source)
		{
			double num = Convert.ToDouble(source);
			if (num >= Math.Pow(1024.0, 3.0))
			{
				return Math.Round(num / Math.Pow(1024.0, 3.0), 2) + " GB";
			}
			if (num >= Math.Pow(1024.0, 2.0))
			{
				return Math.Round(num / Math.Pow(1024.0, 2.0), 2) + " MB";
			}
			if (num >= 1024.0)
			{
				return Math.Round(num / 1024.0, 2) + " KB";
			}
			return num + " Bytes";
		}
	}
}