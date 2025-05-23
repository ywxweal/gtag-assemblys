using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000E1A RID: 3610
	public class CrittersActorSpawner : MonoBehaviour
	{
		// Token: 0x06005A67 RID: 23143 RVA: 0x001B8DDE File Offset: 0x001B6FDE
		private void Awake()
		{
			this.spawnPoint.OnSpawnChanged += this.HandleSpawnedActor;
		}

		// Token: 0x06005A68 RID: 23144 RVA: 0x001B8DF7 File Offset: 0x001B6FF7
		private void OnEnable()
		{
			if (!CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Add(this);
			}
		}

		// Token: 0x06005A69 RID: 23145 RVA: 0x001B8E1F File Offset: 0x001B701F
		private void OnDisable()
		{
			if (CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Remove(this);
			}
		}

		// Token: 0x06005A6A RID: 23146 RVA: 0x001B8E48 File Offset: 0x001B7048
		public void ProcessLocal()
		{
			if (!CrittersManager.instance.LocalAuthority())
			{
				return;
			}
			if (this.nextSpawnTime <= (double)Time.time)
			{
				this.nextSpawnTime = (double)(Time.time + (float)this.spawnDelay);
				if (this.currentSpawnedObject == null || !this.currentSpawnedObject.isEnabled)
				{
					this.SpawnActor();
				}
			}
			if (this.currentSpawnedObject.IsNotNull())
			{
				if (!this.currentSpawnedObject.isEnabled)
				{
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.insideSpawnerCheck.bounds.Contains(this.currentSpawnedObject.transform.position))
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.VerifySpawnAttached())
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
				}
			}
		}

		// Token: 0x06005A6B RID: 23147 RVA: 0x001B8F42 File Offset: 0x001B7142
		public void DoReset()
		{
			this.currentSpawnedObject = null;
		}

		// Token: 0x06005A6C RID: 23148 RVA: 0x001B8F4B File Offset: 0x001B714B
		private void HandleSpawnedActor(CrittersActor spawnedActor)
		{
			this.currentSpawnedObject = spawnedActor;
		}

		// Token: 0x06005A6D RID: 23149 RVA: 0x001B8F54 File Offset: 0x001B7154
		private void SpawnActor()
		{
			CrittersActor crittersActor = CrittersManager.instance.SpawnActor(this.actorType, this.subActorIndex);
			this.spawnPoint.SetSpawnedActor(crittersActor);
			if (crittersActor.IsNull())
			{
				return;
			}
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				crittersActor.GrabbedBy(this.spawnPoint, true, default(Quaternion), default(Vector3), false);
				return;
			}
			crittersActor.MoveActor(this.spawnPoint.transform.position, this.spawnPoint.transform.rotation, false, true, true);
			crittersActor.rb.velocity = Vector3.zero;
			if (this.applyImpulseOnSpawn)
			{
				crittersActor.SetImpulse();
			}
		}

		// Token: 0x06005A6E RID: 23150 RVA: 0x001B9000 File Offset: 0x001B7200
		private bool VerifySpawnAttached()
		{
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				CrittersActor crittersActor;
				CrittersManager.instance.actorById.TryGetValue(this.currentSpawnedObject.parentActorId, out crittersActor);
				if (crittersActor.IsNull() || crittersActor != this.spawnPoint)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04005E8A RID: 24202
		public CrittersActorSpawnerPoint spawnPoint;

		// Token: 0x04005E8B RID: 24203
		public CrittersActor currentSpawnedObject;

		// Token: 0x04005E8C RID: 24204
		public CrittersActor.CrittersActorType actorType;

		// Token: 0x04005E8D RID: 24205
		public int subActorIndex = -1;

		// Token: 0x04005E8E RID: 24206
		public Collider insideSpawnerCheck;

		// Token: 0x04005E8F RID: 24207
		public int spawnDelay = 5;

		// Token: 0x04005E90 RID: 24208
		public bool applyImpulseOnSpawn = true;

		// Token: 0x04005E91 RID: 24209
		public bool attachSpawnedObjectToSpawnLocation;

		// Token: 0x04005E92 RID: 24210
		private double nextSpawnTime;
	}
}
