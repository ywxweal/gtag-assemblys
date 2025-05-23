using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000595 RID: 1429
public class GRBarrierSpectral : MonoBehaviour
{
	// Token: 0x060022F9 RID: 8953 RVA: 0x000AF134 File Offset: 0x000AD334
	public void Awake()
	{
		this.hitFx.SetActive(false);
		this.destroyedFx.SetActive(false);
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000AF150 File Offset: 0x000AD350
	public void RefreshVisuals()
	{
		if (this.lastVisualUpdateHealth != this.health)
		{
			this.lastVisualUpdateHealth = this.health;
			Color color = this.visualMesh.material.GetColor("_BaseColor");
			color.a = (float)this.health / (float)this.maxHealth;
			this.visualMesh.material.SetColor("_BaseColor", color);
		}
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000AF1B9 File Offset: 0x000AD3B9
	public void TryFlash(GRTool tool)
	{
		GameAgentManager.instance.RequestImpact(this.entity, tool, base.transform.position, Vector3.zero, 1);
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x000AF1DF File Offset: 0x000AD3DF
	public void OnImpact(GRTool tool, Vector3 startPos, Vector3 impulse, byte impulseData)
	{
		if (impulseData == 1)
		{
			this.OnFlash(tool);
		}
	}

	// Token: 0x060022FD RID: 8957 RVA: 0x000AF1F0 File Offset: 0x000AD3F0
	public void OnFlash(GRTool tool)
	{
		int num = Mathf.Max(this.health - 1, 0);
		if (num != this.health)
		{
			this.health = num;
			if (this.health == 0)
			{
				this.collider.enabled = false;
				this.visualMesh.enabled = false;
				this.audioSource.PlayOneShot(this.onDestroyedClip, this.onDestroyedVolume);
				this.destroyedFx.SetActive(false);
				this.destroyedFx.SetActive(true);
			}
			else
			{
				this.audioSource.PlayOneShot(this.onDamageClip, this.onDamageVolume);
				this.hitFx.SetActive(false);
				this.hitFx.SetActive(true);
			}
			this.RefreshVisuals();
		}
	}

	// Token: 0x04002716 RID: 10006
	public GameEntity entity;

	// Token: 0x04002717 RID: 10007
	public MeshRenderer visualMesh;

	// Token: 0x04002718 RID: 10008
	public Collider collider;

	// Token: 0x04002719 RID: 10009
	public AudioSource audioSource;

	// Token: 0x0400271A RID: 10010
	public AudioClip onDamageClip;

	// Token: 0x0400271B RID: 10011
	public float onDamageVolume;

	// Token: 0x0400271C RID: 10012
	public AudioClip onDestroyedClip;

	// Token: 0x0400271D RID: 10013
	public float onDestroyedVolume;

	// Token: 0x0400271E RID: 10014
	[SerializeField]
	private GameObject hitFx;

	// Token: 0x0400271F RID: 10015
	[SerializeField]
	private GameObject destroyedFx;

	// Token: 0x04002720 RID: 10016
	public int maxHealth = 3;

	// Token: 0x04002721 RID: 10017
	[ReadOnly]
	public int health = 3;

	// Token: 0x04002722 RID: 10018
	private int lastVisualUpdateHealth = -1;
}
