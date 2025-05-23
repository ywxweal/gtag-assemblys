using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200038F RID: 911
public class GorillaReportButton : MonoBehaviour
{
	// Token: 0x06001514 RID: 5396 RVA: 0x00066B82 File Offset: 0x00064D82
	public void AssignParentLine(GorillaPlayerScoreboardLine parent)
	{
		this.parentLine = parent;
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x00066B8C File Offset: 0x00064D8C
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time)
		{
			this.isOn = !this.isOn;
			this.UpdateColor();
			this.selected = !this.selected;
			this.touchTime = Time.time;
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 0.05f);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, false, 0.05f });
			}
		}
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x00066C7F File Offset: 0x00064E7F
	private void OnTriggerExit(Collider other)
	{
		if (this.metaReportType != GorillaReportButton.MetaReportReason.Cancel)
		{
			other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null;
		}
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x00066C97 File Offset: 0x00064E97
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
	}

	// Token: 0x04001761 RID: 5985
	public GorillaReportButton.MetaReportReason metaReportType;

	// Token: 0x04001762 RID: 5986
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x04001763 RID: 5987
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x04001764 RID: 5988
	public bool isOn;

	// Token: 0x04001765 RID: 5989
	public Material offMaterial;

	// Token: 0x04001766 RID: 5990
	public Material onMaterial;

	// Token: 0x04001767 RID: 5991
	public string offText;

	// Token: 0x04001768 RID: 5992
	public string onText;

	// Token: 0x04001769 RID: 5993
	public Text myText;

	// Token: 0x0400176A RID: 5994
	public float debounceTime = 0.25f;

	// Token: 0x0400176B RID: 5995
	public float touchTime;

	// Token: 0x0400176C RID: 5996
	public bool testPress;

	// Token: 0x0400176D RID: 5997
	public bool selected;

	// Token: 0x02000390 RID: 912
	[SerializeField]
	public enum MetaReportReason
	{
		// Token: 0x0400176F RID: 5999
		HateSpeech,
		// Token: 0x04001770 RID: 6000
		Cheating,
		// Token: 0x04001771 RID: 6001
		Toxicity,
		// Token: 0x04001772 RID: 6002
		Bullying,
		// Token: 0x04001773 RID: 6003
		Doxing,
		// Token: 0x04001774 RID: 6004
		Impersonation,
		// Token: 0x04001775 RID: 6005
		Submit,
		// Token: 0x04001776 RID: 6006
		Cancel
	}
}
