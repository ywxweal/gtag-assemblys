using System;
using UnityEngine;

// Token: 0x02000216 RID: 534
[DefaultExecutionOrder(-9999)]
public class ScenePreparer : MonoBehaviour
{
	// Token: 0x06000C7F RID: 3199 RVA: 0x00041878 File Offset: 0x0003FA78
	protected void Awake()
	{
		bool flag = false;
		GameObject[] array = this.betaEnableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		array = this.betaDisableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!flag);
		}
	}

	// Token: 0x04000F09 RID: 3849
	public OVRManager ovrManager;

	// Token: 0x04000F0A RID: 3850
	public GameObject[] betaDisableObjects;

	// Token: 0x04000F0B RID: 3851
	public GameObject[] betaEnableObjects;
}
