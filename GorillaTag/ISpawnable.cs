using System;
using GorillaTag.CosmeticSystem;

namespace GorillaTag
{
	// Token: 0x02000D07 RID: 3335
	public interface ISpawnable
	{
		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x060053AC RID: 21420
		// (set) Token: 0x060053AD RID: 21421
		bool IsSpawned { get; set; }

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x060053AE RID: 21422
		// (set) Token: 0x060053AF RID: 21423
		ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x060053B0 RID: 21424
		void OnSpawn(VRRig rig);

		// Token: 0x060053B1 RID: 21425
		void OnDespawn();
	}
}
