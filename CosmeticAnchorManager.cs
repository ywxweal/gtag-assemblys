using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003FC RID: 1020
public class CosmeticAnchorManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600189E RID: 6302 RVA: 0x00077658 File Offset: 0x00075858
	protected void Awake()
	{
		if (CosmeticAnchorManager.hasInstance && CosmeticAnchorManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		CosmeticAnchorManager.SetInstance(this);
	}

	// Token: 0x0600189F RID: 6303 RVA: 0x0007767B File Offset: 0x0007587B
	public static void CreateManager()
	{
		CosmeticAnchorManager.SetInstance(new GameObject("CosmeticAnchorManager").AddComponent<CosmeticAnchorManager>());
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x00077691 File Offset: 0x00075891
	private static void SetInstance(CosmeticAnchorManager manager)
	{
		CosmeticAnchorManager.instance = manager;
		CosmeticAnchorManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x000776AC File Offset: 0x000758AC
	public static void RegisterCosmeticAnchor(CosmeticAnchors cA)
	{
		if (!CosmeticAnchorManager.hasInstance)
		{
			CosmeticAnchorManager.CreateManager();
		}
		if ((cA.AffectedByHunt() || cA.AffectedByBuilder()) && !CosmeticAnchorManager.allAnchors.Contains(cA))
		{
			CosmeticAnchorManager.allAnchors.Add(cA);
		}
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x000776E2 File Offset: 0x000758E2
	public static void UnregisterCosmeticAnchor(CosmeticAnchors cA)
	{
		if (!CosmeticAnchorManager.hasInstance)
		{
			CosmeticAnchorManager.CreateManager();
		}
		if ((cA.AffectedByHunt() || cA.AffectedByBuilder()) && CosmeticAnchorManager.allAnchors.Contains(cA))
		{
			CosmeticAnchorManager.allAnchors.Remove(cA);
		}
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x0007771C File Offset: 0x0007591C
	public void SliceUpdate()
	{
		for (int i = 0; i < CosmeticAnchorManager.allAnchors.Count; i++)
		{
			CosmeticAnchorManager.allAnchors[i].TryUpdate();
		}
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04001B65 RID: 7013
	public static CosmeticAnchorManager instance;

	// Token: 0x04001B66 RID: 7014
	public static bool hasInstance = false;

	// Token: 0x04001B67 RID: 7015
	public static List<CosmeticAnchors> allAnchors = new List<CosmeticAnchors>();
}
