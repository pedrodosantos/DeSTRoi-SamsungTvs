// DeSTRoi.Extenders
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Extenders
{
	public static void Enqueue<T>(this Queue<T> queue, T[] values)
	{
		foreach (T item in values)
		{
			queue.Enqueue(item);
		}
	}

	public static T[] Dequeue<T>(this Queue<T> queue, int count)
	{
		T[] array = new T[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = queue.Dequeue();
		}
		return array;
	}

	public static T[] Peek<T>(this Queue<T> queue, int count)
	{
		return SubArray(queue.ToArray(), 0, count);
	}

	public static T[] SubArray<T>(this T[] data, int index, int length)
	{
		T[] array = new T[length];
		Array.Copy(data, index, array, 0, length);
		return array;
	}

	public static byte[] ToByteArray(this string hex)
	{
		return (from x in Enumerable.Range(0, hex.Length)
						where x % 2 == 0
						select Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
	}

	public static string ToHexString(this byte[] byte_array)
	{
		return string.Join(string.Empty, byte_array.Select((byte x) => x.ToString("X2")).ToArray());
	}

	public static byte ToASCII(this char character)
	{
		return Encoding.ASCII.GetBytes(new char[1]
		{
			character
		})[0];
	}
}
