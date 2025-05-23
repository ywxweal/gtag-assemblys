using System;
using UnityEngine;

// Token: 0x0200032F RID: 815
public class EnableUnpremultipliedAlpha : MonoBehaviour
{
	// Token: 0x06001351 RID: 4945 RVA: 0x0005C27C File Offset: 0x0005A47C
	private void Start()
	{
		OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
	}
}
