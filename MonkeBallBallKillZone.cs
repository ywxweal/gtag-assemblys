using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004BF RID: 1215
public class MonkeBallBallKillZone : MonoBehaviour
{
	// Token: 0x06001D64 RID: 7524 RVA: 0x0008EC8C File Offset: 0x0008CE8C
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.RequestResetBall(component.id, -1);
				return;
			}
			GameBallManager.Instance.RequestSetBallPosition(component.id);
		}
	}
}
