using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CE5 RID: 3301
	public class RopeSwingManager : NetworkSceneObject
	{
		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x060051E9 RID: 20969 RVA: 0x0018DC63 File Offset: 0x0018BE63
		// (set) Token: 0x060051EA RID: 20970 RVA: 0x0018DC6A File Offset: 0x0018BE6A
		public static RopeSwingManager instance { get; private set; }

		// Token: 0x060051EB RID: 20971 RVA: 0x0018DC74 File Offset: 0x0018BE74
		private void Awake()
		{
			if (RopeSwingManager.instance != null && RopeSwingManager.instance != this)
			{
				GTDev.LogWarning<string>("Instance of RopeSwingManager already exists. Destroying.", null);
				global::UnityEngine.Object.Destroy(this);
				return;
			}
			if (RopeSwingManager.instance == null)
			{
				RopeSwingManager.instance = this;
			}
		}

		// Token: 0x060051EC RID: 20972 RVA: 0x0018DCC0 File Offset: 0x0018BEC0
		private void RegisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Add(t.ropeId, t);
		}

		// Token: 0x060051ED RID: 20973 RVA: 0x0018DCD4 File Offset: 0x0018BED4
		private void UnregisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Remove(t.ropeId);
		}

		// Token: 0x060051EE RID: 20974 RVA: 0x0018DCE8 File Offset: 0x0018BEE8
		public static void Register(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.RegisterInstance(t);
		}

		// Token: 0x060051EF RID: 20975 RVA: 0x0018DCF5 File Offset: 0x0018BEF5
		public static void Unregister(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.UnregisterInstance(t);
		}

		// Token: 0x060051F0 RID: 20976 RVA: 0x0018DD04 File Offset: 0x0018BF04
		public void SendSetVelocity_RPC(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				this.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { ropeId, boneIndex, velocity, wholeRope });
				return;
			}
			this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, default(PhotonMessageInfoWrapped));
		}

		// Token: 0x060051F1 RID: 20977 RVA: 0x0018DD6E File Offset: 0x0018BF6E
		public bool TryGetRope(int ropeId, out GorillaRopeSwing result)
		{
			return this.ropes.TryGetValue(ropeId, out result);
		}

		// Token: 0x060051F2 RID: 20978 RVA: 0x0018DD80 File Offset: 0x0018BF80
		[PunRPC]
		public void SetVelocity(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfo info)
		{
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
			this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, photonMessageInfoWrapped);
			Utils.Log("Receiving RPC for ropes");
		}

		// Token: 0x060051F3 RID: 20979 RVA: 0x0018DDAC File Offset: 0x0018BFAC
		[Rpc]
		public unsafe static void RPC_SetVelocity(NetworkRunner runner, int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage == SimulationStages.Resimulate)
				{
					return;
				}
				if (runner.HasAnyActiveConnections())
				{
					int num = 8;
					num += 4;
					num += 4;
					num += 12;
					num += 4;
					SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)")), data);
					*(int*)(data + num2) = ropeId;
					num2 += 4;
					*(int*)(data + num2) = boneIndex;
					num2 += 4;
					*(Vector3*)(data + num2) = velocity;
					num2 += 12;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), wholeRope);
					num2 += 4;
					ptr->Offset = num2 * 8;
					ptr->SetStatic();
					runner.SendRpc(ptr);
				}
				info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
			}
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
			RopeSwingManager.instance.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, photonMessageInfoWrapped);
		}

		// Token: 0x060051F4 RID: 20980 RVA: 0x0018DF24 File Offset: 0x0018C124
		private void SetVelocityShared(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfoWrapped info)
		{
			if (info.Sender != null)
			{
				GorillaNot.IncrementRPCCall(info, "SetVelocityShared");
			}
			GorillaRopeSwing gorillaRopeSwing;
			if (this.TryGetRope(ropeId, out gorillaRopeSwing) && gorillaRopeSwing != null)
			{
				gorillaRopeSwing.SetVelocity(boneIndex, velocity, wholeRope, info);
			}
		}

		// Token: 0x060051F6 RID: 20982 RVA: 0x0018DF7C File Offset: 0x0018C17C
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_SetVelocity@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			int num2 = *(int*)(data + num);
			num += 4;
			int num3 = num2;
			int num4 = *(int*)(data + num);
			num += 4;
			int num5 = num4;
			Vector3 vector = *(Vector3*)(data + num);
			num += 12;
			Vector3 vector2 = vector;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
			num += 4;
			bool flag2 = flag;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			RopeSwingManager.RPC_SetVelocity(runner, num3, num5, vector2, flag2, rpcInfo);
		}

		// Token: 0x04005618 RID: 22040
		private Dictionary<int, GorillaRopeSwing> ropes = new Dictionary<int, GorillaRopeSwing>();
	}
}
