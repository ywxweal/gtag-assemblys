using System;

namespace emotitron.Compression
{
	// Token: 0x02000E02 RID: 3586
	public static class ArrayPackBitsExt
	{
		// Token: 0x060058BC RID: 22716 RVA: 0x001B4AA8 File Offset: 0x001B2CA8
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

		// Token: 0x060058BD RID: 22717 RVA: 0x001B4ADC File Offset: 0x001B2CDC
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

		// Token: 0x060058BE RID: 22718 RVA: 0x001B4B10 File Offset: 0x001B2D10
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

		// Token: 0x060058BF RID: 22719 RVA: 0x001B4B44 File Offset: 0x001B2D44
		public static void WritePackedBits(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			int num = value.UsedBitCount();
			int num2 = bits.UsedBitCount();
			buffer.Write((ulong)num, ref bitposition, num2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x060058C0 RID: 22720 RVA: 0x001B4B74 File Offset: 0x001B2D74
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

		// Token: 0x060058C1 RID: 22721 RVA: 0x001B4BA0 File Offset: 0x001B2DA0
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

		// Token: 0x060058C2 RID: 22722 RVA: 0x001B4BCC File Offset: 0x001B2DCC
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

		// Token: 0x060058C3 RID: 22723 RVA: 0x001B4BF8 File Offset: 0x001B2DF8
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

		// Token: 0x060058C4 RID: 22724 RVA: 0x001B4C24 File Offset: 0x001B2E24
		public unsafe static void WriteSignedPackedBits(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArrayPackBitsExt.WritePackedBits(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058C5 RID: 22725 RVA: 0x001B4C44 File Offset: 0x001B2E44
		public unsafe static int ReadSignedPackedBits(ulong* buffer, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBitsExt.ReadPackedBits(buffer, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058C6 RID: 22726 RVA: 0x001B4C68 File Offset: 0x001B2E68
		public static void WriteSignedPackedBits(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058C7 RID: 22727 RVA: 0x001B4C88 File Offset: 0x001B2E88
		public static int ReadSignedPackedBits(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058C8 RID: 22728 RVA: 0x001B4CAC File Offset: 0x001B2EAC
		public static void WriteSignedPackedBits(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058C9 RID: 22729 RVA: 0x001B4CCC File Offset: 0x001B2ECC
		public static int ReadSignedPackedBits(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058CA RID: 22730 RVA: 0x001B4CF0 File Offset: 0x001B2EF0
		public static void WriteSignedPackedBits(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058CB RID: 22731 RVA: 0x001B4D10 File Offset: 0x001B2F10
		public static int ReadSignedPackedBits(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058CC RID: 22732 RVA: 0x001B4D34 File Offset: 0x001B2F34
		public static void WriteSignedPackedBits64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.WritePackedBits(num, ref bitposition, bits);
		}

		// Token: 0x060058CD RID: 22733 RVA: 0x001B4D54 File Offset: 0x001B2F54
		public static long ReadSignedPackedBits64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBits(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}
	}
}
