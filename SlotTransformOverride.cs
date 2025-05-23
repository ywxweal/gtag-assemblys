using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000424 RID: 1060
[Serializable]
public class SlotTransformOverride
{
	// Token: 0x170002CE RID: 718
	// (get) Token: 0x060019DE RID: 6622 RVA: 0x0007E3C3 File Offset: 0x0007C5C3
	// (set) Token: 0x060019DF RID: 6623 RVA: 0x0007E3D0 File Offset: 0x0007C5D0
	private XformOffset _EdXformOffsetRepresenationOf_overrideTransformMatrix
	{
		get
		{
			return new XformOffset(this.overrideTransformMatrix);
		}
		set
		{
			this.overrideTransformMatrix = Matrix4x4.TRS(value.pos, value.rot, value.scale);
		}
	}

	// Token: 0x060019E0 RID: 6624 RVA: 0x0007E3F0 File Offset: 0x0007C5F0
	public void Initialize(Component component, Transform anchor)
	{
		if (!this.useAdvancedGrab)
		{
			return;
		}
		this.AdvOriginLocalToParentAnchorLocal = anchor.worldToLocalMatrix * this.advancedGrabPointOrigin.localToWorldMatrix;
		this.AdvAnchorLocalToAdvOriginLocal = this.advancedGrabPointOrigin.worldToLocalMatrix * this.advancedGrabPointAnchor.localToWorldMatrix;
		foreach (SubGrabPoint subGrabPoint in this.multiPoints)
		{
			if (subGrabPoint == null)
			{
				break;
			}
			subGrabPoint.InitializePoints(anchor, this.advancedGrabPointAnchor, this.advancedGrabPointOrigin);
		}
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x0007E49C File Offset: 0x0007C69C
	public void AddLineButton()
	{
		this.multiPoints.Add(new SubLineGrabPoint());
	}

	// Token: 0x060019E2 RID: 6626 RVA: 0x0007E4B0 File Offset: 0x0007C6B0
	public void AddSubGrabPoint(TransferrableObjectGripPosition togp)
	{
		SubGrabPoint subGrabPoint = togp.CreateSubGrabPoint(this);
		this.multiPoints.Add(subGrabPoint);
	}

	// Token: 0x04001CED RID: 7405
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	public Transform overrideTransform;

	// Token: 0x04001CEE RID: 7406
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	[Delayed]
	public string overrideTransform_path;

	// Token: 0x04001CEF RID: 7407
	public TransferrableObject.PositionState positionState;

	// Token: 0x04001CF0 RID: 7408
	public bool useAdvancedGrab;

	// Token: 0x04001CF1 RID: 7409
	public Matrix4x4 overrideTransformMatrix = Matrix4x4.identity;

	// Token: 0x04001CF2 RID: 7410
	public Transform advancedGrabPointAnchor;

	// Token: 0x04001CF3 RID: 7411
	public Transform advancedGrabPointOrigin;

	// Token: 0x04001CF4 RID: 7412
	[SerializeReference]
	public List<SubGrabPoint> multiPoints = new List<SubGrabPoint>();

	// Token: 0x04001CF5 RID: 7413
	public Matrix4x4 AdvOriginLocalToParentAnchorLocal;

	// Token: 0x04001CF6 RID: 7414
	public Matrix4x4 AdvAnchorLocalToAdvOriginLocal;
}
