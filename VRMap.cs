using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020003D8 RID: 984
[Serializable]
public class VRMap
{
	// Token: 0x1700029C RID: 668
	// (get) Token: 0x060017B7 RID: 6071 RVA: 0x00073838 File Offset: 0x00071A38
	// (set) Token: 0x060017B8 RID: 6072 RVA: 0x00073845 File Offset: 0x00071A45
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x00073854 File Offset: 0x00071A54
	public void MapOther(float lerpValue)
	{
		this.rigTarget.localPosition = Vector3.Lerp(this.rigTarget.localPosition, this.syncPos, lerpValue);
		this.rigTarget.localRotation = Quaternion.Lerp(this.rigTarget.localRotation, this.syncRotation, lerpValue);
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x000738A8 File Offset: 0x00071AA8
	public void MapMine(float ratio, Transform playerOffsetTransform)
	{
		Vector3 position = this.rigTarget.position;
		if (this.overrideTarget != null)
		{
			this.rigTarget.rotation = this.overrideTarget.rotation * Quaternion.Euler(this.trackingRotationOffset);
			this.rigTarget.position = this.overrideTarget.position + this.rigTarget.rotation * this.trackingPositionOffset * ratio;
		}
		else
		{
			if (!this.hasInputDevice && ConnectedControllerHandler.Instance.GetValidForXRNode(this.vrTargetNode))
			{
				this.myInputDevice = InputDevices.GetDeviceAtXRNode(this.vrTargetNode);
				this.hasInputDevice = true;
			}
			if (this.hasInputDevice && this.myInputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.tempRotation) && this.myInputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.tempPosition))
			{
				this.rigTarget.rotation = this.tempRotation * Quaternion.Euler(this.trackingRotationOffset);
				this.rigTarget.position = this.tempPosition + this.rigTarget.rotation * this.trackingPositionOffset * ratio + playerOffsetTransform.position;
				this.rigTarget.RotateAround(playerOffsetTransform.position, Vector3.up, playerOffsetTransform.eulerAngles.y);
			}
		}
		if (this.handholdOverrideTarget != null)
		{
			this.rigTarget.position = Vector3.MoveTowards(position, this.handholdOverrideTarget.position - this.handholdOverrideTargetOffset + this.rigTarget.rotation * this.trackingPositionOffset * ratio, Time.deltaTime * 2f);
		}
	}

	// Token: 0x060017BB RID: 6075 RVA: 0x00073A81 File Offset: 0x00071C81
	public Vector3 GetExtrapolatedControllerPosition()
	{
		return this.rigTarget.position - this.rigTarget.rotation * this.trackingPositionOffset * this.rigTarget.lossyScale.x;
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x00073ABE File Offset: 0x00071CBE
	public virtual void MapOtherFinger(float handSync, float lerpValue)
	{
		this.calcT = handSync;
		this.LerpFinger(lerpValue, true);
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void MapMyFinger(float lerpValue)
	{
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void LerpFinger(float lerpValue, bool isOther)
	{
	}

	// Token: 0x04001A4F RID: 6735
	public XRNode vrTargetNode;

	// Token: 0x04001A50 RID: 6736
	public Transform overrideTarget;

	// Token: 0x04001A51 RID: 6737
	public Transform rigTarget;

	// Token: 0x04001A52 RID: 6738
	public Vector3 trackingPositionOffset;

	// Token: 0x04001A53 RID: 6739
	public Vector3 trackingRotationOffset;

	// Token: 0x04001A54 RID: 6740
	public Transform headTransform;

	// Token: 0x04001A55 RID: 6741
	internal NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x04001A56 RID: 6742
	public Quaternion syncRotation;

	// Token: 0x04001A57 RID: 6743
	public float calcT;

	// Token: 0x04001A58 RID: 6744
	private InputDevice myInputDevice;

	// Token: 0x04001A59 RID: 6745
	private bool hasInputDevice;

	// Token: 0x04001A5A RID: 6746
	private Vector3 tempPosition;

	// Token: 0x04001A5B RID: 6747
	private Quaternion tempRotation;

	// Token: 0x04001A5C RID: 6748
	public int tempInt;

	// Token: 0x04001A5D RID: 6749
	public Transform handholdOverrideTarget;

	// Token: 0x04001A5E RID: 6750
	public Vector3 handholdOverrideTargetOffset;
}
