using System;

namespace emotitron.Compression
{
	// Token: 0x02000E03 RID: 3587
	public static class ArrayPackBytesExt
	{
		// Token: 0x060058CD RID: 22733 RVA: 0x001B4C9C File Offset: 0x001B2E9C
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

		// Token: 0x060058CE RID: 22734 RVA: 0x001B4CD4 File Offset: 0x001B2ED4
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

		// Token: 0x060058CF RID: 22735 RVA: 0x001B4D0C File Offset: 0x001B2F0C
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

		// Token: 0x060058D0 RID: 22736 RVA: 0x001B4D44 File Offset: 0x001B2F44
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

		// Token: 0x060058D1 RID: 22737 RVA: 0x001B4D7C File Offset: 0x001B2F7C
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

		// Token: 0x060058D2 RID: 22738 RVA: 0x001B4DB0 File Offset: 0x001B2FB0
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

		// Token: 0x060058D3 RID: 22739 RVA: 0x001B4DE4 File Offset: 0x001B2FE4
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

		// Token: 0x060058D4 RID: 22740 RVA: 0x001B4E18 File Offset: 0x001B3018
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

		// Token: 0x060058D5 RID: 22741 RVA: 0x001B4E4C File Offset: 0x001B304C
		public unsafe static void WriteSignedPackedBytes(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArrayPackBytesExt.WritePackedBytes(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058D6 RID: 22742 RVA: 0x001B4E6C File Offset: 0x001B306C
		public unsafe static int ReadSignedPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBytesExt.ReadPackedBytes(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058D7 RID: 22743 RVA: 0x001B4E90 File Offset: 0x001B3090
		public static void WriteSignedPackedBytes(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058D8 RID: 22744 RVA: 0x001B4EB0 File Offset: 0x001B30B0
		public static int ReadSignedPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058D9 RID: 22745 RVA: 0x001B4ED4 File Offset: 0x001B30D4
		public static void WriteSignedPackedBytes(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058DA RID: 22746 RVA: 0x001B4EF4 File Offset: 0x001B30F4
		public static int ReadSignedPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058DB RID: 22747 RVA: 0x001B4F18 File Offset: 0x001B3118
		public static void WriteSignedPackedBytes(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058DC RID: 22748 RVA: 0x001B4F38 File Offset: 0x001B3138
		public static int ReadSignedPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060058DD RID: 22749 RVA: 0x001B4F5C File Offset: 0x001B315C
		public static void WriteSignedPackedBytes64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.WritePackedBytes(num, ref bitposition, bits);
		}

		// Token: 0x060058DE RID: 22750 RVA: 0x001B4F7C File Offset: 0x001B317C
		public static long ReadSignedPackedBytes64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBytes(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}
	}
}
