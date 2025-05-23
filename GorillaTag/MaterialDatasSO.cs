using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D18 RID: 3352
	[CreateAssetMenu(fileName = "MaterialDatasSO", menuName = "Gorilla Tag/MaterialDatasSO")]
	public class MaterialDatasSO : ScriptableObject
	{
		// Token: 0x040056D7 RID: 22231
		public List<GTPlayer.MaterialData> datas;

		// Token: 0x040056D8 RID: 22232
		public List<HashWrapper> surfaceEffects;
	}
}
