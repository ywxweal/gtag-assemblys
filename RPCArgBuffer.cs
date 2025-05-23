using System;
using System.Runtime.InteropServices;

// Token: 0x0200029C RID: 668
public struct RPCArgBuffer<T> where T : struct
{
	// Token: 0x06000F95 RID: 3989 RVA: 0x0004E9EE File Offset: 0x0004CBEE
	public RPCArgBuffer(T argStruct)
	{
		this.DataLength = Marshal.SizeOf(typeof(T));
		this.Data = new byte[this.DataLength];
		this.Args = argStruct;
	}

	// Token: 0x0400127E RID: 4734
	public T Args;

	// Token: 0x0400127F RID: 4735
	public byte[] Data;

	// Token: 0x04001280 RID: 4736
	public int DataLength;
}
