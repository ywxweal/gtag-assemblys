using System;

namespace BoingKit
{
	// Token: 0x02000E88 RID: 3720
	public struct BitArray
	{
		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x06005D05 RID: 23813 RVA: 0x001CAF9C File Offset: 0x001C919C
		public int[] Blocks
		{
			get
			{
				return this.m_aBlock;
			}
		}

		// Token: 0x06005D06 RID: 23814 RVA: 0x001CAFA4 File Offset: 0x001C91A4
		private static int GetBlockIndex(int index)
		{
			return index / 4;
		}

		// Token: 0x06005D07 RID: 23815 RVA: 0x001CAFA9 File Offset: 0x001C91A9
		private static int GetSubIndex(int index)
		{
			return index % 4;
		}

		// Token: 0x06005D08 RID: 23816 RVA: 0x001CAFB0 File Offset: 0x001C91B0
		private static void SetBit(int index, bool value, int[] blocks)
		{
			int blockIndex = BitArray.GetBlockIndex(index);
			int subIndex = BitArray.GetSubIndex(index);
			if (value)
			{
				blocks[blockIndex] |= 1 << subIndex;
				return;
			}
			blocks[blockIndex] &= ~(1 << subIndex);
		}

		// Token: 0x06005D09 RID: 23817 RVA: 0x001CAFF2 File Offset: 0x001C91F2
		private static bool IsBitSet(int index, int[] blocks)
		{
			return (blocks[BitArray.GetBlockIndex(index)] & (1 << BitArray.GetSubIndex(index))) != 0;
		}

		// Token: 0x06005D0A RID: 23818 RVA: 0x001CB00C File Offset: 0x001C920C
		public BitArray(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			this.m_aBlock = new int[num];
			this.Clear();
		}

		// Token: 0x06005D0B RID: 23819 RVA: 0x001CB034 File Offset: 0x001C9234
		public void Resize(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			if (num <= this.m_aBlock.Length)
			{
				return;
			}
			int[] array = new int[num];
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				array[i] = this.m_aBlock[i];
				i++;
			}
			this.m_aBlock = array;
		}

		// Token: 0x06005D0C RID: 23820 RVA: 0x001CB083 File Offset: 0x001C9283
		public void Clear()
		{
			this.SetAllBits(false);
		}

		// Token: 0x06005D0D RID: 23821 RVA: 0x001CB08C File Offset: 0x001C928C
		public void SetAllBits(bool value)
		{
			int num = (value ? (-1) : 1);
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				this.m_aBlock[i] = num;
				i++;
			}
		}

		// Token: 0x06005D0E RID: 23822 RVA: 0x001CB0BF File Offset: 0x001C92BF
		public void SetBit(int index, bool value)
		{
			BitArray.SetBit(index, value, this.m_aBlock);
		}

		// Token: 0x06005D0F RID: 23823 RVA: 0x001CB0CE File Offset: 0x001C92CE
		public bool IsBitSet(int index)
		{
			return BitArray.IsBitSet(index, this.m_aBlock);
		}

		// Token: 0x0400611F RID: 24863
		private int[] m_aBlock;
	}
}
