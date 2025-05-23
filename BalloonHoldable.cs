using System;
using GorillaExtensions;
using GorillaLocomotion.Swimming;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000393 RID: 915
public class BalloonHoldable : TransferrableObject, IFXContext
{
	// Token: 0x0600153B RID: 5435 RVA: 0x00067B24 File Offset: 0x00065D24
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.balloonDynamics = base.GetComponent<ITetheredObjectBehavior>();
		if (this.mesh == null)
		{
			this.mesh = base.GetComponent<Renderer>();
		}
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.itemState = (TransferrableObject.ItemStates)0;
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x00067B7D File Offset: 0x00065D7D
	protected override void Start()
	{
		base.Start();
		this.EnableDynamics(false, false, false);
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x00067B90 File Offset: 0x00065D90
	internal override void OnEnable()
	{
		base.OnEnable();
		this.EnableDynamics(false, false, false);
		this.mesh.enabled = true;
		this.lineRenderer.enabled = false;
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.worldShareableInstance != null)
			{
				return;
			}
			base.SpawnTransferableObjectViews();
		}
		if (base.InHand())
		{
			this.Grab();
			return;
		}
		if (base.Dropped())
		{
			this.Release();
		}
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x00067C02 File Offset: 0x00065E02
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.worldShareableInstance != null)
		{
			return;
		}
		base.SpawnTransferableObjectViews();
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x00067C1F File Offset: 0x00065E1F
	private bool ShouldSimulate()
	{
		return !base.Attached() && this.balloonState == BalloonHoldable.BalloonStates.Normal;
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x00067C34 File Offset: 0x00065E34
	internal override void OnDisable()
	{
		base.OnDisable();
		this.lineRenderer.enabled = false;
		this.EnableDynamics(false, false, false);
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x00067C51 File Offset: 0x00065E51
	public override void PreDisable()
	{
		this.originalOwner = null;
		base.PreDisable();
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x00067C60 File Offset: 0x00065E60
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.balloonState = BalloonHoldable.BalloonStates.Normal;
		base.transform.localScale = Vector3.one;
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x00067C80 File Offset: 0x00065E80
	protected override void OnWorldShareableItemSpawn()
	{
		WorldShareableItem worldShareableInstance = this.worldShareableInstance;
		if (worldShareableInstance != null)
		{
			worldShareableInstance.rpcCallBack = new Action(this.PopBalloonRemote);
			worldShareableInstance.onOwnerChangeCb = new WorldShareableItem.OnOwnerChangeDelegate(this.OnOwnerChangeCb);
			worldShareableInstance.EnableRemoteSync = this.ShouldSimulate();
		}
		this.originalOwner = worldShareableInstance.target.owner;
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x00067CE0 File Offset: 0x00065EE0
	public override void ResetToHome()
	{
		if (base.IsLocalObject() && this.worldShareableInstance != null && !this.worldShareableInstance.guard.isTrulyMine)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", RpcTarget.Others, Array.Empty<object>());
			}
			this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
		}
		this.PopBalloon();
		this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
		base.ResetToHome();
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x00067D5E File Offset: 0x00065F5E
	protected override void PlayDestroyedOrDisabledEffect()
	{
		base.PlayDestroyedOrDisabledEffect();
		this.PlayPopBalloonFX();
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x00067D6C File Offset: 0x00065F6C
	protected override void OnItemDestroyedOrDisabled()
	{
		this.PlayPopBalloonFX();
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.ReParent();
		}
		base.transform.parent = base.DefaultAnchor();
		base.OnItemDestroyedOrDisabled();
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x00067DA0 File Offset: 0x00065FA0
	private void PlayPopBalloonFX()
	{
		FXSystem.PlayFXForRig(FXType.BalloonPop, this, default(PhotonMessageInfoWrapped));
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x00067DC0 File Offset: 0x00065FC0
	private void EnableDynamics(bool enable, bool collider, bool forceKinematicOn = false)
	{
		bool flag = false;
		if (forceKinematicOn)
		{
			flag = true;
		}
		else if (NetworkSystem.Instance.InRoom && this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance.gameObject);
			if (photonView != null && !photonView.IsMine)
			{
				flag = true;
			}
		}
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.EnableDynamics(enable, collider, flag);
		}
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x00067E2C File Offset: 0x0006602C
	private void PopBalloon()
	{
		this.PlayPopBalloonFX();
		this.EnableDynamics(false, false, false);
		this.mesh.enabled = false;
		this.lineRenderer.enabled = false;
		if (this.gripInteractor != null)
		{
			this.gripInteractor.gameObject.SetActive(false);
		}
		if ((object.Equals(this.originalOwner, PhotonNetwork.LocalPlayer) || !NetworkSystem.Instance.InRoom) && NetworkSystem.Instance.InRoom && this.worldShareableInstance != null && !this.worldShareableInstance.guard.isTrulyMine)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
		}
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.ReParent();
			this.EnableDynamics(false, false, false);
		}
		if (this.IsMyItem())
		{
			if (base.InLeftHand())
			{
				EquipmentInteractor.instance.ReleaseLeftHand();
			}
			if (base.InRightHand())
			{
				EquipmentInteractor.instance.ReleaseRightHand();
			}
		}
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x00067F25 File Offset: 0x00066125
	public void PopBalloonRemote()
	{
		if (this.ShouldSimulate())
		{
			this.balloonState = BalloonHoldable.BalloonStates.Pop;
		}
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnOwnerChangeCb(NetPlayer newOwner, NetPlayer prevOwner)
	{
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x00067F38 File Offset: 0x00066138
	public override void OnOwnershipTransferred(NetPlayer newOwner, NetPlayer prevOwner)
	{
		base.OnOwnershipTransferred(newOwner, prevOwner);
		if (object.Equals(prevOwner, NetworkSystem.Instance.LocalPlayer) && newOwner == null)
		{
			return;
		}
		if (!object.Equals(newOwner, NetworkSystem.Instance.LocalPlayer))
		{
			this.EnableDynamics(false, true, true);
			return;
		}
		if (this.ShouldSimulate() && this.balloonDynamics != null)
		{
			this.balloonDynamics.EnableDynamics(true, true, false);
		}
		if (!this.rb)
		{
			return;
		}
		if (!this.rb.isKinematic)
		{
			this.rb.AddForceAtPosition(this.forceAppliedAsRemote, this.collisionPtAsRemote, ForceMode.VelocityChange);
		}
		this.forceAppliedAsRemote = Vector3.zero;
		this.collisionPtAsRemote = Vector3.zero;
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x00067FE8 File Offset: 0x000661E8
	private void OwnerPopBalloon()
	{
		if (this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", RpcTarget.Others, Array.Empty<object>());
			}
		}
		this.balloonState = BalloonHoldable.BalloonStates.Pop;
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x00068030 File Offset: 0x00066230
	private void RunLocalPopSM()
	{
		switch (this.balloonState)
		{
		case BalloonHoldable.BalloonStates.Normal:
			break;
		case BalloonHoldable.BalloonStates.Pop:
			this.timer = Time.time;
			this.PopBalloon();
			this.balloonState = BalloonHoldable.BalloonStates.WaitForOwnershipTransfer;
			this.lastOwnershipRequest = Time.time;
			return;
		case BalloonHoldable.BalloonStates.Waiting:
			if (Time.time - this.timer >= this.poppedTimerLength)
			{
				this.timer = Time.time;
				this.mesh.enabled = true;
				this.balloonInflatSource.GTPlay();
				this.balloonState = BalloonHoldable.BalloonStates.Refilling;
				return;
			}
			base.transform.localScale = new Vector3(this.beginScale, this.beginScale, this.beginScale);
			return;
		case BalloonHoldable.BalloonStates.WaitForOwnershipTransfer:
			if (!NetworkSystem.Instance.InRoom)
			{
				this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
				base.ReDock();
				return;
			}
			if (this.worldShareableInstance != null)
			{
				WorldShareableItem worldShareableInstance = this.worldShareableInstance;
				NetPlayer owner = worldShareableInstance.Owner;
				if (worldShareableInstance != null && owner == this.originalOwner)
				{
					this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
					base.ReDock();
				}
				if (base.IsLocalObject() && this.lastOwnershipRequest + 5f < Time.time)
				{
					this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
					this.lastOwnershipRequest = Time.time;
					return;
				}
			}
			break;
		case BalloonHoldable.BalloonStates.WaitForReDock:
			if (base.Attached())
			{
				this.fullyInflatedScale = base.transform.localScale;
				base.ReDock();
				this.balloonState = BalloonHoldable.BalloonStates.Waiting;
				return;
			}
			break;
		case BalloonHoldable.BalloonStates.Refilling:
		{
			float num = Time.time - this.timer;
			if (num >= this.scaleTimerLength)
			{
				base.transform.localScale = this.fullyInflatedScale;
				this.balloonState = BalloonHoldable.BalloonStates.Normal;
				if (this.gripInteractor != null)
				{
					this.gripInteractor.gameObject.SetActive(true);
				}
			}
			num = Mathf.Clamp01(num / this.scaleTimerLength);
			float num2 = Mathf.Lerp(this.beginScale, 1f, num);
			base.transform.localScale = this.fullyInflatedScale * num2;
			return;
		}
		case BalloonHoldable.BalloonStates.Returning:
			if (this.balloonDynamics.ReturnStep())
			{
				this.balloonState = BalloonHoldable.BalloonStates.Normal;
				base.ReDock();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x00068248 File Offset: 0x00066448
	protected override void OnStateChanged()
	{
		if (base.InHand())
		{
			this.Grab();
			return;
		}
		if (base.Dropped())
		{
			this.Release();
			return;
		}
		if (base.OnShoulder())
		{
			if (this.balloonDynamics != null && this.balloonDynamics.IsEnabled())
			{
				this.EnableDynamics(false, false, false);
			}
			this.lineRenderer.enabled = false;
		}
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x000682A8 File Offset: 0x000664A8
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (Time.frameCount == this.enabledOnFrame)
		{
			this.OnStateChanged();
		}
		if (base.InHand() && this.detatchOnGrab)
		{
			float num = ((this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f);
			base.transform.localScale = Vector3.one * num;
		}
		if (base.Dropped() && this.balloonState == BalloonHoldable.BalloonStates.Normal && this.maxDistanceFromOwner > 0f && (!NetworkSystem.Instance.InRoom || this.originalOwner.IsLocal) && (VRRig.LocalRig.transform.position - base.transform.position).IsLongerThan(this.maxDistanceFromOwner * base.transform.localScale.x))
		{
			this.OwnerPopBalloon();
		}
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.EnableRemoteSync = this.ShouldSimulate();
		}
		if (this.balloonState != BalloonHoldable.BalloonStates.Normal)
		{
			this.RunLocalPopSM();
		}
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x000683D1 File Offset: 0x000665D1
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x000683DC File Offset: 0x000665DC
	private void Grab()
	{
		if (this.balloonDynamics == null)
		{
			return;
		}
		if (this.detatchOnGrab)
		{
			float num = ((this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f);
			base.transform.localScale = Vector3.one * num;
			this.EnableDynamics(true, true, false);
			this.balloonDynamics.EnableDistanceConstraints(true, num);
			this.lineRenderer.enabled = true;
			return;
		}
		base.transform.localScale = Vector3.one;
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x00068470 File Offset: 0x00066670
	private void Release()
	{
		if (this.disableRelease)
		{
			this.balloonState = BalloonHoldable.BalloonStates.Returning;
			return;
		}
		if (this.balloonDynamics == null)
		{
			return;
		}
		float num = ((this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f);
		base.transform.localScale = Vector3.one * num;
		this.EnableDynamics(true, true, false);
		this.balloonDynamics.EnableDistanceConstraints(false, num);
		this.lineRenderer.enabled = false;
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x000684FC File Offset: 0x000666FC
	public void OnTriggerEnter(Collider other)
	{
		if (!this.ShouldSimulate())
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		bool flag = false;
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.TriggerEnter(other, ref zero, ref zero2, ref flag);
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (this.worldShareableInstance == null)
		{
			return;
		}
		if (flag)
		{
			RequestableOwnershipGuard component = PhotonView.Get(this.worldShareableInstance.gameObject).GetComponent<RequestableOwnershipGuard>();
			if (!component.isTrulyMine)
			{
				if (zero.magnitude > this.forceAppliedAsRemote.magnitude)
				{
					this.forceAppliedAsRemote = zero;
					this.collisionPtAsRemote = zero2;
				}
				component.RequestOwnershipImmediately(delegate
				{
				});
			}
		}
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x000685C0 File Offset: 0x000667C0
	public void OnCollisionEnter(Collision collision)
	{
		if (!this.ShouldSimulate() || this.disableCollisionHandling)
		{
			return;
		}
		this.balloonBopSource.GTPlay();
		if (!collision.gameObject.IsOnLayer(UnityLayer.GorillaThrowable))
		{
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			this.OwnerPopBalloon();
			return;
		}
		if (this.worldShareableInstance == null)
		{
			return;
		}
		if (PhotonView.Get(this.worldShareableInstance.gameObject).IsMine)
		{
			this.OwnerPopBalloon();
		}
	}

	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06001556 RID: 5462 RVA: 0x00068638 File Offset: 0x00066838
	FXSystemSettings IFXContext.settings
	{
		get
		{
			return this.ownerRig.fxSettings;
		}
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x00068648 File Offset: 0x00066848
	void IFXContext.OnPlayFX()
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab, true);
		gameObject.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.balloonPopFXColor);
		}
	}

	// Token: 0x040017A5 RID: 6053
	private ITetheredObjectBehavior balloonDynamics;

	// Token: 0x040017A6 RID: 6054
	[SerializeField]
	private Renderer mesh;

	// Token: 0x040017A7 RID: 6055
	private LineRenderer lineRenderer;

	// Token: 0x040017A8 RID: 6056
	private Rigidbody rb;

	// Token: 0x040017A9 RID: 6057
	private NetPlayer originalOwner;

	// Token: 0x040017AA RID: 6058
	public GameObject balloonPopFXPrefab;

	// Token: 0x040017AB RID: 6059
	public Color balloonPopFXColor;

	// Token: 0x040017AC RID: 6060
	private float timer;

	// Token: 0x040017AD RID: 6061
	public float scaleTimerLength = 2f;

	// Token: 0x040017AE RID: 6062
	public float poppedTimerLength = 2.5f;

	// Token: 0x040017AF RID: 6063
	public float beginScale = 0.1f;

	// Token: 0x040017B0 RID: 6064
	public float bopSpeed = 1f;

	// Token: 0x040017B1 RID: 6065
	private Vector3 fullyInflatedScale;

	// Token: 0x040017B2 RID: 6066
	public AudioSource balloonBopSource;

	// Token: 0x040017B3 RID: 6067
	public AudioSource balloonInflatSource;

	// Token: 0x040017B4 RID: 6068
	private Vector3 forceAppliedAsRemote;

	// Token: 0x040017B5 RID: 6069
	private Vector3 collisionPtAsRemote;

	// Token: 0x040017B6 RID: 6070
	private WaterVolume waterVolume;

	// Token: 0x040017B7 RID: 6071
	[DebugReadout]
	private BalloonHoldable.BalloonStates balloonState;

	// Token: 0x040017B8 RID: 6072
	private float returnTimer;

	// Token: 0x040017B9 RID: 6073
	[SerializeField]
	private float maxDistanceFromOwner;

	// Token: 0x040017BA RID: 6074
	public float lastOwnershipRequest;

	// Token: 0x040017BB RID: 6075
	[SerializeField]
	private bool disableCollisionHandling;

	// Token: 0x040017BC RID: 6076
	[SerializeField]
	private bool disableRelease;

	// Token: 0x02000394 RID: 916
	private enum BalloonStates
	{
		// Token: 0x040017BE RID: 6078
		Normal,
		// Token: 0x040017BF RID: 6079
		Pop,
		// Token: 0x040017C0 RID: 6080
		Waiting,
		// Token: 0x040017C1 RID: 6081
		WaitForOwnershipTransfer,
		// Token: 0x040017C2 RID: 6082
		WaitForReDock,
		// Token: 0x040017C3 RID: 6083
		Refilling,
		// Token: 0x040017C4 RID: 6084
		Returning
	}
}
