using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000477 RID: 1143
public class DebugSpawnPointChanger : MonoBehaviour
{
	// Token: 0x06001C1C RID: 7196 RVA: 0x0008A16C File Offset: 0x0008836C
	private void AttachSpawnPoint(VRRig rig, Transform[] spawnPts, int locationIndex)
	{
		if (spawnPts == null)
		{
			return;
		}
		GTPlayer gtplayer = Object.FindObjectOfType<GTPlayer>();
		if (gtplayer == null)
		{
			return;
		}
		this.lastLocationIndex = locationIndex;
		int i = 0;
		while (i < spawnPts.Length)
		{
			Transform transform = spawnPts[i];
			if (transform.name == this.levelTriggers[locationIndex].levelName)
			{
				rig.transform.position = transform.position;
				rig.transform.rotation = transform.rotation;
				gtplayer.transform.position = transform.position;
				gtplayer.transform.rotation = transform.rotation;
				gtplayer.InitializeValues();
				SpawnPoint component = transform.GetComponent<SpawnPoint>();
				if (component != null)
				{
					gtplayer.SetScaleMultiplier(component.startSize);
					ZoneManagement.SetActiveZone(component.startZone);
					return;
				}
				Debug.LogWarning("Attempt to spawn at transform that does not have SpawnPoint component will be ignored: " + transform.name);
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06001C1D RID: 7197 RVA: 0x0008A260 File Offset: 0x00088460
	private void ChangePoint(int index)
	{
		SpawnManager spawnManager = Object.FindObjectOfType<SpawnManager>();
		if (spawnManager != null)
		{
			Transform[] array = spawnManager.ChildrenXfs();
			foreach (VRRig vrrig in (VRRig[])Object.FindObjectsOfType(typeof(VRRig)))
			{
				this.AttachSpawnPoint(vrrig, array, index);
			}
		}
	}

	// Token: 0x06001C1E RID: 7198 RVA: 0x0008A2B5 File Offset: 0x000884B5
	public List<string> GetPlausibleJumpLocation()
	{
		return this.levelTriggers[this.lastLocationIndex].canJumpToIndex.Select((int index) => this.levelTriggers[index].levelName).ToList<string>();
	}

	// Token: 0x06001C1F RID: 7199 RVA: 0x0008A2E4 File Offset: 0x000884E4
	public void JumpTo(int canJumpIndex)
	{
		DebugSpawnPointChanger.GeoTriggersGroup geoTriggersGroup = this.levelTriggers[this.lastLocationIndex];
		this.ChangePoint(geoTriggersGroup.canJumpToIndex[canJumpIndex]);
	}

	// Token: 0x06001C20 RID: 7200 RVA: 0x0008A314 File Offset: 0x00088514
	public void SetLastLocation(string levelName)
	{
		for (int i = 0; i < this.levelTriggers.Length; i++)
		{
			if (!(this.levelTriggers[i].levelName != levelName))
			{
				this.lastLocationIndex = i;
				return;
			}
		}
	}

	// Token: 0x04001F3A RID: 7994
	[SerializeField]
	private DebugSpawnPointChanger.GeoTriggersGroup[] levelTriggers;

	// Token: 0x04001F3B RID: 7995
	private int lastLocationIndex;

	// Token: 0x02000478 RID: 1144
	[Serializable]
	private struct GeoTriggersGroup
	{
		// Token: 0x04001F3C RID: 7996
		public string levelName;

		// Token: 0x04001F3D RID: 7997
		public GorillaGeoHideShowTrigger enterTrigger;

		// Token: 0x04001F3E RID: 7998
		public GorillaGeoHideShowTrigger[] leaveTrigger;

		// Token: 0x04001F3F RID: 7999
		public int[] canJumpToIndex;
	}
}
