using System;
using UnityEngine;

// Token: 0x02000991 RID: 2449
public class FlagForLighting : MonoBehaviour
{
	// Token: 0x04003F9D RID: 16285
	public FlagForLighting.TimeOfDay myTimeOfDay;

	// Token: 0x02000992 RID: 2450
	public enum TimeOfDay
	{
		// Token: 0x04003F9F RID: 16287
		Sunrise,
		// Token: 0x04003FA0 RID: 16288
		TenAM,
		// Token: 0x04003FA1 RID: 16289
		Noon,
		// Token: 0x04003FA2 RID: 16290
		ThreePM,
		// Token: 0x04003FA3 RID: 16291
		Sunset,
		// Token: 0x04003FA4 RID: 16292
		Night,
		// Token: 0x04003FA5 RID: 16293
		RainingDay,
		// Token: 0x04003FA6 RID: 16294
		RainingNight,
		// Token: 0x04003FA7 RID: 16295
		None
	}
}
