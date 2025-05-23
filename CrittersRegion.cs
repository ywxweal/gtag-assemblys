using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006A RID: 106
public class CrittersRegion : MonoBehaviour
{
	// Token: 0x1700002B RID: 43
	// (get) Token: 0x06000293 RID: 659 RVA: 0x000104E3 File Offset: 0x0000E6E3
	public static List<CrittersRegion> Regions
	{
		get
		{
			return CrittersRegion._regions;
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000294 RID: 660 RVA: 0x000104EA File Offset: 0x0000E6EA
	public int CritterCount
	{
		get
		{
			return this._critters.Count;
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000295 RID: 661 RVA: 0x000104F7 File Offset: 0x0000E6F7
	// (set) Token: 0x06000296 RID: 662 RVA: 0x000104FF File Offset: 0x0000E6FF
	public int ID { get; private set; }

	// Token: 0x06000297 RID: 663 RVA: 0x00010508 File Offset: 0x0000E708
	private void OnEnable()
	{
		CrittersRegion.RegisterRegion(this);
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00010510 File Offset: 0x0000E710
	private void OnDisable()
	{
		CrittersRegion.UnregisterRegion(this);
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00010518 File Offset: 0x0000E718
	private static void RegisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup[region.ID] = region;
		CrittersRegion._regions.Add(region);
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00010536 File Offset: 0x0000E736
	private static void UnregisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup.Remove(region.ID);
		CrittersRegion._regions.Remove(region);
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00010558 File Offset: 0x0000E758
	public static void AddCritterToRegion(CrittersPawn critter, int regionId)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(regionId, out crittersRegion))
		{
			crittersRegion.AddCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Attempted to add critter to non-existing region {0}.", regionId), null);
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00010594 File Offset: 0x0000E794
	public static void RemoveCritterFromRegion(CrittersPawn critter)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(critter.regionId, out crittersRegion))
		{
			crittersRegion.RemoveCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Couldn't find region with id {0}", critter.regionId), null);
	}

	// Token: 0x0600029D RID: 669 RVA: 0x000105D8 File Offset: 0x0000E7D8
	public void AddCritter(CrittersPawn pawn)
	{
		this._critters.Add(pawn);
	}

	// Token: 0x0600029E RID: 670 RVA: 0x000105E6 File Offset: 0x0000E7E6
	public void RemoveCritter(CrittersPawn pawn)
	{
		this._critters.Remove(pawn);
	}

	// Token: 0x0600029F RID: 671 RVA: 0x000105F8 File Offset: 0x0000E7F8
	public Vector3 GetSpawnPoint()
	{
		float num = this.scale / 2f;
		float num2 = base.transform.lossyScale.y * this.scale;
		Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num, num), num, Random.Range(-num, num)));
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, -base.transform.up, out raycastHit, num2, -1, QueryTriggerInteraction.Ignore))
		{
			Debug.DrawLine(vector, raycastHit.point, Color.green, 5f);
			return raycastHit.point;
		}
		Debug.DrawLine(vector, vector - base.transform.up * num2, Color.red, 5f);
		return vector;
	}

	// Token: 0x04000313 RID: 787
	private static List<CrittersRegion> _regions = new List<CrittersRegion>();

	// Token: 0x04000314 RID: 788
	private static Dictionary<int, CrittersRegion> _regionLookup = new Dictionary<int, CrittersRegion>();

	// Token: 0x04000315 RID: 789
	public CrittersBiome Biome = CrittersBiome.Any;

	// Token: 0x04000316 RID: 790
	public int maxCritters = 10;

	// Token: 0x04000317 RID: 791
	public float scale = 10f;

	// Token: 0x04000318 RID: 792
	public List<CrittersPawn> _critters = new List<CrittersPawn>();
}
