using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001A0 RID: 416
public class TriggerOnJump : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06000A49 RID: 2633 RVA: 0x00035DC4 File Offset: 0x00033FC4
	private void OnEnable()
	{
		if (this.myRig.IsNull())
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		if (this._events == null && this.myRig != null && this.myRig.Creator != null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			this._events.Init(this.myRig.creator);
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnActivate;
		}
		bool flag = !PhotonNetwork.InRoom && this.myRig != null && this.myRig.isOfflineVRRig;
		RigContainer rigContainer;
		bool flag2 = PhotonNetwork.InRoom && this.myRig != null && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer != null && rigContainer.Rig != null && rigContainer.Rig == this.myRig;
		if (flag || flag2)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x00035EEC File Offset: 0x000340EC
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
		this.playerOnGround = false;
		this.jumpStartTime = 0f;
		this.lastActivationTime = 0f;
		this.waitingForGrounding = false;
		if (this._events != null)
		{
			this._events.Activate -= this.OnActivate;
			Object.Destroy(this._events);
			this._events = null;
		}
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x00035F65 File Offset: 0x00034165
	private void OnActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnJumpActivate");
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		this.onJumping.Invoke();
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x00035FA0 File Offset: 0x000341A0
	public void Tick()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			bool flag = this.playerOnGround;
			this.playerOnGround = instance.BodyOnGround || instance.IsHandTouching(true) || instance.IsHandTouching(false);
			float time = Time.time;
			if (this.playerOnGround)
			{
				this.waitingForGrounding = false;
			}
			if (!this.playerOnGround && flag)
			{
				this.jumpStartTime = time;
			}
			if (!this.playerOnGround && !this.waitingForGrounding && instance.RigidbodyVelocity.sqrMagnitude > this.minJumpStrength * this.minJumpStrength && instance.RigidbodyVelocity.y > this.minJumpVertical && time > this.jumpStartTime + this.minJumpTime)
			{
				this.waitingForGrounding = true;
				if (time > this.lastActivationTime + this.cooldownTime)
				{
					this.lastActivationTime = time;
					if (PhotonNetwork.InRoom)
					{
						this._events.Activate.RaiseAll(Array.Empty<object>());
						return;
					}
					this.onJumping.Invoke();
				}
			}
		}
	}

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000A4D RID: 2637 RVA: 0x000360AC File Offset: 0x000342AC
	// (set) Token: 0x06000A4E RID: 2638 RVA: 0x000360B4 File Offset: 0x000342B4
	public bool TickRunning { get; set; }

	// Token: 0x04000C6F RID: 3183
	[SerializeField]
	private float minJumpStrength = 1f;

	// Token: 0x04000C70 RID: 3184
	[SerializeField]
	private float minJumpVertical = 1f;

	// Token: 0x04000C71 RID: 3185
	[SerializeField]
	private float cooldownTime = 1f;

	// Token: 0x04000C72 RID: 3186
	[SerializeField]
	private UnityEvent onJumping;

	// Token: 0x04000C73 RID: 3187
	private RubberDuckEvents _events;

	// Token: 0x04000C74 RID: 3188
	private bool playerOnGround;

	// Token: 0x04000C75 RID: 3189
	private float minJumpTime = 0.05f;

	// Token: 0x04000C76 RID: 3190
	private bool waitingForGrounding;

	// Token: 0x04000C77 RID: 3191
	private float jumpStartTime;

	// Token: 0x04000C78 RID: 3192
	private float lastActivationTime;

	// Token: 0x04000C79 RID: 3193
	private VRRig myRig;
}
