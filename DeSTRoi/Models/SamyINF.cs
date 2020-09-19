// DeSTRoi.Models.SamyINF
using System;
using System.IO;
using System.Linq;
using System.Text;
namespace DeSTRoi.Models
{
	public class SamyINF
	{
		public const int NormalSize = 1356;

		public const int ExtendedSize = 7464;

		public string ChannelName
		{
			get;
			set;
		}

		public string RawTitle
		{
			get;
			set;
		}

		public int ChannelNumber
		{
			get;
			set;
		}

		public DateTime RecTime
		{
			get;
			set;
		}

		public TimeSpan Duration
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Language
		{
			get;
			set;
		}

		public bool IsExtended
		{
			get;
			set;
		}

		public string Info
		{
			get;
			set;
		}

		public DateTime TimerStart
		{
			get;
			set;
		}

		public DateTime TimerEnd
		{
			get;
			set;
		}

		public string ContentLanguage
		{
			get;
			set;
		}

		public string Category
		{
			get;
			set;
		}

		public byte Favorite
		{
			get;
			set;
		}

		public byte Lock
		{
			get;
			set;
		}

		public SamyINF()
		{
			ChannelName = (RawTitle = (Title = (Language = (Info = (Category = (ContentLanguage = ""))))));
			ChannelNumber = -1;
			RecTime = (TimerStart = (TimerEnd = DateTime.MinValue));
			Duration = TimeSpan.MinValue;
			IsExtended = false;
			Favorite = (Lock = 0);
		}

		public SamyINF(string fileName)
			: this()
		{
			Parse(fileName);
		}

		public SamyINF(Stream data)
			: this()
		{
			Parse(data);
		}

		public void Save(Stream data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			long position = data.Position;
			if (IsExtended)
			{
				data.Write(new byte[7464], 0, 7464);
			}
			else
			{
				data.Write(new byte[1356], 0, 1356);
			}
			GC.Collect();
			byte[] array = new byte[4096];
			data.Seek(position, SeekOrigin.Begin);
			Encoding.GetEncoding("UTF-16BE").GetBytes(ChannelName).CopyTo(array, 0);
			data.Write(array, 0, 256);
			Array.Clear(array, 0, array.Length);
			data.Seek(position + 256, SeekOrigin.Begin);
			Encoding.GetEncoding("UTF-16BE").GetBytes(RawTitle).CopyTo(array, 0);
			data.Write(array, 0, 256);
			Array.Clear(array, 0, array.Length);
			data.Seek(position + 512, SeekOrigin.Begin);
			Encoding.UTF8.GetBytes(ChannelNumber.ToString()).CopyTo(array, 0);
			data.Write(array, 0, 16);
			Array.Clear(array, 0, array.Length);
			data.Seek(position + 772, SeekOrigin.Begin);
			uint value = Convert.ToUInt32((RecTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds);
			BitConverter.GetBytes(value).CopyTo(array, 0);
			data.Write(array, 0, 16);
			Array.Clear(array, 0, array.Length);
			data.Seek(position + 776, SeekOrigin.Begin);
			uint value2 = Convert.ToUInt32(Duration.Seconds);
			BitConverter.GetBytes(value2).CopyTo(array, 0);
			data.Write(array, 0, 16);
			Array.Clear(array, 0, array.Length);
			data.Seek(position + 792, SeekOrigin.Begin);
			Encoding.GetEncoding("UTF-16LE").GetBytes(Title).CopyTo(array, 0);
			data.Write(array, 0, 256);
			Array.Clear(array, 0, array.Length);
			data.Seek(position + 1332, SeekOrigin.Begin);
			Encoding.UTF8.GetBytes(new string(Language.Reverse().ToArray())).CopyTo(array, 0);
			data.Write(array, 0, 16);
			Array.Clear(array, 0, array.Length);
			if (IsExtended)
			{
				data.Seek(position + 1782, SeekOrigin.Begin);
				Encoding.GetEncoding("UTF-16BE").GetBytes(Info).CopyTo(array, 0);
				data.Write(array, 0, 4096);
				Array.Clear(array, 0, array.Length);
				data.Seek(position + 6392, SeekOrigin.Begin);
				value = Convert.ToUInt32((TimerStart.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds);
				BitConverter.GetBytes(value).CopyTo(array, 0);
				data.Write(array, 0, 16);
				Array.Clear(array, 0, array.Length);
				data.Seek(position + 6396, SeekOrigin.Begin);
				value = Convert.ToUInt32((TimerEnd.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds);
				BitConverter.GetBytes(value).CopyTo(array, 0);
				data.Write(array, 0, 16);
				Array.Clear(array, 0, array.Length);
				data.Seek(position + 6414, SeekOrigin.Begin);
				Encoding.GetEncoding("UTF-16LE").GetBytes(ContentLanguage).CopyTo(array, 0);
				data.Write(array, 0, 256);
				Array.Clear(array, 0, array.Length);
				data.Seek(position + 6926, SeekOrigin.Begin);
				Encoding.GetEncoding("UTF-16LE").GetBytes(Category).CopyTo(array, 0);
				data.Write(array, 0, 256);
				Array.Clear(array, 0, array.Length);
				data.Seek(position + 7444, SeekOrigin.Begin);
				data.WriteByte(Lock);
				data.Seek(position + 7452, SeekOrigin.Begin);
				data.WriteByte(Favorite);
			}
		}

		public void Save(string fileName)
		{
			using (FileStream data = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				Save(data);
			}
		}

		public void Parse(Stream data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array = new byte[256];
			data.Seek(0L, SeekOrigin.Begin);
			data.Read(array, 0, array.Length);
			ChannelName = Encoding.GetEncoding("UTF-16BE").GetString(array).Trim(' ', '\0');
			Array.Clear(array, 0, array.Length);
			data.Seek(256L, SeekOrigin.Begin);
			data.Read(array, 0, array.Length);
			RawTitle = Encoding.GetEncoding((array[0] == 0) ? "UTF-16BE" : "UTF-8").GetString(array).Trim(' ', '\0');
			byte[] array2 = new byte[16];
			data.Seek(512L, SeekOrigin.Begin);
			data.Read(array2, 0, array2.Length);
			ChannelNumber = Convert.ToInt32(Encoding.UTF8.GetString(array2).Trim(' ', '\0'));
			Array.Clear(array2, 0, array2.Length);
			data.Seek(772L, SeekOrigin.Begin);
			data.Read(array2, 0, array2.Length);
			uint num = BitConverter.ToUInt32(array2, 0);
			RecTime = (new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(num)).ToLocalTime();
			Array.Clear(array2, 0, array2.Length);
			data.Seek(776L, SeekOrigin.Begin);
			data.Read(array2, 0, array2.Length);
			uint num2 = BitConverter.ToUInt32(array2, 0);
			Duration = TimeSpan.FromSeconds(num2);
			Array.Clear(array, 0, array.Length);
			data.Seek(792L, SeekOrigin.Begin);
			data.Read(array, 0, array.Length);
			Title = Encoding.GetEncoding("UTF-16LE").GetString(array).Trim(' ', '\0');
			Array.Clear(array2, 0, array2.Length);
			data.Seek(1332L, SeekOrigin.Begin);
			data.Read(array2, 0, array2.Length);
			Language = new string(Encoding.UTF8.GetString(array2).Substring(0, 3).Reverse()
				.ToArray());
			if (ChannelName == "" || num == 0)
			{
				throw new FormatException("INF File is invalid.");
			}
			if (data.Length == 7464)
			{
				IsExtended = true;
				byte[] array3 = new byte[4096];
				data.Seek(1782L, SeekOrigin.Begin);
				data.Read(array3, 0, array3.Length);
				Info = Encoding.GetEncoding("UTF-16BE").GetString(array3).Trim(' ', '\0');
				Array.Clear(array2, 0, array2.Length);
				data.Seek(6392L, SeekOrigin.Begin);
				data.Read(array2, 0, array2.Length);
				num = BitConverter.ToUInt32(array2, 0);
				TimerStart = (new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(num)).ToLocalTime();
				Array.Clear(array2, 0, array2.Length);
				data.Seek(6396L, SeekOrigin.Begin);
				data.Read(array2, 0, array2.Length);
				num = BitConverter.ToUInt32(array2, 0);
				TimerEnd = (new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(num)).ToLocalTime();
				Array.Clear(array, 0, array.Length);
				data.Seek(6414L, SeekOrigin.Begin);
				data.Read(array, 0, array.Length);
				ContentLanguage = Encoding.GetEncoding("UTF-16LE").GetString(array).Trim(' ', '\0');
				Array.Clear(array, 0, array.Length);
				data.Seek(6926L, SeekOrigin.Begin);
				data.Read(array, 0, array.Length);
				Category = Encoding.GetEncoding("UTF-16LE").GetString(array).Trim(' ', '\0');
				data.Seek(7444L, SeekOrigin.Begin);
				Lock = Convert.ToByte(data.ReadByte());
				data.Seek(7452L, SeekOrigin.Begin);
				Favorite = Convert.ToByte(data.ReadByte());
			}
		}

		public void Parse(string fileName)
		{
			using (FileStream data = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				Parse(data);
			}
		}
	}
}