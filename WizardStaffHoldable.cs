using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class WizardStaffHoldable : TransferrableObject
{
	// Token: 0x0600061A RID: 1562 RVA: 0x0002320C File Offset: 0x0002140C
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.tipTargetLocalPosition = this.tipTransform.localPosition;
		this.hasEffectsGameObject = this.effectsGameObject != null;
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x0002323F File Offset: 0x0002143F
	internal override void OnEnable()
	{
		base.OnEnable();
		this.InitToDefault();
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x0002324D File Offset: 0x0002144D
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x0002325B File Offset: 0x0002145B
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		if (this.hasEffectsGameObject && this.effectsHaveBeenPlayed)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x0002328C File Offset: 0x0002148C
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand() || this.itemState == TransferrableObject.ItemStates.State1 || !GorillaParent.hasInstance || !this.hitLastFrame)
		{
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minSlamVelocity)
		{
			return;
		}
		Vector3 up = this.tipTransform.up;
		Vector3 up2 = Vector3.up;
		if (Vector3.Angle(up, up2) > this.minSlamAngle)
		{
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x00023310 File Offset: 0x00021510
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
			if (this.hasEffectsGameObject)
			{
				this.effectsGameObject.SetActive(false);
			}
			this.effectsHaveBeenPlayed = false;
		}
		if (base.InHand())
		{
			Vector3 position = base.transform.position;
			Vector3 vector = base.transform.TransformPoint(this.tipTargetLocalPosition);
			RaycastHit raycastHit;
			if (Physics.Linecast(position, vector, out raycastHit, this.tipCollisionLayerMask))
			{
				this.tipTransform.position = raycastHit.point;
				this.hitLastFrame = true;
			}
			else
			{
				this.tipTransform.localPosition = this.tipTargetLocalPosition;
				this.hitLastFrame = false;
			}
			if (this.itemState == TransferrableObject.ItemStates.State1 && this.hasEffectsGameObject && !this.effectsHaveBeenPlayed)
			{
				this.effectsGameObject.SetActive(true);
				this.effectsHaveBeenPlayed = true;
			}
		}
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x00023400 File Offset: 0x00021600
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.effectsHaveBeenPlayed)
		{
			this.cooldownRemaining = this.cooldown;
		}
	}

	// Token: 0x04000730 RID: 1840
	[Tooltip("This GameObject will activate when the staff hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x04000731 RID: 1841
	[Tooltip("The Transform of the staff's tip which will be used to determine if the staff is being slammed. Up axis (Y) should point along the length of the staff.")]
	public Transform tipTransform;

	// Token: 0x04000732 RID: 1842
	public float tipCollisionRadius = 0.05f;

	// Token: 0x04000733 RID: 1843
	public LayerMask tipCollisionLayerMask;

	// Token: 0x04000734 RID: 1844
	[Tooltip("Used to calculate velocity of the staff.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000735 RID: 1845
	public float cooldown = 5f;

	// Token: 0x04000736 RID: 1846
	[Tooltip("The velocity of the staff's tip must be greater than this value to activate the effect.")]
	public float minSlamVelocity = 0.5f;

	// Token: 0x04000737 RID: 1847
	[Tooltip("The angle (in degrees) between the staff's tip and the ground must be less than this value to activate the effect.")]
	public float minSlamAngle = 5f;

	// Token: 0x04000738 RID: 1848
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x04000739 RID: 1849
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x0400073A RID: 1850
	private Vector3 tipTargetLocalPosition;

	// Token: 0x0400073B RID: 1851
	private bool hasEffectsGameObject;

	// Token: 0x0400073C RID: 1852
	private bool effectsHaveBeenPlayed;
}
