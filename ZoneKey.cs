using System;

// Token: 0x02000A3A RID: 2618
[Serializable]
public struct ZoneKey : IEquatable<ZoneKey>, IComparable<ZoneKey>, IComparable
{
	// Token: 0x1700061B RID: 1563
	// (get) Token: 0x06003E37 RID: 15927 RVA: 0x001279EE File Offset: 0x00125BEE
	public int intValue
	{
		get
		{
			return ZoneKey.ToIntValue(this.zoneId, this.subZoneId);
		}
	}

	// Token: 0x1700061C RID: 1564
	// (get) Token: 0x06003E38 RID: 15928 RVA: 0x00127A01 File Offset: 0x00125C01
	public string zoneName
	{
		get
		{
			return this.zoneId.GetName<GTZone>();
		}
	}

	// Token: 0x1700061D RID: 1565
	// (get) Token: 0x06003E39 RID: 15929 RVA: 0x00127A0E File Offset: 0x00125C0E
	public string subZoneName
	{
		get
		{
			return this.subZoneId.GetName<GTSubZone>();
		}
	}

	// Token: 0x06003E3A RID: 15930 RVA: 0x00127A1B File Offset: 0x00125C1B
	public ZoneKey(GTZone zone, GTSubZone subZone)
	{
		this.zoneId = zone;
		this.subZoneId = subZone;
	}

	// Token: 0x06003E3B RID: 15931 RVA: 0x00127A2B File Offset: 0x00125C2B
	public override int GetHashCode()
	{
		return this.intValue;
	}

	// Token: 0x06003E3C RID: 15932 RVA: 0x00127A33 File Offset: 0x00125C33
	public override string ToString()
	{
		return string.Concat(new string[] { "ZoneKey { ", this.zoneName, " : ", this.subZoneName, " }" });
	}

	// Token: 0x06003E3D RID: 15933 RVA: 0x00127A6A File Offset: 0x00125C6A
	public static ZoneKey GetKey(GTZone zone, GTSubZone subZone)
	{
		return new ZoneKey(zone, subZone);
	}

	// Token: 0x06003E3E RID: 15934 RVA: 0x00127A73 File Offset: 0x00125C73
	public static int ToIntValue(GTZone zone, GTSubZone subZone)
	{
		if (zone == GTZone.none && subZone == GTSubZone.none)
		{
			return 0;
		}
		return StaticHash.Compute(zone.GetLongValue<GTZone>(), subZone.GetLongValue<GTSubZone>());
	}

	// Token: 0x06003E3F RID: 15935 RVA: 0x00127A90 File Offset: 0x00125C90
	public bool Equals(ZoneKey other)
	{
		return this.intValue == other.intValue && this.zoneId == other.zoneId && this.subZoneId == other.subZoneId;
	}

	// Token: 0x06003E40 RID: 15936 RVA: 0x00127AC0 File Offset: 0x00125CC0
	public override bool Equals(object obj)
	{
		if (obj is ZoneKey)
		{
			ZoneKey zoneKey = (ZoneKey)obj;
			return this.Equals(zoneKey);
		}
		return false;
	}

	// Token: 0x06003E41 RID: 15937 RVA: 0x00127AE5 File Offset: 0x00125CE5
	public static bool operator ==(ZoneKey x, ZoneKey y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003E42 RID: 15938 RVA: 0x00127AEF File Offset: 0x00125CEF
	public static bool operator !=(ZoneKey x, ZoneKey y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06003E43 RID: 15939 RVA: 0x00127AFC File Offset: 0x00125CFC
	public int CompareTo(ZoneKey other)
	{
		int num = this.intValue.CompareTo(other.intValue);
		if (num == 0)
		{
			num = string.CompareOrdinal(this.zoneName, other.zoneName);
		}
		if (num == 0)
		{
			num = string.CompareOrdinal(this.subZoneName, other.subZoneName);
		}
		return num;
	}

	// Token: 0x06003E44 RID: 15940 RVA: 0x00127B4C File Offset: 0x00125D4C
	public int CompareTo(object obj)
	{
		if (obj is ZoneKey)
		{
			ZoneKey zoneKey = (ZoneKey)obj;
			return this.CompareTo(zoneKey);
		}
		return 1;
	}

	// Token: 0x06003E45 RID: 15941 RVA: 0x00127B71 File Offset: 0x00125D71
	public static bool operator <(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) < 0;
	}

	// Token: 0x06003E46 RID: 15942 RVA: 0x00127B7E File Offset: 0x00125D7E
	public static bool operator >(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) > 0;
	}

	// Token: 0x06003E47 RID: 15943 RVA: 0x00127B8B File Offset: 0x00125D8B
	public static bool operator <=(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) <= 0;
	}

	// Token: 0x06003E48 RID: 15944 RVA: 0x00127B9B File Offset: 0x00125D9B
	public static bool operator >=(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) >= 0;
	}

	// Token: 0x06003E49 RID: 15945 RVA: 0x00127BAB File Offset: 0x00125DAB
	public static explicit operator int(ZoneKey key)
	{
		return key.intValue;
	}

	// Token: 0x040042D7 RID: 17111
	public GTZone zoneId;

	// Token: 0x040042D8 RID: 17112
	public GTSubZone subZoneId;

	// Token: 0x040042D9 RID: 17113
	public static readonly ZoneKey Null = new ZoneKey(GTZone.none, GTSubZone.none);
}
