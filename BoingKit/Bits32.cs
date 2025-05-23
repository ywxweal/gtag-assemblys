using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E87 RID: 3719
	[Serializable]
	public struct Bits32
	{
		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x06005D01 RID: 23809 RVA: 0x001CB01B File Offset: 0x001C921B
		public int IntValue
		{
			get
			{
				return this.m_bits;
			}
		}

		// Token: 0x06005D02 RID: 23810 RVA: 0x001CB023 File Offset: 0x001C9223
		public Bits32(int bits = 0)
		{
			this.m_bits = bits;
		}

		// Token: 0x06005D03 RID: 23811 RVA: 0x001CB02C File Offset: 0x001C922C
		public void Clear()
		{
			this.m_bits = 0;
		}

		// Token: 0x06005D04 RID: 23812 RVA: 0x001CB035 File Offset: 0x001C9235
		public void SetBit(int index, bool value)
		{
			if (value)
			{
				this.m_bits |= 1 << index;
				return;
			}
			this.m_bits &= ~(1 << index);
		}

		// Token: 0x06005D05 RID: 23813 RVA: 0x001CB062 File Offset: 0x001C9262
		public bool IsBitSet(int index)
		{
			return (this.m_bits & (1 << index)) != 0;
		}

		// Token: 0x0400611F RID: 24863
		[SerializeField]
		private int m_bits;
	}
}
