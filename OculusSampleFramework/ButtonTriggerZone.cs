using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BCF RID: 3023
	public class ButtonTriggerZone : MonoBehaviour, ColliderZone
	{
		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x06004AA5 RID: 19109 RVA: 0x00163A76 File Offset: 0x00161C76
		// (set) Token: 0x06004AA6 RID: 19110 RVA: 0x00163A7E File Offset: 0x00161C7E
		public Collider Collider { get; private set; }

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x06004AA7 RID: 19111 RVA: 0x00163A87 File Offset: 0x00161C87
		// (set) Token: 0x06004AA8 RID: 19112 RVA: 0x00163A8F File Offset: 0x00161C8F
		public Interactable ParentInteractable { get; private set; }

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x06004AA9 RID: 19113 RVA: 0x00163A98 File Offset: 0x00161C98
		public InteractableCollisionDepth CollisionDepth
		{
			get
			{
				if (this.ParentInteractable.ProximityCollider == this)
				{
					return InteractableCollisionDepth.Proximity;
				}
				if (this.ParentInteractable.ContactCollider == this)
				{
					return InteractableCollisionDepth.Contact;
				}
				if (this.ParentInteractable.ActionCollider != this)
				{
					return InteractableCollisionDepth.None;
				}
				return InteractableCollisionDepth.Action;
			}
		}

		// Token: 0x06004AAA RID: 19114 RVA: 0x00163AD8 File Offset: 0x00161CD8
		private void Awake()
		{
			this.Collider = base.GetComponent<Collider>();
			this.ParentInteractable = this._parentInteractableObj.GetComponent<Interactable>();
		}

		// Token: 0x04004D63 RID: 19811
		[SerializeField]
		private GameObject _parentInteractableObj;
	}
}
