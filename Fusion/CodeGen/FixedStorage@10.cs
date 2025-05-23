using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x02000EB3 RID: 3763
	[WeaverGenerated]
	[NetworkStructWeaved(10)]
	[PreserveInPlugin]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@10 : INetworkStruct
	{
		// Token: 0x0400619D RID: 24989
		[FixedBuffer(typeof(int), 10)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@10.<Data>e__FixedBuffer Data;

		// Token: 0x0400619E RID: 24990
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x0400619F RID: 24991
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x040061A0 RID: 24992
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(12)]
		private int _3;

		// Token: 0x040061A1 RID: 24993
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(16)]
		private int _4;

		// Token: 0x040061A2 RID: 24994
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(20)]
		private int _5;

		// Token: 0x040061A3 RID: 24995
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(24)]
		private int _6;

		// Token: 0x040061A4 RID: 24996
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(28)]
		private int _7;

		// Token: 0x040061A5 RID: 24997
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(32)]
		private int _8;

		// Token: 0x040061A6 RID: 24998
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(36)]
		private int _9;

		// Token: 0x02000EB4 RID: 3764
		[CompilerGenerated]
		[UnsafeValueType]
		[PreserveInPlugin]
		[WeaverGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 40)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x040061A7 RID: 24999
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
