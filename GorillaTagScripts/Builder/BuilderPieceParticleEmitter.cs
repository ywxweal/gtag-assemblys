using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B5F RID: 2911
	public class BuilderPieceParticleEmitter : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x060047F2 RID: 18418 RVA: 0x00157484 File Offset: 0x00155684
		private void OnZoneChanged()
		{
			this.inBuilderZone = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.inBuilderZone && this.isPieceActive)
			{
				this.StartParticles();
				return;
			}
			if (!this.inBuilderZone)
			{
				this.StopParticles();
			}
		}

		// Token: 0x060047F3 RID: 18419 RVA: 0x001574D8 File Offset: 0x001556D8
		private void StopParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (particleSystem.isPlaying)
				{
					particleSystem.Stop();
					particleSystem.Clear();
				}
			}
		}

		// Token: 0x060047F4 RID: 18420 RVA: 0x00157538 File Offset: 0x00155738
		private void StartParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (!particleSystem.isPlaying)
				{
					particleSystem.Play();
				}
			}
		}

		// Token: 0x060047F5 RID: 18421 RVA: 0x00157594 File Offset: 0x00155794
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.StopParticles();
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x060047F6 RID: 18422 RVA: 0x001575C8 File Offset: 0x001557C8
		public void OnPieceDestroy()
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}

		// Token: 0x060047F7 RID: 18423 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x060047F8 RID: 18424 RVA: 0x001575F0 File Offset: 0x001557F0
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			if (this.inBuilderZone)
			{
				this.StartParticles();
			}
		}

		// Token: 0x060047F9 RID: 18425 RVA: 0x00157607 File Offset: 0x00155807
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			this.StopParticles();
		}

		// Token: 0x04004A77 RID: 19063
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04004A78 RID: 19064
		[SerializeField]
		private List<ParticleSystem> particles;

		// Token: 0x04004A79 RID: 19065
		private bool inBuilderZone;

		// Token: 0x04004A7A RID: 19066
		private bool isPieceActive;
	}
}
