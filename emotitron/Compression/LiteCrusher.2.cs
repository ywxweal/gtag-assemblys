using System;

namespace emotitron.Compression
{
	// Token: 0x02000E0E RID: 3598
	[Serializable]
	public abstract class LiteCrusher<T> : LiteCrusher where T : struct
	{
		// Token: 0x060059F0 RID: 23024
		public abstract ulong Encode(T val);

		// Token: 0x060059F1 RID: 23025
		public abstract T Decode(uint val);

		// Token: 0x060059F2 RID: 23026
		public abstract ulong WriteValue(T val, byte[] buffer, ref int bitposition);

		// Token: 0x060059F3 RID: 23027
		public abstract void WriteCValue(uint val, byte[] buffer, ref int bitposition);

		// Token: 0x060059F4 RID: 23028
		public abstract T ReadValue(byte[] buffer, ref int bitposition);
	}
}
