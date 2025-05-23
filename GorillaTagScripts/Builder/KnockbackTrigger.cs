using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B6F RID: 2927
	public class KnockbackTrigger : MonoBehaviour
	{
		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x06004881 RID: 18561 RVA: 0x0015A58F File Offset: 0x0015878F
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x06004882 RID: 18562 RVA: 0x0015A5A0 File Offset: 0x001587A0
		private void CheckZone()
		{
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
		}

		// Token: 0x06004883 RID: 18563 RVA: 0x0015A5F0 File Offset: 0x001587F0
		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.CheckZone();
			if (!this.ignoreScale && this.onlySmallMonke && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			this.collidersEntered.Add(other);
			if (this.collidersEntered.Count > 1)
			{
				return;
			}
			Vector3 vector = this.triggerVolume.ClosestPoint(GorillaTagger.Instance.headCollider.transform.position);
			Vector3 vector2 = vector - base.transform.TransformPoint(this.triggerVolume.center);
			vector2 -= Vector3.Project(vector2, base.transform.TransformDirection(this.localAxis));
			float magnitude = vector2.magnitude;
			Vector3 vector3 = Vector3.up;
			if (magnitude >= 0.01f)
			{
				vector3 = vector2 / magnitude;
			}
			GTPlayer.Instance.SetMaximumSlipThisFrame();
			GTPlayer.Instance.ApplyKnockback(vector3, this.knockbackVelocity * VRRigCache.Instance.localRig.Rig.scaleFactor, false);
			if (this.impactFX != null)
			{
				ObjectPools.instance.Instantiate(this.impactFX, vector, true);
			}
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			this.lastTriggeredFrame = Time.frameCount;
		}

		// Token: 0x06004884 RID: 18564 RVA: 0x0015A792 File Offset: 0x00158992
		private void OnTriggerExit(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.collidersEntered.Remove(other);
		}

		// Token: 0x06004885 RID: 18565 RVA: 0x0015A7CE File Offset: 0x001589CE
		private void OnDisable()
		{
			this.collidersEntered.Clear();
		}

		// Token: 0x04004B2C RID: 19244
		[SerializeField]
		private BoxCollider triggerVolume;

		// Token: 0x04004B2D RID: 19245
		[SerializeField]
		private float knockbackVelocity;

		// Token: 0x04004B2E RID: 19246
		[SerializeField]
		private Vector3 localAxis;

		// Token: 0x04004B2F RID: 19247
		[SerializeField]
		private GameObject impactFX;

		// Token: 0x04004B30 RID: 19248
		[SerializeField]
		private bool onlySmallMonke;

		// Token: 0x04004B31 RID: 19249
		private bool hasCheckedZone;

		// Token: 0x04004B32 RID: 19250
		private bool ignoreScale;

		// Token: 0x04004B33 RID: 19251
		private int lastTriggeredFrame = -1;

		// Token: 0x04004B34 RID: 19252
		private List<Collider> collidersEntered = new List<Collider>(4);
	}
}
