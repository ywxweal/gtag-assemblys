using System;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000A3B RID: 2619
[Serializable]
public struct ZoneNode : IEquatable<ZoneNode>
{
	// Token: 0x1700061E RID: 1566
	// (get) Token: 0x06003E4B RID: 15947 RVA: 0x00127BC3 File Offset: 0x00125DC3
	public static ZoneNode Null { get; } = new ZoneNode
	{
		zoneId = GTZone.none,
		subZoneId = GTSubZone.none,
		isValid = false
	};

	// Token: 0x1700061F RID: 1567
	// (get) Token: 0x06003E4C RID: 15948 RVA: 0x00127BCA File Offset: 0x00125DCA
	public int zoneKey
	{
		get
		{
			return StaticHash.Compute((int)this.zoneId, (int)this.subZoneId);
		}
	}

	// Token: 0x06003E4D RID: 15949 RVA: 0x00127BDD File Offset: 0x00125DDD
	public bool ContainsPoint(Vector3 point)
	{
		return MathUtils.OrientedBoxContains(point, this.center, this.size, this.orientation);
	}

	// Token: 0x06003E4E RID: 15950 RVA: 0x00127BF7 File Offset: 0x00125DF7
	public int SphereOverlap(Vector3 position, float radius)
	{
		return MathUtils.OrientedBoxSphereOverlap(position, radius, this.center, this.size, this.orientation);
	}

	// Token: 0x06003E4F RID: 15951 RVA: 0x00127C12 File Offset: 0x00125E12
	public override string ToString()
	{
		if (this.subZoneId != GTSubZone.none)
		{
			return ZString.Concat<GTZone, string, GTSubZone>(this.zoneId, ".", this.subZoneId);
		}
		return ZString.Concat<GTZone>(this.zoneId);
	}

	// Token: 0x06003E50 RID: 15952 RVA: 0x00127C40 File Offset: 0x00125E40
	public override int GetHashCode()
	{
		int zoneKey = this.zoneKey;
		int hashCode = this.center.QuantizedId128().GetHashCode();
		int hashCode2 = this.size.QuantizedId128().GetHashCode();
		int hashCode3 = this.orientation.QuantizedId128().GetHashCode();
		return StaticHash.Compute(zoneKey, hashCode, hashCode2, hashCode3);
	}

	// Token: 0x06003E51 RID: 15953 RVA: 0x00127CA9 File Offset: 0x00125EA9
	public static bool operator ==(ZoneNode x, ZoneNode y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003E52 RID: 15954 RVA: 0x00127CB3 File Offset: 0x00125EB3
	public static bool operator !=(ZoneNode x, ZoneNode y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06003E53 RID: 15955 RVA: 0x00127CC0 File Offset: 0x00125EC0
	public bool Equals(ZoneNode other)
	{
		return this.zoneId == other.zoneId && this.subZoneId == other.subZoneId && this.center.Approx(other.center, 1E-05f) && this.size.Approx(other.size, 1E-05f) && this.orientation.Approx(other.orientation, 1E-06f);
	}

	// Token: 0x06003E54 RID: 15956 RVA: 0x00127D34 File Offset: 0x00125F34
	public override bool Equals(object obj)
	{
		if (obj is ZoneNode)
		{
			ZoneNode zoneNode = (ZoneNode)obj;
			return this.Equals(zoneNode);
		}
		return false;
	}

	// Token: 0x040042DA RID: 17114
	public GTZone zoneId;

	// Token: 0x040042DB RID: 17115
	public GTSubZone subZoneId;

	// Token: 0x040042DC RID: 17116
	public Vector3 center;

	// Token: 0x040042DD RID: 17117
	public Vector3 size;

	// Token: 0x040042DE RID: 17118
	public Quaternion orientation;

	// Token: 0x040042DF RID: 17119
	public Bounds AABB;

	// Token: 0x040042E0 RID: 17120
	public bool isValid;
}
