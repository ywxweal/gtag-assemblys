using System;
using UnityEngine;

// Token: 0x0200064F RID: 1615
public class GorillaVRConstraint : MonoBehaviour
{
	// Token: 0x06002855 RID: 10325 RVA: 0x000C91EC File Offset: 0x000C73EC
	private void Update()
	{
		if (NetworkSystem.Instance.WrongVersion)
		{
			this.isConstrained = true;
		}
		if (this.isConstrained && Time.realtimeSinceStartup > this.angle)
		{
			GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
		}
	}

	// Token: 0x04002D3A RID: 11578
	public bool isConstrained;

	// Token: 0x04002D3B RID: 11579
	public float angle = 3600f;
}
