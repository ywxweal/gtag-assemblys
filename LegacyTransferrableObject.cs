using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000408 RID: 1032
public class LegacyTransferrableObject : HoldableObject
{
	// Token: 0x060018FC RID: 6396 RVA: 0x00078EBC File Offset: 0x000770BC
	protected void Awake()
	{
		this.latched = false;
		this.initOffset = base.transform.localPosition;
		this.initRotation = base.transform.localRotation;
	}

	// Token: 0x060018FD RID: 6397 RVA: 0x00078EE8 File Offset: 0x000770E8
	protected virtual void Start()
	{
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.OnJoinedRoom));
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.OnLeftRoom));
		RoomSystem.PlayerJoinedEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerJoinedEvent, new Action<NetPlayer>(this.OnPlayerLeftRoom));
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x00078F58 File Offset: 0x00077158
	public void OnEnable()
	{
		if (this.myRig == null && this.myOnlineRig != null && this.myOnlineRig.netView != null && this.myOnlineRig.netView.IsMine)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.myRig == null && this.myOnlineRig == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.objectIndex = this.targetDock.ReturnTransferrableItemIndex(this.myIndex);
		if (this.myRig != null && this.myRig.isOfflineVRRig)
		{
			if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
			{
				this.storedZone = BodyDockPositions.DropPositions.LeftArm;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
			{
				this.storedZone = BodyDockPositions.DropPositions.RightArm;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
			{
				this.storedZone = BodyDockPositions.DropPositions.LeftBack;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
			{
				this.storedZone = BodyDockPositions.DropPositions.RightBack;
			}
			else
			{
				this.storedZone = BodyDockPositions.DropPositions.Chest;
			}
		}
		if (this.objectIndex == -1)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.currentState == TransferrableObject.PositionState.OnLeftArm && this.flipOnXForLeftArm)
		{
			Transform transform = this.GetAnchor(this.currentState);
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		this.initState = this.currentState;
		this.enabledOnFrame = Time.frameCount;
		this.SpawnShareableObject();
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x000790DE File Offset: 0x000772DE
	public void OnDisable()
	{
		this.enabledOnFrame = -1;
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x000790E8 File Offset: 0x000772E8
	private void SpawnShareableObject()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.canDrop && !this.shareable)
		{
			return;
		}
		if (this.worldShareableInstance != null)
		{
			return;
		}
		object[] array = new object[]
		{
			this.myIndex,
			PhotonNetwork.LocalPlayer
		};
		this.worldShareableInstance = PhotonNetwork.Instantiate("Objects/equipment/WorldShareableItem", base.transform.position, base.transform.rotation, 0, array);
		if (this.myRig != null && this.worldShareableInstance != null)
		{
			this.OnWorldShareableItemSpawn();
		}
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x00079184 File Offset: 0x00077384
	public void OnJoinedRoom()
	{
		Debug.Log("Here");
		this.SpawnShareableObject();
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x00079196 File Offset: 0x00077396
	public void OnLeftRoom()
	{
		if (this.worldShareableInstance != null)
		{
			PhotonNetwork.Destroy(this.worldShareableInstance);
		}
		this.OnWorldShareableItemDeallocated(NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x000791C1 File Offset: 0x000773C1
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.OnWorldShareableItemDeallocated(otherPlayer);
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x000791CA File Offset: 0x000773CA
	public void SetWorldShareableItem(GameObject item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnWorldShareableItemDeallocated(NetPlayer player)
	{
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x000791DC File Offset: 0x000773DC
	public virtual void LateUpdate()
	{
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
		}
		if (this.IsMyItem())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
		this.previousState = this.currentState;
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x0007922C File Offset: 0x0007742C
	protected Transform DefaultAnchor()
	{
		if (!(this.anchor == null))
		{
			return this.anchor;
		}
		return base.transform;
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x00079249 File Offset: 0x00077449
	private Transform GetAnchor(TransferrableObject.PositionState pos)
	{
		if (this.grabAnchor == null)
		{
			return this.DefaultAnchor();
		}
		if (this.InHand())
		{
			return this.grabAnchor;
		}
		return this.DefaultAnchor();
	}

	// Token: 0x0600190A RID: 6410 RVA: 0x00079278 File Offset: 0x00077478
	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x000792A8 File Offset: 0x000774A8
	private void UpdateFollowXform()
	{
		if (this.targetRig == null)
		{
			return;
		}
		if (this.targetDock == null)
		{
			this.targetDock = this.targetRig.GetComponent<BodyDockPositions>();
		}
		if (this.anchorOverrides == null)
		{
			this.anchorOverrides = this.targetRig.GetComponent<VRRigAnchorOverrides>();
		}
		Transform transform = this.GetAnchor(this.currentState);
		Transform transform2 = transform;
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState <= TransferrableObject.PositionState.InRightHand)
		{
			switch (positionState)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftArmTransform);
				break;
			case TransferrableObject.PositionState.OnRightArm:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightArmTransform);
				break;
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				break;
			case TransferrableObject.PositionState.InLeftHand:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftHandTransform);
				break;
			default:
				if (positionState == TransferrableObject.PositionState.InRightHand)
				{
					transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightHandTransform);
				}
				break;
			}
		}
		else if (positionState != TransferrableObject.PositionState.OnChest)
		{
			if (positionState != TransferrableObject.PositionState.OnLeftShoulder)
			{
				if (positionState == TransferrableObject.PositionState.OnRightShoulder)
				{
					transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightBackTransform);
				}
			}
			else
			{
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftBackTransform);
			}
		}
		else
		{
			transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.chestTransform);
		}
		LegacyTransferrableObject.InterpolateState interpolateState = this.interpState;
		if (interpolateState != LegacyTransferrableObject.InterpolateState.None)
		{
			if (interpolateState != LegacyTransferrableObject.InterpolateState.Interpolating)
			{
				return;
			}
			float num = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			transform.transform.position = Vector3.Lerp(this.interpStartPos, transform2.transform.position, num);
			transform.transform.rotation = Quaternion.Slerp(this.interpStartRot, transform2.transform.rotation, num);
			this.interpDt -= Time.deltaTime;
			if (this.interpDt <= 0f)
			{
				transform.parent = transform2;
				this.interpState = LegacyTransferrableObject.InterpolateState.None;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				if (this.flipOnXForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				if (this.flipOnYForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(1f, -1f, 1f);
				}
			}
		}
		else if (transform2 != transform.parent)
		{
			if (Time.frameCount == this.enabledOnFrame)
			{
				transform.parent = transform2;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				return;
			}
			this.interpState = LegacyTransferrableObject.InterpolateState.Interpolating;
			this.interpDt = this.interpTime;
			this.interpStartPos = transform.transform.position;
			this.interpStartRot = transform.transform.rotation;
			return;
		}
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x000795CD File Offset: 0x000777CD
	public void DropItem()
	{
		base.transform.parent = null;
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x000795DC File Offset: 0x000777DC
	protected virtual void LateUpdateShared()
	{
		this.disableItem = true;
		for (int i = 0; i < this.targetRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.targetRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
			{
				this.disableItem = false;
				break;
			}
		}
		if (this.disableItem)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.previousState != this.currentState && this.detatchOnGrab && this.InHand())
		{
			base.transform.parent = null;
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped)
		{
			this.UpdateFollowXform();
			return;
		}
		if (this.canDrop)
		{
			this.DropItem();
		}
	}

	// Token: 0x0600190E RID: 6414 RVA: 0x00079684 File Offset: 0x00077884
	protected void ResetXf()
	{
		if (this.canDrop)
		{
			Transform transform = this.DefaultAnchor();
			if (base.transform != transform && base.transform.parent != transform)
			{
				base.transform.parent = transform;
			}
			base.transform.localPosition = this.initOffset;
			base.transform.localRotation = this.initRotation;
		}
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x000796EF File Offset: 0x000778EF
	protected void ReDock()
	{
		if (this.IsMyItem())
		{
			this.currentState = this.initState;
		}
		this.ResetXf();
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x0007970C File Offset: 0x0007790C
	private void HandleLocalInput()
	{
		GameObject[] array;
		if (!this.InHand())
		{
			array = this.gameObjectsActiveOnlyWhileHeld;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			return;
		}
		array = this.gameObjectsActiveOnlyWhileHeld;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		XRNode xrnode = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? XRNode.LeftHand : XRNode.RightHand);
		this.indexTrigger = ControllerInputPoller.TriggerFloat(xrnode);
		bool flag = !this.latched && this.indexTrigger >= this.myThreshold;
		bool flag2 = this.latched && this.indexTrigger < this.myThreshold - this.hysterisis;
		if (flag || this.testActivate)
		{
			this.testActivate = false;
			if (this.CanActivate())
			{
				this.OnActivate();
				return;
			}
		}
		else if (flag2 || this.testDeactivate)
		{
			this.testDeactivate = false;
			if (this.CanDeactivate())
			{
				this.OnDeactivate();
			}
		}
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x000797F8 File Offset: 0x000779F8
	protected virtual void LateUpdateLocal()
	{
		this.wasHover = this.isHover;
		this.isHover = false;
		if (PhotonNetwork.InRoom)
		{
			this.myRig.SetTransferrablePosStates(this.objectIndex, this.currentState);
			this.myRig.SetTransferrableItemStates(this.objectIndex, this.itemState);
		}
		this.targetRig = this.myRig;
		this.HandleLocalInput();
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x00079860 File Offset: 0x00077A60
	protected virtual void LateUpdateReplicated()
	{
		this.currentState = this.myOnlineRig.TransferrablePosStates(this.objectIndex);
		if (this.currentState == TransferrableObject.PositionState.Dropped && !this.canDrop && !this.shareable)
		{
			if (this.previousState == TransferrableObject.PositionState.None)
			{
				base.gameObject.SetActive(false);
			}
			this.currentState = this.previousState;
		}
		this.itemState = this.myOnlineRig.TransferrableItemStates(this.objectIndex);
		this.targetRig = this.myOnlineRig;
		if (this.myOnlineRig != null)
		{
			bool flag = true;
			for (int i = 0; i < this.myOnlineRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myOnlineRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
				{
					flag = false;
					GameObject[] array = this.gameObjectsActiveOnlyWhileHeld;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].SetActive(this.InHand());
					}
				}
			}
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x00079952 File Offset: 0x00077B52
	public virtual void ResetToDefaultState()
	{
		this.canAutoGrabLeft = true;
		this.canAutoGrabRight = true;
		this.wasHover = false;
		this.isHover = false;
		this.ResetXf();
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x00079978 File Offset: 0x00077B78
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		if (!(grabbingHand == this.interactor.leftHand) || this.currentState == TransferrableObject.PositionState.OnLeftArm)
		{
			if (grabbingHand == this.interactor.rightHand && this.currentState != TransferrableObject.PositionState.OnRightArm)
			{
				if (this.currentState == TransferrableObject.PositionState.InLeftHand && this.disableStealing)
				{
					return;
				}
				this.canAutoGrabRight = false;
				this.currentState = TransferrableObject.PositionState.InRightHand;
				EquipmentInteractor.instance.UpdateHandEquipment(this, false);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			}
			return;
		}
		if (this.currentState == TransferrableObject.PositionState.InRightHand && this.disableStealing)
		{
			return;
		}
		this.canAutoGrabLeft = false;
		this.currentState = TransferrableObject.PositionState.InLeftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, true);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
	}

	// Token: 0x06001915 RID: 6421 RVA: 0x00079A80 File Offset: 0x00077C80
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!this.IsMyItem())
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (this.IsHeld() && ((releasingHand == EquipmentInteractor.instance.rightHand && EquipmentInteractor.instance.rightHandHeldEquipment != null && this == (LegacyTransferrableObject)EquipmentInteractor.instance.rightHandHeldEquipment) || (releasingHand == EquipmentInteractor.instance.leftHand && EquipmentInteractor.instance.leftHandHeldEquipment != null && this == (LegacyTransferrableObject)EquipmentInteractor.instance.leftHandHeldEquipment)))
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				this.canAutoGrabLeft = true;
			}
			else
			{
				this.canAutoGrabRight = true;
			}
			if (zoneReleased != null)
			{
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.LeftArm;
				bool flag2 = this.currentState == TransferrableObject.PositionState.InRightHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.RightArm;
				if (flag || flag2)
				{
					return false;
				}
				if (this.targetDock.DropZoneStorageUsed(zoneReleased.dropPosition) == -1 && zoneReleased.forBodyDock == this.targetDock && (zoneReleased.dropPosition & this.dockPositions) != BodyDockPositions.DropPositions.None)
				{
					this.storedZone = zoneReleased.dropPosition;
				}
			}
			this.DropItemCleanup();
			EquipmentInteractor.instance.UpdateHandEquipment(null, releasingHand == EquipmentInteractor.instance.leftHand);
			return true;
		}
		return false;
	}

	// Token: 0x06001916 RID: 6422 RVA: 0x00079BF4 File Offset: 0x00077DF4
	public override void DropItemCleanup()
	{
		if (this.canDrop)
		{
			this.currentState = TransferrableObject.PositionState.Dropped;
			return;
		}
		BodyDockPositions.DropPositions dropPositions = this.storedZone;
		switch (dropPositions)
		{
		case BodyDockPositions.DropPositions.LeftArm:
			this.currentState = TransferrableObject.PositionState.OnLeftArm;
			return;
		case BodyDockPositions.DropPositions.RightArm:
			this.currentState = TransferrableObject.PositionState.OnRightArm;
			return;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
			break;
		case BodyDockPositions.DropPositions.Chest:
			this.currentState = TransferrableObject.PositionState.OnChest;
			return;
		default:
			if (dropPositions == BodyDockPositions.DropPositions.LeftBack)
			{
				this.currentState = TransferrableObject.PositionState.OnLeftShoulder;
				return;
			}
			if (dropPositions != BodyDockPositions.DropPositions.RightBack)
			{
				return;
			}
			this.currentState = TransferrableObject.PositionState.OnRightShoulder;
			break;
		}
	}

	// Token: 0x06001917 RID: 6423 RVA: 0x00079C68 File Offset: 0x00077E68
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		if (!this.wasHover)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.isHover = true;
	}

	// Token: 0x06001918 RID: 6424 RVA: 0x00079CCC File Offset: 0x00077ECC
	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		VRRig vrrig = this.targetRig;
		if ((vrrig != null) ? vrrig.netView : null)
		{
			this.targetRig.rigSerializer.RPC_PlayHandTap(soundIndex, flag, 0.1f, default(PhotonMessageInfo));
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	// Token: 0x06001919 RID: 6425 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void PlayNote(int note, float volume)
	{
	}

	// Token: 0x0600191A RID: 6426 RVA: 0x00079D38 File Offset: 0x00077F38
	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
	}

	// Token: 0x0600191B RID: 6427 RVA: 0x00047642 File Offset: 0x00045842
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x0600191C RID: 6428 RVA: 0x00047642 File Offset: 0x00045842
	public virtual bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x0600191D RID: 6429 RVA: 0x00079D4A File Offset: 0x00077F4A
	public virtual void OnActivate()
	{
		this.latched = true;
	}

	// Token: 0x0600191E RID: 6430 RVA: 0x00079D53 File Offset: 0x00077F53
	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x00079D5C File Offset: 0x00077F5C
	public virtual bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06001920 RID: 6432 RVA: 0x00079D7C File Offset: 0x00077F7C
	protected virtual bool IsHeld()
	{
		return (EquipmentInteractor.instance.leftHandHeldEquipment != null && (LegacyTransferrableObject)EquipmentInteractor.instance.leftHandHeldEquipment == this) || (EquipmentInteractor.instance.rightHandHeldEquipment != null && (LegacyTransferrableObject)EquipmentInteractor.instance.rightHandHeldEquipment == this);
	}

	// Token: 0x06001921 RID: 6433 RVA: 0x00079DD9 File Offset: 0x00077FD9
	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001922 RID: 6434 RVA: 0x00079DEF File Offset: 0x00077FEF
	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x00079DFE File Offset: 0x00077FFE
	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x00079E09 File Offset: 0x00078009
	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x00079E14 File Offset: 0x00078014
	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x06001926 RID: 6438 RVA: 0x00079E20 File Offset: 0x00078020
	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	// Token: 0x06001927 RID: 6439 RVA: 0x00079E38 File Offset: 0x00078038
	protected NetPlayer OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.netView.Owner;
		}
		return NetworkSystem.Instance.LocalPlayer;
	}

	// Token: 0x04001BCA RID: 7114
	protected EquipmentInteractor interactor;

	// Token: 0x04001BCB RID: 7115
	public VRRig myRig;

	// Token: 0x04001BCC RID: 7116
	public VRRig myOnlineRig;

	// Token: 0x04001BCD RID: 7117
	public bool latched;

	// Token: 0x04001BCE RID: 7118
	private float indexTrigger;

	// Token: 0x04001BCF RID: 7119
	public bool testActivate;

	// Token: 0x04001BD0 RID: 7120
	public bool testDeactivate;

	// Token: 0x04001BD1 RID: 7121
	public float myThreshold = 0.8f;

	// Token: 0x04001BD2 RID: 7122
	public float hysterisis = 0.05f;

	// Token: 0x04001BD3 RID: 7123
	public bool flipOnXForLeftHand;

	// Token: 0x04001BD4 RID: 7124
	public bool flipOnYForLeftHand;

	// Token: 0x04001BD5 RID: 7125
	public bool flipOnXForLeftArm;

	// Token: 0x04001BD6 RID: 7126
	public bool disableStealing;

	// Token: 0x04001BD7 RID: 7127
	private TransferrableObject.PositionState initState;

	// Token: 0x04001BD8 RID: 7128
	public TransferrableObject.ItemStates itemState;

	// Token: 0x04001BD9 RID: 7129
	public BodyDockPositions.DropPositions storedZone;

	// Token: 0x04001BDA RID: 7130
	protected TransferrableObject.PositionState previousState;

	// Token: 0x04001BDB RID: 7131
	public TransferrableObject.PositionState currentState;

	// Token: 0x04001BDC RID: 7132
	public BodyDockPositions.DropPositions dockPositions;

	// Token: 0x04001BDD RID: 7133
	public VRRig targetRig;

	// Token: 0x04001BDE RID: 7134
	public BodyDockPositions targetDock;

	// Token: 0x04001BDF RID: 7135
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x04001BE0 RID: 7136
	public bool canAutoGrabLeft;

	// Token: 0x04001BE1 RID: 7137
	public bool canAutoGrabRight;

	// Token: 0x04001BE2 RID: 7138
	public int objectIndex;

	// Token: 0x04001BE3 RID: 7139
	[Tooltip("In Holdables.prefab, assign to the parent of this transform.\nExample: 'Holdables/YellowHandBootsRight' is the anchor of 'Holdables/YellowHandBootsRight/YELLOW HAND BOOTS'")]
	public Transform anchor;

	// Token: 0x04001BE4 RID: 7140
	[Tooltip("In Holdables.prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	// Token: 0x04001BE5 RID: 7141
	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	// Token: 0x04001BE6 RID: 7142
	public int myIndex;

	// Token: 0x04001BE7 RID: 7143
	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	// Token: 0x04001BE8 RID: 7144
	protected GameObject worldShareableInstance;

	// Token: 0x04001BE9 RID: 7145
	private float interpTime = 0.1f;

	// Token: 0x04001BEA RID: 7146
	private float interpDt;

	// Token: 0x04001BEB RID: 7147
	private Vector3 interpStartPos;

	// Token: 0x04001BEC RID: 7148
	private Quaternion interpStartRot;

	// Token: 0x04001BED RID: 7149
	protected int enabledOnFrame = -1;

	// Token: 0x04001BEE RID: 7150
	private Vector3 initOffset;

	// Token: 0x04001BEF RID: 7151
	private Quaternion initRotation;

	// Token: 0x04001BF0 RID: 7152
	public bool canDrop;

	// Token: 0x04001BF1 RID: 7153
	public bool shareable;

	// Token: 0x04001BF2 RID: 7154
	public bool detatchOnGrab;

	// Token: 0x04001BF3 RID: 7155
	private bool wasHover;

	// Token: 0x04001BF4 RID: 7156
	private bool isHover;

	// Token: 0x04001BF5 RID: 7157
	private bool disableItem;

	// Token: 0x04001BF6 RID: 7158
	public const int kPositionStateCount = 8;

	// Token: 0x04001BF7 RID: 7159
	public LegacyTransferrableObject.InterpolateState interpState;

	// Token: 0x02000409 RID: 1033
	public enum InterpolateState
	{
		// Token: 0x04001BF9 RID: 7161
		None,
		// Token: 0x04001BFA RID: 7162
		Interpolating
	}
}
