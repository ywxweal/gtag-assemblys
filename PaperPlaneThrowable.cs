﻿using System;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000465 RID: 1125
public class PaperPlaneThrowable : TransferrableObject
{
	// Token: 0x06001BA2 RID: 7074 RVA: 0x00087A04 File Offset: 0x00085C04
	private void OnLaunchRPC(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnLaunchRPC");
		if (sender != receiver)
		{
			return;
		}
		if (!this)
		{
			return;
		}
		int num = PaperPlaneThrowable.FetchViewID(this);
		int num2 = (int)args[0];
		if (num == -1)
		{
			return;
		}
		if (num2 == -1)
		{
			return;
		}
		if (num != num2)
		{
			return;
		}
		int num3 = (int)args[1];
		int throwableId = this.GetThrowableId();
		if (num3 != throwableId)
		{
			return;
		}
		Vector3 vector = (Vector3)args[2];
		Quaternion quaternion = (Quaternion)args[3];
		Vector3 vector2 = (Vector3)args[4];
		float num4 = 10000f;
		if ((in vector).IsValid(in num4) && (in quaternion).IsValid())
		{
			float num5 = 10000f;
			if ((in vector2).IsValid(in num5) && !this._renderer.forceRenderingOff)
			{
				this.LaunchProjectileLocal(vector, quaternion, vector2);
				return;
			}
		}
	}

	// Token: 0x06001BA3 RID: 7075 RVA: 0x00087ADA File Offset: 0x00085CDA
	internal override void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += this.OnPhotonEvent;
		this._lastWorldPos = base.transform.position;
		this._renderer.forceRenderingOff = false;
		base.OnEnable();
	}

	// Token: 0x06001BA4 RID: 7076 RVA: 0x00087B15 File Offset: 0x00085D15
	internal override void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= this.OnPhotonEvent;
		base.OnDisable();
	}

	// Token: 0x06001BA5 RID: 7077 RVA: 0x00087B34 File Offset: 0x00085D34
	private void OnPhotonEvent(EventData evData)
	{
		if (evData.Code != 176)
		{
			return;
		}
		object[] array = (object[])evData.CustomData;
		object obj = array[0];
		if (!(obj is int))
		{
			return;
		}
		int num = (int)obj;
		if (num != PaperPlaneThrowable.kProjectileEvent)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(evData.Sender);
		NetPlayer netPlayer = base.OwningPlayer();
		if (player != netPlayer)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(new PhotonMessageInfo(netPlayer.GetPlayerRef(), PhotonNetwork.ServerTimestamp, null), "OnPhotonEvent");
		if (!this.m_spamCheck.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		TransferrableObject.PositionState positionState = (TransferrableObject.PositionState)array[1];
		Vector3 vector = (Vector3)array[2];
		Quaternion quaternion = (Quaternion)array[3];
		Vector3 vector2 = (Vector3)array[4];
		TransferrableObject.PositionState positionState2 = positionState;
		if (positionState2 != TransferrableObject.PositionState.InLeftHand)
		{
			if (positionState2 != TransferrableObject.PositionState.InRightHand)
			{
				goto IL_00CE;
			}
			if (base.InRightHand())
			{
				goto IL_00CE;
			}
		}
		else if (base.InLeftHand())
		{
			goto IL_00CE;
		}
		return;
		IL_00CE:
		float num2 = 10000f;
		if ((in vector).IsValid(in num2) && (in quaternion).IsValid())
		{
			float num3 = 10000f;
			if ((in vector2).IsValid(in num3) && !this._renderer.forceRenderingOff && !base.myOnlineRig.IsNull() && base.myOnlineRig.IsPositionInRange(vector, 4f))
			{
				this.LaunchProjectileLocal(vector, quaternion, vector2);
				return;
			}
		}
	}

	// Token: 0x06001BA6 RID: 7078 RVA: 0x00087C77 File Offset: 0x00085E77
	protected override void Start()
	{
		base.Start();
		if (PaperPlaneThrowable._playerView == null)
		{
			PaperPlaneThrowable._playerView = Camera.main;
		}
	}

	// Token: 0x06001BA7 RID: 7079 RVA: 0x00087C96 File Offset: 0x00085E96
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (this._renderer.forceRenderingOff)
		{
			return;
		}
		base.OnGrab(pointGrabbed, grabbingHand);
	}

	// Token: 0x06001BA8 RID: 7080 RVA: 0x00087CB0 File Offset: 0x00085EB0
	private static int FetchViewID(PaperPlaneThrowable ppt)
	{
		NetPlayer netPlayer = ((ppt.myOnlineRig != null) ? ppt.myOnlineRig.creator : ((ppt.myRig != null) ? ((ppt.myRig.creator != null) ? ppt.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
		if (netPlayer == null)
		{
			return -1;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			return -1;
		}
		if (rigContainer.Rig.netView == null)
		{
			return -1;
		}
		return rigContainer.Rig.netView.ViewID;
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x00087D4C File Offset: 0x00085F4C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		TransferrableObject.PositionState currentState = this.currentState;
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
		{
			return false;
		}
		if (this._renderer.forceRenderingOff)
		{
			return false;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.rightHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(flag);
		Vector3 vector = base.transform.TransformPoint(Vector3.zero);
		Quaternion rotation = base.transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		PaperPlaneThrowable.FetchViewID(this);
		this.GetThrowableId();
		this.LaunchProjectileLocal(vector, rotation, averageVelocity);
		if (PaperPlaneThrowable.gRaiseOpts == null)
		{
			PaperPlaneThrowable.gRaiseOpts = RaiseEventOptions.Default;
			PaperPlaneThrowable.gRaiseOpts.Receivers = ReceiverGroup.Others;
		}
		PaperPlaneThrowable.gEventArgs[0] = PaperPlaneThrowable.kProjectileEvent;
		PaperPlaneThrowable.gEventArgs[1] = currentState;
		PaperPlaneThrowable.gEventArgs[2] = vector;
		PaperPlaneThrowable.gEventArgs[3] = rotation;
		PaperPlaneThrowable.gEventArgs[4] = averageVelocity;
		PhotonNetwork.RaiseEvent(176, PaperPlaneThrowable.gEventArgs, PaperPlaneThrowable.gRaiseOpts, SendOptions.SendReliable);
		return true;
	}

	// Token: 0x06001BAA RID: 7082 RVA: 0x00087E78 File Offset: 0x00086078
	private int GetThrowableId()
	{
		int num = this._throwableIdHash.GetValueOrDefault();
		if (this._throwableIdHash == null)
		{
			num = StaticHash.Compute(this._throwableID);
			this._throwableIdHash = new int?(num);
			return num;
		}
		return num;
	}

	// Token: 0x06001BAB RID: 7083 RVA: 0x00087EBC File Offset: 0x000860BC
	private void LaunchProjectileLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel)
	{
		if (releaseVel.sqrMagnitude <= this.minThrowSpeed * base.transform.lossyScale.z * base.transform.lossyScale.z)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this._projectilePrefab.gameObject, launchPos, true);
		gameObject.transform.localScale = base.transform.lossyScale;
		PaperPlaneProjectile component = gameObject.GetComponent<PaperPlaneProjectile>();
		component.OnHit += this.OnProjectileHit;
		component.ResetProjectile();
		component.SetVRRig(base.myRig);
		component.Launch(launchPos, launchRot, releaseVel);
		this._renderer.forceRenderingOff = true;
	}

	// Token: 0x06001BAC RID: 7084 RVA: 0x00087F65 File Offset: 0x00086165
	private void OnProjectileHit(Vector3 endPoint)
	{
		this._renderer.forceRenderingOff = false;
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x00087F74 File Offset: 0x00086174
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		Transform transform = base.transform;
		Vector3 position = transform.position;
		this._itemWorldVel = (position - this._lastWorldPos) / Time.deltaTime;
		Quaternion localRotation = transform.localRotation;
		this._itemWorldAngVel = PaperPlaneThrowable.CalcAngularVelocity(this._lastWorldRot, localRotation, Time.deltaTime);
		this._lastWorldRot = localRotation;
		this._lastWorldPos = position;
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x00087FDC File Offset: 0x000861DC
	private static Vector3 CalcAngularVelocity(Quaternion from, Quaternion to, float dt)
	{
		Vector3 vector = (to * Quaternion.Inverse(from)).eulerAngles;
		if (vector.x > 180f)
		{
			vector.x -= 360f;
		}
		if (vector.y > 180f)
		{
			vector.y -= 360f;
		}
		if (vector.z > 180f)
		{
			vector.z -= 360f;
		}
		vector *= 0.017453292f / dt;
		return vector;
	}

	// Token: 0x06001BAF RID: 7087 RVA: 0x00088064 File Offset: 0x00086264
	public override void DropItem()
	{
		base.DropItem();
	}

	// Token: 0x04001EAF RID: 7855
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04001EB0 RID: 7856
	[SerializeField]
	private GameObject _projectilePrefab;

	// Token: 0x04001EB1 RID: 7857
	[SerializeField]
	private float minThrowSpeed;

	// Token: 0x04001EB2 RID: 7858
	private static Camera _playerView;

	// Token: 0x04001EB3 RID: 7859
	private static PhotonEvent gLaunchRPC;

	// Token: 0x04001EB4 RID: 7860
	private CallLimiterWithCooldown m_spamCheck = new CallLimiterWithCooldown(5f, 4, 1f);

	// Token: 0x04001EB5 RID: 7861
	private static readonly int kProjectileEvent = StaticHash.Compute("PaperPlaneThrowable".GetStaticHash(), "LaunchProjectileLocal".GetStaticHash());

	// Token: 0x04001EB6 RID: 7862
	private static object[] gEventArgs = new object[5];

	// Token: 0x04001EB7 RID: 7863
	private static RaiseEventOptions gRaiseOpts;

	// Token: 0x04001EB8 RID: 7864
	[SerializeField]
	private string _throwableID;

	// Token: 0x04001EB9 RID: 7865
	private int? _throwableIdHash;

	// Token: 0x04001EBA RID: 7866
	[Space]
	private Vector3 _lastWorldPos;

	// Token: 0x04001EBB RID: 7867
	private Quaternion _lastWorldRot;

	// Token: 0x04001EBC RID: 7868
	[Space]
	private Vector3 _itemWorldVel;

	// Token: 0x04001EBD RID: 7869
	private Vector3 _itemWorldAngVel;
}
