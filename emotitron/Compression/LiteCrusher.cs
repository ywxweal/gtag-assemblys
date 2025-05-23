using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x02000E0D RID: 3597
	[Serializable]
	public abstract class LiteCrusher
	{
		// Token: 0x060059EE RID: 23022 RVA: 0x001B7358 File Offset: 0x001B5558
		public static int GetBitsForMaxValue(uint maxvalue)
		{
			for (int i = 0; i < 32; i++)
			{
				if (maxvalue >> i == 0U)
				{
					return i;
				}
			}
			return 32;
		}

		// Token: 0x04005E3A RID: 24122
		[SerializeField]
		protected int bits;
	}
}
