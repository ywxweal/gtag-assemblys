﻿using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000084 RID: 132
public class ReleaseCageWhenUpsideDown : MonoBehaviour
{
	// Token: 0x06000349 RID: 841 RVA: 0x00013BAA File Offset: 0x00011DAA
	private void Awake()
	{
		this.cage = base.GetComponentInChildren<CrittersCage>();
	}

	// Token: 0x0600034A RID: 842 RVA: 0x00013BB8 File Offset: 0x00011DB8
	private void Update()
	{
		this.cage.inReleasingPosition = Vector3.Angle(base.transform.up, Vector3.down) < this.releaseCritterThreshold;
	}

	// Token: 0x040003DD RID: 989
	public CrittersCage cage;

	// Token: 0x040003DE RID: 990
	[FormerlySerializedAs("dumpThreshold")]
	[FormerlySerializedAs("angle")]
	public float releaseCritterThreshold = 30f;
}
