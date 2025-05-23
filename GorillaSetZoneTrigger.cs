using System;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class GorillaSetZoneTrigger : GorillaTriggerBox
{
	// Token: 0x06000D01 RID: 3329 RVA: 0x00044A07 File Offset: 0x00042C07
	public override void OnBoxTriggered()
	{
		ZoneManagement.SetActiveZones(this.zones);
	}

	// Token: 0x04001070 RID: 4208
	[SerializeField]
	private GTZone[] zones;
}
