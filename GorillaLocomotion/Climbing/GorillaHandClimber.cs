using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000CEF RID: 3311
	public class GorillaHandClimber : MonoBehaviour
	{
		// Token: 0x06005226 RID: 21030 RVA: 0x0018FA98 File Offset: 0x0018DC98
		private void Awake()
		{
			this.col = base.GetComponent<Collider>();
		}

		// Token: 0x06005227 RID: 21031 RVA: 0x0018FAA8 File Offset: 0x0018DCA8
		private void Update()
		{
			for (int i = this.potentialClimbables.Count - 1; i >= 0; i--)
			{
				GorillaClimbable gorillaClimbable = this.potentialClimbables[i];
				if (gorillaClimbable == null || !gorillaClimbable.isActiveAndEnabled)
				{
					this.potentialClimbables.RemoveAt(i);
				}
				else if (gorillaClimbable.climbOnlyWhileSmall && !ZoneManagement.IsInZone(GTZone.monkeBlocksShared) && this.player.scale > 0.99f)
				{
					this.potentialClimbables.RemoveAt(i);
				}
			}
			bool grab = ControllerInputPoller.GetGrab(this.xrNode);
			bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
			if (!this.isClimbing)
			{
				if (this.queuedToBecomeValidToGrabAgain && Vector3.Distance(this.lastAutoReleasePos, this.handRoot.localPosition) >= 0.35f)
				{
					this.queuedToBecomeValidToGrabAgain = false;
				}
				if (grabRelease)
				{
					this.queuedToBecomeValidToGrabAgain = false;
					this.dontReclimbLast = null;
				}
				GorillaClimbable closestClimbable = this.GetClosestClimbable();
				if (!this.queuedToBecomeValidToGrabAgain && closestClimbable && grab && !this.equipmentInteractor.GetIsHolding(this.xrNode) && closestClimbable != this.dontReclimbLast && !this.player.inOverlay)
				{
					GorillaClimbableRef gorillaClimbableRef = closestClimbable as GorillaClimbableRef;
					if (gorillaClimbableRef != null)
					{
						this.player.BeginClimbing(gorillaClimbableRef.climb, this, gorillaClimbableRef);
						return;
					}
					this.player.BeginClimbing(closestClimbable, this, null);
					return;
				}
			}
			else if (grabRelease && this.canRelease)
			{
				this.player.EndClimbing(this, false, false);
			}
		}

		// Token: 0x06005228 RID: 21032 RVA: 0x0018FC21 File Offset: 0x0018DE21
		public void SetCanRelease(bool canRelease)
		{
			this.canRelease = canRelease;
		}

		// Token: 0x06005229 RID: 21033 RVA: 0x0018FC2C File Offset: 0x0018DE2C
		public GorillaClimbable GetClosestClimbable()
		{
			if (this.potentialClimbables.Count == 0)
			{
				return null;
			}
			if (this.potentialClimbables.Count == 1)
			{
				return this.potentialClimbables[0];
			}
			Vector3 position = base.transform.position;
			Bounds bounds = this.col.bounds;
			float num = 0.15f;
			GorillaClimbable gorillaClimbable = null;
			foreach (GorillaClimbable gorillaClimbable2 in this.potentialClimbables)
			{
				float num2;
				if (gorillaClimbable2.colliderCache)
				{
					if (!bounds.Intersects(gorillaClimbable2.colliderCache.bounds))
					{
						continue;
					}
					Vector3 vector = gorillaClimbable2.colliderCache.ClosestPoint(position);
					num2 = Vector3.Distance(position, vector);
				}
				else
				{
					num2 = Vector3.Distance(position, gorillaClimbable2.transform.position);
				}
				if (num2 < num)
				{
					gorillaClimbable = gorillaClimbable2;
					num = num2;
				}
			}
			return gorillaClimbable;
		}

		// Token: 0x0600522A RID: 21034 RVA: 0x0018FD2C File Offset: 0x0018DF2C
		private void OnTriggerEnter(Collider other)
		{
			GorillaClimbable gorillaClimbable;
			if (other.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
			{
				this.potentialClimbables.Add(gorillaClimbable);
				return;
			}
			GorillaClimbableRef gorillaClimbableRef;
			if (other.TryGetComponent<GorillaClimbableRef>(out gorillaClimbableRef))
			{
				this.potentialClimbables.Add(gorillaClimbableRef);
			}
		}

		// Token: 0x0600522B RID: 21035 RVA: 0x0018FD68 File Offset: 0x0018DF68
		private void OnTriggerExit(Collider other)
		{
			GorillaClimbable gorillaClimbable;
			if (other.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
			{
				this.potentialClimbables.Remove(gorillaClimbable);
				return;
			}
			GorillaClimbableRef gorillaClimbableRef;
			if (other.TryGetComponent<GorillaClimbableRef>(out gorillaClimbableRef))
			{
				this.potentialClimbables.Remove(gorillaClimbableRef);
			}
		}

		// Token: 0x0600522C RID: 21036 RVA: 0x0018FDA4 File Offset: 0x0018DFA4
		public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
		{
			this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
		}

		// Token: 0x0400565A RID: 22106
		[SerializeField]
		private GTPlayer player;

		// Token: 0x0400565B RID: 22107
		[SerializeField]
		private EquipmentInteractor equipmentInteractor;

		// Token: 0x0400565C RID: 22108
		private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();

		// Token: 0x0400565D RID: 22109
		[Header("Non-hand input should have the component disabled")]
		public XRNode xrNode = XRNode.LeftHand;

		// Token: 0x0400565E RID: 22110
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x0400565F RID: 22111
		[NonSerialized]
		public bool queuedToBecomeValidToGrabAgain;

		// Token: 0x04005660 RID: 22112
		[NonSerialized]
		public GorillaClimbable dontReclimbLast;

		// Token: 0x04005661 RID: 22113
		[NonSerialized]
		public Vector3 lastAutoReleasePos = Vector3.zero;

		// Token: 0x04005662 RID: 22114
		public Transform handRoot;

		// Token: 0x04005663 RID: 22115
		private const float DIST_FOR_CLEAR_RELEASE = 0.35f;

		// Token: 0x04005664 RID: 22116
		private const float DIST_FOR_GRAB = 0.15f;

		// Token: 0x04005665 RID: 22117
		private Collider col;

		// Token: 0x04005666 RID: 22118
		private bool canRelease = true;
	}
}
