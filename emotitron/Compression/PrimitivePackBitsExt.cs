using System;

namespace emotitron.Compression
{
	// Token: 0x02000E0A RID: 3594
	public static class PrimitivePackBitsExt
	{
		// Token: 0x06005970 RID: 22896 RVA: 0x001B6908 File Offset: 0x001B4B08
		public static ulong WritePackedBits(this ulong buffer, uint value, ref int bitposition, int bits)
		{
			int num = ((uint)bits).UsedBitCount();
			int num2 = value.UsedBitCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2);
			return buffer;
		}

		// Token: 0x06005971 RID: 22897 RVA: 0x001B693C File Offset: 0x001B4B3C
		public static uint WritePackedBits(this uint buffer, ushort value, ref int bitposition, int bits)
		{
			int num = ((uint)bits).UsedBitCount();
			int num2 = value.UsedBitCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2);
			return buffer;
		}

		// Token: 0x06005972 RID: 22898 RVA: 0x001B6970 File Offset: 0x001B4B70
		public static ushort WritePackedBits(this ushort buffer, byte value, ref int bitposition, int bits)
		{
			int num = ((uint)bits).UsedBitCount();
			int num2 = value.UsedBitCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2);
			return buffer;
		}

		// Token: 0x06005973 RID: 22899 RVA: 0x001B69A4 File Offset: 0x001B4BA4
		public static ulong ReadPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06005974 RID: 22900 RVA: 0x001B69CC File Offset: 0x001B4BCC
		public static ulong ReadPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return (ulong)buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06005975 RID: 22901 RVA: 0x001B69F4 File Offset: 0x001B4BF4
		public static ulong ReadPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return (ulong)buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06005976 RID: 22902 RVA: 0x001B6A1C File Offset: 0x001B4C1C
		public static ulong WriteSignedPackedBits(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer = buffer.WritePackedBits(num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06005977 RID: 22903 RVA: 0x001B6A40 File Offset: 0x001B4C40
		public static uint WriteSignedPackedBits(this uint buffer, short value, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			buffer = buffer.WritePackedBits((ushort)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06005978 RID: 22904 RVA: 0x001B6A64 File Offset: 0x001B4C64
		public static ushort WriteSignedPackedBits(this ushort buffer, sbyte value, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			buffer = buffer.WritePackedBits((byte)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06005979 RID: 22905 RVA: 0x001B6A88 File Offset: 0x001B4C88
		public static int ReadSignedPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x0600597A RID: 22906 RVA: 0x001B6AAC File Offset: 0x001B4CAC
		public static short ReadSignedPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (short)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}

		// Token: 0x0600597B RID: 22907 RVA: 0x001B6AD0 File Offset: 0x001B4CD0
		public static sbyte ReadSignedPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (sbyte)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}
	}
}
