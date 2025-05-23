using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02000EA7 RID: 3751
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@Fusion_NetworkBool : IElementReaderWriter<NetworkBool>
	{
		// Token: 0x06005D82 RID: 23938 RVA: 0x001CCF59 File Offset: 0x001CB159
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe NetworkBool Read(byte* data, int index)
		{
			return *(NetworkBool*)(data + index * 4);
		}

		// Token: 0x06005D83 RID: 23939 RVA: 0x001CCEDC File Offset: 0x001CB0DC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref NetworkBool ReadRef(byte* data, int index)
		{
			return ref *(NetworkBool*)(data + index * 4);
		}

		// Token: 0x06005D84 RID: 23940 RVA: 0x001CCF69 File Offset: 0x001CB169
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, NetworkBool val)
		{
			*(NetworkBool*)(data + index * 4) = val;
		}

		// Token: 0x06005D85 RID: 23941 RVA: 0x00047642 File Offset: 0x00045842
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x06005D86 RID: 23942 RVA: 0x001CCF7C File Offset: 0x001CB17C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(NetworkBool val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06005D87 RID: 23943 RVA: 0x001CCF98 File Offset: 0x001CB198
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkBool> GetInstance()
		{
			if (ReaderWriter@Fusion_NetworkBool.Instance == null)
			{
				ReaderWriter@Fusion_NetworkBool.Instance = default(ReaderWriter@Fusion_NetworkBool);
			}
			return ReaderWriter@Fusion_NetworkBool.Instance;
		}

		// Token: 0x0400616F RID: 24943
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkBool> Instance;
	}
}
