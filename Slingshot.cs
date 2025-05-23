using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020003A6 RID: 934
public class Slingshot : ProjectileWeapon
{
	// Token: 0x060015D1 RID: 5585 RVA: 0x0006A3E4 File Offset: 0x000685E4
	private void DestroyDummyProjectile()
	{
		if (this.hasDummyProjectile)
		{
			this.dummyProjectile.transform.localScale = Vector3.one * this.dummyProjectileInitialScale;
			this.dummyProjectile.GetComponent<SphereCollider>().enabled = true;
			ObjectPools.instance.Destroy(this.dummyProjectile);
			this.dummyProjectile = null;
			this.hasDummyProjectile = false;
		}
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x0006A448 File Offset: 0x00068648
	protected override void Awake()
	{
		base.Awake();
		this._elasticIntialWidthMultiplier = this.elasticLeft.widthMultiplier;
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x0006A461 File Offset: 0x00068661
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.myRig = rig;
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x0006A474 File Offset: 0x00068674
	internal override void OnEnable()
	{
		this.leftHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapLeft).transform;
		this.rightHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapRight).transform;
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
		this.elasticLeft.positionCount = 2;
		this.elasticRight.positionCount = 2;
		this.dummyProjectile = null;
		base.OnEnable();
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x0006A4ED File Offset: 0x000686ED
	internal override void OnDisable()
	{
		this.DestroyDummyProjectile();
		base.OnDisable();
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x0006A4FC File Offset: 0x000686FC
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		float num = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 vector;
		if (this.InDrawingState())
		{
			if (!this.hasDummyProjectile)
			{
				this.dummyProjectile = ObjectPools.instance.Instantiate(this.projectilePrefab, true);
				this.hasDummyProjectile = true;
				SphereCollider component = this.dummyProjectile.GetComponent<SphereCollider>();
				component.enabled = false;
				this.dummyProjectileColliderRadius = component.radius;
				this.dummyProjectileInitialScale = this.dummyProjectile.transform.localScale.x;
				bool flag;
				bool flag2;
				base.GetIsOnTeams(out flag, out flag2);
				this.dummyProjectile.GetComponent<SlingshotProjectile>().ApplyTeamModelAndColor(flag, flag2, false, default(Color));
			}
			float num2 = this.dummyProjectileInitialScale * num;
			this.dummyProjectile.transform.localScale = Vector3.one * num2;
			Vector3 position = this.drawingHand.transform.position;
			Vector3 position2 = this.centerOrigin.position;
			Vector3 normalized = (position2 - position).normalized;
			float num3 = (EquipmentInteractor.instance.grabRadius - this.dummyProjectileColliderRadius) * num;
			vector = position + normalized * num3;
			this.dummyProjectile.transform.position = vector;
			this.dummyProjectile.transform.rotation = Quaternion.LookRotation(position2 - vector, Vector3.up);
		}
		else
		{
			this.DestroyDummyProjectile();
			vector = this.centerOrigin.position;
		}
		this.center.position = vector;
		this.elasticLeftPoints[0] = this.leftArm.position;
		this.elasticLeftPoints[1] = (this.elasticRightPoints[1] = vector);
		this.elasticRightPoints[0] = this.rightArm.position;
		this.elasticLeft.SetPositions(this.elasticLeftPoints);
		this.elasticRight.SetPositions(this.elasticRightPoints);
		this.elasticLeft.widthMultiplier = this._elasticIntialWidthMultiplier * num;
		this.elasticRight.widthMultiplier = this._elasticIntialWidthMultiplier * num;
		if (!NetworkSystem.Instance.InRoom && this.disableWhenNotInRoom)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x0006A742 File Offset: 0x00068942
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = EquipmentInteractor.instance.rightHand;
				return;
			}
			this.drawingHand = EquipmentInteractor.instance.leftHand;
		}
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x0006A77F File Offset: 0x0006897F
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = this.rightHandSnap.gameObject;
				return;
			}
			this.drawingHand = this.leftHandSnap.gameObject;
		}
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x0006A7BA File Offset: 0x000689BA
	public static bool IsSlingShotEnabled()
	{
		return !(GorillaTagger.Instance == null) && !(GorillaTagger.Instance.offlineVRRig == null) && GorillaTagger.Instance.offlineVRRig.cosmeticSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x0006A7F4 File Offset: 0x000689F4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		bool flag = pointGrabbed == this.nock;
		if (flag && !base.InHand())
		{
			return;
		}
		base.OnGrab(pointGrabbed, grabbingHand);
		if (this.InDrawingState() || base.OnChest())
		{
			return;
		}
		if (flag)
		{
			if (grabbingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = true;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = true;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.itemState = TransferrableObject.ItemStates.State2;
			}
			else
			{
				this.itemState = TransferrableObject.ItemStates.State3;
			}
			this.minTimeToLaunch = Time.time + this.delayLaunchTime;
			GorillaTagger.Instance.StartVibration(!this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
		}
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x0006A8D0 File Offset: 0x00068AD0
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (this.InDrawingState() && releasingHand == this.drawingHand)
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = false;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = false;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.currentState = TransferrableObject.PositionState.InLeftHand;
			}
			else
			{
				this.currentState = TransferrableObject.PositionState.InRightHand;
			}
			this.itemState = TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			if (Time.time > this.minTimeToLaunch)
			{
				base.LaunchProjectile();
			}
		}
		else
		{
			EquipmentInteractor.instance.disableLeftGrab = false;
			EquipmentInteractor.instance.disableRightGrab = false;
		}
		return true;
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x0006A9B1 File Offset: 0x00068BB1
	public override void DropItemCleanup()
	{
		base.DropItemCleanup();
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x00047642 File Offset: 0x00045842
	public override bool AutoGrabTrue(bool leftGrabbingHand)
	{
		return true;
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x0006A9C8 File Offset: 0x00068BC8
	private bool ForLeftHandSlingshot()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x0006A9DE File Offset: 0x00068BDE
	private bool InDrawingState()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State3;
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x0006A9F4 File Offset: 0x00068BF4
	protected override Vector3 GetLaunchPosition()
	{
		return this.dummyProjectile.transform.position;
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x0006AA08 File Offset: 0x00068C08
	protected override Vector3 GetLaunchVelocity()
	{
		float num = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 vector = this.centerOrigin.position - this.center.position;
		vector /= num;
		Vector3 vector2 = Mathf.Min(this.springConstant * this.maxDraw, vector.magnitude * this.springConstant) * vector.normalized * num;
		Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
		return vector2 + averagedVelocity;
	}

	// Token: 0x0400184A RID: 6218
	[FormerlySerializedAs("elastic")]
	public LineRenderer elasticLeft;

	// Token: 0x0400184B RID: 6219
	public LineRenderer elasticRight;

	// Token: 0x0400184C RID: 6220
	public Transform leftArm;

	// Token: 0x0400184D RID: 6221
	public Transform rightArm;

	// Token: 0x0400184E RID: 6222
	public Transform center;

	// Token: 0x0400184F RID: 6223
	public Transform centerOrigin;

	// Token: 0x04001850 RID: 6224
	private GameObject dummyProjectile;

	// Token: 0x04001851 RID: 6225
	public GameObject drawingHand;

	// Token: 0x04001852 RID: 6226
	public InteractionPoint nock;

	// Token: 0x04001853 RID: 6227
	public InteractionPoint grip;

	// Token: 0x04001854 RID: 6228
	public float springConstant;

	// Token: 0x04001855 RID: 6229
	public float maxDraw;

	// Token: 0x04001856 RID: 6230
	private Transform leftHandSnap;

	// Token: 0x04001857 RID: 6231
	private Transform rightHandSnap;

	// Token: 0x04001858 RID: 6232
	public bool disableWhenNotInRoom;

	// Token: 0x04001859 RID: 6233
	private bool hasDummyProjectile;

	// Token: 0x0400185A RID: 6234
	private float delayLaunchTime = 0.07f;

	// Token: 0x0400185B RID: 6235
	private float minTimeToLaunch = -1f;

	// Token: 0x0400185C RID: 6236
	private float dummyProjectileColliderRadius;

	// Token: 0x0400185D RID: 6237
	private float dummyProjectileInitialScale;

	// Token: 0x0400185E RID: 6238
	private int projectileCount;

	// Token: 0x0400185F RID: 6239
	private Vector3[] elasticLeftPoints = new Vector3[2];

	// Token: 0x04001860 RID: 6240
	private Vector3[] elasticRightPoints = new Vector3[2];

	// Token: 0x04001861 RID: 6241
	private float _elasticIntialWidthMultiplier;

	// Token: 0x04001862 RID: 6242
	private new VRRig myRig;

	// Token: 0x020003A7 RID: 935
	public enum SlingshotState
	{
		// Token: 0x04001864 RID: 6244
		NoState = 1,
		// Token: 0x04001865 RID: 6245
		OnChest,
		// Token: 0x04001866 RID: 6246
		LeftHandDrawing = 4,
		// Token: 0x04001867 RID: 6247
		RightHandDrawing = 8
	}

	// Token: 0x020003A8 RID: 936
	public enum SlingshotActions
	{
		// Token: 0x04001869 RID: 6249
		Grab,
		// Token: 0x0400186A RID: 6250
		Release
	}
}
