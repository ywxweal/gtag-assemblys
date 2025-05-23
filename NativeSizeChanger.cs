using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class NativeSizeChanger : MonoBehaviour
{
	// Token: 0x06000E7A RID: 3706 RVA: 0x0004911C File Offset: 0x0004731C
	public void Activate(NativeSizeChangerSettings settings)
	{
		settings.WorldPosition = base.transform.position;
		settings.ActivationTime = Time.time;
		GTPlayer.Instance.SetNativeScale(settings);
	}
}
