using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200054B RID: 1355
public class FlockingUpdateManager : MonoBehaviour
{
	// Token: 0x060020D4 RID: 8404 RVA: 0x000A5170 File Offset: 0x000A3370
	protected void Awake()
	{
		if (FlockingUpdateManager.hasInstance && FlockingUpdateManager.instance != null && FlockingUpdateManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		FlockingUpdateManager.SetInstance(this);
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x000A51A0 File Offset: 0x000A33A0
	public static void CreateManager()
	{
		FlockingUpdateManager.SetInstance(new GameObject("FlockingUpdateManager").AddComponent<FlockingUpdateManager>());
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x000A51B6 File Offset: 0x000A33B6
	private static void SetInstance(FlockingUpdateManager manager)
	{
		FlockingUpdateManager.instance = manager;
		FlockingUpdateManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060020D7 RID: 8407 RVA: 0x000A51D1 File Offset: 0x000A33D1
	public static void RegisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (!FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Add(flocking);
		}
	}

	// Token: 0x060020D8 RID: 8408 RVA: 0x000A51F7 File Offset: 0x000A33F7
	public static void UnregisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Remove(flocking);
		}
	}

	// Token: 0x060020D9 RID: 8409 RVA: 0x000A5220 File Offset: 0x000A3420
	public void Update()
	{
		for (int i = 0; i < FlockingUpdateManager.allFlockings.Count; i++)
		{
			FlockingUpdateManager.allFlockings[i].InvokeUpdate();
		}
	}

	// Token: 0x04002507 RID: 9479
	public static FlockingUpdateManager instance;

	// Token: 0x04002508 RID: 9480
	public static bool hasInstance = false;

	// Token: 0x04002509 RID: 9481
	public static List<Flocking> allFlockings = new List<Flocking>();
}
