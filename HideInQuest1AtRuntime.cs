using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000995 RID: 2453
public class HideInQuest1AtRuntime : MonoBehaviour
{
	// Token: 0x06003AC9 RID: 15049 RVA: 0x00119492 File Offset: 0x00117692
	private void OnEnable()
	{
		if (PlayFabAuthenticator.instance != null && "Quest1" == PlayFabAuthenticator.instance.platform.ToString())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
