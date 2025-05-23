using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200052F RID: 1327
public abstract class CosmeticCritter : MonoBehaviour
{
	// Token: 0x1700033F RID: 831
	// (get) Token: 0x0600202C RID: 8236 RVA: 0x000A242F File Offset: 0x000A062F
	// (set) Token: 0x0600202D RID: 8237 RVA: 0x000A2437 File Offset: 0x000A0637
	public int Seed { get; protected set; }

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x0600202E RID: 8238 RVA: 0x000A2440 File Offset: 0x000A0640
	// (set) Token: 0x0600202F RID: 8239 RVA: 0x000A2448 File Offset: 0x000A0648
	public CosmeticCritterSpawner Spawner { get; protected set; }

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06002030 RID: 8240 RVA: 0x000A2451 File Offset: 0x000A0651
	// (set) Token: 0x06002031 RID: 8241 RVA: 0x000A2459 File Offset: 0x000A0659
	public Type CachedType { get; private set; }

	// Token: 0x06002032 RID: 8242 RVA: 0x000A2462 File Offset: 0x000A0662
	public int GetGlobalMaxCritters()
	{
		return this.globalMaxCritters;
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x000A246A File Offset: 0x000A066A
	public void SetSeedSpawnerTypeAndTime(int seed, CosmeticCritterSpawner spawner, Type type, double time)
	{
		this.Seed = seed;
		this.Spawner = spawner;
		this.CachedType = type;
		this.startTime = time;
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnSpawn()
	{
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnDespawn()
	{
	}

	// Token: 0x06002036 RID: 8246 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void SetRandomVariables()
	{
	}

	// Token: 0x06002037 RID: 8247
	public abstract void Tick();

	// Token: 0x06002038 RID: 8248 RVA: 0x000A2489 File Offset: 0x000A0689
	protected double GetAliveTime()
	{
		if (!PhotonNetwork.InRoom)
		{
			return Time.timeAsDouble - this.startTime;
		}
		return PhotonNetwork.Time - this.startTime;
	}

	// Token: 0x06002039 RID: 8249 RVA: 0x000A24AB File Offset: 0x000A06AB
	public virtual bool Expired()
	{
		return this.GetAliveTime() > (double)this.lifetime || this.GetAliveTime() < 0.0;
	}

	// Token: 0x04002442 RID: 9282
	[Tooltip("After this many seconds the critter will forcibly despawn.")]
	[SerializeField]
	protected float lifetime;

	// Token: 0x04002443 RID: 9283
	[Tooltip("The maximum number of this kind of critter that can be in the room at any given time.")]
	[SerializeField]
	private int globalMaxCritters;

	// Token: 0x04002447 RID: 9287
	protected double startTime;
}
