using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

// Token: 0x020005BE RID: 1470
public class GRTool : MonoBehaviour
{
	// Token: 0x14000051 RID: 81
	// (add) Token: 0x060023C3 RID: 9155 RVA: 0x000B3F64 File Offset: 0x000B2164
	// (remove) Token: 0x060023C4 RID: 9156 RVA: 0x000B3F9C File Offset: 0x000B219C
	public event GRTool.EnergyChangeEvent OnEnergyChange;

	// Token: 0x060023C5 RID: 9157 RVA: 0x000B3FD1 File Offset: 0x000B21D1
	private void Awake()
	{
		this.energy = this.startEnergy;
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		this.RefreshMeters();
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x000B3FFF File Offset: 0x000B21FF
	public void RefillEnergy(int count)
	{
		this.SetEnergyInternal(this.energy + count);
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x000B400F File Offset: 0x000B220F
	public void RefillEnergy()
	{
		this.SetEnergyInternal(this.maxEnergy);
	}

	// Token: 0x060023C8 RID: 9160 RVA: 0x000B401D File Offset: 0x000B221D
	public void UseEnergy()
	{
		this.SetEnergyInternal(this.energy - this.useCost);
	}

	// Token: 0x060023C9 RID: 9161 RVA: 0x000B4032 File Offset: 0x000B2232
	public bool HasEnoughEnergy()
	{
		return this.energy >= this.useCost;
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x000B4045 File Offset: 0x000B2245
	public void SetEnergy(int newEnergy)
	{
		this.SetEnergyInternal(newEnergy);
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x000B404E File Offset: 0x000B224E
	public bool IsEnergyFull()
	{
		return this.energy >= this.maxEnergy;
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x000B4064 File Offset: 0x000B2264
	private void SetEnergyInternal(int value)
	{
		int num = this.energy;
		this.energy = Mathf.Clamp(value, 0, this.maxEnergy);
		int num2 = this.energy - num;
		GRTool.EnergyChangeEvent onEnergyChange = this.OnEnergyChange;
		if (onEnergyChange != null)
		{
			onEnergyChange(this, num2);
		}
		this.RefreshMeters();
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x000B40B0 File Offset: 0x000B22B0
	public void RefreshMeters()
	{
		for (int i = 0; i < this.energyMeters.Count; i++)
		{
			this.energyMeters[i].Refresh();
		}
	}

	// Token: 0x0400289A RID: 10394
	public int maxEnergy;

	// Token: 0x0400289B RID: 10395
	public int useCost;

	// Token: 0x0400289C RID: 10396
	public int startEnergy;

	// Token: 0x0400289D RID: 10397
	public List<GRMeterEnergy> energyMeters;

	// Token: 0x0400289E RID: 10398
	public GameEntity gameEntity;

	// Token: 0x0400289F RID: 10399
	[ReadOnly]
	public int energy;

	// Token: 0x020005BF RID: 1471
	// (Invoke) Token: 0x060023D0 RID: 9168
	public delegate void EnergyChangeEvent(GRTool tool, int energyChange);
}
