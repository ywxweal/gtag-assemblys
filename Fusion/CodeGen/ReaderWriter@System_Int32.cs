using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02000EBA RID: 3770
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Int32 : IElementReaderWriter<int>
	{
		// Token: 0x06005DB2 RID: 23986 RVA: 0x001CD229 File Offset: 0x001CB429
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe int Read(byte* data, int index)
		{
			return *(int*)(data + index * 4);
		}

		// Token: 0x06005DB3 RID: 23987 RVA: 0x001CCEDC File Offset: 0x001CB0DC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref int ReadRef(byte* data, int index)
		{
			return ref *(int*)(data + index * 4);
		}

		// Token: 0x06005DB4 RID: 23988 RVA: 0x001CD235 File Offset: 0x001CB435
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, int val)
		{
			*(int*)(data + index * 4) = val;
		}

		// Token: 0x06005DB5 RID: 23989 RVA: 0x00047642 File Offset: 0x00045842
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x06005DB6 RID: 23990 RVA: 0x001CD244 File Offset: 0x001CB444
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(int val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06005DB7 RID: 23991 RVA: 0x001CD258 File Offset: 0x001CB458
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<int> GetInstance()
		{
			if (ReaderWriter@System_Int32.Instance == null)
			{
				ReaderWriter@System_Int32.Instance = default(ReaderWriter@System_Int32);
			}
			return ReaderWriter@System_Int32.Instance;
		}

		// Token: 0x040061B0 RID: 25008
		[WeaverGenerated]
		public static IElementReaderWriter<int> Instance;
	}
}
