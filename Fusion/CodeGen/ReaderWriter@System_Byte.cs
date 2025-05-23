using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02000ED3 RID: 3795
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Byte : IElementReaderWriter<byte>
	{
		// Token: 0x06005DE6 RID: 24038 RVA: 0x001CD5D9 File Offset: 0x001CB7D9
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe byte Read(byte* data, int index)
		{
			return data[index * 4];
		}

		// Token: 0x06005DE7 RID: 24039 RVA: 0x001CCFB4 File Offset: 0x001CB1B4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref byte ReadRef(byte* data, int index)
		{
			return ref data[index * 4];
		}

		// Token: 0x06005DE8 RID: 24040 RVA: 0x001CD5E5 File Offset: 0x001CB7E5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, byte val)
		{
			data[index * 4] = val;
		}

		// Token: 0x06005DE9 RID: 24041 RVA: 0x00047642 File Offset: 0x00045842
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x06005DEA RID: 24042 RVA: 0x001CD5F4 File Offset: 0x001CB7F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(byte val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06005DEB RID: 24043 RVA: 0x001CD608 File Offset: 0x001CB808
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<byte> GetInstance()
		{
			if (ReaderWriter@System_Byte.Instance == null)
			{
				ReaderWriter@System_Byte.Instance = default(ReaderWriter@System_Byte);
			}
			return ReaderWriter@System_Byte.Instance;
		}

		// Token: 0x040064B1 RID: 25777
		[WeaverGenerated]
		public static IElementReaderWriter<byte> Instance;
	}
}
