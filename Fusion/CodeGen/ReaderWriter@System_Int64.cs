using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02000EBF RID: 3775
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Int64 : IElementReaderWriter<long>
	{
		// Token: 0x06005DBE RID: 23998 RVA: 0x001CD2C1 File Offset: 0x001CB4C1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe long Read(byte* data, int index)
		{
			return *(long*)(data + index * 8);
		}

		// Token: 0x06005DBF RID: 23999 RVA: 0x001CD2CD File Offset: 0x001CB4CD
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref long ReadRef(byte* data, int index)
		{
			return ref *(long*)(data + index * 8);
		}

		// Token: 0x06005DC0 RID: 24000 RVA: 0x001CD2D8 File Offset: 0x001CB4D8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, long val)
		{
			*(long*)(data + index * 8) = val;
		}

		// Token: 0x06005DC1 RID: 24001 RVA: 0x000121FB File Offset: 0x000103FB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 2;
		}

		// Token: 0x06005DC2 RID: 24002 RVA: 0x001CD2E8 File Offset: 0x001CB4E8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(long val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06005DC3 RID: 24003 RVA: 0x001CD2FC File Offset: 0x001CB4FC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<long> GetInstance()
		{
			if (ReaderWriter@System_Int64.Instance == null)
			{
				ReaderWriter@System_Int64.Instance = default(ReaderWriter@System_Int64);
			}
			return ReaderWriter@System_Int64.Instance;
		}

		// Token: 0x04006283 RID: 25219
		[WeaverGenerated]
		public static IElementReaderWriter<long> Instance;
	}
}
