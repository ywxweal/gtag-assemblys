using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C8F RID: 3215
	public class StoreDisplay : MonoBehaviour
	{
		// Token: 0x06004FAC RID: 20396 RVA: 0x0017BED7 File Offset: 0x0017A0D7
		private void GetAllDynamicCosmeticStands()
		{
			this.Stands = base.GetComponentsInChildren<DynamicCosmeticStand>();
		}

		// Token: 0x06004FAD RID: 20397 RVA: 0x0017BEE8 File Offset: 0x0017A0E8
		private void SetDisplayNameForAllStands()
		{
			DynamicCosmeticStand[] componentsInChildren = base.GetComponentsInChildren<DynamicCosmeticStand>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CopyChildsName();
			}
		}

		// Token: 0x040052C3 RID: 21187
		public string displayName = "";

		// Token: 0x040052C4 RID: 21188
		public DynamicCosmeticStand[] Stands;
	}
}
