using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200039F RID: 927
public class InteractionPoint : MonoBehaviour, ISpawnable, IBuildValidation
{
	// Token: 0x17000262 RID: 610
	// (get) Token: 0x060015A1 RID: 5537 RVA: 0x00069A36 File Offset: 0x00067C36
	// (set) Token: 0x060015A2 RID: 5538 RVA: 0x00069A3E File Offset: 0x00067C3E
	public bool ignoreLeftHand { get; private set; }

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x060015A3 RID: 5539 RVA: 0x00069A47 File Offset: 0x00067C47
	// (set) Token: 0x060015A4 RID: 5540 RVA: 0x00069A4F File Offset: 0x00067C4F
	public bool ignoreRightHand { get; private set; }

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x060015A5 RID: 5541 RVA: 0x00069A58 File Offset: 0x00067C58
	public IHoldableObject Holdable
	{
		get
		{
			return this.parentHoldable;
		}
	}

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x060015A6 RID: 5542 RVA: 0x00069A60 File Offset: 0x00067C60
	// (set) Token: 0x060015A7 RID: 5543 RVA: 0x00069A68 File Offset: 0x00067C68
	public bool IsSpawned { get; set; }

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x060015A8 RID: 5544 RVA: 0x00069A71 File Offset: 0x00067C71
	// (set) Token: 0x060015A9 RID: 5545 RVA: 0x00069A79 File Offset: 0x00067C79
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x060015AA RID: 5546 RVA: 0x00069A84 File Offset: 0x00067C84
	public void OnSpawn(VRRig rig)
	{
		this.interactor = EquipmentInteractor.instance;
		this.myCollider = base.GetComponent<Collider>();
		if (this.parentHoldableObject != null)
		{
			this.parentHoldable = this.parentHoldableObject.GetComponent<IHoldableObject>();
		}
		else
		{
			this.parentHoldable = base.GetComponentInParent<IHoldableObject>();
			this.parentHoldableObject = this.parentHoldable.gameObject;
		}
		if (this.parentHoldable == null)
		{
			if (this.parentHoldableObject == null)
			{
				Debug.LogError("InteractionPoint: Disabling because expected field `parentHoldableObject` is null. Path=" + base.transform.GetPathQ());
				base.enabled = false;
				return;
			}
			Debug.LogError("InteractionPoint: Disabling because `parentHoldableObject` does not have a IHoldableObject component. Path=" + base.transform.GetPathQ());
		}
		TransferrableObject transferrableObject = this.parentHoldable as TransferrableObject;
		this.forLocalPlayer = transferrableObject == null || transferrableObject.IsLocalObject() || transferrableObject.isSceneObject || transferrableObject.canDrop;
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x00069B62 File Offset: 0x00067D62
	private void Awake()
	{
		if (this.isNonSpawnedObject)
		{
			this.OnSpawn(null);
		}
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x00069B73 File Offset: 0x00067D73
	private void OnEnable()
	{
		this.wasInLeft = false;
		this.wasInRight = false;
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x00069B83 File Offset: 0x00067D83
	public void OnDisable()
	{
		if (!this.forLocalPlayer || this.interactor == null)
		{
			return;
		}
		this.interactor.InteractionPointDisabled(this);
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x00069BA8 File Offset: 0x00067DA8
	protected void LateUpdate()
	{
		if (!this.forLocalPlayer)
		{
			base.enabled = false;
			this.myCollider.enabled = false;
			return;
		}
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
			return;
		}
		if (this.interactionRadius > 0f || this.myCollider != null)
		{
			if (!this.ignoreLeftHand && this.OverlapCheck(this.interactor.leftHand.transform.position) != this.wasInLeft)
			{
				if (!this.wasInLeft && !this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Add(this);
					this.wasInLeft = true;
				}
				else if (this.wasInLeft && this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Remove(this);
					this.wasInLeft = false;
				}
			}
			if (!this.ignoreRightHand && this.OverlapCheck(this.interactor.rightHand.transform.position) != this.wasInRight)
			{
				if (!this.wasInRight && !this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Add(this);
					this.wasInRight = true;
					return;
				}
				if (this.wasInRight && this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Remove(this);
					this.wasInRight = false;
				}
			}
		}
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x00069D38 File Offset: 0x00067F38
	private bool OverlapCheck(Vector3 point)
	{
		if (this.interactionRadius > 0f)
		{
			return (base.transform.position - point).IsShorterThan(this.interactionRadius * base.transform.lossyScale);
		}
		return this.myCollider != null && this.myCollider.bounds.Contains(point);
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x00047642 File Offset: 0x00045842
	public bool BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x0400181C RID: 6172
	[SerializeField]
	[FormerlySerializedAs("parentTransferrableObject")]
	private GameObject parentHoldableObject;

	// Token: 0x0400181D RID: 6173
	private IHoldableObject parentHoldable;

	// Token: 0x04001820 RID: 6176
	[SerializeField]
	private bool isNonSpawnedObject;

	// Token: 0x04001821 RID: 6177
	[SerializeField]
	private float interactionRadius;

	// Token: 0x04001822 RID: 6178
	public Collider myCollider;

	// Token: 0x04001823 RID: 6179
	public EquipmentInteractor interactor;

	// Token: 0x04001824 RID: 6180
	public bool wasInLeft;

	// Token: 0x04001825 RID: 6181
	public bool wasInRight;

	// Token: 0x04001826 RID: 6182
	public bool forLocalPlayer;
}
