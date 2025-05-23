using System;
using System.Collections.Generic;

// Token: 0x02000999 RID: 2457
public static class LinqUtils
{
	// Token: 0x06003AD5 RID: 15061 RVA: 0x00119633 File Offset: 0x00117833
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

	// Token: 0x06003AD6 RID: 15062 RVA: 0x0011964A File Offset: 0x0011784A
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

	// Token: 0x06003AD7 RID: 15063 RVA: 0x00119664 File Offset: 0x00117864
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T t in source)
		{
			action(t);
		}
		return source;
	}

	// Token: 0x06003AD8 RID: 15064 RVA: 0x001196B0 File Offset: 0x001178B0
	public static T[] AsArray<T>(this IEnumerable<T> source)
	{
		return (T[])source;
	}

	// Token: 0x06003AD9 RID: 15065 RVA: 0x001196B8 File Offset: 0x001178B8
	public static List<T> AsList<T>(this IEnumerable<T> source)
	{
		return (List<T>)source;
	}

	// Token: 0x06003ADA RID: 15066 RVA: 0x001196C0 File Offset: 0x001178C0
	public static IList<T> Transform<T>(this IList<T> list, Func<T, T> action)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = action(list[i]);
		}
		return list;
	}

	// Token: 0x06003ADB RID: 15067 RVA: 0x001196F3 File Offset: 0x001178F3
	public static IEnumerable<T> Self<T>(this T value)
	{
		yield return value;
		yield break;
	}
}
