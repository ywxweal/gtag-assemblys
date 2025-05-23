using System;
using GorillaGameModes;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000664 RID: 1636
public class HoldableHand : HoldableObject
{
	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x060028E0 RID: 10464 RVA: 0x000CBA98 File Offset: 0x000C9C98
	public VRRig Rig
	{
		get
		{
			return this.myPlayer;
		}
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x000CBAA0 File Offset: 0x000C9CA0
	private void Start()
	{
		if (this.myPlayer.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x000CBABC File Offset: 0x000C9CBC
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && !this.myPlayer.creator.IsLocal && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
			this.myPlayer.netView.SendRPC("GrabbedByPlayer", this.myPlayer.Creator, new object[] { this.isBody, this.isLeftHand, flag });
			this.myPlayer.ApplyLocalGrabOverride(this.isBody, this.isLeftHand, grabbingHand.transform);
			EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
			this.ClearOtherGrabs(flag);
		}
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000CBB94 File Offset: 0x000C9D94
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && !this.myPlayer.creator.IsLocal)
		{
			bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
			Vector3 vector = Vector3.zero;
			if (gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				vector = (flag ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
			}
			vector = Vector3.ClampMagnitude(vector, 20f);
			this.myPlayer.netView.SendRPC("DroppedByPlayer", this.myPlayer.Creator, new object[] { vector });
			this.myPlayer.ClearLocalGrabOverride();
			this.myPlayer.ApplyLocalTrajectoryOverride(vector);
			EquipmentInteractor.instance.UpdateHandEquipment(null, flag);
		}
		return true;
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x000CBC85 File Offset: 0x000C9E85
	public override void DropItemCleanup()
	{
		this.myPlayer.ClearLocalGrabOverride();
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x000CBC94 File Offset: 0x000C9E94
	private void ClearOtherGrabs(bool grabbedLeft)
	{
		IHoldableObject holdableObject = (grabbedLeft ? EquipmentInteractor.instance.rightHandHeldEquipment : EquipmentInteractor.instance.leftHandHeldEquipment);
		if (this.isBody)
		{
			if (holdableObject == this.myPlayer.leftHolds || holdableObject == this.myPlayer.rightHolds)
			{
				EquipmentInteractor.instance.UpdateHandEquipment(null, !grabbedLeft);
				return;
			}
		}
		else if (this.isLeftHand)
		{
			if (holdableObject == this.myPlayer.rightHolds || holdableObject == this.myPlayer.bodyHolds)
			{
				EquipmentInteractor.instance.UpdateHandEquipment(null, !grabbedLeft);
				return;
			}
		}
		else if (holdableObject == this.myPlayer.leftHolds || holdableObject == this.myPlayer.bodyHolds)
		{
			EquipmentInteractor.instance.UpdateHandEquipment(null, !grabbedLeft);
		}
	}

	// Token: 0x04002DF2 RID: 11762
	[SerializeField]
	private VRRig myPlayer;

	// Token: 0x04002DF3 RID: 11763
	[SerializeField]
	private bool isBody;

	// Token: 0x04002DF4 RID: 11764
	[SerializeField]
	private bool isLeftHand;
}
