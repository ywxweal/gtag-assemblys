using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02000EAF RID: 3759
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@UnityEngine_Vector3 : IElementReaderWriter<Vector3>
	{
		// Token: 0x06005D94 RID: 23956 RVA: 0x001CD071 File Offset: 0x001CB271
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe Vector3 Read(byte* data, int index)
		{
			return *(Vector3*)(data + index * 12);
		}

		// Token: 0x06005D95 RID: 23957 RVA: 0x001CD081 File Offset: 0x001CB281
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref Vector3 ReadRef(byte* data, int index)
		{
			return ref *(Vector3*)(data + index * 12);
		}

		// Token: 0x06005D96 RID: 23958 RVA: 0x001CD08C File Offset: 0x001CB28C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, Vector3 val)
		{
			*(Vector3*)(data + index * 12) = val;
		}

		// Token: 0x06005D97 RID: 23959 RVA: 0x000C23EE File Offset: 0x000C05EE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 3;
		}

		// Token: 0x06005D98 RID: 23960 RVA: 0x001CD0A0 File Offset: 0x001CB2A0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(Vector3 val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06005D99 RID: 23961 RVA: 0x001CD0BC File Offset: 0x001CB2BC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<Vector3> GetInstance()
		{
			if (ReaderWriter@UnityEngine_Vector3.Instance == null)
			{
				ReaderWriter@UnityEngine_Vector3.Instance = default(ReaderWriter@UnityEngine_Vector3);
			}
			return ReaderWriter@UnityEngine_Vector3.Instance;
		}

		// Token: 0x04006199 RID: 24985
		[WeaverGenerated]
		public static IElementReaderWriter<Vector3> Instance;
	}
}
