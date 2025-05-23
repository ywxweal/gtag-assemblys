using System;
using UnityEngine;

// Token: 0x02000760 RID: 1888
public static class GradientHelper
{
	// Token: 0x06002F0A RID: 12042 RVA: 0x000EB594 File Offset: 0x000E9794
	public static Gradient FromColor(Color color)
	{
		float a = color.a;
		Color color2 = color;
		color2.a = 1f;
		return new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(color2, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(a, 1f)
			}
		};
	}
}
