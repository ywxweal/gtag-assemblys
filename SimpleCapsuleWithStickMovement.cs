using System;
using UnityEngine;

// Token: 0x020002F1 RID: 753
public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
	// Token: 0x1400003C RID: 60
	// (add) Token: 0x0600121C RID: 4636 RVA: 0x000561F0 File Offset: 0x000543F0
	// (remove) Token: 0x0600121D RID: 4637 RVA: 0x00056228 File Offset: 0x00054428
	public event Action CameraUpdated;

	// Token: 0x1400003D RID: 61
	// (add) Token: 0x0600121E RID: 4638 RVA: 0x00056260 File Offset: 0x00054460
	// (remove) Token: 0x0600121F RID: 4639 RVA: 0x00056298 File Offset: 0x00054498
	public event Action PreCharacterMove;

	// Token: 0x06001220 RID: 4640 RVA: 0x000562CD File Offset: 0x000544CD
	private void Awake()
	{
		this._rigidbody = base.GetComponent<Rigidbody>();
		if (this.CameraRig == null)
		{
			this.CameraRig = base.GetComponentInChildren<OVRCameraRig>();
		}
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Start()
	{
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x000562F8 File Offset: 0x000544F8
	private void FixedUpdate()
	{
		if (this.CameraUpdated != null)
		{
			this.CameraUpdated();
		}
		if (this.PreCharacterMove != null)
		{
			this.PreCharacterMove();
		}
		if (this.HMDRotatesPlayer)
		{
			this.RotatePlayerToHMD();
		}
		if (this.EnableLinearMovement)
		{
			this.StickMovement();
		}
		if (this.EnableRotation)
		{
			this.SnapTurn();
		}
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x00056358 File Offset: 0x00054558
	private void RotatePlayerToHMD()
	{
		Transform trackingSpace = this.CameraRig.trackingSpace;
		Transform centerEyeAnchor = this.CameraRig.centerEyeAnchor;
		Vector3 position = trackingSpace.position;
		Quaternion rotation = trackingSpace.rotation;
		base.transform.rotation = Quaternion.Euler(0f, centerEyeAnchor.rotation.eulerAngles.y, 0f);
		trackingSpace.position = position;
		trackingSpace.rotation = rotation;
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x000563C4 File Offset: 0x000545C4
	private void StickMovement()
	{
		Vector3 eulerAngles = this.CameraRig.centerEyeAnchor.rotation.eulerAngles;
		eulerAngles.z = (eulerAngles.x = 0f);
		Quaternion quaternion = Quaternion.Euler(eulerAngles);
		Vector3 vector = Vector3.zero;
		Vector2 vector2 = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Active);
		vector += quaternion * (vector2.x * Vector3.right);
		vector += quaternion * (vector2.y * Vector3.forward);
		this._rigidbody.MovePosition(this._rigidbody.position + vector * this.Speed * Time.fixedDeltaTime);
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x00056488 File Offset: 0x00054688
	private void SnapTurn()
	{
		if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.Active)))
		{
			if (this.ReadyToSnapTurn)
			{
				this.ReadyToSnapTurn = false;
				base.transform.RotateAround(this.CameraRig.centerEyeAnchor.position, Vector3.up, -this.RotationAngle);
				return;
			}
		}
		else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.Active)))
		{
			if (this.ReadyToSnapTurn)
			{
				this.ReadyToSnapTurn = false;
				base.transform.RotateAround(this.CameraRig.centerEyeAnchor.position, Vector3.up, this.RotationAngle);
				return;
			}
		}
		else
		{
			this.ReadyToSnapTurn = true;
		}
	}

	// Token: 0x0400143B RID: 5179
	public bool EnableLinearMovement = true;

	// Token: 0x0400143C RID: 5180
	public bool EnableRotation = true;

	// Token: 0x0400143D RID: 5181
	public bool HMDRotatesPlayer = true;

	// Token: 0x0400143E RID: 5182
	public bool RotationEitherThumbstick;

	// Token: 0x0400143F RID: 5183
	public float RotationAngle = 45f;

	// Token: 0x04001440 RID: 5184
	public float Speed;

	// Token: 0x04001441 RID: 5185
	public OVRCameraRig CameraRig;

	// Token: 0x04001442 RID: 5186
	private bool ReadyToSnapTurn;

	// Token: 0x04001443 RID: 5187
	private Rigidbody _rigidbody;
}
