using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BCF RID: 3023
	public class ButtonTriggerZone : MonoBehaviour, ColliderZone
	{
		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x06004AA4 RID: 19108 RVA: 0x0016399E File Offset: 0x00161B9E
		// (set) Token: 0x06004AA5 RID: 19109 RVA: 0x001639A6 File Offset: 0x00161BA6
		public Collider Collider { get; private set; }

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x06004AA6 RID: 19110 RVA: 0x001639AF File Offset: 0x00161BAF
		// (set) Token: 0x06004AA7 RID: 19111 RVA: 0x001639B7 File Offset: 0x00161BB7
		public Interactable ParentInteractable { get; private set; }

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x06004AA8 RID: 19112 RVA: 0x001639C0 File Offset: 0x00161BC0
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

		// Token: 0x06004AA9 RID: 19113 RVA: 0x00163A00 File Offset: 0x00161C00
		private void Awake()
		{
			this.Collider = base.GetComponent<Collider>();
			this.ParentInteractable = this._parentInteractableObj.GetComponent<Interactable>();
		}

		// Token: 0x04004D62 RID: 19810
		[SerializeField]
		private GameObject _parentInteractableObj;
	}
}
