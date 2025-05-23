using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020006EA RID: 1770
public class GorillaThrowableController : MonoBehaviour
{
	// Token: 0x06002C18 RID: 11288 RVA: 0x000D9541 File Offset: 0x000D7741
	protected void Awake()
	{
		this.gorillaThrowableLayerMask = LayerMask.GetMask(new string[] { "GorillaThrowable" });
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x000D955C File Offset: 0x000D775C
	private void LateUpdate()
	{
		if (this.testCanGrab)
		{
			this.testCanGrab = false;
			this.CanGrabAnObject(this.rightHandController, out this.returnCollider);
			Debug.Log(this.returnCollider.gameObject, this.returnCollider.gameObject);
		}
		if (this.leftHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.LeftHand))
			{
				if (this.leftHandGrabbedObject != null)
				{
					this.leftHandGrabbedObject.ThrowThisThingo();
					this.leftHandGrabbedObject = null;
				}
				this.leftHandIsGrabbing = false;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.LeftHand))
		{
			this.leftHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.leftHandController, out this.returnCollider))
			{
				this.leftHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.leftHandGrabbedObject.Grabbed(this.leftHandController);
			}
		}
		if (this.rightHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.RightHand))
			{
				if (this.rightHandGrabbedObject != null)
				{
					this.rightHandGrabbedObject.ThrowThisThingo();
					this.rightHandGrabbedObject = null;
				}
				this.rightHandIsGrabbing = false;
				return;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.RightHand))
		{
			this.rightHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.rightHandController, out this.returnCollider))
			{
				this.rightHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.rightHandGrabbedObject.Grabbed(this.rightHandController);
			}
		}
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x000D96A8 File Offset: 0x000D78A8
	private bool CheckIfHandHasReleased(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue < 0.75f)
		{
			this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
			if (this.triggerValue < 0.75f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002C1B RID: 11291 RVA: 0x000D9724 File Offset: 0x000D7924
	private bool CheckIfHandHasGrabbed(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue > 0.75f)
		{
			return true;
		}
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		return this.triggerValue > 0.75f;
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x000D97A0 File Offset: 0x000D79A0
	private bool CanGrabAnObject(Transform handTransform, out Collider returnCollider)
	{
		this.magnitude = 100f;
		returnCollider = null;
		Debug.Log("trying:");
		if (Physics.OverlapSphereNonAlloc(handTransform.position, this.handRadius, this.colliders, this.gorillaThrowableLayerMask) > 0)
		{
			Debug.Log("found something!");
			this.minCollider = this.colliders[0];
			foreach (Collider collider in this.colliders)
			{
				if (collider != null)
				{
					Debug.Log("found this", collider);
					if ((collider.transform.position - handTransform.position).magnitude < this.magnitude)
					{
						this.minCollider = collider;
						this.magnitude = (collider.transform.position - handTransform.position).magnitude;
					}
				}
			}
			returnCollider = this.minCollider;
			return true;
		}
		return false;
	}

	// Token: 0x06002C1D RID: 11293 RVA: 0x000D9889 File Offset: 0x000D7A89
	public void GrabbableObjectHover(bool isLeft)
	{
		GorillaTagger.Instance.StartVibration(isLeft, this.hoverVibrationStrength, this.hoverVibrationDuration);
	}

	// Token: 0x0400323E RID: 12862
	public Transform leftHandController;

	// Token: 0x0400323F RID: 12863
	public Transform rightHandController;

	// Token: 0x04003240 RID: 12864
	public bool leftHandIsGrabbing;

	// Token: 0x04003241 RID: 12865
	public bool rightHandIsGrabbing;

	// Token: 0x04003242 RID: 12866
	public GorillaThrowable leftHandGrabbedObject;

	// Token: 0x04003243 RID: 12867
	public GorillaThrowable rightHandGrabbedObject;

	// Token: 0x04003244 RID: 12868
	public float hoverVibrationStrength = 0.25f;

	// Token: 0x04003245 RID: 12869
	public float hoverVibrationDuration = 0.05f;

	// Token: 0x04003246 RID: 12870
	public float handRadius = 0.05f;

	// Token: 0x04003247 RID: 12871
	private InputDevice rightDevice;

	// Token: 0x04003248 RID: 12872
	private InputDevice leftDevice;

	// Token: 0x04003249 RID: 12873
	private InputDevice inputDevice;

	// Token: 0x0400324A RID: 12874
	private float triggerValue;

	// Token: 0x0400324B RID: 12875
	private bool boolVar;

	// Token: 0x0400324C RID: 12876
	private Collider[] colliders = new Collider[10];

	// Token: 0x0400324D RID: 12877
	private Collider minCollider;

	// Token: 0x0400324E RID: 12878
	private Collider returnCollider;

	// Token: 0x0400324F RID: 12879
	private float magnitude;

	// Token: 0x04003250 RID: 12880
	public bool testCanGrab;

	// Token: 0x04003251 RID: 12881
	private int gorillaThrowableLayerMask;
}
