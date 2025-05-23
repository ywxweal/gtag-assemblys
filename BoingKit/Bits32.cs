using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E87 RID: 3719
	[Serializable]
	public struct Bits32
	{
		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x06005D00 RID: 23808 RVA: 0x001CAF43 File Offset: 0x001C9143
		public int IntValue
		{
			get
			{
				return this.m_bits;
			}
		}

		// Token: 0x06005D01 RID: 23809 RVA: 0x001CAF4B File Offset: 0x001C914B
		public Bits32(int bits = 0)
		{
			this.m_bits = bits;
		}

		// Token: 0x06005D02 RID: 23810 RVA: 0x001CAF54 File Offset: 0x001C9154
		public void Clear()
		{
			this.m_bits = 0;
		}

		// Token: 0x06005D03 RID: 23811 RVA: 0x001CAF5D File Offset: 0x001C915D
		public void SetBit(int index, bool value)
		{
			if (value)
			{
				this.m_bits |= 1 << index;
				return;
			}
			this.m_bits &= ~(1 << index);
		}

		// Token: 0x06005D04 RID: 23812 RVA: 0x001CAF8A File Offset: 0x001C918A
		public bool IsBitSet(int index)
		{
			return (this.m_bits & (1 << index)) != 0;
		}

		// Token: 0x0400611E RID: 24862
		[SerializeField]
		private int m_bits;
	}
}
