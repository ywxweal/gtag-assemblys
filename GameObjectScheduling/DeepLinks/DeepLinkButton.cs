using System;
using System.Collections;
using UnityEngine;

namespace GameObjectScheduling.DeepLinks
{
	// Token: 0x02000E2E RID: 3630
	public class DeepLinkButton : GorillaPressableButton
	{
		// Token: 0x06005AC5 RID: 23237 RVA: 0x001B9DE2 File Offset: 0x001B7FE2
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			this.sendingDeepLink = DeepLinkSender.SendDeepLink(this.deepLinkAppID, this.deepLinkPayload, new Action<string>(this.OnDeepLinkSent));
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x06005AC6 RID: 23238 RVA: 0x001B9E1A File Offset: 0x001B801A
		private void OnDeepLinkSent(string message)
		{
			this.sendingDeepLink = false;
			if (!this.isOn)
			{
				this.UpdateColor();
			}
		}

		// Token: 0x06005AC7 RID: 23239 RVA: 0x001B9E31 File Offset: 0x001B8031
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.pressedTime);
			this.isOn = false;
			if (!this.sendingDeepLink)
			{
				this.UpdateColor();
			}
			yield break;
		}

		// Token: 0x04005ECD RID: 24269
		[SerializeField]
		private ulong deepLinkAppID;

		// Token: 0x04005ECE RID: 24270
		[SerializeField]
		private string deepLinkPayload = "";

		// Token: 0x04005ECF RID: 24271
		[SerializeField]
		private float pressedTime = 0.2f;

		// Token: 0x04005ED0 RID: 24272
		private bool sendingDeepLink;
	}
}
