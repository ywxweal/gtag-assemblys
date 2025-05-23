using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B09 RID: 2825
	[NetworkStructWeaved(13)]
	[StructLayout(LayoutKind.Explicit, Size = 52)]
	public struct FlowersDataStruct : INetworkStruct
	{
		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x06004533 RID: 17715 RVA: 0x00147DEB File Offset: 0x00145FEB
		// (set) Token: 0x06004534 RID: 17716 RVA: 0x00147DF3 File Offset: 0x00145FF3
		public int FlowerCount { readonly get; set; }

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x06004535 RID: 17717 RVA: 0x00147DFC File Offset: 0x00145FFC
		[Networked]
		public NetworkLinkedList<byte> FlowerWateredData
		{
			get
			{
				return new NetworkLinkedList<byte>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerWateredData), 1, ReaderWriter@System_Byte.GetInstance());
			}
		}

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x06004536 RID: 17718 RVA: 0x00147E20 File Offset: 0x00146020
		[Networked]
		public NetworkLinkedList<int> FlowerStateData
		{
			get
			{
				return new NetworkLinkedList<int>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerStateData), 1, ReaderWriter@System_Int32.GetInstance());
			}
		}

		// Token: 0x06004537 RID: 17719 RVA: 0x00147E44 File Offset: 0x00146044
		public FlowersDataStruct(List<Flower> allFlowers)
		{
			this.FlowerCount = allFlowers.Count;
			foreach (Flower flower in allFlowers)
			{
				this.FlowerWateredData.Add(flower.IsWatered ? 1 : 0);
				this.FlowerStateData.Add((int)flower.GetCurrentState());
			}
		}

		// Token: 0x040047E6 RID: 18406
		[FixedBufferProperty(typeof(NetworkLinkedList<byte>), typeof(UnityLinkedListSurrogate@ReaderWriter@System_Byte), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@6 _FlowerWateredData;

		// Token: 0x040047E7 RID: 18407
		[FixedBufferProperty(typeof(NetworkLinkedList<int>), typeof(UnityLinkedListSurrogate@ReaderWriter@System_Int32), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(28)]
		private FixedStorage@6 _FlowerStateData;
	}
}
