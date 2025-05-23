using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020006B4 RID: 1716
public class StageMicrophone : MonoBehaviour
{
	// Token: 0x06002AD5 RID: 10965 RVA: 0x000D25E3 File Offset: 0x000D07E3
	private void Awake()
	{
		StageMicrophone.Instance = this;
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x000D25EB File Offset: 0x000D07EB
	public bool IsPlayerAmplified(VRRig player)
	{
		return (player.GetMouthPosition() - base.transform.position).IsShorterThan(this.PickupRadius);
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x000D260E File Offset: 0x000D080E
	public float GetPlayerSpatialBlend(VRRig player)
	{
		if (!this.IsPlayerAmplified(player))
		{
			return 0.9f;
		}
		return this.AmplifiedSpatialBlend;
	}

	// Token: 0x04002FC7 RID: 12231
	public static StageMicrophone Instance;

	// Token: 0x04002FC8 RID: 12232
	[SerializeField]
	private float PickupRadius;

	// Token: 0x04002FC9 RID: 12233
	[SerializeField]
	private float AmplifiedSpatialBlend;
}
