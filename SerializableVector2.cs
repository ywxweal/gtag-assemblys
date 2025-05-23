using System;
using UnityEngine;

// Token: 0x0200027A RID: 634
[Serializable]
public struct SerializableVector2
{
	// Token: 0x06000E96 RID: 3734 RVA: 0x00049855 File Offset: 0x00047A55
	public SerializableVector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x00049865 File Offset: 0x00047A65
	public static implicit operator SerializableVector2(Vector2 v)
	{
		return new SerializableVector2(v.x, v.y);
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x00049878 File Offset: 0x00047A78
	public static implicit operator Vector2(SerializableVector2 v)
	{
		return new Vector2(v.x, v.y);
	}

	// Token: 0x040011C6 RID: 4550
	public float x;

	// Token: 0x040011C7 RID: 4551
	public float y;
}
