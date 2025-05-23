using System;

namespace emotitron.Compression
{
	// Token: 0x02000E0A RID: 3594
	public static class PrimitivePackBitsExt
	{
		// Token: 0x0600596F RID: 22895 RVA: 0x001B6830 File Offset: 0x001B4A30
		public static ulong WritePackedBits(this ulong buffer, uint value, ref int bitposition, int bits)
		{
			int num = ((uint)bits).UsedBitCount();
			int num2 = value.UsedBitCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2);
			return buffer;
		}

		// Token: 0x06005970 RID: 22896 RVA: 0x001B6864 File Offset: 0x001B4A64
		public static uint WritePackedBits(this uint buffer, ushort value, ref int bitposition, int bits)
		{
			int num = ((uint)bits).UsedBitCount();
			int num2 = value.UsedBitCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2);
			return buffer;
		}

		// Token: 0x06005971 RID: 22897 RVA: 0x001B6898 File Offset: 0x001B4A98
		public static ushort WritePackedBits(this ushort buffer, byte value, ref int bitposition, int bits)
		{
			int num = ((uint)bits).UsedBitCount();
			int num2 = value.UsedBitCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2);
			return buffer;
		}

		// Token: 0x06005972 RID: 22898 RVA: 0x001B68CC File Offset: 0x001B4ACC
		public static ulong ReadPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06005973 RID: 22899 RVA: 0x001B68F4 File Offset: 0x001B4AF4
		public static ulong ReadPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return (ulong)buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06005974 RID: 22900 RVA: 0x001B691C File Offset: 0x001B4B1C
		public static ulong ReadPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			int num = bits.UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return (ulong)buffer.Read(ref bitposition, num2);
		}

		// Token: 0x06005975 RID: 22901 RVA: 0x001B6944 File Offset: 0x001B4B44
		public static ulong WriteSignedPackedBits(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer = buffer.WritePackedBits(num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06005976 RID: 22902 RVA: 0x001B6968 File Offset: 0x001B4B68
		public static uint WriteSignedPackedBits(this uint buffer, short value, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			buffer = buffer.WritePackedBits((ushort)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06005977 RID: 22903 RVA: 0x001B698C File Offset: 0x001B4B8C
		public static ushort WriteSignedPackedBits(this ushort buffer, sbyte value, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			buffer = buffer.WritePackedBits((byte)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06005978 RID: 22904 RVA: 0x001B69B0 File Offset: 0x001B4BB0
		public static int ReadSignedPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005979 RID: 22905 RVA: 0x001B69D4 File Offset: 0x001B4BD4
		public static short ReadSignedPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (short)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}

		// Token: 0x0600597A RID: 22906 RVA: 0x001B69F8 File Offset: 0x001B4BF8
		public static sbyte ReadSignedPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (sbyte)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}
	}
}
