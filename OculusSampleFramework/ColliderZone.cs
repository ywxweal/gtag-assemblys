using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BD0 RID: 3024
	public interface ColliderZone
	{
		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x06004AAB RID: 19115
		Collider Collider { get; }

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x06004AAC RID: 19116
		Interactable ParentInteractable { get; }

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x06004AAD RID: 19117
		InteractableCollisionDepth CollisionDepth { get; }
	}
}
