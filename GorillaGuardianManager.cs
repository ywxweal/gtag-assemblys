using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000605 RID: 1541
public class GorillaGuardianManager : GorillaGameManager
{
	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x06002621 RID: 9761 RVA: 0x000BCA3B File Offset: 0x000BAC3B
	// (set) Token: 0x06002622 RID: 9762 RVA: 0x000BCA43 File Offset: 0x000BAC43
	public bool isPlaying { get; private set; }

	// Token: 0x06002623 RID: 9763 RVA: 0x000BCA4C File Offset: 0x000BAC4C
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.isPlaying = true;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StartPlaying();
			}
		}
	}

	// Token: 0x06002624 RID: 9764 RVA: 0x000BCAB0 File Offset: 0x000BACB0
	public override void StopPlaying()
	{
		base.StopPlaying();
		this.isPlaying = false;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StopPlaying();
			}
		}
	}

	// Token: 0x06002625 RID: 9765 RVA: 0x000BCB14 File Offset: 0x000BAD14
	public override void Reset()
	{
		base.Reset();
	}

	// Token: 0x06002626 RID: 9766 RVA: 0x000BCB1C File Offset: 0x000BAD1C
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<GuardianRPCs>();
	}

	// Token: 0x06002627 RID: 9767 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
	}

	// Token: 0x06002628 RID: 9768 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x06002629 RID: 9769 RVA: 0x00045F91 File Offset: 0x00044191
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x0600262A RID: 9770 RVA: 0x000BCB2C File Offset: 0x000BAD2C
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return this.IsPlayerGuardian(myPlayer) && !this.IsHoldingPlayer();
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x000BCB42 File Offset: 0x000BAD42
	public override bool CanJoinFrienship(NetPlayer player)
	{
		return player != null && !this.IsPlayerGuardian(player);
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x000BCB54 File Offset: 0x000BAD54
	public bool IsPlayerGuardian(NetPlayer player)
	{
		using (List<GorillaGuardianZoneManager>.Enumerator enumerator = GorillaGuardianZoneManager.zoneManagers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsPlayerGuardian(player))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x000BCBB0 File Offset: 0x000BADB0
	public void RequestEjectGuardian(NetPlayer player)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.EjectGuardian(player);
			return;
		}
		global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianRequestEject", false, Array.Empty<object>());
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x000BCBD8 File Offset: 0x000BADD8
	public void EjectGuardian(NetPlayer player)
	{
		foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
		{
			if (gorillaGuardianZoneManager.IsPlayerGuardian(player))
			{
				gorillaGuardianZoneManager.SetGuardian(null);
			}
		}
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x000BCC34 File Offset: 0x000BAE34
	public void LaunchPlayer(NetPlayer launcher, Vector3 velocity)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(launcher, out rigContainer))
		{
			return;
		}
		if (Vector3.Magnitude(VRRigCache.Instance.localRig.Rig.transform.position - rigContainer.Rig.transform.position) > this.requiredGuardianDistance + Mathf.Epsilon)
		{
			return;
		}
		if (velocity.sqrMagnitude > this.maxLaunchVelocity * this.maxLaunchVelocity)
		{
			return;
		}
		GTPlayer.Instance.DoLaunch(velocity);
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x000BCCB8 File Offset: 0x000BAEB8
	public override void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
		base.LocalTag(taggedPlayer, taggingPlayer, bodyHit, leftHand);
		if (bodyHit)
		{
			return;
		}
		RigContainer rigContainer;
		Vector3 vector;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) && this.CheckSlap(taggingPlayer, taggedPlayer, leftHand, out vector))
		{
			global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", taggedPlayer, new object[] { vector });
			rigContainer.Rig.ApplyLocalTrajectoryOverride(vector);
			global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("ShowSlapEffects", true, new object[]
			{
				rigContainer.Rig.transform.position,
				vector.normalized
			});
			this.LocalPlaySlapEffect(rigContainer.Rig.transform.position, vector.normalized);
		}
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x000BCD7C File Offset: 0x000BAF7C
	private bool CheckSlap(NetPlayer slapper, NetPlayer target, bool leftHand, out Vector3 velocity)
	{
		velocity = Vector3.zero;
		if (this.IsHoldingPlayer(leftHand))
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(slapper, out rigContainer))
		{
			return false;
		}
		Vector3 vector = (leftHand ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
		Vector3 vector2 = (leftHand ? rigContainer.Rig.leftHandHoldsPlayer.transform.right : rigContainer.Rig.rightHandHoldsPlayer.transform.right);
		if (Vector3.Dot(vector.normalized, vector2) < this.slapFrontAlignmentThreshold && Vector3.Dot(vector.normalized, vector2) > this.slapBackAlignmentThreshold)
		{
			return false;
		}
		if (vector.magnitude < this.launchMinimumStrength)
		{
			return false;
		}
		vector = Vector3.ClampMagnitude(vector, this.maxLaunchVelocity);
		RigContainer rigContainer2;
		if (!VRRigCache.Instance.TryGetVrrig(target, out rigContainer2))
		{
			return false;
		}
		if (this.IsRigBeingHeld(rigContainer2.Rig) || rigContainer2.Rig.IsLocalTrajectoryOverrideActive())
		{
			return false;
		}
		if (!this.CheckLaunchRetriggerDelay(rigContainer2.Rig))
		{
			return false;
		}
		vector *= this.launchStrengthMultiplier;
		Vector3 vector3;
		if (rigContainer2.Rig.IsOnGround(this.launchGroundHeadCheckDist, this.launchGroundHandCheckDist, out vector3))
		{
			vector += vector3 * this.launchGroundKickup * Mathf.Clamp01(1f - Vector3.Dot(vector3, vector.normalized));
		}
		velocity = vector;
		return true;
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x000BCEF8 File Offset: 0x000BB0F8
	public override void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
		base.HandleHandTap(tappingPlayer, hitTappable, leftHand, handVelocity, tapSurfaceNormal);
		if (hitTappable != null)
		{
			TappableGuardianIdol tappableGuardianIdol = hitTappable as TappableGuardianIdol;
			if (tappableGuardianIdol != null && tappableGuardianIdol.isActivationReady)
			{
				tappableGuardianIdol.isActivationReady = false;
				GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tapHapticStrength * this.hapticStrength, GorillaTagger.Instance.tapHapticDuration * this.hapticDuration);
			}
		}
		if (!this.IsPlayerGuardian(tappingPlayer))
		{
			return;
		}
		if (this.IsHoldingPlayer(leftHand))
		{
			return;
		}
		float num = Vector3.Dot(Vector3.down, handVelocity);
		if (num < this.slamTriggerTapSpeed || Vector3.Dot(Vector3.down, handVelocity.normalized) < this.slamTriggerAngle)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(tappingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = (leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand);
		Vector3 vector = vrmap.rigTarget.rotation * vrmap.trackingPositionOffset * rigContainer.Rig.scaleFactor;
		Vector3 vector2 = vrmap.rigTarget.position - vector;
		float num2 = Mathf.Clamp01((num - this.slamTriggerTapSpeed) / (this.slamMaxTapSpeed - this.slamTriggerTapSpeed));
		num2 = Mathf.Lerp(this.slamMinStrengthMultiplier, this.slamMaxStrengthMultiplier, num2);
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			RigContainer rigContainer2;
			if (RoomSystem.PlayersInRoom[i] != tappingPlayer && VRRigCache.Instance.TryGetVrrig(RoomSystem.PlayersInRoom[i], out rigContainer2))
			{
				VRRig rig = rigContainer2.Rig;
				if (!this.IsRigBeingHeld(rig) && this.CheckLaunchRetriggerDelay(rig))
				{
					Vector3 position = rig.transform.position;
					if (Vector3.SqrMagnitude(position - vector2) < this.slamRadius * this.slamRadius)
					{
						Vector3 vector3 = (position - vector2).normalized * num2;
						vector3 = Vector3.ClampMagnitude(vector3, this.maxLaunchVelocity);
						global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", RoomSystem.PlayersInRoom[i], new object[] { vector3 });
					}
				}
			}
		}
		this.LocalPlaySlamEffect(vector2, Vector3.up);
		global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("ShowSlamEffect", true, new object[]
		{
			vector2,
			Vector3.up
		});
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x000BD16A File Offset: 0x000BB36A
	private bool CheckLaunchRetriggerDelay(VRRig launchedRig)
	{
		return launchedRig.fxSettings.callSettings[7].CallLimitSettings.CheckCallTime(Time.time);
	}

	// Token: 0x06002634 RID: 9780 RVA: 0x000BD188 File Offset: 0x000BB388
	private bool IsHoldingPlayer()
	{
		return this.IsHoldingPlayer(true) || this.IsHoldingPlayer(false);
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x000BD19C File Offset: 0x000BB39C
	private bool IsHoldingPlayer(bool leftHand)
	{
		return (leftHand && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment is HoldableHand) || (!leftHand && EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment is HoldableHand);
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000BD1F8 File Offset: 0x000BB3F8
	private bool IsRigBeingHeld(VRRig rig)
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment != null)
		{
			HoldableHand holdableHand = EquipmentInteractor.instance.leftHandHeldEquipment as HoldableHand;
			if (holdableHand != null && holdableHand.Rig == rig)
			{
				return true;
			}
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null)
		{
			HoldableHand holdableHand2 = EquipmentInteractor.instance.rightHandHeldEquipment as HoldableHand;
			if (holdableHand2 != null && holdableHand2.Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002639 RID: 9785 RVA: 0x000BD26C File Offset: 0x000BB46C
	public override GameModeType GameType()
	{
		return GameModeType.Guardian;
	}

	// Token: 0x0600263A RID: 9786 RVA: 0x000BD26F File Offset: 0x000BB46F
	public override string GameModeName()
	{
		return "GUARDIAN";
	}

	// Token: 0x0600263B RID: 9787 RVA: 0x000BD276 File Offset: 0x000BB476
	public void PlaySlapEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlapEffect(location, direction);
	}

	// Token: 0x0600263C RID: 9788 RVA: 0x000BD280 File Offset: 0x000BB480
	private void LocalPlaySlapEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slapImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x0600263D RID: 9789 RVA: 0x000BD29B File Offset: 0x000BB49B
	public void PlaySlamEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlamEffect(location, direction);
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x000BD2A5 File Offset: 0x000BB4A5
	private void LocalPlaySlamEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slamImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x04002A9F RID: 10911
	[Space]
	[SerializeField]
	private float slapFrontAlignmentThreshold = 0.7f;

	// Token: 0x04002AA0 RID: 10912
	[SerializeField]
	private float slapBackAlignmentThreshold = 0.7f;

	// Token: 0x04002AA1 RID: 10913
	[SerializeField]
	private float launchMinimumStrength = 6f;

	// Token: 0x04002AA2 RID: 10914
	[SerializeField]
	private float launchStrengthMultiplier = 1f;

	// Token: 0x04002AA3 RID: 10915
	[SerializeField]
	private float launchGroundHeadCheckDist = 1.2f;

	// Token: 0x04002AA4 RID: 10916
	[SerializeField]
	private float launchGroundHandCheckDist = 0.4f;

	// Token: 0x04002AA5 RID: 10917
	[SerializeField]
	private float launchGroundKickup = 3f;

	// Token: 0x04002AA6 RID: 10918
	[Space]
	[SerializeField]
	private float slamTriggerTapSpeed = 7f;

	// Token: 0x04002AA7 RID: 10919
	[SerializeField]
	private float slamMaxTapSpeed = 16f;

	// Token: 0x04002AA8 RID: 10920
	[SerializeField]
	private float slamTriggerAngle = 0.7f;

	// Token: 0x04002AA9 RID: 10921
	[SerializeField]
	private float slamRadius = 2.4f;

	// Token: 0x04002AAA RID: 10922
	[SerializeField]
	private float slamMinStrengthMultiplier = 3f;

	// Token: 0x04002AAB RID: 10923
	[SerializeField]
	private float slamMaxStrengthMultiplier = 10f;

	// Token: 0x04002AAC RID: 10924
	[Space]
	[SerializeField]
	private GameObject slapImpactPrefab;

	// Token: 0x04002AAD RID: 10925
	[SerializeField]
	private GameObject slamImpactPrefab;

	// Token: 0x04002AAE RID: 10926
	[Space]
	[SerializeField]
	private float hapticStrength = 1f;

	// Token: 0x04002AAF RID: 10927
	[SerializeField]
	private float hapticDuration = 1f;

	// Token: 0x04002AB1 RID: 10929
	private float requiredGuardianDistance = 10f;

	// Token: 0x04002AB2 RID: 10930
	private float maxLaunchVelocity = 20f;
}
