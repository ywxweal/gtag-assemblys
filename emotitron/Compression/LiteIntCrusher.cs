using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x02000E12 RID: 3602
	[Serializable]
	public class LiteIntCrusher : LiteCrusher<int>
	{
		// Token: 0x060059FF RID: 23039 RVA: 0x001B7740 File Offset: 0x001B5940
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

		// Token: 0x06005A00 RID: 23040 RVA: 0x001B7796 File Offset: 0x001B5996
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

		// Token: 0x06005A01 RID: 23041 RVA: 0x001B77D8 File Offset: 0x001B59D8
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

		// Token: 0x06005A02 RID: 23042 RVA: 0x001B7844 File Offset: 0x001B5A44
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

		// Token: 0x06005A03 RID: 23043 RVA: 0x001B7894 File Offset: 0x001B5A94
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

		// Token: 0x06005A04 RID: 23044 RVA: 0x001B78E9 File Offset: 0x001B5AE9
		public override ulong Encode(int value)
		{
			value = ((value > this.biggest) ? this.biggest : ((value < this.smallest) ? this.smallest : value));
			return (ulong)((long)(value - this.smallest));
		}

		// Token: 0x06005A05 RID: 23045 RVA: 0x001B7919 File Offset: 0x001B5B19
		public override int Decode(uint cvalue)
		{
			return (int)((ulong)cvalue + (ulong)((long)this.smallest));
		}

		// Token: 0x06005A06 RID: 23046 RVA: 0x001B7928 File Offset: 0x001B5B28
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

		// Token: 0x06005A07 RID: 23047 RVA: 0x001B7958 File Offset: 0x001B5B58
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

		// Token: 0x04005E54 RID: 24148
		[SerializeField]
		public LiteIntCompressType compressType;

		// Token: 0x04005E55 RID: 24149
		[SerializeField]
		protected int min;

		// Token: 0x04005E56 RID: 24150
		[SerializeField]
		protected int max;

		// Token: 0x04005E57 RID: 24151
		[SerializeField]
		private int smallest;

		// Token: 0x04005E58 RID: 24152
		[SerializeField]
		private int biggest;
	}
}
