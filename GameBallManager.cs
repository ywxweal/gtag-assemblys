using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020004B4 RID: 1204
[NetworkBehaviourWeaved(0)]
public class GameBallManager : NetworkComponent
{
	// Token: 0x06001D07 RID: 7431 RVA: 0x0008C810 File Offset: 0x0008AA10
	protected override void Awake()
	{
		base.Awake();
		GameBallManager.Instance = this;
		this.gameBalls = new List<GameBall>(64);
		this.gameBallData = new List<GameBallData>(64);
		this._callLimiters = new CallLimiter[8];
		this._callLimiters[0] = new CallLimiter(50, 1f, 0.5f);
		this._callLimiters[1] = new CallLimiter(50, 1f, 0.5f);
		this._callLimiters[2] = new CallLimiter(25, 1f, 0.5f);
		this._callLimiters[3] = new CallLimiter(25, 1f, 0.5f);
		this._callLimiters[4] = new CallLimiter(10, 1f, 0.5f);
		this._callLimiters[5] = new CallLimiter(10, 1f, 0.5f);
		this._callLimiters[6] = new CallLimiter(10, 1f, 0.5f);
		this._callLimiters[7] = new CallLimiter(25, 1f, 0.5f);
	}

	// Token: 0x06001D08 RID: 7432 RVA: 0x0008C91C File Offset: 0x0008AB1C
	private bool ValidateCallLimits(GameBallManager.RPC rpcCall, PhotonMessageInfo info)
	{
		if (rpcCall < GameBallManager.RPC.RequestGrabBall || rpcCall >= GameBallManager.RPC.Count)
		{
			return false;
		}
		bool flag = this._callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		if (!flag)
		{
			this.ReportRPCCall(rpcCall, info, "Too many RPC Calls!");
		}
		return flag;
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x0008C957 File Offset: 0x0008AB57
	private void ReportRPCCall(GameBallManager.RPC rpcCall, PhotonMessageInfo info, string susReason)
	{
		GorillaNot.instance.SendReport(string.Format("Reason: {0}   RPC: {1}", susReason, rpcCall), info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06001D0A RID: 7434 RVA: 0x0008C98C File Offset: 0x0008AB8C
	public GameBallId AddGameBall(GameBall gameBall)
	{
		int count = this.gameBallData.Count;
		this.gameBalls.Add(gameBall);
		GameBallData gameBallData = default(GameBallData);
		this.gameBallData.Add(gameBallData);
		gameBall.id = new GameBallId(count);
		return gameBall.id;
	}

	// Token: 0x06001D0B RID: 7435 RVA: 0x0008C9D8 File Offset: 0x0008ABD8
	public GameBall GetGameBall(GameBallId id)
	{
		if (!id.IsValid())
		{
			return null;
		}
		int index = id.index;
		return this.gameBalls[index];
	}

	// Token: 0x06001D0C RID: 7436 RVA: 0x0008CA04 File Offset: 0x0008AC04
	public GameBallId TryGrabLocal(Vector3 handPosition, int teamId)
	{
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		GameBallId gameBallId = GameBallId.Invalid;
		float num = float.MaxValue;
		for (int i = 0; i < this.gameBalls.Count; i++)
		{
			if (this.gameBalls[i].onlyGrabTeamId == -1 || this.gameBalls[i].onlyGrabTeamId == teamId)
			{
				float sqrMagnitude = this.gameBalls[i].GetVelocity().sqrMagnitude;
				double num2 = 0.0625;
				if (sqrMagnitude > 2f)
				{
					num2 = 0.25;
				}
				float sqrMagnitude2 = (handPosition - this.gameBalls[i].transform.position).sqrMagnitude;
				if ((double)sqrMagnitude2 < num2 && sqrMagnitude2 < num)
				{
					gameBallId = this.gameBalls[i].id;
					num = sqrMagnitude2;
				}
			}
		}
		return gameBallId;
	}

	// Token: 0x06001D0D RID: 7437 RVA: 0x0008CAEC File Offset: 0x0008ACEC
	public void RequestGrabBall(GameBallId ballId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation)
	{
		this.GrabBall(ballId, isLeftHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		long num = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
		this.photonView.RPC("RequestGrabBallRPC", RpcTarget.MasterClient, new object[] { ballId.index, isLeftHand, num });
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06001D0E RID: 7438 RVA: 0x0008CB54 File Offset: 0x0008AD54
	[PunRPC]
	private void RequestGrabBallRPC(int gameBallIndex, bool isLeftHand, long packedPosRot, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestGrabBallRPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.ValidateCallLimits(GameBallManager.RPC.RequestGrabBall, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex > this.gameBalls.Count)
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "gameBallIndex out of array.");
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!(in vector).IsValid(in num) || !(in quaternion).IsValid())
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "localPosition or localRotation is invalid.");
			return;
		}
		bool flag = true;
		GameBall gameBall = this.gameBalls[gameBallIndex];
		if (gameBall != null)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				if (!rigContainer.Rig.IsPositionInRange(gameBall.transform.position, 25f))
				{
					flag = false;
					this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "gameBall exceeds max catch distance.");
				}
			}
			else
			{
				flag = false;
				this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "Cannot find VRRig for grabber.");
			}
			if (vector.sqrMagnitude > 25f)
			{
				flag = false;
				this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "gameBall exceeds max catch distance.");
			}
		}
		else
		{
			flag = false;
			this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "gameBall does not exist.");
		}
		if (flag)
		{
			this.photonView.RPC("GrabBallRPC", RpcTarget.All, new object[] { gameBallIndex, isLeftHand, packedPosRot, info.Sender });
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x06001D0F RID: 7439 RVA: 0x0008CCB8 File Offset: 0x0008AEB8
	[PunRPC]
	private void GrabBallRPC(int gameBallIndex, bool isLeftHand, long packedPosRot, Player grabbedBy, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "GrabBallRPC");
		if (!this.ValidateCallLimits(GameBallManager.RPC.GrabBall, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex > this.gameBalls.Count)
		{
			this.ReportRPCCall(GameBallManager.RPC.GrabBall, info, "gameBallIndex out of array.");
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!(in vector).IsValid(in num) || !(in quaternion).IsValid())
		{
			this.ReportRPCCall(GameBallManager.RPC.GrabBall, info, "localPosition or localRotation is invalid.");
			return;
		}
		this.GrabBall(new GameBallId(gameBallIndex), isLeftHand, vector, quaternion, NetPlayer.Get(grabbedBy));
	}

	// Token: 0x06001D10 RID: 7440 RVA: 0x0008CD58 File Offset: 0x0008AF58
	private void GrabBall(GameBallId gameBallId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(grabbedByPlayer, out rigContainer))
		{
			return;
		}
		GameBallData gameBallData = this.gameBallData[gameBallId.index];
		GameBall gameBall = this.gameBalls[gameBallId.index];
		GameBallPlayer gameBallPlayer = ((gameBall.heldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(gameBall.heldByActorNumber));
		int num = ((gameBallPlayer == null) ? (-1) : gameBallPlayer.FindHandIndex(gameBallId));
		bool flag = gameBall.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		int num2 = -1;
		if (gameBallPlayer != null)
		{
			gameBallPlayer.ClearGrabbedIfHeld(gameBallId);
			num2 = gameBallPlayer.teamId;
			if (num != -1 && flag)
			{
				GameBallPlayerLocal.instance.ClearGrabbed(num);
			}
		}
		BodyDockPositions myBodyDockPositions = rigContainer.Rig.myBodyDockPositions;
		Transform transform = (isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform);
		if (!grabbedByPlayer.IsLocal)
		{
			gameBall.SetVisualOffset(true);
		}
		gameBall.transform.SetParent(transform);
		gameBall.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
		}
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(grabbedByPlayer.ActorNumber);
		bool flag2 = num2 == gamePlayer.teamId;
		bool flag3 = gameBall.lastHeldByActorNumber != grabbedByPlayer.ActorNumber;
		MonkeBall component2 = gameBall.GetComponent<MonkeBall>();
		if (component2 != null)
		{
			component2.OnGrabbed();
			if (!flag2 && flag3)
			{
				component2.OnSwitchHeldByTeam(gamePlayer.teamId);
			}
		}
		gameBall.heldByActorNumber = grabbedByPlayer.ActorNumber;
		gameBall.lastHeldByActorNumber = gameBall.heldByActorNumber;
		gameBall.SetHeldByTeamId(gamePlayer.teamId);
		int handIndex = GameBallPlayer.GetHandIndex(isLeftHand);
		gamePlayer.SetGrabbed(gameBallId, handIndex);
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GameBallPlayerLocal.instance.SetGrabbed(gameBallId, GameBallPlayer.GetHandIndex(isLeftHand));
			GameBallPlayerLocal.instance.PlayCatchFx(isLeftHand);
		}
		gameBall.PlayCatchFx();
		if (component2 != null)
		{
			MonkeBallGame.Instance.OnBallGrabbed(gameBallId);
		}
	}

	// Token: 0x06001D11 RID: 7441 RVA: 0x0008CF60 File Offset: 0x0008B160
	public void RequestThrowBall(GameBallId ballId, bool isLeftHand, Vector3 velocity, Vector3 angVelocity)
	{
		GameBall gameBall = this.GetGameBall(ballId);
		if (gameBall == null)
		{
			return;
		}
		Vector3 position = gameBall.transform.position;
		Quaternion rotation = gameBall.transform.rotation;
		this.ThrowBall(ballId, isLeftHand, position, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		this.photonView.RPC("RequestThrowBallRPC", RpcTarget.MasterClient, new object[] { ballId.index, isLeftHand, position, rotation, velocity, angVelocity });
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06001D12 RID: 7442 RVA: 0x0008D008 File Offset: 0x0008B208
	[PunRPC]
	private void RequestThrowBallRPC(int gameBallIndex, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestThrowBallRPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.ValidateCallLimits(GameBallManager.RPC.RequestThrowBall, info))
		{
			return;
		}
		if (!this.ValidateThrowBallParams(gameBallIndex, position, rotation, velocity, angVelocity))
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestThrowBall, info, "ValidateThrowBallParams are invalid.");
			return;
		}
		if (this.gameBalls[gameBallIndex].heldByActorNumber != info.Sender.ActorNumber && this.gameBalls[gameBallIndex].lastHeldByActorNumber != info.Sender.ActorNumber && (this.gameBalls[gameBallIndex].heldByActorNumber != -1 || this.gameBalls[gameBallIndex].lastHeldByActorNumber != -1))
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestThrowBall, info, "gameBall is not held by the thrower.");
			return;
		}
		bool flag = true;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out rigContainer))
		{
			if ((rigContainer.Rig.transform.position - position).sqrMagnitude > 6.25f)
			{
				flag = false;
				this.ReportRPCCall(GameBallManager.RPC.RequestThrowBall, info, "gameBall distance exceeds max distance from hand.");
			}
		}
		else
		{
			flag = false;
			this.ReportRPCCall(GameBallManager.RPC.RequestThrowBall, info, "Player rig cannot be found for thrower.");
		}
		if (flag)
		{
			this.photonView.RPC("ThrowBallRPC", RpcTarget.All, new object[] { gameBallIndex, isLeftHand, position, rotation, velocity, angVelocity, info.Sender, info.SentServerTime });
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x06001D13 RID: 7443 RVA: 0x0008D1A8 File Offset: 0x0008B3A8
	[PunRPC]
	private void ThrowBallRPC(int gameBallIndex, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player thrownBy, double throwTime, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "ThrowBallRPC");
		if (!this.ValidateCallLimits(GameBallManager.RPC.ThrowBall, info))
		{
			return;
		}
		if (!this.ValidateThrowBallParams(gameBallIndex, position, rotation, velocity, angVelocity))
		{
			this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "ValidateThrowBallParams are invalid.");
			return;
		}
		if ((base.transform.position - position).sqrMagnitude > 6400f)
		{
			this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "gameBall distance exceeds max distance from arena.");
			return;
		}
		if (double.IsNaN(throwTime) || double.IsInfinity(throwTime))
		{
			this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "throwTime is not a valid value.");
			return;
		}
		float num = (float)(PhotonNetwork.Time - throwTime);
		if (num < -3f || num > 3f)
		{
			this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "Throw time delta exceeds range.");
			return;
		}
		GameBall gameBall = this.gameBalls[gameBallIndex];
		position = 0.5f * Physics.gravity * gameBall.gravityMult * num * num + velocity * num + position;
		velocity = Physics.gravity * gameBall.gravityMult * num + velocity;
		rotation *= Quaternion.Euler(angVelocity * Time.deltaTime);
		this.ThrowBall(new GameBallId(gameBallIndex), isLeftHand, position, rotation, velocity, angVelocity, NetPlayer.Get(thrownBy));
	}

	// Token: 0x06001D14 RID: 7444 RVA: 0x0008D310 File Offset: 0x0008B510
	private bool ValidateThrowBallParams(int gameBallIndex, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
	{
		if (gameBallIndex < 0 || gameBallIndex >= this.gameBalls.Count)
		{
			return false;
		}
		float num = 10000f;
		if ((in position).IsValid(in num) && (in rotation).IsValid())
		{
			float num2 = 10000f;
			if ((in velocity).IsValid(in num2))
			{
				float num3 = 10000f;
				if ((in angVelocity).IsValid(in num3))
				{
					return velocity.sqrMagnitude <= 1600f;
				}
			}
		}
		return false;
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x0008D380 File Offset: 0x0008B580
	private void ThrowBall(GameBallId gameBallId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer thrownByPlayer)
	{
		GameBall gameBall = this.gameBalls[gameBallId.index];
		if (!thrownByPlayer.IsLocal)
		{
			gameBall.SetVisualOffset(true);
		}
		gameBall.transform.SetParent(null);
		gameBall.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = position;
			component.rotation = rotation;
			component.velocity = velocity;
			component.angularVelocity = angVelocity;
		}
		gameBall.heldByActorNumber = -1;
		MonkeBall monkeBall = MonkeBall.Get(gameBall);
		if (monkeBall != null)
		{
			monkeBall.ClearCannotGrabTeamId();
		}
		bool flag = thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		int handIndex = GameBallPlayer.GetHandIndex(isLeftHand);
		RigContainer rigContainer;
		if (flag)
		{
			GameBallPlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
			GameBallPlayerLocal.instance.ClearGrabbed(handIndex);
			GameBallPlayerLocal.instance.PlayThrowFx(isLeftHand);
		}
		else if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(thrownByPlayer.ActorNumber), out rigContainer))
		{
			GameBallPlayer component2 = rigContainer.Rig.GetComponent<GameBallPlayer>();
			if (component2 != null)
			{
				component2.ClearGrabbedIfHeld(gameBallId);
			}
		}
		gameBall.PlayThrowFx();
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x0008D4AC File Offset: 0x0008B6AC
	public void RequestLaunchBall(GameBallId ballId, Vector3 velocity)
	{
		GameBall gameBall = this.GetGameBall(ballId);
		if (gameBall == null)
		{
			return;
		}
		Vector3 position = gameBall.transform.position;
		Quaternion rotation = gameBall.transform.rotation;
		this.LaunchBall(ballId, position, rotation, velocity);
		this.photonView.RPC("RequestLaunchBallRPC", RpcTarget.MasterClient, new object[] { ballId.index, position, rotation, velocity });
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x0008D534 File Offset: 0x0008B734
	[PunRPC]
	private void RequestLaunchBallRPC(int gameBallIndex, Vector3 position, Quaternion rotation, Vector3 velocity, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestLaunchBallRPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.ValidateCallLimits(GameBallManager.RPC.RequestLaunchBall, info))
		{
			return;
		}
		if (!this.ValidateThrowBallParams(gameBallIndex, position, rotation, velocity, Vector3.zero))
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestLaunchBall, info, "ValidateThrowBallParams are invalid.");
			return;
		}
		bool flag = true;
		if ((MonkeBallGame.Instance.BallLauncher.position - position).sqrMagnitude > 1f)
		{
			flag = false;
			this.ReportRPCCall(GameBallManager.RPC.RequestLaunchBall, info, "gameBall distance exceeds max distance from launcher.");
		}
		if (flag)
		{
			this.photonView.RPC("LaunchBallRPC", RpcTarget.All, new object[] { gameBallIndex, position, rotation, velocity, info.SentServerTime });
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x06001D18 RID: 7448 RVA: 0x0008D60C File Offset: 0x0008B80C
	[PunRPC]
	private void LaunchBallRPC(int gameBallIndex, Vector3 position, Quaternion rotation, Vector3 velocity, double throwTime, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "LaunchBallRPC");
		if (!this.ValidateCallLimits(GameBallManager.RPC.ThrowBall, info))
		{
			return;
		}
		if (!this.ValidateThrowBallParams(gameBallIndex, position, rotation, velocity, Vector3.zero))
		{
			this.ReportRPCCall(GameBallManager.RPC.LaunchBall, info, "ValidateThrowBallParams are invalid.");
			return;
		}
		float num = (float)(PhotonNetwork.Time - throwTime);
		if (num < -3f || num > 3f)
		{
			this.ReportRPCCall(GameBallManager.RPC.LaunchBall, info, "Throw time delta exceeds range.");
			return;
		}
		GameBall gameBall = this.gameBalls[gameBallIndex];
		position = 0.5f * Physics.gravity * gameBall.gravityMult * num * num + velocity * num + position;
		velocity = Physics.gravity * gameBall.gravityMult * num + velocity;
		this.LaunchBall(new GameBallId(gameBallIndex), position, rotation, velocity);
	}

	// Token: 0x06001D19 RID: 7449 RVA: 0x0008D704 File Offset: 0x0008B904
	private void LaunchBall(GameBallId gameBallId, Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		GameBall gameBall = this.gameBalls[gameBallId.index];
		gameBall.transform.SetParent(null);
		gameBall.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = position;
			component.rotation = rotation;
			component.velocity = velocity;
			component.angularVelocity = Vector3.zero;
		}
		gameBall.heldByActorNumber = -1;
		gameBall.lastHeldByActorNumber = -1;
		gameBall.WasLaunched();
		MonkeBall monkeBall = MonkeBall.Get(gameBall);
		if (monkeBall != null)
		{
			monkeBall.ClearCannotGrabTeamId();
			monkeBall.TriggerDelayedResync();
		}
		gameBall.PlayThrowFx();
		GameBallPlayerLocal.instance.gamePlayer.ClearAllGrabbed();
		GameBallPlayerLocal.instance.ClearAllGrabbed();
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x0008D7C8 File Offset: 0x0008B9C8
	public void RequestTeleportBall(GameBallId id, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.photonView.RPC("TeleportBallRPC", RpcTarget.All, new object[] { id.index, position, rotation, velocity, angularVelocity });
	}

	// Token: 0x06001D1B RID: 7451 RVA: 0x0008D828 File Offset: 0x0008BA28
	[PunRPC]
	private void TeleportBallRPC(int gameBallIndex, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "TeleportBallRPC");
		if (!this.ValidateCallLimits(GameBallManager.RPC.TeleportBall, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex >= this.gameBalls.Count)
		{
			this.ReportRPCCall(GameBallManager.RPC.TeleportBall, info, "gameBallIndex is out of range.");
			return;
		}
		float num = 10000f;
		if ((in position).IsValid(in num) && (in rotation).IsValid())
		{
			float num2 = 10000f;
			if ((in velocity).IsValid(in num2))
			{
				float num3 = 10000f;
				if ((in angularVelocity).IsValid(in num3))
				{
					if ((base.transform.position - position).sqrMagnitude > 6400f)
					{
						this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "gameBall distance exceeds max distance from arena.");
						return;
					}
					GameBallId gameBallId = new GameBallId(gameBallIndex);
					this.TeleportBall(gameBallId, position, rotation, velocity, angularVelocity);
					return;
				}
			}
		}
		this.ReportRPCCall(GameBallManager.RPC.TeleportBall, info, "Ball params are invalid.");
	}

	// Token: 0x06001D1C RID: 7452 RVA: 0x0008D910 File Offset: 0x0008BB10
	private void TeleportBall(GameBallId gameBallId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
	{
		int index = gameBallId.index;
		if (index < 0 || index >= this.gameBallData.Count)
		{
			return;
		}
		GameBallData gameBallData = this.gameBallData[index];
		GameBall gameBall = this.gameBalls[index];
		if (gameBall == null)
		{
			return;
		}
		gameBall.SetVisualOffset(false);
		gameBall.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = position;
			component.rotation = rotation;
			component.velocity = velocity;
			component.angularVelocity = angularVelocity;
		}
	}

	// Token: 0x06001D1D RID: 7453 RVA: 0x0008D9A4 File Offset: 0x0008BBA4
	public void RequestSetBallPosition(GameBallId ballId)
	{
		if (this.GetGameBall(ballId) == null)
		{
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		this.photonView.RPC("RequestSetBallPositionRPC", RpcTarget.MasterClient, new object[] { ballId.index });
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06001D1E RID: 7454 RVA: 0x0008D9F8 File Offset: 0x0008BBF8
	[PunRPC]
	private void RequestSetBallPositionRPC(int gameBallIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestSetBallPositionRPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.ValidateCallLimits(GameBallManager.RPC.RequestSetBallPosition, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex >= this.gameBalls.Count)
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestSetBallPosition, info, "gameBallIndex is out of range.");
			return;
		}
		GameBall gameBall = this.gameBalls[gameBallIndex];
		if (gameBall == null)
		{
			return;
		}
		if ((gameBall.transform.position - base.transform.position).sqrMagnitude > 6400f)
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestSetBallPosition, info, "Ball position is outside of arena.");
			return;
		}
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component == null)
		{
			return;
		}
		this.photonView.RPC("TeleportBallRPC", info.Sender, new object[]
		{
			gameBallIndex,
			gameBall.transform.position,
			gameBall.transform.rotation,
			component.velocity,
			component.angularVelocity
		});
	}

	// Token: 0x06001D1F RID: 7455 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06001D20 RID: 7456 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06001D21 RID: 7457 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001D22 RID: 7458 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001D24 RID: 7460 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002047 RID: 8263
	[OnEnterPlay_SetNull]
	public static volatile GameBallManager Instance;

	// Token: 0x04002048 RID: 8264
	public PhotonView photonView;

	// Token: 0x04002049 RID: 8265
	private List<GameBall> gameBalls;

	// Token: 0x0400204A RID: 8266
	private List<GameBallData> gameBallData;

	// Token: 0x0400204B RID: 8267
	public const float MAX_LOCAL_MAGNITUDE_SQR = 6400f;

	// Token: 0x0400204C RID: 8268
	private const float MAX_LAUNCHER_DISTANCE_SQR = 1f;

	// Token: 0x0400204D RID: 8269
	public const float MAX_CATCH_DISTANCE_FROM_HAND_SQR = 25f;

	// Token: 0x0400204E RID: 8270
	public const float MAX_DISTANCE_FROM_HAND_SQR = 6.25f;

	// Token: 0x0400204F RID: 8271
	public const float MAX_THROW_VELOCITY_SQR = 1600f;

	// Token: 0x04002050 RID: 8272
	private CallLimiter[] _callLimiters;

	// Token: 0x020004B5 RID: 1205
	private enum RPC
	{
		// Token: 0x04002052 RID: 8274
		RequestGrabBall,
		// Token: 0x04002053 RID: 8275
		GrabBall,
		// Token: 0x04002054 RID: 8276
		RequestThrowBall,
		// Token: 0x04002055 RID: 8277
		ThrowBall,
		// Token: 0x04002056 RID: 8278
		RequestLaunchBall,
		// Token: 0x04002057 RID: 8279
		LaunchBall,
		// Token: 0x04002058 RID: 8280
		TeleportBall,
		// Token: 0x04002059 RID: 8281
		RequestSetBallPosition,
		// Token: 0x0400205A RID: 8282
		Count
	}
}
