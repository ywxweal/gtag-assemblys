using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068F RID: 1679
public class SizeManagerManager : MonoBehaviour
{
	// Token: 0x060029FB RID: 10747 RVA: 0x000CFB7E File Offset: 0x000CDD7E
	protected void Awake()
	{
		if (SizeManagerManager.hasInstance && SizeManagerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SizeManagerManager.SetInstance(this);
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x000CFBA1 File Offset: 0x000CDDA1
	public static void CreateManager()
	{
		SizeManagerManager.SetInstance(new GameObject("SizeManagerManager").AddComponent<SizeManagerManager>());
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x000CFBB7 File Offset: 0x000CDDB7
	private static void SetInstance(SizeManagerManager manager)
	{
		SizeManagerManager.instance = manager;
		SizeManagerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x000CFBD2 File Offset: 0x000CDDD2
	public static void RegisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (!SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Add(sM);
		}
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x000CFBF8 File Offset: 0x000CDDF8
	public static void UnregisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Remove(sM);
		}
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x000CFC20 File Offset: 0x000CDE20
	public void FixedUpdate()
	{
		for (int i = 0; i < SizeManagerManager.allSM.Count; i++)
		{
			SizeManagerManager.allSM[i].InvokeFixedUpdate();
		}
	}

	// Token: 0x04002F1E RID: 12062
	[OnEnterPlay_SetNull]
	public static SizeManagerManager instance;

	// Token: 0x04002F1F RID: 12063
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x04002F20 RID: 12064
	[OnEnterPlay_Clear]
	public static List<SizeManager> allSM = new List<SizeManager>();
}
