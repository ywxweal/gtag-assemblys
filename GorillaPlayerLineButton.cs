using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006DE RID: 1758
public class GorillaPlayerLineButton : MonoBehaviour
{
	// Token: 0x06002BBA RID: 11194 RVA: 0x000D76AA File Offset: 0x000D58AA
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
	}

	// Token: 0x06002BBB RID: 11195 RVA: 0x000D76C0 File Offset: 0x000D58C0
	private void OnDisable()
	{
		if (Application.isEditor)
		{
			base.StopAllCoroutines();
		}
	}

	// Token: 0x06002BBC RID: 11196 RVA: 0x000D76CF File Offset: 0x000D58CF
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
				{
					this.isOn = !this.isOn;
				}
				this.parentLine.PressButton(this.isOn, this.buttonType);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06002BBD RID: 11197 RVA: 0x000D76E0 File Offset: 0x000D58E0
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
			{
				if (this.isAutoOn)
				{
					this.isOn = false;
				}
				else
				{
					this.isOn = !this.isOn;
				}
			}
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute || this.buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech || this.buttonType == GorillaPlayerLineButton.ButtonType.Cheating || this.buttonType == GorillaPlayerLineButton.ButtonType.Cancel || this.parentLine.canPressNextReportButton)
			{
				this.parentLine.PressButton(this.isOn, this.buttonType);
				if (component != null)
				{
					GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
					GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
					if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, component.isLeftHand, 0.05f });
					}
				}
			}
		}
	}

	// Token: 0x06002BBE RID: 11198 RVA: 0x000D7850 File Offset: 0x000D5A50
	private void OnTriggerExit(Collider other)
	{
		if (this.buttonType != GorillaPlayerLineButton.ButtonType.Mute && other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.parentLine.canPressNextReportButton = true;
		}
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x000D7878 File Offset: 0x000D5A78
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		if (this.isAutoOn)
		{
			base.GetComponent<MeshRenderer>().material = this.autoOnMaterial;
			this.myText.text = this.autoOnText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x040031C6 RID: 12742
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x040031C7 RID: 12743
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x040031C8 RID: 12744
	public bool isOn;

	// Token: 0x040031C9 RID: 12745
	public bool isAutoOn;

	// Token: 0x040031CA RID: 12746
	public Material offMaterial;

	// Token: 0x040031CB RID: 12747
	public Material onMaterial;

	// Token: 0x040031CC RID: 12748
	public Material autoOnMaterial;

	// Token: 0x040031CD RID: 12749
	public string offText;

	// Token: 0x040031CE RID: 12750
	public string onText;

	// Token: 0x040031CF RID: 12751
	public string autoOnText;

	// Token: 0x040031D0 RID: 12752
	public Text myText;

	// Token: 0x040031D1 RID: 12753
	public float debounceTime = 0.25f;

	// Token: 0x040031D2 RID: 12754
	public float touchTime;

	// Token: 0x040031D3 RID: 12755
	public bool testPress;

	// Token: 0x020006DF RID: 1759
	public enum ButtonType
	{
		// Token: 0x040031D5 RID: 12757
		HateSpeech,
		// Token: 0x040031D6 RID: 12758
		Cheating,
		// Token: 0x040031D7 RID: 12759
		Toxicity,
		// Token: 0x040031D8 RID: 12760
		Mute,
		// Token: 0x040031D9 RID: 12761
		Report,
		// Token: 0x040031DA RID: 12762
		Cancel
	}
}
