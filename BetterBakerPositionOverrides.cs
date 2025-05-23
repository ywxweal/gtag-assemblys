using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000976 RID: 2422
public class BetterBakerPositionOverrides : MonoBehaviour
{
	// Token: 0x04003F4D RID: 16205
	public List<BetterBakerPositionOverrides.OverridePosition> overridePositions;

	// Token: 0x02000977 RID: 2423
	[Serializable]
	public struct OverridePosition
	{
		// Token: 0x04003F4E RID: 16206
		public GameObject go;

		// Token: 0x04003F4F RID: 16207
		public Transform bakingTransform;

		// Token: 0x04003F50 RID: 16208
		public Transform gameTransform;
	}
}
