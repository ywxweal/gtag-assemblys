using System;
using UnityEngine;

// Token: 0x02000786 RID: 1926
[Serializable]
public class XformNode
{
	// Token: 0x170004CD RID: 1229
	// (get) Token: 0x06003045 RID: 12357 RVA: 0x000EEA7C File Offset: 0x000ECC7C
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
	// (get) Token: 0x06003046 RID: 12358 RVA: 0x000EEABA File Offset: 0x000ECCBA
	// (set) Token: 0x06003047 RID: 12359 RVA: 0x000EEAC7 File Offset: 0x000ECCC7
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

	// Token: 0x06003048 RID: 12360 RVA: 0x000EEAD5 File Offset: 0x000ECCD5
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, Quaternion.identity, Vector3.one);
	}

	// Token: 0x04003676 RID: 13942
	public Vector4 localPosition;

	// Token: 0x04003677 RID: 13943
	public Transform parent;
}
