using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E81 RID: 3713
	public class Codec
	{
		// Token: 0x06005CBE RID: 23742 RVA: 0x001CA5C8 File Offset: 0x001C87C8
		public static float PackSaturated(float a, float b)
		{
			a = Mathf.Floor(a * 4095f);
			b = Mathf.Floor(b * 4095f);
			return a * 4096f + b;
		}

		// Token: 0x06005CBF RID: 23743 RVA: 0x001CA5EF File Offset: 0x001C87EF
		public static float PackSaturated(Vector2 v)
		{
			return Codec.PackSaturated(v.x, v.y);
		}

		// Token: 0x06005CC0 RID: 23744 RVA: 0x001CA602 File Offset: 0x001C8802
		public static Vector2 UnpackSaturated(float f)
		{
			return new Vector2(Mathf.Floor(f / 4096f), Mathf.Repeat(f, 4096f)) / 4095f;
		}

		// Token: 0x06005CC1 RID: 23745 RVA: 0x001CA62C File Offset: 0x001C882C
		public static Vector2 OctWrap(Vector2 v)
		{
			return (Vector2.one - new Vector2(Mathf.Abs(v.y), Mathf.Abs(v.x))) * new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
		}

		// Token: 0x06005CC2 RID: 23746 RVA: 0x001CA680 File Offset: 0x001C8880
		public static float PackNormal(Vector3 n)
		{
			n /= Mathf.Abs(n.x) + Mathf.Abs(n.y) + Mathf.Abs(n.z);
			return Codec.PackSaturated(((n.z >= 0f) ? new Vector2(n.x, n.y) : Codec.OctWrap(new Vector2(n.x, n.y))) * 0.5f + 0.5f * Vector2.one);
		}

		// Token: 0x06005CC3 RID: 23747 RVA: 0x001CA714 File Offset: 0x001C8914
		public static Vector3 UnpackNormal(float f)
		{
			Vector2 vector = Codec.UnpackSaturated(f);
			vector = vector * 2f - Vector2.one;
			Vector3 vector2 = new Vector3(vector.x, vector.y, 1f - Mathf.Abs(vector.x) - Mathf.Abs(vector.y));
			float num = Mathf.Clamp01(-vector2.z);
			vector2.x += ((vector2.x >= 0f) ? (-num) : num);
			vector2.y += ((vector2.y >= 0f) ? (-num) : num);
			return vector2.normalized;
		}

		// Token: 0x06005CC4 RID: 23748 RVA: 0x001CA7BC File Offset: 0x001C89BC
		public static uint PackRgb(Color color)
		{
			return ((uint)(color.b * 255f) << 16) | ((uint)(color.g * 255f) << 8) | (uint)(color.r * 255f);
		}

		// Token: 0x06005CC5 RID: 23749 RVA: 0x001CA7EC File Offset: 0x001C89EC
		public static Color UnpackRgb(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f);
		}

		// Token: 0x06005CC6 RID: 23750 RVA: 0x001CA828 File Offset: 0x001C8A28
		public static uint PackRgba(Color color)
		{
			return ((uint)(color.a * 255f) << 24) | ((uint)(color.b * 255f) << 16) | ((uint)(color.g * 255f) << 8) | (uint)(color.r * 255f);
		}

		// Token: 0x06005CC7 RID: 23751 RVA: 0x001CA874 File Offset: 0x001C8A74
		public static Color UnpackRgba(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f, ((i & 4278190080U) >> 24) / 255f);
		}

		// Token: 0x06005CC8 RID: 23752 RVA: 0x001CA8CA File Offset: 0x001C8ACA
		public static uint Pack8888(uint x, uint y, uint z, uint w)
		{
			return ((x & 255U) << 24) | ((y & 255U) << 16) | ((z & 255U) << 8) | (w & 255U);
		}

		// Token: 0x06005CC9 RID: 23753 RVA: 0x001CA8F3 File Offset: 0x001C8AF3
		public static void Unpack8888(uint i, out uint x, out uint y, out uint z, out uint w)
		{
			x = (i >> 24) & 255U;
			y = (i >> 16) & 255U;
			z = (i >> 8) & 255U;
			w = i & 255U;
		}

		// Token: 0x06005CCA RID: 23754 RVA: 0x001CA924 File Offset: 0x001C8B24
		private static int IntReinterpret(float f)
		{
			return new Codec.IntFloat
			{
				FloatValue = f
			}.IntValue;
		}

		// Token: 0x06005CCB RID: 23755 RVA: 0x001CA947 File Offset: 0x001C8B47
		public static int HashConcat(int hash, int i)
		{
			return (hash ^ i) * Codec.FnvPrime;
		}

		// Token: 0x06005CCC RID: 23756 RVA: 0x001CA952 File Offset: 0x001C8B52
		public static int HashConcat(int hash, long i)
		{
			hash = Codec.HashConcat(hash, (int)(i & (long)((ulong)(-1))));
			hash = Codec.HashConcat(hash, (int)(i >> 32));
			return hash;
		}

		// Token: 0x06005CCD RID: 23757 RVA: 0x001CA96F File Offset: 0x001C8B6F
		public static int HashConcat(int hash, float f)
		{
			return Codec.HashConcat(hash, Codec.IntReinterpret(f));
		}

		// Token: 0x06005CCE RID: 23758 RVA: 0x001CA97D File Offset: 0x001C8B7D
		public static int HashConcat(int hash, bool b)
		{
			return Codec.HashConcat(hash, b ? 1 : 0);
		}

		// Token: 0x06005CCF RID: 23759 RVA: 0x001CA98C File Offset: 0x001C8B8C
		public static int HashConcat(int hash, params int[] ints)
		{
			foreach (int num in ints)
			{
				hash = Codec.HashConcat(hash, num);
			}
			return hash;
		}

		// Token: 0x06005CD0 RID: 23760 RVA: 0x001CA9B8 File Offset: 0x001C8BB8
		public static int HashConcat(int hash, params float[] floats)
		{
			foreach (float num in floats)
			{
				hash = Codec.HashConcat(hash, num);
			}
			return hash;
		}

		// Token: 0x06005CD1 RID: 23761 RVA: 0x001CA9E3 File Offset: 0x001C8BE3
		public static int HashConcat(int hash, Vector2 v)
		{
			return Codec.HashConcat(hash, new float[] { v.x, v.y });
		}

		// Token: 0x06005CD2 RID: 23762 RVA: 0x001CAA03 File Offset: 0x001C8C03
		public static int HashConcat(int hash, Vector3 v)
		{
			return Codec.HashConcat(hash, new float[] { v.x, v.y, v.z });
		}

		// Token: 0x06005CD3 RID: 23763 RVA: 0x001CAA2C File Offset: 0x001C8C2C
		public static int HashConcat(int hash, Vector4 v)
		{
			return Codec.HashConcat(hash, new float[] { v.x, v.y, v.z, v.w });
		}

		// Token: 0x06005CD4 RID: 23764 RVA: 0x001CAA5E File Offset: 0x001C8C5E
		public static int HashConcat(int hash, Quaternion q)
		{
			return Codec.HashConcat(hash, new float[] { q.x, q.y, q.z, q.w });
		}

		// Token: 0x06005CD5 RID: 23765 RVA: 0x001CAA90 File Offset: 0x001C8C90
		public static int HashConcat(int hash, Color c)
		{
			return Codec.HashConcat(hash, new float[] { c.r, c.g, c.b, c.a });
		}

		// Token: 0x06005CD6 RID: 23766 RVA: 0x001CAAC2 File Offset: 0x001C8CC2
		public static int HashConcat(int hash, Transform t)
		{
			return Codec.HashConcat(hash, t.GetHashCode());
		}

		// Token: 0x06005CD7 RID: 23767 RVA: 0x001CAAD0 File Offset: 0x001C8CD0
		public static int Hash(int i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06005CD8 RID: 23768 RVA: 0x001CAADD File Offset: 0x001C8CDD
		public static int Hash(long i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06005CD9 RID: 23769 RVA: 0x001CAAEA File Offset: 0x001C8CEA
		public static int Hash(float f)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, f);
		}

		// Token: 0x06005CDA RID: 23770 RVA: 0x001CAAF7 File Offset: 0x001C8CF7
		public static int Hash(bool b)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, b);
		}

		// Token: 0x06005CDB RID: 23771 RVA: 0x001CAB04 File Offset: 0x001C8D04
		public static int Hash(params int[] ints)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, ints);
		}

		// Token: 0x06005CDC RID: 23772 RVA: 0x001CAB11 File Offset: 0x001C8D11
		public static int Hash(params float[] floats)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, floats);
		}

		// Token: 0x06005CDD RID: 23773 RVA: 0x001CAB1E File Offset: 0x001C8D1E
		public static int Hash(Vector2 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06005CDE RID: 23774 RVA: 0x001CAB2B File Offset: 0x001C8D2B
		public static int Hash(Vector3 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06005CDF RID: 23775 RVA: 0x001CAB38 File Offset: 0x001C8D38
		public static int Hash(Vector4 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06005CE0 RID: 23776 RVA: 0x001CAB45 File Offset: 0x001C8D45
		public static int Hash(Quaternion q)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, q);
		}

		// Token: 0x06005CE1 RID: 23777 RVA: 0x001CAB52 File Offset: 0x001C8D52
		public static int Hash(Color c)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, c);
		}

		// Token: 0x06005CE2 RID: 23778 RVA: 0x001CAB60 File Offset: 0x001C8D60
		private static int HashTransformHierarchyRecurvsive(int hash, Transform t)
		{
			hash = Codec.HashConcat(hash, t);
			hash = Codec.HashConcat(hash, t.childCount);
			for (int i = 0; i < t.childCount; i++)
			{
				hash = Codec.HashTransformHierarchyRecurvsive(hash, t.GetChild(i));
			}
			return hash;
		}

		// Token: 0x06005CE3 RID: 23779 RVA: 0x001CABA5 File Offset: 0x001C8DA5
		public static int HashTransformHierarchy(Transform t)
		{
			return Codec.HashTransformHierarchyRecurvsive(Codec.FnvDefaultBasis, t);
		}

		// Token: 0x04006110 RID: 24848
		public static readonly int FnvDefaultBasis = -2128831035;

		// Token: 0x04006111 RID: 24849
		public static readonly int FnvPrime = 16777619;

		// Token: 0x02000E82 RID: 3714
		[StructLayout(LayoutKind.Explicit)]
		private struct IntFloat
		{
			// Token: 0x04006112 RID: 24850
			[FieldOffset(0)]
			public int IntValue;

			// Token: 0x04006113 RID: 24851
			[FieldOffset(0)]
			public float FloatValue;
		}
	}
}
