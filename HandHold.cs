using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion.Gameplay;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200065B RID: 1627
[RequireComponent(typeof(Collider))]
public class HandHold : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x14000058 RID: 88
	// (add) Token: 0x060028AF RID: 10415 RVA: 0x000CAD54 File Offset: 0x000C8F54
	// (remove) Token: 0x060028B0 RID: 10416 RVA: 0x000CAD88 File Offset: 0x000C8F88
	public static event HandHold.HandHoldPositionEvent HandPositionRequestOverride;

	// Token: 0x14000059 RID: 89
	// (add) Token: 0x060028B1 RID: 10417 RVA: 0x000CADBC File Offset: 0x000C8FBC
	// (remove) Token: 0x060028B2 RID: 10418 RVA: 0x000CADF0 File Offset: 0x000C8FF0
	public static event HandHold.HandHoldEvent HandPositionReleaseOverride;

	// Token: 0x060028B3 RID: 10419 RVA: 0x000CAE24 File Offset: 0x000C9024
	public void OnDisable()
	{
		for (int i = 0; i < this.currentGrabbers.Count; i++)
		{
			if (this.currentGrabbers[i].IsNotNull())
			{
				this.currentGrabbers[i].Ungrab(this);
			}
		}
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x000CAE6C File Offset: 0x000C906C
	private void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.myTappable = base.GetComponent<Tappable>();
		this.myCollider = base.GetComponent<Collider>();
		this.initialized = true;
	}

	// Token: 0x060028B5 RID: 10421 RVA: 0x00047642 File Offset: 0x00045842
	public virtual bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x000CAE98 File Offset: 0x000C9098
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		this.Initialize();
		grabbedTransform = base.transform;
		Vector3 position = g.transform.position;
		localGrabbedPosition = base.transform.InverseTransformPoint(position);
		Vector3 vector;
		g.Player.AddHandHold(base.transform, localGrabbedPosition, g, g.IsRightHand, this.rotatePlayerWhenHeld, out vector);
		this.currentGrabbers.AddIfNew(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionRequestOverride != null)
		{
			HandHold.HandPositionRequestOverride(this, g.IsRightHand, this.CalculateOffset(position));
		}
		UnityEvent<Vector3> onGrab = this.OnGrab;
		if (onGrab != null)
		{
			onGrab.Invoke(vector);
		}
		UnityEvent<HandHold> onGrabHandHold = this.OnGrabHandHold;
		if (onGrabHandHold != null)
		{
			onGrabHandHold.Invoke(this);
		}
		UnityEvent<bool> onGrabHanded = this.OnGrabHanded;
		if (onGrabHanded != null)
		{
			onGrabHanded.Invoke(g.IsRightHand);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnGrab();
		}
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x000CAF80 File Offset: 0x000C9180
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		this.Initialize();
		g.Player.RemoveHandHold(g, g.IsRightHand);
		this.currentGrabbers.Remove(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionReleaseOverride != null)
		{
			HandHold.HandPositionReleaseOverride(this, g.IsRightHand);
		}
		UnityEvent onRelease = this.OnRelease;
		if (onRelease != null)
		{
			onRelease.Invoke();
		}
		UnityEvent<HandHold> onReleaseHandHold = this.OnReleaseHandHold;
		if (onReleaseHandHold != null)
		{
			onReleaseHandHold.Invoke(this);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnRelease();
		}
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x000CB010 File Offset: 0x000C9210
	private Vector3 CalculateOffset(Vector3 position)
	{
		switch (this.handSnapMethod)
		{
		case HandHold.HandSnapMethod.SnapToNearestEdge:
			if (this.myCollider == null)
			{
				this.myCollider = base.GetComponent<Collider>();
				if (this.myCollider is MeshCollider && !(this.myCollider as MeshCollider).convex)
				{
					this.handSnapMethod = HandHold.HandSnapMethod.None;
					return Vector3.zero;
				}
			}
			return base.transform.position - this.myCollider.ClosestPoint(position);
		case HandHold.HandSnapMethod.SnapToXAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.right * base.transform.InverseTransformPoint(position).x);
		case HandHold.HandSnapMethod.SnapToYAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.up * base.transform.InverseTransformPoint(position).y);
		case HandHold.HandSnapMethod.SnapToZAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.forward * base.transform.InverseTransformPoint(position).z);
		default:
			return Vector3.zero;
		}
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x000CB14E File Offset: 0x000C934E
	public bool MomentaryGrabOnly()
	{
		return this.forceMomentary;
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x000CB156 File Offset: 0x000C9356
	public void CopyProperties(HandHoldSettings handHoldSettings)
	{
		this.handSnapMethod = (HandHold.HandSnapMethod)handHoldSettings.handSnapMethod;
		this.rotatePlayerWhenHeld = handHoldSettings.rotatePlayerWhenHeld;
		this.forceMomentary = !handHoldSettings.allowPreGrab;
	}

	// Token: 0x060028BC RID: 10428 RVA: 0x0001396B File Offset: 0x00011B6B
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04002DAE RID: 11694
	private Dictionary<Transform, Transform> attached = new Dictionary<Transform, Transform>();

	// Token: 0x04002DAF RID: 11695
	[SerializeField]
	private HandHold.HandSnapMethod handSnapMethod;

	// Token: 0x04002DB0 RID: 11696
	[SerializeField]
	private bool rotatePlayerWhenHeld;

	// Token: 0x04002DB1 RID: 11697
	[SerializeField]
	private UnityEvent<Vector3> OnGrab;

	// Token: 0x04002DB2 RID: 11698
	[SerializeField]
	private UnityEvent<HandHold> OnGrabHandHold;

	// Token: 0x04002DB3 RID: 11699
	[SerializeField]
	private UnityEvent<bool> OnGrabHanded;

	// Token: 0x04002DB4 RID: 11700
	[SerializeField]
	private UnityEvent OnRelease;

	// Token: 0x04002DB5 RID: 11701
	[SerializeField]
	private UnityEvent<HandHold> OnReleaseHandHold;

	// Token: 0x04002DB6 RID: 11702
	private bool initialized;

	// Token: 0x04002DB7 RID: 11703
	private Collider myCollider;

	// Token: 0x04002DB8 RID: 11704
	private Tappable myTappable;

	// Token: 0x04002DB9 RID: 11705
	[Tooltip("Turning this on disables \"pregrabbing\". Use pregrabbing to allow players to catch a handhold even if they have squeezed the trigger too soon. Useful if you're anticipating jumping players needed to grab while airborne")]
	[SerializeField]
	private bool forceMomentary = true;

	// Token: 0x04002DBA RID: 11706
	private List<GorillaGrabber> currentGrabbers = new List<GorillaGrabber>();

	// Token: 0x0200065C RID: 1628
	private enum HandSnapMethod
	{
		// Token: 0x04002DBC RID: 11708
		None,
		// Token: 0x04002DBD RID: 11709
		SnapToCenterPoint,
		// Token: 0x04002DBE RID: 11710
		SnapToNearestEdge,
		// Token: 0x04002DBF RID: 11711
		SnapToXAxisPoint,
		// Token: 0x04002DC0 RID: 11712
		SnapToYAxisPoint,
		// Token: 0x04002DC1 RID: 11713
		SnapToZAxisPoint
	}

	// Token: 0x0200065D RID: 1629
	// (Invoke) Token: 0x060028BE RID: 10430
	public delegate void HandHoldPositionEvent(HandHold hh, bool rh, Vector3 pos);

	// Token: 0x0200065E RID: 1630
	// (Invoke) Token: 0x060028C2 RID: 10434
	public delegate void HandHoldEvent(HandHold hh, bool rh);
}
