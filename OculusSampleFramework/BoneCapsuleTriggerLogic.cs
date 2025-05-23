using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BCC RID: 3020
	public class BoneCapsuleTriggerLogic : MonoBehaviour
	{
		// Token: 0x06004A91 RID: 19089 RVA: 0x0016332F File Offset: 0x0016152F
		private void OnDisable()
		{
			this.CollidersTouchingUs.Clear();
		}

		// Token: 0x06004A92 RID: 19090 RVA: 0x0016333C File Offset: 0x0016153C
		private void Update()
		{
			this.CleanUpDeadColliders();
		}

		// Token: 0x06004A93 RID: 19091 RVA: 0x00163344 File Offset: 0x00161544
		private void OnTriggerEnter(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Add(component);
			}
		}

		// Token: 0x06004A94 RID: 19092 RVA: 0x00163384 File Offset: 0x00161584
		private void OnTriggerExit(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Remove(component);
			}
		}

		// Token: 0x06004A95 RID: 19093 RVA: 0x001633C4 File Offset: 0x001615C4
		private void CleanUpDeadColliders()
		{
			this._elementsToCleanUp.Clear();
			foreach (ColliderZone colliderZone in this.CollidersTouchingUs)
			{
				if (!colliderZone.Collider.gameObject.activeInHierarchy)
				{
					this._elementsToCleanUp.Add(colliderZone);
				}
			}
			foreach (ColliderZone colliderZone2 in this._elementsToCleanUp)
			{
				this.CollidersTouchingUs.Remove(colliderZone2);
			}
		}

		// Token: 0x04004D4E RID: 19790
		public InteractableToolTags ToolTags;

		// Token: 0x04004D4F RID: 19791
		public HashSet<ColliderZone> CollidersTouchingUs = new HashSet<ColliderZone>();

		// Token: 0x04004D50 RID: 19792
		private List<ColliderZone> _elementsToCleanUp = new List<ColliderZone>();
	}
}
