using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000663 RID: 1635
public class HeldButton : MonoBehaviour
{
	// Token: 0x060028DC RID: 10460 RVA: 0x000CB83C File Offset: 0x000C9A3C
	private void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		if ((componentInParent.isLeftHand && !this.leftHandPressable) || (!componentInParent.isLeftHand && !this.rightHandPressable))
		{
			return;
		}
		if (!this.pendingPress || other != this.pendingPressCollider)
		{
			UnityEvent unityEvent = this.onStartPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.touchTime = Time.time;
			this.pendingPressCollider = other;
			this.pressingHand = componentInParent;
			this.pendingPress = true;
			this.SetOn(true);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x060028DD RID: 10461 RVA: 0x000CB8FC File Offset: 0x000C9AFC
	private void LateUpdate()
	{
		if (!this.pendingPress)
		{
			return;
		}
		if (this.touchTime < this.releaseTime && this.releaseTime + this.debounceTime < Time.time)
		{
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.SetOn(false);
			return;
		}
		if (this.touchTime + this.pressDuration < Time.time)
		{
			this.onPressButton.Invoke();
			if (this.pressingHand != null)
			{
				GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, this.pressingHand.isLeftHand, 0.1f);
				GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			UnityEvent unityEvent2 = this.onStopPressingButton;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.releaseTime = Time.time;
			this.SetOn(false);
			return;
		}
		if (this.touchTime > this.releaseTime && this.pressingHand != null)
		{
			GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, Time.fixedDeltaTime);
		}
	}

	// Token: 0x060028DE RID: 10462 RVA: 0x000CBA5B File Offset: 0x000C9C5B
	private void OnTriggerExit(Collider other)
	{
		if (this.pendingPress && this.pendingPressCollider == other)
		{
			this.releaseTime = Time.time;
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060028DF RID: 10463 RVA: 0x000CBA90 File Offset: 0x000C9C90
	public void SetOn(bool inOn)
	{
		if (inOn == this.isOn)
		{
			return;
		}
		this.isOn = inOn;
		if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
	}

	// Token: 0x04002DE1 RID: 11745
	public Material pressedMaterial;

	// Token: 0x04002DE2 RID: 11746
	public Material unpressedMaterial;

	// Token: 0x04002DE3 RID: 11747
	public MeshRenderer buttonRenderer;

	// Token: 0x04002DE4 RID: 11748
	private bool isOn;

	// Token: 0x04002DE5 RID: 11749
	public float debounceTime = 0.25f;

	// Token: 0x04002DE6 RID: 11750
	public bool leftHandPressable;

	// Token: 0x04002DE7 RID: 11751
	public bool rightHandPressable = true;

	// Token: 0x04002DE8 RID: 11752
	public float pressDuration = 0.5f;

	// Token: 0x04002DE9 RID: 11753
	public UnityEvent onStartPressingButton;

	// Token: 0x04002DEA RID: 11754
	public UnityEvent onStopPressingButton;

	// Token: 0x04002DEB RID: 11755
	public UnityEvent onPressButton;

	// Token: 0x04002DEC RID: 11756
	[TextArea]
	public string offText;

	// Token: 0x04002DED RID: 11757
	[TextArea]
	public string onText;

	// Token: 0x04002DEE RID: 11758
	public Text myText;

	// Token: 0x04002DEF RID: 11759
	private float touchTime;

	// Token: 0x04002DF0 RID: 11760
	private float releaseTime;

	// Token: 0x04002DF1 RID: 11761
	private bool pendingPress;

	// Token: 0x04002DF2 RID: 11762
	private Collider pendingPressCollider;

	// Token: 0x04002DF3 RID: 11763
	private GorillaTriggerColliderHandIndicator pressingHand;
}
