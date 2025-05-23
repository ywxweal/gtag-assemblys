using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020006B4 RID: 1716
public class StageMicrophone : MonoBehaviour
{
	// Token: 0x06002AD4 RID: 10964 RVA: 0x000D253F File Offset: 0x000D073F
	private void Awake()
	{
		StageMicrophone.Instance = this;
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x000D2547 File Offset: 0x000D0747
	public bool IsPlayerAmplified(VRRig player)
	{
		return (player.GetMouthPosition() - base.transform.position).IsShorterThan(this.PickupRadius);
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x000D256A File Offset: 0x000D076A
	public float GetPlayerSpatialBlend(VRRig player)
	{
		if (!this.IsPlayerAmplified(player))
		{
			return 0.9f;
		}
		return this.AmplifiedSpatialBlend;
	}

	// Token: 0x04002FC5 RID: 12229
	public static StageMicrophone Instance;

	// Token: 0x04002FC6 RID: 12230
	[SerializeField]
	private float PickupRadius;

	// Token: 0x04002FC7 RID: 12231
	[SerializeField]
	private float AmplifiedSpatialBlend;
}
