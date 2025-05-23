using System;
using UnityEngine;

// Token: 0x020004C1 RID: 1217
[Serializable]
public class MonkeBallTeam
{
	// Token: 0x0400208F RID: 8335
	public Color color;

	// Token: 0x04002090 RID: 8336
	public int score;

	// Token: 0x04002091 RID: 8337
	public Transform ballStartLocation;

	// Token: 0x04002092 RID: 8338
	public Transform ballLaunchPosition;

	// Token: 0x04002093 RID: 8339
	[Tooltip("The min/max random velocity of the ball when launched.")]
	public Vector2 ballLaunchVelocityRange = new Vector2(8f, 15f);

	// Token: 0x04002094 RID: 8340
	[Tooltip("The min/max random x-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleXRange = new Vector2(0f, 0f);

	// Token: 0x04002095 RID: 8341
	[Tooltip("The min/max random y-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleYRange = new Vector2(0f, 0f);
}
