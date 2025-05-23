using System;
using MathGeoLib;
using UnityEngine;

// Token: 0x02000A35 RID: 2613
[Serializable]
public struct BoundsInfo
{
	// Token: 0x17000610 RID: 1552
	// (get) Token: 0x06003E0E RID: 15886 RVA: 0x00126DE7 File Offset: 0x00124FE7
	public Vector3 sizeComputed
	{
		get
		{
			return Vector3.Scale(this.size, this.scale) * this.inflate;
		}
	}

	// Token: 0x17000611 RID: 1553
	// (get) Token: 0x06003E0F RID: 15887 RVA: 0x00126E05 File Offset: 0x00125005
	public Vector3 sizeComputedAA
	{
		get
		{
			return Vector3.Scale(this.sizeAA, this.scaleAA) * this.inflateAA;
		}
	}

	// Token: 0x06003E10 RID: 15888 RVA: 0x00126E24 File Offset: 0x00125024
	public static BoundsInfo ComputeBounds(Vector3[] vertices)
	{
		if (vertices.Length == 0)
		{
			return default(BoundsInfo);
		}
		OrientedBoundingBox orientedBoundingBox = OrientedBoundingBox.BruteEnclosing(vertices);
		Vector4 vector = orientedBoundingBox.Axis1;
		Vector4 vector2 = orientedBoundingBox.Axis2;
		Vector4 vector3 = orientedBoundingBox.Axis3;
		Vector4 vector4 = new Vector4(0f, 0f, 0f, 1f);
		BoundsInfo boundsInfo = default(BoundsInfo);
		boundsInfo.center = orientedBoundingBox.Center;
		boundsInfo.size = orientedBoundingBox.Extent * 2f;
		boundsInfo.rotation = new Matrix4x4(vector, vector2, vector3, vector4).rotation;
		boundsInfo.scale = Vector3.one;
		boundsInfo.inflate = 1f;
		Bounds bounds = GeometryUtility.CalculateBounds(vertices, Matrix4x4.identity);
		boundsInfo.centerAA = bounds.center;
		boundsInfo.sizeAA = bounds.size;
		boundsInfo.scaleAA = Vector3.one;
		boundsInfo.inflateAA = 1f;
		return boundsInfo;
	}

	// Token: 0x06003E11 RID: 15889 RVA: 0x00126F28 File Offset: 0x00125128
	public static BoxCollider CreateBoxCollider(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int hashCode3 = bounds.rotation.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2, hashCode3);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.center;
		transform.rotation = bounds.rotation;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputed;
		return boxCollider;
	}

	// Token: 0x06003E12 RID: 15890 RVA: 0x00126FD4 File Offset: 0x001251D4
	public static BoxCollider CreateBoxColliderAA(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.centerAA;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputedAA;
		return boxCollider;
	}

	// Token: 0x0400429F RID: 17055
	public Vector3 center;

	// Token: 0x040042A0 RID: 17056
	public Vector3 size;

	// Token: 0x040042A1 RID: 17057
	public Quaternion rotation;

	// Token: 0x040042A2 RID: 17058
	public Vector3 scale;

	// Token: 0x040042A3 RID: 17059
	public float inflate;

	// Token: 0x040042A4 RID: 17060
	[Space]
	public Vector3 centerAA;

	// Token: 0x040042A5 RID: 17061
	public Vector3 sizeAA;

	// Token: 0x040042A6 RID: 17062
	public Vector3 scaleAA;

	// Token: 0x040042A7 RID: 17063
	public float inflateAA;
}
