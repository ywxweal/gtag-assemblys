using System;
using UnityEngine;

// Token: 0x0200045B RID: 1115
public class FirstPersonXRaySpecs : MonoBehaviour
{
	// Token: 0x06001B69 RID: 7017 RVA: 0x00086CE9 File Offset: 0x00084EE9
	private void OnEnable()
	{
		GorillaBodyRenderer.SetAllSkeletons(true);
	}

	// Token: 0x06001B6A RID: 7018 RVA: 0x00086CF1 File Offset: 0x00084EF1
	private void OnDisable()
	{
		GorillaBodyRenderer.SetAllSkeletons(false);
	}
}
