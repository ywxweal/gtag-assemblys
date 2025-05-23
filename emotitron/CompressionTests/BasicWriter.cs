using System;

namespace emotitron.CompressionTests
{
	// Token: 0x02000E19 RID: 3609
	public class BasicWriter
	{
		// Token: 0x06005A63 RID: 23139 RVA: 0x001B8DAA File Offset: 0x001B6FAA
		public static void Reset()
		{
			BasicWriter.pos = 0;
		}

		// Token: 0x06005A64 RID: 23140 RVA: 0x001B8DB2 File Offset: 0x001B6FB2
		public static byte[] BasicWrite(byte[] buffer, byte value)
		{
			buffer[BasicWriter.pos] = value;
			BasicWriter.pos++;
			return buffer;
		}

		// Token: 0x06005A65 RID: 23141 RVA: 0x001B8DC9 File Offset: 0x001B6FC9
		public static byte BasicRead(byte[] buffer)
		{
			byte b = buffer[BasicWriter.pos];
			BasicWriter.pos++;
			return b;
		}

		// Token: 0x04005E89 RID: 24201
		public static int pos;
	}
}
