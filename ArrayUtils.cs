using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// Token: 0x0200096B RID: 2411
public static class ArrayUtils
{
	// Token: 0x06003A23 RID: 14883 RVA: 0x00116D56 File Offset: 0x00114F56
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int BinarySearch<T>(this T[] array, T value) where T : IComparable<T>
	{
		return Array.BinarySearch<T>(array, 0, array.Length, value);
	}

	// Token: 0x06003A24 RID: 14884 RVA: 0x00116D63 File Offset: 0x00114F63
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty<T>(this T[] array)
	{
		return array == null || array.Length == 0;
	}

	// Token: 0x06003A25 RID: 14885 RVA: 0x00116D6F File Offset: 0x00114F6F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty<T>(this List<T> list)
	{
		return list == null || list.Count == 0;
	}

	// Token: 0x06003A26 RID: 14886 RVA: 0x00116D80 File Offset: 0x00114F80
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this T[] array, int from, int to)
	{
		T t = array[from];
		T t2 = array[to];
		array[to] = t;
		array[from] = t2;
	}

	// Token: 0x06003A27 RID: 14887 RVA: 0x00116DB8 File Offset: 0x00114FB8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this List<T> list, int from, int to)
	{
		T t = list[from];
		T t2 = list[to];
		list[to] = t;
		list[from] = t2;
	}

	// Token: 0x06003A28 RID: 14888 RVA: 0x00116DF4 File Offset: 0x00114FF4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Clone<T>(T[] source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Length == 0)
		{
			return Array.Empty<T>();
		}
		T[] array = new T[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			array[i] = source[i];
		}
		return array;
	}

	// Token: 0x06003A29 RID: 14889 RVA: 0x00116E36 File Offset: 0x00115036
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<T> Clone<T>(List<T> source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Count == 0)
		{
			return new List<T>();
		}
		return new List<T>(source);
	}

	// Token: 0x06003A2A RID: 14890 RVA: 0x00116E54 File Offset: 0x00115054
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfRef<T>(this T[] array, T value) where T : class
	{
		if (array == null || array.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06003A2B RID: 14891 RVA: 0x00116E90 File Offset: 0x00115090
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfRef<T>(this List<T> list, T value) where T : class
	{
		if (list == null || list.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == value)
			{
				return i;
			}
		}
		return -1;
	}
}
