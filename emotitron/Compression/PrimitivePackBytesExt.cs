using System;

namespace emotitron.Compression
{
	// Token: 0x02000E0B RID: 3595
	public static class PrimitivePackBytesExt
	{
		// Token: 0x0600597B RID: 22907 RVA: 0x001B6A1C File Offset: 0x001B4C1C
		public static ulong WritePackedBytes(this ulong buffer, ulong value, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write(value, ref bitposition, num2 << 3);
			return buffer;
		}

		// Token: 0x0600597C RID: 22908 RVA: 0x001B6A58 File Offset: 0x001B4C58
		public static uint WritePackedBytes(this uint buffer, uint value, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2 << 3);
			return buffer;
		}

		// Token: 0x0600597D RID: 22909 RVA: 0x001B6A94 File Offset: 0x001B4C94
		public static void InjectPackedBytes(this ulong value, ref ulong buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write(value, ref bitposition, num2 << 3);
		}

		// Token: 0x0600597E RID: 22910 RVA: 0x001B6AD0 File Offset: 0x001B4CD0
		public static void InjectPackedBytes(this uint value, ref uint buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = value.UsedByteCount();
			buffer = buffer.Write((ulong)num2, ref bitposition, num);
			buffer = buffer.Write((ulong)value, ref bitposition, num2 << 3);
		}

		// Token: 0x0600597F RID: 22911 RVA: 0x001B6B0C File Offset: 0x001B4D0C
		public static ulong ReadPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2 << 3);
		}

		// Token: 0x06005980 RID: 22912 RVA: 0x001B6B38 File Offset: 0x001B4D38
		public static uint ReadPackedBytes(this uint buffer, ref int bitposition, int bits)
		{
			int num = (bits + 7 >> 3).UsedBitCount();
			int num2 = (int)buffer.Read(ref bitposition, num);
			return buffer.Read(ref bitposition, num2 << 3);
		}

		// Token: 0x06005981 RID: 22913 RVA: 0x001B6B64 File Offset: 0x001B4D64
		public static ulong WriteSignedPackedBytes(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005982 RID: 22914 RVA: 0x001B6B84 File Offset: 0x001B4D84
		public static int ReadSignedPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}
	}
}
