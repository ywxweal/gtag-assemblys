using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200058F RID: 1423
public class GRArmorEnemy : MonoBehaviour
{
	// Token: 0x060022DA RID: 8922 RVA: 0x000AEB97 File Offset: 0x000ACD97
	private void Awake()
	{
		this.SetHp(0);
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x000AEBA0 File Offset: 0x000ACDA0
	public void SetHp(int hp)
	{
		this.hp = hp;
		this.RefreshArmor();
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x000AEBB0 File Offset: 0x000ACDB0
	private void RefreshArmor()
	{
		bool flag = this.hp > 0;
		GRArmorEnemy.Hide(this.renderers, !flag);
		if (flag && this.armorStateMaterials.Count > 0 && this.armorStateMaterials.Count == this.armorStateThresholds.Length)
		{
			Material material = this.armorStateMaterials[0];
			int num = 0;
			while (num < this.armorStateMaterials.Count && this.hp <= this.armorStateThresholds[num])
			{
				material = this.armorStateMaterials[num];
				if (this.hp == this.armorStateThresholds[num])
				{
					break;
				}
				num++;
			}
			if (material != this.renderers[0].material)
			{
				this.renderers[0].material = material;
				this.SetArmorColor(this.GetArmorColor());
			}
		}
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000AEC8C File Offset: 0x000ACE8C
	public void SetArmorColor(Color newColor)
	{
		if (this.renderers != null && this.renderers.Count > 0)
		{
			this.renderers[0].material.SetColor("_BaseColor", newColor);
		}
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x000AECC0 File Offset: 0x000ACEC0
	public Color GetArmorColor()
	{
		Color color = Color.white;
		if (this.renderers.Count > 0)
		{
			color = this.renderers[0].material.GetColor("_BaseColor");
		}
		return color;
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000AED00 File Offset: 0x000ACF00
	public static void Hide(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000AED41 File Offset: 0x000ACF41
	public void PlayHitFx(Vector3 position)
	{
		this.PlayFx(this.fxHit, position);
		this.PlaySound(this.hitSound, this.hitSoundVolume, position);
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x000AED63 File Offset: 0x000ACF63
	public void PlayBlockFx(Vector3 position)
	{
		this.PlayFx(this.fxBlock, position);
		this.PlaySound(this.blockSound, this.blockSoundVolume, position);
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x000AED85 File Offset: 0x000ACF85
	public void PlayDestroyFx(Vector3 position)
	{
		this.PlayFx(this.fxDestroy, position);
		this.PlaySound(this.destroySound, this.destroySoundVolume, position);
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x000AEDA7 File Offset: 0x000ACFA7
	private void PlayFx(GameObject fx, Vector3 position)
	{
		if (fx == null)
		{
			return;
		}
		fx.SetActive(false);
		fx.SetActive(true);
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x000AEDC1 File Offset: 0x000ACFC1
	private void PlaySound(AudioClip clip, float volume, Vector3 position)
	{
		this.audioSource.clip = clip;
		this.audioSource.volume = volume;
		this.audioSource.Play();
	}

	// Token: 0x040026EE RID: 9966
	[SerializeField]
	private List<Renderer> renderers;

	// Token: 0x040026EF RID: 9967
	[SerializeField]
	private int hpShellMax = 3;

	// Token: 0x040026F0 RID: 9968
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040026F1 RID: 9969
	[SerializeField]
	private GameObject fxHit;

	// Token: 0x040026F2 RID: 9970
	[SerializeField]
	private AudioClip hitSound;

	// Token: 0x040026F3 RID: 9971
	[SerializeField]
	private float hitSoundVolume;

	// Token: 0x040026F4 RID: 9972
	[SerializeField]
	private GameObject fxBlock;

	// Token: 0x040026F5 RID: 9973
	[SerializeField]
	private AudioClip blockSound;

	// Token: 0x040026F6 RID: 9974
	[SerializeField]
	private float blockSoundVolume;

	// Token: 0x040026F7 RID: 9975
	[SerializeField]
	private GameObject fxDestroy;

	// Token: 0x040026F8 RID: 9976
	[SerializeField]
	private AudioClip destroySound;

	// Token: 0x040026F9 RID: 9977
	[SerializeField]
	private float destroySoundVolume;

	// Token: 0x040026FA RID: 9978
	[SerializeField]
	private List<Material> armorStateMaterials;

	// Token: 0x040026FB RID: 9979
	[SerializeField]
	private int[] armorStateThresholds;

	// Token: 0x040026FC RID: 9980
	private int hp;
}
