using System;
using UnityEngine;

// Token: 0x020004BE RID: 1214
public class MonkeBallBallEjectZone : MonoBehaviour
{
	// Token: 0x06001D62 RID: 7522 RVA: 0x0008EC24 File Offset: 0x0008CE24
	private void OnCollisionEnter(Collision collision)
	{
		GameBall component = collision.gameObject.GetComponent<GameBall>();
		if (component != null && collision.contacts.Length != 0)
		{
			component.SetVelocity(collision.contacts[0].impulse.normalized * this.ejectVelocity);
		}
	}

	// Token: 0x04002089 RID: 8329
	public Transform target;

	// Token: 0x0400208A RID: 8330
	public float ejectVelocity = 15f;
}
