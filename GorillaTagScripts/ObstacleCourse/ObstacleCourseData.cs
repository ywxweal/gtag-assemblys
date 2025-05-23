using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B46 RID: 2886
	[NetworkStructWeaved(9)]
	[StructLayout(LayoutKind.Explicit, Size = 36)]
	public struct ObstacleCourseData : INetworkStruct
	{
		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x0600473B RID: 18235 RVA: 0x00152EAD File Offset: 0x001510AD
		// (set) Token: 0x0600473C RID: 18236 RVA: 0x00152EB5 File Offset: 0x001510B5
		public int ObstacleCourseCount { readonly get; set; }

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x0600473D RID: 18237 RVA: 0x00152EC0 File Offset: 0x001510C0
		[Networked]
		[Capacity(4)]
		public NetworkArray<int> WinnerActorNumber
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._WinnerActorNumber), 4, ReaderWriter@System_Int32.GetInstance());
			}
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x0600473E RID: 18238 RVA: 0x00152EE4 File Offset: 0x001510E4
		[Networked]
		[Capacity(4)]
		public NetworkArray<int> CurrentRaceState
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._CurrentRaceState), 4, ReaderWriter@System_Int32.GetInstance());
			}
		}

		// Token: 0x0600473F RID: 18239 RVA: 0x00152F08 File Offset: 0x00151108
		public ObstacleCourseData(List<ObstacleCourse> courses)
		{
			this.ObstacleCourseCount = courses.Count;
			int[] array = new int[this.ObstacleCourseCount];
			int[] array2 = new int[this.ObstacleCourseCount];
			for (int i = 0; i < courses.Count; i++)
			{
				array[i] = courses[i].winnerActorNumber;
				array2[i] = (int)courses[i].currentState;
			}
			this.WinnerActorNumber.CopyFrom(array, 0, this.ObstacleCourseCount);
			this.CurrentRaceState.CopyFrom(array2, 0, this.ObstacleCourseCount);
		}

		// Token: 0x040049A3 RID: 18851
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@4 _WinnerActorNumber;

		// Token: 0x040049A4 RID: 18852
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(20)]
		private FixedStorage@4 _CurrentRaceState;
	}
}
