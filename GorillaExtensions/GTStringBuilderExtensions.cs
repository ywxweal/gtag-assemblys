using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using UnityEngine;

namespace GorillaExtensions
{
	// Token: 0x02000CF3 RID: 3315
	public static class GTStringBuilderExtensions
	{
		// Token: 0x0600523D RID: 21053 RVA: 0x0019031C File Offset: 0x0018E51C
		public unsafe static IEnumerable<ReadOnlyMemory<char>> GetSegmentsOfMem(this Utf16ValueStringBuilder sb, int maxCharsPerSegment = 16300)
		{
			int i = 0;
			List<ReadOnlyMemory<char>> list = new List<ReadOnlyMemory<char>>(64);
			ReadOnlyMemory<char> readOnlyMemory = sb.AsMemory();
			while (i < readOnlyMemory.Length)
			{
				int num = Mathf.Min(i + maxCharsPerSegment, readOnlyMemory.Length);
				if (num < readOnlyMemory.Length)
				{
					int num2 = -1;
					for (int j = num - 1; j >= i; j--)
					{
						if (*readOnlyMemory.Span[j] == 10)
						{
							num2 = j;
							break;
						}
					}
					if (num2 != -1)
					{
						num = num2;
					}
				}
				list.Add(readOnlyMemory.Slice(i, num - i));
				i = num + 1;
			}
			return list;
		}

		// Token: 0x0600523E RID: 21054 RVA: 0x001903B1 File Offset: 0x0018E5B1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTAddPath(this Utf16ValueStringBuilder stringBuilderToAddTo, GameObject gameObject)
		{
			gameObject.transform.GetPathQ(ref stringBuilderToAddTo);
		}

		// Token: 0x0600523F RID: 21055 RVA: 0x001903C0 File Offset: 0x0018E5C0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTAddPath(this Utf16ValueStringBuilder stringBuilderToAddTo, Transform transform)
		{
			transform.GetPathQ(ref stringBuilderToAddTo);
		}

		// Token: 0x06005240 RID: 21056 RVA: 0x001903CA File Offset: 0x0018E5CA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Q(this Utf16ValueStringBuilder sb, string value)
		{
			sb.Append('"');
			sb.Append(value);
			sb.Append('"');
		}

		// Token: 0x06005241 RID: 21057 RVA: 0x001903E6 File Offset: 0x0018E5E6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b)
		{
			sb.Append(a);
			sb.Append(b);
		}

		// Token: 0x06005242 RID: 21058 RVA: 0x001903F8 File Offset: 0x0018E5F8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
		}

		// Token: 0x06005243 RID: 21059 RVA: 0x00190412 File Offset: 0x0018E612
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
		}

		// Token: 0x06005244 RID: 21060 RVA: 0x00190435 File Offset: 0x0018E635
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
		}

		// Token: 0x06005245 RID: 21061 RVA: 0x00190461 File Offset: 0x0018E661
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
		}

		// Token: 0x06005246 RID: 21062 RVA: 0x00190496 File Offset: 0x0018E696
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
		}

		// Token: 0x06005247 RID: 21063 RVA: 0x001904D4 File Offset: 0x0018E6D4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
		}

		// Token: 0x06005248 RID: 21064 RVA: 0x00190528 File Offset: 0x0018E728
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
			sb.Append(i);
		}

		// Token: 0x06005249 RID: 21065 RVA: 0x00190584 File Offset: 0x0018E784
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i, string j)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
			sb.Append(i);
			sb.Append(j);
		}
	}
}
