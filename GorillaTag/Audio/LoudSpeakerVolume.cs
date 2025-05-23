using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D97 RID: 3479
	public class LoudSpeakerVolume : MonoBehaviour
	{
		// Token: 0x0600565E RID: 22110 RVA: 0x001A4814 File Offset: 0x001A2A14
		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("GorillaPlayer"))
			{
				VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
				if (component != null && component.creator != null)
				{
					if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
					{
						this._trigger.OnPlayerEnter(component);
						return;
					}
				}
				else
				{
					Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerEnter no colliding rig found!");
				}
			}
		}

		// Token: 0x0600565F RID: 22111 RVA: 0x001A4884 File Offset: 0x001A2A84
		public void OnTriggerExit(Collider other)
		{
			VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
				{
					this._trigger.OnPlayerExit(component);
					return;
				}
			}
			else
			{
				Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerExit no colliding rig found!");
			}
		}

		// Token: 0x04005A27 RID: 23079
		[SerializeField]
		private LoudSpeakerTrigger _trigger;
	}
}
