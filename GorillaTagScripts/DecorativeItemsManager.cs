using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000B03 RID: 2819
	[NetworkBehaviourWeaved(1)]
	public class DecorativeItemsManager : NetworkComponent
	{
		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x060044F5 RID: 17653 RVA: 0x00146914 File Offset: 0x00144B14
		public static DecorativeItemsManager Instance
		{
			get
			{
				return DecorativeItemsManager._instance;
			}
		}

		// Token: 0x060044F6 RID: 17654 RVA: 0x0014691C File Offset: 0x00144B1C
		protected override void Awake()
		{
			base.Awake();
			if (DecorativeItemsManager._instance != null && DecorativeItemsManager._instance != this)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				DecorativeItemsManager._instance = this;
			}
			this.currentIndex = -1;
			this.shouldRunUpdate = true;
			this.zone = base.GetComponent<ZoneBasedObject>();
			foreach (DecorativeItem decorativeItem in this.decorativeItemsContainer.GetComponentsInChildren<DecorativeItem>(false))
			{
				if (decorativeItem)
				{
					this.itemsList.Add(decorativeItem);
					DecorativeItem decorativeItem2 = decorativeItem;
					decorativeItem2.respawnItem = (UnityAction<DecorativeItem>)Delegate.Combine(decorativeItem2.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
				}
			}
			foreach (AttachPoint attachPoint in this.respawnableHooksContainer.GetComponentsInChildren<AttachPoint>(false))
			{
				if (attachPoint)
				{
					this.respawnableHooks.Add(attachPoint);
				}
			}
			this.allHooks.AddRange(this.respawnableHooks);
			foreach (GameObject gameObject in this.nonRespawnableHooksContainer)
			{
				foreach (AttachPoint attachPoint2 in gameObject.GetComponentsInChildren<AttachPoint>(false))
				{
					if (attachPoint2)
					{
						this.allHooks.Add(attachPoint2);
					}
				}
			}
		}

		// Token: 0x060044F7 RID: 17655 RVA: 0x00146A84 File Offset: 0x00144C84
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (DecorativeItem decorativeItem in this.itemsList)
			{
				decorativeItem.respawnItem = (UnityAction<DecorativeItem>)Delegate.Remove(decorativeItem.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
			}
			this.itemsList.Clear();
			this.respawnableHooks.Clear();
			if (DecorativeItemsManager._instance == this)
			{
				DecorativeItemsManager._instance = null;
			}
		}

		// Token: 0x060044F8 RID: 17656 RVA: 0x00146B20 File Offset: 0x00144D20
		private void Update()
		{
			if (!PhotonNetwork.InRoom)
			{
				return;
			}
			if (this.wasInZone != this.zone.IsLocalPlayerInZone())
			{
				this.shouldRunUpdate = true;
			}
			if (!this.shouldRunUpdate)
			{
				return;
			}
			if (base.IsMine)
			{
				if (this.wasInZone != this.zone.IsLocalPlayerInZone())
				{
					foreach (AttachPoint attachPoint in this.allHooks)
					{
						attachPoint.SetIsHook(false);
					}
					for (int i = 0; i < this.itemsList.Count; i++)
					{
						this.itemsList[i].itemState = TransferrableObject.ItemStates.State2;
						this.SpawnItem(i);
					}
					this.shouldRunUpdate = false;
				}
				this.wasInZone = this.zone.IsLocalPlayerInZone();
				this.SpawnItem(this.UpdateListPerFrame());
			}
		}

		// Token: 0x060044F9 RID: 17657 RVA: 0x00146C10 File Offset: 0x00144E10
		private void SpawnItem(int index)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			if (index < 0 || index >= this.itemsList.Count)
			{
				return;
			}
			if (this.respawnableHooks == null)
			{
				return;
			}
			if (this.itemsList == null)
			{
				return;
			}
			if (this.itemsList.Count > this.respawnableHooks.Count)
			{
				Debug.LogError("Trying to snap more decorative items than allowed! Some items will be left un-hooked!");
				return;
			}
			Transform transform = this.RandomSpawn();
			if (transform == null)
			{
				return;
			}
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			DecorativeItem decorativeItem = this.itemsList[index];
			decorativeItem.WorldShareableRequestOwnership();
			decorativeItem.Respawn(position, rotation);
			base.SendRPC("RespawnItemRPC", RpcTarget.Others, new object[] { index, position, rotation });
		}

		// Token: 0x060044FA RID: 17658 RVA: 0x00146CD7 File Offset: 0x00144ED7
		[PunRPC]
		private void RespawnItemRPC(int index, Vector3 _transformPos, Quaternion _transformRot, PhotonMessageInfo info)
		{
			this.RespawnItemShared(index, _transformPos, _transformRot, info);
		}

		// Token: 0x060044FB RID: 17659 RVA: 0x00146CEC File Offset: 0x00144EEC
		[Rpc]
		private unsafe void RPC_RespawnItem(int index, Vector3 _transformPos, Quaternion _transformRot, RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) == 0)
					{
						NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTagScripts.DecorativeItemsManager::RPC_RespawnItem(System.Int32,UnityEngine.Vector3,UnityEngine.Quaternion,Fusion.RpcInfo)", base.Object, 7);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							int num = 8;
							num += 4;
							num += 12;
							num += 16;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
							*(int*)(data + num2) = index;
							num2 += 4;
							*(Vector3*)(data + num2) = _transformPos;
							num2 += 12;
							*(Quaternion*)(data + num2) = _transformRot;
							num2 += 16;
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0012;
						}
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_0012:
			this.RespawnItemShared(index, _transformPos, _transformRot, info);
		}

		// Token: 0x060044FC RID: 17660 RVA: 0x00146E8C File Offset: 0x0014508C
		protected void RespawnItemShared(int index, Vector3 _transformPos, Quaternion _transformRot, PhotonMessageInfoWrapped info)
		{
			if (index >= 0 && index <= this.itemsList.Count - 1)
			{
				float num = 10000f;
				if ((in _transformPos).IsValid(in num) && (in _transformRot).IsValid() && info.Sender == NetworkSystem.Instance.MasterClient)
				{
					GorillaNot.IncrementRPCCall(info, "RespawnItemRPC");
					this.itemsList[index].Respawn(_transformPos, _transformRot);
					return;
				}
			}
		}

		// Token: 0x060044FD RID: 17661 RVA: 0x00146EFC File Offset: 0x001450FC
		private Transform RandomSpawn()
		{
			this.lastIndex = this.currentIndex;
			bool flag = false;
			bool flag2 = this.zone.IsLocalPlayerInZone();
			int num = Random.Range(0, this.respawnableHooks.Count);
			while (!flag)
			{
				num = Random.Range(0, this.respawnableHooks.Count);
				if (!this.respawnableHooks[num].inForest == flag2)
				{
					flag = true;
				}
			}
			if (!this.respawnableHooks[num].IsHooked())
			{
				this.currentIndex = num;
			}
			else
			{
				this.currentIndex = -1;
			}
			if (this.currentIndex != this.lastIndex && this.currentIndex > -1)
			{
				return this.respawnableHooks[this.currentIndex].attachPoint;
			}
			this.currentIndex = -1;
			return null;
		}

		// Token: 0x060044FE RID: 17662 RVA: 0x00146FBE File Offset: 0x001451BE
		private int UpdateListPerFrame()
		{
			this.arrayIndex++;
			if (this.arrayIndex >= this.itemsList.Count || this.arrayIndex < 0)
			{
				this.shouldRunUpdate = false;
				return -1;
			}
			return this.arrayIndex;
		}

		// Token: 0x060044FF RID: 17663 RVA: 0x00146FFC File Offset: 0x001451FC
		private void OnRequestToRespawn(DecorativeItem item)
		{
			if (base.IsMine)
			{
				if (item == null)
				{
					return;
				}
				int num = this.itemsList.IndexOf(item);
				this.SpawnItem(num);
			}
		}

		// Token: 0x06004500 RID: 17664 RVA: 0x00147030 File Offset: 0x00145230
		public AttachPoint getCurrentAttachPointByPosition(Vector3 _attachPoint)
		{
			foreach (AttachPoint attachPoint in this.allHooks)
			{
				if (attachPoint.attachPoint.position == _attachPoint)
				{
					return attachPoint;
				}
			}
			return null;
		}

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x06004501 RID: 17665 RVA: 0x00147098 File Offset: 0x00145298
		// (set) Token: 0x06004502 RID: 17666 RVA: 0x001470BE File Offset: 0x001452BE
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe int Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing DecorativeItemsManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return this.Ptr[0];
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing DecorativeItemsManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				this.Ptr[0] = value;
			}
		}

		// Token: 0x06004503 RID: 17667 RVA: 0x001470E5 File Offset: 0x001452E5
		public override void WriteDataFusion()
		{
			this.Data = this.currentIndex;
		}

		// Token: 0x06004504 RID: 17668 RVA: 0x001470F3 File Offset: 0x001452F3
		public override void ReadDataFusion()
		{
			this.currentIndex = this.Data;
		}

		// Token: 0x06004505 RID: 17669 RVA: 0x00147101 File Offset: 0x00145301
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentIndex);
		}

		// Token: 0x06004506 RID: 17670 RVA: 0x00147122 File Offset: 0x00145322
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			this.currentIndex = (int)stream.ReceiveNext();
		}

		// Token: 0x06004508 RID: 17672 RVA: 0x0014717E File Offset: 0x0014537E
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06004509 RID: 17673 RVA: 0x00147196 File Offset: 0x00145396
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0600450A RID: 17674 RVA: 0x001471AC File Offset: 0x001453AC
		[NetworkRpcWeavedInvoker(1, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_RespawnItem@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			int num2 = *(int*)(data + num);
			num += 4;
			int num3 = num2;
			Vector3 vector = *(Vector3*)(data + num);
			num += 12;
			Vector3 vector2 = vector;
			Quaternion quaternion = *(Quaternion*)(data + num);
			num += 16;
			Quaternion quaternion2 = quaternion;
			RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((DecorativeItemsManager)behaviour).RPC_RespawnItem(num3, vector2, quaternion2, rpcInfo);
		}

		// Token: 0x040047B3 RID: 18355
		public GameObject decorativeItemsContainer;

		// Token: 0x040047B4 RID: 18356
		public GameObject respawnableHooksContainer;

		// Token: 0x040047B5 RID: 18357
		public List<GameObject> nonRespawnableHooksContainer = new List<GameObject>();

		// Token: 0x040047B6 RID: 18358
		private readonly List<DecorativeItem> itemsList = new List<DecorativeItem>();

		// Token: 0x040047B7 RID: 18359
		private readonly List<AttachPoint> respawnableHooks = new List<AttachPoint>();

		// Token: 0x040047B8 RID: 18360
		private readonly List<AttachPoint> allHooks = new List<AttachPoint>();

		// Token: 0x040047B9 RID: 18361
		private int lastIndex;

		// Token: 0x040047BA RID: 18362
		private int currentIndex;

		// Token: 0x040047BB RID: 18363
		private int arrayIndex = -1;

		// Token: 0x040047BC RID: 18364
		private bool shouldRunUpdate;

		// Token: 0x040047BD RID: 18365
		private ZoneBasedObject zone;

		// Token: 0x040047BE RID: 18366
		private bool wasInZone;

		// Token: 0x040047BF RID: 18367
		[OnEnterPlay_SetNull]
		private static DecorativeItemsManager _instance;

		// Token: 0x040047C0 RID: 18368
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int _Data;
	}
}
