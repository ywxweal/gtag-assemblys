using System;
using System.Collections.Generic;
using Photon.Pun;

// Token: 0x02000043 RID: 67
public class CrittersActorSpawnerPoint : CrittersActor
{
	// Token: 0x14000002 RID: 2
	// (add) Token: 0x0600014A RID: 330 RVA: 0x00008E9C File Offset: 0x0000709C
	// (remove) Token: 0x0600014B RID: 331 RVA: 0x00008ED4 File Offset: 0x000070D4
	public event Action<CrittersActor> OnSpawnChanged;

	// Token: 0x0600014C RID: 332 RVA: 0x00008F09 File Offset: 0x00007109
	public override void Initialize()
	{
		base.Initialize();
		base.UpdateImpulses(false, false);
	}

	// Token: 0x0600014D RID: 333 RVA: 0x00008F19 File Offset: 0x00007119
	public override void OnDisable()
	{
		base.OnDisable();
		this.spawnedActorID = -1;
		this.spawnedActor = null;
	}

	// Token: 0x0600014E RID: 334 RVA: 0x00008F30 File Offset: 0x00007130
	public void SetSpawnedActor(CrittersActor actor)
	{
		if (this.spawnedActor == actor)
		{
			return;
		}
		this.spawnedActor = actor;
		if (this.spawnedActor != null)
		{
			this.spawnedActorID = this.spawnedActor.actorId;
		}
		else
		{
			this.spawnedActorID = -1;
		}
		Action<CrittersActor> onSpawnChanged = this.OnSpawnChanged;
		if (onSpawnChanged != null)
		{
			onSpawnChanged(this.spawnedActor);
		}
		this.updatedSinceLastFrame = true;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x00008F9C File Offset: 0x0000719C
	private void UpdateSpawnedActor(int newSpawnedActorID)
	{
		if (this.spawnedActorID == newSpawnedActorID)
		{
			return;
		}
		if (newSpawnedActorID == -1)
		{
			this.spawnedActorID = newSpawnedActorID;
			this.spawnedActor = null;
		}
		else
		{
			CrittersActor crittersActor;
			if (!CrittersManager.instance.actorById.TryGetValue(newSpawnedActorID, out crittersActor))
			{
				return;
			}
			this.spawnedActorID = newSpawnedActorID;
			this.spawnedActor = crittersActor;
		}
		Action<CrittersActor> onSpawnChanged = this.OnSpawnChanged;
		if (onSpawnChanged == null)
		{
			return;
		}
		onSpawnChanged(this.spawnedActor);
	}

	// Token: 0x06000150 RID: 336 RVA: 0x00009002 File Offset: 0x00007202
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.spawnedActorID);
	}

	// Token: 0x06000151 RID: 337 RVA: 0x0000901C File Offset: 0x0000721C
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		if (!base.UpdateSpecificActor(stream))
		{
			return false;
		}
		int num;
		if (!CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num))
		{
			return false;
		}
		if (num < -1 || num >= CrittersManager.instance.universalActorId)
		{
			return false;
		}
		this.UpdateSpawnedActor(num);
		return true;
	}

	// Token: 0x06000152 RID: 338 RVA: 0x00009062 File Offset: 0x00007262
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.spawnedActorID);
		return this.TotalActorDataLength();
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00009084 File Offset: 0x00007284
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00009090 File Offset: 0x00007290
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		if (num >= -1 && num < CrittersManager.instance.universalActorId)
		{
			return this.TotalActorDataLength();
		}
		this.UpdateSpawnedActor(num);
		return this.TotalActorDataLength();
	}

	// Token: 0x04000175 RID: 373
	private CrittersActor spawnedActor;

	// Token: 0x04000176 RID: 374
	private int spawnedActorID = -1;
}
