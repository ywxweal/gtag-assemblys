using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B2 RID: 946
public class SlingshotProjectileManager : MonoBehaviour
{
	// Token: 0x06001628 RID: 5672 RVA: 0x0006BA13 File Offset: 0x00069C13
	protected void Awake()
	{
		if (SlingshotProjectileManager.hasInstance && SlingshotProjectileManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SlingshotProjectileManager.SetInstance(this);
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x0006BA36 File Offset: 0x00069C36
	public static void CreateManager()
	{
		SlingshotProjectileManager.SetInstance(new GameObject("SlingshotProjectileManager").AddComponent<SlingshotProjectileManager>());
	}

	// Token: 0x0600162A RID: 5674 RVA: 0x0006BA4C File Offset: 0x00069C4C
	private static void SetInstance(SlingshotProjectileManager manager)
	{
		SlingshotProjectileManager.instance = manager;
		SlingshotProjectileManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x0600162B RID: 5675 RVA: 0x0006BA67 File Offset: 0x00069C67
	public static void RegisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (!SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Add(sP);
		}
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x0006BA8D File Offset: 0x00069C8D
	public static void UnregisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Remove(sP);
		}
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x0006BAB4 File Offset: 0x00069CB4
	public void Update()
	{
		for (int i = 0; i < SlingshotProjectileManager.allsP.Count; i++)
		{
			SlingshotProjectileManager.allsP[i].InvokeUpdate();
		}
	}

	// Token: 0x040018A2 RID: 6306
	public static SlingshotProjectileManager instance;

	// Token: 0x040018A3 RID: 6307
	public static bool hasInstance = false;

	// Token: 0x040018A4 RID: 6308
	public static List<SlingshotProjectile> allsP = new List<SlingshotProjectile>();
}
