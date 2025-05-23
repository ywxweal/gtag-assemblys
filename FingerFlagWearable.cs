using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020000DD RID: 221
public class FingerFlagWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000585 RID: 1413 RVA: 0x000200D1 File Offset: 0x0001E2D1
	// (set) Token: 0x06000586 RID: 1414 RVA: 0x000200D9 File Offset: 0x0001E2D9
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x06000587 RID: 1415 RVA: 0x000200E2 File Offset: 0x0001E2E2
	// (set) Token: 0x06000588 RID: 1416 RVA: 0x000200EA File Offset: 0x0001E2EA
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000589 RID: 1417 RVA: 0x000200F3 File Offset: 0x0001E2F3
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = base.GetComponentInParent<VRRig>(true);
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0002011C File Offset: 0x0001E31C
	protected void OnEnable()
	{
		int num = (this.attachedToLeftHand ? 1 : 2);
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00020154 File Offset: 0x0001E354
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

	// Token: 0x0600058D RID: 1421 RVA: 0x000201D4 File Offset: 0x0001E3D4
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
		}
		bool flag = this.fullyRetracted;
		this.fullyRetracted = this.extended && this.retractExtendTime <= 0f;
		if (flag != this.fullyRetracted)
		{
			Transform[] array = this.clothRigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(!this.fullyRetracted);
			}
		}
		this.UpdateAnimation();
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x00020262 File Offset: 0x0001E462
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x0002029B File Offset: 0x0001E49B
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x000202B8 File Offset: 0x0001E4B8
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

	// Token: 0x06000591 RID: 1425 RVA: 0x000202D8 File Offset: 0x0001E4D8
	private void UpdateAnimation()
	{
		float num = (this.extended ? this.extendSpeed : (-this.retractSpeed));
		this.retractExtendTime = Mathf.Clamp01(this.retractExtendTime + Time.deltaTime * num);
		this.animator.SetFloat(this.retractExtendTimeAnimParam, this.retractExtendTime);
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x00020330 File Offset: 0x0001E530
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

	// Token: 0x0400067E RID: 1662
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x0400067F RID: 1663
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x04000680 RID: 1664
	public Transform thumbRingBone;

	// Token: 0x04000681 RID: 1665
	public Transform[] clothBones;

	// Token: 0x04000682 RID: 1666
	public Transform[] clothRigidbodies;

	// Token: 0x04000683 RID: 1667
	[Header("Animation")]
	public Animator animator;

	// Token: 0x04000684 RID: 1668
	public float extendSpeed = 1.5f;

	// Token: 0x04000685 RID: 1669
	public float retractSpeed = 2.25f;

	// Token: 0x04000686 RID: 1670
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x04000687 RID: 1671
	public AudioClip extendAudioClip;

	// Token: 0x04000688 RID: 1672
	public AudioClip retractAudioClip;

	// Token: 0x04000689 RID: 1673
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x0400068A RID: 1674
	public float extendVibrationStrength = 0.2f;

	// Token: 0x0400068B RID: 1675
	public float retractVibrationDuration = 0.05f;

	// Token: 0x0400068C RID: 1676
	public float retractVibrationStrength = 0.2f;

	// Token: 0x0400068D RID: 1677
	private readonly int retractExtendTimeAnimParam = Animator.StringToHash("retractExtendTime");

	// Token: 0x0400068E RID: 1678
	private bool networkedExtended;

	// Token: 0x0400068F RID: 1679
	private bool extended;

	// Token: 0x04000690 RID: 1680
	private bool fullyRetracted;

	// Token: 0x04000691 RID: 1681
	private float retractExtendTime;

	// Token: 0x04000692 RID: 1682
	private InputDevice inputDevice;

	// Token: 0x04000693 RID: 1683
	private VRRig myRig;

	// Token: 0x04000694 RID: 1684
	private int stateBitIndex;
}
