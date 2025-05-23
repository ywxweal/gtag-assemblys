using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D7C RID: 3452
	[Serializable]
	public struct CosmeticPartMirrorOption
	{
		// Token: 0x0400595B RID: 22875
		public ECosmeticPartMirrorAxis axis;

		// Token: 0x0400595C RID: 22876
		[Tooltip("This will multiply the local scale for the selected axis by -1.")]
		public bool negativeScale;
	}
}
