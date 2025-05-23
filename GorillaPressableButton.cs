using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020006E3 RID: 1763
public class GorillaPressableButton : MonoBehaviour
{
	// Token: 0x1400005C RID: 92
	// (add) Token: 0x06002BDA RID: 11226 RVA: 0x000D8520 File Offset: 0x000D6720
	// (remove) Token: 0x06002BDB RID: 11227 RVA: 0x000D8558 File Offset: 0x000D6758
	public event Action<GorillaPressableButton, bool> onPressed;

	// Token: 0x06002BDC RID: 11228 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void Start()
	{
	}

	// Token: 0x06002BDD RID: 11229 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnEnable()
	{
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnDisable()
	{
	}

	// Token: 0x06002BDF RID: 11231 RVA: 0x000D8590 File Offset: 0x000D6790
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.touchTime = Time.time;
		GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
		UnityEvent unityEvent = this.onPressButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		Action<GorillaPressableButton, bool> action = this.onPressed;
		if (action != null)
		{
			action(this, component.isLeftHand);
		}
		this.ButtonActivation();
		this.ButtonActivationWithHand(component.isLeftHand);
		if (component == null)
		{
			return;
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component.isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, component.isLeftHand, 0.05f });
		}
	}

	// Token: 0x06002BE0 RID: 11232 RVA: 0x000D86CC File Offset: 0x000D68CC
	public virtual void UpdateColor()
	{
		if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myTmpText != null)
			{
				this.myTmpText.text = this.onText;
			}
			if (this.myTmpText2 != null)
			{
				this.myTmpText2.text = this.onText;
				return;
			}
			if (this.myText != null)
			{
				this.myText.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myTmpText != null)
			{
				this.myTmpText.text = this.offText;
			}
			if (this.myTmpText2 != null)
			{
				this.myTmpText2.text = this.offText;
				return;
			}
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
	}

	// Token: 0x06002BE1 RID: 11233 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void ButtonActivation()
	{
	}

	// Token: 0x06002BE2 RID: 11234 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void ButtonActivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x06002BE3 RID: 11235 RVA: 0x000D87C3 File Offset: 0x000D69C3
	public virtual void ResetState()
	{
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04003209 RID: 12809
	public Material pressedMaterial;

	// Token: 0x0400320A RID: 12810
	public Material unpressedMaterial;

	// Token: 0x0400320B RID: 12811
	public MeshRenderer buttonRenderer;

	// Token: 0x0400320C RID: 12812
	public int pressButtonSoundIndex = 67;

	// Token: 0x0400320D RID: 12813
	public bool isOn;

	// Token: 0x0400320E RID: 12814
	public float debounceTime = 0.25f;

	// Token: 0x0400320F RID: 12815
	public float touchTime;

	// Token: 0x04003210 RID: 12816
	public bool testPress;

	// Token: 0x04003211 RID: 12817
	public bool testHandLeft;

	// Token: 0x04003212 RID: 12818
	[TextArea]
	public string offText;

	// Token: 0x04003213 RID: 12819
	[TextArea]
	public string onText;

	// Token: 0x04003214 RID: 12820
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText;

	// Token: 0x04003215 RID: 12821
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText2;

	// Token: 0x04003216 RID: 12822
	public Text myText;

	// Token: 0x04003217 RID: 12823
	[Space]
	public UnityEvent onPressButton;
}
