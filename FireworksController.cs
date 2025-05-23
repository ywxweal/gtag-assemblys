using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020009F2 RID: 2546
public class FireworksController : MonoBehaviour
{
	// Token: 0x06003CF2 RID: 15602 RVA: 0x001218E2 File Offset: 0x0011FAE2
	private void Awake()
	{
		this._launchOrder = this.fireworks.ToArray<Firework>();
		this._rnd = new SRand(this.seed);
	}

	// Token: 0x06003CF3 RID: 15603 RVA: 0x00121908 File Offset: 0x0011FB08
	public void LaunchVolley()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this._rnd.Shuffle<Firework>(this._launchOrder);
		for (int i = 0; i < this._launchOrder.Length; i++)
		{
			MonoBehaviour monoBehaviour = this._launchOrder[i];
			float num = this._rnd.NextFloat() * this.roundLength;
			monoBehaviour.Invoke("Launch", num);
		}
	}

	// Token: 0x06003CF4 RID: 15604 RVA: 0x0012196C File Offset: 0x0011FB6C
	public void LaunchVolleyRound()
	{
		int num = 0;
		while ((long)num < (long)((ulong)this.roundNumVolleys))
		{
			float num2 = this._rnd.NextFloat() * this.roundLength;
			base.Invoke("LaunchVolley", num2);
			num++;
		}
	}

	// Token: 0x06003CF5 RID: 15605 RVA: 0x001219B0 File Offset: 0x0011FBB0
	public void Launch(Firework fw)
	{
		if (!fw)
		{
			return;
		}
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		AudioSource sourceOrigin = fw.sourceOrigin;
		int num = this._rnd.NextInt(this.bursts.Length);
		AudioClip audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		AudioClip audioClip2 = this.bursts[num];
		while (this._lastWhistle == audioClip)
		{
			audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		}
		while (this._lastBurst == audioClip2)
		{
			num = this._rnd.NextInt(this.bursts.Length);
			audioClip2 = this.bursts[num];
		}
		this._lastWhistle = audioClip;
		this._lastBurst = audioClip2;
		int num2 = this._rnd.NextInt(fw.explosions.Length);
		ParticleSystem particleSystem = fw.explosions[num2];
		if (fw.doTrail)
		{
			ParticleSystem trail = fw.trail;
			trail.startColor = fw.colorOrigin;
			trail.subEmitters.GetSubEmitterSystem(0).colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fw.colorOrigin, fw.colorTarget);
			trail.Stop();
			trail.Play();
		}
		sourceOrigin.pitch = this._rnd.NextFloat(0.92f, 1f);
		fw.doTrailAudio = this._rnd.NextBool();
		FireworksController.ExplosionEvent explosionEvent = new FireworksController.ExplosionEvent
		{
			firework = fw,
			timeSince = TimeSince.Now(),
			burstIndex = num,
			explosionIndex = num2,
			delay = (double)(fw.doTrail ? audioClip.length : 0f),
			active = true
		};
		if (fw.doExplosion)
		{
			this.PostExplosionEvent(explosionEvent);
		}
		if (fw.doTrailAudio && this._timeSinceLastWhistle > this.minWhistleDelay)
		{
			this._timeSinceLastWhistle = TimeSince.Now();
			sourceOrigin.PlayOneShot(audioClip, this._rnd.NextFloat(this.whistleVolumeMin, this.whistleVolumeMax));
		}
		particleSystem.Stop();
		particleSystem.transform.position = position2;
	}

	// Token: 0x06003CF6 RID: 15606 RVA: 0x00121BE0 File Offset: 0x0011FDE0
	private void PostExplosionEvent(FireworksController.ExplosionEvent ev)
	{
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			if (!this._explosionQueue[i].active)
			{
				this._explosionQueue[i] = ev;
				return;
			}
		}
	}

	// Token: 0x06003CF7 RID: 15607 RVA: 0x00121C21 File Offset: 0x0011FE21
	private void Update()
	{
		this.ProcessEvents();
	}

	// Token: 0x06003CF8 RID: 15608 RVA: 0x00121C2C File Offset: 0x0011FE2C
	private void ProcessEvents()
	{
		if (this._explosionQueue == null || this._explosionQueue.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			FireworksController.ExplosionEvent explosionEvent = this._explosionQueue[i];
			if (explosionEvent.active && explosionEvent.timeSince >= explosionEvent.delay)
			{
				this.DoExplosion(explosionEvent);
				this._explosionQueue[i] = default(FireworksController.ExplosionEvent);
			}
		}
	}

	// Token: 0x06003CF9 RID: 15609 RVA: 0x00121CA0 File Offset: 0x0011FEA0
	private void DoExplosion(FireworksController.ExplosionEvent ev)
	{
		Firework firework = ev.firework;
		ParticleSystem particleSystem = firework.explosions[ev.explosionIndex];
		ParticleSystem.MinMaxGradient minMaxGradient = new ParticleSystem.MinMaxGradient(firework.colorOrigin, firework.colorTarget);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime2 = particleSystem.subEmitters.GetSubEmitterSystem(0).colorOverLifetime;
		colorOverLifetime.color = minMaxGradient;
		colorOverLifetime2.color = minMaxGradient;
		ParticleSystem particleSystem2 = firework.explosions[ev.explosionIndex];
		particleSystem2.Stop();
		particleSystem2.Play();
		firework.sourceTarget.PlayOneShot(this.bursts[ev.burstIndex]);
	}

	// Token: 0x06003CFA RID: 15610 RVA: 0x00121D30 File Offset: 0x0011FF30
	public void RenderGizmo(Firework fw, Color c)
	{
		if (!fw)
		{
			return;
		}
		if (!fw.origin || !fw.target)
		{
			return;
		}
		Gizmos.color = c;
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		Gizmos.DrawLine(position, position2);
		Gizmos.DrawWireCube(position, Vector3.one * 0.5f);
		Gizmos.DrawWireCube(position2, Vector3.one * 0.5f);
	}

	// Token: 0x040040A3 RID: 16547
	public Firework[] fireworks;

	// Token: 0x040040A4 RID: 16548
	public AudioClip[] whistles;

	// Token: 0x040040A5 RID: 16549
	public AudioClip[] bursts;

	// Token: 0x040040A6 RID: 16550
	[Space]
	[Range(0f, 1f)]
	public float whistleVolumeMin = 0.1f;

	// Token: 0x040040A7 RID: 16551
	[Range(0f, 1f)]
	public float whistleVolumeMax = 0.15f;

	// Token: 0x040040A8 RID: 16552
	public float minWhistleDelay = 1f;

	// Token: 0x040040A9 RID: 16553
	[Space]
	[NonSerialized]
	private AudioClip _lastWhistle;

	// Token: 0x040040AA RID: 16554
	[NonSerialized]
	private AudioClip _lastBurst;

	// Token: 0x040040AB RID: 16555
	[NonSerialized]
	private Firework[] _launchOrder;

	// Token: 0x040040AC RID: 16556
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x040040AD RID: 16557
	[NonSerialized]
	private FireworksController.ExplosionEvent[] _explosionQueue = new FireworksController.ExplosionEvent[8];

	// Token: 0x040040AE RID: 16558
	[NonSerialized]
	private TimeSince _timeSinceLastWhistle = 10f;

	// Token: 0x040040AF RID: 16559
	[Space]
	public string seed = "Fireworks.Summer23";

	// Token: 0x040040B0 RID: 16560
	[Space]
	public uint roundNumVolleys = 6U;

	// Token: 0x040040B1 RID: 16561
	public uint roundLength = 6U;

	// Token: 0x040040B2 RID: 16562
	[FormerlySerializedAs("_timeOfDayEvent")]
	[FormerlySerializedAs("_timeOfDay")]
	[Space]
	[SerializeField]
	private TimeEvent _fireworksEvent;

	// Token: 0x020009F3 RID: 2547
	[Serializable]
	public struct ExplosionEvent
	{
		// Token: 0x040040B3 RID: 16563
		public TimeSince timeSince;

		// Token: 0x040040B4 RID: 16564
		public double delay;

		// Token: 0x040040B5 RID: 16565
		public int explosionIndex;

		// Token: 0x040040B6 RID: 16566
		public int burstIndex;

		// Token: 0x040040B7 RID: 16567
		public bool active;

		// Token: 0x040040B8 RID: 16568
		public Firework firework;
	}
}
