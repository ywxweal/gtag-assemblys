using System;
using UnityEngine;

// Token: 0x02000746 RID: 1862
public class ZoneBasedObject : MonoBehaviour
{
	// Token: 0x06002E89 RID: 11913 RVA: 0x000E8654 File Offset: 0x000E6854
	public bool IsLocalPlayerInZone()
	{
		GTZone[] array = this.zones;
		for (int i = 0; i < array.Length; i++)
		{
			if (ZoneManagement.IsInZone(array[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x000E8684 File Offset: 0x000E6884
	public static ZoneBasedObject SelectRandomEligible(ZoneBasedObject[] objects, string overrideChoice = "")
	{
		if (overrideChoice != "")
		{
			foreach (ZoneBasedObject zoneBasedObject in objects)
			{
				if (zoneBasedObject.gameObject.name == overrideChoice)
				{
					return zoneBasedObject;
				}
			}
		}
		ZoneBasedObject zoneBasedObject2 = null;
		int num = 0;
		foreach (ZoneBasedObject zoneBasedObject3 in objects)
		{
			if (zoneBasedObject3.gameObject.activeInHierarchy)
			{
				GTZone[] array = zoneBasedObject3.zones;
				for (int j = 0; j < array.Length; j++)
				{
					if (ZoneManagement.IsInZone(array[j]))
					{
						if (Random.Range(0, num) == 0)
						{
							zoneBasedObject2 = zoneBasedObject3;
						}
						num++;
						break;
					}
				}
			}
		}
		return zoneBasedObject2;
	}

	// Token: 0x04003512 RID: 13586
	public GTZone[] zones;
}
