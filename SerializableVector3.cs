using System;
using UnityEngine;

// Token: 0x0200027B RID: 635
[Serializable]
public struct SerializableVector3
{
	// Token: 0x06000E99 RID: 3737 RVA: 0x0004988B File Offset: 0x00047A8B
	public SerializableVector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x000498A2 File Offset: 0x00047AA2
	public static implicit operator SerializableVector3(Vector3 v)
	{
		return new SerializableVector3(v.x, v.y, v.z);
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x000498BB File Offset: 0x00047ABB
	public static implicit operator Vector3(SerializableVector3 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	// Token: 0x040011C8 RID: 4552
	public float x;

	// Token: 0x040011C9 RID: 4553
	public float y;

	// Token: 0x040011CA RID: 4554
	public float z;
}
