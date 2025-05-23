using System;
using UnityEngine;

// Token: 0x02000A00 RID: 2560
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class LightningStrike : MonoBehaviour
{
	// Token: 0x06003D30 RID: 15664 RVA: 0x001227F8 File Offset: 0x001209F8
	private void Initialize()
	{
		this.ps = base.GetComponent<ParticleSystem>();
		this.psMain = this.ps.main;
		this.psMain.playOnAwake = true;
		this.psMain.stopAction = ParticleSystemStopAction.Disable;
		this.psShape = this.ps.shape;
		this.psTrails = this.ps.trails;
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.playOnAwake = true;
	}

	// Token: 0x06003D31 RID: 15665 RVA: 0x00122874 File Offset: 0x00120A74
	public void Play(Vector3 p1, Vector3 p2, float beamWidthMultiplier, float audioVolume, float duration, Gradient colorOverLifetime)
	{
		if (this.ps == null)
		{
			this.Initialize();
		}
		base.transform.position = p1;
		base.transform.rotation = Quaternion.LookRotation(p1 - p2);
		this.psShape.radius = Vector3.Distance(p1, p2) * 0.5f;
		this.psShape.position = new Vector3(0f, 0f, -this.psShape.radius);
		this.psShape.randomPositionAmount = Mathf.Clamp(this.psShape.radius / 50f, 0f, 1f);
		this.psTrails.widthOverTrail = new ParticleSystem.MinMaxCurve(beamWidthMultiplier * 0.1f, beamWidthMultiplier);
		this.psTrails.colorOverLifetime = colorOverLifetime;
		this.psMain.duration = duration;
		this.audioSource.volume = Mathf.Clamp(this.psShape.radius / 5f, 0f, 1f) * audioVolume;
		base.gameObject.SetActive(true);
	}

	// Token: 0x040040E5 RID: 16613
	public static SRand rand = new SRand("LightningStrike");

	// Token: 0x040040E6 RID: 16614
	private ParticleSystem ps;

	// Token: 0x040040E7 RID: 16615
	private ParticleSystem.MainModule psMain;

	// Token: 0x040040E8 RID: 16616
	private ParticleSystem.ShapeModule psShape;

	// Token: 0x040040E9 RID: 16617
	private ParticleSystem.TrailModule psTrails;

	// Token: 0x040040EA RID: 16618
	private AudioSource audioSource;
}
