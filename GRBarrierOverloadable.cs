using System;
using UnityEngine;

// Token: 0x02000593 RID: 1427
public class GRBarrierOverloadable : MonoBehaviour
{
	// Token: 0x060022F4 RID: 8948 RVA: 0x000AF04D File Offset: 0x000AD24D
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000AF080 File Offset: 0x000AD280
	private void OnEnergyChange(GRTool tool, int energyChange)
	{
		if (this.state == GRBarrierOverloadable.State.Active && tool.energy >= tool.maxEnergy)
		{
			this.SetState(GRBarrierOverloadable.State.Destroyed);
			if (GameEntityManager.instance.IsAuthority())
			{
				GameEntityManager.instance.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000AF0D1 File Offset: 0x000AD2D1
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!GameEntityManager.instance.IsAuthority())
		{
			this.SetState((GRBarrierOverloadable.State)nextState);
		}
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000AF0EC File Offset: 0x000AD2EC
	public void SetState(GRBarrierOverloadable.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRBarrierOverloadable.State state = this.state;
			if (state == GRBarrierOverloadable.State.Active)
			{
				this.meshRenderer.enabled = true;
				this.collider.enabled = true;
				return;
			}
			if (state != GRBarrierOverloadable.State.Destroyed)
			{
				return;
			}
			this.audioSource.Play();
			this.meshRenderer.enabled = false;
			this.collider.enabled = false;
		}
	}

	// Token: 0x0400270D RID: 9997
	public GRTool tool;

	// Token: 0x0400270E RID: 9998
	public GameEntity gameEntity;

	// Token: 0x0400270F RID: 9999
	public AudioSource audioSource;

	// Token: 0x04002710 RID: 10000
	public MeshRenderer meshRenderer;

	// Token: 0x04002711 RID: 10001
	public Collider collider;

	// Token: 0x04002712 RID: 10002
	private GRBarrierOverloadable.State state;

	// Token: 0x02000594 RID: 1428
	public enum State
	{
		// Token: 0x04002714 RID: 10004
		Active,
		// Token: 0x04002715 RID: 10005
		Destroyed
	}
}
