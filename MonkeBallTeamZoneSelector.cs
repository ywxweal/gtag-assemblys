using System;
using UnityEngine;

// Token: 0x020004CC RID: 1228
public class MonkeBallTeamZoneSelector : MonoBehaviour
{
	// Token: 0x06001DC9 RID: 7625 RVA: 0x00090D0C File Offset: 0x0008EF0C
	private void OnTriggerEnter(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.IsLocalPlayer() && gamePlayer.teamId != this.teamId)
		{
			MonkeBallGame.Instance.RequestSetTeam(this.teamId);
		}
	}

	// Token: 0x040020E8 RID: 8424
	public int teamId;
}
