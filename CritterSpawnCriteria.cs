﻿using System;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class CritterSpawnCriteria : ScriptableObject
{
	// Token: 0x06000285 RID: 645 RVA: 0x00010184 File Offset: 0x0000E384
	public bool CanSpawn()
	{
		if (this.spawnTimings.Length == 0)
		{
			return true;
		}
		string currentTimeOfDay = BetterDayNightManager.instance.currentTimeOfDay;
		string[] array = this.spawnTimings;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == currentTimeOfDay)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000305 RID: 773
	public string[] spawnTimings;
}
