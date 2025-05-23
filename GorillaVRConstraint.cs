using System;
using UnityEngine;

// Token: 0x0200064F RID: 1615
public class GorillaVRConstraint : MonoBehaviour
{
	// Token: 0x06002854 RID: 10324 RVA: 0x000C9148 File Offset: 0x000C7348
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

	// Token: 0x04002D38 RID: 11576
	public bool isConstrained;

	// Token: 0x04002D39 RID: 11577
	public float angle = 3600f;
}
