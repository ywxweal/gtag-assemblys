using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000423 RID: 1059
[Serializable]
public class SubSplineGrabPoint : SubLineGrabPoint
{
	// Token: 0x060019D9 RID: 6617 RVA: 0x0007E28D File Offset: 0x0007C48D
	public override Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return CatmullRomSpline.Evaluate(this.controlPointsTransformsRelativeToGrabOrigin, advancedItemState.distAlongLine);
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x0007E2A0 File Offset: 0x0007C4A0
	public override void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		base.InitializePoints(anchor, grabPointAnchor, advancedGrabPointOrigin);
		this.controlPointsRelativeToGrabOrigin = new List<Vector3>();
		foreach (Transform transform in this.spline.controlPointTransforms)
		{
			this.controlPointsRelativeToGrabOrigin.Add(advancedGrabPointOrigin.InverseTransformPoint(transform.position));
			this.controlPointsTransformsRelativeToGrabOrigin.Add(advancedGrabPointOrigin.worldToLocalMatrix * transform.localToWorldMatrix);
		}
	}

	// Token: 0x060019DB RID: 6619 RVA: 0x0007E314 File Offset: 0x0007C514
	public override AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		Vector3 vector = objectTransform.InverseTransformPoint(handTransform.position);
		Vector3 vector2;
		return new AdvancedItemState.PreData
		{
			distAlongLine = CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPointsRelativeToGrabOrigin, vector, out vector2),
			pointType = AdvancedItemState.PointType.DistanceBased
		};
	}

	// Token: 0x060019DC RID: 6620 RVA: 0x0007E350 File Offset: 0x0007C550
	public override float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		Vector3 vector = objectTransform.InverseTransformPoint(handTransform.position);
		Vector3 vector2;
		CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPointsRelativeToGrabOrigin, vector, out vector2);
		return Vector3.SqrMagnitude(vector2 - vector);
	}

	// Token: 0x04001CEA RID: 7402
	public CatmullRomSpline spline;

	// Token: 0x04001CEB RID: 7403
	public List<Vector3> controlPointsRelativeToGrabOrigin = new List<Vector3>();

	// Token: 0x04001CEC RID: 7404
	public List<Matrix4x4> controlPointsTransformsRelativeToGrabOrigin = new List<Matrix4x4>();
}
