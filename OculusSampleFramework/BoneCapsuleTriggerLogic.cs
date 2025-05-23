using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BCC RID: 3020
	public class BoneCapsuleTriggerLogic : MonoBehaviour
	{
		// Token: 0x06004A92 RID: 19090 RVA: 0x00163407 File Offset: 0x00161607
		private void OnDisable()
		{
			this.CollidersTouchingUs.Clear();
		}

		// Token: 0x06004A93 RID: 19091 RVA: 0x00163414 File Offset: 0x00161614
		private void Update()
		{
			this.CleanUpDeadColliders();
		}

		// Token: 0x06004A94 RID: 19092 RVA: 0x0016341C File Offset: 0x0016161C
		private void OnTriggerEnter(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Add(component);
			}
		}

		// Token: 0x06004A95 RID: 19093 RVA: 0x0016345C File Offset: 0x0016165C
		private void OnTriggerExit(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Remove(component);
			}
		}

		// Token: 0x06004A96 RID: 19094 RVA: 0x0016349C File Offset: 0x0016169C
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

		// Token: 0x04004D4F RID: 19791
		public InteractableToolTags ToolTags;

		// Token: 0x04004D50 RID: 19792
		public HashSet<ColliderZone> CollidersTouchingUs = new HashSet<ColliderZone>();

		// Token: 0x04004D51 RID: 19793
		private List<ColliderZone> _elementsToCleanUp = new List<ColliderZone>();
	}
}
