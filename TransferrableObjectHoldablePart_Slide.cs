using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020003BF RID: 959
public class TransferrableObjectHoldablePart_Slide : TransferrableObjectHoldablePart
{
	// Token: 0x0600164A RID: 5706 RVA: 0x0006C2F8 File Offset: 0x0006A4F8
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		int num = (isHeldLeftHand ? 0 : 1);
		GTPlayer instance = GTPlayer.Instance;
		if (!rig.isOfflineVRRig)
		{
			Vector3 vector = (isHeldLeftHand ? instance.leftHandOffset : instance.rightHandOffset) * rig.scaleFactor;
			VRMap vrmap = (isHeldLeftHand ? rig.leftHand : rig.rightHand);
			this._snapToLine.target.position = vrmap.GetExtrapolatedControllerPosition() - vector;
			return;
		}
		Transform transform = ((num == 0) ? instance.leftControllerTransform : instance.rightControllerTransform);
		Vector3 position = transform.position;
		Vector3 snappedPoint = this._snapToLine.GetSnappedPoint(position);
		if (this._maxHandSnapDistance > 0f && (transform.position - snappedPoint).IsLongerThan(this._maxHandSnapDistance))
		{
			this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			return;
		}
		transform.position = snappedPoint;
		this._snapToLine.target.position = snappedPoint;
	}

	// Token: 0x040018D0 RID: 6352
	[SerializeField]
	private float _maxHandSnapDistance;

	// Token: 0x040018D1 RID: 6353
	[SerializeField]
	private SnapXformToLine _snapToLine;

	// Token: 0x040018D2 RID: 6354
	private const int LEFT = 0;

	// Token: 0x040018D3 RID: 6355
	private const int RIGHT = 1;
}
