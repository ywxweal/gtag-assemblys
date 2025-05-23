using System;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000A3B RID: 2619
[Serializable]
public struct ZoneNode : IEquatable<ZoneNode>
{
	// Token: 0x1700061E RID: 1566
	// (get) Token: 0x06003E4A RID: 15946 RVA: 0x00127AEB File Offset: 0x00125CEB
	public static ZoneNode Null { get; } = new ZoneNode
	{
		zoneId = GTZone.none,
		subZoneId = GTSubZone.none,
		isValid = false
	};

	// Token: 0x1700061F RID: 1567
	// (get) Token: 0x06003E4B RID: 15947 RVA: 0x00127AF2 File Offset: 0x00125CF2
	public int zoneKey
	{
		get
		{
			return StaticHash.Compute((int)this.zoneId, (int)this.subZoneId);
		}
	}

	// Token: 0x06003E4C RID: 15948 RVA: 0x00127B05 File Offset: 0x00125D05
	public bool ContainsPoint(Vector3 point)
	{
		return MathUtils.OrientedBoxContains(point, this.center, this.size, this.orientation);
	}

	// Token: 0x06003E4D RID: 15949 RVA: 0x00127B1F File Offset: 0x00125D1F
	public int SphereOverlap(Vector3 position, float radius)
	{
		return MathUtils.OrientedBoxSphereOverlap(position, radius, this.center, this.size, this.orientation);
	}

	// Token: 0x06003E4E RID: 15950 RVA: 0x00127B3A File Offset: 0x00125D3A
	public override string ToString()
	{
		if (this.subZoneId != GTSubZone.none)
		{
			return ZString.Concat<GTZone, string, GTSubZone>(this.zoneId, ".", this.subZoneId);
		}
		return ZString.Concat<GTZone>(this.zoneId);
	}

	// Token: 0x06003E4F RID: 15951 RVA: 0x00127B68 File Offset: 0x00125D68
	public override int GetHashCode()
	{
		int zoneKey = this.zoneKey;
		int hashCode = this.center.QuantizedId128().GetHashCode();
		int hashCode2 = this.size.QuantizedId128().GetHashCode();
		int hashCode3 = this.orientation.QuantizedId128().GetHashCode();
		return StaticHash.Compute(zoneKey, hashCode, hashCode2, hashCode3);
	}

	// Token: 0x06003E50 RID: 15952 RVA: 0x00127BD1 File Offset: 0x00125DD1
	public static bool operator ==(ZoneNode x, ZoneNode y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003E51 RID: 15953 RVA: 0x00127BDB File Offset: 0x00125DDB
	public static bool operator !=(ZoneNode x, ZoneNode y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06003E52 RID: 15954 RVA: 0x00127BE8 File Offset: 0x00125DE8
	public bool Equals(ZoneNode other)
	{
		return this.zoneId == other.zoneId && this.subZoneId == other.subZoneId && this.center.Approx(other.center, 1E-05f) && this.size.Approx(other.size, 1E-05f) && this.orientation.Approx(other.orientation, 1E-06f);
	}

	// Token: 0x06003E53 RID: 15955 RVA: 0x00127C5C File Offset: 0x00125E5C
	public override bool Equals(object obj)
	{
		if (obj is ZoneNode)
		{
			ZoneNode zoneNode = (ZoneNode)obj;
			return this.Equals(zoneNode);
		}
		return false;
	}

	// Token: 0x040042D9 RID: 17113
	public GTZone zoneId;

	// Token: 0x040042DA RID: 17114
	public GTSubZone subZoneId;

	// Token: 0x040042DB RID: 17115
	public Vector3 center;

	// Token: 0x040042DC RID: 17116
	public Vector3 size;

	// Token: 0x040042DD RID: 17117
	public Quaternion orientation;

	// Token: 0x040042DE RID: 17118
	public Bounds AABB;

	// Token: 0x040042DF RID: 17119
	public bool isValid;
}
