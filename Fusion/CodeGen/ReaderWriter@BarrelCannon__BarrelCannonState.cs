using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02000EA5 RID: 3749
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@BarrelCannon__BarrelCannonState : IElementReaderWriter<BarrelCannon.BarrelCannonState>
	{
		// Token: 0x06005D79 RID: 23929 RVA: 0x001CCECC File Offset: 0x001CB0CC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe BarrelCannon.BarrelCannonState Read(byte* data, int index)
		{
			return *(BarrelCannon.BarrelCannonState*)(data + index * 4);
		}

		// Token: 0x06005D7A RID: 23930 RVA: 0x001CCEDC File Offset: 0x001CB0DC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref BarrelCannon.BarrelCannonState ReadRef(byte* data, int index)
		{
			return ref *(BarrelCannon.BarrelCannonState*)(data + index * 4);
		}

		// Token: 0x06005D7B RID: 23931 RVA: 0x001CCEE7 File Offset: 0x001CB0E7
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, BarrelCannon.BarrelCannonState val)
		{
			*(BarrelCannon.BarrelCannonState*)(data + index * 4) = val;
		}

		// Token: 0x06005D7C RID: 23932 RVA: 0x00047642 File Offset: 0x00045842
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x06005D7D RID: 23933 RVA: 0x001CCEF8 File Offset: 0x001CB0F8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(BarrelCannon.BarrelCannonState val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06005D7E RID: 23934 RVA: 0x001CCF14 File Offset: 0x001CB114
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<BarrelCannon.BarrelCannonState> GetInstance()
		{
			if (ReaderWriter@BarrelCannon__BarrelCannonState.Instance == null)
			{
				ReaderWriter@BarrelCannon__BarrelCannonState.Instance = default(ReaderWriter@BarrelCannon__BarrelCannonState);
			}
			return ReaderWriter@BarrelCannon__BarrelCannonState.Instance;
		}

		// Token: 0x0400616D RID: 24941
		[WeaverGenerated]
		public static IElementReaderWriter<BarrelCannon.BarrelCannonState> Instance;
	}
}
