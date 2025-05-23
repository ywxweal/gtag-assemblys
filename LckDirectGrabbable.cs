using System;
using GorillaLocomotion.Gameplay;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

// Token: 0x0200025D RID: 605
public class LckDirectGrabbable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06000DCE RID: 3534 RVA: 0x00047314 File Offset: 0x00045514
	// (remove) Token: 0x06000DCF RID: 3535 RVA: 0x0004734C File Offset: 0x0004554C
	public event Action onGrabbed;

	// Token: 0x14000027 RID: 39
	// (add) Token: 0x06000DD0 RID: 3536 RVA: 0x00047384 File Offset: 0x00045584
	// (remove) Token: 0x06000DD1 RID: 3537 RVA: 0x000473BC File Offset: 0x000455BC
	public event Action onReleased;

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000DD2 RID: 3538 RVA: 0x000473F1 File Offset: 0x000455F1
	public GorillaGrabber grabber
	{
		get
		{
			return this._grabber;
		}
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000DD3 RID: 3539 RVA: 0x000473F9 File Offset: 0x000455F9
	public bool isGrabbed
	{
		get
		{
			return this._grabber != null;
		}
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x00047407 File Offset: 0x00045607
	public Vector3 GetLocalGrabbedPosition(GorillaGrabber grabber)
	{
		if (grabber == null)
		{
			return Vector3.zero;
		}
		return base.transform.InverseTransformPoint(grabber.transform.position);
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0004742E File Offset: 0x0004562E
	public bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return this._grabber == null || grabber == this._grabber;
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0004744C File Offset: 0x0004564C
	public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		if (!base.isActiveAndEnabled)
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		if (this._grabber != null && this._grabber != grabber)
		{
			this.ForceRelease();
		}
		bool flag;
		bool flag2;
		if (this._precise && this.IsSlingshotHeldInHand(out flag, out flag2) && ((grabber.XrNode == XRNode.LeftHand && flag) || (grabber.XrNode == XRNode.RightHand && flag2)))
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		this._grabber = grabber;
		GtColliderTriggerProcessor.CurrentGrabbedHand = grabber.XrNode;
		GtColliderTriggerProcessor.IsGrabbingTablet = true;
		grabbedTransform = base.transform;
		localGrabbedPosition = this.GetLocalGrabbedPosition(this._grabber);
		this.target.SetParent(grabber.transform, true);
		Action action = this.onGrabbed;
		if (action != null)
		{
			action();
		}
		UnityEvent onTabletGrabbed = this.OnTabletGrabbed;
		if (onTabletGrabbed == null)
		{
			return;
		}
		onTabletGrabbed.Invoke();
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x0004754C File Offset: 0x0004574C
	public void OnGrabReleased(GorillaGrabber grabber)
	{
		this.target.transform.SetParent(this._originalTargetParent, true);
		this._grabber = null;
		GtColliderTriggerProcessor.IsGrabbingTablet = false;
		Action action = this.onReleased;
		if (action != null)
		{
			action();
		}
		UnityEvent onTabletReleased = this.OnTabletReleased;
		if (onTabletReleased == null)
		{
			return;
		}
		onTabletReleased.Invoke();
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x0004759E File Offset: 0x0004579E
	public void ForceGrab(GorillaGrabber grabber)
	{
		grabber.Inject(base.transform, this.GetLocalGrabbedPosition(grabber));
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x000475B3 File Offset: 0x000457B3
	public void ForceRelease()
	{
		if (this._grabber == null)
		{
			return;
		}
		this._grabber.Inject(null, Vector3.zero);
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x000475D8 File Offset: 0x000457D8
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig rig = VRRigCache.Instance.localRig.Rig;
		if (rig == null || rig.projectileWeapon == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = rig.projectileWeapon.InLeftHand();
		rightHand = rig.projectileWeapon.InRightHand();
		return rig.projectileWeapon.InHand();
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00047639 File Offset: 0x00045839
	public void SetOriginalTargetParent(Transform parent)
	{
		this._originalTargetParent = parent;
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x00047642 File Offset: 0x00045842
	public bool MomentaryGrabOnly()
	{
		return true;
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x0001396B File Offset: 0x00011B6B
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x0400114E RID: 4430
	public UnityEvent OnTabletGrabbed = new UnityEvent();

	// Token: 0x0400114F RID: 4431
	public UnityEvent OnTabletReleased = new UnityEvent();

	// Token: 0x04001150 RID: 4432
	[SerializeField]
	private Transform _originalTargetParent;

	// Token: 0x04001151 RID: 4433
	public Transform target;

	// Token: 0x04001152 RID: 4434
	[SerializeField]
	private bool _precise;

	// Token: 0x04001153 RID: 4435
	private GorillaGrabber _grabber;
}
