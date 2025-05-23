using System;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x0200021E RID: 542
public static class GTPosRotScaleToString
{
	// Token: 0x06000C9B RID: 3227 RVA: 0x00042148 File Offset: 0x00040348
	public static string ToString(Vector3 pos, Vector3 rot, Vector3 scale, bool isWorldSpace, string parentPath = null)
	{
		string text = (isWorldSpace ? "WorldPRS" : "LocalPRS");
		string text2 = string.Concat(new string[]
		{
			text,
			" { p=",
			GTPosRotScaleToString.ValToStr(pos),
			", r=",
			GTPosRotScaleToString.ValToStr(rot),
			", s=",
			GTPosRotScaleToString.ValToStr(scale)
		});
		if (!string.IsNullOrEmpty(parentPath))
		{
			text2 = text2 + " parent=\"" + parentPath + "\"";
		}
		return text2 + " }";
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x000421D1 File Offset: 0x000403D1
	private static string ValToStr(Vector3 v)
	{
		return string.Format("({0:R}, {1:R}, {2:R})", v.x, v.y, v.z);
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x000421FE File Offset: 0x000403FE
	public static bool ParseIsWorldSpace(string input)
	{
		return input.Contains("WorldPRS");
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x0004220C File Offset: 0x0004040C
	public static string ParseParentPath(string input)
	{
		MatchCollection matchCollection = Regex.Matches(input, "parent\\s*=\\s*\"(?<parent>.*?)\"");
		if (matchCollection.Count <= 0)
		{
			return null;
		}
		return matchCollection[0].Groups["parent"].Value;
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0004224B File Offset: 0x0004044B
	public static bool TryParsePos(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Pos, input, out v);
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x00042259 File Offset: 0x00040459
	public static bool TryParseRot(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Rot, input, out v);
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x00042267 File Offset: 0x00040467
	public static bool TryParseScale(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Scale, input, out v) || GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Vec3, input, out v);
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x00042285 File Offset: 0x00040485
	public static bool TryParseVec3(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Vec3, input, out v);
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x00042294 File Offset: 0x00040494
	private static bool TryParseVec3_internal(Regex regex, string input, out Vector3 v)
	{
		v = Vector3.zero;
		MatchCollection matchCollection = regex.Matches(input);
		if (matchCollection.Count <= 0)
		{
			return false;
		}
		v = GTPosRotScaleToString.StringToVector3(matchCollection[0]);
		return true;
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x000422D4 File Offset: 0x000404D4
	private static Vector3 StringToVector3(Match match)
	{
		float num = float.Parse(match.Groups["x"].Value);
		float num2 = float.Parse(match.Groups["y"].Value);
		float num3 = float.Parse(match.Groups["z"].Value);
		return new Vector3(num, num2, num3);
	}

	// Token: 0x04000F21 RID: 3873
	public const string k_LocalPRSLabel = "LocalPRS";

	// Token: 0x04000F22 RID: 3874
	public const string k_WorldPRSLabel = "WorldPRS";
}
