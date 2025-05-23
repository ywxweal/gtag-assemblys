using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200059B RID: 1435
public class GRCollectibleDispenser : MonoBehaviour
{
	// Token: 0x17000362 RID: 866
	// (get) Token: 0x0600230D RID: 8973 RVA: 0x000AF5D8 File Offset: 0x000AD7D8
	public bool CollectibleAlreadySpawned
	{
		get
		{
			return this.currentCollectible != null;
		}
	}

	// Token: 0x17000363 RID: 867
	// (get) Token: 0x0600230E RID: 8974 RVA: 0x000AF5E8 File Offset: 0x000AD7E8
	public bool ReadyToDispenseNewCollectible
	{
		get
		{
			double num = (double)this.collectibleRespawnTimeMinutes * 60.0;
			bool flag = this.collectiblesDispensed < this.maxDispenseCount;
			return !this.CollectibleAlreadySpawned && flag && Time.timeAsDouble - this.collectibleDispenseRequestTime > num && Time.timeAsDouble - this.collectibleDispenseTime > num && Time.timeAsDouble - this.collectibleConsumedTime > num;
		}
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000AF652 File Offset: 0x000AD852
	private void OnEnable()
	{
		if (GhostReactor.instance != null)
		{
			GhostReactor.instance.collectibleDispensers.Add(this);
		}
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x000AF671 File Offset: 0x000AD871
	private void OnDisable()
	{
		if (GhostReactor.instance != null)
		{
			GhostReactor.instance.collectibleDispensers.Remove(this);
		}
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x000AF694 File Offset: 0x000AD894
	public void RequestDispenseCollectible()
	{
		if (this.ReadyToDispenseNewCollectible)
		{
			if (GameEntityManager.instance.IsAuthority())
			{
				GameEntityManager.instance.RequestCreateItem(this.collectiblePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)GameEntityManager.instance.GetNetIdFromEntityId(this.gameEntity.id));
			}
			this.collectiblesDispensed++;
			this.collectibleDispenseTime = Time.timeAsDouble;
		}
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x000AF71C File Offset: 0x000AD91C
	public void OnCollectibleConsumed()
	{
		if (this.currentCollectible != null && this.currentCollectible.IsNotNull())
		{
			GRCollectible grcollectible = this.currentCollectible;
			grcollectible.OnCollected = (Action)Delegate.Remove(grcollectible.OnCollected, new Action(this.OnCollectibleConsumed));
			GameEntity entity = this.currentCollectible.entity;
			entity.OnGrabbed = (Action)Delegate.Remove(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
			this.currentCollectible = null;
		}
		this.collectibleConsumedTime = Time.timeAsDouble;
		if (this.collectiblesDispensed >= this.maxDispenseCount)
		{
			this.dispenserExhaustedEffect.Play();
			this.audioSource.PlayOneShot(this.dispenserExhaustedClip, this.dispenserExhaustedVolume);
			this.stillDispensingModel.gameObject.SetActive(false);
			this.fullyConsumedModel.gameObject.SetActive(true);
			return;
		}
		this.collectibleTakenEffect.Play();
		this.audioSource.PlayOneShot(this.collectibleTakenClip, this.collectibleTakenVolume);
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000AF820 File Offset: 0x000ADA20
	public void GetSpawnedCollectible(GRCollectible collectible)
	{
		this.currentCollectible = collectible;
		collectible.OnCollected = (Action)Delegate.Combine(collectible.OnCollected, new Action(this.OnCollectibleConsumed));
		GameEntity entity = collectible.entity;
		entity.OnGrabbed = (Action)Delegate.Combine(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
	}

	// Token: 0x04002739 RID: 10041
	public GameEntity gameEntity;

	// Token: 0x0400273A RID: 10042
	public GameEntity collectiblePrefab;

	// Token: 0x0400273B RID: 10043
	public Transform spawnLocation;

	// Token: 0x0400273C RID: 10044
	public LayerMask collectibleLayerMask;

	// Token: 0x0400273D RID: 10045
	public float collectibleRespawnTimeMinutes = 1.5f;

	// Token: 0x0400273E RID: 10046
	public int maxDispenseCount = 3;

	// Token: 0x0400273F RID: 10047
	public AudioSource audioSource;

	// Token: 0x04002740 RID: 10048
	public Transform stillDispensingModel;

	// Token: 0x04002741 RID: 10049
	public Transform fullyConsumedModel;

	// Token: 0x04002742 RID: 10050
	public ParticleSystem collectibleTakenEffect;

	// Token: 0x04002743 RID: 10051
	public AudioClip collectibleTakenClip;

	// Token: 0x04002744 RID: 10052
	public float collectibleTakenVolume;

	// Token: 0x04002745 RID: 10053
	public ParticleSystem dispenserExhaustedEffect;

	// Token: 0x04002746 RID: 10054
	public AudioClip dispenserExhaustedClip;

	// Token: 0x04002747 RID: 10055
	public float dispenserExhaustedVolume;

	// Token: 0x04002748 RID: 10056
	private GRCollectible currentCollectible;

	// Token: 0x04002749 RID: 10057
	private Coroutine getSpawnedCollectibleCoroutine;

	// Token: 0x0400274A RID: 10058
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x0400274B RID: 10059
	private int collectiblesDispensed;

	// Token: 0x0400274C RID: 10060
	private double collectibleDispenseRequestTime = -10000.0;

	// Token: 0x0400274D RID: 10061
	private double collectibleDispenseTime = -10000.0;

	// Token: 0x0400274E RID: 10062
	private double collectibleConsumedTime = -10000.0;
}
