using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000276 RID: 630
public class NativeSizeVolume : MonoBehaviour
{
	// Token: 0x06000E83 RID: 3715 RVA: 0x0004919C File Offset: 0x0004739C
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onEnterAction = this.OnEnterAction;
		if (onEnterAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onEnterAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x000491F4 File Offset: 0x000473F4
	private void OnTriggerExit(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onExitAction = this.OnExitAction;
		if (onExitAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onExitAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x040011BA RID: 4538
	[SerializeField]
	private Collider triggerVolume;

	// Token: 0x040011BB RID: 4539
	[SerializeField]
	private NativeSizeChangerSettings settings;

	// Token: 0x040011BC RID: 4540
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnEnterAction;

	// Token: 0x040011BD RID: 4541
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnExitAction;

	// Token: 0x02000277 RID: 631
	[Serializable]
	private enum NativeSizeVolumeAction
	{
		// Token: 0x040011BF RID: 4543
		None,
		// Token: 0x040011C0 RID: 4544
		ApplySettings,
		// Token: 0x040011C1 RID: 4545
		ResetSize
	}
}
