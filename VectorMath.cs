using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020009E0 RID: 2528
public static class VectorMath
{
	// Token: 0x06003C6E RID: 15470 RVA: 0x0012074C File Offset: 0x0011E94C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Int Clamped(this Vector3Int v, int min, int max)
	{
		v.x = Math.Clamp(v.x, min, max);
		v.y = Math.Clamp(v.y, min, max);
		v.z = Math.Clamp(v.z, min, max);
		return v;
	}

	// Token: 0x06003C6F RID: 15471 RVA: 0x00120799 File Offset: 0x0011E999
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetXYZ(this Vector3 v, float f)
	{
		v.x = f;
		v.y = f;
		v.z = f;
	}

	// Token: 0x06003C70 RID: 15472 RVA: 0x001207B0 File Offset: 0x0011E9B0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Int Abs(this Vector3Int v)
	{
		v.x = Math.Abs(v.x);
		v.y = Math.Abs(v.y);
		v.z = Math.Abs(v.z);
		return v;
	}

	// Token: 0x06003C71 RID: 15473 RVA: 0x001207EC File Offset: 0x0011E9EC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Abs(this Vector3 v)
	{
		v.x = Math.Abs(v.x);
		v.y = Math.Abs(v.y);
		v.z = Math.Abs(v.z);
		return v;
	}

	// Token: 0x06003C72 RID: 15474 RVA: 0x00120825 File Offset: 0x0011EA25
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Min(this Vector3 v, Vector3 other)
	{
		return new Vector3(Math.Min(v.x, other.x), Math.Min(v.y, other.y), Math.Min(v.z, other.z));
	}

	// Token: 0x06003C73 RID: 15475 RVA: 0x0012085F File Offset: 0x0011EA5F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Max(this Vector3 v, Vector3 other)
	{
		return new Vector3(Math.Max(v.x, other.x), Math.Max(v.y, other.y), Math.Max(v.z, other.z));
	}

	// Token: 0x06003C74 RID: 15476 RVA: 0x00120899 File Offset: 0x0011EA99
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Add(this Vector3 v, float amount)
	{
		v.x += amount;
		v.y += amount;
		v.z += amount;
		return v;
	}

	// Token: 0x06003C75 RID: 15477 RVA: 0x001208C0 File Offset: 0x0011EAC0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Sub(this Vector3 v, float amount)
	{
		v.x -= amount;
		v.y -= amount;
		v.z -= amount;
		return v;
	}

	// Token: 0x06003C76 RID: 15478 RVA: 0x001208E7 File Offset: 0x0011EAE7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Mul(this Vector3 v, float amount)
	{
		v.x *= amount;
		v.y *= amount;
		v.z *= amount;
		return v;
	}

	// Token: 0x06003C77 RID: 15479 RVA: 0x00120910 File Offset: 0x0011EB10
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Div(this Vector3 v, float amount)
	{
		float num = 1f / amount;
		v.x *= num;
		v.y *= num;
		v.z *= num;
		return v;
	}

	// Token: 0x06003C78 RID: 15480 RVA: 0x0012094C File Offset: 0x0011EB4C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Max(this Vector3 v)
	{
		float num = Math.Max(Math.Max(v.x, v.y), v.z);
		v.x = num;
		v.y = num;
		v.z = num;
		return v;
	}

	// Token: 0x06003C79 RID: 15481 RVA: 0x00120990 File Offset: 0x0011EB90
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Max(this Vector3 v, float max)
	{
		float num = Math.Max(Math.Max(Math.Max(v.x, v.y), v.z), max);
		v.x = num;
		v.y = num;
		v.z = num;
		return v;
	}

	// Token: 0x06003C7A RID: 15482 RVA: 0x001209DC File Offset: 0x0011EBDC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 Max(this float3 v)
	{
		float num = Math.Max(v.x, Math.Max(v.y, v.z));
		v.x = num;
		v.y = num;
		v.z = num;
		return v;
	}

	// Token: 0x06003C7B RID: 15483 RVA: 0x00120A1F File Offset: 0x0011EC1F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsFinite(this Vector3 v)
	{
		return float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
	}

	// Token: 0x06003C7C RID: 15484 RVA: 0x00120A48 File Offset: 0x0011EC48
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Clamped(this Vector3 v, Vector3 min, Vector3 max)
	{
		v.x = Math.Clamp(v.x, min.x, max.x);
		v.y = Math.Clamp(v.y, min.y, max.y);
		v.z = Math.Clamp(v.z, min.z, max.z);
		return v;
	}

	// Token: 0x06003C7D RID: 15485 RVA: 0x00120AB0 File Offset: 0x0011ECB0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx0(this Vector3 v, float epsilon = 1E-05f)
	{
		float x = v.x;
		float y = v.y;
		float z = v.z;
		return x * x + y * y + z * z <= epsilon * epsilon;
	}

	// Token: 0x06003C7E RID: 15486 RVA: 0x00120AE4 File Offset: 0x0011ECE4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx1(this Vector3 v, float epsilon = 1E-05f)
	{
		float num = v.x - 1f;
		float num2 = v.y - 1f;
		float num3 = v.z - 1f;
		return num * num + num2 * num2 + num3 * num3 <= epsilon * epsilon;
	}

	// Token: 0x06003C7F RID: 15487 RVA: 0x00120B2C File Offset: 0x0011ED2C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this Vector3 a, Vector3 b, float epsilon = 1E-05f)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		return num * num + num2 * num2 + num3 * num3 <= epsilon * epsilon;
	}

	// Token: 0x06003C80 RID: 15488 RVA: 0x00120B74 File Offset: 0x0011ED74
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this Vector4 a, Vector4 b, float epsilon = 1E-05f)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		float num4 = a.w - b.w;
		return num * num + num2 * num2 + num3 * num3 + num4 * num4 <= epsilon * epsilon;
	}
}
