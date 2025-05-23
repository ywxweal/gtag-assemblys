using System;

namespace emotitron.Compression
{
	// Token: 0x02000E0B RID: 3595
	public static class PrimitivePackBytesExt
	{
		// Token: 0x0600597C RID: 22908 RVA: 0x001B6AF4 File Offset: 0x001B4CF4
		public static ulong WritePackedBytes(this ulong buffer, ulong value, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write(value, ref bitposition, num2 << 3);
			return buffer;
		}

		// Token: 0x0600597D RID: 22909 RVA: 0x001B6B30 File Offset: 0x001B4D30
		public static uint WritePackedBytes(this uint buffer, uint value, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2 << 3);
			return buffer;
		}

		// Token: 0x0600597E RID: 22910 RVA: 0x001B6B6C File Offset: 0x001B4D6C
		public static void InjectPackedBytes(this ulong value, ref ulong buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write(value, ref bitposition, num2 << 3);
		}

		// Token: 0x0600597F RID: 22911 RVA: 0x001B6BA8 File Offset: 0x001B4DA8
		public static void InjectPackedBytes(this uint value, ref uint buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2 << 3);
		}

		// Token: 0x06005980 RID: 22912 RVA: 0x001B6BE4 File Offset: 0x001B4DE4
		public static ulong ReadPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2 << 3);
		}

		// Token: 0x06005981 RID: 22913 RVA: 0x001B6C10 File Offset: 0x001B4E10
		public static uint ReadPackedBytes(this uint buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2 << 3);
		}

		// Token: 0x06005982 RID: 22914 RVA: 0x001B6C3C File Offset: 0x001B4E3C
		public static ulong WriteSignedPackedBytes(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005983 RID: 22915 RVA: 0x001B6C5C File Offset: 0x001B4E5C
		public static int ReadSignedPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}
	}
}
