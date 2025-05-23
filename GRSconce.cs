using System;
using UnityEngine;

// Token: 0x020005BC RID: 1468
public class GRSconce : MonoBehaviour
{
	// Token: 0x060023BB RID: 9147 RVA: 0x000B3E08 File Offset: 0x000B2008
	private void Awake()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange += this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged += this.OnStateChange;
		}
		this.state = GRSconce.State.Off;
		this.StopLight();
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x000ABAB1 File Offset: 0x000A9CB1
	private bool IsAuthority()
	{
		return GameEntityManager.instance.IsAuthority();
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x000B3E6C File Offset: 0x000B206C
	private void SetState(GRSconce.State newState)
	{
		this.state = newState;
		GRSconce.State state = this.state;
		if (state != GRSconce.State.Off)
		{
			if (state == GRSconce.State.On)
			{
				this.StartLight();
			}
		}
		else
		{
			this.StopLight();
		}
		if (this.IsAuthority())
		{
			GameEntityManager.instance.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x000B3EC0 File Offset: 0x000B20C0
	private void StartLight()
	{
		this.gameLight.gameObject.SetActive(true);
		this.audioSource.volume = this.lightOnSoundVolume;
		this.audioSource.clip = this.lightOnSound;
		this.audioSource.Play();
		this.meshRenderer.material = this.onMaterial;
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000B3F1C File Offset: 0x000B211C
	private void StopLight()
	{
		this.gameLight.gameObject.SetActive(false);
		this.meshRenderer.material = this.offMaterial;
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000B3F40 File Offset: 0x000B2140
	private void OnEnergyChange(GRTool tool, int energy)
	{
		if (this.IsAuthority() && this.state == GRSconce.State.Off && tool.IsEnergyFull())
		{
			this.SetState(GRSconce.State.On);
		}
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000B3F64 File Offset: 0x000B2164
	private void OnStateChange(long prevState, long nextState)
	{
		if (!this.IsAuthority())
		{
			GRSconce.State state = (GRSconce.State)nextState;
			this.SetState(state);
		}
	}

	// Token: 0x0400288D RID: 10381
	public GameEntity gameEntity;

	// Token: 0x0400288E RID: 10382
	public GameLight gameLight;

	// Token: 0x0400288F RID: 10383
	public GRTool tool;

	// Token: 0x04002890 RID: 10384
	public MeshRenderer meshRenderer;

	// Token: 0x04002891 RID: 10385
	public Material offMaterial;

	// Token: 0x04002892 RID: 10386
	public Material onMaterial;

	// Token: 0x04002893 RID: 10387
	public AudioSource audioSource;

	// Token: 0x04002894 RID: 10388
	public AudioClip lightOnSound;

	// Token: 0x04002895 RID: 10389
	public float lightOnSoundVolume;

	// Token: 0x04002896 RID: 10390
	private GRSconce.State state;

	// Token: 0x020005BD RID: 1469
	private enum State
	{
		// Token: 0x04002898 RID: 10392
		Off,
		// Token: 0x04002899 RID: 10393
		On
	}
}
