using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008DF RID: 2271
public class GorillaNetworkPublicTestsJoin : GorillaTriggerBox
{
	// Token: 0x06003738 RID: 14136 RVA: 0x000023F4 File Offset: 0x000005F4
	public void Awake()
	{
	}

	// Token: 0x06003739 RID: 14137 RVA: 0x0010B2B4 File Offset: 0x001094B4
	public void LateUpdate()
	{
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				if ((GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f) && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				float magnitude = (GTPlayer.Instance.transform.position - this.lastPosition).magnitude;
			}
			this.lastPosition = GTPlayer.Instance.transform.position;
		}
		catch
		{
		}
	}

	// Token: 0x0600373A RID: 14138 RVA: 0x0010B3E8 File Offset: 0x001095E8
	private IEnumerator GracePeriod()
	{
		this.waiting = true;
		yield return new WaitForSeconds(30f);
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic)
				{
					GorillaNot.instance.SendReport("gorvity bisdabled", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f)
				{
					GorillaNot.instance.SendReport(string.Concat(new string[]
					{
						"jimp 2mcuh.",
						GTPlayer.Instance.jumpMultiplier.ToString(),
						".",
						GTPlayer.Instance.maxJumpSpeed.ToString(),
						"."
					}), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GorillaTagger.Instance.sphereCastRadius > 0.04f)
				{
					GorillaNot.instance.SendReport("wack rad. " + GorillaTagger.Instance.sphereCastRadius.ToString(), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
			}
			this.waiting = false;
			yield break;
		}
		catch
		{
			yield break;
		}
		yield break;
	}

	// Token: 0x04003CCA RID: 15562
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04003CCB RID: 15563
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04003CCC RID: 15564
	public string gameModeName;

	// Token: 0x04003CCD RID: 15565
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04003CCE RID: 15566
	public string componentTypeToAdd;

	// Token: 0x04003CCF RID: 15567
	public GameObject componentTarget;

	// Token: 0x04003CD0 RID: 15568
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x04003CD1 RID: 15569
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x04003CD2 RID: 15570
	private Transform tosPition;

	// Token: 0x04003CD3 RID: 15571
	private Transform othsTosPosition;

	// Token: 0x04003CD4 RID: 15572
	private PhotonView fotVew;

	// Token: 0x04003CD5 RID: 15573
	private bool waiting;

	// Token: 0x04003CD6 RID: 15574
	private Vector3 lastPosition;

	// Token: 0x04003CD7 RID: 15575
	private VRRig tempRig;
}
