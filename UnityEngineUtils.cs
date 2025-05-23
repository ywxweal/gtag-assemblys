﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020009D7 RID: 2519
public static class UnityEngineUtils
{
	// Token: 0x06003C47 RID: 15431 RVA: 0x00120162 File Offset: 0x0011E362
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EqualsColor(this Color32 c, Color32 other)
	{
		return c.r == other.r && c.g == other.g && c.b == other.b && c.a == other.a;
	}

	// Token: 0x06003C48 RID: 15432 RVA: 0x001201A0 File Offset: 0x0011E3A0
	public static Color32 IdToColor32(this Object obj, int alpha = -1, bool distinct = true)
	{
		if (!(obj == null))
		{
			return obj.GetInstanceID().IdToColor32(alpha, distinct);
		}
		return default(Color32);
	}

	// Token: 0x06003C49 RID: 15433 RVA: 0x001201D0 File Offset: 0x0011E3D0
	public unsafe static Color32 IdToColor32(this int id, int alpha = -1, bool distinct = true)
	{
		if (distinct)
		{
			id = StaticHash.ComputeTriple32(id);
		}
		Color32 color = *Unsafe.As<int, Color32>(ref id);
		if (alpha > -1)
		{
			color.a = (byte)Math.Clamp(alpha, 0, 255);
		}
		return color;
	}

	// Token: 0x06003C4A RID: 15434 RVA: 0x00120210 File Offset: 0x0011E410
	public static Color32 ToHighViz(this Color32 c)
	{
		float num;
		float num2;
		float num3;
		Color.RGBToHSV(c, out num, out num2, out num3);
		return Color.HSVToRGB(num, 1f, 1f);
	}

	// Token: 0x06003C4B RID: 15435 RVA: 0x00120244 File Offset: 0x0011E444
	public unsafe static int Color32ToId(this Color32 c, bool distinct = true)
	{
		int num = *Unsafe.As<Color32, int>(ref c);
		if (distinct)
		{
			num = StaticHash.ReverseTriple32(num);
		}
		return num;
	}

	// Token: 0x06003C4C RID: 15436 RVA: 0x00120268 File Offset: 0x0011E468
	public static Hash128 QuantizedHash128(this Matrix4x4 m)
	{
		Hash128 hash = default(Hash128);
		HashUtilities.QuantisedMatrixHash(ref m, ref hash);
		return hash;
	}

	// Token: 0x06003C4D RID: 15437 RVA: 0x00120288 File Offset: 0x0011E488
	public static Hash128 QuantizedHash128(this Vector3 v)
	{
		Hash128 hash = default(Hash128);
		HashUtilities.QuantisedVectorHash(ref v, ref hash);
		return hash;
	}

	// Token: 0x06003C4E RID: 15438 RVA: 0x001202A7 File Offset: 0x0011E4A7
	public static Id128 QuantizedId128(this Vector3 v)
	{
		return v.QuantizedHash128();
	}

	// Token: 0x06003C4F RID: 15439 RVA: 0x001202B4 File Offset: 0x0011E4B4
	public static Id128 QuantizedId128(this Matrix4x4 m)
	{
		return m.QuantizedHash128();
	}

	// Token: 0x06003C50 RID: 15440 RVA: 0x001202C4 File Offset: 0x0011E4C4
	public static Id128 QuantizedId128(this Quaternion q)
	{
		int num = (int)((double)q.x * 1000.0 + 0.5);
		int num2 = (int)((double)q.y * 1000.0 + 0.5);
		int num3 = (int)((double)q.z * 1000.0 + 0.5);
		int num4 = (int)((double)q.w * 1000.0 + 0.5);
		return new Id128(num, num2, num3, num4);
	}

	// Token: 0x06003C51 RID: 15441 RVA: 0x0012034C File Offset: 0x0011E54C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long QuantizedHash64(this Vector4 v)
	{
		int num = (int)((double)v.x * 1000.0 + 0.5);
		int num2 = (int)((double)v.y * 1000.0 + 0.5);
		int num3 = (int)((double)v.z * 1000.0 + 0.5);
		int num4 = (int)((double)v.w * 1000.0 + 0.5);
		ulong num5 = UnityEngineUtils.MergeTo64(num, num2);
		ulong num6 = UnityEngineUtils.MergeTo64(num3, num4);
		return StaticHash.Compute128To64(num5, num6);
	}

	// Token: 0x06003C52 RID: 15442 RVA: 0x001203E0 File Offset: 0x0011E5E0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static long QuantizedHash64(this Matrix4x4 m)
	{
		m4x4 m4x = *m4x4.From(ref m);
		long num = m4x.r0.QuantizedHash64();
		long num2 = m4x.r1.QuantizedHash64();
		long num3 = m4x.r2.QuantizedHash64();
		long num4 = m4x.r3.QuantizedHash64();
		long num5 = StaticHash.Compute128To64(num, num2);
		long num6 = StaticHash.Compute128To64(num3, num4);
		return StaticHash.Compute128To64(num5, num6);
	}

	// Token: 0x06003C53 RID: 15443 RVA: 0x00120440 File Offset: 0x0011E640
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong MergeTo64(int a, int b)
	{
		return ((ulong)b << 32) | (ulong)a;
	}

	// Token: 0x06003C54 RID: 15444 RVA: 0x00120457 File Offset: 0x0011E657
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector4 ToVector(this Quaternion q)
	{
		return *Unsafe.As<Quaternion, Vector4>(ref q);
	}

	// Token: 0x06003C55 RID: 15445 RVA: 0x00120465 File Offset: 0x0011E665
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo(this Quaternion q, ref Vector4 v)
	{
		v.x = q.x;
		v.y = q.y;
		v.z = q.z;
		v.w = q.w;
	}
}
