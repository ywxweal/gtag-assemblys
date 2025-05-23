using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002D4 RID: 724
public class NonCosmeticItemProvider : MonoBehaviour
{
	// Token: 0x06001173 RID: 4467 RVA: 0x00053FA8 File Offset: 0x000521A8
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component != null)
		{
			GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { true, component.isLeftHand });
		}
	}

	// Token: 0x040013A6 RID: 5030
	public GTZone zone;

	// Token: 0x040013A7 RID: 5031
	[Tooltip("only for honeycomb")]
	public bool useCondition;

	// Token: 0x040013A8 RID: 5032
	public int conditionThreshold;

	// Token: 0x040013A9 RID: 5033
	public NonCosmeticItemProvider.ItemType itemType;

	// Token: 0x020002D5 RID: 725
	public enum ItemType
	{
		// Token: 0x040013AB RID: 5035
		honeycomb
	}
}
