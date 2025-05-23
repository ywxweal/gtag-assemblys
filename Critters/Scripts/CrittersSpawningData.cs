using System;
using System.Collections.Generic;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000E1D RID: 3613
	public class CrittersSpawningData : MonoBehaviour
	{
		// Token: 0x06005A77 RID: 23159 RVA: 0x001B9034 File Offset: 0x001B7234
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

		// Token: 0x06005A78 RID: 23160 RVA: 0x001B9080 File Offset: 0x001B7280
		public int GetRandomTemplate()
		{
			int num = Random.Range(0, this.templateCollection.Count - 1);
			return this.templateCollection[num];
		}

		// Token: 0x04005E93 RID: 24211
		public List<CrittersSpawningData.CreatureSpawnParameters> SpawnParametersList;

		// Token: 0x04005E94 RID: 24212
		private List<int> templateCollection = new List<int>();

		// Token: 0x02000E1E RID: 3614
		[Serializable]
		public class CreatureSpawnParameters
		{
			// Token: 0x04005E95 RID: 24213
			public CritterTemplate Template;

			// Token: 0x04005E96 RID: 24214
			public int ChancesToSpawn;

			// Token: 0x04005E97 RID: 24215
			[HideInInspector]
			[NonSerialized]
			public int StartingIndex;
		}
	}
}
