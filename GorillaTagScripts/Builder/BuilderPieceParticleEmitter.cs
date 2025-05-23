using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B5F RID: 2911
	public class BuilderPieceParticleEmitter : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x060047F3 RID: 18419 RVA: 0x0015755C File Offset: 0x0015575C
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

		// Token: 0x060047F4 RID: 18420 RVA: 0x001575B0 File Offset: 0x001557B0
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

		// Token: 0x060047F5 RID: 18421 RVA: 0x00157610 File Offset: 0x00155810
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

		// Token: 0x060047F6 RID: 18422 RVA: 0x0015766C File Offset: 0x0015586C
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.StopParticles();
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x060047F7 RID: 18423 RVA: 0x001576A0 File Offset: 0x001558A0
		public void OnPieceDestroy()
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}

		// Token: 0x060047F8 RID: 18424 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x060047F9 RID: 18425 RVA: 0x001576C8 File Offset: 0x001558C8
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			if (this.inBuilderZone)
			{
				this.StartParticles();
			}
		}

		// Token: 0x060047FA RID: 18426 RVA: 0x001576DF File Offset: 0x001558DF
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			this.StopParticles();
		}

		// Token: 0x04004A78 RID: 19064
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04004A79 RID: 19065
		[SerializeField]
		private List<ParticleSystem> particles;

		// Token: 0x04004A7A RID: 19066
		private bool inBuilderZone;

		// Token: 0x04004A7B RID: 19067
		private bool isPieceActive;
	}
}
