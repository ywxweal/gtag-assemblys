using System;
using UnityEngine;

// Token: 0x020004B6 RID: 1206
public class GameBallPlayer : MonoBehaviour
{
	// Token: 0x06001D26 RID: 7462 RVA: 0x0008DB0C File Offset: 0x0008BD0C
	private void Awake()
	{
		this.hands = new GameBallPlayer.HandData[2];
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
		this.teamId = -1;
	}

	// Token: 0x06001D27 RID: 7463 RVA: 0x0008DB40 File Offset: 0x0008BD40
	public void CleanupPlayer()
	{
		MonkeBallPlayer component = base.GetComponent<MonkeBallPlayer>();
		if (component != null)
		{
			component.currGoalZone = null;
			for (int i = 0; i < MonkeBallGame.Instance.goalZones.Count; i++)
			{
				MonkeBallGame.Instance.goalZones[i].CleanupPlayer(component);
			}
		}
	}

	// Token: 0x06001D28 RID: 7464 RVA: 0x0008DB94 File Offset: 0x0008BD94
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		if (gameBallId.IsValid())
		{
			this.ClearGrabbedIfHeld(gameBallId);
		}
		GameBallPlayer.HandData handData = this.hands[handIndex];
		handData.grabbedGameBallId = gameBallId;
		this.hands[handIndex] = handData;
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x0008DBD4 File Offset: 0x0008BDD4
	public void ClearGrabbedIfHeld(GameBallId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x0008DC0D File Offset: 0x0008BE0D
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x0008DC1C File Offset: 0x0008BE1C
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x06001D2C RID: 7468 RVA: 0x0008DC43 File Offset: 0x0008BE43
	public void SetInGoalZone(bool inZone)
	{
		if (inZone)
		{
			this.inGoalZone++;
			return;
		}
		this.inGoalZone--;
	}

	// Token: 0x06001D2D RID: 7469 RVA: 0x0008DC68 File Offset: 0x0008BE68
	public bool IsHoldingBall()
	{
		return this.GetGameBallId().IsValid();
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x0008DC83 File Offset: 0x0008BE83
	public GameBallId GetGameBallId(int handIndex)
	{
		return this.hands[handIndex].grabbedGameBallId;
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x0008DC98 File Offset: 0x0008BE98
	public int FindHandIndex(GameBallId gameBallId)
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x0008DCD4 File Offset: 0x0008BED4
	public GameBallId GetGameBallId()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId.IsValid())
			{
				return this.hands[i].grabbedGameBallId;
			}
		}
		return GameBallId.Invalid;
	}

	// Token: 0x06001D31 RID: 7473 RVA: 0x0008DD23 File Offset: 0x0008BF23
	public bool IsLocalPlayer()
	{
		return VRRigCache.Instance.localRig.Creator.ActorNumber == this.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x06001D32 RID: 7474 RVA: 0x0008DD4B File Offset: 0x0008BF4B
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06001D33 RID: 7475 RVA: 0x0008DD51 File Offset: 0x0008BF51
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06001D34 RID: 7476 RVA: 0x0008DD5C File Offset: 0x0008BF5C
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		RigContainer rigContainer;
		if (player == null || player.IsNull || !VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x0008DD98 File Offset: 0x0008BF98
	public static GameBallPlayer GetGamePlayer(int actorNumber)
	{
		if (actorNumber < 0)
		{
			return null;
		}
		VRRig vrrig = GameBallPlayer.GetRig(actorNumber);
		if (vrrig == null)
		{
			return null;
		}
		return vrrig.GetComponent<GameBallPlayer>();
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x0008DDC4 File Offset: 0x0008BFC4
	public static GameBallPlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameBallPlayer component = transform.GetComponent<GameBallPlayer>();
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

	// Token: 0x0400205B RID: 8283
	public VRRig rig;

	// Token: 0x0400205C RID: 8284
	public int teamId;

	// Token: 0x0400205D RID: 8285
	private GameBallPlayer.HandData[] hands;

	// Token: 0x0400205E RID: 8286
	public const int MAX_HANDS = 2;

	// Token: 0x0400205F RID: 8287
	public const int LEFT_HAND = 0;

	// Token: 0x04002060 RID: 8288
	public const int RIGHT_HAND = 1;

	// Token: 0x04002061 RID: 8289
	private int inGoalZone;

	// Token: 0x020004B7 RID: 1207
	private struct HandData
	{
		// Token: 0x04002062 RID: 8290
		public GameBallId grabbedGameBallId;
	}
}
