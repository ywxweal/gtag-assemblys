using System;

namespace emotitron.Compression
{
	// Token: 0x02000E03 RID: 3587
	public static class ArrayPackBytesExt
	{
		// Token: 0x060058CE RID: 22734 RVA: 0x001B4D74 File Offset: 0x001B2F74
		public unsafe static void WritePackedBytes(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			ArraySerializeUnsafe.Write(uPtr, (ulong)num2, ref bitposition, num);
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, num2 << 3);
		}

		// Token: 0x060058CF RID: 22735 RVA: 0x001B4DAC File Offset: 0x001B2FAC
		public static void WritePackedBytes(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer.Write((ulong)num2, ref bitposition, num);
			buffer.Write(value, ref bitposition, num2 << 3);
		}

		// Token: 0x060058D0 RID: 22736 RVA: 0x001B4DE4 File Offset: 0x001B2FE4
		public static void WritePackedBytes(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer.Write((ulong)num2, ref bitposition, num);
			buffer.Write(value, ref bitposition, num2 << 3);
		}

		// Token: 0x060058D1 RID: 22737 RVA: 0x001B4E1C File Offset: 0x001B301C
		public static void WritePackedBytes(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer.Write((ulong)num2, ref bitposition, num);
			buffer.Write(value, ref bitposition, num2 << 3);
		}

		// Token: 0x060058D2 RID: 22738 RVA: 0x001B4E54 File Offset: 0x001B3054
		public unsafe static ulong ReadPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)ArraySerializeUnsafe.Read(uPtr, ref bitposition, num) << 3;
			return ArraySerializeUnsafe.Read(uPtr, ref bitposition, num2);
		}

		// Token: 0x060058D3 RID: 22739 RVA: 0x001B4E88 File Offset: 0x001B3088
		public static ulong ReadPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num) << 3;
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x060058D4 RID: 22740 RVA: 0x001B4EBC File Offset: 0x001B30BC
		public static ulong ReadPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num) << 3;
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x060058D5 RID: 22741 RVA: 0x001B4EF0 File Offset: 0x001B30F0
		public static ulong ReadPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num) << 3;
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x060058D6 RID: 22742 RVA: 0x001B4F24 File Offset: 0x001B3124
		public unsafe static void WriteSignedPackedBytes(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArrayPackBytesExt.WritePackedBytes(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058D7 RID: 22743 RVA: 0x001B4F44 File Offset: 0x001B3144
		public unsafe static int ReadSignedPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBytesExt.ReadPackedBytes(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058D8 RID: 22744 RVA: 0x001B4F68 File Offset: 0x001B3168
		public static void WriteSignedPackedBytes(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058D9 RID: 22745 RVA: 0x001B4F88 File Offset: 0x001B3188
		public static int ReadSignedPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058DA RID: 22746 RVA: 0x001B4FAC File Offset: 0x001B31AC
		public static void WriteSignedPackedBytes(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058DB RID: 22747 RVA: 0x001B4FCC File Offset: 0x001B31CC
		public static int ReadSignedPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058DC RID: 22748 RVA: 0x001B4FF0 File Offset: 0x001B31F0
		public static void WriteSignedPackedBytes(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058DD RID: 22749 RVA: 0x001B5010 File Offset: 0x001B3210
		public static int ReadSignedPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058DE RID: 22750 RVA: 0x001B5034 File Offset: 0x001B3234
		public static void WriteSignedPackedBytes64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.WritePackedBytes(num, ref bitposition, bits);
		}

		// Token: 0x060058DF RID: 22751 RVA: 0x001B5054 File Offset: 0x001B3254
		public static long ReadSignedPackedBytes64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBytes(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}
	}
}
