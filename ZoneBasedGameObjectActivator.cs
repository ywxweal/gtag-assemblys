using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A33 RID: 2611
public class ZoneBasedGameObjectActivator : MonoBehaviour
{
	// Token: 0x06003E07 RID: 15879 RVA: 0x00126BEA File Offset: 0x00124DEA
	private void OnEnable()
	{
		ZoneManagement.OnZoneChange += this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x06003E08 RID: 15880 RVA: 0x00126BFD File Offset: 0x00124DFD
	private void OnDisable()
	{
		ZoneManagement.OnZoneChange -= this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x06003E09 RID: 15881 RVA: 0x00126C10 File Offset: 0x00124E10
	private void ZoneManagement_OnZoneChange(ZoneData[] zoneData)
	{
		HashSet<GTZone> hashSet = new HashSet<GTZone>(this.zones);
		bool flag = false;
		for (int i = 0; i < zoneData.Length; i++)
		{
			flag |= zoneData[i].active && hashSet.Contains(zoneData[i].zone);
		}
		for (int j = 0; j < this.gameObjects.Length; j++)
		{
			this.gameObjects[j].SetActive(flag);
		}
	}

	// Token: 0x04004297 RID: 17047
	[SerializeField]
	private GTZone[] zones;

	// Token: 0x04004298 RID: 17048
	[SerializeField]
	private GameObject[] gameObjects;
}
