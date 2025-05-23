using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001ED RID: 493
public class ObjectHierarchyFlattenerManager : MonoBehaviour
{
	// Token: 0x06000B68 RID: 2920 RVA: 0x0003D2CA File Offset: 0x0003B4CA
	protected void Awake()
	{
		if (ObjectHierarchyFlattenerManager.hasInstance && ObjectHierarchyFlattenerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ObjectHierarchyFlattenerManager.SetInstance(this);
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x0003D2ED File Offset: 0x0003B4ED
	public static void CreateManager()
	{
		ObjectHierarchyFlattenerManager.SetInstance(new GameObject("ObjectHierarchyFlattenerManager").AddComponent<ObjectHierarchyFlattenerManager>());
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x0003D303 File Offset: 0x0003B503
	private static void SetInstance(ObjectHierarchyFlattenerManager manager)
	{
		ObjectHierarchyFlattenerManager.instance = manager;
		ObjectHierarchyFlattenerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x0003D31E File Offset: 0x0003B51E
	public static void RegisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (!ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Add(rbWI);
		}
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0003D344 File Offset: 0x0003B544
	public static void UnregisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Remove(rbWI);
		}
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x0003D36C File Offset: 0x0003B56C
	public void LateUpdate()
	{
		for (int i = 0; i < ObjectHierarchyFlattenerManager.alloHF.Count; i++)
		{
			ObjectHierarchyFlattenerManager.alloHF[i].InvokeLateUpdate();
		}
	}

	// Token: 0x04000E08 RID: 3592
	public static ObjectHierarchyFlattenerManager instance;

	// Token: 0x04000E09 RID: 3593
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x04000E0A RID: 3594
	public static List<ObjectHierarchyFlattener> alloHF = new List<ObjectHierarchyFlattener>();
}
