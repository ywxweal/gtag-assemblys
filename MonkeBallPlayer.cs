using System;
using UnityEngine;

// Token: 0x020004C6 RID: 1222
public class MonkeBallPlayer : MonoBehaviour
{
	// Token: 0x06001DAC RID: 7596 RVA: 0x000908DF File Offset: 0x0008EADF
	private void Awake()
	{
		if (this.gamePlayer == null)
		{
			this.gamePlayer = base.GetComponent<GameBallPlayer>();
		}
	}

	// Token: 0x040020C5 RID: 8389
	public GameBallPlayer gamePlayer;

	// Token: 0x040020C6 RID: 8390
	public MonkeBallGoalZone currGoalZone;
}
