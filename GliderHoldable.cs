﻿using System;
using System.Collections;
using System.Runtime.InteropServices;
using AA;
using Fusion;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020008F7 RID: 2295
[RequireComponent(typeof(Rigidbody))]
[NetworkBehaviourWeaved(11)]
public class GliderHoldable : NetworkHoldableObject, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x1700057F RID: 1407
	// (get) Token: 0x060037A4 RID: 14244 RVA: 0x0010C454 File Offset: 0x0010A654
	private bool OutOfBounds
	{
		get
		{
			return this.maxDistanceRespawnOrigin != null && (this.maxDistanceRespawnOrigin.position - base.transform.position).sqrMagnitude > this.maxDistanceBeforeRespawn * this.maxDistanceBeforeRespawn;
		}
	}

	// Token: 0x060037A5 RID: 14245 RVA: 0x0010C4A4 File Offset: 0x0010A6A4
	protected override void Awake()
	{
		base.Awake();
		base.transform.parent = null;
		this.defaultMaxDistanceBeforeRespawn = this.maxDistanceBeforeRespawn;
		this.spawnPosition = (this.skyJungleSpawnPostion = base.transform.position);
		this.spawnRotation = (this.skyJungleSpawnRotation = base.transform.rotation);
		this.skyJungleRespawnOrigin = this.maxDistanceRespawnOrigin;
		this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		this.rb = base.GetComponent<Rigidbody>();
		this.yaw = base.transform.rotation.eulerAngles.y;
		this.oneHandRotationRateExp = Mathf.Exp(this.oneHandHoldRotationRate);
		this.twoHandRotationRateExp = Mathf.Exp(this.twoHandHoldRotationRate);
		this.subtlePlayerPitchRateExp = Mathf.Exp(this.subtlePlayerPitchRate);
		this.subtlePlayerRollRateExp = Mathf.Exp(this.subtlePlayerRollRate);
		this.accelSmoothingFollowRateExp = Mathf.Exp(this.accelSmoothingFollowRate);
		this.networkSyncFollowRateExp = Mathf.Exp(this.networkSyncFollowRate);
		this.ownershipGuard.AddCallbackTarget(this);
		this.calmAudio.volume = 0f;
		this.activeAudio.volume = 0f;
		this.whistlingAudio.volume = 0f;
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x0010C5F2 File Offset: 0x0010A7F2
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.ownershipGuard != null)
		{
			this.ownershipGuard.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x0010C614 File Offset: 0x0010A814
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
	}

	// Token: 0x060037A8 RID: 14248 RVA: 0x0010C622 File Offset: 0x0010A822
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.Respawn();
		base.OnDisable();
	}

	// Token: 0x060037A9 RID: 14249 RVA: 0x0010C638 File Offset: 0x0010A838
	public void Respawn()
	{
		if ((base.IsValid && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			if (EquipmentInteractor.instance != null)
			{
				if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.leftHand);
				}
				if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.rightHand);
				}
			}
			this.rb.isKinematic = true;
			base.transform.position = this.spawnPosition;
			base.transform.rotation = this.spawnRotation;
			this.lastHeldTime = -1f;
			this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		}
	}

	// Token: 0x060037AA RID: 14250 RVA: 0x0010C709 File Offset: 0x0010A909
	public void CustomMapLoad(Transform placeholderTransform, float respawnDistance)
	{
		this.maxDistanceRespawnOrigin = placeholderTransform;
		this.spawnPosition = placeholderTransform.position;
		this.spawnRotation = placeholderTransform.rotation;
		this.maxDistanceBeforeRespawn = respawnDistance;
		this.Respawn();
	}

	// Token: 0x060037AB RID: 14251 RVA: 0x0010C737 File Offset: 0x0010A937
	public void CustomMapUnload()
	{
		this.maxDistanceRespawnOrigin = this.skyJungleRespawnOrigin;
		this.spawnPosition = this.skyJungleSpawnPostion;
		this.spawnRotation = this.skyJungleSpawnRotation;
		this.maxDistanceBeforeRespawn = this.defaultMaxDistanceBeforeRespawn;
		this.Respawn();
	}

	// Token: 0x17000580 RID: 1408
	// (get) Token: 0x060037AC RID: 14252 RVA: 0x00047642 File Offset: 0x00045842
	public override bool TwoHanded
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x0010C770 File Offset: 0x0010A970
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
		}
	}

	// Token: 0x060037AE RID: 14254 RVA: 0x0010C7EC File Offset: 0x0010A9EC
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
			return;
		}
		if (NetworkSystem.Instance.InRoom && !base.IsMine && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
		}
	}

	// Token: 0x060037AF RID: 14255 RVA: 0x0010C894 File Offset: 0x0010AA94
	public void OnGrabAuthority(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if ((flag && !EquipmentInteractor.instance.isLeftGrabbing) || (!flag && !EquipmentInteractor.instance.isRightGrabbing))
		{
			return;
		}
		if (this.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		Vector3 vector = this.ClosestPointInHandle(grabbingHand.transform.position, pointGrabbed);
		if (flag)
		{
			this.leftHold.Activate(grabbingHand.transform, base.transform, vector);
		}
		else
		{
			this.rightHold.Activate(grabbingHand.transform, base.transform, vector);
		}
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 handsVector = this.GetHandsVector(this.leftHold.transform.position, this.rightHold.transform.position, GTPlayer.Instance.headCollider.transform.position, true);
			this.twoHandRotationOffsetAxis = Vector3.Cross(handsVector, base.transform.right).normalized;
			if ((double)this.twoHandRotationOffsetAxis.sqrMagnitude < 0.001)
			{
				this.twoHandRotationOffsetAxis = base.transform.right;
				this.twoHandRotationOffsetAngle = 0f;
			}
			else
			{
				this.twoHandRotationOffsetAngle = Vector3.SignedAngle(handsVector, base.transform.right, this.twoHandRotationOffsetAxis);
			}
		}
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		this.ridersMaterialOverideIndex = 0;
		if (this.cosmeticMaterialOverrides.Length != 0)
		{
			VRRig offlineVRRig = this.cachedRig;
			if (offlineVRRig == null)
			{
				offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			}
			if (offlineVRRig != null)
			{
				for (int i = 0; i < this.cosmeticMaterialOverrides.Length; i++)
				{
					if (this.cosmeticMaterialOverrides[i].cosmeticName != null && offlineVRRig.cosmeticSet != null && offlineVRRig.cosmeticSet.HasItem(this.cosmeticMaterialOverrides[i].cosmeticName))
					{
						this.ridersMaterialOverideIndex = i + 1;
						break;
					}
				}
			}
		}
		this.infectedState = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			this.infectedState = this.syncedState.tagged;
		}
		if (this.infectedState)
		{
			this.leafMesh.material = this.GetInfectedMaterial();
		}
		else
		{
			this.leafMesh.material = this.GetMaterialFromIndex((byte)this.ridersMaterialOverideIndex);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != EquipmentInteractor.instance.rightHandHeldEquipment)
		{
			this.holdingTwoGliders = true;
		}
	}

	// Token: 0x060037B0 RID: 14256 RVA: 0x0010CC20 File Offset: 0x0010AE20
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		this.holdingTwoGliders = false;
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		if (this.leftHold.active && this.rightHold.active)
		{
			if (flag)
			{
				this.rightHold.Activate(this.rightHold.transform, base.transform, this.ClosestPointInHandle(this.rightHold.transform.position, this.handle));
			}
			else
			{
				this.leftHold.Activate(this.leftHold.transform, base.transform, this.ClosestPointInHandle(this.leftHold.transform.position, this.handle));
			}
		}
		Vector3 vector = Vector3.zero;
		if (flag)
		{
			this.leftHold.Deactivate();
			vector = GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		else
		{
			this.rightHold.Deactivate();
			vector = GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.audioLevel = 0f;
			this.riderId = -1;
			this.cachedRig = null;
			this.subtlePlayerPitch = 0f;
			this.subtlePlayerRoll = 0f;
			this.leftHoldPositionLocal = null;
			this.rightHoldPositionLocal = null;
			this.ridersMaterialOverideIndex = 0;
			if (base.IsMine || !NetworkSystem.Instance.InRoom)
			{
				this.rb.isKinematic = false;
				this.rb.useGravity = true;
				this.rb.velocity = vector;
				this.syncedState.riderId = -1;
				this.syncedState.tagged = false;
				this.syncedState.materialIndex = 0;
				this.syncedState.position = base.transform.position;
				this.syncedState.rotation = base.transform.rotation;
				this.syncedState.audioLevel = 0;
			}
			this.leafMesh.material = this.baseLeafMaterial;
		}
		return true;
	}

	// Token: 0x060037B1 RID: 14257 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void DropItemCleanup()
	{
	}

	// Token: 0x060037B2 RID: 14258 RVA: 0x0010CE68 File Offset: 0x0010B068
	public void FixedUpdate()
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (this.holdingTwoGliders)
		{
			instance.AddForce(Physics.gravity, ForceMode.Acceleration);
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.previousVelocity = this.currentVelocity;
			this.currentVelocity = instance.RigidbodyVelocity;
			float magnitude = this.currentVelocity.magnitude;
			this.accelerationAverage.AddSample((this.currentVelocity - this.previousVelocity) / Time.fixedDeltaTime, Time.fixedTime);
			float rollAngle180Wrapping = this.GetRollAngle180Wrapping();
			float num = this.liftIncreaseVsRoll.Evaluate(Mathf.Clamp01(Mathf.Abs(rollAngle180Wrapping / 180f))) * this.liftIncreaseVsRollMaxAngle;
			Vector3 vector = Vector3.RotateTowards(this.currentVelocity, Quaternion.AngleAxis(num, -base.transform.right) * base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime);
			Vector3 vector2 = vector - this.currentVelocity;
			float num2 = this.NormalizeAngle180(Vector3.SignedAngle(Vector3.ProjectOnPlane(this.currentVelocity, base.transform.right), base.transform.forward, base.transform.right));
			if (num2 > 90f)
			{
				num2 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num2));
			}
			else if (num2 < -90f)
			{
				num2 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num2));
			}
			float num3 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, num2));
			Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, this.pitch));
			float num4 = this.liftVsAttack.Evaluate(num3);
			instance.AddForce(vector2 * num4, ForceMode.VelocityChange);
			float num5 = this.dragVsAttack.Evaluate(num3);
			float num6 = ((this.syncedState.riderId != -1 && this.syncedState.materialIndex == 1) ? (this.dragVsSpeedMaxSpeed + this.infectedSpeedIncrease) : this.dragVsSpeedMaxSpeed);
			float num7 = this.dragVsSpeed.Evaluate(Mathf.Clamp01(magnitude / num6));
			float num8 = Mathf.Clamp01(num5 * this.attackDragFactor + num7 * this.dragVsSpeedDragFactor);
			instance.AddForce(-this.currentVelocity * num8, ForceMode.Acceleration);
			if (this.pitch > 0f && this.currentVelocity.y > 0f && (this.currentVelocity - this.previousVelocity).y > 0f)
			{
				float num9 = Mathf.InverseLerp(0f, this.pullUpLiftActivationVelocity, this.currentVelocity.y);
				float num10 = Mathf.InverseLerp(0f, this.pullUpLiftActivationAcceleration, (this.currentVelocity - this.previousVelocity).y / fixedDeltaTime);
				float num11 = Mathf.Min(num9, num10);
				instance.AddForce(-Physics.gravity * this.pullUpLiftBonus * num11, ForceMode.Acceleration);
			}
			if (Vector3.Dot(vector, Physics.gravity) > 0f)
			{
				instance.AddForce(-Physics.gravity * this.gravityCompensation, ForceMode.Acceleration);
				return;
			}
		}
		else
		{
			Vector3 vector3 = this.WindResistanceForceOffset(base.transform.up, Vector3.down);
			Vector3 vector4 = base.transform.position - vector3 * this.gravityUprightTorqueMultiplier;
			this.rb.AddForceAtPosition(-this.fallingGravityReduction * Physics.gravity, vector4, ForceMode.Acceleration);
		}
	}

	// Token: 0x060037B3 RID: 14259 RVA: 0x0010D25C File Offset: 0x0010B45C
	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.AuthorityUpdate(deltaTime);
			return;
		}
		this.RemoteSyncUpdate(deltaTime);
	}

	// Token: 0x060037B4 RID: 14260 RVA: 0x0010D29C File Offset: 0x0010B49C
	private void AuthorityUpdate(float dt)
	{
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.AuthorityUpdateUnheld(dt);
		}
		else if (this.leftHold.active || this.rightHold.active)
		{
			this.AuthorityUpdateHeld(dt);
		}
		this.syncedState.audioLevel = (byte)Mathf.FloorToInt(255f * this.audioLevel);
	}

	// Token: 0x060037B5 RID: 14261 RVA: 0x0010D30C File Offset: 0x0010B50C
	private void AuthorityUpdateHeld(float dt)
	{
		if (this.gliderState != GliderHoldable.GliderState.LocallyHeld)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyHeld;
		}
		this.rb.isKinematic = true;
		this.lastHeldTime = Time.time;
		if (this.leftHold.active)
		{
			this.leftHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.leftHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		if (this.rightHold.active)
		{
			this.rightHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.rightHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		Vector3 vector = Vector3.zero;
		if (this.leftHold.active && this.rightHold.active)
		{
			vector = (this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos)) * 0.5f;
		}
		else if (this.leftHold.active)
		{
			vector = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos);
		}
		else if (this.rightHold.active)
		{
			vector = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos);
		}
		this.UpdateGliderPosition();
		float magnitude = this.currentVelocity.magnitude;
		if (this.setMaxHandSlipDuringFlight && magnitude > this.maxSlipOverrideSpeedThreshold)
		{
			if (this.leftHold.active)
			{
				GTPlayer.Instance.SetLeftMaximumSlipThisFrame();
			}
			if (this.rightHold.active)
			{
				GTPlayer.Instance.SetRightMaximumSlipThisFrame();
			}
		}
		bool flag = false;
		GorillaTagManager gorillaTagManager = GorillaGameManager.instance as GorillaTagManager;
		if (gorillaTagManager != null)
		{
			flag = gorillaTagManager.IsInfected(NetworkSystem.Instance.LocalPlayer);
		}
		bool flag2 = flag != this.infectedState;
		this.infectedState = flag;
		if (flag2)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		Vector3 average = this.accelerationAverage.GetAverage();
		this.accelerationSmoothed = Mathf.Lerp(average.magnitude, this.accelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
		float num = Mathf.InverseLerp(this.hapticMaxSpeedInputRange.x, this.hapticMaxSpeedInputRange.y, magnitude);
		float num2 = Mathf.InverseLerp(this.hapticAccelInputRange.x, this.hapticAccelInputRange.y, this.accelerationSmoothed);
		float num3 = Mathf.InverseLerp(this.hapticSpeedInputRange.x, this.hapticSpeedInputRange.y, magnitude);
		this.UpdateAudioSource(this.calmAudio, num * this.audioVolumeMultiplier);
		this.UpdateAudioSource(this.activeAudio, num2 * num * this.audioVolumeMultiplier);
		if (this.infectedState)
		{
			this.UpdateAudioSource(this.whistlingAudio, Mathf.InverseLerp(this.whistlingAudioSpeedInputRange.x, this.whistlingAudioSpeedInputRange.y, magnitude) * num2 * num * this.audioVolumeMultiplier);
		}
		else
		{
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
		float num4 = Mathf.Max(num2 * this.hapticAccelOutputMax * num, num3 * this.hapticSpeedOutputMax);
		if (this.rightHold.active)
		{
			GorillaTagger.Instance.DoVibration(XRNode.RightHand, num4, dt);
		}
		if (this.leftHold.active)
		{
			GorillaTagger.Instance.DoVibration(XRNode.LeftHand, num4, dt);
		}
		Vector3 vector2 = this.handle.transform.position + this.handle.transform.rotation * new Vector3(0f, 0f, 1f);
		if (Time.frameCount % 2 == 0)
		{
			Vector3 vector3 = this.handle.transform.rotation * new Vector3(-0.707f, 0f, 0.707f);
			RaycastHit raycastHit;
			if (this.leftWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(vector2, vector3), out raycastHit, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				this.leftWhooshStartTime = Time.time;
				this.leftWhooshHitPoint = raycastHit.point;
				this.leftWhooshAudio.GTStop();
				this.leftWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.leftWhooshAudio.GTPlay();
			}
		}
		else
		{
			Vector3 vector4 = this.handle.transform.rotation * new Vector3(0.707f, 0f, 0.707f);
			RaycastHit raycastHit2;
			if (this.rightWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(vector2, vector4), out raycastHit2, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				this.rightWhooshStartTime = Time.time;
				this.rightWhooshHitPoint = raycastHit2.point;
				this.rightWhooshAudio.GTStop();
				this.rightWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.rightWhooshAudio.GTPlay();
			}
		}
		Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
		if (this.leftWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.leftWhooshAudio.transform.position = this.leftWhooshHitPoint;
		}
		else
		{
			this.leftWhooshAudio.transform.localPosition = new Vector3(-this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.rightWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.rightWhooshAudio.transform.position = this.rightWhooshHitPoint;
		}
		else
		{
			this.rightWhooshAudio.transform.localPosition = new Vector3(this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.extendTagRangeInFlight)
		{
			float num5 = Mathf.Lerp(this.tagRangeOutput.x, this.tagRangeOutput.y, Mathf.InverseLerp(this.tagRangeSpeedInput.x, this.tagRangeSpeedInput.y, magnitude));
			GorillaTagger.Instance.SetTagRadiusOverrideThisFrame(num5);
			if (this.debugDrawTagRange)
			{
				GorillaTagger.Instance.DebugDrawTagCasts(Color.yellow);
			}
		}
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.right, Vector3.up).normalized;
		Vector3 normalized2 = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float num6 = -Vector3.Dot(vector - this.handle.transform.position, normalized2);
		Vector3 vector5 = this.handle.transform.position - normalized2 * (this.riderPosRange.y * 0.5f + this.riderPosRangeOffset + num6);
		float num7 = Vector3.Dot(headCenterPosition - vector5, normalized);
		float num8 = Vector3.Dot(headCenterPosition - vector5, normalized2);
		num7 /= this.riderPosRange.x * 0.5f;
		num8 /= this.riderPosRange.y * 0.5f;
		this.riderPosition.x = Mathf.Sign(num7) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.x, 1f, Mathf.Abs(num7)));
		this.riderPosition.y = Mathf.Sign(num8) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.y, 1f, Mathf.Abs(num8)));
		Vector3 vector6;
		Vector3 vector7;
		if (this.leftHold.active && this.rightHold.active)
		{
			vector6 = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector6));
			vector7 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector7));
		}
		else if (this.leftHold.active)
		{
			vector6 = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector6));
			Vector3 vector8 = vector6 + this.leftHold.transform.up * this.oneHandSimulatedHoldOffset.x;
			if (this.rightHoldPositionLocal != null)
			{
				this.rightHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector8), this.rightHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector7 = GTPlayer.Instance.transform.TransformPoint(this.rightHoldPositionLocal.Value);
			}
			else
			{
				vector7 = vector8;
				this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector7));
			}
		}
		else
		{
			vector7 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector7));
			Vector3 vector9 = vector7 + this.rightHold.transform.up * this.oneHandSimulatedHoldOffset.x;
			if (this.leftHoldPositionLocal != null)
			{
				this.leftHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector9), this.leftHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector6 = GTPlayer.Instance.transform.TransformPoint(this.leftHoldPositionLocal.Value);
			}
			else
			{
				vector6 = vector9;
				this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector6));
			}
		}
		Vector3 vector10;
		Vector3 vector11;
		this.GetHandsOrientationVectors(vector6, vector7, GTPlayer.Instance.headCollider.transform, false, out vector10, out vector11);
		float num9 = this.riderPosition.y * this.riderPosDirectPitchMax;
		if (!this.leftHold.active || !this.rightHold.active)
		{
			num9 *= this.oneHandPitchMultiplier;
		}
		Spring.CriticalSpringDamperExact(ref this.pitch, ref this.pitchVel, num9, 0f, this.pitchHalfLife, dt);
		this.pitch = Mathf.Clamp(this.pitch, this.pitchMinMax.x, this.pitchMinMax.y);
		Quaternion quaternion = Quaternion.AngleAxis(this.pitch, Vector3.right);
		this.twoHandRotationOffsetAngle = Mathf.Lerp(0f, this.twoHandRotationOffsetAngle, Mathf.Exp(-8f * dt));
		Vector3 vector12 = (this.twoHandGliderInversionOnYawInsteadOfRoll ? vector11 : Vector3.up);
		Quaternion quaternion2 = Quaternion.AngleAxis(this.twoHandRotationOffsetAngle, this.twoHandRotationOffsetAxis) * Quaternion.LookRotation(vector10, vector12) * Quaternion.AngleAxis(-90f, Vector3.up);
		float num10 = ((this.leftHold.active && this.rightHold.active) ? this.twoHandRotationRateExp : this.oneHandRotationRateExp);
		base.transform.rotation = Quaternion.Slerp(quaternion2 * quaternion, base.transform.rotation, Mathf.Exp(-num10 * dt));
		if (this.subtlePlayerPitchActive || this.subtlePlayerRollActive)
		{
			float num11 = Mathf.InverseLerp(this.subtlePlayerRotationSpeedRampMinMax.x, this.subtlePlayerRotationSpeedRampMinMax.y, this.currentVelocity.magnitude);
			Quaternion quaternion3 = Quaternion.identity;
			if (this.subtlePlayerRollActive)
			{
				float num12 = this.GetRollAngle180Wrapping();
				if (num12 > 90f)
				{
					num12 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num12));
				}
				else if (num12 < -90f)
				{
					num12 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num12));
				}
				Vector3 normalized3 = new Vector3(this.currentVelocity.x, 0f, this.currentVelocity.z).normalized;
				Vector3 vector13 = new Vector3(average.x, 0f, average.z);
				float num13 = Vector3.Dot(vector13 - Vector3.Dot(vector13, normalized3) * normalized3, Vector3.Cross(normalized3, Vector3.up));
				this.turnAccelerationSmoothed = Mathf.Lerp(num13, this.turnAccelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
				float num14 = 0f;
				if (num13 * num12 > 0f)
				{
					num14 = Mathf.InverseLerp(this.subtlePlayerRollAccelMinMax.x, this.subtlePlayerRollAccelMinMax.y, Mathf.Abs(this.turnAccelerationSmoothed));
				}
				float num15 = num12 * this.subtlePlayerRollFactor * Mathf.Min(num11, num14);
				this.subtlePlayerRoll = Mathf.Lerp(num15, this.subtlePlayerRoll, Mathf.Exp(-this.subtlePlayerRollRateExp * dt));
				quaternion3 = Quaternion.AngleAxis(this.subtlePlayerRoll, base.transform.forward);
			}
			Quaternion quaternion4 = Quaternion.identity;
			if (this.subtlePlayerPitchActive)
			{
				float num16 = this.pitch * this.subtlePlayerPitchFactor * Mathf.Min(num11, 1f);
				this.subtlePlayerPitch = Mathf.Lerp(num16, this.subtlePlayerPitch, Mathf.Exp(-this.subtlePlayerPitchRateExp * dt));
				quaternion4 = Quaternion.AngleAxis(this.subtlePlayerPitch, -base.transform.right);
			}
			GTPlayer.Instance.PlayerRotationOverride = quaternion4 * quaternion3;
		}
		this.UpdateGliderPosition();
		if (this.syncedState.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = (this.syncedState.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		this.syncedState.tagged = this.infectedState;
		this.syncedState.materialIndex = (byte)this.ridersMaterialOverideIndex;
		if (this.cachedRig != null)
		{
			this.syncedState.position = this.cachedRig.transform.InverseTransformPoint(base.transform.position);
			this.syncedState.rotation = Quaternion.Inverse(this.cachedRig.transform.rotation) * base.transform.rotation;
		}
		else
		{
			Debug.LogError("Glider failed to get a reference to the local player's VRRig while the player was flying", this);
		}
		this.audioLevel = num2 * num;
		if (this.OutOfBounds)
		{
			this.Respawn();
		}
		if (this.leftHold.active && EquipmentInteractor.instance.leftHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.leftHand);
		}
		if (this.rightHold.active && EquipmentInteractor.instance.rightHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.rightHand);
		}
	}

	// Token: 0x060037B6 RID: 14262 RVA: 0x0010E2B4 File Offset: 0x0010C4B4
	private void AuthorityUpdateUnheld(float dt)
	{
		this.syncedState.position = base.transform.position;
		this.syncedState.rotation = base.transform.rotation;
		if (this.gliderState != GliderHoldable.GliderState.LocallyDropped)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.tagged = false;
			this.leafMesh.material = this.baseLeafMaterial;
		}
		if (this.audioLevel * this.audioVolumeMultiplier > 0.001f)
		{
			this.audioLevel = Mathf.Lerp(0f, this.audioLevel, Mathf.Exp(-2f * dt));
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.audioVolumeMultiplier);
		}
		if (this.OutOfBounds || (this.lastHeldTime > 0f && this.lastHeldTime < Time.time - this.maxDroppedTimeToRespawn))
		{
			this.Respawn();
		}
	}

	// Token: 0x060037B7 RID: 14263 RVA: 0x0010E3E8 File Offset: 0x0010C5E8
	private void RemoteSyncUpdate(float dt)
	{
		this.rb.isKinematic = true;
		int num = this.syncedState.riderId;
		bool flag = this.riderId != num;
		if (flag)
		{
			this.riderId = num;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		if (this.riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.cachedRig = null;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.audioLevel = 0;
		}
		if (this.syncedState.riderId == -1)
		{
			base.transform.position = Vector3.Lerp(this.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.rotation = Quaternion.Slerp(this.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
		}
		else if (this.cachedRig != null)
		{
			this.positionLocalToVRRig = Vector3.Lerp(this.syncedState.position, this.positionLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			this.rotationLocalToVRRig = Quaternion.Slerp(this.syncedState.rotation, this.rotationLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.position = this.cachedRig.transform.TransformPoint(this.positionLocalToVRRig);
			base.transform.rotation = this.cachedRig.transform.rotation * this.rotationLocalToVRRig;
		}
		bool flag2 = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			flag2 = this.syncedState.tagged;
		}
		bool flag3 = flag2 != this.infectedState;
		this.infectedState = flag2;
		if (flag3 || flag)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		float num2 = Mathf.Clamp01((float)this.syncedState.audioLevel / 255f);
		if (this.audioLevel != num2)
		{
			this.audioLevel = num2;
			if (this.syncedState.riderId != -1 && this.syncedState.tagged)
			{
				this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				return;
			}
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
	}

	// Token: 0x060037B8 RID: 14264 RVA: 0x0010E6EC File Offset: 0x0010C8EC
	private VRRig getNewHolderRig(int riderId)
	{
		if (riderId >= 0)
		{
			NetPlayer netPlayer;
			if (riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				netPlayer = NetworkSystem.Instance.LocalPlayer;
			}
			else
			{
				netPlayer = NetworkSystem.Instance.GetPlayer(riderId);
			}
			RigContainer rigContainer;
			if (netPlayer != null && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				return rigContainer.Rig;
			}
		}
		return null;
	}

	// Token: 0x060037B9 RID: 14265 RVA: 0x0010E744 File Offset: 0x0010C944
	private Vector3 ClosestPointInHandle(Vector3 startingPoint, InteractionPoint interactionPoint)
	{
		CapsuleCollider component = interactionPoint.GetComponent<CapsuleCollider>();
		Vector3 vector = startingPoint;
		if (component != null)
		{
			Vector3 vector2 = ((component.direction == 0) ? Vector3.right : ((component.direction == 1) ? Vector3.up : Vector3.forward));
			Vector3 vector3 = component.transform.rotation * vector2;
			Vector3 vector4 = component.transform.position + component.transform.rotation * component.center;
			float num = Mathf.Clamp(Vector3.Dot(vector - vector4, vector3), -component.height * 0.5f, component.height * 0.5f);
			vector = vector4 + vector3 * num;
		}
		return vector;
	}

	// Token: 0x060037BA RID: 14266 RVA: 0x0010E804 File Offset: 0x0010CA04
	private void UpdateGliderPosition()
	{
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 vector = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			Vector3 vector2 = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
			base.transform.position = (vector + vector2) * 0.5f;
			return;
		}
		if (this.leftHold.active)
		{
			base.transform.position = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			return;
		}
		if (this.rightHold.active)
		{
			base.transform.position = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
		}
	}

	// Token: 0x060037BB RID: 14267 RVA: 0x0010E95C File Offset: 0x0010CB5C
	private Vector3 GetHandsVector(Vector3 leftHandPos, Vector3 rightHandPos, Vector3 headPos, bool flipBasedOnFacingDir)
	{
		Vector3 vector = rightHandPos - leftHandPos;
		Vector3 vector2 = (rightHandPos + leftHandPos) * 0.5f - headPos;
		Vector3 normalized = Vector3.Cross(Vector3.up, vector2).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(vector, normalized) < 0f)
		{
			vector = -vector;
		}
		return vector;
	}

	// Token: 0x060037BC RID: 14268 RVA: 0x0010E9B8 File Offset: 0x0010CBB8
	private void GetHandsOrientationVectors(Vector3 leftHandPos, Vector3 rightHandPos, Transform head, bool flipBasedOnFacingDir, out Vector3 handsVector, out Vector3 handsUpVector)
	{
		handsVector = rightHandPos - leftHandPos;
		float magnitude = handsVector.magnitude;
		handsVector /= Mathf.Max(magnitude, 0.001f);
		Vector3 position = head.position;
		float num = 1f;
		Vector3 vector = ((Vector3.Dot(head.right, handsVector) < 0f) ? handsVector : (-handsVector));
		Vector3 normalized = Vector3.ProjectOnPlane(-head.forward, vector).normalized;
		Vector3 vector2 = normalized * num + position;
		Vector3 vector3 = (leftHandPos + rightHandPos) * 0.5f;
		Vector3 vector4 = Vector3.ProjectOnPlane(vector3 - head.position, Vector3.up);
		float magnitude2 = vector4.magnitude;
		vector4 /= Mathf.Max(magnitude2, 0.001f);
		Vector3 normalized2 = Vector3.ProjectOnPlane(-base.transform.forward, Vector3.up).normalized;
		Vector3 vector5 = -vector4 * num + position;
		float num2 = Vector3.Dot(normalized2, -vector4);
		float num3 = Vector3.Dot(normalized2, normalized);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0f)
		{
			num2 = Mathf.Abs(num2);
			num3 = Mathf.Abs(num3);
		}
		num2 = Mathf.Max(num2, 0f);
		num3 = Mathf.Max(num3, 0f);
		Vector3 vector6 = (vector5 * num2 + vector2 * num3) / Mathf.Max(num2 + num3, 0.001f);
		Vector3 vector7 = vector3 - vector6;
		Vector3 normalized3 = Vector3.Cross(Vector3.up, vector7).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(handsVector, normalized3) < 0f)
		{
			handsVector = -handsVector;
		}
		handsUpVector = Vector3.Cross(Vector3.ProjectOnPlane(vector7, Vector3.up), handsVector).normalized;
	}

	// Token: 0x060037BD RID: 14269 RVA: 0x0010EBE3 File Offset: 0x0010CDE3
	private Material GetMaterialFromIndex(byte materialIndex)
	{
		if (materialIndex < 1 || (int)materialIndex > this.cosmeticMaterialOverrides.Length)
		{
			return this.baseLeafMaterial;
		}
		return this.cosmeticMaterialOverrides[(int)(materialIndex - 1)].material;
	}

	// Token: 0x060037BE RID: 14270 RVA: 0x0010EC10 File Offset: 0x0010CE10
	private float GetRollAngle180Wrapping()
	{
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float num = Vector3.SignedAngle(Vector3.Cross(Vector3.up, normalized).normalized, base.transform.right, base.transform.forward);
		return this.NormalizeAngle180(num);
	}

	// Token: 0x060037BF RID: 14271 RVA: 0x0010EC71 File Offset: 0x0010CE71
	private float SignedAngleInPlane(Vector3 from, Vector3 to, Vector3 normal)
	{
		from = Vector3.ProjectOnPlane(from, normal);
		to = Vector3.ProjectOnPlane(to, normal);
		return Vector3.SignedAngle(from, to, normal);
	}

	// Token: 0x060037C0 RID: 14272 RVA: 0x0010EC8D File Offset: 0x0010CE8D
	private float NormalizeAngle180(float angle)
	{
		angle = (angle + 180f) % 360f;
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle - 180f;
	}

	// Token: 0x060037C1 RID: 14273 RVA: 0x0010ECB8 File Offset: 0x0010CEB8
	private void UpdateAudioSource(AudioSource source, float level)
	{
		source.volume = level;
		if (!source.isPlaying && level > 0.01f)
		{
			source.GTPlay();
			return;
		}
		if (source.isPlaying && level < 0.01f && this.syncedState.riderId == -1)
		{
			source.GTStop();
		}
	}

	// Token: 0x060037C2 RID: 14274 RVA: 0x0010ED07 File Offset: 0x0010CF07
	private Material GetInfectedMaterial()
	{
		if (GorillaGameManager.instance is GorillaFreezeTagManager)
		{
			return this.frozenLeafMaterial;
		}
		return this.infectedLeafMaterial;
	}

	// Token: 0x060037C3 RID: 14275 RVA: 0x0010ED24 File Offset: 0x0010CF24
	public void OnTriggerStay(Collider other)
	{
		GliderWindVolume component = other.GetComponent<GliderWindVolume>();
		if (component == null)
		{
			return;
		}
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		if (Time.frameCount == this.windVolumeForceAppliedFrame)
		{
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			Vector3 accelFromVelocity = component.GetAccelFromVelocity(GTPlayer.Instance.RigidbodyVelocity);
			GTPlayer.Instance.AddForce(accelFromVelocity, ForceMode.Acceleration);
			this.windVolumeForceAppliedFrame = Time.frameCount;
			return;
		}
		Vector3 accelFromVelocity2 = component.GetAccelFromVelocity(this.rb.velocity);
		Vector3 vector = this.WindResistanceForceOffset(base.transform.up, component.WindDirection);
		Vector3 vector2 = base.transform.position + vector * this.windUprightTorqueMultiplier;
		this.rb.AddForceAtPosition(accelFromVelocity2, vector2, ForceMode.Acceleration);
		this.windVolumeForceAppliedFrame = Time.frameCount;
	}

	// Token: 0x060037C4 RID: 14276 RVA: 0x0010EE12 File Offset: 0x0010D012
	private Vector3 WindResistanceForceOffset(Vector3 upDir, Vector3 windDir)
	{
		if (Vector3.Dot(upDir, windDir) < 0f)
		{
			upDir *= -1f;
		}
		return Vector3.ProjectOnPlane(upDir - windDir, upDir);
	}

	// Token: 0x17000581 RID: 1409
	// (get) Token: 0x060037C5 RID: 14277 RVA: 0x0010EE3C File Offset: 0x0010D03C
	// (set) Token: 0x060037C6 RID: 14278 RVA: 0x0010EE66 File Offset: 0x0010D066
	[Networked]
	[NetworkedWeaved(0, 11)]
	internal unsafe GliderHoldable.SyncedState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GliderHoldable.SyncedState*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GliderHoldable.SyncedState*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060037C7 RID: 14279 RVA: 0x0010EE94 File Offset: 0x0010D094
	public override void ReadDataFusion()
	{
		int num = this.syncedState.riderId;
		this.syncedState = this.Data;
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x060037C8 RID: 14280 RVA: 0x0010EEE7 File Offset: 0x0010D0E7
	public override void WriteDataFusion()
	{
		this.Data = this.syncedState;
	}

	// Token: 0x060037C9 RID: 14281 RVA: 0x0010EEF8 File Offset: 0x0010D0F8
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		PunNetPlayer punNetPlayer = (PunNetPlayer)this.ownershipGuard.actualOwner;
		if (sender != ((punNetPlayer != null) ? punNetPlayer.PlayerRef : null))
		{
			return;
		}
		int num = this.syncedState.riderId;
		this.syncedState.riderId = (int)stream.ReceiveNext();
		this.syncedState.tagged = (bool)stream.ReceiveNext();
		this.syncedState.materialIndex = (byte)stream.ReceiveNext();
		this.syncedState.audioLevel = (byte)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.syncedState.position).SetValueSafe(in vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		(ref this.syncedState.rotation).SetValueSafe(in quaternion);
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x060037CA RID: 14282 RVA: 0x0010F000 File Offset: 0x0010D200
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		object sender = info.Sender;
		NetPlayer actualOwner = this.ownershipGuard.actualOwner;
		if (!sender.Equals((actualOwner != null) ? actualOwner.GetPlayerRef() : null))
		{
			return;
		}
		stream.SendNext(this.syncedState.riderId);
		stream.SendNext(this.syncedState.tagged);
		stream.SendNext(this.syncedState.materialIndex);
		stream.SendNext(this.syncedState.audioLevel);
		stream.SendNext(this.syncedState.position);
		stream.SendNext(this.syncedState.rotation);
	}

	// Token: 0x060037CB RID: 14283 RVA: 0x0010F0BB File Offset: 0x0010D2BB
	private IEnumerator ReenableOwnershipRequest()
	{
		yield return new WaitForSeconds(3f);
		this.pendingOwnershipRequest = false;
		yield break;
	}

	// Token: 0x060037CC RID: 14284 RVA: 0x0010F0CC File Offset: 0x0010D2CC
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			this.pendingOwnershipRequest = false;
			if (!this.leftHold.active && !this.rightHold.active && (this.spawnPosition - base.transform.position).sqrMagnitude > 1f)
			{
				this.rb.isKinematic = false;
				this.rb.WakeUp();
				this.lastHeldTime = Time.time;
			}
		}
	}

	// Token: 0x060037CD RID: 14285 RVA: 0x0010F14E File Offset: 0x0010D34E
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return !base.IsMine || !NetworkSystem.Instance.InRoom || (!this.leftHold.active && !this.rightHold.active);
	}

	// Token: 0x060037CE RID: 14286 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x060037CF RID: 14287 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x060037D0 RID: 14288 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x060037D4 RID: 14292 RVA: 0x0010F627 File Offset: 0x0010D827
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060037D5 RID: 14293 RVA: 0x0010F63F File Offset: 0x0010D83F
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04003D27 RID: 15655
	[Header("Flight Settings")]
	[SerializeField]
	private Vector2 pitchMinMax = new Vector2(-80f, 80f);

	// Token: 0x04003D28 RID: 15656
	[SerializeField]
	private Vector2 rollMinMax = new Vector2(-70f, 70f);

	// Token: 0x04003D29 RID: 15657
	[SerializeField]
	private float pitchHalfLife = 0.2f;

	// Token: 0x04003D2A RID: 15658
	public Vector2 pitchVelocityTargetMinMax = new Vector2(-60f, 60f);

	// Token: 0x04003D2B RID: 15659
	public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-1f, 1f);

	// Token: 0x04003D2C RID: 15660
	[SerializeField]
	private float pitchVelocityFollowRateAngle = 60f;

	// Token: 0x04003D2D RID: 15661
	[SerializeField]
	private float pitchVelocityFollowRateMagnitude = 5f;

	// Token: 0x04003D2E RID: 15662
	[SerializeField]
	private AnimationCurve liftVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003D2F RID: 15663
	[SerializeField]
	private AnimationCurve dragVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003D30 RID: 15664
	[SerializeField]
	[Range(0f, 1f)]
	public float attackDragFactor = 0.1f;

	// Token: 0x04003D31 RID: 15665
	[SerializeField]
	private AnimationCurve dragVsSpeed = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003D32 RID: 15666
	[SerializeField]
	public float dragVsSpeedMaxSpeed = 30f;

	// Token: 0x04003D33 RID: 15667
	[SerializeField]
	[Range(0f, 1f)]
	public float dragVsSpeedDragFactor = 0.2f;

	// Token: 0x04003D34 RID: 15668
	[SerializeField]
	private AnimationCurve liftIncreaseVsRoll = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003D35 RID: 15669
	[SerializeField]
	private float liftIncreaseVsRollMaxAngle = 20f;

	// Token: 0x04003D36 RID: 15670
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityCompensation = 0.8f;

	// Token: 0x04003D37 RID: 15671
	[Range(0f, 1f)]
	public float pullUpLiftBonus = 0.1f;

	// Token: 0x04003D38 RID: 15672
	public float pullUpLiftActivationVelocity = 1f;

	// Token: 0x04003D39 RID: 15673
	public float pullUpLiftActivationAcceleration = 3f;

	// Token: 0x04003D3A RID: 15674
	[Header("Body Positioning Control")]
	[SerializeField]
	private float riderPosDirectPitchMax = 70f;

	// Token: 0x04003D3B RID: 15675
	[SerializeField]
	private Vector2 riderPosRange = new Vector2(2.2f, 0.75f);

	// Token: 0x04003D3C RID: 15676
	[SerializeField]
	private float riderPosRangeOffset = 0.15f;

	// Token: 0x04003D3D RID: 15677
	[SerializeField]
	private Vector2 riderPosRangeNormalizedDeadzone = new Vector2(0.15f, 0.05f);

	// Token: 0x04003D3E RID: 15678
	[Header("Direct Handle Control")]
	[SerializeField]
	private float oneHandHoldRotationRate = 2f;

	// Token: 0x04003D3F RID: 15679
	private Vector3 oneHandSimulatedHoldOffset = new Vector3(0.5f, -0.35f, 0.25f);

	// Token: 0x04003D40 RID: 15680
	private float oneHandPitchMultiplier = 0.8f;

	// Token: 0x04003D41 RID: 15681
	[SerializeField]
	private float twoHandHoldRotationRate = 4f;

	// Token: 0x04003D42 RID: 15682
	[SerializeField]
	private bool twoHandGliderInversionOnYawInsteadOfRoll;

	// Token: 0x04003D43 RID: 15683
	[Header("Player Settings")]
	[SerializeField]
	private bool setMaxHandSlipDuringFlight = true;

	// Token: 0x04003D44 RID: 15684
	[SerializeField]
	private float maxSlipOverrideSpeedThreshold = 5f;

	// Token: 0x04003D45 RID: 15685
	[Header("Player Camera Rotation")]
	[SerializeField]
	private float subtlePlayerPitchFactor = 0.2f;

	// Token: 0x04003D46 RID: 15686
	[SerializeField]
	private float subtlePlayerPitchRate = 2f;

	// Token: 0x04003D47 RID: 15687
	[SerializeField]
	private float subtlePlayerRollFactor = 0.2f;

	// Token: 0x04003D48 RID: 15688
	[SerializeField]
	private float subtlePlayerRollRate = 2f;

	// Token: 0x04003D49 RID: 15689
	[SerializeField]
	private Vector2 subtlePlayerRotationSpeedRampMinMax = new Vector2(2f, 8f);

	// Token: 0x04003D4A RID: 15690
	[SerializeField]
	private Vector2 subtlePlayerRollAccelMinMax = new Vector2(0f, 30f);

	// Token: 0x04003D4B RID: 15691
	[SerializeField]
	private Vector2 subtlePlayerPitchAccelMinMax = new Vector2(0f, 10f);

	// Token: 0x04003D4C RID: 15692
	[SerializeField]
	private float accelSmoothingFollowRate = 2f;

	// Token: 0x04003D4D RID: 15693
	[Header("Haptics")]
	[SerializeField]
	private Vector2 hapticAccelInputRange = new Vector2(5f, 20f);

	// Token: 0x04003D4E RID: 15694
	[SerializeField]
	private float hapticAccelOutputMax = 0.35f;

	// Token: 0x04003D4F RID: 15695
	[SerializeField]
	private Vector2 hapticMaxSpeedInputRange = new Vector2(5f, 10f);

	// Token: 0x04003D50 RID: 15696
	[SerializeField]
	private Vector2 hapticSpeedInputRange = new Vector2(3f, 30f);

	// Token: 0x04003D51 RID: 15697
	[SerializeField]
	private float hapticSpeedOutputMax = 0.15f;

	// Token: 0x04003D52 RID: 15698
	[SerializeField]
	private Vector2 whistlingAudioSpeedInputRange = new Vector2(15f, 30f);

	// Token: 0x04003D53 RID: 15699
	[Header("Audio")]
	[SerializeField]
	private float audioVolumeMultiplier = 0.25f;

	// Token: 0x04003D54 RID: 15700
	[SerializeField]
	private float infectedAudioVolumeMultiplier = 0.5f;

	// Token: 0x04003D55 RID: 15701
	[SerializeField]
	private Vector2 whooshSpeedThresholdInput = new Vector2(10f, 25f);

	// Token: 0x04003D56 RID: 15702
	[SerializeField]
	private Vector2 whooshVolumeOutput = new Vector2(0.2f, 0.75f);

	// Token: 0x04003D57 RID: 15703
	[SerializeField]
	private float whooshCheckDistance = 2f;

	// Token: 0x04003D58 RID: 15704
	[Header("Tag Adjustment")]
	[SerializeField]
	private bool extendTagRangeInFlight = true;

	// Token: 0x04003D59 RID: 15705
	[SerializeField]
	private Vector2 tagRangeSpeedInput = new Vector2(5f, 20f);

	// Token: 0x04003D5A RID: 15706
	[SerializeField]
	private Vector2 tagRangeOutput = new Vector2(0.03f, 3f);

	// Token: 0x04003D5B RID: 15707
	[SerializeField]
	private bool debugDrawTagRange = true;

	// Token: 0x04003D5C RID: 15708
	[Header("Infected State")]
	[SerializeField]
	private float infectedSpeedIncrease = 5f;

	// Token: 0x04003D5D RID: 15709
	[Header("Glider Materials")]
	[SerializeField]
	private MeshRenderer leafMesh;

	// Token: 0x04003D5E RID: 15710
	[SerializeField]
	private Material baseLeafMaterial;

	// Token: 0x04003D5F RID: 15711
	[SerializeField]
	private Material infectedLeafMaterial;

	// Token: 0x04003D60 RID: 15712
	[SerializeField]
	private Material frozenLeafMaterial;

	// Token: 0x04003D61 RID: 15713
	[SerializeField]
	private GliderHoldable.CosmeticMaterialOverride[] cosmeticMaterialOverrides;

	// Token: 0x04003D62 RID: 15714
	[Header("Network Syncing")]
	[SerializeField]
	private float networkSyncFollowRate = 2f;

	// Token: 0x04003D63 RID: 15715
	[Header("Life Cycle")]
	[SerializeField]
	private Transform maxDistanceRespawnOrigin;

	// Token: 0x04003D64 RID: 15716
	[SerializeField]
	private float maxDistanceBeforeRespawn = 180f;

	// Token: 0x04003D65 RID: 15717
	[SerializeField]
	private float maxDroppedTimeToRespawn = 120f;

	// Token: 0x04003D66 RID: 15718
	[Header("Rigidbody")]
	[SerializeField]
	private float windUprightTorqueMultiplier = 1f;

	// Token: 0x04003D67 RID: 15719
	[SerializeField]
	private float gravityUprightTorqueMultiplier = 0.5f;

	// Token: 0x04003D68 RID: 15720
	[SerializeField]
	private float fallingGravityReduction = 0.1f;

	// Token: 0x04003D69 RID: 15721
	[Header("References")]
	[SerializeField]
	private AudioSource calmAudio;

	// Token: 0x04003D6A RID: 15722
	[SerializeField]
	private AudioSource activeAudio;

	// Token: 0x04003D6B RID: 15723
	[SerializeField]
	private AudioSource whistlingAudio;

	// Token: 0x04003D6C RID: 15724
	[SerializeField]
	private AudioSource leftWhooshAudio;

	// Token: 0x04003D6D RID: 15725
	[SerializeField]
	private AudioSource rightWhooshAudio;

	// Token: 0x04003D6E RID: 15726
	[SerializeField]
	private InteractionPoint handle;

	// Token: 0x04003D6F RID: 15727
	[SerializeField]
	private RequestableOwnershipGuard ownershipGuard;

	// Token: 0x04003D70 RID: 15728
	private bool subtlePlayerPitchActive = true;

	// Token: 0x04003D71 RID: 15729
	private bool subtlePlayerRollActive = true;

	// Token: 0x04003D72 RID: 15730
	private float subtlePlayerPitch;

	// Token: 0x04003D73 RID: 15731
	private float subtlePlayerRoll;

	// Token: 0x04003D74 RID: 15732
	private float subtlePlayerPitchRateExp = 0.75f;

	// Token: 0x04003D75 RID: 15733
	private float subtlePlayerRollRateExp = 0.025f;

	// Token: 0x04003D76 RID: 15734
	private float defaultMaxDistanceBeforeRespawn = 180f;

	// Token: 0x04003D77 RID: 15735
	private GliderHoldable.HoldingHand leftHold;

	// Token: 0x04003D78 RID: 15736
	private GliderHoldable.HoldingHand rightHold;

	// Token: 0x04003D79 RID: 15737
	private GliderHoldable.SyncedState syncedState;

	// Token: 0x04003D7A RID: 15738
	private Vector3 twoHandRotationOffsetAxis = Vector3.forward;

	// Token: 0x04003D7B RID: 15739
	private float twoHandRotationOffsetAngle;

	// Token: 0x04003D7C RID: 15740
	private Rigidbody rb;

	// Token: 0x04003D7D RID: 15741
	private Vector2 riderPosition = Vector2.zero;

	// Token: 0x04003D7E RID: 15742
	private Vector3 previousVelocity;

	// Token: 0x04003D7F RID: 15743
	private Vector3 currentVelocity;

	// Token: 0x04003D80 RID: 15744
	private float pitch;

	// Token: 0x04003D81 RID: 15745
	private float yaw;

	// Token: 0x04003D82 RID: 15746
	private float roll;

	// Token: 0x04003D83 RID: 15747
	private float pitchVel;

	// Token: 0x04003D84 RID: 15748
	private float yawVel;

	// Token: 0x04003D85 RID: 15749
	private float rollVel;

	// Token: 0x04003D86 RID: 15750
	private float oneHandRotationRateExp;

	// Token: 0x04003D87 RID: 15751
	private float twoHandRotationRateExp;

	// Token: 0x04003D88 RID: 15752
	private Quaternion playerFacingRotationOffset = Quaternion.identity;

	// Token: 0x04003D89 RID: 15753
	private const float accelAveragingWindow = 0.1f;

	// Token: 0x04003D8A RID: 15754
	private AverageVector3 accelerationAverage = new AverageVector3(0.1f);

	// Token: 0x04003D8B RID: 15755
	private float accelerationSmoothed;

	// Token: 0x04003D8C RID: 15756
	private float turnAccelerationSmoothed;

	// Token: 0x04003D8D RID: 15757
	private float accelSmoothingFollowRateExp = 1f;

	// Token: 0x04003D8E RID: 15758
	private float networkSyncFollowRateExp = 2f;

	// Token: 0x04003D8F RID: 15759
	private bool pendingOwnershipRequest;

	// Token: 0x04003D90 RID: 15760
	private Vector3 positionLocalToVRRig = Vector3.zero;

	// Token: 0x04003D91 RID: 15761
	private Quaternion rotationLocalToVRRig = Quaternion.identity;

	// Token: 0x04003D92 RID: 15762
	private Coroutine reenableOwnershipRequestCoroutine;

	// Token: 0x04003D93 RID: 15763
	private Vector3 spawnPosition;

	// Token: 0x04003D94 RID: 15764
	private Quaternion spawnRotation;

	// Token: 0x04003D95 RID: 15765
	private Vector3 skyJungleSpawnPostion;

	// Token: 0x04003D96 RID: 15766
	private Quaternion skyJungleSpawnRotation;

	// Token: 0x04003D97 RID: 15767
	private Transform skyJungleRespawnOrigin;

	// Token: 0x04003D98 RID: 15768
	private float lastHeldTime = -1f;

	// Token: 0x04003D99 RID: 15769
	private Vector3? leftHoldPositionLocal;

	// Token: 0x04003D9A RID: 15770
	private Vector3? rightHoldPositionLocal;

	// Token: 0x04003D9B RID: 15771
	private float whooshSoundDuration = 1f;

	// Token: 0x04003D9C RID: 15772
	private float whooshSoundRetriggerThreshold = 0.5f;

	// Token: 0x04003D9D RID: 15773
	private float leftWhooshStartTime = -1f;

	// Token: 0x04003D9E RID: 15774
	private Vector3 leftWhooshHitPoint = Vector3.zero;

	// Token: 0x04003D9F RID: 15775
	private Vector3 whooshAudioPositionOffset = new Vector3(0.5f, -0.25f, 0.5f);

	// Token: 0x04003DA0 RID: 15776
	private float rightWhooshStartTime = -1f;

	// Token: 0x04003DA1 RID: 15777
	private Vector3 rightWhooshHitPoint = Vector3.zero;

	// Token: 0x04003DA2 RID: 15778
	private int ridersMaterialOverideIndex;

	// Token: 0x04003DA3 RID: 15779
	private int windVolumeForceAppliedFrame = -1;

	// Token: 0x04003DA4 RID: 15780
	private bool holdingTwoGliders;

	// Token: 0x04003DA5 RID: 15781
	private GliderHoldable.GliderState gliderState;

	// Token: 0x04003DA6 RID: 15782
	private float audioLevel;

	// Token: 0x04003DA7 RID: 15783
	private int riderId = -1;

	// Token: 0x04003DA8 RID: 15784
	[SerializeField]
	private VRRig cachedRig;

	// Token: 0x04003DA9 RID: 15785
	private bool infectedState;

	// Token: 0x04003DAA RID: 15786
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 11)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GliderHoldable.SyncedState _Data;

	// Token: 0x020008F8 RID: 2296
	private enum GliderState
	{
		// Token: 0x04003DAC RID: 15788
		LocallyHeld,
		// Token: 0x04003DAD RID: 15789
		LocallyDropped,
		// Token: 0x04003DAE RID: 15790
		RemoteSyncing
	}

	// Token: 0x020008F9 RID: 2297
	private struct HoldingHand
	{
		// Token: 0x060037D6 RID: 14294 RVA: 0x0010F654 File Offset: 0x0010D854
		public void Activate(Transform handTransform, Transform gliderTransform, Vector3 worldGrabPoint)
		{
			this.active = true;
			this.transform = handTransform.transform;
			this.holdLocalPos = handTransform.InverseTransformPoint(worldGrabPoint);
			this.handleLocalPos = gliderTransform.InverseTransformVector(gliderTransform.position - worldGrabPoint);
			this.localHoldRotation = Quaternion.Inverse(handTransform.rotation) * gliderTransform.rotation;
		}

		// Token: 0x060037D7 RID: 14295 RVA: 0x0010F6B5 File Offset: 0x0010D8B5
		public void Deactivate()
		{
			this.active = false;
			this.transform = null;
			this.holdLocalPos = Vector3.zero;
			this.handleLocalPos = Vector3.zero;
			this.localHoldRotation = Quaternion.identity;
		}

		// Token: 0x04003DAF RID: 15791
		public bool active;

		// Token: 0x04003DB0 RID: 15792
		public Transform transform;

		// Token: 0x04003DB1 RID: 15793
		public Vector3 holdLocalPos;

		// Token: 0x04003DB2 RID: 15794
		public Vector3 handleLocalPos;

		// Token: 0x04003DB3 RID: 15795
		public Quaternion localHoldRotation;
	}

	// Token: 0x020008FA RID: 2298
	[NetworkStructWeaved(11)]
	[StructLayout(LayoutKind.Explicit, Size = 44)]
	internal struct SyncedState : INetworkStruct
	{
		// Token: 0x060037D8 RID: 14296 RVA: 0x0010F6E6 File Offset: 0x0010D8E6
		public void Init(Vector3 defaultPosition, Quaternion defaultRotation)
		{
			this.riderId = -1;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.position = defaultPosition;
			this.rotation = defaultRotation;
		}

		// Token: 0x060037D9 RID: 14297 RVA: 0x0010F70B File Offset: 0x0010D90B
		public SyncedState(int id = -1)
		{
			this.riderId = id;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.tagged = default(NetworkBool);
			this.position = default(Vector3);
			this.rotation = default(Quaternion);
		}

		// Token: 0x04003DB4 RID: 15796
		[FieldOffset(0)]
		public int riderId;

		// Token: 0x04003DB5 RID: 15797
		[FieldOffset(4)]
		public byte materialIndex;

		// Token: 0x04003DB6 RID: 15798
		[FieldOffset(8)]
		public byte audioLevel;

		// Token: 0x04003DB7 RID: 15799
		[FieldOffset(12)]
		public NetworkBool tagged;

		// Token: 0x04003DB8 RID: 15800
		[FieldOffset(16)]
		public Vector3 position;

		// Token: 0x04003DB9 RID: 15801
		[FieldOffset(28)]
		public Quaternion rotation;
	}

	// Token: 0x020008FB RID: 2299
	[Serializable]
	private struct CosmeticMaterialOverride
	{
		// Token: 0x04003DBA RID: 15802
		public string cosmeticName;

		// Token: 0x04003DBB RID: 15803
		public Material material;
	}
}
