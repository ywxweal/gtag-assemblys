﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000983 RID: 2435
public static class ColorUtils
{
	// Token: 0x06003A86 RID: 14982 RVA: 0x00118299 File Offset: 0x00116499
	public static Color WithAlpha(this Color c, float alpha)
	{
		c.a = Math.Clamp(alpha, 0f, 1f);
		return c;
	}

	// Token: 0x06003A87 RID: 14983 RVA: 0x001182B3 File Offset: 0x001164B3
	public static Color32 WithAlpha(this Color32 c, byte alpha)
	{
		c.a = alpha;
		return c;
	}

	// Token: 0x06003A88 RID: 14984 RVA: 0x001182C0 File Offset: 0x001164C0
	public static Color ComposeHDR(Color baseColor, float intensity)
	{
		intensity = Mathf.Clamp(intensity, -10f, 10f);
		Color color = baseColor;
		if (baseColor.maxColorComponent > 1f)
		{
			color = ColorUtils.DecomposeHDR(baseColor).Item1;
		}
		float num = Mathf.Pow(2f, intensity);
		if (QualitySettings.activeColorSpace == ColorSpace.Linear)
		{
			num = Mathf.GammaToLinearSpace(intensity);
		}
		color *= num;
		color.a = baseColor.a;
		return color;
	}

	// Token: 0x06003A89 RID: 14985 RVA: 0x0011832C File Offset: 0x0011652C
	[return: TupleElementNames(new string[] { "baseColor", "intensity" })]
	public static ValueTuple<Color, float> DecomposeHDR(Color hdrColor)
	{
		Color32 color = default(Color32);
		float num = 0f;
		float maxColorComponent = hdrColor.maxColorComponent;
		if (maxColorComponent == 0f || (maxColorComponent <= 1f && maxColorComponent >= 0.003921569f))
		{
			color.r = (byte)Mathf.RoundToInt(hdrColor.r * 255f);
			color.g = (byte)Mathf.RoundToInt(hdrColor.g * 255f);
			color.b = (byte)Mathf.RoundToInt(hdrColor.b * 255f);
		}
		else
		{
			float num2 = 191f / maxColorComponent;
			num = Mathf.Log(255f / num2) / Mathf.Log(2f);
			color.r = Math.Min(191, (byte)Mathf.CeilToInt(num2 * hdrColor.r));
			color.g = Math.Min(191, (byte)Mathf.CeilToInt(num2 * hdrColor.g));
			color.b = Math.Min(191, (byte)Mathf.CeilToInt(num2 * hdrColor.b));
		}
		return new ValueTuple<Color, float>(color, num);
	}

	// Token: 0x04003F73 RID: 16243
	private const byte kMaxByteForOverexposedColor = 191;
}
