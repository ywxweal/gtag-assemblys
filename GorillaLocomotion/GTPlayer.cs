using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AA;
using BoingKit;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion
{
	// Token: 0x02000CC1 RID: 3265
	public class GTPlayer : MonoBehaviour
	{
		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x060050AD RID: 20653 RVA: 0x00180E26 File Offset: 0x0017F026
		public static GTPlayer Instance
		{
			get
			{
				return GTPlayer._instance;
			}
		}

		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x060050AE RID: 20654 RVA: 0x00180E2D File Offset: 0x0017F02D
		public Vector3 InstantaneousVelocity
		{
			get
			{
				return this.currentVelocity;
			}
		}

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x060050AF RID: 20655 RVA: 0x00180E35 File Offset: 0x0017F035
		public Vector3 AveragedVelocity
		{
			get
			{
				return this.averagedVelocity;
			}
		}

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x060050B0 RID: 20656 RVA: 0x00180E3D File Offset: 0x0017F03D
		public Transform CosmeticsHeadTarget
		{
			get
			{
				return this.cosmeticsHeadTarget;
			}
		}

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x060050B1 RID: 20657 RVA: 0x00180E45 File Offset: 0x0017F045
		public float scale
		{
			get
			{
				return this.scaleMultiplier * this.nativeScale;
			}
		}

		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x060050B2 RID: 20658 RVA: 0x00180E54 File Offset: 0x0017F054
		public float NativeScale
		{
			get
			{
				return this.nativeScale;
			}
		}

		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x060050B3 RID: 20659 RVA: 0x00180E5C File Offset: 0x0017F05C
		public float ScaleMultiplier
		{
			get
			{
				return this.scaleMultiplier;
			}
		}

		// Token: 0x060050B4 RID: 20660 RVA: 0x00180E64 File Offset: 0x0017F064
		public void SetScaleMultiplier(float s)
		{
			this.scaleMultiplier = s;
		}

		// Token: 0x060050B5 RID: 20661 RVA: 0x00180E70 File Offset: 0x0017F070
		public void SetNativeScale(NativeSizeChangerSettings s)
		{
			float num = this.nativeScale;
			if (s != null && s.playerSizeScale > 0f && s.playerSizeScale != 1f)
			{
				this.activeSizeChangerSettings = s;
			}
			else
			{
				this.activeSizeChangerSettings = null;
			}
			if (this.activeSizeChangerSettings == null)
			{
				this.nativeScale = 1f;
			}
			else
			{
				this.nativeScale = this.activeSizeChangerSettings.playerSizeScale;
			}
			if (num != this.nativeScale && NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig != null;
			}
		}

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x060050B6 RID: 20662 RVA: 0x00180EFB File Offset: 0x0017F0FB
		public bool IsDefaultScale
		{
			get
			{
				return Mathf.Abs(1f - this.scale) < 0.001f;
			}
		}

		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x060050B7 RID: 20663 RVA: 0x00180F15 File Offset: 0x0017F115
		public bool turnedThisFrame
		{
			get
			{
				return this.degreesTurnedThisFrame != 0f;
			}
		}

		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x060050B8 RID: 20664 RVA: 0x00180F27 File Offset: 0x0017F127
		public List<GTPlayer.MaterialData> materialData
		{
			get
			{
				return this.materialDatasSO.datas;
			}
		}

		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x060050B9 RID: 20665 RVA: 0x00180F34 File Offset: 0x0017F134
		// (set) Token: 0x060050BA RID: 20666 RVA: 0x00180F3C File Offset: 0x0017F13C
		protected bool IsFrozen { get; set; }

		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x060050BB RID: 20667 RVA: 0x00180F45 File Offset: 0x0017F145
		public List<WaterVolume> HeadOverlappingWaterVolumes
		{
			get
			{
				return this.headOverlappingWaterVolumes;
			}
		}

		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x060050BC RID: 20668 RVA: 0x00180F4D File Offset: 0x0017F14D
		public bool InWater
		{
			get
			{
				return this.bodyInWater;
			}
		}

		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x060050BD RID: 20669 RVA: 0x00180F55 File Offset: 0x0017F155
		public bool HeadInWater
		{
			get
			{
				return this.headInWater;
			}
		}

		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x060050BE RID: 20670 RVA: 0x00180F5D File Offset: 0x0017F15D
		public WaterVolume CurrentWaterVolume
		{
			get
			{
				if (this.bodyOverlappingWaterVolumes.Count <= 0)
				{
					return null;
				}
				return this.bodyOverlappingWaterVolumes[0];
			}
		}

		// Token: 0x17000827 RID: 2087
		// (get) Token: 0x060050BF RID: 20671 RVA: 0x00180F7B File Offset: 0x0017F17B
		public WaterVolume.SurfaceQuery WaterSurfaceForHead
		{
			get
			{
				return this.waterSurfaceForHead;
			}
		}

		// Token: 0x17000828 RID: 2088
		// (get) Token: 0x060050C0 RID: 20672 RVA: 0x00180F83 File Offset: 0x0017F183
		public WaterVolume LeftHandWaterVolume
		{
			get
			{
				return this.leftHandWaterVolume;
			}
		}

		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x060050C1 RID: 20673 RVA: 0x00180F8B File Offset: 0x0017F18B
		public WaterVolume RightHandWaterVolume
		{
			get
			{
				return this.rightHandWaterVolume;
			}
		}

		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x060050C2 RID: 20674 RVA: 0x00180F93 File Offset: 0x0017F193
		public WaterVolume.SurfaceQuery LeftHandWaterSurface
		{
			get
			{
				return this.leftHandWaterSurface;
			}
		}

		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x060050C3 RID: 20675 RVA: 0x00180F9B File Offset: 0x0017F19B
		public WaterVolume.SurfaceQuery RightHandWaterSurface
		{
			get
			{
				return this.rightHandWaterSurface;
			}
		}

		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x060050C4 RID: 20676 RVA: 0x00180FA3 File Offset: 0x0017F1A3
		public Vector3 LastLeftHandPosition
		{
			get
			{
				return this.lastLeftHandPosition;
			}
		}

		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x060050C5 RID: 20677 RVA: 0x00180FAB File Offset: 0x0017F1AB
		public Vector3 LastRightHandPosition
		{
			get
			{
				return this.lastRightHandPosition;
			}
		}

		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x060050C6 RID: 20678 RVA: 0x00180FB3 File Offset: 0x0017F1B3
		public Vector3 RigidbodyVelocity
		{
			get
			{
				return this.playerRigidBody.velocity;
			}
		}

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x060050C7 RID: 20679 RVA: 0x00180FC0 File Offset: 0x0017F1C0
		public Vector3 HeadCenterPosition
		{
			get
			{
				return this.headCollider.transform.position + this.headCollider.transform.rotation * new Vector3(0f, 0f, -0.11f);
			}
		}

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x060050C8 RID: 20680 RVA: 0x00181000 File Offset: 0x0017F200
		public bool HandContactingSurface
		{
			get
			{
				return this.isLeftHandColliding || this.isRightHandColliding;
			}
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x060050C9 RID: 20681 RVA: 0x00181012 File Offset: 0x0017F212
		public bool BodyOnGround
		{
			get
			{
				return this.bodyGroundContactTime >= Time.time - 0.05f;
			}
		}

		// Token: 0x17000832 RID: 2098
		// (set) Token: 0x060050CA RID: 20682 RVA: 0x0018102A File Offset: 0x0017F22A
		public Quaternion PlayerRotationOverride
		{
			set
			{
				this.playerRotationOverride = value;
				this.playerRotationOverrideFrame = Time.frameCount;
			}
		}

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x060050CB RID: 20683 RVA: 0x0018103E File Offset: 0x0017F23E
		// (set) Token: 0x060050CC RID: 20684 RVA: 0x00181046 File Offset: 0x0017F246
		public bool IsBodySliding { get; set; }

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x060050CD RID: 20685 RVA: 0x0018104F File Offset: 0x0017F24F
		public GorillaClimbable CurrentClimbable
		{
			get
			{
				return this.currentClimbable;
			}
		}

		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x060050CE RID: 20686 RVA: 0x00181057 File Offset: 0x0017F257
		public GorillaHandClimber CurrentClimber
		{
			get
			{
				return this.currentClimber;
			}
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x060050CF RID: 20687 RVA: 0x0018105F File Offset: 0x0017F25F
		// (set) Token: 0x060050D0 RID: 20688 RVA: 0x00181067 File Offset: 0x0017F267
		public float jumpMultiplier
		{
			get
			{
				return this._jumpMultiplier;
			}
			set
			{
				this._jumpMultiplier = value;
			}
		}

		// Token: 0x060050D1 RID: 20689 RVA: 0x00181070 File Offset: 0x0017F270
		private void Awake()
		{
			if (GTPlayer._instance != null && GTPlayer._instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				GTPlayer._instance = this;
				GTPlayer.hasInstance = true;
			}
			this.InitializeValues();
			this.playerRigidBody.maxAngularVelocity = 0f;
			this.bodyOffsetVector = new Vector3(0f, -this.bodyCollider.height / 2f, 0f);
			this.bodyInitialHeight = this.bodyCollider.height;
			this.bodyInitialRadius = this.bodyCollider.radius;
			this.rayCastNonAllocColliders = new RaycastHit[5];
			this.crazyCheckVectors = new Vector3[7];
			this.emptyHit = default(RaycastHit);
			this.crazyCheckVectors[0] = Vector3.up;
			this.crazyCheckVectors[1] = Vector3.down;
			this.crazyCheckVectors[2] = Vector3.left;
			this.crazyCheckVectors[3] = Vector3.right;
			this.crazyCheckVectors[4] = Vector3.forward;
			this.crazyCheckVectors[5] = Vector3.back;
			this.crazyCheckVectors[6] = Vector3.zero;
			if (this.controllerState == null)
			{
				this.controllerState = base.GetComponent<ConnectedControllerHandler>();
			}
			this.layerChanger = base.GetComponent<LayerChanger>();
			this.bodyTouchedSurfaces = new Dictionary<GameObject, PhysicMaterial>();
		}

		// Token: 0x060050D2 RID: 20690 RVA: 0x001811DC File Offset: 0x0017F3DC
		protected void Start()
		{
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
			}
			this.mainCamera.farClipPlane = 500f;
			this.lastScale = this.scale;
			this.layerChanger.InitializeLayers(base.transform);
			float num = Quaternion.Angle(Quaternion.identity, GorillaTagger.Instance.offlineVRRig.transform.rotation) * Mathf.Sign(Vector3.Dot(Vector3.up, GorillaTagger.Instance.offlineVRRig.transform.right));
			this.Turn(num);
		}

		// Token: 0x060050D3 RID: 20691 RVA: 0x00181279 File Offset: 0x0017F479
		protected void OnDestroy()
		{
			if (GTPlayer._instance == this)
			{
				GTPlayer._instance = null;
				GTPlayer.hasInstance = false;
			}
			if (this.climbHelper)
			{
				Object.Destroy(this.climbHelper.gameObject);
			}
		}

		// Token: 0x060050D4 RID: 20692 RVA: 0x001812B4 File Offset: 0x0017F4B4
		public void InitializeValues()
		{
			Physics.SyncTransforms();
			this.playerRigidBody = base.GetComponent<Rigidbody>();
			this.velocityHistory = new Vector3[this.velocityHistorySize];
			this.slideAverageHistory = new Vector3[this.velocityHistorySize];
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Vector3.zero;
				this.slideAverageHistory[i] = Vector3.zero;
			}
			this.leftHandFollower.transform.position = this.leftControllerTransform.position;
			this.rightHandFollower.transform.position = this.rightControllerTransform.position;
			this.lastLeftHandPosition = this.leftHandFollower.transform.position;
			this.lastRightHandPosition = this.rightHandFollower.transform.position;
			this.lastHeadPosition = this.headCollider.transform.position;
			this.wasLeftHandColliding = false;
			this.wasRightHandColliding = false;
			this.velocityIndex = 0;
			this.averagedVelocity = Vector3.zero;
			this.slideVelocity = Vector3.zero;
			this.lastPosition = base.transform.position;
			this.lastRealTime = Time.realtimeSinceStartup;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
		}

		// Token: 0x060050D5 RID: 20693 RVA: 0x00181464 File Offset: 0x0017F664
		public void SetHalloweenLevitation(float levitateStrength, float levitateDuration, float levitateBlendOutDuration, float levitateBonusStrength, float levitateBonusOffAtYSpeed, float levitateBonusFullAtYSpeed)
		{
			this.halloweenLevitationStrength = levitateStrength;
			this.halloweenLevitationFullStrengthDuration = levitateDuration;
			this.halloweenLevitationTotalDuration = levitateDuration + levitateBlendOutDuration;
			this.halloweenLevitateBonusFullAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitateBonusOffAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitationBonusStrength = levitateBonusStrength;
		}

		// Token: 0x060050D6 RID: 20694 RVA: 0x00181495 File Offset: 0x0017F695
		public void TeleportToTrain(bool enable)
		{
			this.teleportToTrain = enable;
		}

		// Token: 0x060050D7 RID: 20695 RVA: 0x001814A0 File Offset: 0x0017F6A0
		public void TeleportTo(Vector3 position, Quaternion rotation)
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = true;
				component.position = position;
				component.rotation = rotation;
				component.isKinematic = false;
			}
			base.transform.position = position;
			base.transform.rotation = rotation;
			this.leftHandFollower.position = this.leftControllerTransform.position;
			this.leftHandFollower.rotation = this.leftControllerTransform.rotation;
			this.rightHandFollower.position = this.rightControllerTransform.position;
			this.rightHandFollower.rotation = this.rightControllerTransform.rotation;
			this.lastLeftHandPosition = this.leftHandFollower.transform.position;
			this.lastRightHandPosition = this.rightHandFollower.transform.position;
			this.lastHeadPosition = this.headCollider.transform.position;
			this.lastPosition = position;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			GorillaTagger.Instance.offlineVRRig.transform.position = position;
		}

		// Token: 0x060050D8 RID: 20696 RVA: 0x001815C0 File Offset: 0x0017F7C0
		public void TeleportTo(Transform destination, bool matchDestinationRotation = true, bool maintainVelocity = true)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = this.mainCamera.transform.position - position;
			Vector3 vector2 = destination.position - vector;
			float num = destination.rotation.eulerAngles.y - this.mainCamera.transform.rotation.eulerAngles.y;
			Vector3 vector3 = this.currentVelocity;
			if (!maintainVelocity)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			else if (matchDestinationRotation)
			{
				vector3 = Quaternion.AngleAxis(num, base.transform.up) * this.currentVelocity;
				this.SetPlayerVelocity(vector3);
			}
			if (matchDestinationRotation)
			{
				this.Turn(num);
			}
			this.TeleportTo(vector2, base.transform.rotation);
			if (maintainVelocity)
			{
				this.SetPlayerVelocity(vector3);
			}
		}

		// Token: 0x060050D9 RID: 20697 RVA: 0x00181699 File Offset: 0x0017F899
		public void AddForce(Vector3 force, ForceMode mode)
		{
			this.playerRigidBody.AddForce(force, mode);
		}

		// Token: 0x060050DA RID: 20698 RVA: 0x001816A8 File Offset: 0x0017F8A8
		public void SetPlayerVelocity(Vector3 newVelocity)
		{
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = newVelocity;
			}
			this.playerRigidBody.AddForce(newVelocity - this.playerRigidBody.velocity, ForceMode.VelocityChange);
		}

		// Token: 0x060050DB RID: 20699 RVA: 0x001816F2 File Offset: 0x0017F8F2
		public void SetGravityOverride(Object caller, Action<GTPlayer> gravityFunction)
		{
			if (!this.gravityOverrides.ContainsKey(caller))
			{
				this.gravityOverrides.Add(caller, gravityFunction);
				return;
			}
			this.gravityOverrides[caller] = gravityFunction;
		}

		// Token: 0x060050DC RID: 20700 RVA: 0x0018171D File Offset: 0x0017F91D
		public void UnsetGravityOverride(Object caller)
		{
			this.gravityOverrides.Remove(caller);
		}

		// Token: 0x060050DD RID: 20701 RVA: 0x0018172C File Offset: 0x0017F92C
		private void ApplyGravityOverrides()
		{
			foreach (KeyValuePair<Object, Action<GTPlayer>> keyValuePair in this.gravityOverrides)
			{
				keyValuePair.Value(this);
			}
		}

		// Token: 0x060050DE RID: 20702 RVA: 0x00181788 File Offset: 0x0017F988
		public void ApplyKnockback(Vector3 direction, float speed, bool forceOffTheGround = false)
		{
			if (forceOffTheGround)
			{
				if (this.wasLeftHandColliding || this.wasRightHandColliding)
				{
					this.wasLeftHandColliding = false;
					this.wasRightHandColliding = false;
					this.playerRigidBody.transform.position += this.minimumRaycastDistance * this.scale * Vector3.up;
				}
				this.didAJump = true;
				this.SetMaximumSlipThisFrame();
			}
			if (speed > 0.01f)
			{
				float num = Vector3.Dot(this.averagedVelocity, direction);
				float num2 = Mathf.InverseLerp(1.5f, 0.5f, num / speed);
				Vector3 vector = this.averagedVelocity + direction * speed * num2;
				this.playerRigidBody.velocity = vector;
				for (int i = 0; i < this.velocityHistory.Length; i++)
				{
					this.velocityHistory[i] = vector;
				}
			}
		}

		// Token: 0x060050DF RID: 20703 RVA: 0x00181864 File Offset: 0x0017FA64
		public void FixedUpdate()
		{
			this.AntiTeleportTechnology();
			this.IsFrozen = GorillaTagger.Instance.offlineVRRig.IsFrozen || this.debugFreezeTag;
			bool isDefaultScale = this.IsDefaultScale;
			this.playerRigidBody.useGravity = false;
			if (this.gravityOverrides.Count > 0)
			{
				this.ApplyGravityOverrides();
			}
			else
			{
				if (!this.isClimbing)
				{
					this.playerRigidBody.AddForce(Physics.gravity * this.scale, ForceMode.Acceleration);
				}
				if (this.halloweenLevitationBonusStrength > 0f || this.halloweenLevitationStrength > 0f)
				{
					float num = Time.time - this.lastTouchedGroundTimestamp;
					if (num < this.halloweenLevitationTotalDuration)
					{
						this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationStrength * Mathf.InverseLerp(this.halloweenLevitationFullStrengthDuration, this.halloweenLevitationTotalDuration, num), ForceMode.Acceleration);
					}
					float y = this.playerRigidBody.velocity.y;
					if (y <= this.halloweenLevitateBonusFullAtYSpeed)
					{
						this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength, ForceMode.Acceleration);
					}
					else if (y <= this.halloweenLevitateBonusOffAtYSpeed)
					{
						Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.velocity.y);
						this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength * Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.velocity.y), ForceMode.Acceleration);
					}
				}
			}
			if (this.enableHoverMode)
			{
				this.playerRigidBody.velocity = this.HoverboardFixedUpdate(this.playerRigidBody.velocity);
			}
			else
			{
				this.didHoverLastFrame = false;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.bodyInWater = false;
			Vector3 vector = this.swimmingVelocity;
			this.swimmingVelocity = Vector3.MoveTowards(this.swimmingVelocity, Vector3.zero, this.swimmingParams.swimmingVelocityOutOfWaterDrainRate * fixedDeltaTime);
			this.leftHandNonDiveHapticsAmount = 0f;
			this.rightHandNonDiveHapticsAmount = 0f;
			if (this.bodyOverlappingWaterVolumes.Count > 0)
			{
				WaterVolume waterVolume = null;
				float num2 = float.MinValue;
				Vector3 vector2 = this.headCollider.transform.position + Vector3.down * this.swimmingParams.floatingWaterLevelBelowHead * this.scale;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.bodyOverlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery;
					if (this.bodyOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector2, out surfaceQuery, false))
					{
						float num3 = Vector3.Dot(surfaceQuery.surfacePoint - vector2, surfaceQuery.surfaceNormal);
						if (num3 > num2)
						{
							num2 = num3;
							waterVolume = this.bodyOverlappingWaterVolumes[i];
							this.waterSurfaceForHead = surfaceQuery;
						}
						WaterCurrent waterCurrent = this.bodyOverlappingWaterVolumes[i].Current;
						if (waterCurrent != null && num3 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (waterVolume != null)
				{
					Vector3 velocity = this.playerRigidBody.velocity;
					float magnitude = velocity.magnitude;
					bool flag = this.headInWater;
					this.headInWater = this.headCollider.transform.position.y < this.waterSurfaceForHead.surfacePoint.y && this.headCollider.transform.position.y > this.waterSurfaceForHead.surfacePoint.y - this.waterSurfaceForHead.maxDepth;
					if (this.headInWater && !flag)
					{
						this.audioSetToUnderwater = true;
						this.audioManager.SetMixerSnapshot(this.audioManager.underwaterSnapshot, 0.1f);
					}
					else if (!this.headInWater && flag)
					{
						this.audioSetToUnderwater = false;
						this.audioManager.UnsetMixerSnapshot(0.1f);
					}
					this.bodyInWater = vector2.y < this.waterSurfaceForHead.surfacePoint.y && vector2.y > this.waterSurfaceForHead.surfacePoint.y - this.waterSurfaceForHead.maxDepth;
					if (this.bodyInWater)
					{
						GTPlayer.LiquidProperties liquidProperties = this.liquidPropertiesList[(int)waterVolume.LiquidType];
						if (waterVolume != null)
						{
							float num7;
							if (this.swimmingParams.extendBouyancyFromSpeed)
							{
								float num4 = Mathf.Clamp(Vector3.Dot(velocity / this.scale, this.waterSurfaceForHead.surfaceNormal), this.swimmingParams.speedToBouyancyExtensionMinMax.x, this.swimmingParams.speedToBouyancyExtensionMinMax.y);
								float num5 = this.swimmingParams.speedToBouyancyExtension.Evaluate(num4);
								this.buoyancyExtension = Mathf.Max(this.buoyancyExtension, num5);
								float num6 = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist + this.buoyancyExtension, num2 / this.scale + this.buoyancyExtension);
								this.buoyancyExtension = Spring.DamperDecayExact(this.buoyancyExtension, this.swimmingParams.buoyancyExtensionDecayHalflife, fixedDeltaTime, 1E-05f);
								num7 = num6;
							}
							else
							{
								num7 = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist, num2 / this.scale);
							}
							Vector3 vector3 = Physics.gravity * this.scale;
							Vector3 vector4 = liquidProperties.buoyancy * -vector3 * num7;
							if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
							{
								vector4 *= this.frozenBodyBuoyancyFactor;
							}
							this.playerRigidBody.AddForce(vector4, ForceMode.Acceleration);
						}
						Vector3 vector5 = Vector3.zero;
						Vector3 vector6 = Vector3.zero;
						for (int j = 0; j < this.activeWaterCurrents.Count; j++)
						{
							WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
							Vector3 vector7 = velocity + vector5;
							Vector3 vector8;
							Vector3 vector9;
							if (waterCurrent2.GetCurrentAtPoint(this.bodyCollider.transform.position, vector7, fixedDeltaTime, out vector8, out vector9))
							{
								vector6 += vector8;
								vector5 += vector9;
							}
						}
						if (magnitude > Mathf.Epsilon)
						{
							float num8 = 0.01f;
							Vector3 vector10 = velocity / magnitude;
							Vector3 right = this.leftHandFollower.right;
							Vector3 vector11 = -this.rightHandFollower.right;
							Vector3 forward = this.leftHandFollower.forward;
							Vector3 forward2 = this.rightHandFollower.forward;
							Vector3 vector12 = vector10;
							float num9 = 0f;
							float num10 = 0f;
							float num11 = 0f;
							if (this.swimmingParams.applyDiveSteering && !this.disableMovement && isDefaultScale)
							{
								float num12 = Vector3.Dot(velocity - vector6, vector10);
								float num13 = Mathf.Clamp(num12, this.swimmingParams.swimSpeedToRedirectAmountMinMax.x, this.swimmingParams.swimSpeedToRedirectAmountMinMax.y);
								float num14 = this.swimmingParams.swimSpeedToRedirectAmount.Evaluate(num13);
								num13 = Mathf.Clamp(num12, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.x, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.y);
								float num15 = this.swimmingParams.swimSpeedToMaxRedirectAngle.Evaluate(num13);
								float num16 = Mathf.Acos(Vector3.Dot(vector10, forward)) / 3.1415927f * -2f + 1f;
								float num17 = Mathf.Acos(Vector3.Dot(vector10, forward2)) / 3.1415927f * -2f + 1f;
								float num18 = Mathf.Clamp(num16, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float num19 = Mathf.Clamp(num17, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float num20 = ((!float.IsNaN(num18)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num18) : 0f);
								float num21 = ((!float.IsNaN(num19)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num19) : 0f);
								Vector3 vector13 = Vector3.ProjectOnPlane(vector10, right);
								Vector3 vector14 = Vector3.ProjectOnPlane(vector10, right);
								float num22 = Mathf.Min(vector13.magnitude, 1f);
								float num23 = Mathf.Min(vector14.magnitude, 1f);
								float magnitude2 = this.leftHandCenterVelocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float magnitude3 = this.rightHandCenterVelocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float num24 = Mathf.Clamp(magnitude2, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float num25 = Mathf.Clamp(magnitude3, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float num26 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(num24);
								float num27 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(num25);
								float averageSpeedChangeMagnitudeInDirection = this.leftHandCenterVelocityTracker.GetAverageSpeedChangeMagnitudeInDirection(right, false, this.swimmingParams.diveVelocityAveragingWindow);
								float averageSpeedChangeMagnitudeInDirection2 = this.rightHandCenterVelocityTracker.GetAverageSpeedChangeMagnitudeInDirection(vector11, false, this.swimmingParams.diveVelocityAveragingWindow);
								float num28 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float num29 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection2, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float num30 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(num28);
								float num31 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(num29);
								num9 = Mathf.Min(num20, Mathf.Min(num26, num30));
								float num32 = ((Vector3.Dot(vector10, forward) > 0f) ? (Mathf.Min(num9, num14) * num22) : 0f);
								num10 = Mathf.Min(num21, Mathf.Min(num27, num31));
								float num33 = ((Vector3.Dot(vector10, forward2) > 0f) ? (Mathf.Min(num10, num14) * num23) : 0f);
								if (this.swimmingParams.reduceDiveSteeringBelowVelocityPlane)
								{
									Vector3 vector15;
									if (Vector3.Dot(this.headCollider.transform.up, vector10) > 0.95f)
									{
										vector15 = -this.headCollider.transform.forward;
									}
									else
									{
										vector15 = Vector3.Cross(Vector3.Cross(vector10, this.headCollider.transform.up), vector10).normalized;
									}
									Vector3 position = this.headCollider.transform.position;
									Vector3 vector16 = position - this.leftHandFollower.position;
									Vector3 vector17 = position - this.rightHandFollower.position;
									float reduceDiveSteeringBelowPlaneFadeStartDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeStartDist;
									float reduceDiveSteeringBelowPlaneFadeEndDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeEndDist;
									float num34 = Vector3.Dot(vector16, Vector3.up);
									float num35 = Vector3.Dot(vector17, Vector3.up);
									float num36 = Vector3.Dot(vector16, vector15);
									float num37 = Vector3.Dot(vector17, vector15);
									float num38 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(num34), Mathf.Abs(num36)));
									float num39 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(num35), Mathf.Abs(num37)));
									num32 *= num38;
									num33 *= num39;
								}
								float num40 = num33 + num32;
								Vector3 vector18 = Vector3.zero;
								if (this.swimmingParams.applyDiveSteering && num40 > num8)
								{
									vector18 = ((num32 * vector13 + num33 * vector14) / num40).normalized;
									vector18 = Vector3.Lerp(vector10, vector18, num40);
									vector12 = Vector3.RotateTowards(vector10, vector18, 0.017453292f * num15 * fixedDeltaTime, 0f);
								}
								else
								{
									vector12 = vector10;
								}
								num11 = Mathf.Clamp01((num9 + num10) * 0.5f);
							}
							float num41 = Mathf.Clamp(Vector3.Dot(vector, vector10), 0f, magnitude);
							float num42 = magnitude - num41;
							if (this.swimmingParams.applyDiveSwimVelocityConversion && !this.disableMovement && num11 > num8 && num41 < this.swimmingParams.diveMaxSwimVelocityConversion)
							{
								float num43 = Mathf.Min(this.swimmingParams.diveSwimVelocityConversionRate * fixedDeltaTime, num42) * num11;
								num41 += num43;
								num42 -= num43;
							}
							float num44 = this.swimmingParams.swimUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float num45 = this.swimmingParams.baseUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float num46 = Spring.DamperDecayExact(num41 / this.scale, num44, fixedDeltaTime, 1E-05f) * this.scale;
							float num47 = Spring.DamperDecayExact(num42 / this.scale, num45, fixedDeltaTime, 1E-05f) * this.scale;
							if (this.swimmingParams.applyDiveDampingMultiplier && !this.disableMovement)
							{
								float num48 = Mathf.Lerp(1f, this.swimmingParams.diveDampingMultiplier, num11);
								num46 = Mathf.Lerp(num41, num46, num48);
								num47 = Mathf.Lerp(num42, num47, num48);
								float num49 = Mathf.Clamp((1f - num9) * (num41 + num42), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num8, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num8);
								float num50 = Mathf.Clamp((1f - num10) * (num41 + num42), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num8, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num8);
								this.leftHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(num49);
								this.rightHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(num50);
							}
							this.swimmingVelocity = num46 * vector12 + vector5 * this.scale;
							this.playerRigidBody.velocity = this.swimmingVelocity + num47 * vector12;
						}
					}
				}
			}
			else if (this.audioSetToUnderwater)
			{
				this.audioSetToUnderwater = false;
				this.audioManager.UnsetMixerSnapshot(0.1f);
			}
			this.handleClimbing(Time.fixedDeltaTime);
			this.stuckHandsCheckFixedUpdate();
			this.FixedUpdate_HandHolds(Time.fixedDeltaTime);
		}

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x060050E0 RID: 20704 RVA: 0x001826AF File Offset: 0x001808AF
		// (set) Token: 0x060050E1 RID: 20705 RVA: 0x001826B7 File Offset: 0x001808B7
		public bool isHoverAllowed { get; private set; }

		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x060050E2 RID: 20706 RVA: 0x001826C0 File Offset: 0x001808C0
		// (set) Token: 0x060050E3 RID: 20707 RVA: 0x001826C8 File Offset: 0x001808C8
		public bool enableHoverMode { get; private set; }

		// Token: 0x060050E4 RID: 20708 RVA: 0x001826D1 File Offset: 0x001808D1
		public void SetHoverboardPosRot(Vector3 worldPos, Quaternion worldRot)
		{
			this.hoverboardPlayerLocalPos = this.headCollider.transform.InverseTransformPoint(worldPos);
			this.hoverboardPlayerLocalRot = this.headCollider.transform.InverseTransformRotation(worldRot);
		}

		// Token: 0x060050E5 RID: 20709 RVA: 0x00182704 File Offset: 0x00180904
		private void HoverboardLateUpdate()
		{
			Vector3 eulerAngles = this.headCollider.transform.eulerAngles;
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				RaycastHit raycastHit;
				hoverBoardCast.didHit = Physics.SphereCast(new Ray(this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin), this.hoverboardVisual.transform.rotation * hoverBoardCast.localDirection), hoverBoardCast.sphereRadius, out raycastHit, hoverBoardCast.distance, this.locomotionEnabledLayers);
				if (hoverBoardCast.didHit)
				{
					HoverboardCantHover hoverboardCantHover;
					if (raycastHit.collider.TryGetComponent<HoverboardCantHover>(out hoverboardCantHover))
					{
						hoverBoardCast.didHit = false;
					}
					else
					{
						hoverBoardCast.pointHit = raycastHit.point;
						hoverBoardCast.normalHit = raycastHit.normal;
					}
				}
				this.hoverboardCasts[i] = hoverBoardCast;
				if (hoverBoardCast.didHit)
				{
					flag = true;
				}
			}
			this.hasHoverPoint = flag;
			this.bodyCollider.enabled = (this.bodyCollider.transform.position - this.hoverboardVisual.transform.TransformPoint(Vector3.up * this.hoverBodyCollisionRadiusUpOffset)).IsLongerThan(this.hoverBodyHasCollisionsOutsideRadius);
		}

		// Token: 0x060050E6 RID: 20710 RVA: 0x0018284C File Offset: 0x00180A4C
		private Vector3 HoverboardFixedUpdate(Vector3 velocity)
		{
			this.hoverboardVisual.transform.position = this.headCollider.transform.TransformPoint(this.hoverboardPlayerLocalPos);
			this.hoverboardVisual.transform.rotation = this.headCollider.transform.TransformRotation(this.hoverboardPlayerLocalRot);
			if (this.didHoverLastFrame)
			{
				velocity += Vector3.up * this.hoverGeneralUpwardForce * Time.fixedDeltaTime;
			}
			Vector3 position = this.hoverboardVisual.transform.position;
			Vector3 vector = position + velocity * Time.fixedDeltaTime;
			Vector3 vector2 = this.hoverboardVisual.transform.forward;
			Vector3 vector3 = (this.hoverboardCasts[0].didHit ? this.hoverboardCasts[0].normalHit : Vector3.up);
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				if (hoverBoardCast.didHit)
				{
					Vector3 vector4 = position + Vector3.Project(hoverBoardCast.pointHit - position, vector2);
					Vector3 vector5 = vector + Vector3.Project(hoverBoardCast.pointHit - position, vector2);
					bool flag2 = hoverBoardCast.isSolid || Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - vector5) + this.hoverIdealHeight > 0f;
					float num = (hoverBoardCast.isSolid ? (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin + hoverBoardCast.localDirection * hoverBoardCast.distance)) + hoverBoardCast.sphereRadius) : (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - vector4) + this.hoverIdealHeight));
					if (flag2)
					{
						flag = true;
						this.boostEnabledUntilTimestamp = Time.time + this.hoverboardBoostGracePeriod;
						if (Vector3.Dot(velocity, hoverBoardCast.normalHit) < 0f)
						{
							velocity = Vector3.ProjectOnPlane(velocity, hoverBoardCast.normalHit);
						}
						this.playerRigidBody.transform.position += hoverBoardCast.normalHit * num;
						Vector3 vector6 = this.turnParent.transform.rotation * (this.hoverboardVisual.IsLeftHanded ? this.leftHandCenterVelocityTracker : this.rightHandCenterVelocityTracker).GetAverageVelocity(false, 0.15f, false);
						if (Vector3.Dot(vector6, hoverBoardCast.normalHit) < 0f)
						{
							velocity -= Vector3.Project(vector6, hoverBoardCast.normalHit) * this.hoverSlamJumpStrengthFactor * Time.fixedDeltaTime;
						}
						vector = position + velocity * Time.fixedDeltaTime;
					}
				}
			}
			float num2 = Mathf.Abs(Mathf.DeltaAngle(0f, Mathf.Acos(Vector3.Dot(this.hoverboardVisual.transform.up, Vector3.ProjectOnPlane(vector3, vector2).normalized)) * 57.29578f));
			float num3 = this.hoverCarveAngleResponsiveness.Evaluate(num2);
			vector2 = (vector2 + Vector3.ProjectOnPlane(this.hoverboardVisual.transform.up, vector3) * this.hoverTiltAdjustsForwardFactor).normalized;
			if (!flag)
			{
				this.didHoverLastFrame = false;
				num3 = 0f;
			}
			Vector3 vector7 = velocity;
			if (this.enableHoverMode && this.hasHoverPoint)
			{
				Vector3 vector8 = Vector3.ProjectOnPlane(velocity, vector3);
				Vector3 vector9 = velocity - vector8;
				Vector3 vector10 = Vector3.Project(vector8, vector2);
				float num4 = vector8.magnitude;
				if (num4 <= this.hoveringSlowSpeed)
				{
					num4 *= this.hoveringSlowStoppingFactor;
				}
				Vector3 vector11 = vector8 - vector10;
				float num5 = 0f;
				bool flag3 = false;
				if (num3 > 0f)
				{
					if (vector11.IsLongerThan(vector10))
					{
						num5 = Mathf.Min((vector11.magnitude - vector10.magnitude) * this.hoverCarveSidewaysSpeedLossFactor * num3, num4);
						if (num5 > 0f && num4 > this.hoverMinGrindSpeed)
						{
							flag3 = true;
							this.hoverboardVisual.PlayGrindHaptic();
						}
						num4 -= num5;
					}
					vector11 *= 1f - num3 * this.sidewaysDrag;
					if (!this.isLeftHandColliding && !this.isRightHandColliding)
					{
						velocity = (vector10 + vector11).normalized * num4 + vector9;
					}
				}
				else
				{
					velocity = vector8.normalized * num4 + vector9;
				}
				float magnitude = (velocity - vector7).magnitude;
				this.hoverboardAudio.UpdateAudioLoop(velocity.magnitude, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, magnitude, flag3 ? num5 : 0f);
				if (magnitude > 0f && !flag3)
				{
					this.hoverboardVisual.PlayCarveHaptic(magnitude);
				}
			}
			else
			{
				this.hoverboardAudio.UpdateAudioLoop(0f, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, 0f, 0f);
			}
			return velocity;
		}

		// Token: 0x060050E7 RID: 20711 RVA: 0x00182DA8 File Offset: 0x00180FA8
		public void GrabPersonalHoverboard(bool isLeftHand, Vector3 pos, Quaternion rot, Color col)
		{
			if (this.hoverboardVisual.IsHeld)
			{
				this.hoverboardVisual.DropFreeBoard();
			}
			this.hoverboardVisual.SetIsHeld(isLeftHand, pos, rot, col);
			this.hoverboardVisual.ProxyGrabHandle(isLeftHand);
			FreeHoverboardManager.instance.PreserveMaxHoverboardsConstraint(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}

		// Token: 0x060050E8 RID: 20712 RVA: 0x00182E04 File Offset: 0x00181004
		public void SetHoverAllowed(bool allowed, bool force = false)
		{
			if (allowed)
			{
				this.hoverAllowedCount++;
				this.isHoverAllowed = true;
				return;
			}
			this.hoverAllowedCount = ((force || this.hoverAllowedCount == 0) ? 0 : (this.hoverAllowedCount - 1));
			if (this.hoverAllowedCount == 0 && this.isHoverAllowed)
			{
				this.isHoverAllowed = false;
				if (this.enableHoverMode)
				{
					this.SetHoverActive(false);
					VRRig.LocalRig.hoverboardVisual.SetNotHeld();
				}
			}
		}

		// Token: 0x060050E9 RID: 20713 RVA: 0x00182E7C File Offset: 0x0018107C
		public void SetHoverActive(bool enable)
		{
			if (enable && !this.isHoverAllowed)
			{
				return;
			}
			this.enableHoverMode = enable;
			if (!enable)
			{
				this.bodyCollider.enabled = true;
				this.hasHoverPoint = false;
				this.didHoverLastFrame = false;
				for (int i = 0; i < this.hoverboardCasts.Length; i++)
				{
					this.hoverboardCasts[i].didHit = false;
				}
				this.hoverboardAudio.Stop();
			}
		}

		// Token: 0x060050EA RID: 20714 RVA: 0x00182EEC File Offset: 0x001810EC
		private void BodyCollider()
		{
			if (this.MaxSphereSizeForNoOverlap(this.bodyInitialRadius * this.scale, this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), false, out this.bodyMaxRadius))
			{
				if (this.scale > 0f)
				{
					this.bodyCollider.radius = this.bodyMaxRadius / this.scale;
				}
				if (Physics.SphereCast(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), this.bodyMaxRadius, Vector3.down, out this.bodyHitInfo, this.bodyInitialHeight * this.scale - this.bodyMaxRadius, this.locomotionEnabledLayers))
				{
					this.bodyCollider.height = (this.bodyHitInfo.distance + this.bodyMaxRadius) / this.scale;
				}
				else
				{
					this.bodyCollider.height = this.bodyInitialHeight;
				}
				if (!this.bodyCollider.gameObject.activeSelf)
				{
					this.bodyCollider.gameObject.SetActive(true);
				}
			}
			else
			{
				this.bodyCollider.gameObject.SetActive(false);
			}
			this.bodyCollider.height = Mathf.Lerp(this.bodyCollider.height, this.bodyInitialHeight, this.bodyLerp);
			this.bodyCollider.radius = Mathf.Lerp(this.bodyCollider.radius, this.bodyInitialRadius, this.bodyLerp);
			this.bodyOffsetVector = Vector3.down * this.bodyCollider.height / 2f;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector * this.scale;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
		}

		// Token: 0x060050EB RID: 20715 RVA: 0x001830F4 File Offset: 0x001812F4
		private Vector3 GetCurrentHandPosition(Transform handTransform, Vector3 handOffset)
		{
			if (this.inOverlay)
			{
				return this.headCollider.transform.position + this.headCollider.transform.up * -0.5f * this.scale;
			}
			if ((this.PositionWithOffset(handTransform, handOffset) - this.headCollider.transform.position).magnitude < this.maxArmLength * this.scale)
			{
				return this.PositionWithOffset(handTransform, handOffset);
			}
			return this.headCollider.transform.position + (this.PositionWithOffset(handTransform, handOffset) - this.headCollider.transform.position).normalized * this.maxArmLength * this.scale;
		}

		// Token: 0x060050EC RID: 20716 RVA: 0x001831D1 File Offset: 0x001813D1
		private Vector3 GetLastLeftHandPosition()
		{
			return this.lastLeftHandPosition + this.MovingSurfaceMovement();
		}

		// Token: 0x060050ED RID: 20717 RVA: 0x001831E4 File Offset: 0x001813E4
		private Vector3 GetLastRightHandPosition()
		{
			return this.lastRightHandPosition + this.MovingSurfaceMovement();
		}

		// Token: 0x060050EE RID: 20718 RVA: 0x001831F8 File Offset: 0x001813F8
		private Vector3 GetCurrentLeftHandPosition()
		{
			if (this.inOverlay)
			{
				return this.headCollider.transform.position + this.headCollider.transform.up * -0.5f * this.scale;
			}
			if ((this.PositionWithOffset(this.leftControllerTransform, this.leftHandOffset) - this.headCollider.transform.position).magnitude < this.maxArmLength * this.scale)
			{
				return this.PositionWithOffset(this.leftControllerTransform, this.leftHandOffset);
			}
			return this.headCollider.transform.position + (this.PositionWithOffset(this.leftControllerTransform, this.leftHandOffset) - this.headCollider.transform.position).normalized * this.maxArmLength * this.scale;
		}

		// Token: 0x060050EF RID: 20719 RVA: 0x001832F4 File Offset: 0x001814F4
		private Vector3 GetCurrentRightHandPosition()
		{
			if (this.inOverlay)
			{
				return this.headCollider.transform.position + this.headCollider.transform.up * -0.5f * this.scale;
			}
			if ((this.PositionWithOffset(this.rightControllerTransform, this.rightHandOffset) - this.headCollider.transform.position).magnitude < this.maxArmLength * this.scale)
			{
				return this.PositionWithOffset(this.rightControllerTransform, this.rightHandOffset);
			}
			return this.headCollider.transform.position + (this.PositionWithOffset(this.rightControllerTransform, this.rightHandOffset) - this.headCollider.transform.position).normalized * this.maxArmLength * this.scale;
		}

		// Token: 0x060050F0 RID: 20720 RVA: 0x001833EF File Offset: 0x001815EF
		private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
		{
			return transformToModify.position + transformToModify.rotation * offsetVector * this.scale;
		}

		// Token: 0x060050F1 RID: 20721 RVA: 0x00183414 File Offset: 0x00181614
		public void ScaleAwayFromPoint(float oldScale, float newScale, Vector3 scaleCenter)
		{
			if (oldScale < newScale)
			{
				this.lastHeadPosition = GTPlayer.ScalePointAwayFromCenter(this.lastHeadPosition, this.headCollider.radius, oldScale, newScale, scaleCenter);
				this.lastLeftHandPosition = GTPlayer.ScalePointAwayFromCenter(this.lastLeftHandPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
				this.lastRightHandPosition = GTPlayer.ScalePointAwayFromCenter(this.lastRightHandPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
			}
		}

		// Token: 0x060050F2 RID: 20722 RVA: 0x00183478 File Offset: 0x00181678
		private static Vector3 ScalePointAwayFromCenter(Vector3 point, float baseRadius, float oldScale, float newScale, Vector3 scaleCenter)
		{
			float magnitude = (point - scaleCenter).magnitude;
			float num = magnitude + Mathf.Epsilon + baseRadius * (newScale - oldScale);
			return scaleCenter + (point - scaleCenter) * num / magnitude;
		}

		// Token: 0x060050F3 RID: 20723 RVA: 0x001834C0 File Offset: 0x001816C0
		private void LateUpdate()
		{
			if (this.playerRigidBody.isKinematic)
			{
				return;
			}
			float time = Time.time;
			Vector3 position = this.headCollider.transform.position;
			if (this.playerRotationOverrideFrame < Time.frameCount - 1)
			{
				this.playerRotationOverride = Quaternion.Slerp(Quaternion.identity, this.playerRotationOverride, Mathf.Exp(-this.playerRotationOverrideDecayRate * Time.deltaTime));
			}
			base.transform.rotation = this.playerRotationOverride;
			this.turnParent.transform.localScale = VRRig.LocalRig.transform.localScale;
			this.playerRigidBody.MovePosition(this.playerRigidBody.position + position - this.headCollider.transform.position);
			if (Mathf.Abs(this.lastScale - this.scale) > 0.001f)
			{
				if (this.mainCamera == null)
				{
					this.mainCamera = Camera.main;
				}
				this.mainCamera.nearClipPlane = ((this.scale > 0.5f) ? 0.01f : 0.002f);
			}
			this.lastScale = this.scale;
			this.debugLastRightHandPosition = this.lastRightHandPosition;
			this.debugPlatformDeltaPosition = this.MovingSurfaceMovement();
			if (this.debugMovement)
			{
				this.tempRealTime = Time.time;
				this.calcDeltaTime = Time.deltaTime;
				this.lastRealTime = this.tempRealTime;
			}
			else
			{
				this.tempRealTime = Time.realtimeSinceStartup;
				this.calcDeltaTime = this.tempRealTime - this.lastRealTime;
				this.lastRealTime = this.tempRealTime;
				if (this.calcDeltaTime > 0.1f)
				{
					this.calcDeltaTime = 0.05f;
				}
			}
			Vector3 vector;
			if (this.lastFrameHasValidTouchPos && this.lastPlatformTouched != null && GTPlayer.ComputeWorldHitPoint(this.lastHitInfoHand, this.lastFrameTouchPosLocal, out vector))
			{
				this.refMovement = vector - this.lastFrameTouchPosWorld;
			}
			else
			{
				this.refMovement = Vector3.zero;
			}
			Vector3 vector2 = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			Vector3 vector3 = this.headCollider.transform.position;
			Vector3 vector4;
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeWorldHitPoint(this.lastMovingSurfaceHit, this.lastMovingSurfaceTouchLocal, out vector4))
			{
				if (this.wasMovingSurfaceMonkeBlock && (this.lastMonkeBlock == null || this.lastMonkeBlock.state != BuilderPiece.State.AttachedAndPlaced))
				{
					this.movingSurfaceOffset = Vector3.zero;
				}
				else
				{
					this.movingSurfaceOffset = vector4 - this.lastMovingSurfaceTouchWorld;
					vector2 = this.movingSurfaceOffset / this.calcDeltaTime;
					quaternion = this.lastMovingSurfaceHit.collider.transform.rotation * Quaternion.Inverse(this.lastMovingSurfaceRot);
					vector3 = vector4;
				}
			}
			else
			{
				this.movingSurfaceOffset = Vector3.zero;
			}
			float num = 40f * this.scale;
			if (vector2.sqrMagnitude >= num * num)
			{
				this.movingSurfaceOffset = Vector3.zero;
				vector2 = Vector3.zero;
				quaternion = Quaternion.identity;
			}
			if (!this.didAJump && (this.wasLeftHandColliding || this.wasRightHandColliding))
			{
				base.transform.position = base.transform.position + 4.9f * Vector3.down * this.calcDeltaTime * this.calcDeltaTime * this.scale;
				if (Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) <= 0f && Vector3.Dot(Vector3.up, this.slideAverageNormal) > 0f)
				{
					base.transform.position = base.transform.position - Vector3.Project(Mathf.Min(this.stickDepth * this.scale, Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude * this.calcDeltaTime) * this.slideAverageNormal, Vector3.down);
				}
			}
			if (!this.didAJump && (this.wasLeftHandSliding || this.wasRightHandSliding))
			{
				base.transform.position = base.transform.position + this.slideVelocity * this.calcDeltaTime;
				this.slideVelocity += 9.8f * Vector3.down * this.calcDeltaTime * this.scale;
			}
			float num2 = ((Time.time > this.boostEnabledUntilTimestamp) ? 0f : (Time.deltaTime * Mathf.Clamp(this.playerRigidBody.velocity.magnitude * this.hoverboardPaddleBoostMultiplier, 0f, this.hoverboardPaddleBoostMax)));
			Vector3 vector5 = (this.enableHoverMode ? (this.turnParent.transform.rotation * -this.leftHandCenterVelocityTracker.GetAverageVelocity(false, 0.15f, false) * num2) : Vector3.zero);
			Vector3 vector6;
			this.FirstHandIteration(this.leftControllerTransform, this.leftHandOffset, this.GetLastLeftHandPosition(), vector5, this.wasLeftHandSliding, this.wasLeftHandColliding, this.LeftSlipOverriddenToMax(), out vector6, ref this.leftHandSlipPercentage, ref this.isLeftHandSliding, ref this.leftHandSlideNormal, ref this.isLeftHandColliding, ref this.leftHandMaterialTouchIndex, ref this.leftHandSurfaceOverride, this.leftHandHolding, this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT);
			this.isLeftHandColliding = this.isLeftHandColliding && this.controllerState.LeftValid;
			this.isLeftHandSliding = this.isLeftHandSliding && this.controllerState.LeftValid;
			RaycastHit raycastHit = this.lastHitInfoHand;
			Vector3 vector7 = (this.enableHoverMode ? (this.turnParent.transform.rotation * -this.rightHandCenterVelocityTracker.GetAverageVelocity(false, 0.15f, false) * num2) : Vector3.zero);
			Vector3 vector8;
			this.FirstHandIteration(this.rightControllerTransform, this.rightHandOffset, this.GetLastRightHandPosition(), vector7, this.wasRightHandSliding, this.wasRightHandColliding, this.RightSlipOverriddenToMax(), out vector8, ref this.rightHandSlipPercentage, ref this.isRightHandSliding, ref this.rightHandSlideNormal, ref this.isRightHandColliding, ref this.rightHandMaterialTouchIndex, ref this.rightHandSurfaceOverride, this.rightHandHolding, this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.RIGHT);
			this.isRightHandColliding = this.isRightHandColliding && this.controllerState.RightValid;
			this.isRightHandSliding = this.isRightHandSliding && this.controllerState.RightValid;
			this.touchPoints = 0;
			Vector3 vector9 = Vector3.zero;
			if (this.isLeftHandColliding || this.wasLeftHandColliding)
			{
				if (this.leftHandSurfaceOverride && this.leftHandSurfaceOverride.disablePushBackEffect)
				{
					vector9 += Vector3.zero;
				}
				else
				{
					vector9 += vector6;
				}
				this.touchPoints++;
			}
			if (this.isRightHandColliding || this.wasRightHandColliding)
			{
				if (this.rightHandSurfaceOverride && this.rightHandSurfaceOverride.disablePushBackEffect)
				{
					vector9 += Vector3.zero;
				}
				else
				{
					vector9 += vector8;
				}
				this.touchPoints++;
			}
			if (this.touchPoints != 0)
			{
				vector9 /= (float)this.touchPoints;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.RIGHT || this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT)
			{
				vector9 += this.movingSurfaceOffset;
			}
			else if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.BODY)
			{
				Vector3 vector10 = this.lastHeadPosition + this.movingSurfaceOffset - this.headCollider.transform.position;
				vector9 += vector10;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition, true, out this.maxSphereSize1) && !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
			}
			Vector3 vector11;
			float num3;
			if (this.IterativeCollisionSphereCast(this.lastHeadPosition, this.headCollider.radius * 0.9f * this.scale, this.headCollider.transform.position + vector9 - this.lastHeadPosition, Vector3.zero, out vector11, false, out num3, out this.junkHit, true))
			{
				vector9 = vector11 - this.headCollider.transform.position;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition + vector9, true, out this.maxSphereSize1) || !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition + vector9))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
				vector9 = this.lastHeadPosition - this.headCollider.transform.position;
			}
			else if (this.headCollider.radius * 0.9f * 0.825f * this.scale < this.maxSphereSize1)
			{
				this.lastOpenHeadPosition = this.headCollider.transform.position + vector9;
			}
			if (vector9 != Vector3.zero)
			{
				base.transform.position += vector9;
			}
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && quaternion != Quaternion.identity && !this.isClimbing && !this.rightHandHolding && !this.leftHandHolding)
			{
				this.RotateWithSurface(quaternion, vector3);
			}
			this.lastHeadPosition = this.headCollider.transform.position;
			this.areBothTouching = (!this.isLeftHandColliding && !this.wasLeftHandColliding) || (!this.isRightHandColliding && !this.wasRightHandColliding);
			Vector3 vector12 = this.FinalHandPosition(this.leftControllerTransform, this.leftHandOffset, this.GetLastLeftHandPosition(), vector5, this.areBothTouching, this.isLeftHandColliding, out this.isLeftHandColliding, this.isLeftHandSliding, out this.isLeftHandSliding, this.leftHandMaterialTouchIndex, out this.leftHandMaterialTouchIndex, this.leftHandSurfaceOverride, out this.leftHandSurfaceOverride, this.leftHandHolding, ref this.leftHandHitInfo);
			this.isLeftHandColliding = this.isLeftHandColliding && this.controllerState.LeftValid;
			this.isLeftHandSliding = this.isLeftHandSliding && this.controllerState.LeftValid;
			raycastHit = this.lastHitInfoHand;
			Vector3 vector13 = this.FinalHandPosition(this.rightControllerTransform, this.rightHandOffset, this.GetLastRightHandPosition(), vector7, this.areBothTouching, this.isRightHandColliding, out this.isRightHandColliding, this.isRightHandSliding, out this.isRightHandSliding, this.rightHandMaterialTouchIndex, out this.rightHandMaterialTouchIndex, this.rightHandSurfaceOverride, out this.rightHandSurfaceOverride, this.rightHandHolding, ref this.rightHandHitInfo);
			this.isRightHandColliding = this.isRightHandColliding && this.controllerState.RightValid;
			this.isRightHandSliding = this.isRightHandSliding && this.controllerState.RightValid;
			Vector3 vector14 = this.lastPosition;
			GTPlayer.MovingSurfaceContactPoint movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
			int num4 = -1;
			int num5 = -1;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = this.isRightHandColliding && this.IsTouchingMovingSurface(this.GetLastRightHandPosition(), this.lastHitInfoHand, out num4, out flag, out flag2);
			if (flag4 && !flag)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
				this.lastMovingSurfaceHit = this.lastHitInfoHand;
			}
			else
			{
				bool flag5 = false;
				BuilderPiece builderPiece = (flag4 ? this.lastMonkeBlock : null);
				if (this.isLeftHandColliding && this.IsTouchingMovingSurface(this.GetLastLeftHandPosition(), raycastHit, out num5, out flag5, out flag3))
				{
					if (flag5 && flag2 == flag3)
					{
						if (flag && num5.Equals(num4) && (double)Vector3.Dot(raycastHit.point - this.GetLastLeftHandPosition(), this.lastHitInfoHand.point - this.GetLastRightHandPosition()) < 0.3)
						{
							movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
							this.lastMovingSurfaceHit = this.lastHitInfoHand;
							this.lastMonkeBlock = builderPiece;
						}
					}
					else
					{
						movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.LEFT;
						this.lastMovingSurfaceHit = raycastHit;
					}
				}
			}
			this.StoreVelocities();
			if (this.InWater)
			{
				PlayerGameEvents.PlayerSwam((this.lastPosition - vector14).magnitude, this.currentVelocity.magnitude);
			}
			else
			{
				PlayerGameEvents.PlayerMoved((this.lastPosition - vector14).magnitude, this.currentVelocity.magnitude);
			}
			this.didAJump = false;
			bool flag6 = this.exitMovingSurface;
			this.exitMovingSurface = false;
			if (this.LeftSlipOverriddenToMax() && this.RightSlipOverriddenToMax())
			{
				this.didAJump = true;
				this.exitMovingSurface = true;
			}
			else if (this.isRightHandSliding || this.isLeftHandSliding)
			{
				this.slideAverageNormal = Vector3.zero;
				this.touchPoints = 0;
				this.averageSlipPercentage = 0f;
				if (this.isLeftHandSliding)
				{
					this.slideAverageNormal += this.leftHandSlideNormal.normalized;
					this.averageSlipPercentage += this.leftHandSlipPercentage;
					this.touchPoints++;
				}
				if (this.isRightHandSliding)
				{
					this.slideAverageNormal += this.rightHandSlideNormal.normalized;
					this.averageSlipPercentage += this.rightHandSlipPercentage;
					this.touchPoints++;
				}
				this.slideAverageNormal = this.slideAverageNormal.normalized;
				this.averageSlipPercentage /= (float)this.touchPoints;
				if (this.touchPoints == 1)
				{
					this.surfaceDirection = (this.isRightHandSliding ? Vector3.ProjectOnPlane(this.rightControllerTransform.forward, this.rightHandSlideNormal) : Vector3.ProjectOnPlane(this.leftControllerTransform.forward, this.leftHandSlideNormal));
					if (Vector3.Dot(this.slideVelocity, this.surfaceDirection) > 0f)
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
					else
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, -this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
				}
				if (!this.wasLeftHandSliding && !this.wasRightHandSliding)
				{
					this.slideVelocity = ((Vector3.Dot(this.playerRigidBody.velocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.playerRigidBody.velocity, this.slideAverageNormal) : this.playerRigidBody.velocity);
				}
				else
				{
					this.slideVelocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
				}
				this.slideVelocity = this.slideVelocity.normalized * Mathf.Min(this.slideVelocity.magnitude, Mathf.Max(0.5f, this.averagedVelocity.magnitude * 2f));
				this.playerRigidBody.velocity = Vector3.zero;
			}
			else if (this.isLeftHandColliding || this.isRightHandColliding)
			{
				if (!this.turnedThisFrame)
				{
					this.playerRigidBody.velocity = Vector3.zero;
				}
				else
				{
					this.playerRigidBody.velocity = this.playerRigidBody.velocity.normalized * Mathf.Min(2f, this.playerRigidBody.velocity.magnitude);
				}
			}
			else if (this.wasLeftHandSliding || this.wasRightHandSliding)
			{
				this.playerRigidBody.velocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
			}
			if ((this.isRightHandColliding || this.isLeftHandColliding) && !this.disableMovement && !this.turnedThisFrame && !this.didAJump)
			{
				if (this.isRightHandSliding || this.isLeftHandSliding)
				{
					if (Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > this.slideVelocityLimit * this.scale && Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) > 0f && Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > Vector3.Project(this.slideVelocity, this.slideAverageNormal).magnitude)
					{
						this.isLeftHandSliding = false;
						this.isRightHandSliding = false;
						this.didAJump = true;
						float num6 = this.ApplyNativeScaleAdjustment(Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude));
						this.playerRigidBody.velocity = num6 * this.slideAverageNormal.normalized + Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal);
						if (num6 > this.slideVelocityLimit * this.scale * this.exitMovingSurfaceThreshold)
						{
							this.exitMovingSurface = true;
						}
					}
				}
				else if (this.averagedVelocity.magnitude > this.velocityLimit * this.scale)
				{
					float num7 = ((this.InWater && this.CurrentWaterVolume != null) ? this.liquidPropertiesList[(int)this.CurrentWaterVolume.LiquidType].surfaceJumpFactor : 1f);
					float num8 = this.ApplyNativeScaleAdjustment(this.enableHoverMode ? Mathf.Min(this.hoverMaxPaddleSpeed, this.averagedVelocity.magnitude) : Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * num7 * this.averagedVelocity.magnitude));
					Vector3 vector15 = num8 * this.averagedVelocity.normalized;
					this.didAJump = true;
					this.playerRigidBody.velocity = vector15;
					if (this.InWater)
					{
						this.swimmingVelocity += vector15 * this.swimmingParams.underwaterJumpsAsSwimVelocityFactor;
					}
					if (num8 > this.velocityLimit * this.scale * this.exitMovingSurfaceThreshold)
					{
						this.exitMovingSurface = true;
					}
				}
			}
			this.stuckHandsCheckLateUpdate(ref vector12, ref vector13);
			if (this.lastPlatformTouched != null && this.currentPlatform == null)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.velocity += this.refMovement / this.calcDeltaTime;
				}
				this.refMovement = Vector3.zero;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.velocity += this.lastMovingSurfaceVelocity;
				}
				this.lastMovingSurfaceVelocity = Vector3.zero;
			}
			if (this.enableHoverMode)
			{
				this.HoverboardLateUpdate();
			}
			else
			{
				this.hasHoverPoint = false;
			}
			Vector3 vector16 = Vector3.zero;
			float num9 = 0f;
			Vector3 vector17;
			if (this.GetSwimmingVelocityForHand(this.lastLeftHandPosition, vector12, this.leftControllerTransform.right, this.calcDeltaTime, ref this.leftHandWaterVolume, ref this.leftHandWaterSurface, out vector17) && !this.turnedThisFrame)
			{
				num9 = Mathf.InverseLerp(0f, 0.2f, vector17.magnitude) * this.swimmingParams.swimmingHapticsStrength;
				vector16 += vector17;
			}
			float num10 = 0f;
			Vector3 vector18;
			if (this.GetSwimmingVelocityForHand(this.lastRightHandPosition, vector13, -this.rightControllerTransform.right, this.calcDeltaTime, ref this.rightHandWaterVolume, ref this.rightHandWaterSurface, out vector18) && !this.turnedThisFrame)
			{
				num10 = Mathf.InverseLerp(0f, 0.15f, vector18.magnitude) * this.swimmingParams.swimmingHapticsStrength;
				vector16 += vector18;
			}
			Vector3 vector19 = Vector3.zero;
			Vector3 vector20;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.lastLeftHandPosition, vector12, this.leftControllerTransform.right, this.leftHandCenterVelocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.leftHandWaterVolume, this.leftHandWaterSurface, out vector20))
			{
				if (time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown)
				{
					vector19 += vector20;
				}
				this.lastWaterSurfaceJumpTimeLeft = Time.time;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			Vector3 vector21;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.lastRightHandPosition, vector13, -this.rightControllerTransform.right, this.rightHandCenterVelocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.rightHandWaterVolume, this.rightHandWaterSurface, out vector21))
			{
				if (time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown)
				{
					vector19 += vector21;
				}
				this.lastWaterSurfaceJumpTimeRight = Time.time;
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			vector19 = Vector3.ClampMagnitude(vector19, this.swimmingParams.waterSurfaceJumpMaxSpeed * this.scale);
			float num11 = Mathf.Max(num9, this.leftHandNonDiveHapticsAmount);
			if (num11 > 0.001f && time - this.lastWaterSurfaceJumpTimeLeft > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.LeftHand, num11, this.calcDeltaTime);
			}
			float num12 = Mathf.Max(num10, this.rightHandNonDiveHapticsAmount);
			if (num12 > 0.001f && time - this.lastWaterSurfaceJumpTimeRight > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.RightHand, num12, this.calcDeltaTime);
			}
			if (!this.disableMovement)
			{
				this.swimmingVelocity += vector16;
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.velocity += vector16 + vector19;
				}
			}
			else
			{
				this.swimmingVelocity = Vector3.zero;
			}
			if (GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				if (!this.IsFrozen || !this.primaryButtonPressed)
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
					if (this.bodyTouchedSurfaces.Count > 0)
					{
						foreach (KeyValuePair<GameObject, PhysicMaterial> keyValuePair in this.bodyTouchedSurfaces)
						{
							MeshCollider meshCollider;
							if (keyValuePair.Key.TryGetComponent<MeshCollider>(out meshCollider))
							{
								meshCollider.material = keyValuePair.Value;
							}
						}
						this.bodyTouchedSurfaces.Clear();
					}
				}
				else if (this.BodyOnGround && this.primaryButtonPressed)
				{
					float num13 = this.bodyInitialHeight / 2f - this.bodyInitialRadius;
					RaycastHit raycastHit2;
					if (Physics.SphereCast(this.bodyCollider.transform.position - new Vector3(0f, num13, 0f), this.bodyInitialRadius - 0.01f, Vector3.down, out raycastHit2, 1f, ~LayerMask.GetMask(new string[] { "Gorilla Body Collider", "GorillaInteractable" }), QueryTriggerInteraction.Ignore))
					{
						this.IsBodySliding = true;
						MeshCollider meshCollider2;
						if (!this.bodyTouchedSurfaces.ContainsKey(raycastHit2.transform.gameObject) && raycastHit2.transform.gameObject.TryGetComponent<MeshCollider>(out meshCollider2))
						{
							this.bodyTouchedSurfaces.Add(raycastHit2.transform.gameObject, meshCollider2.material);
							raycastHit2.transform.gameObject.GetComponent<MeshCollider>().material = this.slipperyMaterial;
						}
					}
				}
				else
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
				}
			}
			else
			{
				this.IsBodySliding = false;
				if (this.bodyTouchedSurfaces.Count > 0)
				{
					foreach (KeyValuePair<GameObject, PhysicMaterial> keyValuePair2 in this.bodyTouchedSurfaces)
					{
						MeshCollider meshCollider3;
						if (keyValuePair2.Key.TryGetComponent<MeshCollider>(out meshCollider3))
						{
							meshCollider3.material = keyValuePair2.Value;
						}
					}
					this.bodyTouchedSurfaces.Clear();
				}
			}
			this.leftHandFollower.position = vector12;
			this.rightHandFollower.position = vector13;
			this.leftHandFollower.rotation = this.leftControllerTransform.rotation * this.leftHandRotOffset;
			this.rightHandFollower.rotation = this.rightControllerTransform.rotation * this.rightHandRotOffset;
			this.wasLeftHandColliding = this.isLeftHandColliding;
			this.wasRightHandColliding = this.isRightHandColliding;
			this.wasLeftHandSliding = this.isLeftHandSliding;
			this.wasRightHandSliding = this.isRightHandSliding;
			if ((this.isLeftHandColliding && !this.isLeftHandSliding) || (this.isRightHandColliding && !this.isRightHandSliding))
			{
				this.lastTouchedGroundTimestamp = Time.time;
			}
			this.degreesTurnedThisFrame = 0f;
			this.lastPlatformTouched = this.currentPlatform;
			this.currentPlatform = null;
			this.lastMovingSurfaceVelocity = vector2;
			this.lastLeftHandPosition = vector12;
			this.lastRightHandPosition = vector13;
			Vector3 vector22;
			if (GTPlayer.ComputeLocalHitPoint(this.lastHitInfoHand, out vector22))
			{
				this.lastFrameHasValidTouchPos = true;
				this.lastFrameTouchPosLocal = vector22;
				this.lastFrameTouchPosWorld = this.lastHitInfoHand.point;
			}
			else
			{
				this.lastFrameHasValidTouchPos = false;
				this.lastFrameTouchPosLocal = Vector3.zero;
				this.lastFrameTouchPosWorld = Vector3.zero;
			}
			this.lastRigidbodyPosition = this.playerRigidBody.transform.position;
			RaycastHit raycastHit3 = default(RaycastHit);
			this.BodyCollider();
			if (this.bodyHitInfo.collider != null)
			{
				this.wasBodyOnGround = true;
				raycastHit3 = this.bodyHitInfo;
			}
			else if (movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.bodyCollider.gameObject.activeSelf)
			{
				bool flag7 = false;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				Vector3 vector23 = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + (this.bodyInitialHeight * this.scale - this.bodyMaxRadius) * Vector3.down;
				this.bufferCount = Physics.SphereCastNonAlloc(vector23, this.bodyMaxRadius, Vector3.down, this.rayCastNonAllocColliders, this.minimumRaycastDistance * this.scale, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int i = 0; i < this.bufferCount; i++)
					{
						if (this.tempHitInfo.distance > 0f && (!flag7 || this.rayCastNonAllocColliders[i].distance < this.tempHitInfo.distance))
						{
							flag7 = true;
							raycastHit3 = this.rayCastNonAllocColliders[i];
						}
					}
				}
				this.wasBodyOnGround = flag7;
			}
			int num14 = -1;
			bool flag8 = false;
			bool flag9;
			if (this.wasBodyOnGround && movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.IsTouchingMovingSurface(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), raycastHit3, out num14, out flag9, out flag8) && !flag9)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.BODY;
				this.lastMovingSurfaceHit = raycastHit3;
			}
			Vector3 vector24;
			if (movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeLocalHitPoint(this.lastMovingSurfaceHit, out vector24))
			{
				this.lastMovingSurfaceTouchLocal = vector24;
				this.lastMovingSurfaceTouchWorld = this.lastMovingSurfaceHit.point;
				this.lastMovingSurfaceRot = this.lastMovingSurfaceHit.collider.transform.rotation;
				this.lastAttachedToMovingSurfaceFrame = Time.frameCount;
			}
			else
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
				this.lastMovingSurfaceTouchLocal = Vector3.zero;
				this.lastMovingSurfaceTouchWorld = Vector3.zero;
				this.lastMovingSurfaceRot = Quaternion.identity;
			}
			Vector3 vector25 = this.lastMovingSurfaceTouchWorld;
			int num15 = -1;
			bool flag10 = false;
			switch (movingSurfaceContactPoint)
			{
			case GTPlayer.MovingSurfaceContactPoint.NONE:
				if (flag6)
				{
					this.exitMovingSurface = true;
				}
				num15 = -1;
				break;
			case GTPlayer.MovingSurfaceContactPoint.RIGHT:
				num15 = num4;
				flag10 = flag2;
				vector25 = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.LEFT:
				num15 = num5;
				flag10 = flag3;
				vector25 = GorillaTagger.Instance.offlineVRRig.leftHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.BODY:
				num15 = num14;
				flag10 = flag8;
				vector25 = GorillaTagger.Instance.offlineVRRig.bodyTransform.position;
				break;
			}
			if (!flag10)
			{
				this.lastMonkeBlock = null;
			}
			if (num15 != this.lastMovingSurfaceID || this.lastMovingSurfaceContact != movingSurfaceContactPoint || flag10 != this.wasMovingSurfaceMonkeBlock)
			{
				if (num15 == -1)
				{
					if (Time.frameCount - this.lastAttachedToMovingSurfaceFrame > 3)
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (flag10)
				{
					if (this.lastMonkeBlock != null)
					{
						VRRig.AttachLocalPlayerToMovingSurface(num15, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, this.lastMonkeBlock.transform.InverseTransformPoint(vector25), flag10);
						this.lastMovingSurfaceID = num15;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (MovingSurfaceManager.instance != null)
				{
					MovingSurface movingSurface;
					if (MovingSurfaceManager.instance.TryGetMovingSurface(num15, out movingSurface))
					{
						VRRig.AttachLocalPlayerToMovingSurface(num15, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, movingSurface.transform.InverseTransformPoint(vector25), flag10);
						this.lastMovingSurfaceID = num15;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else
				{
					VRRig.DetachLocalPlayerFromMovingSurface();
					this.lastMovingSurfaceID = -1;
				}
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE && movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			this.lastMovingSurfaceContact = movingSurfaceContactPoint;
			this.wasMovingSurfaceMonkeBlock = flag10;
			if (this.activeSizeChangerSettings != null)
			{
				if (this.activeSizeChangerSettings.ExpireOnDistance > 0f && Vector3.Distance(base.transform.position, this.activeSizeChangerSettings.WorldPosition) > this.activeSizeChangerSettings.ExpireOnDistance)
				{
					this.SetNativeScale(null);
				}
				if (this.activeSizeChangerSettings.ExpireAfterSeconds > 0f && Time.time - this.activeSizeChangerSettings.ActivationTime > this.activeSizeChangerSettings.ExpireAfterSeconds)
				{
					this.SetNativeScale(null);
				}
			}
		}

		// Token: 0x060050F4 RID: 20724 RVA: 0x00185368 File Offset: 0x00183568
		private float ApplyNativeScaleAdjustment(float adjustedMagnitude)
		{
			if (this.nativeScale > 0f && this.nativeScale != 1f)
			{
				return adjustedMagnitude *= this.nativeScaleMagnitudeAdjustmentFactor.Evaluate(this.nativeScale);
			}
			return adjustedMagnitude;
		}

		// Token: 0x060050F5 RID: 20725 RVA: 0x0018539C File Offset: 0x0018359C
		private float RotateWithSurface(Quaternion rotationDelta, Vector3 pivot)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			QuaternionUtil.DecomposeSwingTwist(rotationDelta, Vector3.up, out quaternion, out quaternion2);
			float num = quaternion2.eulerAngles.y;
			if (num > 270f)
			{
				num -= 360f;
			}
			else if (num > 90f)
			{
				num -= 180f;
			}
			if (Mathf.Abs(num) < 90f * this.calcDeltaTime)
			{
				this.turnParent.transform.RotateAround(pivot, base.transform.up, num);
				return num;
			}
			return 0f;
		}

		// Token: 0x060050F6 RID: 20726 RVA: 0x00185420 File Offset: 0x00183620
		private void stuckHandsCheckFixedUpdate()
		{
			this.stuckLeft = !this.controllerState.LeftValid || (this.isLeftHandColliding && (this.GetCurrentLeftHandPosition() - this.GetLastLeftHandPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (this.GetCurrentLeftHandPosition() - this.headCollider.transform.position).normalized, (this.GetCurrentLeftHandPosition() - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value));
			this.stuckRight = !this.controllerState.RightValid || (this.isRightHandColliding && (this.GetCurrentRightHandPosition() - this.GetLastRightHandPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (this.GetCurrentRightHandPosition() - this.headCollider.transform.position).normalized, (this.GetCurrentRightHandPosition() - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value));
		}

		// Token: 0x060050F7 RID: 20727 RVA: 0x001855A0 File Offset: 0x001837A0
		private void stuckHandsCheckLateUpdate(ref Vector3 finalLeftHandPosition, ref Vector3 finalRightHandPosition)
		{
			if (this.stuckLeft)
			{
				finalLeftHandPosition = this.GetCurrentLeftHandPosition();
				this.stuckLeft = (this.isLeftHandColliding = false);
			}
			if (this.stuckRight)
			{
				finalRightHandPosition = this.GetCurrentRightHandPosition();
				this.stuckRight = (this.isRightHandColliding = false);
			}
		}

		// Token: 0x060050F8 RID: 20728 RVA: 0x001855F8 File Offset: 0x001837F8
		private void handleClimbing(float deltaTime)
		{
			if (this.isClimbing && (this.inOverlay || this.climbHelper == null || this.currentClimbable == null || !this.currentClimbable.isActiveAndEnabled))
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			Vector3 vector = Vector3.zero;
			if (this.isClimbing && (this.currentClimber.transform.position - this.climbHelper.position).magnitude > 1f)
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			if (this.isClimbing)
			{
				this.playerRigidBody.velocity = Vector3.zero;
				this.climbHelper.localPosition = Vector3.MoveTowards(this.climbHelper.localPosition, this.climbHelperTargetPos, deltaTime * 12f);
				vector = this.currentClimber.transform.position - this.climbHelper.position;
				vector = ((vector.sqrMagnitude > this.maxArmLength * this.maxArmLength) ? (vector.normalized * this.maxArmLength) : vector);
				if (this.isClimbableMoving)
				{
					Quaternion quaternion = this.currentClimbable.transform.rotation * Quaternion.Inverse(this.lastClimbableRotation);
					this.RotateWithSurface(quaternion, this.currentClimber.handRoot.position);
					this.lastClimbableRotation = this.currentClimbable.transform.rotation;
				}
				this.playerRigidBody.MovePosition(this.playerRigidBody.position - vector);
				if (this.currentSwing)
				{
					this.currentSwing.lastGrabTime = Time.time;
				}
			}
		}

		// Token: 0x060050F9 RID: 20729 RVA: 0x001857B8 File Offset: 0x001839B8
		private Vector3 FirstHandIteration(Transform handTransform, Vector3 handOffset, Vector3 lastHandPosition, Vector3 boostVector, bool wasHandSlide, bool wasHandTouching, bool fullSlideOverride, out Vector3 pushDisplacement, ref float handSlipPercentage, ref bool handSlide, ref Vector3 slideNormal, ref bool handColliding, ref int materialTouchIndex, ref GorillaSurfaceOverride touchedOverride, bool skipCollisionChecks, bool hitMovingSurface)
		{
			Vector3 vector = this.GetCurrentHandPosition(handTransform, handOffset) + this.movingSurfaceOffset;
			Vector3 vector2 = vector;
			Vector3 vector3 = vector - lastHandPosition;
			if (!this.didAJump && wasHandSlide && Vector3.Dot(slideNormal, Vector3.up) > 0f)
			{
				vector3 += Vector3.Project(-this.slideAverageNormal * this.stickDepth * this.scale, Vector3.down);
			}
			float num = this.minimumRaycastDistance * this.scale;
			if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				num = (this.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.scale;
			}
			Vector3 vector4 = Vector3.zero;
			if (hitMovingSurface && !this.exitMovingSurface)
			{
				vector4 = Vector3.Project(-this.lastMovingSurfaceHit.normal * (this.stickDepth * this.scale), Vector3.down);
				if (this.scale < 0.5f)
				{
					Vector3 normalized = this.MovingSurfaceMovement().normalized;
					if (normalized != Vector3.zero)
					{
						float num2 = Vector3.Dot(Vector3.up, normalized);
						if ((double)num2 > 0.9 || (double)num2 < -0.9)
						{
							vector4 *= 6f;
							num *= 1.1f;
						}
					}
				}
			}
			Vector3 vector5;
			float num3;
			if (this.IterativeCollisionSphereCast(lastHandPosition, num, vector3 + vector4, boostVector, out vector5, true, out num3, out this.tempHitInfo, fullSlideOverride) && !skipCollisionChecks && !this.InReportMenu)
			{
				if (wasHandTouching && num3 <= this.defaultSlideFactor && !boostVector.IsLongerThan(0f))
				{
					vector2 = lastHandPosition;
					pushDisplacement = lastHandPosition - vector;
				}
				else
				{
					vector2 = vector5;
					pushDisplacement = vector5 - vector;
				}
				handSlipPercentage = num3;
				handSlide = num3 > this.iceThreshold;
				slideNormal = this.tempHitInfo.normal;
				handColliding = true;
				materialTouchIndex = this.currentMaterialIndex;
				touchedOverride = this.currentOverride;
				this.lastHitInfoHand = this.tempHitInfo;
			}
			else
			{
				pushDisplacement = Vector3.zero;
				handSlipPercentage = 0f;
				handSlide = false;
				slideNormal = Vector3.up;
				handColliding = false;
				materialTouchIndex = 0;
				touchedOverride = null;
			}
			return vector2;
		}

		// Token: 0x060050FA RID: 20730 RVA: 0x00185A30 File Offset: 0x00183C30
		private Vector3 FinalHandPosition(Transform handTransform, Vector3 handOffset, Vector3 lastHandPosition, Vector3 boostVector, bool bothTouching, bool isHandTouching, out bool handColliding, bool isHandSlide, out bool handSlide, int currentMaterialTouchIndex, out int materialTouchIndex, GorillaSurfaceOverride currentSurface, out GorillaSurfaceOverride touchedOverride, bool skipCollisionChecks, ref RaycastHit hitInfoCopy)
		{
			handColliding = isHandTouching;
			handSlide = isHandSlide;
			materialTouchIndex = currentMaterialTouchIndex;
			touchedOverride = currentSurface;
			Vector3 vector = this.GetCurrentHandPosition(handTransform, handOffset) - lastHandPosition;
			float num = this.minimumRaycastDistance * this.scale;
			if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				num = (this.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.scale;
			}
			Vector3 vector2;
			float num2;
			if (this.IterativeCollisionSphereCast(lastHandPosition, num, vector, boostVector, out vector2, bothTouching, out num2, out this.junkHit, false) && !skipCollisionChecks)
			{
				handColliding = true;
				handSlide = num2 > this.iceThreshold;
				materialTouchIndex = this.currentMaterialIndex;
				touchedOverride = this.currentOverride;
				this.lastHitInfoHand = this.junkHit;
				hitInfoCopy = this.junkHit;
				return vector2;
			}
			return this.GetCurrentHandPosition(handTransform, handOffset);
		}

		// Token: 0x060050FB RID: 20731 RVA: 0x00185B14 File Offset: 0x00183D14
		private bool IterativeCollisionSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, Vector3 boostVector, out Vector3 endPosition, bool singleHand, out float slipPercentage, out RaycastHit iterativeHitInfo, bool fullSlide)
		{
			slipPercentage = this.defaultSlideFactor;
			if (!this.CollisionsSphereCast(startPosition, sphereRadius, movementVector, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				endPosition = Vector3.zero;
				return false;
			}
			this.firstPosition = endPosition;
			iterativeHitInfo = this.tempIterativeHit;
			this.slideFactor = this.GetSlidePercentage(iterativeHitInfo);
			slipPercentage = ((this.slideFactor != this.defaultSlideFactor) ? this.slideFactor : ((!singleHand) ? this.defaultSlideFactor : 0.001f));
			if (fullSlide)
			{
				slipPercentage = 1f;
			}
			this.movementToProjectedAboveCollisionPlane = Vector3.ProjectOnPlane(startPosition + movementVector - this.firstPosition, iterativeHitInfo.normal) * slipPercentage;
			Vector3 vector = Vector3.zero;
			if (boostVector.IsLongerThan(0f))
			{
				vector = Vector3.ProjectOnPlane(boostVector, iterativeHitInfo.normal);
				this.movementToProjectedAboveCollisionPlane += vector;
				this.CollisionsSphereCast(this.firstPosition, sphereRadius, vector, out endPosition, out this.tempIterativeHit);
				this.firstPosition = endPosition;
			}
			if (this.CollisionsSphereCast(this.firstPosition, sphereRadius, this.movementToProjectedAboveCollisionPlane, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			if (this.CollisionsSphereCast(this.movementToProjectedAboveCollisionPlane + this.firstPosition, sphereRadius, startPosition + movementVector + vector - (this.movementToProjectedAboveCollisionPlane + this.firstPosition), out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			endPosition = Vector3.zero;
			return false;
		}

		// Token: 0x060050FC RID: 20732 RVA: 0x00185CD0 File Offset: 0x00183ED0
		private bool CollisionsSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 finalPosition, out RaycastHit collisionsHitInfo)
		{
			this.MaxSphereSizeForNoOverlap(sphereRadius, startPosition, false, out this.maxSphereSize1);
			bool flag = false;
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.SphereCastNonAlloc(startPosition, this.maxSphereSize1, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int i = 0; i < this.bufferCount; i++)
				{
					if (this.tempHitInfo.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < this.tempHitInfo.distance))
					{
						flag = true;
						this.tempHitInfo = this.rayCastNonAllocColliders[i];
					}
				}
			}
			if (flag)
			{
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = collisionsHitInfo.point + collisionsHitInfo.normal * sphereRadius;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int j = 0; j < this.bufferCount; j++)
					{
						if (this.rayCastNonAllocColliders[j].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[j];
						}
					}
					finalPosition = startPosition + movementVector.normalized * this.tempHitInfo.distance;
				}
				this.MaxSphereSizeForNoOverlap(sphereRadius, finalPosition, false, out this.maxSphereSize2);
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.SphereCastNonAlloc(startPosition, Mathf.Min(this.maxSphereSize1, this.maxSphereSize2), (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int k = 0; k < this.bufferCount; k++)
					{
						if (this.rayCastNonAllocColliders[k].collider != null && this.rayCastNonAllocColliders[k].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[k];
						}
					}
					finalPosition = startPosition + this.tempHitInfo.distance * (finalPosition - startPosition).normalized;
					collisionsHitInfo = this.tempHitInfo;
				}
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int l = 0; l < this.bufferCount; l++)
					{
						if (this.rayCastNonAllocColliders[l].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[l];
						}
					}
					collisionsHitInfo = this.tempHitInfo;
					finalPosition = startPosition;
				}
				return true;
			}
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.RaycastNonAlloc(startPosition, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int m = 0; m < this.bufferCount; m++)
				{
					if (this.rayCastNonAllocColliders[m].collider != null && this.rayCastNonAllocColliders[m].distance < this.tempHitInfo.distance)
					{
						this.tempHitInfo = this.rayCastNonAllocColliders[m];
					}
				}
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = startPosition;
				return true;
			}
			finalPosition = startPosition + movementVector;
			collisionsHitInfo = default(RaycastHit);
			return false;
		}

		// Token: 0x060050FD RID: 20733 RVA: 0x0018618E File Offset: 0x0018438E
		public bool IsHandTouching(bool forLeftHand)
		{
			if (forLeftHand)
			{
				return this.wasLeftHandColliding;
			}
			return this.wasRightHandColliding;
		}

		// Token: 0x060050FE RID: 20734 RVA: 0x001861A0 File Offset: 0x001843A0
		public bool IsHandSliding(bool forLeftHand)
		{
			if (forLeftHand)
			{
				return this.wasLeftHandSliding || this.isLeftHandSliding;
			}
			return this.wasRightHandSliding || this.isRightHandSliding;
		}

		// Token: 0x060050FF RID: 20735 RVA: 0x001861C8 File Offset: 0x001843C8
		public float GetSlidePercentage(RaycastHit raycastHit)
		{
			this.currentOverride = raycastHit.collider.gameObject.GetComponent<GorillaSurfaceOverride>();
			BasePlatform component = raycastHit.collider.gameObject.GetComponent<BasePlatform>();
			if (component != null)
			{
				this.currentPlatform = component;
			}
			if (this.currentOverride != null)
			{
				if (this.currentOverride.slidePercentageOverride >= 0f)
				{
					return this.currentOverride.slidePercentageOverride;
				}
				this.currentMaterialIndex = this.currentOverride.overrideIndex;
				if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
				{
					return this.FreezeTagSlidePercentage();
				}
				if (!this.materialData[this.currentMaterialIndex].overrideSlidePercent)
				{
					return this.defaultSlideFactor;
				}
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			else
			{
				this.meshCollider = raycastHit.collider as MeshCollider;
				if (this.meshCollider == null || this.meshCollider.sharedMesh == null || this.meshCollider.convex)
				{
					return this.defaultSlideFactor;
				}
				this.collidedMesh = this.meshCollider.sharedMesh;
				if (!this.meshTrianglesDict.TryGetValue(this.collidedMesh, out this.sharedMeshTris))
				{
					this.sharedMeshTris = this.collidedMesh.triangles;
					this.meshTrianglesDict.Add(this.collidedMesh, (int[])this.sharedMeshTris.Clone());
				}
				this.vertex1 = this.sharedMeshTris[raycastHit.triangleIndex * 3];
				this.vertex2 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 1];
				this.vertex3 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 2];
				this.slideRenderer = raycastHit.collider.GetComponent<Renderer>();
				if (this.slideRenderer != null)
				{
					this.slideRenderer.GetSharedMaterials(this.tempMaterialArray);
				}
				else
				{
					this.tempMaterialArray.Clear();
				}
				if (this.tempMaterialArray.Count > 1)
				{
					for (int i = 0; i < this.tempMaterialArray.Count; i++)
					{
						this.collidedMesh.GetTriangles(this.trianglesList, i);
						int j = 0;
						while (j < this.trianglesList.Count)
						{
							if (this.trianglesList[j] == this.vertex1 && this.trianglesList[j + 1] == this.vertex2 && this.trianglesList[j + 2] == this.vertex3)
							{
								this.findMatName = this.tempMaterialArray[i].name;
								if (this.findMatName.EndsWith("Uber"))
								{
									string text = this.findMatName;
									this.findMatName = text.Substring(0, text.Length - 4);
								}
								this.foundMatData = this.materialData.Find((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								this.currentMaterialIndex = this.materialData.FindIndex((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								if (this.currentMaterialIndex == -1)
								{
									this.currentMaterialIndex = 0;
								}
								if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
								{
									return this.FreezeTagSlidePercentage();
								}
								if (!this.foundMatData.overrideSlidePercent)
								{
									return this.defaultSlideFactor;
								}
								return this.foundMatData.slidePercent;
							}
							else
							{
								j += 3;
							}
						}
					}
				}
				else if (this.tempMaterialArray.Count > 0)
				{
					this.findMatName = this.tempMaterialArray[0].name;
					if (this.findMatName.EndsWith("Uber"))
					{
						string text = this.findMatName;
						this.findMatName = text.Substring(0, text.Length - 4);
					}
					this.foundMatData = this.materialData.Find((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
					this.currentMaterialIndex = this.materialData.FindIndex((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
					if (this.currentMaterialIndex == -1)
					{
						this.currentMaterialIndex = 0;
					}
					if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
					{
						return this.FreezeTagSlidePercentage();
					}
					if (!this.foundMatData.overrideSlidePercent)
					{
						return this.defaultSlideFactor;
					}
					return this.foundMatData.slidePercent;
				}
				this.currentMaterialIndex = 0;
				return this.defaultSlideFactor;
			}
		}

		// Token: 0x06005100 RID: 20736 RVA: 0x00186624 File Offset: 0x00184824
		public bool IsTouchingMovingSurface(Vector3 rayOrigin, RaycastHit raycastHit, out int movingSurfaceId, out bool sideTouch, out bool isMonkeBlock)
		{
			movingSurfaceId = -1;
			sideTouch = false;
			isMonkeBlock = false;
			float num = Vector3.Dot(rayOrigin - raycastHit.point, Vector3.up);
			if (num < -0.3f)
			{
				return false;
			}
			if (num < 0f)
			{
				sideTouch = true;
			}
			if (raycastHit.collider == null)
			{
				return false;
			}
			MovingSurface component = raycastHit.collider.GetComponent<MovingSurface>();
			if (component != null)
			{
				isMonkeBlock = false;
				movingSurfaceId = component.GetID();
				return true;
			}
			if (!BuilderTable.IsLocalPlayerInBuilderZone())
			{
				return false;
			}
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(raycastHit.collider);
			if (builderPieceFromCollider != null && builderPieceFromCollider.IsPieceMoving())
			{
				isMonkeBlock = true;
				movingSurfaceId = builderPieceFromCollider.pieceId;
				this.lastMonkeBlock = builderPieceFromCollider;
				return true;
			}
			sideTouch = false;
			return false;
		}

		// Token: 0x06005101 RID: 20737 RVA: 0x001866E0 File Offset: 0x001848E0
		public void Turn(float degrees)
		{
			Vector3 vector = this.headCollider.transform.position;
			bool flag = this.isRightHandColliding || this.rightHandHolding;
			bool flag2 = this.isLeftHandColliding || this.leftHandHolding;
			if (flag != flag2 && flag)
			{
				vector = this.rightControllerTransform.position;
			}
			if (flag != flag2 && flag2)
			{
				vector = this.leftControllerTransform.position;
			}
			this.turnParent.transform.RotateAround(vector, base.transform.up, degrees);
			this.degreesTurnedThisFrame = degrees;
			this.averagedVelocity = Vector3.zero;
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Quaternion.Euler(0f, degrees, 0f) * this.velocityHistory[i];
				this.averagedVelocity += this.velocityHistory[i];
			}
			this.averagedVelocity /= (float)this.velocityHistorySize;
		}

		// Token: 0x06005102 RID: 20738 RVA: 0x001867F4 File Offset: 0x001849F4
		public void BeginClimbing(GorillaClimbable climbable, GorillaHandClimber hand, GorillaClimbableRef climbableRef = null)
		{
			if (this.currentClimber != null)
			{
				this.EndClimbing(this.currentClimber, true, false);
			}
			try
			{
				Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb = climbable.onBeforeClimb;
				if (onBeforeClimb != null)
				{
					onBeforeClimb(hand, climbableRef);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
			Rigidbody rigidbody;
			climbable.TryGetComponent<Rigidbody>(out rigidbody);
			this.VerifyClimbHelper();
			this.climbHelper.SetParent(climbable.transform);
			this.climbHelper.position = hand.transform.position;
			Vector3 localPosition = this.climbHelper.localPosition;
			if (climbable.snapX)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|380_0(ref localPosition.x, climbable.maxDistanceSnap);
			}
			if (climbable.snapY)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|380_0(ref localPosition.y, climbable.maxDistanceSnap);
			}
			if (climbable.snapZ)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|380_0(ref localPosition.z, climbable.maxDistanceSnap);
			}
			this.climbHelperTargetPos = localPosition;
			climbable.isBeingClimbed = true;
			hand.isClimbing = true;
			this.currentClimbable = climbable;
			this.currentClimber = hand;
			this.isClimbing = true;
			if (climbable.climbOnlyWhileSmall)
			{
				BuilderPiece componentInParent = climbable.GetComponentInParent<BuilderPiece>();
				if (componentInParent != null && componentInParent.IsPieceMoving())
				{
					this.isClimbableMoving = true;
					this.lastClimbableRotation = climbable.transform.rotation;
				}
				else
				{
					this.isClimbableMoving = false;
				}
			}
			else
			{
				this.isClimbableMoving = false;
			}
			GorillaRopeSegment gorillaRopeSegment;
			GorillaZipline gorillaZipline;
			PhotonView photonView;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (climbable.TryGetComponent<GorillaRopeSegment>(out gorillaRopeSegment) && gorillaRopeSegment.swing)
			{
				this.currentSwing = gorillaRopeSegment.swing;
				this.currentSwing.AttachLocalPlayer(hand.xrNode, climbable.transform, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.transform.parent && climbable.transform.parent.TryGetComponent<GorillaZipline>(out gorillaZipline))
			{
				this.currentZipline = gorillaZipline;
			}
			else if (climbable.TryGetComponent<PhotonView>(out photonView))
			{
				VRRig.AttachLocalPlayerToPhotonView(photonView, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef))
			{
				VRRig.AttachLocalPlayerToPhotonView(photonViewXSceneRef.photonView, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			GorillaTagger.Instance.StartVibration(this.currentClimber.xrNode == XRNode.LeftHand, 0.6f, 0.06f);
			if (climbable.clip)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(climbable.clip, hand.xrNode == XRNode.LeftHand);
			}
		}

		// Token: 0x06005103 RID: 20739 RVA: 0x00186A60 File Offset: 0x00184C60
		private void VerifyClimbHelper()
		{
			if (this.climbHelper == null || this.climbHelper.gameObject == null)
			{
				this.climbHelper = new GameObject("Climb Helper").transform;
			}
		}

		// Token: 0x06005104 RID: 20740 RVA: 0x00186A98 File Offset: 0x00184C98
		public GorillaVelocityTracker GetInteractPointVelocityTracker(bool isRightHand)
		{
			if (!isRightHand)
			{
				return this.leftInteractPointVelocityTracker;
			}
			return this.rightInteractPointVelocityTracker;
		}

		// Token: 0x06005105 RID: 20741 RVA: 0x00186AAC File Offset: 0x00184CAC
		public void EndClimbing(GorillaHandClimber hand, bool startingNewClimb, bool doDontReclimb = false)
		{
			if (hand != this.currentClimber)
			{
				return;
			}
			hand.SetCanRelease(true);
			if (!startingNewClimb)
			{
				this.enablePlayerGravity(true);
			}
			Rigidbody rigidbody = null;
			if (this.currentClimbable)
			{
				this.currentClimbable.TryGetComponent<Rigidbody>(out rigidbody);
				this.currentClimbable.isBeingClimbed = false;
			}
			Vector3 vector = Vector3.zero;
			if (this.currentClimber)
			{
				this.currentClimber.isClimbing = false;
				if (doDontReclimb)
				{
					this.currentClimber.dontReclimbLast = this.currentClimbable;
				}
				else
				{
					this.currentClimber.dontReclimbLast = null;
				}
				this.currentClimber.queuedToBecomeValidToGrabAgain = true;
				this.currentClimber.lastAutoReleasePos = this.currentClimber.handRoot.localPosition;
				if (!startingNewClimb && this.currentClimbable)
				{
					GorillaVelocityTracker gorillaVelocityTracker = ((this.currentClimber.xrNode == XRNode.LeftHand) ? this.leftInteractPointVelocityTracker : this.rightInteractPointVelocityTracker);
					if (rigidbody)
					{
						this.playerRigidBody.velocity = rigidbody.velocity;
					}
					else if (this.currentSwing)
					{
						this.playerRigidBody.velocity = this.currentSwing.velocityTracker.GetAverageVelocity(true, 0.25f, false);
					}
					else if (this.currentZipline)
					{
						this.playerRigidBody.velocity = this.currentZipline.GetCurrentDirection() * this.currentZipline.currentSpeed;
					}
					else
					{
						this.playerRigidBody.velocity = Vector3.zero;
					}
					vector = this.turnParent.transform.rotation * -gorillaVelocityTracker.GetAverageVelocity(false, 0.1f, true) * this.scale;
					vector = Vector3.ClampMagnitude(vector, 5.5f * this.scale);
					this.playerRigidBody.AddForce(vector, ForceMode.VelocityChange);
				}
			}
			if (this.currentSwing)
			{
				this.currentSwing.DetachLocalPlayer();
			}
			PhotonView photonView;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (this.currentClimbable.TryGetComponent<PhotonView>(out photonView) || this.currentClimbable.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef) || this.currentClimbable.IsPlayerAttached)
			{
				VRRig.DetachLocalPlayerFromPhotonView();
			}
			if (!startingNewClimb && vector.magnitude > 2f && this.currentClimbable && this.currentClimbable.clipOnFullRelease)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(this.currentClimbable.clipOnFullRelease, hand.xrNode == XRNode.LeftHand);
			}
			this.currentClimbable = null;
			this.currentClimber = null;
			this.currentSwing = null;
			this.currentZipline = null;
			this.isClimbing = false;
		}

		// Token: 0x06005106 RID: 20742 RVA: 0x00186D44 File Offset: 0x00184F44
		private void enablePlayerGravity(bool useGravity)
		{
			this.playerRigidBody.useGravity = useGravity;
		}

		// Token: 0x06005107 RID: 20743 RVA: 0x00186D52 File Offset: 0x00184F52
		public void SetVelocity(Vector3 velocity)
		{
			this.playerRigidBody.velocity = velocity;
		}

		// Token: 0x06005108 RID: 20744 RVA: 0x00186D60 File Offset: 0x00184F60
		private void StoreVelocities()
		{
			this.velocityIndex = (this.velocityIndex + 1) % this.velocityHistorySize;
			this.currentVelocity = (base.transform.position - this.lastPosition - this.MovingSurfaceMovement()) / this.calcDeltaTime;
			this.velocityHistory[this.velocityIndex] = this.currentVelocity;
			this.averagedVelocity = Vector3.zero;
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.averagedVelocity += this.velocityHistory[i];
			}
			this.averagedVelocity /= (float)this.velocityHistorySize;
			this.lastPosition = base.transform.position;
		}

		// Token: 0x06005109 RID: 20745 RVA: 0x00186E30 File Offset: 0x00185030
		private void AntiTeleportTechnology()
		{
			if ((this.headCollider.transform.position - this.lastHeadPosition).magnitude >= this.teleportThresholdNoVel + this.playerRigidBody.velocity.magnitude * this.calcDeltaTime)
			{
				base.transform.position = base.transform.position + this.lastHeadPosition - this.headCollider.transform.position;
			}
		}

		// Token: 0x0600510A RID: 20746 RVA: 0x00186EBC File Offset: 0x001850BC
		private bool MaxSphereSizeForNoOverlap(float testRadius, Vector3 checkPosition, bool ignoreOneWay, out float overlapRadiusTest)
		{
			overlapRadiusTest = testRadius;
			this.overlapAttempts = 0;
			int num = 100;
			while (this.overlapAttempts < num && overlapRadiusTest > testRadius * 0.75f)
			{
				this.ClearColliderBuffer(ref this.overlapColliders);
				this.bufferCount = Physics.OverlapSphereNonAlloc(checkPosition, overlapRadiusTest, this.overlapColliders, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				if (ignoreOneWay)
				{
					int num2 = 0;
					for (int i = 0; i < this.bufferCount; i++)
					{
						if (this.overlapColliders[i].CompareTag("NoCrazyCheck"))
						{
							num2++;
						}
					}
					if (num2 == this.bufferCount)
					{
						return true;
					}
				}
				if (this.bufferCount <= 0)
				{
					overlapRadiusTest *= 0.995f;
					return true;
				}
				overlapRadiusTest = Mathf.Lerp(testRadius, 0f, (float)this.overlapAttempts / (float)num);
				this.overlapAttempts++;
			}
			return false;
		}

		// Token: 0x0600510B RID: 20747 RVA: 0x00186F9C File Offset: 0x0018519C
		private bool CrazyCheck2(float sphereSize, Vector3 startPosition)
		{
			for (int i = 0; i < this.crazyCheckVectors.Length; i++)
			{
				if (this.NonAllocRaycast(startPosition, startPosition + this.crazyCheckVectors[i] * sphereSize) > 0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600510C RID: 20748 RVA: 0x00186FE4 File Offset: 0x001851E4
		private int NonAllocRaycast(Vector3 startPosition, Vector3 endPosition)
		{
			Vector3 vector = endPosition - startPosition;
			int num = Physics.RaycastNonAlloc(startPosition, vector, this.rayCastNonAllocColliders, vector.magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (!this.rayCastNonAllocColliders[i].collider.gameObject.CompareTag("NoCrazyCheck"))
				{
					num2++;
				}
			}
			return num2;
		}

		// Token: 0x0600510D RID: 20749 RVA: 0x00187050 File Offset: 0x00185250
		private void ClearColliderBuffer(ref Collider[] colliders)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i] = null;
			}
		}

		// Token: 0x0600510E RID: 20750 RVA: 0x00187074 File Offset: 0x00185274
		private void ClearRaycasthitBuffer(ref RaycastHit[] raycastHits)
		{
			for (int i = 0; i < raycastHits.Length; i++)
			{
				raycastHits[i] = this.emptyHit;
			}
		}

		// Token: 0x0600510F RID: 20751 RVA: 0x0018709E File Offset: 0x0018529E
		private Vector3 MovingSurfaceMovement()
		{
			return this.refMovement + this.movingSurfaceOffset;
		}

		// Token: 0x06005110 RID: 20752 RVA: 0x001870B4 File Offset: 0x001852B4
		private static bool ComputeLocalHitPoint(RaycastHit hit, out Vector3 localHitPoint)
		{
			if (hit.collider == null || hit.point.sqrMagnitude < 0.001f)
			{
				localHitPoint = Vector3.zero;
				return false;
			}
			localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
			return true;
		}

		// Token: 0x06005111 RID: 20753 RVA: 0x00187112 File Offset: 0x00185312
		private static bool ComputeWorldHitPoint(RaycastHit hit, Vector3 localPoint, out Vector3 worldHitPoint)
		{
			if (hit.collider == null)
			{
				worldHitPoint = Vector3.zero;
				return false;
			}
			worldHitPoint = hit.collider.transform.TransformPoint(localPoint);
			return true;
		}

		// Token: 0x06005112 RID: 20754 RVA: 0x0018714C File Offset: 0x0018534C
		private float ExtraVelMultiplier()
		{
			float num = 1f;
			if (this.leftHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHandSurfaceOverride.extraVelMultiplier);
			}
			if (this.rightHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHandSurfaceOverride.extraVelMultiplier);
			}
			return num;
		}

		// Token: 0x06005113 RID: 20755 RVA: 0x001871A0 File Offset: 0x001853A0
		private float ExtraVelMaxMultiplier()
		{
			float num = 1f;
			if (this.leftHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHandSurfaceOverride.extraVelMaxMultiplier);
			}
			if (this.rightHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHandSurfaceOverride.extraVelMaxMultiplier);
			}
			return num * this.scale;
		}

		// Token: 0x06005114 RID: 20756 RVA: 0x001871FD File Offset: 0x001853FD
		public void SetMaximumSlipThisFrame()
		{
			this.leftSlipSetToMaxFrameIdx = Time.frameCount;
			this.rightSlipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06005115 RID: 20757 RVA: 0x00187215 File Offset: 0x00185415
		public void SetLeftMaximumSlipThisFrame()
		{
			this.leftSlipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06005116 RID: 20758 RVA: 0x00187222 File Offset: 0x00185422
		public void SetRightMaximumSlipThisFrame()
		{
			this.rightSlipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06005117 RID: 20759 RVA: 0x0018722F File Offset: 0x0018542F
		public bool LeftSlipOverriddenToMax()
		{
			return this.leftSlipSetToMaxFrameIdx == Time.frameCount;
		}

		// Token: 0x06005118 RID: 20760 RVA: 0x0018723E File Offset: 0x0018543E
		public bool RightSlipOverriddenToMax()
		{
			return this.rightSlipSetToMaxFrameIdx == Time.frameCount;
		}

		// Token: 0x06005119 RID: 20761 RVA: 0x0018724D File Offset: 0x0018544D
		public void ChangeLayer(string layerName)
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.ChangeLayer(base.transform.parent, layerName);
			}
		}

		// Token: 0x0600511A RID: 20762 RVA: 0x00187274 File Offset: 0x00185474
		public void RestoreLayer()
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.RestoreOriginalLayers();
			}
		}

		// Token: 0x0600511B RID: 20763 RVA: 0x00187290 File Offset: 0x00185490
		public void OnEnterWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireInWater)
			{
				this.SetNativeScale(null);
			}
			if (playerCollider == this.headCollider)
			{
				if (!this.headOverlappingWaterVolumes.Contains(volume))
				{
					this.headOverlappingWaterVolumes.Add(volume);
					return;
				}
			}
			else if (playerCollider == this.bodyCollider && !this.bodyOverlappingWaterVolumes.Contains(volume))
			{
				this.bodyOverlappingWaterVolumes.Add(volume);
			}
		}

		// Token: 0x0600511C RID: 20764 RVA: 0x0018730A File Offset: 0x0018550A
		public void OnExitWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (playerCollider == this.headCollider)
			{
				this.headOverlappingWaterVolumes.Remove(volume);
				return;
			}
			if (playerCollider == this.bodyCollider)
			{
				this.bodyOverlappingWaterVolumes.Remove(volume);
			}
		}

		// Token: 0x0600511D RID: 20765 RVA: 0x00187344 File Offset: 0x00185544
		private bool GetSwimmingVelocityForHand(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, float dt, ref WaterVolume contactingWaterVolume, ref WaterVolume.SurfaceQuery waterSurface, out Vector3 swimmingVelocityChange)
		{
			contactingWaterVolume = null;
			this.bufferCount = Physics.OverlapSphereNonAlloc(endingHandPosition, this.minimumRaycastDistance, this.overlapColliders, this.waterLayer.value, QueryTriggerInteraction.Collide);
			if (this.bufferCount > 0)
			{
				float num = float.MinValue;
				for (int i = 0; i < this.bufferCount; i++)
				{
					WaterVolume component = this.overlapColliders[i].GetComponent<WaterVolume>();
					WaterVolume.SurfaceQuery surfaceQuery;
					if (component != null && component.GetSurfaceQueryForPoint(endingHandPosition, out surfaceQuery, false) && surfaceQuery.surfacePoint.y > num)
					{
						num = surfaceQuery.surfacePoint.y;
						contactingWaterVolume = component;
						waterSurface = surfaceQuery;
					}
				}
			}
			if (contactingWaterVolume != null)
			{
				Vector3 vector = endingHandPosition - startingHandPosition;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = this.playerRigidBody.transform.position - this.lastRigidbodyPosition;
				if (this.turnedThisFrame)
				{
					Vector3 vector4 = startingHandPosition - this.headCollider.transform.position;
					vector2 = Quaternion.AngleAxis(this.degreesTurnedThisFrame, Vector3.up) * vector4 - vector4;
				}
				float num2 = Vector3.Dot(vector - vector2 - vector3, palmForwardDirection);
				float num3 = 0f;
				if (num2 > 0f)
				{
					Plane surfacePlane = waterSurface.surfacePlane;
					float distanceToPoint = surfacePlane.GetDistanceToPoint(startingHandPosition);
					float distanceToPoint2 = surfacePlane.GetDistanceToPoint(endingHandPosition);
					if (distanceToPoint <= 0f && distanceToPoint2 <= 0f)
					{
						num3 = 1f;
					}
					else if (distanceToPoint > 0f && distanceToPoint2 <= 0f)
					{
						num3 = -distanceToPoint2 / (distanceToPoint - distanceToPoint2);
					}
					else if (distanceToPoint <= 0f && distanceToPoint2 > 0f)
					{
						num3 = -distanceToPoint / (distanceToPoint2 - distanceToPoint);
					}
					if (num3 > Mathf.Epsilon)
					{
						float resistance = this.liquidPropertiesList[(int)contactingWaterVolume.LiquidType].resistance;
						swimmingVelocityChange = -palmForwardDirection * num2 * 2f * resistance * num3;
						Vector3 forward = this.mainCamera.transform.forward;
						if (forward.y < 0f)
						{
							Vector3 vector5 = forward.x0z();
							float magnitude = vector5.magnitude;
							vector5 /= magnitude;
							float num4 = Vector3.Dot(swimmingVelocityChange, vector5);
							if (num4 > 0f)
							{
								Vector3 vector6 = vector5 * num4;
								swimmingVelocityChange = swimmingVelocityChange - vector6 + vector6 * magnitude + Vector3.up * forward.y * num4;
							}
						}
						return true;
					}
				}
			}
			swimmingVelocityChange = Vector3.zero;
			return false;
		}

		// Token: 0x0600511E RID: 20766 RVA: 0x00187600 File Offset: 0x00185800
		private bool CheckWaterSurfaceJump(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, Vector3 handAvgVelocity, PlayerSwimmingParameters parameters, WaterVolume contactingWaterVolume, WaterVolume.SurfaceQuery waterSurface, out Vector3 jumpVelocity)
		{
			if (contactingWaterVolume != null)
			{
				Plane surfacePlane = waterSurface.surfacePlane;
				bool flag = handAvgVelocity.sqrMagnitude > parameters.waterSurfaceJumpHandSpeedThreshold * parameters.waterSurfaceJumpHandSpeedThreshold;
				if (surfacePlane.GetSide(startingHandPosition) && !surfacePlane.GetSide(endingHandPosition) && flag)
				{
					float num = Vector3.Dot(palmForwardDirection, -waterSurface.surfaceNormal);
					float num2 = Vector3.Dot(handAvgVelocity.normalized, -waterSurface.surfaceNormal);
					float num3 = parameters.waterSurfaceJumpPalmFacingCurve.Evaluate(Mathf.Clamp(num, 0.01f, 0.99f));
					float num4 = parameters.waterSurfaceJumpHandVelocityFacingCurve.Evaluate(Mathf.Clamp(num2, 0.01f, 0.99f));
					jumpVelocity = -handAvgVelocity * parameters.waterSurfaceJumpAmount * num3 * num4;
					return true;
				}
			}
			jumpVelocity = Vector3.zero;
			return false;
		}

		// Token: 0x0600511F RID: 20767 RVA: 0x001876F9 File Offset: 0x001858F9
		private bool TryNormalize(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > eps)
			{
				normalized = input / magnitude;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x06005120 RID: 20768 RVA: 0x00187726 File Offset: 0x00185926
		private bool TryNormalizeDown(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > 1f)
			{
				normalized = input / magnitude;
				return true;
			}
			if (magnitude >= eps)
			{
				normalized = input;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x06005121 RID: 20769 RVA: 0x00187768 File Offset: 0x00185968
		private float FreezeTagSlidePercentage()
		{
			if (this.materialData[this.currentMaterialIndex].overrideSlidePercent && this.materialData[this.currentMaterialIndex].slidePercent > this.freezeTagHandSlidePercent)
			{
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			return this.freezeTagHandSlidePercent;
		}

		// Token: 0x06005122 RID: 20770 RVA: 0x001877C8 File Offset: 0x001859C8
		private void OnCollisionStay(global::UnityEngine.Collision collision)
		{
			this.bodyCollisionContactsCount = collision.GetContacts(this.bodyCollisionContacts);
			float num = -1f;
			for (int i = 0; i < this.bodyCollisionContactsCount; i++)
			{
				float num2 = Vector3.Dot(this.bodyCollisionContacts[i].normal, Vector3.up);
				if (num2 > num)
				{
					this.bodyGroundContact = this.bodyCollisionContacts[i];
					num = num2;
				}
			}
			float num3 = 0.5f;
			if (num > num3)
			{
				this.bodyGroundContactTime = Time.time;
			}
		}

		// Token: 0x06005123 RID: 20771 RVA: 0x00187850 File Offset: 0x00185A50
		public async void DoLaunch(Vector3 velocity)
		{
			if (this.isClimbing)
			{
				this.EndClimbing(this.CurrentClimber, false, false);
			}
			this.playerRigidBody.velocity = velocity;
			this.disableMovement = true;
			await Task.Delay(1);
			this.disableMovement = false;
		}

		// Token: 0x06005124 RID: 20772 RVA: 0x0018788F File Offset: 0x00185A8F
		private void OnEnable()
		{
			RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.OnJoinedRoom));
		}

		// Token: 0x06005125 RID: 20773 RVA: 0x001878B1 File Offset: 0x00185AB1
		private void OnJoinedRoom()
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireOnRoomJoin)
			{
				this.SetNativeScale(null);
			}
		}

		// Token: 0x06005126 RID: 20774 RVA: 0x001878CF File Offset: 0x00185ACF
		private void OnDisable()
		{
			RoomSystem.JoinedRoomEvent = (Action)Delegate.Remove(RoomSystem.JoinedRoomEvent, new Action(this.OnJoinedRoom));
		}

		// Token: 0x06005127 RID: 20775 RVA: 0x001878F4 File Offset: 0x00185AF4
		internal void AddHandHold(Transform objectHeld, Vector3 localPositionHeld, GorillaGrabber grabber, bool rightHand, bool rotatePlayerWhenHeld, out Vector3 grabbedVelocity)
		{
			if (!this.leftHandHolding && !this.rightHandHolding)
			{
				grabbedVelocity = -this.bodyCollider.attachedRigidbody.velocity;
				this.playerRigidBody.AddForce(grabbedVelocity, ForceMode.VelocityChange);
			}
			else
			{
				grabbedVelocity = Vector3.zero;
			}
			this.secondaryHandHold = this.activeHandHold;
			Vector3 position = grabber.transform.position;
			this.activeHandHold = new GTPlayer.HandHoldState
			{
				grabber = grabber,
				objectHeld = objectHeld,
				localPositionHeld = localPositionHeld,
				localRotationalOffset = grabber.transform.rotation.eulerAngles.y - objectHeld.rotation.eulerAngles.y,
				applyRotation = rotatePlayerWhenHeld
			};
			if (rightHand)
			{
				this.rightHandHolding = true;
			}
			else
			{
				this.leftHandHolding = true;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06005128 RID: 20776 RVA: 0x001879E4 File Offset: 0x00185BE4
		internal void RemoveHandHold(GorillaGrabber grabber, bool rightHand)
		{
			this.activeHandHold.objectHeld == grabber;
			if (this.activeHandHold.grabber == grabber)
			{
				this.activeHandHold = this.secondaryHandHold;
			}
			this.secondaryHandHold = default(GTPlayer.HandHoldState);
			if (rightHand)
			{
				this.rightHandHolding = false;
			}
			else
			{
				this.leftHandHolding = false;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06005129 RID: 20777 RVA: 0x00187A4C File Offset: 0x00185C4C
		private void OnChangeActiveHandhold()
		{
			if (this.activeHandHold.objectHeld != null)
			{
				PhotonView photonView;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonView>(out photonView))
				{
					VRRig.AttachLocalPlayerToPhotonView(photonView, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
					return;
				}
				PhotonViewXSceneRef photonViewXSceneRef;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef))
				{
					PhotonView photonView2 = photonViewXSceneRef.photonView;
					if (photonView2 != null)
					{
						VRRig.AttachLocalPlayerToPhotonView(photonView2, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
						return;
					}
				}
				BuilderPieceHandHold builderPieceHandHold;
				if (this.activeHandHold.objectHeld.TryGetComponent<BuilderPieceHandHold>(out builderPieceHandHold) && builderPieceHandHold.IsHandHoldMoving())
				{
					this.isHandHoldMoving = true;
					this.lastHandHoldRotation = builderPieceHandHold.transform.rotation;
					this.movingHandHoldReleaseVelocity = this.playerRigidBody.velocity;
				}
				else
				{
					this.isHandHoldMoving = false;
					this.lastHandHoldRotation = Quaternion.identity;
					this.movingHandHoldReleaseVelocity = Vector3.zero;
				}
			}
			VRRig.DetachLocalPlayerFromPhotonView();
		}

		// Token: 0x0600512A RID: 20778 RVA: 0x00187B5C File Offset: 0x00185D5C
		private void FixedUpdate_HandHolds(float timeDelta)
		{
			if (this.activeHandHold.objectHeld == null)
			{
				if (this.wasHoldingHandhold)
				{
					this.playerRigidBody.velocity = Vector3.ClampMagnitude(this.secondLastPreHandholdVelocity, 5.5f * this.scale);
				}
				this.wasHoldingHandhold = false;
				return;
			}
			Vector3 vector = this.activeHandHold.objectHeld.TransformPoint(this.activeHandHold.localPositionHeld);
			Vector3 position = this.activeHandHold.grabber.transform.position;
			this.secondLastPreHandholdVelocity = this.lastPreHandholdVelocity;
			this.lastPreHandholdVelocity = this.playerRigidBody.velocity;
			this.wasHoldingHandhold = true;
			if (this.isHandHoldMoving)
			{
				this.lastPreHandholdVelocity = this.movingHandHoldReleaseVelocity;
				this.playerRigidBody.velocity = Vector3.zero;
				Vector3 vector2 = vector - position;
				this.playerRigidBody.transform.position += vector2;
				this.movingHandHoldReleaseVelocity = vector2 / timeDelta;
				Quaternion quaternion = this.activeHandHold.objectHeld.rotation * Quaternion.Inverse(this.lastHandHoldRotation);
				this.RotateWithSurface(quaternion, vector);
				this.lastHandHoldRotation = this.activeHandHold.objectHeld.rotation;
				return;
			}
			this.playerRigidBody.velocity = (vector - position) / timeDelta;
			if (this.activeHandHold.applyRotation)
			{
				this.turnParent.transform.RotateAround(vector, base.transform.up, this.activeHandHold.localRotationalOffset - (this.activeHandHold.grabber.transform.rotation.eulerAngles.y - this.activeHandHold.objectHeld.rotation.eulerAngles.y));
			}
		}

		// Token: 0x06005130 RID: 20784 RVA: 0x00188115 File Offset: 0x00186315
		[CompilerGenerated]
		internal static void <BeginClimbing>g__SnapAxis|380_0(ref float val, float maxDist)
		{
			if (val > maxDist)
			{
				val = maxDist;
				return;
			}
			if (val < -maxDist)
			{
				val = -maxDist;
			}
		}

		// Token: 0x040053C9 RID: 21449
		private static GTPlayer _instance;

		// Token: 0x040053CA RID: 21450
		public static bool hasInstance;

		// Token: 0x040053CB RID: 21451
		public SphereCollider headCollider;

		// Token: 0x040053CC RID: 21452
		public CapsuleCollider bodyCollider;

		// Token: 0x040053CD RID: 21453
		private float bodyInitialRadius;

		// Token: 0x040053CE RID: 21454
		private float bodyInitialHeight;

		// Token: 0x040053CF RID: 21455
		private RaycastHit bodyHitInfo;

		// Token: 0x040053D0 RID: 21456
		private RaycastHit lastHitInfoHand;

		// Token: 0x040053D1 RID: 21457
		public Transform leftHandFollower;

		// Token: 0x040053D2 RID: 21458
		public Transform rightHandFollower;

		// Token: 0x040053D3 RID: 21459
		public Transform rightControllerTransform;

		// Token: 0x040053D4 RID: 21460
		public Transform leftControllerTransform;

		// Token: 0x040053D5 RID: 21461
		public GorillaVelocityTracker rightHandCenterVelocityTracker;

		// Token: 0x040053D6 RID: 21462
		public GorillaVelocityTracker leftHandCenterVelocityTracker;

		// Token: 0x040053D7 RID: 21463
		public GorillaVelocityTracker rightInteractPointVelocityTracker;

		// Token: 0x040053D8 RID: 21464
		public GorillaVelocityTracker leftInteractPointVelocityTracker;

		// Token: 0x040053D9 RID: 21465
		public GorillaVelocityTracker bodyVelocityTracker;

		// Token: 0x040053DA RID: 21466
		public PlayerAudioManager audioManager;

		// Token: 0x040053DB RID: 21467
		private Vector3 lastLeftHandPosition;

		// Token: 0x040053DC RID: 21468
		private Vector3 lastRightHandPosition;

		// Token: 0x040053DD RID: 21469
		public Vector3 lastHeadPosition;

		// Token: 0x040053DE RID: 21470
		private Vector3 lastRigidbodyPosition;

		// Token: 0x040053DF RID: 21471
		private Rigidbody playerRigidBody;

		// Token: 0x040053E0 RID: 21472
		private Camera mainCamera;

		// Token: 0x040053E1 RID: 21473
		public int velocityHistorySize;

		// Token: 0x040053E2 RID: 21474
		public float maxArmLength = 1f;

		// Token: 0x040053E3 RID: 21475
		public float unStickDistance = 1f;

		// Token: 0x040053E4 RID: 21476
		public float velocityLimit;

		// Token: 0x040053E5 RID: 21477
		public float slideVelocityLimit;

		// Token: 0x040053E6 RID: 21478
		public float maxJumpSpeed;

		// Token: 0x040053E7 RID: 21479
		private float _jumpMultiplier;

		// Token: 0x040053E8 RID: 21480
		public float minimumRaycastDistance = 0.05f;

		// Token: 0x040053E9 RID: 21481
		public float defaultSlideFactor = 0.03f;

		// Token: 0x040053EA RID: 21482
		public float slidingMinimum = 0.9f;

		// Token: 0x040053EB RID: 21483
		public float defaultPrecision = 0.995f;

		// Token: 0x040053EC RID: 21484
		public float teleportThresholdNoVel = 1f;

		// Token: 0x040053ED RID: 21485
		public float frictionConstant = 1f;

		// Token: 0x040053EE RID: 21486
		public float slideControl = 0.00425f;

		// Token: 0x040053EF RID: 21487
		public float stickDepth = 0.01f;

		// Token: 0x040053F0 RID: 21488
		private Vector3[] velocityHistory;

		// Token: 0x040053F1 RID: 21489
		private Vector3[] slideAverageHistory;

		// Token: 0x040053F2 RID: 21490
		private int velocityIndex;

		// Token: 0x040053F3 RID: 21491
		private Vector3 currentVelocity;

		// Token: 0x040053F4 RID: 21492
		private Vector3 averagedVelocity;

		// Token: 0x040053F5 RID: 21493
		private Vector3 lastPosition;

		// Token: 0x040053F6 RID: 21494
		public Vector3 rightHandOffset;

		// Token: 0x040053F7 RID: 21495
		public Vector3 leftHandOffset;

		// Token: 0x040053F8 RID: 21496
		public Quaternion rightHandRotOffset = Quaternion.identity;

		// Token: 0x040053F9 RID: 21497
		public Quaternion leftHandRotOffset = Quaternion.identity;

		// Token: 0x040053FA RID: 21498
		public Vector3 bodyOffset;

		// Token: 0x040053FB RID: 21499
		public LayerMask locomotionEnabledLayers;

		// Token: 0x040053FC RID: 21500
		public LayerMask waterLayer;

		// Token: 0x040053FD RID: 21501
		public bool wasLeftHandColliding;

		// Token: 0x040053FE RID: 21502
		public bool wasRightHandColliding;

		// Token: 0x040053FF RID: 21503
		public bool wasHeadTouching;

		// Token: 0x04005400 RID: 21504
		public int currentMaterialIndex;

		// Token: 0x04005401 RID: 21505
		public bool isLeftHandSliding;

		// Token: 0x04005402 RID: 21506
		public Vector3 leftHandSlideNormal;

		// Token: 0x04005403 RID: 21507
		public bool isRightHandSliding;

		// Token: 0x04005404 RID: 21508
		public Vector3 rightHandSlideNormal;

		// Token: 0x04005405 RID: 21509
		public Vector3 headSlideNormal;

		// Token: 0x04005406 RID: 21510
		public float rightHandSlipPercentage;

		// Token: 0x04005407 RID: 21511
		public float leftHandSlipPercentage;

		// Token: 0x04005408 RID: 21512
		public float headSlipPercentage;

		// Token: 0x04005409 RID: 21513
		public bool wasLeftHandSliding;

		// Token: 0x0400540A RID: 21514
		public bool wasRightHandSliding;

		// Token: 0x0400540B RID: 21515
		public Vector3 rightHandHitPoint;

		// Token: 0x0400540C RID: 21516
		public Vector3 leftHandHitPoint;

		// Token: 0x0400540D RID: 21517
		[SerializeField]
		private Transform cosmeticsHeadTarget;

		// Token: 0x0400540E RID: 21518
		[SerializeField]
		private float nativeScale = 1f;

		// Token: 0x0400540F RID: 21519
		[SerializeField]
		private float scaleMultiplier = 1f;

		// Token: 0x04005410 RID: 21520
		private NativeSizeChangerSettings activeSizeChangerSettings;

		// Token: 0x04005411 RID: 21521
		public bool debugMovement;

		// Token: 0x04005412 RID: 21522
		public bool disableMovement;

		// Token: 0x04005413 RID: 21523
		[NonSerialized]
		public bool inOverlay;

		// Token: 0x04005414 RID: 21524
		[NonSerialized]
		public bool isUserPresent;

		// Token: 0x04005415 RID: 21525
		public GameObject turnParent;

		// Token: 0x04005416 RID: 21526
		public int leftHandMaterialTouchIndex;

		// Token: 0x04005417 RID: 21527
		public GorillaSurfaceOverride leftHandSurfaceOverride;

		// Token: 0x04005418 RID: 21528
		public RaycastHit leftHandHitInfo;

		// Token: 0x04005419 RID: 21529
		public int rightHandMaterialTouchIndex;

		// Token: 0x0400541A RID: 21530
		public GorillaSurfaceOverride rightHandSurfaceOverride;

		// Token: 0x0400541B RID: 21531
		public RaycastHit rightHandHitInfo;

		// Token: 0x0400541C RID: 21532
		public GorillaSurfaceOverride currentOverride;

		// Token: 0x0400541D RID: 21533
		public MaterialDatasSO materialDatasSO;

		// Token: 0x0400541E RID: 21534
		private bool isLeftHandColliding;

		// Token: 0x0400541F RID: 21535
		private bool isRightHandColliding;

		// Token: 0x04005420 RID: 21536
		private float degreesTurnedThisFrame;

		// Token: 0x04005421 RID: 21537
		private Vector3 bodyOffsetVector;

		// Token: 0x04005422 RID: 21538
		private Vector3 movementToProjectedAboveCollisionPlane;

		// Token: 0x04005423 RID: 21539
		private MeshCollider meshCollider;

		// Token: 0x04005424 RID: 21540
		private Mesh collidedMesh;

		// Token: 0x04005425 RID: 21541
		private GTPlayer.MaterialData foundMatData;

		// Token: 0x04005426 RID: 21542
		private string findMatName;

		// Token: 0x04005427 RID: 21543
		private int vertex1;

		// Token: 0x04005428 RID: 21544
		private int vertex2;

		// Token: 0x04005429 RID: 21545
		private int vertex3;

		// Token: 0x0400542A RID: 21546
		private List<int> trianglesList = new List<int>(1000000);

		// Token: 0x0400542B RID: 21547
		private Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(128);

		// Token: 0x0400542C RID: 21548
		private int[] sharedMeshTris;

		// Token: 0x0400542D RID: 21549
		private float lastRealTime;

		// Token: 0x0400542E RID: 21550
		private float calcDeltaTime;

		// Token: 0x0400542F RID: 21551
		private float tempRealTime;

		// Token: 0x04005430 RID: 21552
		private Vector3 slideVelocity;

		// Token: 0x04005431 RID: 21553
		private Vector3 slideAverageNormal;

		// Token: 0x04005432 RID: 21554
		private RaycastHit tempHitInfo;

		// Token: 0x04005433 RID: 21555
		private RaycastHit junkHit;

		// Token: 0x04005434 RID: 21556
		private Vector3 firstPosition;

		// Token: 0x04005435 RID: 21557
		private RaycastHit tempIterativeHit;

		// Token: 0x04005436 RID: 21558
		private float maxSphereSize1;

		// Token: 0x04005437 RID: 21559
		private float maxSphereSize2;

		// Token: 0x04005438 RID: 21560
		private Collider[] overlapColliders = new Collider[10];

		// Token: 0x04005439 RID: 21561
		private int overlapAttempts;

		// Token: 0x0400543A RID: 21562
		private int touchPoints;

		// Token: 0x0400543B RID: 21563
		private float averageSlipPercentage;

		// Token: 0x0400543C RID: 21564
		private Vector3 surfaceDirection;

		// Token: 0x0400543D RID: 21565
		public float iceThreshold = 0.9f;

		// Token: 0x0400543E RID: 21566
		private float bodyMaxRadius;

		// Token: 0x0400543F RID: 21567
		public float bodyLerp = 0.17f;

		// Token: 0x04005440 RID: 21568
		private bool areBothTouching;

		// Token: 0x04005441 RID: 21569
		private float slideFactor;

		// Token: 0x04005442 RID: 21570
		[DebugOption]
		public bool didAJump;

		// Token: 0x04005443 RID: 21571
		private Renderer slideRenderer;

		// Token: 0x04005444 RID: 21572
		private RaycastHit[] rayCastNonAllocColliders;

		// Token: 0x04005445 RID: 21573
		private Vector3[] crazyCheckVectors;

		// Token: 0x04005446 RID: 21574
		private RaycastHit emptyHit;

		// Token: 0x04005447 RID: 21575
		private int bufferCount;

		// Token: 0x04005448 RID: 21576
		private Vector3 lastOpenHeadPosition;

		// Token: 0x04005449 RID: 21577
		private List<Material> tempMaterialArray = new List<Material>(16);

		// Token: 0x0400544A RID: 21578
		private int leftSlipSetToMaxFrameIdx = -1;

		// Token: 0x0400544B RID: 21579
		private int rightSlipSetToMaxFrameIdx = -1;

		// Token: 0x0400544C RID: 21580
		private const float CameraFarClipDefault = 500f;

		// Token: 0x0400544D RID: 21581
		private const float CameraNearClipDefault = 0.01f;

		// Token: 0x0400544E RID: 21582
		private const float CameraNearClipTiny = 0.002f;

		// Token: 0x0400544F RID: 21583
		private Dictionary<GameObject, PhysicMaterial> bodyTouchedSurfaces;

		// Token: 0x04005450 RID: 21584
		private bool primaryButtonPressed = true;

		// Token: 0x04005451 RID: 21585
		[Header("Swimming")]
		public PlayerSwimmingParameters swimmingParams;

		// Token: 0x04005452 RID: 21586
		public WaterParameters waterParams;

		// Token: 0x04005453 RID: 21587
		public List<GTPlayer.LiquidProperties> liquidPropertiesList = new List<GTPlayer.LiquidProperties>(16);

		// Token: 0x04005454 RID: 21588
		public bool debugDrawSwimming;

		// Token: 0x04005455 RID: 21589
		[Header("Slam/Hit effects")]
		public GameObject wizardStaffSlamEffects;

		// Token: 0x04005456 RID: 21590
		public GameObject geodeHitEffects;

		// Token: 0x04005457 RID: 21591
		[Header("Freeze Tag")]
		public float freezeTagHandSlidePercent = 0.88f;

		// Token: 0x04005458 RID: 21592
		public bool debugFreezeTag;

		// Token: 0x04005459 RID: 21593
		public float frozenBodyBuoyancyFactor = 1.5f;

		// Token: 0x0400545B RID: 21595
		[Space]
		private WaterVolume leftHandWaterVolume;

		// Token: 0x0400545C RID: 21596
		private WaterVolume rightHandWaterVolume;

		// Token: 0x0400545D RID: 21597
		private WaterVolume.SurfaceQuery leftHandWaterSurface;

		// Token: 0x0400545E RID: 21598
		private WaterVolume.SurfaceQuery rightHandWaterSurface;

		// Token: 0x0400545F RID: 21599
		private Vector3 swimmingVelocity = Vector3.zero;

		// Token: 0x04005460 RID: 21600
		private WaterVolume.SurfaceQuery waterSurfaceForHead;

		// Token: 0x04005461 RID: 21601
		private bool bodyInWater;

		// Token: 0x04005462 RID: 21602
		private bool headInWater;

		// Token: 0x04005463 RID: 21603
		private bool audioSetToUnderwater;

		// Token: 0x04005464 RID: 21604
		private float buoyancyExtension;

		// Token: 0x04005465 RID: 21605
		private float lastWaterSurfaceJumpTimeLeft = -1f;

		// Token: 0x04005466 RID: 21606
		private float lastWaterSurfaceJumpTimeRight = -1f;

		// Token: 0x04005467 RID: 21607
		private float waterSurfaceJumpCooldown = 0.1f;

		// Token: 0x04005468 RID: 21608
		private float leftHandNonDiveHapticsAmount;

		// Token: 0x04005469 RID: 21609
		private float rightHandNonDiveHapticsAmount;

		// Token: 0x0400546A RID: 21610
		private List<WaterVolume> headOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x0400546B RID: 21611
		private List<WaterVolume> bodyOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x0400546C RID: 21612
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x0400546D RID: 21613
		private Quaternion playerRotationOverride;

		// Token: 0x0400546E RID: 21614
		private int playerRotationOverrideFrame = -1;

		// Token: 0x0400546F RID: 21615
		private float playerRotationOverrideDecayRate = Mathf.Exp(1.5f);

		// Token: 0x04005471 RID: 21617
		private ContactPoint[] bodyCollisionContacts = new ContactPoint[8];

		// Token: 0x04005472 RID: 21618
		private int bodyCollisionContactsCount;

		// Token: 0x04005473 RID: 21619
		private ContactPoint bodyGroundContact;

		// Token: 0x04005474 RID: 21620
		private float bodyGroundContactTime;

		// Token: 0x04005475 RID: 21621
		private const float movingSurfaceVelocityLimit = 40f;

		// Token: 0x04005476 RID: 21622
		private bool exitMovingSurface;

		// Token: 0x04005477 RID: 21623
		private float exitMovingSurfaceThreshold = 6f;

		// Token: 0x04005478 RID: 21624
		private bool isClimbableMoving;

		// Token: 0x04005479 RID: 21625
		private Quaternion lastClimbableRotation;

		// Token: 0x0400547A RID: 21626
		private int lastAttachedToMovingSurfaceFrame;

		// Token: 0x0400547B RID: 21627
		private const int MIN_FRAMES_OFF_SURFACE_TO_DETACH = 3;

		// Token: 0x0400547C RID: 21628
		private bool isHandHoldMoving;

		// Token: 0x0400547D RID: 21629
		private Quaternion lastHandHoldRotation;

		// Token: 0x0400547E RID: 21630
		private Vector3 movingHandHoldReleaseVelocity;

		// Token: 0x0400547F RID: 21631
		private GTPlayer.MovingSurfaceContactPoint lastMovingSurfaceContact;

		// Token: 0x04005480 RID: 21632
		private int lastMovingSurfaceID = -1;

		// Token: 0x04005481 RID: 21633
		private BuilderPiece lastMonkeBlock;

		// Token: 0x04005482 RID: 21634
		private Quaternion lastMovingSurfaceRot;

		// Token: 0x04005483 RID: 21635
		private RaycastHit lastMovingSurfaceHit;

		// Token: 0x04005484 RID: 21636
		private Vector3 lastMovingSurfaceTouchLocal;

		// Token: 0x04005485 RID: 21637
		private Vector3 lastMovingSurfaceTouchWorld;

		// Token: 0x04005486 RID: 21638
		private Vector3 movingSurfaceOffset;

		// Token: 0x04005487 RID: 21639
		private bool wasMovingSurfaceMonkeBlock;

		// Token: 0x04005488 RID: 21640
		private Vector3 lastMovingSurfaceVelocity;

		// Token: 0x04005489 RID: 21641
		private bool wasBodyOnGround;

		// Token: 0x0400548A RID: 21642
		private BasePlatform currentPlatform;

		// Token: 0x0400548B RID: 21643
		private BasePlatform lastPlatformTouched;

		// Token: 0x0400548C RID: 21644
		private Vector3 lastFrameTouchPosLocal;

		// Token: 0x0400548D RID: 21645
		private Vector3 lastFrameTouchPosWorld;

		// Token: 0x0400548E RID: 21646
		private bool lastFrameHasValidTouchPos;

		// Token: 0x0400548F RID: 21647
		private Vector3 refMovement = Vector3.zero;

		// Token: 0x04005490 RID: 21648
		private Vector3 platformTouchOffset;

		// Token: 0x04005491 RID: 21649
		private Vector3 debugLastRightHandPosition;

		// Token: 0x04005492 RID: 21650
		private Vector3 debugPlatformDeltaPosition;

		// Token: 0x04005493 RID: 21651
		private const float climbingMaxThrowSpeed = 5.5f;

		// Token: 0x04005494 RID: 21652
		private const float climbHelperSmoothSnapSpeed = 12f;

		// Token: 0x04005495 RID: 21653
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x04005496 RID: 21654
		private GorillaClimbable currentClimbable;

		// Token: 0x04005497 RID: 21655
		private GorillaHandClimber currentClimber;

		// Token: 0x04005498 RID: 21656
		private Vector3 climbHelperTargetPos = Vector3.zero;

		// Token: 0x04005499 RID: 21657
		private Transform climbHelper;

		// Token: 0x0400549A RID: 21658
		private GorillaRopeSwing currentSwing;

		// Token: 0x0400549B RID: 21659
		private GorillaZipline currentZipline;

		// Token: 0x0400549C RID: 21660
		[SerializeField]
		private ConnectedControllerHandler controllerState;

		// Token: 0x0400549D RID: 21661
		public int sizeLayerMask;

		// Token: 0x0400549E RID: 21662
		public bool InReportMenu;

		// Token: 0x0400549F RID: 21663
		private LayerChanger layerChanger;

		// Token: 0x040054A0 RID: 21664
		private float halloweenLevitationStrength;

		// Token: 0x040054A1 RID: 21665
		private float halloweenLevitationFullStrengthDuration;

		// Token: 0x040054A2 RID: 21666
		private float halloweenLevitationTotalDuration = 1f;

		// Token: 0x040054A3 RID: 21667
		private float halloweenLevitationBonusStrength;

		// Token: 0x040054A4 RID: 21668
		private float halloweenLevitateBonusOffAtYSpeed;

		// Token: 0x040054A5 RID: 21669
		private float halloweenLevitateBonusFullAtYSpeed = 1f;

		// Token: 0x040054A6 RID: 21670
		private float lastTouchedGroundTimestamp;

		// Token: 0x040054A7 RID: 21671
		private bool teleportToTrain;

		// Token: 0x040054A8 RID: 21672
		public bool isAttachedToTrain;

		// Token: 0x040054A9 RID: 21673
		private bool stuckLeft;

		// Token: 0x040054AA RID: 21674
		private bool stuckRight;

		// Token: 0x040054AB RID: 21675
		private float lastScale;

		// Token: 0x040054AC RID: 21676
		private Vector3 currentSlopDirection;

		// Token: 0x040054AD RID: 21677
		private Vector3 lastSlopeDirection = Vector3.zero;

		// Token: 0x040054AE RID: 21678
		private Dictionary<Object, Action<GTPlayer>> gravityOverrides = new Dictionary<Object, Action<GTPlayer>>();

		// Token: 0x040054B1 RID: 21681
		private int hoverAllowedCount;

		// Token: 0x040054B2 RID: 21682
		[Header("Hoverboard Proto")]
		[SerializeField]
		private float hoverIdealHeight = 0.5f;

		// Token: 0x040054B3 RID: 21683
		[SerializeField]
		private float hoverCarveSidewaysSpeedLossFactor = 1f;

		// Token: 0x040054B4 RID: 21684
		[SerializeField]
		private AnimationCurve hoverCarveAngleResponsiveness;

		// Token: 0x040054B5 RID: 21685
		[SerializeField]
		private HoverboardVisual hoverboardVisual;

		// Token: 0x040054B6 RID: 21686
		[SerializeField]
		private float sidewaysDrag = 0.1f;

		// Token: 0x040054B7 RID: 21687
		[SerializeField]
		private float hoveringSlowSpeed = 0.1f;

		// Token: 0x040054B8 RID: 21688
		[SerializeField]
		private float hoveringSlowStoppingFactor = 0.95f;

		// Token: 0x040054B9 RID: 21689
		[SerializeField]
		private float hoverboardPaddleBoostMultiplier = 0.1f;

		// Token: 0x040054BA RID: 21690
		[SerializeField]
		private float hoverboardPaddleBoostMax = 10f;

		// Token: 0x040054BB RID: 21691
		[SerializeField]
		private float hoverboardBoostGracePeriod = 1f;

		// Token: 0x040054BC RID: 21692
		[SerializeField]
		private float hoverBodyHasCollisionsOutsideRadius = 0.5f;

		// Token: 0x040054BD RID: 21693
		[SerializeField]
		private float hoverBodyCollisionRadiusUpOffset = 0.2f;

		// Token: 0x040054BE RID: 21694
		[SerializeField]
		private float hoverGeneralUpwardForce = 8f;

		// Token: 0x040054BF RID: 21695
		[SerializeField]
		private float hoverTiltAdjustsForwardFactor = 0.2f;

		// Token: 0x040054C0 RID: 21696
		[SerializeField]
		private float hoverMinGrindSpeed = 1f;

		// Token: 0x040054C1 RID: 21697
		[SerializeField]
		private float hoverSlamJumpStrengthFactor = 25f;

		// Token: 0x040054C2 RID: 21698
		[SerializeField]
		private float hoverMaxPaddleSpeed = 35f;

		// Token: 0x040054C3 RID: 21699
		[SerializeField]
		private HoverboardAudio hoverboardAudio;

		// Token: 0x040054C4 RID: 21700
		private bool hasHoverPoint;

		// Token: 0x040054C5 RID: 21701
		private float boostEnabledUntilTimestamp;

		// Token: 0x040054C6 RID: 21702
		private GTPlayer.HoverBoardCast[] hoverboardCasts = new GTPlayer.HoverBoardCast[]
		{
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 1f, 0.36f),
				localDirection = Vector3.down,
				distance = 1f,
				sphereRadius = 0.2f,
				intersectToVelocityCap = 0.1f
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, 0.36f),
				localDirection = Vector3.forward,
				distance = 0.25f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, -0.1f),
				localDirection = -Vector3.forward,
				distance = 0.24f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			}
		};

		// Token: 0x040054C7 RID: 21703
		private Vector3 hoverboardPlayerLocalPos;

		// Token: 0x040054C8 RID: 21704
		private Quaternion hoverboardPlayerLocalRot;

		// Token: 0x040054C9 RID: 21705
		private bool didHoverLastFrame;

		// Token: 0x040054CA RID: 21706
		private GTPlayer.HandHoldState activeHandHold;

		// Token: 0x040054CB RID: 21707
		private GTPlayer.HandHoldState secondaryHandHold;

		// Token: 0x040054CC RID: 21708
		private bool rightHandHolding;

		// Token: 0x040054CD RID: 21709
		private bool leftHandHolding;

		// Token: 0x040054CE RID: 21710
		public PhysicMaterial slipperyMaterial;

		// Token: 0x040054CF RID: 21711
		private bool wasHoldingHandhold;

		// Token: 0x040054D0 RID: 21712
		private Vector3 secondLastPreHandholdVelocity;

		// Token: 0x040054D1 RID: 21713
		private Vector3 lastPreHandholdVelocity;

		// Token: 0x040054D2 RID: 21714
		[Header("Native Scale Adjustment")]
		[SerializeField]
		private AnimationCurve nativeScaleMagnitudeAdjustmentFactor;

		// Token: 0x02000CC2 RID: 3266
		private enum MovingSurfaceContactPoint
		{
			// Token: 0x040054D4 RID: 21716
			NONE,
			// Token: 0x040054D5 RID: 21717
			RIGHT,
			// Token: 0x040054D6 RID: 21718
			LEFT,
			// Token: 0x040054D7 RID: 21719
			BODY
		}

		// Token: 0x02000CC3 RID: 3267
		[Serializable]
		public struct MaterialData
		{
			// Token: 0x040054D8 RID: 21720
			public string matName;

			// Token: 0x040054D9 RID: 21721
			public bool overrideAudio;

			// Token: 0x040054DA RID: 21722
			public AudioClip audio;

			// Token: 0x040054DB RID: 21723
			public bool overrideSlidePercent;

			// Token: 0x040054DC RID: 21724
			public float slidePercent;

			// Token: 0x040054DD RID: 21725
			public int surfaceEffectIndex;
		}

		// Token: 0x02000CC4 RID: 3268
		[Serializable]
		public struct LiquidProperties
		{
			// Token: 0x040054DE RID: 21726
			[Range(0f, 2f)]
			[Tooltip("0: no resistance just like air, 1: full resistance like solid geometry")]
			public float resistance;

			// Token: 0x040054DF RID: 21727
			[Range(0f, 3f)]
			[Tooltip("0: no buoyancy. 1: Fully compensates gravity. 2: net force is upwards equal to gravity")]
			public float buoyancy;

			// Token: 0x040054E0 RID: 21728
			[Range(0f, 3f)]
			[Tooltip("Damping Half-life Multiplier")]
			public float dampingFactor;

			// Token: 0x040054E1 RID: 21729
			[Range(0f, 1f)]
			public float surfaceJumpFactor;
		}

		// Token: 0x02000CC5 RID: 3269
		public enum LiquidType
		{
			// Token: 0x040054E3 RID: 21731
			Water,
			// Token: 0x040054E4 RID: 21732
			Lava
		}

		// Token: 0x02000CC6 RID: 3270
		private struct HoverBoardCast
		{
			// Token: 0x040054E5 RID: 21733
			public Vector3 localOrigin;

			// Token: 0x040054E6 RID: 21734
			public Vector3 localDirection;

			// Token: 0x040054E7 RID: 21735
			public float sphereRadius;

			// Token: 0x040054E8 RID: 21736
			public float distance;

			// Token: 0x040054E9 RID: 21737
			public float intersectToVelocityCap;

			// Token: 0x040054EA RID: 21738
			public bool isSolid;

			// Token: 0x040054EB RID: 21739
			public bool didHit;

			// Token: 0x040054EC RID: 21740
			public Vector3 pointHit;

			// Token: 0x040054ED RID: 21741
			public Vector3 normalHit;
		}

		// Token: 0x02000CC7 RID: 3271
		private struct HandHoldState
		{
			// Token: 0x040054EE RID: 21742
			public GorillaGrabber grabber;

			// Token: 0x040054EF RID: 21743
			public Transform objectHeld;

			// Token: 0x040054F0 RID: 21744
			public Vector3 localPositionHeld;

			// Token: 0x040054F1 RID: 21745
			public float localRotationalOffset;

			// Token: 0x040054F2 RID: 21746
			public bool applyRotation;
		}
	}
}
