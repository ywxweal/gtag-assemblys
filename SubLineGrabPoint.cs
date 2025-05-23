using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000422 RID: 1058
[Serializable]
public class SubLineGrabPoint : SubGrabPoint
{
	// Token: 0x060019D2 RID: 6610 RVA: 0x0007E0A0 File Offset: 0x0007C2A0
	public override Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		float distAlongLine = advancedItemState.distAlongLine;
		Vector3 vector = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), distAlongLine);
		Quaternion quaternion = Quaternion.Slerp(this.startPointRelativeTransformToGrabPointOrigin.rotation, this.endPointRelativeTransformToGrabPointOrigin.rotation, distAlongLine);
		return Matrix4x4.TRS(vector, quaternion, Vector3.one);
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x0007E0F8 File Offset: 0x0007C2F8
	public override void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		base.InitializePoints(anchor, grabPointAnchor, advancedGrabPointOrigin);
		if (this.startPoint == null || this.endPoint == null)
		{
			return;
		}
		this.startPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.startPoint.position);
		this.endPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.endPoint.position);
		this.endPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.endPoint.localToWorldMatrix;
		this.startPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.startPoint.localToWorldMatrix;
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x0007E191 File Offset: 0x0007C391
	public override AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		return new AdvancedItemState.PreData
		{
			distAlongLine = SubLineGrabPoint.<GetPreData>g__FindNearestFractionOnLine|8_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position),
			pointType = AdvancedItemState.PointType.DistanceBased
		};
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x0007E1C8 File Offset: 0x0007C3C8
	public override float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		float num = SubLineGrabPoint.<EvaluateScore>g__FindNearestFractionOnLine|9_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position);
		Vector3 vector = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), num);
		Vector3 vector2 = objectTransform.InverseTransformPoint(handTransform.position);
		return Vector3.SqrMagnitude(vector - vector2);
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x0007E238 File Offset: 0x0007C438
	[CompilerGenerated]
	internal static float <GetPreData>g__FindNearestFractionOnLine|8_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x0007E274 File Offset: 0x0007C474
	[CompilerGenerated]
	internal static float <EvaluateScore>g__FindNearestFractionOnLine|9_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x04001CE4 RID: 7396
	public Transform startPoint;

	// Token: 0x04001CE5 RID: 7397
	public Transform endPoint;

	// Token: 0x04001CE6 RID: 7398
	public Vector3 startPointRelativeToGrabPointOrigin;

	// Token: 0x04001CE7 RID: 7399
	public Vector3 endPointRelativeToGrabPointOrigin;

	// Token: 0x04001CE8 RID: 7400
	public Matrix4x4 startPointRelativeTransformToGrabPointOrigin;

	// Token: 0x04001CE9 RID: 7401
	public Matrix4x4 endPointRelativeTransformToGrabPointOrigin;
}
