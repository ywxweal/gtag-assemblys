using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DC7 RID: 3527
	public class PickupableBanner : PickupableVariant
	{
		// Token: 0x0600576B RID: 22379 RVA: 0x001AD5C8 File Offset: 0x001AB7C8
		protected internal override void Pickup()
		{
			UnityEvent onPickup = this.OnPickup;
			if (onPickup != null)
			{
				onPickup.Invoke();
			}
			this.rb.isKinematic = true;
			this.rb.velocity = Vector3.zero;
			if (this.holdableParent != null)
			{
				base.transform.parent = this.holdableParent.transform;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.scale = 1f;
			this.placedOnFloorTime = -1f;
			this.placedOnFloor = false;
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = true;
			}
			base.enabled = false;
		}

		// Token: 0x0600576C RID: 22380 RVA: 0x001AD694 File Offset: 0x001AB894
		protected internal override void DelayedPickup()
		{
			base.StartCoroutine(this.DelayedPickup_Internal());
		}

		// Token: 0x0600576D RID: 22381 RVA: 0x001AD6A3 File Offset: 0x001AB8A3
		private IEnumerator DelayedPickup_Internal()
		{
			yield return new WaitForSeconds(1f);
			this.Pickup();
			yield break;
		}

		// Token: 0x0600576E RID: 22382 RVA: 0x001AD6B4 File Offset: 0x001AB8B4
		protected internal override void Release(HoldableObject holdable, Vector3 startPosition, Vector3 velocity, float playerScale)
		{
			this.holdableParent = holdable;
			base.transform.parent = null;
			base.transform.position = startPosition;
			base.transform.localScale = Vector3.one * playerScale;
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
			this.rb.velocity = velocity;
			if (!this.allowPickupFromGround && this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.scale = playerScale;
			base.enabled = true;
		}

		// Token: 0x0600576F RID: 22383 RVA: 0x001AD74C File Offset: 0x001AB94C
		private void FixedUpdate()
		{
			if (this.placedOnFloor && Time.time - this.placedOnFloorTime > this.autoPickupAfterSeconds)
			{
				this.Pickup();
				this.placedOnFloorTime = -1f;
			}
			if (!this.placedOnFloor)
			{
				float num = this.RaycastCheckDist * this.scale;
				int value = this.floorLayerMask.value;
				for (int i = 0; i < this.RaycastChecksMax; i++)
				{
					Vector3 onUnitSphere = Random.onUnitSphere;
					RaycastHit raycastHit;
					if (Physics.Raycast(this.raycastOrigin.position, onUnitSphere, out raycastHit, num, value, QueryTriggerInteraction.Ignore))
					{
						UnityEvent onPlaced = this.OnPlaced;
						if (onPlaced != null)
						{
							onPlaced.Invoke();
						}
						this.placedOnFloor = true;
						this.placedOnFloorTime = Time.time;
						this.rb.isKinematic = true;
						this.rb.useGravity = false;
						this.rb.velocity = Vector3.zero;
						Vector3 normal = raycastHit.normal;
						base.transform.position = raycastHit.point + normal * this.placementOffset;
						Quaternion quaternion = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
						base.transform.rotation = quaternion;
						return;
					}
				}
			}
		}

		// Token: 0x04005C04 RID: 23556
		[Tooltip("The distance to check if the banner is close to the floor (from a raycast check).")]
		public float RaycastCheckDist = 0.2f;

		// Token: 0x04005C05 RID: 23557
		[Tooltip("How many checks should we attempt for a raycast.")]
		public int RaycastChecksMax = 12;

		// Token: 0x04005C06 RID: 23558
		[SerializeField]
		private InteractionPoint interactionPoint;

		// Token: 0x04005C07 RID: 23559
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x04005C08 RID: 23560
		[SerializeField]
		private bool allowPickupFromGround = true;

		// Token: 0x04005C09 RID: 23561
		[SerializeField]
		private LayerMask floorLayerMask;

		// Token: 0x04005C0A RID: 23562
		[SerializeField]
		private float placementOffset;

		// Token: 0x04005C0B RID: 23563
		[SerializeField]
		private Transform raycastOrigin;

		// Token: 0x04005C0C RID: 23564
		[SerializeField]
		private float autoPickupAfterSeconds;

		// Token: 0x04005C0D RID: 23565
		public UnityEvent OnPickup;

		// Token: 0x04005C0E RID: 23566
		public UnityEvent OnPlaced;

		// Token: 0x04005C0F RID: 23567
		private bool placedOnFloor;

		// Token: 0x04005C10 RID: 23568
		private float placedOnFloorTime = -1f;

		// Token: 0x04005C11 RID: 23569
		private VRRig cachedLocalRig;

		// Token: 0x04005C12 RID: 23570
		private HoldableObject holdableParent;

		// Token: 0x04005C13 RID: 23571
		private double throwSettledTime = -1.0;

		// Token: 0x04005C14 RID: 23572
		private int landingSide;

		// Token: 0x04005C15 RID: 23573
		private float scale;
	}
}
