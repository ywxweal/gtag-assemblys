using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DD1 RID: 3537
	public class HeadlessHead : HoldableObject
	{
		// Token: 0x060057B6 RID: 22454 RVA: 0x001AF2F0 File Offset: 0x001AD4F0
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = this.ownerRig.isOfflineVRRig;
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
			this.baseLocalPosition = base.transform.localPosition;
			this.hasFirstPersonRenderer = this.firstPersonRenderer != null;
		}

		// Token: 0x060057B7 RID: 22455 RVA: 0x001AF374 File Offset: 0x001AD574
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("HeadlessHead \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.NoHead);
		}

		// Token: 0x060057B8 RID: 22456 RVA: 0x001AF3CD File Offset: 0x001AD5CD
		private void OnDisable()
		{
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.Default);
		}

		// Token: 0x060057B9 RID: 22457 RVA: 0x001AF3E0 File Offset: 0x001AD5E0
		protected virtual void LateUpdate()
		{
			if (this.isLocal)
			{
				this.LateUpdateLocal();
			}
			else
			{
				this.LateUpdateReplicated();
			}
			this.LateUpdateShared();
		}

		// Token: 0x060057BA RID: 22458 RVA: 0x001AF3FE File Offset: 0x001AD5FE
		protected virtual void LateUpdateLocal()
		{
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, (this.isHeld ? 1 : 0) + (this.isHeldLeftHand ? 2 : 0));
		}

		// Token: 0x060057BB RID: 22459 RVA: 0x001AF43C File Offset: 0x001AD63C
		protected virtual void LateUpdateReplicated()
		{
			int num = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
			this.isHeld = num != 0;
			this.isHeldLeftHand = (num & 2) != 0;
		}

		// Token: 0x060057BC RID: 22460 RVA: 0x001AF488 File Offset: 0x001AD688
		protected virtual void LateUpdateShared()
		{
			if (this.isHeld != this.wasHeld || this.isHeldLeftHand != this.wasHeldLeftHand)
			{
				this.blendingFromPosition = base.transform.position;
				this.blendingFromRotation = base.transform.rotation;
				this.blendFraction = 0f;
			}
			Quaternion quaternion;
			Vector3 vector;
			if (this.isHeldLeftHand)
			{
				quaternion = this.ownerRig.leftHandTransform.rotation * this.rotationFromLeftHand;
				vector = this.ownerRig.leftHandTransform.TransformPoint(this.offsetFromLeftHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else if (this.isHeld)
			{
				quaternion = this.ownerRig.rightHandTransform.rotation * this.rotationFromRightHand;
				vector = this.ownerRig.rightHandTransform.TransformPoint(this.offsetFromRightHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else
			{
				quaternion = base.transform.parent.rotation;
				vector = base.transform.parent.TransformPoint(this.baseLocalPosition);
			}
			if (this.blendFraction < 1f)
			{
				this.blendFraction += Time.deltaTime / this.blendDuration;
				quaternion = Quaternion.Lerp(this.blendingFromRotation, quaternion, this.blendFraction);
				vector = Vector3.Lerp(this.blendingFromPosition, vector, this.blendFraction);
			}
			base.transform.rotation = quaternion;
			base.transform.position = vector;
			if (this.hasFirstPersonRenderer)
			{
				float x = base.transform.lossyScale.x;
				this.firstPersonRenderer.enabled = (this.firstPersonHideCenter.transform.position - GTPlayer.Instance.headCollider.transform.position).IsLongerThan(this.firstPersonHiddenRadius * x);
			}
			this.wasHeld = this.isHeld;
			this.wasHeldLeftHand = this.isHeldLeftHand;
		}

		// Token: 0x060057BD RID: 22461 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
		{
		}

		// Token: 0x060057BE RID: 22462 RVA: 0x001AF68F File Offset: 0x001AD88F
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			this.isHeld = true;
			this.isHeldLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
			EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		}

		// Token: 0x060057BF RID: 22463 RVA: 0x001AF6C3 File Offset: 0x001AD8C3
		public override void DropItemCleanup()
		{
			this.isHeld = false;
			this.isHeldLeftHand = false;
		}

		// Token: 0x060057C0 RID: 22464 RVA: 0x001AF6D4 File Offset: 0x001AD8D4
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
			{
				return false;
			}
			if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
			{
				return false;
			}
			EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
			this.isHeld = false;
			this.isHeldLeftHand = false;
			return true;
		}

		// Token: 0x04005C6A RID: 23658
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.Face;

		// Token: 0x04005C6B RID: 23659
		[SerializeField]
		private Vector3 offsetFromLeftHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04005C6C RID: 23660
		[SerializeField]
		private Vector3 offsetFromRightHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04005C6D RID: 23661
		[SerializeField]
		private Quaternion rotationFromLeftHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x04005C6E RID: 23662
		[SerializeField]
		private Quaternion rotationFromRightHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x04005C6F RID: 23663
		private Vector3 baseLocalPosition;

		// Token: 0x04005C70 RID: 23664
		private VRRig ownerRig;

		// Token: 0x04005C71 RID: 23665
		private bool isLocal;

		// Token: 0x04005C72 RID: 23666
		private bool isHeld;

		// Token: 0x04005C73 RID: 23667
		private bool isHeldLeftHand;

		// Token: 0x04005C74 RID: 23668
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x04005C75 RID: 23669
		[SerializeField]
		private MeshRenderer firstPersonRenderer;

		// Token: 0x04005C76 RID: 23670
		[SerializeField]
		private float firstPersonHiddenRadius;

		// Token: 0x04005C77 RID: 23671
		[SerializeField]
		private Transform firstPersonHideCenter;

		// Token: 0x04005C78 RID: 23672
		[SerializeField]
		private Transform holdAnchorPoint;

		// Token: 0x04005C79 RID: 23673
		private bool hasFirstPersonRenderer;

		// Token: 0x04005C7A RID: 23674
		private Vector3 blendingFromPosition;

		// Token: 0x04005C7B RID: 23675
		private Quaternion blendingFromRotation;

		// Token: 0x04005C7C RID: 23676
		private float blendFraction;

		// Token: 0x04005C7D RID: 23677
		private bool wasHeld;

		// Token: 0x04005C7E RID: 23678
		private bool wasHeldLeftHand;

		// Token: 0x04005C7F RID: 23679
		[SerializeField]
		private float blendDuration = 0.3f;
	}
}
