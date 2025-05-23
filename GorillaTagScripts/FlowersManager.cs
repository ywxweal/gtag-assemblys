using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B07 RID: 2823
	[NetworkBehaviourWeaved(13)]
	public class FlowersManager : NetworkComponent
	{
		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x0600451E RID: 17694 RVA: 0x00147619 File Offset: 0x00145819
		// (set) Token: 0x0600451F RID: 17695 RVA: 0x00147620 File Offset: 0x00145820
		public static FlowersManager Instance { get; private set; }

		// Token: 0x06004520 RID: 17696 RVA: 0x00147628 File Offset: 0x00145828
		protected override void Awake()
		{
			base.Awake();
			FlowersManager.Instance = this;
			this.hitNotifiers = base.GetComponentsInChildren<SlingshotProjectileHitNotifier>();
			foreach (SlingshotProjectileHitNotifier slingshotProjectileHitNotifier in this.hitNotifiers)
			{
				if (slingshotProjectileHitNotifier != null)
				{
					slingshotProjectileHitNotifier.OnProjectileTriggerEnter += this.ProjectileHitReceiver;
				}
				else
				{
					Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject children");
				}
			}
			foreach (FlowersManager.FlowersInZone flowersInZone in this.sections)
			{
				foreach (GameObject gameObject in flowersInZone.sections)
				{
					this.sectionToZonesDict[gameObject] = flowersInZone.zone;
					Flower[] componentsInChildren = gameObject.GetComponentsInChildren<Flower>();
					this.allFlowers.AddRange(componentsInChildren);
					this.sectionToFlowersDict[gameObject] = componentsInChildren.ToList<Flower>();
				}
			}
		}

		// Token: 0x06004521 RID: 17697 RVA: 0x0014774C File Offset: 0x0014594C
		private new void Start()
		{
			NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			if (base.IsMine)
			{
				foreach (Flower flower in this.allFlowers)
				{
					flower.UpdateFlowerState(Flower.FlowerState.Healthy, false, false);
				}
			}
		}

		// Token: 0x06004522 RID: 17698 RVA: 0x001477E0 File Offset: 0x001459E0
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (SlingshotProjectileHitNotifier slingshotProjectileHitNotifier in this.hitNotifiers)
			{
				if (slingshotProjectileHitNotifier != null)
				{
					slingshotProjectileHitNotifier.OnProjectileTriggerEnter -= this.ProjectileHitReceiver;
				}
			}
			FlowersManager.Instance = null;
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
		}

		// Token: 0x06004523 RID: 17699 RVA: 0x00147853 File Offset: 0x00145A53
		private void ProjectileHitReceiver(SlingshotProjectile projectile, Collider collider)
		{
			if (!projectile.CompareTag("WaterBalloonProjectile"))
			{
				return;
			}
			this.WaterFlowers(collider);
		}

		// Token: 0x06004524 RID: 17700 RVA: 0x0014786C File Offset: 0x00145A6C
		private void WaterFlowers(Collider collider)
		{
			if (!base.IsMine)
			{
				return;
			}
			GameObject gameObject = collider.gameObject;
			if (gameObject == null)
			{
				Debug.LogError("Could not find any flowers section");
				return;
			}
			foreach (Flower flower in this.sectionToFlowersDict[gameObject])
			{
				flower.WaterFlower(true);
			}
		}

		// Token: 0x06004525 RID: 17701 RVA: 0x001478E8 File Offset: 0x00145AE8
		private void HandleOnZoneChanged()
		{
			foreach (KeyValuePair<GameObject, GTZone> keyValuePair in this.sectionToZonesDict)
			{
				bool flag = ZoneManagement.instance.IsZoneActive(keyValuePair.Value);
				foreach (Flower flower in this.sectionToFlowersDict[keyValuePair.Key])
				{
					flower.UpdateVisuals(flag);
				}
			}
		}

		// Token: 0x06004526 RID: 17702 RVA: 0x00147994 File Offset: 0x00145B94
		public int GetHealthyFlowersInZoneCount(GTZone zone)
		{
			int num = 0;
			foreach (KeyValuePair<GameObject, GTZone> keyValuePair in this.sectionToZonesDict)
			{
				if (keyValuePair.Value == zone)
				{
					using (List<Flower>.Enumerator enumerator2 = this.sectionToFlowersDict[keyValuePair.Key].GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.GetCurrentState() == Flower.FlowerState.Healthy)
							{
								num++;
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06004527 RID: 17703 RVA: 0x00147A40 File Offset: 0x00145C40
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.allFlowers.Count);
			for (int i = 0; i < this.allFlowers.Count; i++)
			{
				stream.SendNext(this.allFlowers[i].IsWatered);
				stream.SendNext(this.allFlowers[i].GetCurrentState());
			}
		}

		// Token: 0x06004528 RID: 17704 RVA: 0x00147AC0 File Offset: 0x00145CC0
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				bool flag = (bool)stream.ReceiveNext();
				Flower.FlowerState currentState = this.allFlowers[i].GetCurrentState();
				Flower.FlowerState flowerState = (Flower.FlowerState)stream.ReceiveNext();
				if (currentState != flowerState)
				{
					this.allFlowers[i].UpdateFlowerState(flowerState, flag, true);
				}
			}
		}

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06004529 RID: 17705 RVA: 0x00147B33 File Offset: 0x00145D33
		// (set) Token: 0x0600452A RID: 17706 RVA: 0x00147B5D File Offset: 0x00145D5D
		[Networked]
		[NetworkedWeaved(0, 13)]
		private unsafe FlowersDataStruct Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FlowersManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(FlowersDataStruct*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FlowersManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(FlowersDataStruct*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x0600452B RID: 17707 RVA: 0x00147B88 File Offset: 0x00145D88
		public override void WriteDataFusion()
		{
			if (base.HasStateAuthority)
			{
				this.Data = new FlowersDataStruct(this.allFlowers);
			}
		}

		// Token: 0x0600452C RID: 17708 RVA: 0x00147BA4 File Offset: 0x00145DA4
		public override void ReadDataFusion()
		{
			if (this.Data.FlowerCount > 0)
			{
				for (int i = 0; i < this.Data.FlowerCount; i++)
				{
					bool flag = this.Data.FlowerWateredData[i] == 1;
					Flower.FlowerState currentState = this.allFlowers[i].GetCurrentState();
					Flower.FlowerState flowerState = (Flower.FlowerState)this.Data.FlowerStateData[i];
					if (currentState != flowerState)
					{
						this.allFlowers[i].UpdateFlowerState(flowerState, flag, true);
					}
				}
			}
		}

		// Token: 0x0600452D RID: 17709 RVA: 0x00147C3C File Offset: 0x00145E3C
		private void Update()
		{
			int num = this.flowerCheckIndex + 1;
			while (num < this.allFlowers.Count && num < this.flowerCheckIndex + this.flowersToCheck)
			{
				this.allFlowers[num].AnimCatch();
				num++;
			}
			this.flowerCheckIndex = ((this.flowerCheckIndex + this.flowersToCheck >= this.allFlowers.Count) ? 0 : (this.flowerCheckIndex + this.flowersToCheck));
		}

		// Token: 0x0600452F RID: 17711 RVA: 0x00147CE7 File Offset: 0x00145EE7
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06004530 RID: 17712 RVA: 0x00147CFF File Offset: 0x00145EFF
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x040047D9 RID: 18393
		public List<FlowersManager.FlowersInZone> sections;

		// Token: 0x040047DA RID: 18394
		public int flowersToCheck = 1;

		// Token: 0x040047DB RID: 18395
		public int flowerCheckIndex;

		// Token: 0x040047DC RID: 18396
		private readonly List<Flower> allFlowers = new List<Flower>();

		// Token: 0x040047DD RID: 18397
		private SlingshotProjectileHitNotifier[] hitNotifiers;

		// Token: 0x040047DE RID: 18398
		private readonly Dictionary<GameObject, List<Flower>> sectionToFlowersDict = new Dictionary<GameObject, List<Flower>>();

		// Token: 0x040047DF RID: 18399
		private readonly Dictionary<GameObject, GTZone> sectionToZonesDict = new Dictionary<GameObject, GTZone>();

		// Token: 0x040047E0 RID: 18400
		private bool hasBeenSerialized;

		// Token: 0x040047E1 RID: 18401
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 13)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private FlowersDataStruct _Data;

		// Token: 0x02000B08 RID: 2824
		[Serializable]
		public class FlowersInZone
		{
			// Token: 0x040047E2 RID: 18402
			public GTZone zone;

			// Token: 0x040047E3 RID: 18403
			public List<GameObject> sections;
		}
	}
}
