﻿using System;
using UnityEngine;

// Token: 0x02000A2A RID: 2602
[RequireComponent(typeof(GorillaVelocityEstimator))]
public class VelocityBasedActivator : MonoBehaviour
{
	// Token: 0x06003DEC RID: 15852 RVA: 0x0012612F File Offset: 0x0012432F
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
	}

	// Token: 0x06003DED RID: 15853 RVA: 0x00126140 File Offset: 0x00124340
	private void Update()
	{
		this.k += this.velocityEstimator.linearVelocity.sqrMagnitude;
		this.k = Mathf.Max(this.k - Time.deltaTime * this.decay, 0f);
		if (!this.active && this.k > this.threshold)
		{
			this.activate(true);
		}
		if (this.active && this.k < this.threshold)
		{
			this.activate(false);
		}
	}

	// Token: 0x06003DEE RID: 15854 RVA: 0x001261CC File Offset: 0x001243CC
	private void activate(bool v)
	{
		this.active = v;
		for (int i = 0; i < this.activationTargets.Length; i++)
		{
			this.activationTargets[i].SetActive(v);
		}
	}

	// Token: 0x06003DEF RID: 15855 RVA: 0x00126201 File Offset: 0x00124401
	private void OnDisable()
	{
		if (this.active)
		{
			this.activate(false);
		}
	}

	// Token: 0x0400425B RID: 16987
	[SerializeField]
	private GameObject[] activationTargets;

	// Token: 0x0400425C RID: 16988
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x0400425D RID: 16989
	private float k;

	// Token: 0x0400425E RID: 16990
	private bool active;

	// Token: 0x0400425F RID: 16991
	[SerializeField]
	private float decay = 1f;

	// Token: 0x04004260 RID: 16992
	[SerializeField]
	private float threshold = 1f;
}
