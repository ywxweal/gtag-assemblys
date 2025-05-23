using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class GorillaVelocityEstimatorManager : MonoBehaviour
{
	// Token: 0x060005AE RID: 1454 RVA: 0x00020C76 File Offset: 0x0001EE76
	protected void Awake()
	{
		if (GorillaVelocityEstimatorManager.hasInstance && GorillaVelocityEstimatorManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaVelocityEstimatorManager.SetInstance(this);
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x00020C99 File Offset: 0x0001EE99
	protected void OnDestroy()
	{
		if (GorillaVelocityEstimatorManager.instance == this)
		{
			GorillaVelocityEstimatorManager.hasInstance = false;
			GorillaVelocityEstimatorManager.instance = null;
		}
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x00020CB4 File Offset: 0x0001EEB4
	protected void LateUpdate()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < GorillaVelocityEstimatorManager.estimators.Count; i++)
		{
			if (GorillaVelocityEstimatorManager.estimators[i] != null)
			{
				GorillaVelocityEstimatorManager.estimators[i].TriggeredLateUpdate();
			}
		}
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x00020D01 File Offset: 0x0001EF01
	public static void CreateManager()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		GorillaVelocityEstimatorManager.SetInstance(new GameObject("GorillaVelocityEstimatorManager").AddComponent<GorillaVelocityEstimatorManager>());
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00020D1F File Offset: 0x0001EF1F
	private static void SetInstance(GorillaVelocityEstimatorManager manager)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		GorillaVelocityEstimatorManager.instance = manager;
		GorillaVelocityEstimatorManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00020D42 File Offset: 0x0001EF42
	public static void Register(GorillaVelocityEstimator velEstimator)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!GorillaVelocityEstimatorManager.hasInstance)
		{
			GorillaVelocityEstimatorManager.CreateManager();
		}
		if (!GorillaVelocityEstimatorManager.estimators.Contains(velEstimator))
		{
			GorillaVelocityEstimatorManager.estimators.Add(velEstimator);
		}
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x00020D70 File Offset: 0x0001EF70
	public static void Unregister(GorillaVelocityEstimator velEstimator)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!GorillaVelocityEstimatorManager.hasInstance)
		{
			GorillaVelocityEstimatorManager.CreateManager();
		}
		if (GorillaVelocityEstimatorManager.estimators.Contains(velEstimator))
		{
			GorillaVelocityEstimatorManager.estimators.Remove(velEstimator);
		}
	}

	// Token: 0x040006B9 RID: 1721
	public static GorillaVelocityEstimatorManager instance;

	// Token: 0x040006BA RID: 1722
	public static bool hasInstance = false;

	// Token: 0x040006BB RID: 1723
	public static readonly List<GorillaVelocityEstimator> estimators = new List<GorillaVelocityEstimator>(1024);
}
