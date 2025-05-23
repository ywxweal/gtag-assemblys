using System;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class SpeedDrivenAnim : MonoBehaviour
{
	// Token: 0x06000A23 RID: 2595 RVA: 0x000354B0 File Offset: 0x000336B0
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.animator = base.GetComponent<Animator>();
		this.keyHash = Animator.StringToHash(this.animKey);
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x000354DC File Offset: 0x000336DC
	private void Update()
	{
		float num = Mathf.InverseLerp(this.speed0, this.speed1, this.velocityEstimator.linearVelocity.magnitude);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, num, this.maxChangePerSecond * Time.deltaTime);
		this.animator.SetFloat(this.keyHash, this.currentBlend);
	}

	// Token: 0x04000C3B RID: 3131
	[SerializeField]
	private float speed0;

	// Token: 0x04000C3C RID: 3132
	[SerializeField]
	private float speed1 = 1f;

	// Token: 0x04000C3D RID: 3133
	[SerializeField]
	private float maxChangePerSecond = 1f;

	// Token: 0x04000C3E RID: 3134
	[SerializeField]
	private string animKey = "speed";

	// Token: 0x04000C3F RID: 3135
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000C40 RID: 3136
	private Animator animator;

	// Token: 0x04000C41 RID: 3137
	private int keyHash;

	// Token: 0x04000C42 RID: 3138
	private float currentBlend;
}
