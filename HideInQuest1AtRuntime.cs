using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000995 RID: 2453
public class HideInQuest1AtRuntime : MonoBehaviour
{
	// Token: 0x06003AC8 RID: 15048 RVA: 0x001193BA File Offset: 0x001175BA
	private void OnEnable()
	{
		if (PlayFabAuthenticator.instance != null && "Quest1" == PlayFabAuthenticator.instance.platform.ToString())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
