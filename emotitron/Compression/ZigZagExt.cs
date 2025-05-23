using System;

namespace emotitron.Compression
{
	// Token: 0x02000E13 RID: 3603
	public static class ZigZagExt
	{
		// Token: 0x06005A09 RID: 23049 RVA: 0x001B7AB5 File Offset: 0x001B5CB5
		public static ulong ZigZag(this long s)
		{
			return (ulong)((s << 1) ^ (s >> 63));
		}

		// Token: 0x06005A0A RID: 23050 RVA: 0x001B7ABF File Offset: 0x001B5CBF
		public static long UnZigZag(this ulong u)
		{
			return (long)((u >> 1) ^ -(long)(u & 1UL));
		}

		// Token: 0x06005A0B RID: 23051 RVA: 0x001B7ACA File Offset: 0x001B5CCA
		public static uint ZigZag(this int s)
		{
			return (uint)((s << 1) ^ (s >> 31));
		}

		// Token: 0x06005A0C RID: 23052 RVA: 0x001B7AD4 File Offset: 0x001B5CD4
		public static int UnZigZag(this uint u)
		{
			return (int)((ulong)(u >> 1) ^ (ulong)((long)(-(long)(u & 1U))));
		}

		// Token: 0x06005A0D RID: 23053 RVA: 0x001B7AE1 File Offset: 0x001B5CE1
		public static ushort ZigZag(this short s)
		{
			return (ushort)(((int)s << 1) ^ (s >> 15));
		}

		// Token: 0x06005A0E RID: 23054 RVA: 0x001B7AEC File Offset: 0x001B5CEC
		public static short UnZigZag(this ushort u)
		{
			return (short)((u >> 1) ^ (int)(-(int)((short)(u & 1))));
		}

		// Token: 0x06005A0F RID: 23055 RVA: 0x001B7AF8 File Offset: 0x001B5CF8
		public static byte ZigZag(this sbyte s)
		{
			return (byte)(((int)s << 1) ^ (s >> 7));
		}

		// Token: 0x06005A10 RID: 23056 RVA: 0x001B7B02 File Offset: 0x001B5D02
		public static sbyte UnZigZag(this byte u)
		{
			return (sbyte)((u >> 1) ^ (int)(-(int)((sbyte)(u & 1))));
		}
	}
}
