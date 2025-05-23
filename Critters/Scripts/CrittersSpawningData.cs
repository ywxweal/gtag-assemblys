using System;
using System.Collections.Generic;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000E1D RID: 3613
	public class CrittersSpawningData : MonoBehaviour
	{
		// Token: 0x06005A78 RID: 23160 RVA: 0x001B910C File Offset: 0x001B730C
		public void InitializeSpawnCollection()
		{
			for (int i = 0; i < this.SpawnParametersList.Count; i++)
			{
				for (int j = 0; j < this.SpawnParametersList[i].ChancesToSpawn; j++)
				{
					this.templateCollection.Add(i);
				}
			}
		}

		// Token: 0x06005A79 RID: 23161 RVA: 0x001B9158 File Offset: 0x001B7358
		public int GetRandomTemplate()
		{
			int num = Random.Range(0, this.templateCollection.Count - 1);
			return this.templateCollection[num];
		}

		// Token: 0x04005E94 RID: 24212
		public List<CrittersSpawningData.CreatureSpawnParameters> SpawnParametersList;

		// Token: 0x04005E95 RID: 24213
		private List<int> templateCollection = new List<int>();

		// Token: 0x02000E1E RID: 3614
		[Serializable]
		public class CreatureSpawnParameters
		{
			// Token: 0x04005E96 RID: 24214
			public CritterTemplate Template;

			// Token: 0x04005E97 RID: 24215
			public int ChancesToSpawn;

			// Token: 0x04005E98 RID: 24216
			[HideInInspector]
			[NonSerialized]
			public int StartingIndex;
		}
	}
}
