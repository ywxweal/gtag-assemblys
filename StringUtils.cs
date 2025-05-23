using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Text;
using UnityEngine;

// Token: 0x020009CC RID: 2508
public static class StringUtils
{
	// Token: 0x06003C0B RID: 15371 RVA: 0x0011F505 File Offset: 0x0011D705
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty(this string s)
	{
		return string.IsNullOrEmpty(s);
	}

	// Token: 0x06003C0C RID: 15372 RVA: 0x0011F50D File Offset: 0x0011D70D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrWhiteSpace(this string s)
	{
		return string.IsNullOrWhiteSpace(s);
	}

	// Token: 0x06003C0D RID: 15373 RVA: 0x0011F518 File Offset: 0x0011D718
	public static string ToAlphaNumeric(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return string.Empty;
		}
		string text;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			foreach (char c in s)
			{
				if (char.IsLetterOrDigit(c))
				{
					utf16ValueStringBuilder.Append(c);
				}
			}
			text = utf16ValueStringBuilder.ToString();
		}
		return text;
	}

	// Token: 0x06003C0E RID: 15374 RVA: 0x0011F594 File Offset: 0x0011D794
	public static string Capitalize(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		char[] array = s.ToCharArray();
		array[0] = char.ToUpperInvariant(array[0]);
		return new string(array);
	}

	// Token: 0x06003C0F RID: 15375 RVA: 0x0011F5C3 File Offset: 0x0011D7C3
	public static string Concat(this IEnumerable<string> source)
	{
		return string.Concat(source);
	}

	// Token: 0x06003C10 RID: 15376 RVA: 0x0011F5CB File Offset: 0x0011D7CB
	public static string Join(this IEnumerable<string> source, string separator)
	{
		return string.Join(separator, source);
	}

	// Token: 0x06003C11 RID: 15377 RVA: 0x0011F5D4 File Offset: 0x0011D7D4
	public static string Join(this IEnumerable<string> source, char separator)
	{
		return string.Join<string>(separator, source);
	}

	// Token: 0x06003C12 RID: 15378 RVA: 0x0011F5DD File Offset: 0x0011D7DD
	public static string RemoveAll(this string s, string value, StringComparison mode = StringComparison.OrdinalIgnoreCase)
	{
		if (string.IsNullOrEmpty(s))
		{
			return s;
		}
		return s.Replace(value, string.Empty, mode);
	}

	// Token: 0x06003C13 RID: 15379 RVA: 0x0011F5F6 File Offset: 0x0011D7F6
	public static string RemoveAll(this string s, char value, StringComparison mode = StringComparison.OrdinalIgnoreCase)
	{
		return s.RemoveAll(value.ToString(), mode);
	}

	// Token: 0x06003C14 RID: 15380 RVA: 0x0011F606 File Offset: 0x0011D806
	public static byte[] ToBytesASCII(this string s)
	{
		return Encoding.ASCII.GetBytes(s);
	}

	// Token: 0x06003C15 RID: 15381 RVA: 0x0011F613 File Offset: 0x0011D813
	public static byte[] ToBytesUTF8(this string s)
	{
		return Encoding.UTF8.GetBytes(s);
	}

	// Token: 0x06003C16 RID: 15382 RVA: 0x0011F620 File Offset: 0x0011D820
	public static byte[] ToBytesUnicode(this string s)
	{
		return Encoding.Unicode.GetBytes(s);
	}

	// Token: 0x06003C17 RID: 15383 RVA: 0x0011F630 File Offset: 0x0011D830
	public static string ComputeSHV2(this string s)
	{
		return Hash128.Compute(s).ToString();
	}

	// Token: 0x06003C18 RID: 15384 RVA: 0x0011F651 File Offset: 0x0011D851
	public static string ToQueryString(this Dictionary<string, string> d)
	{
		if (d == null)
		{
			return null;
		}
		return "?" + string.Join("&", d.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value));
	}

	// Token: 0x06003C19 RID: 15385 RVA: 0x0011F694 File Offset: 0x0011D894
	public static string Combine(string separator, params string[] values)
	{
		if (values == null || values.Length == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = !string.IsNullOrEmpty(separator);
		for (int i = 0; i < values.Length; i++)
		{
			if (flag)
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(values);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06003C1A RID: 15386 RVA: 0x0011F6E4 File Offset: 0x0011D8E4
	public static string ToUpperCamelCase(this string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return string.Empty;
		}
		string[] array = Regex.Split(input, "[^A-Za-z0-9]+");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Length > 0)
			{
				string[] array2 = array;
				int num = i;
				string text = char.ToUpper(array[i][0]).ToString();
				string text2;
				if (array[i].Length <= 1)
				{
					text2 = "";
				}
				else
				{
					string text3 = array[i];
					text2 = text3.Substring(1, text3.Length - 1).ToLower();
				}
				array2[num] = text + text2;
			}
		}
		return string.Join("", array);
	}

	// Token: 0x06003C1B RID: 15387 RVA: 0x0011F778 File Offset: 0x0011D978
	public static string ToUpperCaseFromCamelCase(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		input = input.Trim();
		string text;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			bool flag = true;
			foreach (char c in input)
			{
				if (char.IsUpper(c) && !flag)
				{
					utf16ValueStringBuilder.Append(' ');
				}
				utf16ValueStringBuilder.Append(char.ToUpper(c));
				flag = char.IsUpper(c);
			}
			text = utf16ValueStringBuilder.ToString().Trim();
		}
		return text;
	}

	// Token: 0x06003C1C RID: 15388 RVA: 0x0011F818 File Offset: 0x0011DA18
	public static string RemoveStart(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		if (string.IsNullOrEmpty(s) || !s.StartsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(value.Length);
	}

	// Token: 0x06003C1D RID: 15389 RVA: 0x0011F83A File Offset: 0x0011DA3A
	public static string RemoveEnd(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		if (string.IsNullOrEmpty(s) || !s.EndsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(0, s.Length - value.Length);
	}

	// Token: 0x06003C1E RID: 15390 RVA: 0x0011F864 File Offset: 0x0011DA64
	public static string RemoveBothEnds(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		return s.RemoveEnd(value, comparison).RemoveStart(value, comparison);
	}

	// Token: 0x04004037 RID: 16439
	public const string kForwardSlash = "/";

	// Token: 0x04004038 RID: 16440
	public const string kBackSlash = "/";

	// Token: 0x04004039 RID: 16441
	public const string kBackTick = "`";

	// Token: 0x0400403A RID: 16442
	public const string kMinusDash = "-";

	// Token: 0x0400403B RID: 16443
	public const string kPeriod = ".";

	// Token: 0x0400403C RID: 16444
	public const string kUnderScore = "_";

	// Token: 0x0400403D RID: 16445
	public const string kColon = ":";
}
