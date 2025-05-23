using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B6C RID: 2924
	public class BuilderSmallHandTrigger : MonoBehaviour
	{
		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x0600486C RID: 18540 RVA: 0x00159998 File Offset: 0x00157B98
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x0600486D RID: 18541 RVA: 0x001599A8 File Offset: 0x00157BA8
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (this.onlySmallHands && !this.ignoreScale && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.requireMinimumVelocity)
			{
				float num = this.minimumVelocityMagnitude * GorillaTagger.Instance.offlineVRRig.scaleFactor;
				if ((componentInParent.isLeftHand ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.1f, false).sqrMagnitude < num * num)
				{
					return;
				}
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			this.lastTriggeredFrame = Time.frameCount;
			UnityEvent triggeredEvent = this.TriggeredEvent;
			if (triggeredEvent != null)
			{
				triggeredEvent.Invoke();
			}
			if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
			{
				this.timeline.Play();
			}
			if (this.animation != null && this.animation.clip != null)
			{
				this.animation.Play();
			}
		}

		// Token: 0x04004B05 RID: 19205
		[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
		public PlayableDirector timeline;

		// Token: 0x04004B06 RID: 19206
		[Tooltip("Optional animation to play")]
		public Animation animation;

		// Token: 0x04004B07 RID: 19207
		private int lastTriggeredFrame = -1;

		// Token: 0x04004B08 RID: 19208
		public bool onlySmallHands;

		// Token: 0x04004B09 RID: 19209
		[SerializeField]
		protected bool requireMinimumVelocity;

		// Token: 0x04004B0A RID: 19210
		[SerializeField]
		protected float minimumVelocityMagnitude = 0.1f;

		// Token: 0x04004B0B RID: 19211
		private bool hasCheckedZone;

		// Token: 0x04004B0C RID: 19212
		private bool ignoreScale;

		// Token: 0x04004B0D RID: 19213
		internal UnityEvent TriggeredEvent = new UnityEvent();

		// Token: 0x04004B0E RID: 19214
		[SerializeField]
		private BuilderPiece myPiece;
	}
}
