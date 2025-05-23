using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BDB RID: 3035
	public class InteractableRegistry : MonoBehaviour
	{
		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06004AE7 RID: 19175 RVA: 0x001642FC File Offset: 0x001624FC
		public static HashSet<Interactable> Interactables
		{
			get
			{
				return InteractableRegistry._interactables;
			}
		}

		// Token: 0x06004AE8 RID: 19176 RVA: 0x00164303 File Offset: 0x00162503
		public static void RegisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Add(interactable);
		}

		// Token: 0x06004AE9 RID: 19177 RVA: 0x00164311 File Offset: 0x00162511
		public static void UnregisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Remove(interactable);
		}

		// Token: 0x04004D9B RID: 19867
		public static HashSet<Interactable> _interactables = new HashSet<Interactable>();
	}
}
