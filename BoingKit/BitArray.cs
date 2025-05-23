using System;

namespace BoingKit
{
	// Token: 0x02000E88 RID: 3720
	public struct BitArray
	{
		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x06005D06 RID: 23814 RVA: 0x001CB074 File Offset: 0x001C9274
		public int[] Blocks
		{
			get
			{
				return this.m_aBlock;
			}
		}

		// Token: 0x06005D07 RID: 23815 RVA: 0x001CB07C File Offset: 0x001C927C
		private static int GetBlockIndex(int index)
		{
			return index / 4;
		}

		// Token: 0x06005D08 RID: 23816 RVA: 0x001CB081 File Offset: 0x001C9281
		private static int GetSubIndex(int index)
		{
			return index % 4;
		}

		// Token: 0x06005D09 RID: 23817 RVA: 0x001CB088 File Offset: 0x001C9288
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

		// Token: 0x06005D0A RID: 23818 RVA: 0x001CB0CA File Offset: 0x001C92CA
		private static bool IsBitSet(int index, int[] blocks)
		{
			return (blocks[BitArray.GetBlockIndex(index)] & (1 << BitArray.GetSubIndex(index))) != 0;
		}

		// Token: 0x06005D0B RID: 23819 RVA: 0x001CB0E4 File Offset: 0x001C92E4
		public BitArray(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			this.m_aBlock = new int[num];
			this.Clear();
		}

		// Token: 0x06005D0C RID: 23820 RVA: 0x001CB10C File Offset: 0x001C930C
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

		// Token: 0x06005D0D RID: 23821 RVA: 0x001CB15B File Offset: 0x001C935B
		public void Clear()
		{
			this.SetAllBits(false);
		}

		// Token: 0x06005D0E RID: 23822 RVA: 0x001CB164 File Offset: 0x001C9364
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

		// Token: 0x06005D0F RID: 23823 RVA: 0x001CB197 File Offset: 0x001C9397
		public void SetBit(int index, bool value)
		{
			BitArray.SetBit(index, value, this.m_aBlock);
		}

		// Token: 0x06005D10 RID: 23824 RVA: 0x001CB1A6 File Offset: 0x001C93A6
		public bool IsBitSet(int index)
		{
			return BitArray.IsBitSet(index, this.m_aBlock);
		}

		// Token: 0x04006120 RID: 24864
		private int[] m_aBlock;
	}
}
