using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006DB RID: 1755
public abstract class GorillaKeyButton<TBinding> : MonoBehaviour where TBinding : Enum
{
	// Token: 0x06002BAE RID: 11182 RVA: 0x000D74C5 File Offset: 0x000D56C5
	private void Start()
	{
		if (this.ButtonRenderer == null)
		{
			this.ButtonRenderer = base.GetComponent<Renderer>();
		}
		this.propBlock = new MaterialPropertyBlock();
		this.pressTime = 0f;
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x000D74F8 File Offset: 0x000D56F8
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			this.OnButtonPressedEvent();
			this.PressButtonColourUpdate();
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
				GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, component.isLeftHand, 0.1f);
				if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 66, component.isLeftHand, 0.1f });
				}
			}
		}
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x000D75DC File Offset: 0x000D57DC
	public void PressButtonColourUpdate()
	{
		this.propBlock.SetColor("_BaseColor", this.ButtonColorSettings.PressedColor);
		this.propBlock.SetColor("_Color", this.ButtonColorSettings.PressedColor);
		this.ButtonRenderer.SetPropertyBlock(this.propBlock);
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonColourUpdate>g__ButtonColorUpdate_Local|13_0());
	}

	// Token: 0x06002BB1 RID: 11185
	public abstract void OnButtonPressedEvent();

	// Token: 0x06002BB3 RID: 11187 RVA: 0x000D765B File Offset: 0x000D585B
	[CompilerGenerated]
	private IEnumerator <PressButtonColourUpdate>g__ButtonColorUpdate_Local|13_0()
	{
		yield return new WaitForSeconds(this.ButtonColorSettings.PressedTime);
		if (this.pressTime != 0f && Time.time > this.ButtonColorSettings.PressedTime + this.pressTime)
		{
			this.propBlock.SetColor("_BaseColor", this.ButtonColorSettings.UnpressedColor);
			this.propBlock.SetColor("_Color", this.ButtonColorSettings.UnpressedColor);
			this.ButtonRenderer.SetPropertyBlock(this.propBlock);
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x040031BA RID: 12730
	public string characterString;

	// Token: 0x040031BB RID: 12731
	public TBinding Binding;

	// Token: 0x040031BC RID: 12732
	public float pressTime;

	// Token: 0x040031BD RID: 12733
	public bool functionKey;

	// Token: 0x040031BE RID: 12734
	public bool testClick;

	// Token: 0x040031BF RID: 12735
	public bool repeatTestClick;

	// Token: 0x040031C0 RID: 12736
	public float repeatCooldown = 2f;

	// Token: 0x040031C1 RID: 12737
	public Renderer ButtonRenderer;

	// Token: 0x040031C2 RID: 12738
	public ButtonColorSettings ButtonColorSettings;

	// Token: 0x040031C3 RID: 12739
	private float lastTestClick;

	// Token: 0x040031C4 RID: 12740
	private MaterialPropertyBlock propBlock;
}
