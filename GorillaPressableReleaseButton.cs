using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006E4 RID: 1764
public class GorillaPressableReleaseButton : GorillaPressableButton
{
	// Token: 0x06002BE5 RID: 11237 RVA: 0x000D87F0 File Offset: 0x000D69F0
	private new void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (this.touchingCollider)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component == null)
		{
			return;
		}
		this.touchTime = Time.time;
		this.touchingCollider = other;
		UnityEvent onPressButton = this.onPressButton;
		if (onPressButton != null)
		{
			onPressButton.Invoke();
		}
		this.ButtonActivation();
		this.ButtonActivationWithHand(component.isLeftHand);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component.isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, component.isLeftHand, 0.05f });
		}
	}

	// Token: 0x06002BE6 RID: 11238 RVA: 0x000D8918 File Offset: 0x000D6B18
	private void OnTriggerExit(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		if (other != this.touchingCollider)
		{
			return;
		}
		this.touchingCollider = null;
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component == null)
		{
			return;
		}
		UnityEvent unityEvent = this.onReleaseButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this.ButtonDeactivation();
		this.ButtonDeactivationWithHand(component.isLeftHand);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component.isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, component.isLeftHand, 0.05f });
		}
	}

	// Token: 0x06002BE7 RID: 11239 RVA: 0x000D8A20 File Offset: 0x000D6C20
	public override void ResetState()
	{
		base.ResetState();
		this.touchingCollider = null;
	}

	// Token: 0x06002BE8 RID: 11240 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void ButtonDeactivation()
	{
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void ButtonDeactivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x04003219 RID: 12825
	public UnityEvent onReleaseButton;

	// Token: 0x0400321A RID: 12826
	private Collider touchingCollider;
}
