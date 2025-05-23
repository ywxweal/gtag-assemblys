using System;
using System.Collections;
using System.Threading;
using UnityEngine;

// Token: 0x020005FD RID: 1533
public class GorillaDayNight : MonoBehaviour
{
	// Token: 0x060025BF RID: 9663 RVA: 0x000BBA80 File Offset: 0x000B9C80
	public void Awake()
	{
		if (GorillaDayNight.instance == null)
		{
			GorillaDayNight.instance = this;
		}
		else if (GorillaDayNight.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.test = false;
		this.working = false;
		this.lerpValue = 0.5f;
		this.workingLightMapDatas = new LightmapData[3];
		this.workingLightMapData = new LightmapData();
		this.workingLightMapData.lightmapColor = this.lightmapDatas[0].lightTextures[0];
		this.workingLightMapData.lightmapDir = this.lightmapDatas[0].dirTextures[0];
	}

	// Token: 0x060025C0 RID: 9664 RVA: 0x000BBB24 File Offset: 0x000B9D24
	public void Update()
	{
		if (this.test)
		{
			this.test = false;
			base.StartCoroutine(this.LightMapSet(this.firstData, this.secondData, this.lerpValue));
		}
	}

	// Token: 0x060025C1 RID: 9665 RVA: 0x000BBB54 File Offset: 0x000B9D54
	public void DoWork()
	{
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		this.done = true;
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x000BBD60 File Offset: 0x000B9F60
	public void DoLightsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x060025C3 RID: 9667 RVA: 0x000BBE24 File Offset: 0x000BA024
	public void DoDirsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x000BBEE7 File Offset: 0x000BA0E7
	private IEnumerator LightMapSet(int setFirstData, int setSecondData, float setLerp)
	{
		this.working = true;
		this.firstData = setFirstData;
		this.secondData = setSecondData;
		this.lerpValue = setLerp;
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.lightsThread = new Thread(new ThreadStart(this.DoLightsStep));
			this.lightsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapColor.Apply(false);
			this.dirsThread = new Thread(new ThreadStart(this.DoDirsStep));
			this.dirsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		LightmapSettings.lightmaps = this.workingLightMapDatas;
		this.working = false;
		this.done = true;
		yield break;
	}

	// Token: 0x04002A55 RID: 10837
	[OnEnterPlay_SetNull]
	public static volatile GorillaDayNight instance;

	// Token: 0x04002A56 RID: 10838
	public GorillaLightmapData[] lightmapDatas;

	// Token: 0x04002A57 RID: 10839
	private LightmapData[] workingLightMapDatas;

	// Token: 0x04002A58 RID: 10840
	private LightmapData workingLightMapData;

	// Token: 0x04002A59 RID: 10841
	public float lerpValue;

	// Token: 0x04002A5A RID: 10842
	public bool done;

	// Token: 0x04002A5B RID: 10843
	public bool finishedStep;

	// Token: 0x04002A5C RID: 10844
	private Color[] fromPixels;

	// Token: 0x04002A5D RID: 10845
	private Color[] toPixels;

	// Token: 0x04002A5E RID: 10846
	private Color[] mixedPixels;

	// Token: 0x04002A5F RID: 10847
	public int firstData;

	// Token: 0x04002A60 RID: 10848
	public int secondData;

	// Token: 0x04002A61 RID: 10849
	public int i;

	// Token: 0x04002A62 RID: 10850
	public int j;

	// Token: 0x04002A63 RID: 10851
	public int k;

	// Token: 0x04002A64 RID: 10852
	public int l;

	// Token: 0x04002A65 RID: 10853
	private Thread lightsThread;

	// Token: 0x04002A66 RID: 10854
	private Thread dirsThread;

	// Token: 0x04002A67 RID: 10855
	public bool test;

	// Token: 0x04002A68 RID: 10856
	public bool working;
}
