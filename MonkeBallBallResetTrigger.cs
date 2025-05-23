using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004C0 RID: 1216
public class MonkeBallBallResetTrigger : MonoBehaviour
{
	// Token: 0x06001D66 RID: 7526 RVA: 0x0008ECFC File Offset: 0x0008CEFC
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			GameBallPlayer gameBallPlayer = ((component.heldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.heldByActorNumber));
			if (gameBallPlayer == null)
			{
				gameBallPlayer = ((component.lastHeldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.lastHeldByActorNumber));
				if (gameBallPlayer == null)
				{
					return;
				}
			}
			this._lastBall = component;
			int num = gameBallPlayer.teamId;
			if (num == -1)
			{
				num = component.lastHeldByTeamId;
			}
			if (num >= 0 && num < this.teamMaterials.Length)
			{
				this.trigger.sharedMaterial = this.teamMaterials[num];
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(true, num);
			}
		}
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x0008EDB4 File Offset: 0x0008CFB4
	private void OnTriggerExit(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (component == this._lastBall)
			{
				this.trigger.sharedMaterial = this.neutralMaterial;
				this._lastBall = null;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(false, -1);
			}
		}
	}

	// Token: 0x0400208B RID: 8331
	public Renderer trigger;

	// Token: 0x0400208C RID: 8332
	public Material[] teamMaterials;

	// Token: 0x0400208D RID: 8333
	public Material neutralMaterial;

	// Token: 0x0400208E RID: 8334
	private GameBall _lastBall;
}
