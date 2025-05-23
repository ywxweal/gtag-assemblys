using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02000ED6 RID: 3798
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Boolean : IElementReaderWriter<bool>
	{
		// Token: 0x06005DF1 RID: 24049 RVA: 0x001CD5A4 File Offset: 0x001CB7A4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe bool Read(byte* data, int index)
		{
			return ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + index * 4));
		}

		// Token: 0x06005DF2 RID: 24050 RVA: 0x001CD5B4 File Offset: 0x001CB7B4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref bool ReadRef(byte* data, int index)
		{
			throw new NotSupportedException("Only supported for trivially copyable types. System.Boolean is not trivially copyable.");
		}

		// Token: 0x06005DF3 RID: 24051 RVA: 0x001CD5C0 File Offset: 0x001CB7C0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, bool val)
		{
			ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + index * 4), val);
		}

		// Token: 0x06005DF4 RID: 24052 RVA: 0x00047642 File Offset: 0x00045842
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x06005DF5 RID: 24053 RVA: 0x001CD5D4 File Offset: 0x001CB7D4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(bool val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06005DF6 RID: 24054 RVA: 0x001CD5E8 File Offset: 0x001CB7E8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<bool> GetInstance()
		{
			if (ReaderWriter@System_Boolean.Instance == null)
			{
				ReaderWriter@System_Boolean.Instance = default(ReaderWriter@System_Boolean);
			}
			return ReaderWriter@System_Boolean.Instance;
		}

		// Token: 0x040064B3 RID: 25779
		[WeaverGenerated]
		public static IElementReaderWriter<bool> Instance;
	}
}
