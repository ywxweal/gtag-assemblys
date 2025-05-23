using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200042E RID: 1070
[DefaultExecutionOrder(1549)]
public class TransferrableObjectManager : MonoBehaviour
{
	// Token: 0x06001A64 RID: 6756 RVA: 0x00081BA6 File Offset: 0x0007FDA6
	protected void Awake()
	{
		if (TransferrableObjectManager.hasInstance && TransferrableObjectManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		TransferrableObjectManager.SetInstance(this);
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x00081BC9 File Offset: 0x0007FDC9
	protected void OnDestroy()
	{
		if (TransferrableObjectManager.instance == this)
		{
			TransferrableObjectManager.hasInstance = false;
			TransferrableObjectManager.instance = null;
		}
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x00081BE4 File Offset: 0x0007FDE4
	protected void LateUpdate()
	{
		for (int i = 0; i < TransferrableObjectManager.transObs.Count; i++)
		{
			TransferrableObjectManager.transObs[i].TriggeredLateUpdate();
		}
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x00081C16 File Offset: 0x0007FE16
	public static void CreateManager()
	{
		TransferrableObjectManager.SetInstance(new GameObject("TransferrableObjectManager").AddComponent<TransferrableObjectManager>());
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x00081C2C File Offset: 0x0007FE2C
	private static void SetInstance(TransferrableObjectManager manager)
	{
		TransferrableObjectManager.instance = manager;
		TransferrableObjectManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x00081C47 File Offset: 0x0007FE47
	public static void Register(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (!TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Add(transOb);
		}
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x00081C6D File Offset: 0x0007FE6D
	public static void Unregister(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Remove(transOb);
		}
	}

	// Token: 0x04001D80 RID: 7552
	public static TransferrableObjectManager instance;

	// Token: 0x04001D81 RID: 7553
	public static bool hasInstance = false;

	// Token: 0x04001D82 RID: 7554
	public static readonly List<TransferrableObject> transObs = new List<TransferrableObject>(1024);
}
