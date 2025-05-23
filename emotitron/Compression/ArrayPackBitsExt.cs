using System;

namespace emotitron.Compression
{
	// Token: 0x02000E02 RID: 3586
	public static class ArrayPackBitsExt
	{
		// Token: 0x060058BB RID: 22715 RVA: 0x001B49D0 File Offset: 0x001B2BD0
		public unsafe static void WritePackedBits(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int num2 = bits.UsedBitCount();
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, num2);
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, num);
		}

		// Token: 0x060058BC RID: 22716 RVA: 0x001B4A04 File Offset: 0x001B2C04
		public static void WritePackedBits(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int num2 = bits.UsedBitCount();
			buffer.Write((ulong)num, ref bitposition, num2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x060058BD RID: 22717 RVA: 0x001B4A38 File Offset: 0x001B2C38
		public static void WritePackedBits(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int num2 = bits.UsedBitCount();
			buffer.Write((ulong)((long)num), ref bitposition, num2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x060058BE RID: 22718 RVA: 0x001B4A6C File Offset: 0x001B2C6C
		public static void WritePackedBits(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			int num = value.UsedBitCount();
			int num2 = bits.UsedBitCount();
			buffer.Write((ulong)num, ref bitposition, num2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x060058BF RID: 22719 RVA: 0x001B4A9C File Offset: 0x001B2C9C
		public unsafe static ulong ReadPackedBits(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = bits.UsedBitCount();
			int num2 = (int)ArraySerializeUnsafe.Read(uPtr, ref bitposition, num);
			return ArraySerializeUnsafe.Read(uPtr, ref bitposition, num2);
		}

		// Token: 0x060058C0 RID: 22720 RVA: 0x001B4AC8 File Offset: 0x001B2CC8
		public static ulong ReadPackedBits(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x060058C1 RID: 22721 RVA: 0x001B4AF4 File Offset: 0x001B2CF4
		public static ulong ReadPackedBits(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x060058C2 RID: 22722 RVA: 0x001B4B20 File Offset: 0x001B2D20
		public static ulong ReadPackedBits(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x060058C3 RID: 22723 RVA: 0x001B4B4C File Offset: 0x001B2D4C
		public unsafe static void WriteSignedPackedBits(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArrayPackBitsExt.WritePackedBits(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058C4 RID: 22724 RVA: 0x001B4B6C File Offset: 0x001B2D6C
		public unsafe static int ReadSignedPackedBits(ulong* buffer, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBitsExt.ReadPackedBits(buffer, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058C5 RID: 22725 RVA: 0x001B4B90 File Offset: 0x001B2D90
		public static void WriteSignedPackedBits(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058C6 RID: 22726 RVA: 0x001B4BB0 File Offset: 0x001B2DB0
		public static int ReadSignedPackedBits(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058C7 RID: 22727 RVA: 0x001B4BD4 File Offset: 0x001B2DD4
		public static void WriteSignedPackedBits(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058C8 RID: 22728 RVA: 0x001B4BF4 File Offset: 0x001B2DF4
		public static int ReadSignedPackedBits(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058C9 RID: 22729 RVA: 0x001B4C18 File Offset: 0x001B2E18
		public static void WriteSignedPackedBits(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058CA RID: 22730 RVA: 0x001B4C38 File Offset: 0x001B2E38
		public static int ReadSignedPackedBits(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058CB RID: 22731 RVA: 0x001B4C5C File Offset: 0x001B2E5C
		public static void WriteSignedPackedBits64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.WritePackedBits(num, ref bitposition, bits);
		}

		// Token: 0x060058CC RID: 22732 RVA: 0x001B4C7C File Offset: 0x001B2E7C
		public static long ReadSignedPackedBits64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBits(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}
	}
}
