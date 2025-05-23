﻿using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000421 RID: 1057
[Serializable]
public class SubGrabPoint
{
	// Token: 0x060019C8 RID: 6600 RVA: 0x0007DCF5 File Offset: 0x0007BEF5
	public virtual Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return this.gripPointLocalToAdvOriginLocal;
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x0007DCFD File Offset: 0x0007BEFD
	public virtual Quaternion GetRotationRelativeToObjectAnchor(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return this.gripRotation_ParentAnchorLocal;
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x0007DD05 File Offset: 0x0007BF05
	public virtual Vector3 GetGrabPositionRelativeToGrabPointOrigin(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return this.gripPoint_AdvOriginLocal;
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x0007DD10 File Offset: 0x0007BF10
	public virtual void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		if (this.gripPoint == null)
		{
			return;
		}
		this.gripPoint_AdvOriginLocal = advancedGrabPointOrigin.InverseTransformPoint(this.gripPoint.position);
		this.gripRotation_AdvOriginLocal = Quaternion.Inverse(advancedGrabPointOrigin.rotation) * this.gripPoint.rotation;
		this.advAnchor_ParentAnchorLocal = Quaternion.Inverse(anchor.rotation) * grabPointAnchor.rotation;
		this.gripRotation_ParentAnchorLocal = Quaternion.Inverse(anchor.rotation) * this.gripPoint.rotation;
		this.gripPointLocalToAdvOriginLocal = advancedGrabPointOrigin.worldToLocalMatrix * this.gripPoint.localToWorldMatrix;
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x0007DDBD File Offset: 0x0007BFBD
	public Vector3 GetPositionOnObject(Transform transferableObject, SlotTransformOverride slotTransformOverride)
	{
		return transferableObject.TransformPoint(this.gripPoint_AdvOriginLocal);
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x0007DDCC File Offset: 0x0007BFCC
	public virtual Matrix4x4 GetTransformFromPositionState(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride, Transform targetDockXf)
	{
		Quaternion quaternion = advancedItemState.deltaRotation;
		if (!(in quaternion).IsValid())
		{
			quaternion = Quaternion.identity;
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.zero, quaternion, Vector3.one);
		Matrix4x4 matrix4x2 = this.GetTransformation_GripPointLocalToAdvOriginLocal(advancedItemState.preData, slotTransformOverride) * matrix4x.inverse;
		Matrix4x4 matrix4x3 = slotTransformOverride.AdvAnchorLocalToAdvOriginLocal * matrix4x2.inverse;
		return slotTransformOverride.AdvOriginLocalToParentAnchorLocal * matrix4x3;
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x0007DE3C File Offset: 0x0007C03C
	public AdvancedItemState GetAdvancedItemStateFromHand(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		AdvancedItemState.PreData preData = this.GetPreData(objectTransform, handTransform, targetDock, slotTransformOverride);
		Matrix4x4 matrix4x = targetDock.localToWorldMatrix * slotTransformOverride.AdvOriginLocalToParentAnchorLocal * slotTransformOverride.AdvAnchorLocalToAdvOriginLocal;
		Matrix4x4 matrix4x2 = objectTransform.localToWorldMatrix * this.GetTransformation_GripPointLocalToAdvOriginLocal(preData, slotTransformOverride);
		Quaternion quaternion = (matrix4x.inverse * matrix4x2).rotation;
		Vector3 vector = quaternion * Vector3.up;
		Vector3 vector2 = quaternion * Vector3.right;
		Vector3 vector3 = quaternion * Vector3.forward;
		bool flag = false;
		Vector2 up = Vector2.up;
		float num = 0f;
		switch (this.limitAxis)
		{
		case LimitAxis.NoMovement:
			quaternion = Quaternion.identity;
			break;
		case LimitAxis.YAxis:
			if (this.allowReverseGrip)
			{
				if (Vector3.Dot(vector, Vector3.up) < 0f)
				{
					Debug.Log("Using Reverse Grip");
					flag = true;
					vector = Vector3.down;
				}
				else
				{
					vector = Vector3.up;
				}
			}
			else
			{
				vector = Vector3.up;
			}
			vector2 = Vector3.Cross(vector, vector3);
			vector3 = Vector3.Cross(vector2, vector);
			up = new Vector2(vector3.z, vector3.x);
			quaternion = Quaternion.LookRotation(vector3, vector);
			break;
		case LimitAxis.XAxis:
			vector2 = Vector3.right;
			vector3 = Vector3.Cross(vector2, vector);
			vector = Vector3.Cross(vector3, vector2);
			break;
		case LimitAxis.ZAxis:
			vector3 = Vector3.forward;
			vector2 = Vector3.Cross(vector, vector3);
			vector = Vector3.Cross(vector3, vector2);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return new AdvancedItemState
		{
			preData = preData,
			limitAxis = this.limitAxis,
			angle = num,
			reverseGrip = flag,
			angleVectorWhereUpIsStandard = up,
			deltaRotation = quaternion
		};
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x0007E000 File Offset: 0x0007C200
	public virtual AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		return new AdvancedItemState.PreData
		{
			pointType = AdvancedItemState.PointType.Standard
		};
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x0007E010 File Offset: 0x0007C210
	public virtual float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		Vector3 vector = objectTransform.InverseTransformPoint(handTransform.position);
		float num = Vector3.SqrMagnitude(this.gripPoint_AdvOriginLocal - vector);
		float num2;
		Vector3 vector2;
		(Quaternion.Inverse(objectTransform.rotation * this.gripRotation_AdvOriginLocal) * targetDock.rotation * this.advAnchor_ParentAnchorLocal).ToAngleAxis(out num2, out vector2);
		return num + Mathf.Abs(num2) * 0.0001f;
	}

	// Token: 0x04001CDB RID: 7387
	[FormerlySerializedAs("transform")]
	public Transform gripPoint;

	// Token: 0x04001CDC RID: 7388
	public LimitAxis limitAxis;

	// Token: 0x04001CDD RID: 7389
	public bool allowReverseGrip;

	// Token: 0x04001CDE RID: 7390
	private Vector3 gripPoint_AdvOriginLocal;

	// Token: 0x04001CDF RID: 7391
	private Vector3 gripPointOffset_AdvOriginLocal;

	// Token: 0x04001CE0 RID: 7392
	public Quaternion gripRotation_AdvOriginLocal;

	// Token: 0x04001CE1 RID: 7393
	public Quaternion advAnchor_ParentAnchorLocal;

	// Token: 0x04001CE2 RID: 7394
	public Quaternion gripRotation_ParentAnchorLocal;

	// Token: 0x04001CE3 RID: 7395
	public Matrix4x4 gripPointLocalToAdvOriginLocal;
}
