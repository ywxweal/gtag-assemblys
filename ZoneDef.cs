using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000A36 RID: 2614
public class ZoneDef : MonoBehaviour
{
	// Token: 0x17000612 RID: 1554
	// (get) Token: 0x06003E13 RID: 15891 RVA: 0x00127058 File Offset: 0x00125258
	public GroupJoinZoneAB groupZoneAB
	{
		get
		{
			return new GroupJoinZoneAB
			{
				a = this.groupZone,
				b = this.groupZoneB
			};
		}
	}

	// Token: 0x17000613 RID: 1555
	// (get) Token: 0x06003E14 RID: 15892 RVA: 0x00127088 File Offset: 0x00125288
	public GroupJoinZoneAB excludeGroupZoneAB
	{
		get
		{
			return new GroupJoinZoneAB
			{
				a = this.excludeGroupZone,
				b = this.excludeGroupZoneB
			};
		}
	}

	// Token: 0x040042A8 RID: 17064
	public GTZone zoneId;

	// Token: 0x040042A9 RID: 17065
	[FormerlySerializedAs("subZoneType")]
	[FormerlySerializedAs("subZone")]
	public GTSubZone subZoneId;

	// Token: 0x040042AA RID: 17066
	public GroupJoinZoneA groupZone;

	// Token: 0x040042AB RID: 17067
	public GroupJoinZoneB groupZoneB;

	// Token: 0x040042AC RID: 17068
	public GroupJoinZoneA excludeGroupZone;

	// Token: 0x040042AD RID: 17069
	public GroupJoinZoneB excludeGroupZoneB;

	// Token: 0x040042AE RID: 17070
	[Space]
	public bool trackEnter = true;

	// Token: 0x040042AF RID: 17071
	public bool trackExit;

	// Token: 0x040042B0 RID: 17072
	public bool trackStay = true;

	// Token: 0x040042B1 RID: 17073
	public int priority = 1;

	// Token: 0x040042B2 RID: 17074
	[Space]
	public BoxCollider[] colliders = new BoxCollider[0];

	// Token: 0x040042B3 RID: 17075
	[Space]
	public ZoneNode[] nodes = new ZoneNode[0];

	// Token: 0x040042B4 RID: 17076
	[Space]
	public Bounds bounds;

	// Token: 0x040042B5 RID: 17077
	[Space]
	public ZoneDef[] zoneOverlaps = new ZoneDef[0];
}
