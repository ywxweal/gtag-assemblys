﻿using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B6D RID: 2925
	public class BuilderSmallMonkeTrigger : MonoBehaviour
	{
		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x0600486F RID: 18543 RVA: 0x00159B76 File Offset: 0x00157D76
		public int overlapCount
		{
			get
			{
				return this.overlappingColliders.Count;
			}
		}

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x06004870 RID: 18544 RVA: 0x00159B83 File Offset: 0x00157D83
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x1400007F RID: 127
		// (add) Token: 0x06004871 RID: 18545 RVA: 0x00159B94 File Offset: 0x00157D94
		// (remove) Token: 0x06004872 RID: 18546 RVA: 0x00159BCC File Offset: 0x00157DCC
		public event Action onTriggerFirstEntered;

		// Token: 0x14000080 RID: 128
		// (add) Token: 0x06004873 RID: 18547 RVA: 0x00159C04 File Offset: 0x00157E04
		// (remove) Token: 0x06004874 RID: 18548 RVA: 0x00159C3C File Offset: 0x00157E3C
		public event Action onTriggerLastExited;

		// Token: 0x06004875 RID: 18549 RVA: 0x00159C74 File Offset: 0x00157E74
		public void ValidateOverlappingColliders()
		{
			for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
			{
				if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
				{
					this.overlappingColliders.RemoveAt(i);
				}
				else
				{
					VRRig vrrig = this.overlappingColliders[i].attachedRigidbody.gameObject.GetComponent<VRRig>();
					if (vrrig == null)
					{
						if (GTPlayer.Instance.bodyCollider == this.overlappingColliders[i] || GTPlayer.Instance.headCollider == this.overlappingColliders[i])
						{
							vrrig = GorillaTagger.Instance.offlineVRRig;
						}
						else
						{
							this.overlappingColliders.RemoveAt(i);
						}
					}
					if (!this.ignoreScale && vrrig != null && (double)vrrig.scaleFactor > 0.99)
					{
						this.overlappingColliders.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x06004876 RID: 18550 RVA: 0x00159D98 File Offset: 0x00157F98
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other) && !(GTPlayer.Instance.headCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(vrrig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (!this.ignoreScale && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			bool flag = this.overlappingColliders.Count == 0;
			if (!this.overlappingColliders.Contains(other))
			{
				this.overlappingColliders.Add(other);
			}
			this.lastTriggeredFrame = Time.frameCount;
			if (flag)
			{
				Action action = this.onTriggerFirstEntered;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x06004877 RID: 18551 RVA: 0x00159E92 File Offset: 0x00158092
		private void OnTriggerExit(Collider other)
		{
			if (this.overlappingColliders.Remove(other) && this.overlappingColliders.Count == 0)
			{
				Action action = this.onTriggerLastExited;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x04004B0F RID: 19215
		private int lastTriggeredFrame = -1;

		// Token: 0x04004B10 RID: 19216
		private List<Collider> overlappingColliders = new List<Collider>(20);

		// Token: 0x04004B13 RID: 19219
		private bool hasCheckedZone;

		// Token: 0x04004B14 RID: 19220
		private bool ignoreScale;
	}
}
