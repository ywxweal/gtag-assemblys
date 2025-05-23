using System;

namespace emotitron.Compression
{
	// Token: 0x02000E13 RID: 3603
	public static class ZigZagExt
	{
		// Token: 0x06005A08 RID: 23048 RVA: 0x001B79DD File Offset: 0x001B5BDD
		public static ulong ZigZag(this long s)
		{
			return (ulong)((s << 1) ^ (s >> 63));
		}

		// Token: 0x06005A09 RID: 23049 RVA: 0x001B79E7 File Offset: 0x001B5BE7
		public static long UnZigZag(this ulong u)
		{
			return (long)((u >> 1) ^ -(long)(u & 1UL));
		}

		// Token: 0x06005A0A RID: 23050 RVA: 0x001B79F2 File Offset: 0x001B5BF2
		public static uint ZigZag(this int s)
		{
			return (uint)((s << 1) ^ (s >> 31));
		}

		// Token: 0x06005A0B RID: 23051 RVA: 0x001B79FC File Offset: 0x001B5BFC
		public static int UnZigZag(this uint u)
		{
			return (int)((ulong)(u >> 1) ^ (ulong)((long)(-(long)(u & 1U))));
		}

		// Token: 0x06005A0C RID: 23052 RVA: 0x001B7A09 File Offset: 0x001B5C09
		public static ushort ZigZag(this short s)
		{
			return (ushort)(((int)s << 1) ^ (s >> 15));
		}

		// Token: 0x06005A0D RID: 23053 RVA: 0x001B7A14 File Offset: 0x001B5C14
		public static short UnZigZag(this ushort u)
		{
			return (short)((u >> 1) ^ (int)(-(int)((short)(u & 1))));
		}

		// Token: 0x06005A0E RID: 23054 RVA: 0x001B7A20 File Offset: 0x001B5C20
		public static byte ZigZag(this sbyte s)
		{
			return (byte)(((int)s << 1) ^ (s >> 7));
		}

		// Token: 0x06005A0F RID: 23055 RVA: 0x001B7A2A File Offset: 0x001B5C2A
		public static sbyte UnZigZag(this byte u)
		{
			return (sbyte)((u >> 1) ^ (int)(-(int)((sbyte)(u & 1))));
		}
	}
}
