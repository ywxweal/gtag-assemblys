using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004C5 RID: 1221
public class MonkeBallGoalZone : MonoBehaviour
{
	// Token: 0x06001DA7 RID: 7591 RVA: 0x00090730 File Offset: 0x0008E930
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (MonkeBallGame.Instance.GetGameState() == MonkeBallGame.GameState.Playing)
		{
			for (int i = 0; i < this.playersInGoalZone.Count; i++)
			{
				MonkeBallPlayer monkeBallPlayer = this.playersInGoalZone[i];
				if (monkeBallPlayer.gamePlayer.teamId != this.teamId)
				{
					GameBallId gameBallId = monkeBallPlayer.gamePlayer.GetGameBallId();
					if (gameBallId.IsValid())
					{
						MonkeBallGame.Instance.RequestScore(monkeBallPlayer.gamePlayer.teamId);
						GameBallId gameBallId2 = monkeBallPlayer.gamePlayer.GetGameBallId();
						int otherTeam = MonkeBallGame.Instance.GetOtherTeam(monkeBallPlayer.gamePlayer.teamId);
						if (MonkeBallGame.Instance.resetBallPositionOnScore)
						{
							MonkeBallGame.Instance.RequestResetBall(gameBallId2, otherTeam);
						}
						MonkeBallGame.Instance.RequestRestrictBallToTeamOnScore(gameBallId2, otherTeam);
						monkeBallPlayer.gamePlayer.ClearGrabbedIfHeld(gameBallId);
					}
				}
			}
		}
	}

	// Token: 0x06001DA8 RID: 7592 RVA: 0x00090810 File Offset: 0x0008EA10
	private void OnTriggerEnter(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.teamId != this.teamId)
		{
			MonkeBallPlayer component = gamePlayer.GetComponent<MonkeBallPlayer>();
			if (component != null)
			{
				component.currGoalZone = this;
				this.playersInGoalZone.Add(component);
			}
		}
	}

	// Token: 0x06001DA9 RID: 7593 RVA: 0x00090860 File Offset: 0x0008EA60
	private void OnTriggerExit(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.teamId != this.teamId)
		{
			MonkeBallPlayer component = gamePlayer.GetComponent<MonkeBallPlayer>();
			if (component != null)
			{
				component.currGoalZone = null;
				this.playersInGoalZone.Remove(component);
			}
		}
	}

	// Token: 0x06001DAA RID: 7594 RVA: 0x000908B0 File Offset: 0x0008EAB0
	public void CleanupPlayer(MonkeBallPlayer player)
	{
		this.playersInGoalZone.Remove(player);
	}

	// Token: 0x040020C3 RID: 8387
	public int teamId;

	// Token: 0x040020C4 RID: 8388
	public List<MonkeBallPlayer> playersInGoalZone;
}
