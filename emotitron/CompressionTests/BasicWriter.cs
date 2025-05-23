using System;

namespace emotitron.CompressionTests
{
	// Token: 0x02000E19 RID: 3609
	public class BasicWriter
	{
		// Token: 0x06005A62 RID: 23138 RVA: 0x001B8CD2 File Offset: 0x001B6ED2
		public static void Reset()
		{
			BasicWriter.pos = 0;
		}

		// Token: 0x06005A63 RID: 23139 RVA: 0x001B8CDA File Offset: 0x001B6EDA
		public static byte[] BasicWrite(byte[] buffer, byte value)
		{
			buffer[BasicWriter.pos] = value;
			BasicWriter.pos++;
			return buffer;
		}

		// Token: 0x06005A64 RID: 23140 RVA: 0x001B8CF1 File Offset: 0x001B6EF1
		public static byte BasicRead(byte[] buffer)
		{
			byte b = buffer[BasicWriter.pos];
			BasicWriter.pos++;
			return b;
		}

		// Token: 0x04005E88 RID: 24200
		public static int pos;
	}
}
