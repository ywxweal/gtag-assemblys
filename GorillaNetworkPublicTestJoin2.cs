using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008DD RID: 2269
public class GorillaNetworkPublicTestJoin2 : GorillaTriggerBox
{
	// Token: 0x0600372F RID: 14127 RVA: 0x000023F4 File Offset: 0x000005F4
	public void Awake()
	{
	}

	// Token: 0x06003730 RID: 14128 RVA: 0x0010B074 File Offset: 0x00109274
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

	// Token: 0x06003731 RID: 14129 RVA: 0x0010B1A8 File Offset: 0x001093A8
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

	// Token: 0x04003CBA RID: 15546
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04003CBB RID: 15547
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04003CBC RID: 15548
	public string gameModeName;

	// Token: 0x04003CBD RID: 15549
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04003CBE RID: 15550
	public string componentTypeToAdd;

	// Token: 0x04003CBF RID: 15551
	public GameObject componentTarget;

	// Token: 0x04003CC0 RID: 15552
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x04003CC1 RID: 15553
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x04003CC2 RID: 15554
	private Transform tosPition;

	// Token: 0x04003CC3 RID: 15555
	private Transform othsTosPosition;

	// Token: 0x04003CC4 RID: 15556
	private PhotonView fotVew;

	// Token: 0x04003CC5 RID: 15557
	private bool waiting;

	// Token: 0x04003CC6 RID: 15558
	private Vector3 lastPosition;

	// Token: 0x04003CC7 RID: 15559
	private VRRig tempRig;
}
