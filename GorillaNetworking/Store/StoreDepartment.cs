using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C8E RID: 3214
	public class StoreDepartment : MonoBehaviour
	{
		// Token: 0x06004FAB RID: 20395 RVA: 0x0017BF2C File Offset: 0x0017A12C
		private void FindAllDisplays()
		{
			this.Displays = base.GetComponentsInChildren<StoreDisplay>();
			for (int i = this.Displays.Length - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(this.Displays[i].displayName))
				{
					this.Displays[i] = this.Displays[this.Displays.Length - 1];
					Array.Resize<StoreDisplay>(ref this.Displays, this.Displays.Length - 1);
				}
			}
		}

		// Token: 0x040052C2 RID: 21186
		public StoreDisplay[] Displays;

		// Token: 0x040052C3 RID: 21187
		public string departmentName = "";
	}
}
