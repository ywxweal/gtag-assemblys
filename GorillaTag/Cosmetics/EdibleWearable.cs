using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DCF RID: 3535
	public class EdibleWearable : MonoBehaviour
	{
		// Token: 0x060057AD RID: 22445 RVA: 0x001AEC90 File Offset: 0x001ACE90
		protected void Awake()
		{
			this.edibleState = 0;
			this.previousEdibleState = 0;
			this.ownerRig = base.GetComponentInParent<VRRig>();
			this.isLocal = this.ownerRig != null && this.ownerRig.isOfflineVRRig;
			this.isHandSlot = this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand || this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.RightHand;
			this.isLeftHand = this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand;
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
		}

		// Token: 0x060057AE RID: 22446 RVA: 0x001AED1C File Offset: 0x001ACF1C
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("EdibleWearable \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			for (int i = 0; i < this.edibleStateInfos.Length; i++)
			{
				this.edibleStateInfos[i].gameObject.SetActive(i == this.edibleState);
			}
		}

		// Token: 0x060057AF RID: 22447 RVA: 0x001AED96 File Offset: 0x001ACF96
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

		// Token: 0x060057B0 RID: 22448 RVA: 0x001AEDB4 File Offset: 0x001ACFB4
		protected virtual void LateUpdateLocal()
		{
			if (this.edibleState == this.edibleStateInfos.Length - 1)
			{
				if (!this.isNonRespawnable && Time.time > this.lastFullyEatenTime + this.respawnTime)
				{
					this.edibleState = 0;
					this.previousEdibleState = 0;
					this.OnEdibleHoldableStateChange();
				}
				if (this.isNonRespawnable && Time.time > this.lastFullyEatenTime)
				{
					this.edibleState = 0;
					this.previousEdibleState = 0;
					this.OnEdibleHoldableStateChange();
					GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { false, this.isLeftHand });
				}
			}
			else if (Time.time > this.lastEatTime + this.biteCooldown)
			{
				Vector3 vector = base.transform.TransformPoint(this.edibleBiteOffset);
				bool flag = false;
				float num = this.biteDistance * this.biteDistance;
				if (!GorillaParent.hasInstance)
				{
					return;
				}
				if ((GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - vector).sqrMagnitude < num)
				{
					flag = true;
				}
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!flag)
					{
						if (vrrig.head == null)
						{
							break;
						}
						if (vrrig.head.rigTarget == null)
						{
							break;
						}
						if ((vrrig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - vector).sqrMagnitude < num)
						{
							flag = true;
						}
					}
				}
				if (flag && !this.wasInBiteZoneLastFrame && this.edibleState < this.edibleStateInfos.Length)
				{
					this.edibleState++;
					this.lastEatTime = Time.time;
					this.lastFullyEatenTime = Time.time;
				}
				this.wasInBiteZoneLastFrame = flag;
			}
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, this.edibleState);
		}

		// Token: 0x060057B1 RID: 22449 RVA: 0x001AEFF8 File Offset: 0x001AD1F8
		protected virtual void LateUpdateReplicated()
		{
			this.edibleState = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
		}

		// Token: 0x060057B2 RID: 22450 RVA: 0x001AF028 File Offset: 0x001AD228
		protected virtual void LateUpdateShared()
		{
			int num = this.edibleState;
			if (num != this.previousEdibleState)
			{
				this.OnEdibleHoldableStateChange();
			}
			this.previousEdibleState = num;
		}

		// Token: 0x060057B3 RID: 22451 RVA: 0x001AF054 File Offset: 0x001AD254
		protected virtual void OnEdibleHoldableStateChange()
		{
			if (this.previousEdibleState >= 0 && this.previousEdibleState < this.edibleStateInfos.Length)
			{
				this.edibleStateInfos[this.previousEdibleState].gameObject.SetActive(false);
			}
			if (this.edibleState >= 0 && this.edibleState < this.edibleStateInfos.Length)
			{
				this.edibleStateInfos[this.edibleState].gameObject.SetActive(true);
			}
			if (this.edibleState > 0 && this.edibleState < this.edibleStateInfos.Length && this.audioSource != null)
			{
				this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState].sound, this.volume);
			}
			if (this.edibleState == this.edibleStateInfos.Length && this.audioSource != null)
			{
				this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState - 1].sound, this.volume);
			}
			float num = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (this.isLocal && this.isHandSlot)
			{
				GorillaTagger.Instance.StartVibration(this.isLeftHand, num, fixedDeltaTime);
			}
		}

		// Token: 0x04005C53 RID: 23635
		[Tooltip("Check when using non cosmetic edible items like honeycomb")]
		public bool isNonRespawnable;

		// Token: 0x04005C54 RID: 23636
		[Tooltip("Eating sounds are played through this AudioSource using PlayOneShot.")]
		public AudioSource audioSource;

		// Token: 0x04005C55 RID: 23637
		[Tooltip("Volume each bite should play at.")]
		public float volume = 0.08f;

		// Token: 0x04005C56 RID: 23638
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.LeftHand;

		// Token: 0x04005C57 RID: 23639
		[Tooltip("Time between bites.")]
		public float biteCooldown = 1f;

		// Token: 0x04005C58 RID: 23640
		[Tooltip("How long it takes to pop back to the uneaten state after being fully eaten.")]
		public float respawnTime = 7f;

		// Token: 0x04005C59 RID: 23641
		[Tooltip("Distance from mouth to item required to trigger a bite.")]
		public float biteDistance = 0.5f;

		// Token: 0x04005C5A RID: 23642
		[Tooltip("Offset from Gorilla's head to mouth.")]
		public Vector3 gorillaHeadMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04005C5B RID: 23643
		[Tooltip("Offset from edible's transform to the bite point.")]
		public Vector3 edibleBiteOffset = new Vector3(0f, 0f, 0f);

		// Token: 0x04005C5C RID: 23644
		public EdibleWearable.EdibleStateInfo[] edibleStateInfos;

		// Token: 0x04005C5D RID: 23645
		private VRRig ownerRig;

		// Token: 0x04005C5E RID: 23646
		private bool isLocal;

		// Token: 0x04005C5F RID: 23647
		private bool isHandSlot;

		// Token: 0x04005C60 RID: 23648
		private bool isLeftHand;

		// Token: 0x04005C61 RID: 23649
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x04005C62 RID: 23650
		private int edibleState;

		// Token: 0x04005C63 RID: 23651
		private int previousEdibleState;

		// Token: 0x04005C64 RID: 23652
		private float lastEatTime;

		// Token: 0x04005C65 RID: 23653
		private float lastFullyEatenTime;

		// Token: 0x04005C66 RID: 23654
		private bool wasInBiteZoneLastFrame;

		// Token: 0x02000DD0 RID: 3536
		[Serializable]
		public struct EdibleStateInfo
		{
			// Token: 0x04005C67 RID: 23655
			[Tooltip("Will be activated when this stage is reached.")]
			public GameObject gameObject;

			// Token: 0x04005C68 RID: 23656
			[Tooltip("Will be played when this stage is reached.")]
			public AudioClip sound;
		}
	}
}
