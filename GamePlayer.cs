using System;
using System.IO;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000573 RID: 1395
public class GamePlayer : MonoBehaviour
{
	// Token: 0x06002216 RID: 8726 RVA: 0x000AAC70 File Offset: 0x000A8E70
	private void Awake()
	{
		this.handTransforms = new Transform[2];
		this.handTransforms[0] = this.leftHand;
		this.handTransforms[1] = this.rightHand;
		this.hands = new GamePlayer.HandData[2];
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
		this.newJoinZoneLimiter = new CallLimiter(10, 10f, 0.5f);
		this.netImpulseLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netGrabLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netThrowLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netStateLimiter = new CallLimiter(25, 1f, 0.5f);
	}

	// Token: 0x06002217 RID: 8727 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnEnable()
	{
	}

	// Token: 0x06002218 RID: 8728 RVA: 0x000AAD38 File Offset: 0x000A8F38
	public void SetGrabbed(GameEntityId gameBallId, int handIndex)
	{
		if (gameBallId.IsValid())
		{
			this.ClearGrabbedIfHeld(gameBallId);
		}
		GamePlayer.HandData handData = this.hands[handIndex];
		handData.grabbedEntityId = gameBallId;
		this.hands[handIndex] = handData;
	}

	// Token: 0x06002219 RID: 8729 RVA: 0x000AAD78 File Offset: 0x000A8F78
	public void Clear()
	{
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x000023F4 File Offset: 0x000005F4
	public void ClearZone()
	{
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000AAD98 File Offset: 0x000A8F98
	public void ClearGrabbedIfHeld(GameEntityId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedEntityId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x000AADD1 File Offset: 0x000A8FD1
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex);
	}

	// Token: 0x0600221D RID: 8733 RVA: 0x000AADDF File Offset: 0x000A8FDF
	public bool IsGrabbingDisabled()
	{
		return this.grabbingDisabled;
	}

	// Token: 0x0600221E RID: 8734 RVA: 0x000AADE7 File Offset: 0x000A8FE7
	public void DisableGrabbing(bool disable)
	{
		this.grabbingDisabled = disable;
	}

	// Token: 0x0600221F RID: 8735 RVA: 0x000AADF0 File Offset: 0x000A8FF0
	public bool IsHoldingEntity(GameEntityId gameEntityId, bool isLeftHand)
	{
		return this.GetGameEntityId(GamePlayer.GetHandIndex(isLeftHand)) == gameEntityId;
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x000AAE04 File Offset: 0x000A9004
	public bool IsHoldingEntity(bool isLeftHand)
	{
		return GameEntityManager.instance.GetGameEntity(this.GetGameEntityId(GamePlayer.GetHandIndex(isLeftHand))) != null;
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x000AAE24 File Offset: 0x000A9024
	public GameEntityId GetGameEntityId(bool isLeftHand)
	{
		return this.GetGameEntityId(GamePlayer.GetHandIndex(isLeftHand));
	}

	// Token: 0x06002222 RID: 8738 RVA: 0x000AAE32 File Offset: 0x000A9032
	public GameEntityId GetGameEntityId(int handIndex)
	{
		return this.hands[handIndex].grabbedEntityId;
	}

	// Token: 0x06002223 RID: 8739 RVA: 0x000AAE48 File Offset: 0x000A9048
	public int FindHandIndex(GameEntityId gameBallId)
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedEntityId == gameBallId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x000AAE84 File Offset: 0x000A9084
	public GameEntityId GetGameBallId()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedEntityId.IsValid())
			{
				return this.hands[i].grabbedEntityId;
			}
		}
		return GameEntityId.Invalid;
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x0008DD6B File Offset: 0x0008BF6B
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002226 RID: 8742 RVA: 0x0008DD71 File Offset: 0x0008BF71
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002227 RID: 8743 RVA: 0x000AAED4 File Offset: 0x000A90D4
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		if (player == null)
		{
			return null;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x06002228 RID: 8744 RVA: 0x000AAF09 File Offset: 0x000A9109
	public static GamePlayer GetGamePlayer(Player player)
	{
		if (player == null)
		{
			return null;
		}
		return GamePlayer.GetGamePlayer(player.ActorNumber);
	}

	// Token: 0x06002229 RID: 8745 RVA: 0x000AAF1B File Offset: 0x000A911B
	public static GamePlayer GetGamePlayer(int actorNumber)
	{
		if (actorNumber < 0)
		{
			return null;
		}
		return GamePlayer.GetGamePlayer(GamePlayer.GetRig(actorNumber));
	}

	// Token: 0x0600222A RID: 8746 RVA: 0x000AAF2E File Offset: 0x000A912E
	public static GamePlayer GetGamePlayer(VRRig rig)
	{
		if (rig == null)
		{
			return null;
		}
		return rig.GetComponent<GamePlayer>();
	}

	// Token: 0x0600222B RID: 8747 RVA: 0x000AAF44 File Offset: 0x000A9144
	public static GamePlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GamePlayer component = transform.GetComponent<GamePlayer>();
			if (component != null)
			{
				return component;
			}
			if (bodyOnly)
			{
				break;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x000AAF80 File Offset: 0x000A9180
	public static Transform GetHandTransform(VRRig rig, int handIndex)
	{
		if (handIndex < 0 || handIndex >= 2)
		{
			return null;
		}
		return GamePlayer.GetGamePlayer(rig).handTransforms[handIndex];
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x000AAF99 File Offset: 0x000A9199
	public bool IsLocal()
	{
		return GamePlayerLocal.instance != null && GamePlayerLocal.instance.gamePlayer == this;
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x000AAFC0 File Offset: 0x000A91C0
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player)
	{
		for (int i = 0; i < 2; i++)
		{
			int netId = GameEntity.GetNetId(this.hands[i].grabbedEntityId);
			writer.Write(netId);
			if (netId != -1)
			{
				long num = 0L;
				GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(this.hands[i].grabbedEntityId);
				if (gameEntity != null)
				{
					num = BitPackUtils.PackHandPosRotForNetwork(gameEntity.transform.localPosition, gameEntity.transform.localRotation);
				}
				writer.Write(num);
			}
		}
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x000AB048 File Offset: 0x000A9248
	public static void DeserializeNetworkState(BinaryReader reader, GamePlayer gamePlayer)
	{
		for (int i = 0; i < 2; i++)
		{
			int num = reader.ReadInt32();
			if (num != -1)
			{
				long num2 = reader.ReadInt64();
				GameEntityId idFromNetId = GameEntity.GetIdFromNetId(num);
				if (idFromNetId.IsValid())
				{
					GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(idFromNetId);
					if (num2 != 0L && !(gameEntity == null))
					{
						Vector3 vector;
						Quaternion quaternion;
						BitPackUtils.UnpackHandPosRotFromNetwork(num2, out vector, out quaternion);
						if (gamePlayer != null && gamePlayer.rig.OwningNetPlayer != null)
						{
							GameEntityManager.instance.GrabEntityOnCreate(idFromNetId, GamePlayer.IsLeftHand(i), vector, quaternion, gamePlayer.rig.OwningNetPlayer);
						}
					}
				}
			}
		}
	}

	// Token: 0x04002623 RID: 9763
	public VRRig rig;

	// Token: 0x04002624 RID: 9764
	public Transform leftHand;

	// Token: 0x04002625 RID: 9765
	public Transform rightHand;

	// Token: 0x04002626 RID: 9766
	private Transform[] handTransforms;

	// Token: 0x04002627 RID: 9767
	private GamePlayer.HandData[] hands;

	// Token: 0x04002628 RID: 9768
	public const int MAX_HANDS = 2;

	// Token: 0x04002629 RID: 9769
	public const int LEFT_HAND = 0;

	// Token: 0x0400262A RID: 9770
	public const int RIGHT_HAND = 1;

	// Token: 0x0400262B RID: 9771
	public CallLimiter newJoinZoneLimiter;

	// Token: 0x0400262C RID: 9772
	public CallLimiter netImpulseLimiter;

	// Token: 0x0400262D RID: 9773
	public CallLimiter netGrabLimiter;

	// Token: 0x0400262E RID: 9774
	public CallLimiter netThrowLimiter;

	// Token: 0x0400262F RID: 9775
	public CallLimiter netStateLimiter;

	// Token: 0x04002630 RID: 9776
	private bool grabbingDisabled;

	// Token: 0x02000574 RID: 1396
	private struct HandData
	{
		// Token: 0x04002631 RID: 9777
		public GameEntityId grabbedEntityId;
	}

	// Token: 0x02000575 RID: 1397
	public enum ZoneState
	{
		// Token: 0x04002633 RID: 9779
		NotInZone,
		// Token: 0x04002634 RID: 9780
		WaitingForState,
		// Token: 0x04002635 RID: 9781
		Active
	}
}
