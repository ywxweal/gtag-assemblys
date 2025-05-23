using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C8F RID: 3215
	public class StoreDisplay : MonoBehaviour
	{
		// Token: 0x06004FAD RID: 20397 RVA: 0x0017BFAF File Offset: 0x0017A1AF
		private void GetAllDynamicCosmeticStands()
		{
			this.Stands = base.GetComponentsInChildren<DynamicCosmeticStand>();
		}

		// Token: 0x06004FAE RID: 20398 RVA: 0x0017BFC0 File Offset: 0x0017A1C0
		private void SetDisplayNameForAllStands()
		{
			DynamicCosmeticStand[] componentsInChildren = base.GetComponentsInChildren<DynamicCosmeticStand>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CopyChildsName();
			}
		}

		// Token: 0x040052C4 RID: 21188
		public string displayName = "";

		// Token: 0x040052C5 RID: 21189
		public DynamicCosmeticStand[] Stands;
	}
}
