using System;
using System.Diagnostics;
using Cysharp.Text;
using Drawing;
using UnityEngine;

// Token: 0x020001D5 RID: 469
public static class GTDev
{
	// Token: 0x06000AF1 RID: 2801 RVA: 0x0003AB64 File Offset: 0x00038D64
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void InitializeOnLoad()
	{
		GTDev.FetchDevID();
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	public static void Log<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	public static void Log<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	public static void LogError<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	public static void LogError<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	public static void LogWarning<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	public static void LogWarning<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	public static void LogSilent<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	public static void LogSilent<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void CallEditorOnly(Action call)
	{
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000AFD RID: 2813 RVA: 0x0003AB6C File Offset: 0x00038D6C
	public static int DevID
	{
		get
		{
			return GTDev.FetchDevID();
		}
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0003AB74 File Offset: 0x00038D74
	private static int FetchDevID()
	{
		if (GTDev.gHasDevID)
		{
			return GTDev.gDevID;
		}
		int num = StaticHash.Compute(SystemInfo.deviceUniqueIdentifier);
		int num2 = StaticHash.Compute(Environment.UserDomainName);
		int num3 = StaticHash.Compute(Environment.UserName);
		int num4 = StaticHash.Compute(Application.unityVersion);
		GTDev.gDevID = StaticHash.Compute(num, num2, num3, num4);
		GTDev.gHasDevID = true;
		return GTDev.gDevID;
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	[Conditional("_GTDEV_ON_")]
	private static void _Log<T>(Action<object, Object> log, Action<object> logNoCtx, T msg, Object ctx, string channel)
	{
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0003ABD1 File Offset: 0x00038DD1
	private static Mesh SphereMesh()
	{
		if (!GTDev.gSphereMesh)
		{
			GTDev.gSphereMesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
		}
		return GTDev.gSphereMesh;
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x0003ABF4 File Offset: 0x00038DF4
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D(this Collider col, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		if (color.a.Approx0(1E-06f))
		{
			return;
		}
		Matrix4x4 localToWorldMatrix = col.transform.localToWorldMatrix;
		SRand srand = new SRand(localToWorldMatrix.QuantizedId128().GetHashCode());
		color.r = srand.NextFloat();
		color.g = srand.NextFloat();
		color.b = srand.NextFloat();
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			commandBuilder.PushMatrix(localToWorldMatrix);
			commandBuilder.PushLineWidth(2f, true);
			commandBuilder.PushColor(color);
			BoxCollider boxCollider = col as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = col as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = col as CapsuleCollider;
					if (capsuleCollider != null)
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius, color);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius, color);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size);
			}
			commandBuilder.Label2D(Vector3.zero, col.name, 16f, LabelAlignment.Center);
			commandBuilder.PopColor();
			commandBuilder.PopLineWidth();
			commandBuilder.PopMatrix();
		}
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x0003AD98 File Offset: 0x00038F98
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D(this Vector3 vec, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		else
		{
			color.a = GTDev.gDefaultColor.a;
		}
		string text = ZString.Format<float, float, float>("{{ X: {0:##0.0000}, Y: {1:##0.0000}, Z: {2:##0.0000} }}", vec.x, vec.y, vec.z);
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			using (commandBuilder.WithLineWidth(2f, true))
			{
				commandBuilder.Cross(vec, 0.64f, color);
			}
			commandBuilder.Label2D(vec + Vector3.down * 0.64f, text, 16f, LabelAlignment.Center, color);
		}
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0003AE8C File Offset: 0x0003908C
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D<T>(this T value, Vector3 position, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		string text = ZString.Concat<T>(value);
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			commandBuilder.Label2D(position, text, 16f, LabelAlignment.Center, color);
		}
	}

	// Token: 0x04000D6C RID: 3436
	[OnEnterPlay_Set(0)]
	private static int gDevID;

	// Token: 0x04000D6D RID: 3437
	[OnEnterPlay_Set(false)]
	private static bool gHasDevID;

	// Token: 0x04000D6E RID: 3438
	private static readonly Color gDefaultColor = new Color(0f, 1f, 1f, 0.32f);

	// Token: 0x04000D6F RID: 3439
	private const string kFormatF = "{{ X: {0:##0.0000}, Y: {1:##0.0000}, Z: {2:##0.0000} }}";

	// Token: 0x04000D70 RID: 3440
	private const float kDuration = 8f;

	// Token: 0x04000D71 RID: 3441
	private static Mesh gSphereMesh;
}
