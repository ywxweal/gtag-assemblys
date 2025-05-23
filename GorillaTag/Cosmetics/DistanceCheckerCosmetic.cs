using System;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DD8 RID: 3544
	public class DistanceCheckerCosmetic : MonoBehaviour, ISpawnable
	{
		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x060057DC RID: 22492 RVA: 0x001AFFF4 File Offset: 0x001AE1F4
		// (set) Token: 0x060057DD RID: 22493 RVA: 0x001AFFFC File Offset: 0x001AE1FC
		public bool IsSpawned { get; set; }

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x060057DE RID: 22494 RVA: 0x001B0005 File Offset: 0x001AE205
		// (set) Token: 0x060057DF RID: 22495 RVA: 0x001B000D File Offset: 0x001AE20D
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x060057E0 RID: 22496 RVA: 0x001B0016 File Offset: 0x001AE216
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x060057E1 RID: 22497 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnDespawn()
		{
		}

		// Token: 0x060057E2 RID: 22498 RVA: 0x001B001F File Offset: 0x001AE21F
		private void OnEnable()
		{
			this.currentState = DistanceCheckerCosmetic.State.None;
			this.transferableObject = base.GetComponentInParent<TransferrableObject>();
			if (this.transferableObject != null)
			{
				this.ownerRig = this.transferableObject.ownerRig;
			}
			this.ResetClosestPlayer();
		}

		// Token: 0x060057E3 RID: 22499 RVA: 0x001B0059 File Offset: 0x001AE259
		private void Update()
		{
			this.UpdateDistance();
		}

		// Token: 0x060057E4 RID: 22500 RVA: 0x001B0061 File Offset: 0x001AE261
		private bool IsBelowThreshold(Vector3 distance)
		{
			return distance.IsShorterThan(this.distanceThreshold);
		}

		// Token: 0x060057E5 RID: 22501 RVA: 0x001B0074 File Offset: 0x001AE274
		private bool IsAboveThreshold(Vector3 distance)
		{
			return distance.IsLongerThan(this.distanceThreshold);
		}

		// Token: 0x060057E6 RID: 22502 RVA: 0x001B0088 File Offset: 0x001AE288
		private void UpdateClosestPlayer(bool others = false)
		{
			if (!PhotonNetwork.InRoom)
			{
				this.ResetClosestPlayer();
				return;
			}
			VRRig vrrig = this.currentClosestPlayer;
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
			foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
			{
				if (!others || !(this.ownerRig != null) || !(vrrig2 == this.ownerRig))
				{
					Vector3 vector = vrrig2.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(vector) && vector.sqrMagnitude < this.closestDistance.sqrMagnitude)
					{
						this.closestDistance = vector;
						this.currentClosestPlayer = vrrig2;
					}
				}
			}
			if (this.currentClosestPlayer != null && this.currentClosestPlayer != vrrig)
			{
				UnityEvent<VRRig, float> unityEvent = this.onClosestPlayerBelowThresholdChanged;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.currentClosestPlayer, this.closestDistance.magnitude);
			}
		}

		// Token: 0x060057E7 RID: 22503 RVA: 0x001B01A4 File Offset: 0x001AE3A4
		private void ResetClosestPlayer()
		{
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
		}

		// Token: 0x060057E8 RID: 22504 RVA: 0x001B01B8 File Offset: 0x001AE3B8
		private void UpdateDistance()
		{
			bool flag = true;
			switch (this.distanceTo)
			{
			case DistanceCheckerCosmetic.DistanceCondition.Owner:
			{
				Vector3 vector = this.myRig.transform.position - this.distanceFrom.position;
				if (this.IsBelowThreshold(vector))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
					return;
				}
				if (this.IsAboveThreshold(vector))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
				}
				break;
			}
			case DistanceCheckerCosmetic.DistanceCondition.Others:
				this.UpdateClosestPlayer(true);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!(this.ownerRig != null) || !(vrrig == this.ownerRig))
					{
						Vector3 vector2 = vrrig.transform.position - this.distanceFrom.position;
						if (this.IsBelowThreshold(vector2))
						{
							this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
							flag = false;
						}
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			case DistanceCheckerCosmetic.DistanceCondition.Everyone:
				this.UpdateClosestPlayer(false);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
				{
					Vector3 vector3 = vrrig2.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(vector3))
					{
						this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
						flag = false;
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060057E9 RID: 22505 RVA: 0x001B0360 File Offset: 0x001AE560
		private void UpdateState(DistanceCheckerCosmetic.State newState)
		{
			if (this.currentState == newState)
			{
				return;
			}
			this.currentState = newState;
			if (this.currentState != DistanceCheckerCosmetic.State.AboveThreshold)
			{
				if (this.currentState == DistanceCheckerCosmetic.State.BelowThreshold)
				{
					UnityEvent unityEvent = this.onOneIsBelowThreshold;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
				return;
			}
			UnityEvent unityEvent2 = this.onAllAreAboveThreshold;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke();
		}

		// Token: 0x04005CB5 RID: 23733
		[SerializeField]
		private Transform distanceFrom;

		// Token: 0x04005CB6 RID: 23734
		[SerializeField]
		private DistanceCheckerCosmetic.DistanceCondition distanceTo;

		// Token: 0x04005CB7 RID: 23735
		[Tooltip("Receive events when above or below this distance")]
		public float distanceThreshold;

		// Token: 0x04005CB8 RID: 23736
		public UnityEvent onOneIsBelowThreshold;

		// Token: 0x04005CB9 RID: 23737
		public UnityEvent onAllAreAboveThreshold;

		// Token: 0x04005CBA RID: 23738
		public UnityEvent<VRRig, float> onClosestPlayerBelowThresholdChanged;

		// Token: 0x04005CBB RID: 23739
		private VRRig myRig;

		// Token: 0x04005CBC RID: 23740
		private DistanceCheckerCosmetic.State currentState;

		// Token: 0x04005CBD RID: 23741
		private Vector3 closestDistance;

		// Token: 0x04005CBE RID: 23742
		private VRRig currentClosestPlayer;

		// Token: 0x04005CBF RID: 23743
		private VRRig ownerRig;

		// Token: 0x04005CC0 RID: 23744
		private TransferrableObject transferableObject;

		// Token: 0x02000DD9 RID: 3545
		private enum State
		{
			// Token: 0x04005CC4 RID: 23748
			AboveThreshold,
			// Token: 0x04005CC5 RID: 23749
			BelowThreshold,
			// Token: 0x04005CC6 RID: 23750
			None
		}

		// Token: 0x02000DDA RID: 3546
		private enum DistanceCondition
		{
			// Token: 0x04005CC8 RID: 23752
			None,
			// Token: 0x04005CC9 RID: 23753
			Owner,
			// Token: 0x04005CCA RID: 23754
			Others,
			// Token: 0x04005CCB RID: 23755
			Everyone
		}
	}
}
