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
		// (get) Token: 0x06005097 RID: 20631 RVA: 0x00180E02 File Offset: 0x0017F002
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

		// Token: 0x040053BA RID: 21434
		[SerializeField]
		private GameModeType[] modes;

		// Token: 0x040053BB RID: 21435
		private HashSet<GameModeType> modesHash;

		// Token: 0x040053BC RID: 21436
		public TagEffectPack tagEffect;

		// Token: 0x040053BD RID: 21437
		public bool blockTagOverride;

		// Token: 0x040053BE RID: 21438
		public bool blockFistBumpOverride;

		// Token: 0x040053BF RID: 21439
		public bool blockHiveFiveOverride;
	}
}
