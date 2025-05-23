using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x02000E12 RID: 3602
	[Serializable]
	public class LiteIntCrusher : LiteCrusher<int>
	{
		// Token: 0x06005A00 RID: 23040 RVA: 0x001B7818 File Offset: 0x001B5A18
		public LiteIntCrusher()
		{
			this.compressType = LiteIntCompressType.PackSigned;
			this.min = -128;
			this.max = 127;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(this.min, this.max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x06005A01 RID: 23041 RVA: 0x001B786E File Offset: 0x001B5A6E
		public LiteIntCrusher(LiteIntCompressType comType = LiteIntCompressType.PackSigned, int min = -128, int max = 127)
		{
			this.compressType = comType;
			this.min = min;
			this.max = max;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(min, max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x06005A02 RID: 23042 RVA: 0x001B78B0 File Offset: 0x001B5AB0
		public override ulong WriteValue(int val, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
			{
				uint num = (uint)((val << 1) ^ (val >> 31));
				buffer.WritePackedBytes((ulong)num, ref bitposition, 32);
				return (ulong)num;
			}
			case LiteIntCompressType.PackUnsigned:
				buffer.WritePackedBytes((ulong)val, ref bitposition, 32);
				return (ulong)val;
			case LiteIntCompressType.Range:
			{
				ulong num2 = this.Encode(val);
				buffer.Write(num2, ref bitposition, this.bits);
				return num2;
			}
			default:
				return 0UL;
			}
		}

		// Token: 0x06005A03 RID: 23043 RVA: 0x001B791C File Offset: 0x001B5B1C
		public override void WriteCValue(uint cval, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				buffer.WritePackedBytes((ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.PackUnsigned:
				buffer.WritePackedBytes((ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.Range:
				buffer.Write((ulong)cval, ref bitposition, this.bits);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005A04 RID: 23044 RVA: 0x001B796C File Offset: 0x001B5B6C
		public override int ReadValue(byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				return buffer.ReadSignedPackedBytes(ref bitposition, 32);
			case LiteIntCompressType.PackUnsigned:
				return (int)buffer.ReadPackedBytes(ref bitposition, 32);
			case LiteIntCompressType.Range:
			{
				uint num = (uint)buffer.Read(ref bitposition, this.bits);
				return this.Decode(num);
			}
			default:
				return 0;
			}
		}

		// Token: 0x06005A05 RID: 23045 RVA: 0x001B79C1 File Offset: 0x001B5BC1
		public override ulong Encode(int value)
		{
			value = ((value > this.biggest) ? this.biggest : ((value < this.smallest) ? this.smallest : value));
			return (ulong)((long)(value - this.smallest));
		}

		// Token: 0x06005A06 RID: 23046 RVA: 0x001B79F1 File Offset: 0x001B5BF1
		public override int Decode(uint cvalue)
		{
			return (int)((ulong)cvalue + (ulong)((long)this.smallest));
		}

		// Token: 0x06005A07 RID: 23047 RVA: 0x001B7A00 File Offset: 0x001B5C00
		public static void Recalculate(int min, int max, ref int smallest, ref int biggest, ref int bits)
		{
			if (min < max)
			{
				smallest = min;
				biggest = max;
			}
			else
			{
				smallest = max;
				biggest = min;
			}
			int num = biggest - smallest;
			bits = LiteCrusher.GetBitsForMaxValue((uint)num);
		}

		// Token: 0x06005A08 RID: 23048 RVA: 0x001B7A30 File Offset: 0x001B5C30
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.GetType().Name,
				" ",
				this.compressType.ToString(),
				" mn: ",
				this.min.ToString(),
				" mx: ",
				this.max.ToString(),
				" sm: ",
				this.smallest.ToString()
			});
		}

		// Token: 0x04005E55 RID: 24149
		[SerializeField]
		public LiteIntCompressType compressType;

		// Token: 0x04005E56 RID: 24150
		[SerializeField]
		protected int min;

		// Token: 0x04005E57 RID: 24151
		[SerializeField]
		protected int max;

		// Token: 0x04005E58 RID: 24152
		[SerializeField]
		private int smallest;

		// Token: 0x04005E59 RID: 24153
		[SerializeField]
		private int biggest;
	}
}
