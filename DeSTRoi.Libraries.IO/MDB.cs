// DeSTRoi.Libraries.IO.MDB
using System;
using System.IO;
namespace DeSTRoi.Libraries.IO
{
	public class MDB
	{
		public static byte[] Parse(Stream stream)
		{
			byte[] array = new byte[16];
			stream.Seek(8L, SeekOrigin.Begin);
			for (byte b = 0; b < 16; b = (byte)(b + 1))
			{
				array[(b & 0xC) + (3 - (b & 3))] = Convert.ToByte(stream.ReadByte());
			}
			return array;
		}

		public static byte[] Parse(string filename)
		{
			byte[] array = null;
			using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				return Parse(stream);
			}
		}

		public static byte[] Parse(string filename, bool saveKey)
		{
			byte[] array = Parse(filename);
			if (saveKey)
			{
				File.WriteAllBytes(Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + ".key", array);
			}
			return array;
		}
	}
}