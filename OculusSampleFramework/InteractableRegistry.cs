using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BDB RID: 3035
	public class InteractableRegistry : MonoBehaviour
	{
		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06004AE6 RID: 19174 RVA: 0x00164224 File Offset: 0x00162424
		public static HashSet<Interactable> Interactables
		{
			get
			{
				return InteractableRegistry._interactables;
			}
		}

		// Token: 0x06004AE7 RID: 19175 RVA: 0x0016422B File Offset: 0x0016242B
		public static void RegisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Add(interactable);
		}

		// Token: 0x06004AE8 RID: 19176 RVA: 0x00164239 File Offset: 0x00162439
		public static void UnregisterInteractable(Interactable interactable)
		{
			InteractableRegistry.Interactables.Remove(interactable);
		}

		// Token: 0x04004D9A RID: 19866
		public static HashSet<Interactable> _interactables = new HashSet<Interactable>();
	}
}
