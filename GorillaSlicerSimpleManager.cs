using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000636 RID: 1590
public class GorillaSlicerSimpleManager : MonoBehaviour
{
	// Token: 0x0600279F RID: 10143 RVA: 0x000C439D File Offset: 0x000C259D
	protected void Awake()
	{
		if (GorillaSlicerSimpleManager.hasInstance && GorillaSlicerSimpleManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaSlicerSimpleManager.SetInstance(this);
	}

	// Token: 0x060027A0 RID: 10144 RVA: 0x000C43C0 File Offset: 0x000C25C0
	public static void CreateManager()
	{
		GorillaSlicerSimpleManager gorillaSlicerSimpleManager = new GameObject("GorillaSlicerSimpleManager").AddComponent<GorillaSlicerSimpleManager>();
		gorillaSlicerSimpleManager.fixedUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.updateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.lateUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.sW = new Stopwatch();
		GorillaSlicerSimpleManager.SetInstance(gorillaSlicerSimpleManager);
	}

	// Token: 0x060027A1 RID: 10145 RVA: 0x000C440D File Offset: 0x000C260D
	private static void SetInstance(GorillaSlicerSimpleManager manager)
	{
		GorillaSlicerSimpleManager.instance = manager;
		GorillaSlicerSimpleManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x000C4428 File Offset: 0x000C2628
	public static void RegisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (!GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (!GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (!GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Add(gSS);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x000C44BC File Offset: 0x000C26BC
	public static void UnregisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Remove(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Remove(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Remove(gSS);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x000C4554 File Offset: 0x000C2754
	public void FixedUpdate()
	{
		if (this.updateIndex < 0 || this.updateIndex >= this.fixedUpdateSlice.Count + this.updateSlice.Count + this.lateUpdateSlice.Count)
		{
			this.updateIndex = 0;
		}
		this.ticksThisFrame = 0L;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && this.updateIndex < this.fixedUpdateSlice.Count)
		{
			int num = 0;
			while (num < this.checkEveryXUpdateSteps && this.updateIndex < this.fixedUpdateSlice.Count)
			{
				if (0 <= this.updateIndex && this.updateIndex < this.fixedUpdateSlice.Count && this.fixedUpdateSlice[this.updateIndex].isActiveAndEnabled)
				{
					this.fixedUpdateSlice[this.updateIndex].SliceUpdate();
				}
				this.updateIndex++;
				num++;
			}
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x000C4680 File Offset: 0x000C2880
	public void Update()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int num = count + count2;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && count <= this.updateIndex && this.updateIndex < num)
		{
			int num2 = 0;
			while (num2 < this.checkEveryXUpdateSteps && this.updateIndex < num)
			{
				if (0 <= this.updateIndex - count && this.updateIndex - count < this.updateSlice.Count && this.updateSlice[this.updateIndex - count].isActiveAndEnabled)
				{
					this.updateSlice[this.updateIndex - count].SliceUpdate();
				}
				this.updateIndex++;
				num2++;
			}
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x000C4784 File Offset: 0x000C2984
	public void LateUpdate()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int count3 = this.lateUpdateSlice.Count;
		int num = count + count2;
		int num2 = num + count3;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && num <= this.updateIndex && this.updateIndex < num2)
		{
			int num3 = 0;
			while (num3 < this.checkEveryXUpdateSteps && this.updateIndex < num2)
			{
				if (0 <= this.updateIndex - num && this.updateIndex - num < this.lateUpdateSlice.Count && this.lateUpdateSlice[this.updateIndex - num].isActiveAndEnabled)
				{
					this.lateUpdateSlice[this.updateIndex - num].SliceUpdate();
				}
				this.updateIndex++;
				num3++;
			}
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
		if (this.updateIndex >= num2)
		{
			this.updateIndex = -1;
		}
	}

	// Token: 0x04002C1E RID: 11294
	public static GorillaSlicerSimpleManager instance;

	// Token: 0x04002C1F RID: 11295
	public static bool hasInstance;

	// Token: 0x04002C20 RID: 11296
	public List<IGorillaSliceableSimple> fixedUpdateSlice;

	// Token: 0x04002C21 RID: 11297
	public List<IGorillaSliceableSimple> updateSlice;

	// Token: 0x04002C22 RID: 11298
	public List<IGorillaSliceableSimple> lateUpdateSlice;

	// Token: 0x04002C23 RID: 11299
	public long ticksPerFrame = 1500L;

	// Token: 0x04002C24 RID: 11300
	public long ticksThisFrame;

	// Token: 0x04002C25 RID: 11301
	public int checkEveryXUpdateSteps = 10;

	// Token: 0x04002C26 RID: 11302
	public int updateIndex = -1;

	// Token: 0x04002C27 RID: 11303
	public Stopwatch sW;

	// Token: 0x02000637 RID: 1591
	public enum UpdateStep
	{
		// Token: 0x04002C29 RID: 11305
		FixedUpdate,
		// Token: 0x04002C2A RID: 11306
		Update,
		// Token: 0x04002C2B RID: 11307
		LateUpdate
	}
}
