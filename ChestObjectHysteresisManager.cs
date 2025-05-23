using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003FA RID: 1018
[DefaultExecutionOrder(2000)]
public class ChestObjectHysteresisManager : MonoBehaviour
{
	// Token: 0x0600188E RID: 6286 RVA: 0x000773AE File Offset: 0x000755AE
	protected void Awake()
	{
		if (ChestObjectHysteresisManager.hasInstance && ChestObjectHysteresisManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ChestObjectHysteresisManager.SetInstance(this);
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x000773D1 File Offset: 0x000755D1
	public static void CreateManager()
	{
		ChestObjectHysteresisManager.SetInstance(new GameObject("ChestObjectHysteresisManager").AddComponent<ChestObjectHysteresisManager>());
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x000773E7 File Offset: 0x000755E7
	private static void SetInstance(ChestObjectHysteresisManager manager)
	{
		ChestObjectHysteresisManager.instance = manager;
		ChestObjectHysteresisManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x00077402 File Offset: 0x00075602
	public static void RegisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (!ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Add(cOH);
		}
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x00077428 File Offset: 0x00075628
	public static void UnregisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Remove(cOH);
		}
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x00077450 File Offset: 0x00075650
	public void Update()
	{
		for (int i = 0; i < ChestObjectHysteresisManager.allChests.Count; i++)
		{
			ChestObjectHysteresisManager.allChests[i].InvokeUpdate();
		}
	}

	// Token: 0x04001B5A RID: 7002
	public static ChestObjectHysteresisManager instance;

	// Token: 0x04001B5B RID: 7003
	public static bool hasInstance = false;

	// Token: 0x04001B5C RID: 7004
	public static List<ChestObjectHysteresis> allChests = new List<ChestObjectHysteresis>();
}
