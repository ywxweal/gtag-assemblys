using System;
using System.Collections.Generic;
using Fusion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003DF RID: 991
public class VRRigReliableState : MonoBehaviour, IWrappedSerializable, INetworkStruct
{
	// Token: 0x170002AC RID: 684
	// (get) Token: 0x060017E5 RID: 6117 RVA: 0x000742EE File Offset: 0x000724EE
	public bool HasBracelet
	{
		get
		{
			return this.braceletBeadColors.Count > 0;
		}
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x060017E6 RID: 6118 RVA: 0x000742FE File Offset: 0x000724FE
	// (set) Token: 0x060017E7 RID: 6119 RVA: 0x00074306 File Offset: 0x00072506
	public bool isDirty { get; private set; } = true;

	// Token: 0x060017E8 RID: 6120 RVA: 0x00074310 File Offset: 0x00072510
	private void Awake()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Combine(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.SetIsDirty));
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x0007435D File Offset: 0x0007255D
	private void OnDestroy()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Remove(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
	}

	// Token: 0x060017EA RID: 6122 RVA: 0x0007437F File Offset: 0x0007257F
	public void SetIsDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x00074388 File Offset: 0x00072588
	public void SetIsNotDirty()
	{
		this.isDirty = false;
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x00074394 File Offset: 0x00072594
	public void SharedStart(bool isOfflineVRRig_, BodyDockPositions bDock_)
	{
		this.isOfflineVRRig = isOfflineVRRig_;
		this.bDock = bDock_;
		this.activeTransferrableObjectIndex = new int[5];
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			this.activeTransferrableObjectIndex[i] = -1;
		}
		this.transferrablePosStates = new TransferrableObject.PositionState[5];
		this.transferrableItemStates = new TransferrableObject.ItemStates[5];
		this.transferableDockPositions = new BodyDockPositions.DropPositions[5];
	}

	// Token: 0x060017ED RID: 6125 RVA: 0x000743FC File Offset: 0x000725FC
	void IWrappedSerializable.OnSerializeRead(object newData)
	{
		this.Data = (ReliableStateData)newData;
		long header = this.Data.Header;
		int num;
		this.SetHeader(header, out num);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & (1L << (i & 31))) != 0L)
			{
				long num2 = this.Data.TransferrableStates[i];
				this.activeTransferrableObjectIndex[i] = (int)num2;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)((num2 >> 32) & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)((num2 >> 40) & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)((num2 >> 48) & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = this.Data.WearablesPackedState;
		this.lThrowableProjectileIndex = this.Data.LThrowableProjectileIndex;
		this.rThrowableProjectileIndex = this.Data.RThrowableProjectileIndex;
		this.sizeLayerMask = this.Data.SizeLayerMask;
		this.randomThrowableIndex = this.Data.RandomThrowableIndex;
		this.braceletBeadColors.Clear();
		if (num > 0)
		{
			if (num <= 3)
			{
				int num3 = (int)this.Data.PackedBeads;
				this.braceletSelfIndex = num3 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num3, 0, num, this.braceletBeadColors);
			}
			else
			{
				long packedBeads = this.Data.PackedBeads;
				this.braceletSelfIndex = (int)(packedBeads >> 60);
				if (num <= 6)
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, num, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors(this.Data.PackedBeadsMoreThan6, 6, num, this.braceletBeadColors);
				}
			}
		}
		this.bDock.RefreshTransferrableItems();
		this.bDock.myRig.UpdateFriendshipBracelet();
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x000745D8 File Offset: 0x000727D8
	object IWrappedSerializable.OnSerializeWrite()
	{
		this.isDirty = false;
		ReliableStateData reliableStateData = default(ReliableStateData);
		long header = this.GetHeader();
		reliableStateData.Header = header;
		long[] array = this.GetTransferrableStates(header).ToArray();
		reliableStateData.TransferrableStates.CopyFrom(array, 0, array.Length);
		reliableStateData.WearablesPackedState = this.wearablesPackedStates;
		reliableStateData.LThrowableProjectileIndex = this.lThrowableProjectileIndex;
		reliableStateData.RThrowableProjectileIndex = this.rThrowableProjectileIndex;
		reliableStateData.SizeLayerMask = this.sizeLayerMask;
		reliableStateData.RandomThrowableIndex = this.randomThrowableIndex;
		if (this.braceletBeadColors.Count > 0)
		{
			long num = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num |= (long)this.braceletSelfIndex << 30;
				reliableStateData.PackedBeads = num;
			}
			else
			{
				num |= (long)this.braceletSelfIndex << 60;
				reliableStateData.PackedBeads = num;
				if (this.braceletBeadColors.Count > 6)
				{
					reliableStateData.PackedBeadsMoreThan6 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6);
				}
			}
		}
		this.Data = reliableStateData;
		return reliableStateData;
	}

	// Token: 0x060017EF RID: 6127 RVA: 0x000746F0 File Offset: 0x000728F0
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		long header = this.GetHeader();
		stream.SendNext(header);
		foreach (long num in this.GetTransferrableStates(header))
		{
			stream.SendNext(num);
		}
		stream.SendNext(this.wearablesPackedStates);
		stream.SendNext(this.lThrowableProjectileIndex);
		stream.SendNext(this.rThrowableProjectileIndex);
		stream.SendNext(this.sizeLayerMask);
		stream.SendNext(this.randomThrowableIndex);
		if (this.braceletBeadColors.Count > 0)
		{
			long num2 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num2 |= (long)this.braceletSelfIndex << 30;
				stream.SendNext((int)num2);
				return;
			}
			num2 |= (long)this.braceletSelfIndex << 60;
			stream.SendNext(num2);
			if (this.braceletBeadColors.Count > 6)
			{
				stream.SendNext(VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6));
			}
		}
	}

	// Token: 0x060017F0 RID: 6128 RVA: 0x00074844 File Offset: 0x00072A44
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		long num = (long)stream.ReceiveNext();
		this.isMicEnabled = (num & 32L) != 0L;
		this.isBraceletLeftHanded = (num & 64L) != 0L;
		this.isBuilderWatchEnabled = (num & 128L) != 0L;
		int num2 = (int)(num >> 12) & 15;
		this.lThrowableProjectileColor.r = (byte)(num >> 16);
		this.lThrowableProjectileColor.g = (byte)(num >> 24);
		this.lThrowableProjectileColor.b = (byte)(num >> 32);
		this.rThrowableProjectileColor.r = (byte)(num >> 40);
		this.rThrowableProjectileColor.g = (byte)(num >> 48);
		this.rThrowableProjectileColor.b = (byte)(num >> 56);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((num & (1L << (i & 31))) != 0L)
			{
				long num3 = (long)stream.ReceiveNext();
				this.activeTransferrableObjectIndex[i] = (int)num3;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)((num3 >> 32) & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)((num3 >> 40) & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)((num3 >> 48) & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = (int)stream.ReceiveNext();
		this.lThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.rThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.sizeLayerMask = (int)stream.ReceiveNext();
		this.randomThrowableIndex = (int)stream.ReceiveNext();
		this.braceletBeadColors.Clear();
		if (num2 > 0)
		{
			if (num2 <= 3)
			{
				int num4 = (int)stream.ReceiveNext();
				this.braceletSelfIndex = num4 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num4, 0, num2, this.braceletBeadColors);
			}
			else
			{
				long num5 = (long)stream.ReceiveNext();
				this.braceletSelfIndex = (int)(num5 >> 60);
				if (num2 <= 6)
				{
					VRRigReliableState.UnpackBeadColors(num5, 0, num2, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(num5, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors((long)stream.ReceiveNext(), 6, num2, this.braceletBeadColors);
				}
			}
		}
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			this.bDock.RefreshTransferrableItems();
		}
		this.bDock.myRig.UpdateFriendshipBracelet();
		this.bDock.myRig.EnableBuilderResizeWatch(this.isBuilderWatchEnabled);
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x00074AB4 File Offset: 0x00072CB4
	private long GetHeader()
	{
		long num = 0L;
		if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
			{
				if (this.activeTransferrableObjectIndex[i] != -1 && (this.transferrablePosStates[i] == TransferrableObject.PositionState.InLeftHand || this.transferrablePosStates[i] == TransferrableObject.PositionState.InRightHand))
				{
					num |= (long)((ulong)((byte)(1 << i)));
				}
			}
		}
		else
		{
			for (int j = 0; j < this.activeTransferrableObjectIndex.Length; j++)
			{
				if (this.activeTransferrableObjectIndex[j] != -1)
				{
					num |= (long)((ulong)((byte)(1 << j)));
				}
			}
		}
		if (this.isBraceletLeftHanded)
		{
			num |= 64L;
		}
		if (this.isMicEnabled)
		{
			num |= 32L;
		}
		if (this.isBuilderWatchEnabled && !CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			num |= 128L;
		}
		num |= ((long)this.braceletBeadColors.Count & 15L) << 12;
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.r) << 16);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.g) << 24);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.b) << 32);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.r) << 40);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.g) << 48);
		return num | (long)((long)((ulong)this.rThrowableProjectileColor.b) << 56);
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x00074BFC File Offset: 0x00072DFC
	private void SetHeader(long header, out int numBeadsToRead)
	{
		this.isMicEnabled = (header & 32L) != 0L;
		this.isBraceletLeftHanded = (header & 64L) != 0L;
		numBeadsToRead = (int)(header >> 12) & 15;
		this.lThrowableProjectileColor.r = (byte)(header >> 16);
		this.lThrowableProjectileColor.g = (byte)(header >> 24);
		this.lThrowableProjectileColor.b = (byte)(header >> 32);
		this.rThrowableProjectileColor.r = (byte)(header >> 40);
		this.rThrowableProjectileColor.g = (byte)(header >> 48);
		this.rThrowableProjectileColor.b = (byte)(header >> 56);
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x00074C94 File Offset: 0x00072E94
	private List<long> GetTransferrableStates(long header)
	{
		List<long> list = new List<long>();
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & (1L << (i & 31))) != 0L && this.activeTransferrableObjectIndex[i] != -1)
			{
				long num = (long)((ulong)this.activeTransferrableObjectIndex[i]);
				num |= (long)this.transferrablePosStates[i] << 32;
				num |= (long)this.transferrableItemStates[i] << 40;
				num |= (long)this.transferableDockPositions[i] << 48;
				list.Add(num);
			}
		}
		return list;
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x00074D10 File Offset: 0x00072F10
	private static long PackBeadColors(List<Color> beadColors, int fromIndex)
	{
		long num = 0L;
		int num2 = Mathf.Min(fromIndex + 6, beadColors.Count);
		int num3 = 0;
		for (int i = fromIndex; i < num2; i++)
		{
			long num4 = (long)FriendshipGroupDetection.PackColor(beadColors[i]);
			num |= num4 << num3;
			num3 += 10;
		}
		return num;
	}

	// Token: 0x060017F5 RID: 6133 RVA: 0x00074D5C File Offset: 0x00072F5C
	private static void UnpackBeadColors(long packed, int startIndex, int endIndex, List<Color> beadColorsResult)
	{
		int num = Mathf.Min(startIndex + 6, endIndex);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			short num3 = (short)((packed >> num2) & 1023L);
			beadColorsResult.Add(FriendshipGroupDetection.UnpackColor(num3));
			num2 += 10;
		}
	}

	// Token: 0x04001AB6 RID: 6838
	[NonSerialized]
	public int[] activeTransferrableObjectIndex;

	// Token: 0x04001AB7 RID: 6839
	[NonSerialized]
	public TransferrableObject.PositionState[] transferrablePosStates;

	// Token: 0x04001AB8 RID: 6840
	[NonSerialized]
	public TransferrableObject.ItemStates[] transferrableItemStates;

	// Token: 0x04001AB9 RID: 6841
	[NonSerialized]
	public BodyDockPositions.DropPositions[] transferableDockPositions;

	// Token: 0x04001ABA RID: 6842
	[NonSerialized]
	public int wearablesPackedStates;

	// Token: 0x04001ABB RID: 6843
	[NonSerialized]
	public int lThrowableProjectileIndex = -1;

	// Token: 0x04001ABC RID: 6844
	[NonSerialized]
	public int rThrowableProjectileIndex = -1;

	// Token: 0x04001ABD RID: 6845
	[NonSerialized]
	public Color32 lThrowableProjectileColor = Color.white;

	// Token: 0x04001ABE RID: 6846
	[NonSerialized]
	public Color32 rThrowableProjectileColor = Color.white;

	// Token: 0x04001ABF RID: 6847
	[NonSerialized]
	public int randomThrowableIndex;

	// Token: 0x04001AC0 RID: 6848
	[NonSerialized]
	public bool isMicEnabled;

	// Token: 0x04001AC1 RID: 6849
	private bool isOfflineVRRig;

	// Token: 0x04001AC2 RID: 6850
	private BodyDockPositions bDock;

	// Token: 0x04001AC3 RID: 6851
	[NonSerialized]
	public int sizeLayerMask = 1;

	// Token: 0x04001AC4 RID: 6852
	private const long IS_MIC_ENABLED_BIT = 32L;

	// Token: 0x04001AC5 RID: 6853
	private const long BRACELET_LEFTHAND_BIT = 64L;

	// Token: 0x04001AC6 RID: 6854
	private const long BUILDER_WATCH_ENABLED_BIT = 128L;

	// Token: 0x04001AC7 RID: 6855
	private const int BRACELET_NUM_BEADS_SHIFT = 12;

	// Token: 0x04001AC8 RID: 6856
	private const int LPROJECTILECOLOR_R_SHIFT = 16;

	// Token: 0x04001AC9 RID: 6857
	private const int LPROJECTILECOLOR_G_SHIFT = 24;

	// Token: 0x04001ACA RID: 6858
	private const int LPROJECTILECOLOR_B_SHIFT = 32;

	// Token: 0x04001ACB RID: 6859
	private const int RPROJECTILECOLOR_R_SHIFT = 40;

	// Token: 0x04001ACC RID: 6860
	private const int RPROJECTILECOLOR_G_SHIFT = 48;

	// Token: 0x04001ACD RID: 6861
	private const int RPROJECTILECOLOR_B_SHIFT = 56;

	// Token: 0x04001ACE RID: 6862
	private const int POS_STATES_SHIFT = 32;

	// Token: 0x04001ACF RID: 6863
	private const int ITEM_STATES_SHIFT = 40;

	// Token: 0x04001AD0 RID: 6864
	private const int DOCK_POSITIONS_SHIFT = 48;

	// Token: 0x04001AD1 RID: 6865
	private const int BRACELET_SELF_INDEX_SHIFT = 60;

	// Token: 0x04001AD2 RID: 6866
	[NonSerialized]
	public bool isBraceletLeftHanded;

	// Token: 0x04001AD3 RID: 6867
	[NonSerialized]
	public int braceletSelfIndex;

	// Token: 0x04001AD4 RID: 6868
	[NonSerialized]
	public List<Color> braceletBeadColors = new List<Color>(10);

	// Token: 0x04001AD5 RID: 6869
	[NonSerialized]
	public bool isBuilderWatchEnabled;

	// Token: 0x04001AD7 RID: 6871
	private ReliableStateData Data;
}
