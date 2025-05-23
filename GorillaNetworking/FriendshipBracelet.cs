using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C27 RID: 3111
	public class FriendshipBracelet : MonoBehaviour
	{
		// Token: 0x06004D0C RID: 19724 RVA: 0x0016EE96 File Offset: 0x0016D096
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
		}

		// Token: 0x06004D0D RID: 19725 RVA: 0x0016EEA4 File Offset: 0x0016D0A4
		private AudioSource GetAudioSource()
		{
			if (!this.isLeftHand)
			{
				return this.ownerRig.rightHandPlayer;
			}
			return this.ownerRig.leftHandPlayer;
		}

		// Token: 0x06004D0E RID: 19726 RVA: 0x0016EEC5 File Offset: 0x0016D0C5
		private void OnEnable()
		{
			this.PlayAppearEffects();
		}

		// Token: 0x06004D0F RID: 19727 RVA: 0x0016EECD File Offset: 0x0016D0CD
		public void PlayAppearEffects()
		{
			this.GetAudioSource().GTPlayOneShot(this.braceletFormedSound, 1f);
			if (this.braceletFormedParticle)
			{
				this.braceletFormedParticle.Play();
			}
		}

		// Token: 0x06004D10 RID: 19728 RVA: 0x0016EF00 File Offset: 0x0016D100
		private void OnDisable()
		{
			if (!this.ownerRig.gameObject.activeInHierarchy)
			{
				return;
			}
			this.GetAudioSource().GTPlayOneShot(this.braceletBrokenSound, 1f);
			if (this.braceletBrokenParticle)
			{
				this.braceletBrokenParticle.Play();
			}
		}

		// Token: 0x06004D11 RID: 19729 RVA: 0x0016EF50 File Offset: 0x0016D150
		public void UpdateBeads(List<Color> colors, int selfIndex)
		{
			int num = colors.Count - 1;
			int num2 = (this.braceletBeads.Length - num) / 2;
			for (int i = 0; i < this.braceletBeads.Length; i++)
			{
				int num3 = i - num2;
				if (num3 >= 0 && num3 < num)
				{
					this.braceletBeads[i].enabled = true;
					this.braceletBeads[i].material.color = colors[num3];
					this.braceletBananas[i].gameObject.SetActive(num3 == selfIndex);
				}
				else
				{
					this.braceletBeads[i].enabled = false;
					this.braceletBananas[i].gameObject.SetActive(false);
				}
			}
			SkinnedMeshRenderer[] array = this.braceletStrings;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].material.color = colors[colors.Count - 1];
			}
		}

		// Token: 0x04004FF6 RID: 20470
		[SerializeField]
		private SkinnedMeshRenderer[] braceletStrings;

		// Token: 0x04004FF7 RID: 20471
		[SerializeField]
		private MeshRenderer[] braceletBeads;

		// Token: 0x04004FF8 RID: 20472
		[SerializeField]
		private MeshRenderer[] braceletBananas;

		// Token: 0x04004FF9 RID: 20473
		[SerializeField]
		private bool isLeftHand;

		// Token: 0x04004FFA RID: 20474
		[SerializeField]
		private AudioClip braceletFormedSound;

		// Token: 0x04004FFB RID: 20475
		[SerializeField]
		private AudioClip braceletBrokenSound;

		// Token: 0x04004FFC RID: 20476
		[SerializeField]
		private ParticleSystem braceletFormedParticle;

		// Token: 0x04004FFD RID: 20477
		[SerializeField]
		private ParticleSystem braceletBrokenParticle;

		// Token: 0x04004FFE RID: 20478
		private VRRig ownerRig;
	}
}
