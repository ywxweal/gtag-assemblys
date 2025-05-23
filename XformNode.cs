using System;
using UnityEngine;

// Token: 0x02000786 RID: 1926
[Serializable]
public class XformNode
{
	// Token: 0x170004CD RID: 1229
	// (get) Token: 0x06003044 RID: 12356 RVA: 0x000EE9D8 File Offset: 0x000ECBD8
	public Vector4 worldPosition
	{
		get
		{
			if (!this.parent)
			{
				return this.localPosition;
			}
			Matrix4x4 localToWorldMatrix = this.parent.localToWorldMatrix;
			Vector4 vector = this.localPosition;
			MatrixUtils.MultiplyXYZ3x4(ref localToWorldMatrix, ref vector);
			return vector;
		}
	}

	// Token: 0x170004CE RID: 1230
	// (get) Token: 0x06003045 RID: 12357 RVA: 0x000EEA16 File Offset: 0x000ECC16
	// (set) Token: 0x06003046 RID: 12358 RVA: 0x000EEA23 File Offset: 0x000ECC23
	public float radius
	{
		get
		{
			return this.localPosition.w;
		}
		set
		{
			this.localPosition.w = value;
		}
	}

	// Token: 0x06003047 RID: 12359 RVA: 0x000EEA31 File Offset: 0x000ECC31
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, Quaternion.identity, Vector3.one);
	}

	// Token: 0x04003674 RID: 13940
	public Vector4 localPosition;

	// Token: 0x04003675 RID: 13941
	public Transform parent;
}
