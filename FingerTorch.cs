using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020000FB RID: 251
public class FingerTorch : MonoBehaviour, ISpawnable
{
	// Token: 0x17000078 RID: 120
	// (get) Token: 0x0600063D RID: 1597 RVA: 0x00023DBB File Offset: 0x00021FBB
	// (set) Token: 0x0600063E RID: 1598 RVA: 0x00023DC3 File Offset: 0x00021FC3
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x0600063F RID: 1599 RVA: 0x00023DCC File Offset: 0x00021FCC
	// (set) Token: 0x06000640 RID: 1600 RVA: 0x00023DD4 File Offset: 0x00021FD4
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000641 RID: 1601 RVA: 0x00023DDD File Offset: 0x00021FDD
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x00023E00 File Offset: 0x00022000
	protected void OnEnable()
	{
		int num = (this.attachedToLeftHand ? 1 : 2);
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x000023F4 File Offset: 0x000005F4
	protected void OnDisable()
	{
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x00023E38 File Offset: 0x00022038
	private void UpdateLocal()
	{
		int num = (this.attachedToLeftHand ? 4 : 5);
		bool flag = ControllerInputPoller.GripFloat((XRNode)num) > 0.25f;
		bool flag2 = ControllerInputPoller.PrimaryButtonPress((XRNode)num);
		bool flag3 = ControllerInputPoller.SecondaryButtonPress((XRNode)num);
		bool flag4 = flag && (flag2 || flag3);
		this.networkedExtended = flag4;
		if (PhotonNetwork.InRoom && this.myRig)
		{
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.networkedExtended);
		}
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x00023EB8 File Offset: 0x000220B8
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
			this.particleFX.SetActive(this.extended);
		}
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00023EEC File Offset: 0x000220EC
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00023F25 File Offset: 0x00022125
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00023F42 File Offset: 0x00022142
	protected void LateUpdate()
	{
		if (this.IsMyItem())
		{
			this.UpdateLocal();
		}
		else
		{
			this.UpdateReplicated();
		}
		this.UpdateShared();
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x00023F60 File Offset: 0x00022160
	private void OnExtendStateChanged(bool playAudio)
	{
		this.audioSource.clip = (this.extended ? this.extendAudioClip : this.retractAudioClip);
		if (playAudio)
		{
			this.audioSource.GTPlay();
		}
		if (this.IsMyItem() && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(this.attachedToLeftHand, this.extended ? this.extendVibrationDuration : this.retractVibrationDuration, this.extended ? this.extendVibrationStrength : this.retractVibrationStrength);
		}
	}

	// Token: 0x0400076C RID: 1900
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x0400076D RID: 1901
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x0400076E RID: 1902
	public Transform thumbRingBone;

	// Token: 0x0400076F RID: 1903
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x04000770 RID: 1904
	public AudioClip extendAudioClip;

	// Token: 0x04000771 RID: 1905
	public AudioClip retractAudioClip;

	// Token: 0x04000772 RID: 1906
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x04000773 RID: 1907
	public float extendVibrationStrength = 0.2f;

	// Token: 0x04000774 RID: 1908
	public float retractVibrationDuration = 0.05f;

	// Token: 0x04000775 RID: 1909
	public float retractVibrationStrength = 0.2f;

	// Token: 0x04000776 RID: 1910
	[Header("Particle FX")]
	public GameObject particleFX;

	// Token: 0x04000777 RID: 1911
	private bool networkedExtended;

	// Token: 0x04000778 RID: 1912
	private bool extended;

	// Token: 0x04000779 RID: 1913
	private InputDevice inputDevice;

	// Token: 0x0400077A RID: 1914
	private VRRig myRig;

	// Token: 0x0400077B RID: 1915
	private int stateBitIndex;
}
