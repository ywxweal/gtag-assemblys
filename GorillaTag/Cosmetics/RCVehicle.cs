using System;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DBD RID: 3517
	public class RCVehicle : MonoBehaviour, ISpawnable
	{
		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x06005718 RID: 22296 RVA: 0x001AB7C4 File Offset: 0x001A99C4
		public bool HasLocalAuthority
		{
			get
			{
				return !PhotonNetwork.InRoom || (this.networkSync != null && this.networkSync.photonView.IsMine);
			}
		}

		// Token: 0x06005719 RID: 22297 RVA: 0x001AB7F0 File Offset: 0x001A99F0
		public virtual void WakeUpRemote(RCCosmeticNetworkSync sync)
		{
			this.networkSync = sync;
			this.hasNetworkSync = sync != null;
			if (this.HasLocalAuthority)
			{
				return;
			}
			if (!base.enabled || !base.gameObject.activeSelf)
			{
				this.localStatePrev = RCVehicle.State.Disabled;
				base.enabled = true;
				base.gameObject.SetActive(true);
				this.RemoteUpdate(Time.deltaTime);
			}
		}

		// Token: 0x0600571A RID: 22298 RVA: 0x001AB854 File Offset: 0x001A9A54
		public virtual void StartConnection(RCRemoteHoldable remote, RCCosmeticNetworkSync sync)
		{
			this.connectedRemote = remote;
			this.networkSync = sync;
			this.hasNetworkSync = sync != null;
			base.enabled = true;
			base.gameObject.SetActive(true);
			this.useLeftDock = remote.XRNode == XRNode.LeftHand;
			if (this.HasLocalAuthority && this.localState != RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginDocked();
			}
		}

		// Token: 0x0600571B RID: 22299 RVA: 0x001AB8B5 File Offset: 0x001A9AB5
		public virtual void EndConnection()
		{
			this.connectedRemote = null;
			this.activeInput = default(RCRemoteHoldable.RCInput);
			this.disconnectionTime = Time.time;
		}

		// Token: 0x0600571C RID: 22300 RVA: 0x001AB8D8 File Offset: 0x001A9AD8
		protected virtual void ResetToSpawnPosition()
		{
			if (this.rb == null)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
			if (this.rb != null)
			{
				this.rb.isKinematic = true;
			}
			base.transform.parent = (this.useLeftDock ? this.leftDockParent : this.rightDockParent);
			base.transform.SetLocalPositionAndRotation(this.useLeftDock ? this.dockLeftOffset.pos : this.dockRightOffset.pos, this.useLeftDock ? this.dockLeftOffset.rot : this.dockRightOffset.rot);
			base.transform.localScale = (this.useLeftDock ? this.dockLeftOffset.scale : this.dockRightOffset.scale);
		}

		// Token: 0x0600571D RID: 22301 RVA: 0x001AB9B0 File Offset: 0x001A9BB0
		protected virtual void AuthorityBeginDocked()
		{
			this.localState = (this.useLeftDock ? RCVehicle.State.DockedLeft : RCVehicle.State.DockedRight);
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			this.waitingForTriggerRelease = true;
			this.ResetToSpawnPosition();
			if (this.connectedRemote == null)
			{
				this.SetDisabledState();
			}
		}

		// Token: 0x0600571E RID: 22302 RVA: 0x001ABA20 File Offset: 0x001A9C20
		protected virtual void AuthorityBeginMobilization()
		{
			this.localState = RCVehicle.State.Mobilized;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			base.transform.parent = null;
			this.rb.isKinematic = false;
		}

		// Token: 0x0600571F RID: 22303 RVA: 0x001ABA7C File Offset: 0x001A9C7C
		protected virtual void AuthorityBeginCrash()
		{
			this.localState = RCVehicle.State.Crashed;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
		}

		// Token: 0x06005720 RID: 22304 RVA: 0x001ABAB8 File Offset: 0x001A9CB8
		protected virtual void SetDisabledState()
		{
			this.localState = RCVehicle.State.Disabled;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.ResetToSpawnPosition();
			base.enabled = false;
			base.gameObject.SetActive(false);
		}

		// Token: 0x06005721 RID: 22305 RVA: 0x001ABB0A File Offset: 0x001A9D0A
		protected virtual void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06005722 RID: 22306 RVA: 0x000023F4 File Offset: 0x000005F4
		protected virtual void OnEnable()
		{
		}

		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x06005723 RID: 22307 RVA: 0x001ABB18 File Offset: 0x001A9D18
		// (set) Token: 0x06005724 RID: 22308 RVA: 0x001ABB20 File Offset: 0x001A9D20
		bool ISpawnable.IsSpawned { get; set; }

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x06005725 RID: 22309 RVA: 0x001ABB29 File Offset: 0x001A9D29
		// (set) Token: 0x06005726 RID: 22310 RVA: 0x001ABB31 File Offset: 0x001A9D31
		ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

		// Token: 0x06005727 RID: 22311 RVA: 0x001ABB3C File Offset: 0x001A9D3C
		void ISpawnable.OnSpawn(VRRig rig)
		{
			if (rig == null)
			{
				GTDev.LogError<string>("RCVehicle: Could not find VRRig in parents. If you are trying to make this a world item rather than a cosmetic then you'll have to refactor how it teleports back to the arms.", this, null);
				return;
			}
			string text;
			if (!GTHardCodedBones.TryGetBoneXforms(rig, out this._vrRigBones, out text))
			{
				Debug.LogError("RCVehicle: " + text, this);
				return;
			}
			if (this.leftDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockLeftOffset.bone, out this.leftDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find left dock transform.", this, null);
			}
			if (this.rightDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockRightOffset.bone, out this.rightDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find right dock transform.", this, null);
			}
		}

		// Token: 0x06005728 RID: 22312 RVA: 0x000023F4 File Offset: 0x000005F4
		void ISpawnable.OnDespawn()
		{
		}

		// Token: 0x06005729 RID: 22313 RVA: 0x001ABBF7 File Offset: 0x001A9DF7
		protected virtual void OnDisable()
		{
			this.localState = RCVehicle.State.Disabled;
			this.localStatePrev = RCVehicle.State.Disabled;
		}

		// Token: 0x0600572A RID: 22314 RVA: 0x001ABC08 File Offset: 0x001A9E08
		public void ApplyRemoteControlInput(RCRemoteHoldable.RCInput rcInput)
		{
			this.activeInput.joystick.y = Mathf.Sign(rcInput.joystick.y) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.y)));
			this.activeInput.joystick.x = Mathf.Sign(rcInput.joystick.x) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.x)));
			this.activeInput.trigger = Mathf.Clamp(rcInput.trigger, -1f, 1f);
			this.activeInput.buttons = rcInput.buttons;
		}

		// Token: 0x0600572B RID: 22315 RVA: 0x001ABCE8 File Offset: 0x001A9EE8
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			if (this.HasLocalAuthority)
			{
				this.AuthorityUpdate(deltaTime);
			}
			else
			{
				this.RemoteUpdate(deltaTime);
			}
			this.SharedUpdate(deltaTime);
			this.localStatePrev = this.localState;
		}

		// Token: 0x0600572C RID: 22316 RVA: 0x001ABD28 File Offset: 0x001A9F28
		protected virtual void AuthorityUpdate(float dt)
		{
			switch (this.localState)
			{
			default:
				if (this.localState != this.localStatePrev)
				{
					this.ResetToSpawnPosition();
				}
				if (this.connectedRemote == null)
				{
					this.SetDisabledState();
					return;
				}
				if (this.waitingForTriggerRelease && this.activeInput.trigger < 0.25f)
				{
					this.waitingForTriggerRelease = false;
				}
				if (!this.waitingForTriggerRelease && this.activeInput.trigger > 0.25f)
				{
					this.AuthorityBeginMobilization();
					return;
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.networkSync != null)
				{
					this.networkSync.syncedState.position = base.transform.position;
					this.networkSync.syncedState.rotation = base.transform.rotation;
				}
				bool flag = (base.transform.position - this.leftDockParent.position).sqrMagnitude > this.maxRange * this.maxRange;
				bool flag2 = this.connectedRemote == null && Time.time - this.disconnectionTime > this.maxDisconnectionTime;
				if (flag || flag2)
				{
					this.AuthorityBeginCrash();
					return;
				}
				break;
			}
			case RCVehicle.State.Crashed:
				if (Time.time > this.stateStartTime + this.crashRespawnDelay)
				{
					this.AuthorityBeginDocked();
				}
				break;
			}
		}

		// Token: 0x0600572D RID: 22317 RVA: 0x001ABE88 File Offset: 0x001AA088
		protected virtual void RemoteUpdate(float dt)
		{
			if (this.networkSync == null)
			{
				this.SetDisabledState();
				return;
			}
			this.localState = (RCVehicle.State)this.networkSync.syncedState.state;
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				this.SetDisabledState();
				break;
			default:
				if (this.localStatePrev != RCVehicle.State.DockedLeft)
				{
					this.useLeftDock = true;
					this.ResetToSpawnPosition();
					return;
				}
				break;
			case RCVehicle.State.DockedRight:
				if (this.localStatePrev != RCVehicle.State.DockedRight)
				{
					this.useLeftDock = false;
					this.ResetToSpawnPosition();
					return;
				}
				break;
			case RCVehicle.State.Mobilized:
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.rb.isKinematic = true;
					base.transform.parent = null;
				}
				base.transform.position = Vector3.Lerp(this.networkSync.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				base.transform.rotation = Quaternion.Slerp(this.networkSync.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				return;
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.rb.isKinematic = false;
					base.transform.parent = null;
					if (this.localStatePrev != RCVehicle.State.Mobilized)
					{
						base.transform.position = this.networkSync.syncedState.position;
						base.transform.rotation = this.networkSync.syncedState.rotation;
						return;
					}
				}
				break;
			}
		}

		// Token: 0x0600572E RID: 22318 RVA: 0x000023F4 File Offset: 0x000005F4
		protected virtual void SharedUpdate(float dt)
		{
		}

		// Token: 0x0600572F RID: 22319 RVA: 0x001AC010 File Offset: 0x001AA210
		public virtual void AuthorityApplyImpact(Vector3 hitVelocity, bool isProjectile)
		{
			if (this.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				float num = (isProjectile ? this.projectileVelocityTransfer : this.hitVelocityTransfer);
				this.rb.AddForce(Vector3.ClampMagnitude(hitVelocity * num, this.hitMaxHitSpeed), ForceMode.VelocityChange);
				if (isProjectile || (this.crashOnHit && hitVelocity.sqrMagnitude > this.crashOnHitSpeedThreshold * this.crashOnHitSpeedThreshold))
				{
					this.AuthorityBeginCrash();
				}
			}
		}

		// Token: 0x06005730 RID: 22320 RVA: 0x0010EC8D File Offset: 0x0010CE8D
		protected float NormalizeAngle180(float angle)
		{
			angle = (angle + 180f) % 360f;
			if (angle < 0f)
			{
				angle += 360f;
			}
			return angle - 180f;
		}

		// Token: 0x06005731 RID: 22321 RVA: 0x001AC088 File Offset: 0x001AA288
		protected static void AddScaledGravityCompensationForce(Rigidbody rb, float scaleFactor, float gravityCompensation)
		{
			Vector3 gravity = Physics.gravity;
			Vector3 vector = -gravity * gravityCompensation;
			Vector3 vector2 = gravity + vector;
			Vector3 vector3 = vector2 * scaleFactor - vector2;
			rb.AddForce(vector + vector3, ForceMode.Acceleration);
		}

		// Token: 0x04005BA1 RID: 23457
		[SerializeField]
		private Transform leftDockParent;

		// Token: 0x04005BA2 RID: 23458
		[SerializeField]
		private Transform rightDockParent;

		// Token: 0x04005BA3 RID: 23459
		[SerializeField]
		private float maxRange = 100f;

		// Token: 0x04005BA4 RID: 23460
		[SerializeField]
		private float maxDisconnectionTime = 10f;

		// Token: 0x04005BA5 RID: 23461
		[SerializeField]
		private float crashRespawnDelay = 3f;

		// Token: 0x04005BA6 RID: 23462
		[SerializeField]
		private bool crashOnHit;

		// Token: 0x04005BA7 RID: 23463
		[SerializeField]
		private float crashOnHitSpeedThreshold = 5f;

		// Token: 0x04005BA8 RID: 23464
		[SerializeField]
		[Range(0f, 1f)]
		private float hitVelocityTransfer = 0.5f;

		// Token: 0x04005BA9 RID: 23465
		[SerializeField]
		[Range(0f, 1f)]
		private float projectileVelocityTransfer = 0.1f;

		// Token: 0x04005BAA RID: 23466
		[SerializeField]
		private float hitMaxHitSpeed = 4f;

		// Token: 0x04005BAB RID: 23467
		[SerializeField]
		[Range(0f, 1f)]
		private float joystickDeadzone = 0.1f;

		// Token: 0x04005BAC RID: 23468
		protected RCVehicle.State localState;

		// Token: 0x04005BAD RID: 23469
		protected RCVehicle.State localStatePrev;

		// Token: 0x04005BAE RID: 23470
		protected float stateStartTime;

		// Token: 0x04005BAF RID: 23471
		protected RCRemoteHoldable connectedRemote;

		// Token: 0x04005BB0 RID: 23472
		protected RCCosmeticNetworkSync networkSync;

		// Token: 0x04005BB1 RID: 23473
		protected bool hasNetworkSync;

		// Token: 0x04005BB2 RID: 23474
		protected RCRemoteHoldable.RCInput activeInput;

		// Token: 0x04005BB3 RID: 23475
		protected Rigidbody rb;

		// Token: 0x04005BB4 RID: 23476
		private bool waitingForTriggerRelease;

		// Token: 0x04005BB5 RID: 23477
		private float disconnectionTime;

		// Token: 0x04005BB6 RID: 23478
		private bool useLeftDock;

		// Token: 0x04005BB7 RID: 23479
		private BoneOffset dockLeftOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_L, new Vector3(-0.062f, 0.283f, -0.136f), new Vector3(275f, 0f, 25f));

		// Token: 0x04005BB8 RID: 23480
		private BoneOffset dockRightOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_R, new Vector3(0.069f, 0.265f, -0.128f), new Vector3(275f, 0f, 335f));

		// Token: 0x04005BB9 RID: 23481
		private float networkSyncFollowRateExp = 2f;

		// Token: 0x04005BBA RID: 23482
		private Transform[] _vrRigBones;

		// Token: 0x02000DBE RID: 3518
		protected enum State
		{
			// Token: 0x04005BBE RID: 23486
			Disabled,
			// Token: 0x04005BBF RID: 23487
			DockedLeft,
			// Token: 0x04005BC0 RID: 23488
			DockedRight,
			// Token: 0x04005BC1 RID: 23489
			Mobilized,
			// Token: 0x04005BC2 RID: 23490
			Crashed
		}
	}
}
