using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000CBD RID: 3261
	[Serializable]
	public class ModeTagEffect
	{
		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x06005096 RID: 20630 RVA: 0x00180D2A File Offset: 0x0017EF2A
		public HashSet<GameModeType> Modes
		{
			get
			{
				if (this.modesHash == null)
				{
					this.modesHash = new HashSet<GameModeType>(this.modes);
				}
				return this.modesHash;
			}
		}

		// Token: 0x040053B9 RID: 21433
		[SerializeField]
		private GameModeType[] modes;

		// Token: 0x040053BA RID: 21434
		private HashSet<GameModeType> modesHash;

		// Token: 0x040053BB RID: 21435
		public TagEffectPack tagEffect;

		// Token: 0x040053BC RID: 21436
		public bool blockTagOverride;

		// Token: 0x040053BD RID: 21437
		public bool blockFistBumpOverride;

		// Token: 0x040053BE RID: 21438
		public bool blockHiveFiveOverride;
	}
}
