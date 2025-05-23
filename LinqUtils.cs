using System;
using System.Collections.Generic;

// Token: 0x02000999 RID: 2457
public static class LinqUtils
{
	// Token: 0x06003AD4 RID: 15060 RVA: 0x0011955B File Offset: 0x0011775B
	public static IEnumerable<TResult> SelectManyNullSafe<TSource, TResult>(this IEnumerable<TSource> sources, Func<TSource, IEnumerable<TResult>> selector)
	{
		if (sources == null)
		{
			yield break;
		}
		if (selector == null)
		{
			yield break;
		}
		foreach (TSource tsource in sources)
		{
			if (tsource != null)
			{
				IEnumerable<TResult> enumerable = selector(tsource);
				foreach (TResult tresult in enumerable)
				{
					if (tresult != null)
					{
						yield return tresult;
					}
				}
				IEnumerator<TResult> enumerator2 = null;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06003AD5 RID: 15061 RVA: 0x00119572 File Offset: 0x00117772
	public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		HashSet<TResult> set = new HashSet<TResult>();
		foreach (TSource tsource in source)
		{
			TResult tresult = selector(tsource);
			if (set.Add(tresult))
			{
				yield return tsource;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06003AD6 RID: 15062 RVA: 0x0011958C File Offset: 0x0011778C
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T t in source)
		{
			action(t);
		}
		return source;
	}

	// Token: 0x06003AD7 RID: 15063 RVA: 0x001195D8 File Offset: 0x001177D8
	public static T[] AsArray<T>(this IEnumerable<T> source)
	{
		return (T[])source;
	}

	// Token: 0x06003AD8 RID: 15064 RVA: 0x001195E0 File Offset: 0x001177E0
	public static List<T> AsList<T>(this IEnumerable<T> source)
	{
		return (List<T>)source;
	}

	// Token: 0x06003AD9 RID: 15065 RVA: 0x001195E8 File Offset: 0x001177E8
	public static IList<T> Transform<T>(this IList<T> list, Func<T, T> action)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = action(list[i]);
		}
		return list;
	}

	// Token: 0x06003ADA RID: 15066 RVA: 0x0011961B File Offset: 0x0011781B
	public static IEnumerable<T> Self<T>(this T value)
	{
		yield return value;
		yield break;
	}
}
