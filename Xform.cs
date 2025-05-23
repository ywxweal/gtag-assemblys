using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000785 RID: 1925
[ExecuteAlways]
public class Xform : MonoBehaviour
{
	// Token: 0x170004CC RID: 1228
	// (get) Token: 0x0600303F RID: 12351 RVA: 0x000EE7EF File Offset: 0x000EC9EF
	public float3 localExtents
	{
		get
		{
			return this.localScale * 0.5f;
		}
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x000EE801 File Offset: 0x000ECA01
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, this.localRotation, this.localScale);
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x000EE824 File Offset: 0x000ECA24
	public Matrix4x4 TRS()
	{
		if (this.parent.AsNull<Transform>() == null)
		{
			return this.LocalTRS();
		}
		return this.parent.localToWorldMatrix * this.LocalTRS();
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x000EE858 File Offset: 0x000ECA58
	private unsafe void Update()
	{
		Matrix4x4 matrix4x = this.TRS();
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(matrix4x))
		{
			using (commandBuilder.WithLineWidth(2f, true))
			{
				commandBuilder.PlaneWithNormal(Xform.AXIS_XR_RT * 0.5f, Xform.AXIS_XR_RT, Xform.F2_ONE, Xform.CR);
				commandBuilder.PlaneWithNormal(Xform.AXIS_YG_UP * 0.5f, Xform.AXIS_YG_UP, Xform.F2_ONE, Xform.CG);
				commandBuilder.PlaneWithNormal(Xform.AXIS_ZB_FW * 0.5f, Xform.AXIS_ZB_FW, Xform.F2_ONE, Xform.CB);
				commandBuilder.WireBox(float3.zero, quaternion.identity, 1f, this.displayColor);
			}
		}
	}

	// Token: 0x04003669 RID: 13929
	public Transform parent;

	// Token: 0x0400366A RID: 13930
	[Space]
	public Color displayColor = SRand.New().NextColor();

	// Token: 0x0400366B RID: 13931
	[Space]
	public float3 localPosition = float3.zero;

	// Token: 0x0400366C RID: 13932
	public float3 localScale = Vector3.one;

	// Token: 0x0400366D RID: 13933
	public Quaternion localRotation = quaternion.identity;

	// Token: 0x0400366E RID: 13934
	private static readonly float3 F3_ONE = 1f;

	// Token: 0x0400366F RID: 13935
	private static readonly float2 F2_ONE = 1f;

	// Token: 0x04003670 RID: 13936
	private static readonly float3 AXIS_ZB_FW = new float3(0f, 0f, 1f);

	// Token: 0x04003671 RID: 13937
	private static readonly float3 AXIS_YG_UP = new float3(0f, 1f, 0f);

	// Token: 0x04003672 RID: 13938
	private static readonly float3 AXIS_XR_RT = new float3(1f, 0f, 0f);

	// Token: 0x04003673 RID: 13939
	private static readonly Color CR = new Color(1f, 0f, 0f, 0.24f);

	// Token: 0x04003674 RID: 13940
	private static readonly Color CG = new Color(0f, 1f, 0f, 0.24f);

	// Token: 0x04003675 RID: 13941
	private static readonly Color CB = new Color(0f, 0f, 1f, 0.24f);
}
