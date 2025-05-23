using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02000EB1 RID: 3761
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Single : IElementReaderWriter<float>
	{
		// Token: 0x06005D9D RID: 23965 RVA: 0x001CD101 File Offset: 0x001CB301
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe float Read(byte* data, int index)
		{
			return *(float*)(data + index * 4);
		}

		// Token: 0x06005D9E RID: 23966 RVA: 0x001CCEDC File Offset: 0x001CB0DC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref float ReadRef(byte* data, int index)
		{
			return ref *(float*)(data + index * 4);
		}

		// Token: 0x06005D9F RID: 23967 RVA: 0x001CD10D File Offset: 0x001CB30D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, float val)
		{
			*(float*)(data + index * 4) = val;
		}

		// Token: 0x06005DA0 RID: 23968 RVA: 0x00047642 File Offset: 0x00045842
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x06005DA1 RID: 23969 RVA: 0x001CD11C File Offset: 0x001CB31C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(float val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06005DA2 RID: 23970 RVA: 0x001CD130 File Offset: 0x001CB330
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<float> GetInstance()
		{
			if (ReaderWriter@System_Single.Instance == null)
			{
				ReaderWriter@System_Single.Instance = default(ReaderWriter@System_Single);
			}
			return ReaderWriter@System_Single.Instance;
		}

		// Token: 0x0400619B RID: 24987
		[WeaverGenerated]
		public static IElementReaderWriter<float> Instance;
	}
}
