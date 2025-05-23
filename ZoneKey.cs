using System;

// Token: 0x02000A3A RID: 2618
[Serializable]
public struct ZoneKey : IEquatable<ZoneKey>, IComparable<ZoneKey>, IComparable
{
	// Token: 0x1700061B RID: 1563
	// (get) Token: 0x06003E36 RID: 15926 RVA: 0x00127916 File Offset: 0x00125B16
	public int intValue
	{
		get
		{
			return ZoneKey.ToIntValue(this.zoneId, this.subZoneId);
		}
	}

	// Token: 0x1700061C RID: 1564
	// (get) Token: 0x06003E37 RID: 15927 RVA: 0x00127929 File Offset: 0x00125B29
	public string zoneName
	{
		get
		{
			return this.zoneId.GetName<GTZone>();
		}
	}

	// Token: 0x1700061D RID: 1565
	// (get) Token: 0x06003E38 RID: 15928 RVA: 0x00127936 File Offset: 0x00125B36
	public string subZoneName
	{
		get
		{
			return this.subZoneId.GetName<GTSubZone>();
		}
	}

	// Token: 0x06003E39 RID: 15929 RVA: 0x00127943 File Offset: 0x00125B43
	public ZoneKey(GTZone zone, GTSubZone subZone)
	{
		this.zoneId = zone;
		this.subZoneId = subZone;
	}

	// Token: 0x06003E3A RID: 15930 RVA: 0x00127953 File Offset: 0x00125B53
	public override int GetHashCode()
	{
		return this.intValue;
	}

	// Token: 0x06003E3B RID: 15931 RVA: 0x0012795B File Offset: 0x00125B5B
	public override string ToString()
	{
		return string.Concat(new string[] { "ZoneKey { ", this.zoneName, " : ", this.subZoneName, " }" });
	}

	// Token: 0x06003E3C RID: 15932 RVA: 0x00127992 File Offset: 0x00125B92
	public static ZoneKey GetKey(GTZone zone, GTSubZone subZone)
	{
		return new ZoneKey(zone, subZone);
	}

	// Token: 0x06003E3D RID: 15933 RVA: 0x0012799B File Offset: 0x00125B9B
	public static int ToIntValue(GTZone zone, GTSubZone subZone)
	{
		if (zone == GTZone.none && subZone == GTSubZone.none)
		{
			return 0;
		}
		return StaticHash.Compute(zone.GetLongValue<GTZone>(), subZone.GetLongValue<GTSubZone>());
	}

	// Token: 0x06003E3E RID: 15934 RVA: 0x001279B8 File Offset: 0x00125BB8
	public bool Equals(ZoneKey other)
	{
		return this.intValue == other.intValue && this.zoneId == other.zoneId && this.subZoneId == other.subZoneId;
	}

	// Token: 0x06003E3F RID: 15935 RVA: 0x001279E8 File Offset: 0x00125BE8
	public override bool Equals(object obj)
	{
		if (obj is ZoneKey)
		{
			ZoneKey zoneKey = (ZoneKey)obj;
			return this.Equals(zoneKey);
		}
		return false;
	}

	// Token: 0x06003E40 RID: 15936 RVA: 0x00127A0D File Offset: 0x00125C0D
	public static bool operator ==(ZoneKey x, ZoneKey y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003E41 RID: 15937 RVA: 0x00127A17 File Offset: 0x00125C17
	public static bool operator !=(ZoneKey x, ZoneKey y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06003E42 RID: 15938 RVA: 0x00127A24 File Offset: 0x00125C24
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

	// Token: 0x06003E43 RID: 15939 RVA: 0x00127A74 File Offset: 0x00125C74
	public int CompareTo(object obj)
	{
		if (obj is ZoneKey)
		{
			ZoneKey zoneKey = (ZoneKey)obj;
			return this.CompareTo(zoneKey);
		}
		return 1;
	}

	// Token: 0x06003E44 RID: 15940 RVA: 0x00127A99 File Offset: 0x00125C99
	public static bool operator <(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) < 0;
	}

	// Token: 0x06003E45 RID: 15941 RVA: 0x00127AA6 File Offset: 0x00125CA6
	public static bool operator >(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) > 0;
	}

	// Token: 0x06003E46 RID: 15942 RVA: 0x00127AB3 File Offset: 0x00125CB3
	public static bool operator <=(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) <= 0;
	}

	// Token: 0x06003E47 RID: 15943 RVA: 0x00127AC3 File Offset: 0x00125CC3
	public static bool operator >=(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) >= 0;
	}

	// Token: 0x06003E48 RID: 15944 RVA: 0x00127AD3 File Offset: 0x00125CD3
	public static explicit operator int(ZoneKey key)
	{
		return key.intValue;
	}

	// Token: 0x040042D6 RID: 17110
	public GTZone zoneId;

	// Token: 0x040042D7 RID: 17111
	public GTSubZone subZoneId;

	// Token: 0x040042D8 RID: 17112
	public static readonly ZoneKey Null = new ZoneKey(GTZone.none, GTSubZone.none);
}
