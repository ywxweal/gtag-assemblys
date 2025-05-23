using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C28 RID: 3112
	public class SafeAccountObjectSwapper : MonoBehaviour
	{
		// Token: 0x06004D13 RID: 19731 RVA: 0x0016F02A File Offset: 0x0016D22A
		public void Start()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				this.SwitchToSafeMode();
			}
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.SafeAccountUpdated));
		}

		// Token: 0x06004D14 RID: 19732 RVA: 0x0016F068 File Offset: 0x0016D268
		public void SafeAccountUpdated(bool isSafety)
		{
			if (isSafety)
			{
				this.SwitchToSafeMode();
			}
		}

		// Token: 0x06004D15 RID: 19733 RVA: 0x0016F074 File Offset: 0x0016D274
		public void SwitchToSafeMode()
		{
			foreach (GameObject gameObject in this.UnSafeGameObjects)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			foreach (GameObject gameObject2 in this.UnSafeTexts)
			{
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
			foreach (GameObject gameObject3 in this.SafeTexts)
			{
				if (gameObject3 != null)
				{
					gameObject3.SetActive(true);
				}
			}
			foreach (GameObject gameObject4 in this.SafeModeObjects)
			{
				if (gameObject4 != null)
				{
					gameObject4.SetActive(true);
				}
			}
		}

		// Token: 0x04004FFF RID: 20479
		public GameObject[] UnSafeGameObjects;

		// Token: 0x04005000 RID: 20480
		public GameObject[] UnSafeTexts;

		// Token: 0x04005001 RID: 20481
		public GameObject[] SafeTexts;

		// Token: 0x04005002 RID: 20482
		public GameObject[] SafeModeObjects;
	}
}
