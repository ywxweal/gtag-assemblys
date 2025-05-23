using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C3F RID: 3135
	public class GorillaNetworkDisconnectTrigger : GorillaTriggerBox
	{
		// Token: 0x06004E00 RID: 19968 RVA: 0x00174148 File Offset: 0x00172348
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (this.makeSureThisIsEnabled != null)
			{
				this.makeSureThisIsEnabled.SetActive(true);
			}
			GameObject[] array = this.makeSureTheseAreEnabled;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.componentTypeToRemove != "" && this.componentTarget.GetComponent(this.componentTypeToRemove) != null)
				{
					Object.Destroy(this.componentTarget.GetComponent(this.componentTypeToRemove));
				}
				PhotonNetwork.Disconnect();
				SkinnedMeshRenderer[] array2 = this.photonNetworkController.offlineVRRig;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].enabled = true;
				}
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		// Token: 0x0400510A RID: 20746
		public PhotonNetworkController photonNetworkController;

		// Token: 0x0400510B RID: 20747
		public GameObject offlineVRRig;

		// Token: 0x0400510C RID: 20748
		public GameObject makeSureThisIsEnabled;

		// Token: 0x0400510D RID: 20749
		public GameObject[] makeSureTheseAreEnabled;

		// Token: 0x0400510E RID: 20750
		public string componentTypeToRemove;

		// Token: 0x0400510F RID: 20751
		public GameObject componentTarget;
	}
}
