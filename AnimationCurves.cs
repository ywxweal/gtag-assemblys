using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200074A RID: 1866
public static class AnimationCurves
{
	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x06002EA2 RID: 11938 RVA: 0x000E9184 File Offset: 0x000E7384
	public static AnimationCurve EaseInQuad
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 2.000003f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x06002EA3 RID: 11939 RVA: 0x000E91F0 File Offset: 0x000E73F0
	public static AnimationCurve EaseOutQuad
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 2.000003f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x1700048F RID: 1167
	// (get) Token: 0x06002EA4 RID: 11940 RVA: 0x000E925C File Offset: 0x000E745C
	public static AnimationCurve EaseInOutQuad
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
				new Keyframe(0.5f, 0.5f, 1.999994f, 1.999994f, 0.333334f, 0.333334f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
			});
		}
	}

	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x06002EA5 RID: 11941 RVA: 0x000E92F4 File Offset: 0x000E74F4
	public static AnimationCurve EaseInCubic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 3.000003f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x06002EA6 RID: 11942 RVA: 0x000E9360 File Offset: 0x000E7560
	public static AnimationCurve EaseOutCubic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 3.000003f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x06002EA7 RID: 11943 RVA: 0x000E93CC File Offset: 0x000E75CC
	public static AnimationCurve EaseInOutCubic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
				new Keyframe(0.5f, 0.5f, 2.999994f, 2.999994f, 0.333334f, 0.333334f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
			});
		}
	}

	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x06002EA8 RID: 11944 RVA: 0x000E9464 File Offset: 0x000E7664
	public static AnimationCurve EaseInQuart
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.0139424f, 0f, 0.434789f),
				new Keyframe(1f, 1f, 3.985819f, 0f, 0.269099f, 0f)
			});
		}
	}

	// Token: 0x17000494 RID: 1172
	// (get) Token: 0x06002EA9 RID: 11945 RVA: 0x000E94D0 File Offset: 0x000E76D0
	public static AnimationCurve EaseOutQuart
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 3.985823f, 0f, 0.269099f),
				new Keyframe(1f, 1f, 0.01394233f, 0f, 0.434789f, 0f)
			});
		}
	}

	// Token: 0x17000495 RID: 1173
	// (get) Token: 0x06002EAA RID: 11946 RVA: 0x000E953C File Offset: 0x000E773C
	public static AnimationCurve EaseInOutQuart
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.01394243f, 0f, 0.434788f),
				new Keyframe(0.5f, 0.5f, 3.985842f, 3.985834f, 0.269098f, 0.269098f),
				new Keyframe(1f, 1f, 0.0139425f, 0f, 0.434788f, 0f)
			});
		}
	}

	// Token: 0x17000496 RID: 1174
	// (get) Token: 0x06002EAB RID: 11947 RVA: 0x000E95D4 File Offset: 0x000E77D4
	public static AnimationCurve EaseInQuint
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.02411811f, 0f, 0.519568f),
				new Keyframe(1f, 1f, 4.951815f, 0f, 0.225963f, 0f)
			});
		}
	}

	// Token: 0x17000497 RID: 1175
	// (get) Token: 0x06002EAC RID: 11948 RVA: 0x000E9640 File Offset: 0x000E7840
	public static AnimationCurve EaseOutQuint
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 4.953289f, 0f, 0.225963f),
				new Keyframe(1f, 1f, 0.02414908f, 0f, 0.518901f, 0f)
			});
		}
	}

	// Token: 0x17000498 RID: 1176
	// (get) Token: 0x06002EAD RID: 11949 RVA: 0x000E96AC File Offset: 0x000E78AC
	public static AnimationCurve EaseInOutQuint
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.02412004f, 0f, 0.519568f),
				new Keyframe(0.5f, 0.5f, 4.951789f, 4.953269f, 0.225964f, 0.225964f),
				new Keyframe(1f, 1f, 0.02415099f, 0f, 0.5189019f, 0f)
			});
		}
	}

	// Token: 0x17000499 RID: 1177
	// (get) Token: 0x06002EAE RID: 11950 RVA: 0x000E9744 File Offset: 0x000E7944
	public static AnimationCurve EaseInSine
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, -0.001208493f, 0f, 0.36078f),
				new Keyframe(1f, 1f, 1.572508f, 0f, 0.326514f, 0f)
			});
		}
	}

	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x06002EAF RID: 11951 RVA: 0x000E97B0 File Offset: 0x000E79B0
	public static AnimationCurve EaseOutSine
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 1.573552f, 0f, 0.330931f),
				new Keyframe(1f, 1f, -0.0009282457f, 0f, 0.358689f, 0f)
			});
		}
	}

	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x06002EB0 RID: 11952 RVA: 0x000E981C File Offset: 0x000E7A1C
	public static AnimationCurve EaseInOutSine
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, -0.001202949f, 0f, 0.36078f),
				new Keyframe(0.5f, 0.5f, 1.572508f, 1.573372f, 0.326514f, 0.33093f),
				new Keyframe(1f, 1f, -0.0009312395f, 0f, 0.358688f, 0f)
			});
		}
	}

	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x06002EB1 RID: 11953 RVA: 0x000E98B4 File Offset: 0x000E7AB4
	public static AnimationCurve EaseInExpo
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.03124388f, 0f, 0.636963f),
				new Keyframe(1f, 1f, 6.815432f, 0f, 0.155667f, 0f)
			});
		}
	}

	// Token: 0x1700049D RID: 1181
	// (get) Token: 0x06002EB2 RID: 11954 RVA: 0x000E9920 File Offset: 0x000E7B20
	public static AnimationCurve EaseOutExpo
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 6.815433f, 0f, 0.155667f),
				new Keyframe(1f, 1f, 0.03124354f, 0f, 0.636963f, 0f)
			});
		}
	}

	// Token: 0x1700049E RID: 1182
	// (get) Token: 0x06002EB3 RID: 11955 RVA: 0x000E998C File Offset: 0x000E7B8C
	public static AnimationCurve EaseInOutExpo
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.03124509f, 0f, 0.636964f),
				new Keyframe(0.5f, 0.5f, 6.815477f, 6.815476f, 0.155666f, 0.155666f),
				new Keyframe(1f, 1f, 0.03124377f, 0f, 0.636964f, 0f)
			});
		}
	}

	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x06002EB4 RID: 11956 RVA: 0x000E9A24 File Offset: 0x000E7C24
	public static AnimationCurve EaseInCirc
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.002162338f, 0f, 0.55403f),
				new Keyframe(1f, 1f, 459.267f, 0f, 0.001197994f, 0f)
			});
		}
	}

	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x06002EB5 RID: 11957 RVA: 0x000E9A90 File Offset: 0x000E7C90
	public static AnimationCurve EaseOutCirc
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 461.7679f, 0f, 0.001198f),
				new Keyframe(1f, 1f, 0.00216235f, 0f, 0.554024f, 0f)
			});
		}
	}

	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x06002EB6 RID: 11958 RVA: 0x000E9AFC File Offset: 0x000E7CFC
	public static AnimationCurve EaseInOutCirc
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.002162353f, 0f, 0.554026f),
				new Keyframe(0.5f, 0.5f, 461.7703f, 461.7474f, 0.001197994f, 0.001198053f),
				new Keyframe(1f, 1f, 0.00216245f, 0f, 0.554026f, 0f)
			});
		}
	}

	// Token: 0x170004A2 RID: 1186
	// (get) Token: 0x06002EB7 RID: 11959 RVA: 0x000E9B94 File Offset: 0x000E7D94
	public static AnimationCurve EaseInBounce
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.6874897f, 0f, 0.3333663f),
				new Keyframe(0.0909f, 0f, -0.687694f, 1.374792f, 0.3332673f, 0.3334159f),
				new Keyframe(0.2727f, 0f, -1.375608f, 2.749388f, 0.3332179f, 0.3333489f),
				new Keyframe(0.6364f, 0f, -2.749183f, 5.501642f, 0.3333737f, 0.3332673f),
				new Keyframe(1f, 1f, 0f, 0f, 0.3333663f, 0f)
			});
		}
	}

	// Token: 0x170004A3 RID: 1187
	// (get) Token: 0x06002EB8 RID: 11960 RVA: 0x000E9C80 File Offset: 0x000E7E80
	public static AnimationCurve EaseOutBounce
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.3333663f),
				new Keyframe(0.3636f, 1f, 5.501643f, -2.749183f, 0.3332673f, 0.3333737f),
				new Keyframe(0.7273f, 1f, 2.749366f, -1.375609f, 0.3333516f, 0.3332178f),
				new Keyframe(0.9091f, 1f, 1.374792f, -0.6877043f, 0.3334158f, 0.3332673f),
				new Keyframe(1f, 1f, 0.6875f, 0f, 0.3333663f, 0f)
			});
		}
	}

	// Token: 0x170004A4 RID: 1188
	// (get) Token: 0x06002EB9 RID: 11961 RVA: 0x000E9D6C File Offset: 0x000E7F6C
	public static AnimationCurve EaseInOutBounce
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.6875001f, 0f, 0.333011f),
				new Keyframe(0.0455f, 0f, -0.6854643f, 1.377057f, 0.334f, 0.3328713f),
				new Keyframe(0.1364f, 0f, -1.373381f, 2.751643f, 0.3337624f, 0.3331683f),
				new Keyframe(0.3182f, 0f, -2.749192f, 5.501634f, 0.3334654f, 0.3332673f),
				new Keyframe(0.5f, 0.5f, 0f, 0f, 0.3333663f, 0.3333663f),
				new Keyframe(0.6818f, 1f, 5.501634f, -2.749191f, 0.3332673f, 0.3334653f),
				new Keyframe(0.8636f, 1f, 2.751642f, -1.37338f, 0.3331683f, 0.3319367f),
				new Keyframe(0.955f, 1f, 1.354673f, -0.7087823f, 0.3365205f, 0.3266002f),
				new Keyframe(1f, 1f, 0.6875f, 0f, 0.3367105f, 0f)
			});
		}
	}

	// Token: 0x170004A5 RID: 1189
	// (get) Token: 0x06002EBA RID: 11962 RVA: 0x000E9F00 File Offset: 0x000E8100
	public static AnimationCurve EaseInBack
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 4.701583f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x170004A6 RID: 1190
	// (get) Token: 0x06002EBB RID: 11963 RVA: 0x000E9F6C File Offset: 0x000E816C
	public static AnimationCurve EaseOutBack
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 4.701584f, 0f, 0.333333f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333333f, 0f)
			});
		}
	}

	// Token: 0x170004A7 RID: 1191
	// (get) Token: 0x06002EBC RID: 11964 RVA: 0x000E9FD8 File Offset: 0x000E81D8
	public static AnimationCurve EaseInOutBack
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0.333334f),
				new Keyframe(0.5f, 0.5f, 5.594898f, 5.594899f, 0.333334f, 0.333334f),
				new Keyframe(1f, 1f, 0f, 0f, 0.333334f, 0f)
			});
		}
	}

	// Token: 0x170004A8 RID: 1192
	// (get) Token: 0x06002EBD RID: 11965 RVA: 0x000EA070 File Offset: 0x000E8270
	public static AnimationCurve EaseInElastic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.0143284f, 0f, 1f),
				new Keyframe(0.175f, 0f, 0f, -0.06879552f, 0.008331452f, 0.8916667f),
				new Keyframe(0.475f, 0f, -0.4081632f, -0.5503653f, 0.4083333f, 0.8666668f),
				new Keyframe(0.775f, 0f, -3.26241f, -4.402922f, 0.3916665f, 0.5916666f),
				new Keyframe(1f, 1f, 12.51956f, 0f, 0.5916666f, 0f)
			});
		}
	}

	// Token: 0x170004A9 RID: 1193
	// (get) Token: 0x06002EBE RID: 11966 RVA: 0x000EA15C File Offset: 0x000E835C
	public static AnimationCurve EaseOutElastic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 12.51956f, 0f, 0.5916667f),
				new Keyframe(0.225f, 1f, -4.402922f, -3.262408f, 0.5916666f, 0.3916667f),
				new Keyframe(0.525f, 1f, -0.5503654f, -0.4081634f, 0.8666667f, 0.4083333f),
				new Keyframe(0.825f, 1f, -0.06879558f, 0f, 0.8916666f, 0.008331367f),
				new Keyframe(1f, 1f, 0.01432861f, 0f, 1f, 0f)
			});
		}
	}

	// Token: 0x170004AA RID: 1194
	// (get) Token: 0x06002EBF RID: 11967 RVA: 0x000EA248 File Offset: 0x000E8448
	public static AnimationCurve EaseInOutElastic
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0.01433143f, 0f, 1f),
				new Keyframe(0.0875f, 0f, 0f, -0.06879253f, 0.008331452f, 0.8916667f),
				new Keyframe(0.2375f, 0f, -0.4081632f, -0.5503692f, 0.4083333f, 0.8666668f),
				new Keyframe(0.3875f, 0f, -3.262419f, -4.402895f, 0.3916665f, 0.5916712f),
				new Keyframe(0.5f, 0.5f, 12.51967f, 12.51958f, 0.5916621f, 0.5916664f),
				new Keyframe(0.6125f, 1f, -4.402927f, -3.262402f, 0.5916669f, 0.3916666f),
				new Keyframe(0.7625f, 1f, -0.5503691f, -0.4081627f, 0.8666668f, 0.4083335f),
				new Keyframe(0.9125f, 1f, -0.06879289f, 0f, 0.8916666f, 0.008331029f),
				new Keyframe(1f, 1f, 0.01432828f, 0f, 1f, 0f)
			});
		}
	}

	// Token: 0x170004AB RID: 1195
	// (get) Token: 0x06002EC0 RID: 11968 RVA: 0x000EA3DC File Offset: 0x000E85DC
	public static AnimationCurve Spring
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 3.582263f, 0f, 0.2385296f),
				new Keyframe(0.336583f, 0.828268f, 1.767519f, 1.767491f, 0.4374225f, 0.2215123f),
				new Keyframe(0.550666f, 1.079651f, 0.3095257f, 0.3095275f, 0.4695607f, 0.4154884f),
				new Keyframe(0.779498f, 0.974607f, -0.2321364f, -0.2321428f, 0.3585643f, 0.3623514f),
				new Keyframe(0.897999f, 1.003668f, 0.2797853f, 0.2797431f, 0.3331026f, 0.3306926f),
				new Keyframe(1f, 1f, -0.2023914f, 0f, 0.3296829f, 0f)
			});
		}
	}

	// Token: 0x170004AC RID: 1196
	// (get) Token: 0x06002EC1 RID: 11969 RVA: 0x000EA4F0 File Offset: 0x000E86F0
	public static AnimationCurve Linear
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 1f, 0f, 0f),
				new Keyframe(1f, 1f, 1f, 0f, 0f, 0f)
			});
		}
	}

	// Token: 0x170004AD RID: 1197
	// (get) Token: 0x06002EC2 RID: 11970 RVA: 0x000EA55C File Offset: 0x000E875C
	public static AnimationCurve Step
	{
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
				new Keyframe(0.5f, 0f, 0f, 0f, 0f, 0f),
				new Keyframe(0.5f, 1f, 0f, 0f, 0f, 0f),
				new Keyframe(1f, 1f, 0f, 0f, 0f, 0f)
			});
		}
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x000EA61C File Offset: 0x000E881C
	static AnimationCurves()
	{
		Dictionary<AnimationCurves.EaseType, AnimationCurve> dictionary = new Dictionary<AnimationCurves.EaseType, AnimationCurve>();
		dictionary[AnimationCurves.EaseType.EaseInQuad] = AnimationCurves.EaseInQuad;
		dictionary[AnimationCurves.EaseType.EaseOutQuad] = AnimationCurves.EaseOutQuad;
		dictionary[AnimationCurves.EaseType.EaseInOutQuad] = AnimationCurves.EaseInOutQuad;
		dictionary[AnimationCurves.EaseType.EaseInCubic] = AnimationCurves.EaseInCubic;
		dictionary[AnimationCurves.EaseType.EaseOutCubic] = AnimationCurves.EaseOutCubic;
		dictionary[AnimationCurves.EaseType.EaseInOutCubic] = AnimationCurves.EaseInOutCubic;
		dictionary[AnimationCurves.EaseType.EaseInQuart] = AnimationCurves.EaseInQuart;
		dictionary[AnimationCurves.EaseType.EaseOutQuart] = AnimationCurves.EaseOutQuart;
		dictionary[AnimationCurves.EaseType.EaseInOutQuart] = AnimationCurves.EaseInOutQuart;
		dictionary[AnimationCurves.EaseType.EaseInQuint] = AnimationCurves.EaseInQuint;
		dictionary[AnimationCurves.EaseType.EaseOutQuint] = AnimationCurves.EaseOutQuint;
		dictionary[AnimationCurves.EaseType.EaseInOutQuint] = AnimationCurves.EaseInOutQuint;
		dictionary[AnimationCurves.EaseType.EaseInSine] = AnimationCurves.EaseInSine;
		dictionary[AnimationCurves.EaseType.EaseOutSine] = AnimationCurves.EaseOutSine;
		dictionary[AnimationCurves.EaseType.EaseInOutSine] = AnimationCurves.EaseInOutSine;
		dictionary[AnimationCurves.EaseType.EaseInExpo] = AnimationCurves.EaseInExpo;
		dictionary[AnimationCurves.EaseType.EaseOutExpo] = AnimationCurves.EaseOutExpo;
		dictionary[AnimationCurves.EaseType.EaseInOutExpo] = AnimationCurves.EaseInOutExpo;
		dictionary[AnimationCurves.EaseType.EaseInCirc] = AnimationCurves.EaseInCirc;
		dictionary[AnimationCurves.EaseType.EaseOutCirc] = AnimationCurves.EaseOutCirc;
		dictionary[AnimationCurves.EaseType.EaseInOutCirc] = AnimationCurves.EaseInOutCirc;
		dictionary[AnimationCurves.EaseType.EaseInBounce] = AnimationCurves.EaseInBounce;
		dictionary[AnimationCurves.EaseType.EaseOutBounce] = AnimationCurves.EaseOutBounce;
		dictionary[AnimationCurves.EaseType.EaseInOutBounce] = AnimationCurves.EaseInOutBounce;
		dictionary[AnimationCurves.EaseType.EaseInBack] = AnimationCurves.EaseInBack;
		dictionary[AnimationCurves.EaseType.EaseOutBack] = AnimationCurves.EaseOutBack;
		dictionary[AnimationCurves.EaseType.EaseInOutBack] = AnimationCurves.EaseInOutBack;
		dictionary[AnimationCurves.EaseType.EaseInElastic] = AnimationCurves.EaseInElastic;
		dictionary[AnimationCurves.EaseType.EaseOutElastic] = AnimationCurves.EaseOutElastic;
		dictionary[AnimationCurves.EaseType.EaseInOutElastic] = AnimationCurves.EaseInOutElastic;
		dictionary[AnimationCurves.EaseType.Spring] = AnimationCurves.Spring;
		dictionary[AnimationCurves.EaseType.Linear] = AnimationCurves.Linear;
		dictionary[AnimationCurves.EaseType.Step] = AnimationCurves.Step;
		AnimationCurves.gEaseTypeToCurve = dictionary;
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x000EA7D8 File Offset: 0x000E89D8
	public static AnimationCurve GetCurveForEase(AnimationCurves.EaseType ease)
	{
		return AnimationCurves.gEaseTypeToCurve[ease];
	}

	// Token: 0x04003529 RID: 13609
	private static Dictionary<AnimationCurves.EaseType, AnimationCurve> gEaseTypeToCurve;

	// Token: 0x0200074B RID: 1867
	public enum EaseType
	{
		// Token: 0x0400352B RID: 13611
		EaseInQuad = 1,
		// Token: 0x0400352C RID: 13612
		EaseOutQuad,
		// Token: 0x0400352D RID: 13613
		EaseInOutQuad,
		// Token: 0x0400352E RID: 13614
		EaseInCubic,
		// Token: 0x0400352F RID: 13615
		EaseOutCubic,
		// Token: 0x04003530 RID: 13616
		EaseInOutCubic,
		// Token: 0x04003531 RID: 13617
		EaseInQuart,
		// Token: 0x04003532 RID: 13618
		EaseOutQuart,
		// Token: 0x04003533 RID: 13619
		EaseInOutQuart,
		// Token: 0x04003534 RID: 13620
		EaseInQuint,
		// Token: 0x04003535 RID: 13621
		EaseOutQuint,
		// Token: 0x04003536 RID: 13622
		EaseInOutQuint,
		// Token: 0x04003537 RID: 13623
		EaseInSine,
		// Token: 0x04003538 RID: 13624
		EaseOutSine,
		// Token: 0x04003539 RID: 13625
		EaseInOutSine,
		// Token: 0x0400353A RID: 13626
		EaseInExpo,
		// Token: 0x0400353B RID: 13627
		EaseOutExpo,
		// Token: 0x0400353C RID: 13628
		EaseInOutExpo,
		// Token: 0x0400353D RID: 13629
		EaseInCirc,
		// Token: 0x0400353E RID: 13630
		EaseOutCirc,
		// Token: 0x0400353F RID: 13631
		EaseInOutCirc,
		// Token: 0x04003540 RID: 13632
		EaseInBounce,
		// Token: 0x04003541 RID: 13633
		EaseOutBounce,
		// Token: 0x04003542 RID: 13634
		EaseInOutBounce,
		// Token: 0x04003543 RID: 13635
		EaseInBack,
		// Token: 0x04003544 RID: 13636
		EaseOutBack,
		// Token: 0x04003545 RID: 13637
		EaseInOutBack,
		// Token: 0x04003546 RID: 13638
		EaseInElastic,
		// Token: 0x04003547 RID: 13639
		EaseOutElastic,
		// Token: 0x04003548 RID: 13640
		EaseInOutElastic,
		// Token: 0x04003549 RID: 13641
		Spring,
		// Token: 0x0400354A RID: 13642
		Linear,
		// Token: 0x0400354B RID: 13643
		Step
	}
}
