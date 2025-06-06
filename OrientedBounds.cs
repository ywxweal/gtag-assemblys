﻿using System;
using UnityEngine;

// Token: 0x02000776 RID: 1910
[Serializable]
public struct OrientedBounds
{
	// Token: 0x170004BC RID: 1212
	// (get) Token: 0x06002F97 RID: 12183 RVA: 0x000ED034 File Offset: 0x000EB234
	public static OrientedBounds Empty { get; } = new OrientedBounds
	{
		size = Vector3.zero,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x170004BD RID: 1213
	// (get) Token: 0x06002F98 RID: 12184 RVA: 0x000ED03B File Offset: 0x000EB23B
	public static OrientedBounds Identity { get; } = new OrientedBounds
	{
		size = Vector3.one,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x06002F99 RID: 12185 RVA: 0x000ED042 File Offset: 0x000EB242
	public Matrix4x4 TRS()
	{
		return Matrix4x4.TRS(this.center, this.rotation, this.size);
	}

	// Token: 0x0400361C RID: 13852
	public Vector3 size;

	// Token: 0x0400361D RID: 13853
	public Vector3 center;

	// Token: 0x0400361E RID: 13854
	public Quaternion rotation;
}
