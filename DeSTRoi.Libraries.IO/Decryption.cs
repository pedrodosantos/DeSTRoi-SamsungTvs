// DeSTRoi.Libraries.IO.Decryption
using System.Security.Cryptography;
namespace DeSTRoi.Libraries.IO
{
	public class Decryption
	{
		public const int TS_FRAME_SIZE = 188;

		public static int Sync(byte[] buffer)
		{
			for (int i = 0; i < buffer.Length - 188; i++)
			{
				if (buffer[i] == 71 && buffer[i + 188] == 71 && buffer[i + 376] == 71)
				{
					return i;
				}
			}
			return -1;
		}

		public static byte[] DecryptPacket(byte[] input, byte[] key, bool decryptAdaption)
		{
			byte[] array = new byte[188];
			input.CopyTo(array, 0);
			if ((input[3] & 0xC0) != 0)
			{
				int num = 4;
				if (!decryptAdaption)
				{
					if ((input[3] & 0x20) != 0)
					{
						num += input[4] + 1;
					}
					array[3] &= 63;
					if (num > 188)
					{
						num = 188;
					}
				}
				array[3] &= 63;
				int num2 = (array.Length - num) / 16;
				using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
				{
					rijndaelManaged.Padding = PaddingMode.Zeros;
					rijndaelManaged.KeySize = 128;
					rijndaelManaged.Mode = CipherMode.ECB;
					byte[] rgbIV = new byte[16];
					using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(key, rgbIV))
					{
						for (int i = 0; i < num2; i++)
						{
							cryptoTransform.TransformBlock(input, num + i * 16, 16, array, num + i * 16);
						}
						return array;
					}
				}
			}
			return array;
		}
	}
}