using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x020006D1 RID: 1745
public class DayNightCycle : MonoBehaviour
{
	// Token: 0x06002B85 RID: 11141 RVA: 0x000D6748 File Offset: 0x000D4948
	public void Awake()
	{
		this.fromMap = new Texture2D(this._sunriseMap.width, this._sunriseMap.height);
		this.fromMap = LightmapSettings.lightmaps[0].lightmapColor;
		this.toMap = new Texture2D(this._dayMap.width, this._dayMap.height);
		this.toMap.SetPixels(this._dayMap.GetPixels());
		this.toMap.Apply();
		this.workBlockMix = new Color[this.subTextureSize * this.subTextureSize];
		this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height, this.fromMap.graphicsFormat, TextureCreationFlags.None);
		this.newData = new LightmapData();
		this.textureHeight = this.fromMap.height;
		this.textureWidth = this.fromMap.width;
		this.subTextureArray = new Texture2D[(int)Mathf.Pow((float)(this.textureHeight / this.subTextureSize), 2f)];
		Debug.Log("aaaa " + this.fromMap.format.ToString());
		Debug.Log("aaaa " + this.fromMap.graphicsFormat.ToString());
		this.startJob = false;
		this.startCoroutine = false;
		this.startedCoroutine = false;
		this.finishedCoroutine = false;
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x000D68CC File Offset: 0x000D4ACC
	public void Update()
	{
		if (this.startJob)
		{
			this.startJob = false;
			this.startTime = Time.realtimeSinceStartup;
			base.StartCoroutine(this.UpdateWork());
			this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
		}
		if (this.jobStarted && this.jobHandle.IsCompleted)
		{
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.jobHandle.Complete();
			this.jobStarted = false;
			this.newTexture.SetPixels(this.job.mixedPixels.ToArray());
			this.newData.lightmapDir = LightmapSettings.lightmaps[0].lightmapDir;
			LightmapSettings.lightmaps = new LightmapData[] { this.newData };
			this.job.fromPixels.Dispose();
			this.job.toPixels.Dispose();
			this.job.mixedPixels.Dispose();
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
		if (this.startCoroutine)
		{
			this.startCoroutine = false;
			this.startTime = Time.realtimeSinceStartup;
			this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height);
			base.StartCoroutine(this.UpdateWork());
		}
		if (this.startedCoroutine && this.finishedCoroutine)
		{
			this.startedCoroutine = false;
			this.finishedCoroutine = false;
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.newData = LightmapSettings.lightmaps[0];
			this.newData.lightmapColor = this.fromMap;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			lightmaps[0].lightmapColor = this.fromMap;
			LightmapSettings.lightmaps = lightmaps;
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
	}

	// Token: 0x06002B87 RID: 11143 RVA: 0x000D6ABA File Offset: 0x000D4CBA
	public IEnumerator UpdateWork()
	{
		yield return 0;
		this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
		this.startTime = Time.realtimeSinceStartup;
		this.startedCoroutine = true;
		this.currentSubTexture = 0;
		int num;
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.subTextureArray[i] = new Texture2D(this.subTextureSize, this.subTextureSize, this.fromMap.graphicsFormat, TextureCreationFlags.None);
			yield return 0;
			num = i;
		}
		for (int i = 0; i < this.textureWidth / this.subTextureSize; i = num + 1)
		{
			this.currentColumn = i;
			for (int j = 0; j < this.textureHeight / this.subTextureSize; j = num + 1)
			{
				this.currentRow = j;
				this.workBlockFrom = this.fromMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				this.workBlockTo = this.toMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				for (int k = 0; k < this.subTextureSize * this.subTextureSize - 1; k++)
				{
					this.workBlockMix[k] = Color.Lerp(this.workBlockFrom[k], this.workBlockTo[k], this.lerpAmount);
				}
				this.subTextureArray[j * (this.textureWidth / this.subTextureSize) + i].SetPixels(0, 0, this.subTextureSize, this.subTextureSize, this.workBlockMix);
				yield return 0;
				num = j;
			}
			num = i;
		}
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.currentSubTexture = i;
			this.subTextureArray[i].Apply();
			yield return 0;
			Graphics.CopyTexture(this.subTextureArray[i], 0, 0, 0, 0, this.subTextureSize, this.subTextureSize, this.newTexture, 0, 0, i * this.subTextureSize % this.textureHeight, (int)Mathf.Floor((float)(this.subTextureSize * i / this.textureHeight)) * this.subTextureSize);
			yield return 0;
			num = i;
		}
		this.finishedCoroutine = true;
		yield break;
	}

	// Token: 0x04003164 RID: 12644
	public Texture2D _dayMap;

	// Token: 0x04003165 RID: 12645
	private Texture2D fromMap;

	// Token: 0x04003166 RID: 12646
	public Texture2D _sunriseMap;

	// Token: 0x04003167 RID: 12647
	private Texture2D toMap;

	// Token: 0x04003168 RID: 12648
	public DayNightCycle.LerpBakedLightingJob job;

	// Token: 0x04003169 RID: 12649
	public JobHandle jobHandle;

	// Token: 0x0400316A RID: 12650
	public bool isComplete;

	// Token: 0x0400316B RID: 12651
	private float startTime;

	// Token: 0x0400316C RID: 12652
	public float timeTakenStartingJob;

	// Token: 0x0400316D RID: 12653
	public float timeTakenPostJob;

	// Token: 0x0400316E RID: 12654
	public float timeTakenDuringJob;

	// Token: 0x0400316F RID: 12655
	public LightmapData newData;

	// Token: 0x04003170 RID: 12656
	private Color[] fromPixels;

	// Token: 0x04003171 RID: 12657
	private Color[] toPixels;

	// Token: 0x04003172 RID: 12658
	private Color[] mixedPixels;

	// Token: 0x04003173 RID: 12659
	private LightmapData[] newDatas;

	// Token: 0x04003174 RID: 12660
	public Texture2D newTexture;

	// Token: 0x04003175 RID: 12661
	public int textureWidth;

	// Token: 0x04003176 RID: 12662
	public int textureHeight;

	// Token: 0x04003177 RID: 12663
	private Color[] workBlockFrom;

	// Token: 0x04003178 RID: 12664
	private Color[] workBlockTo;

	// Token: 0x04003179 RID: 12665
	private Color[] workBlockMix;

	// Token: 0x0400317A RID: 12666
	public int subTextureSize = 1024;

	// Token: 0x0400317B RID: 12667
	public Texture2D[] subTextureArray;

	// Token: 0x0400317C RID: 12668
	public bool startCoroutine;

	// Token: 0x0400317D RID: 12669
	public bool startedCoroutine;

	// Token: 0x0400317E RID: 12670
	public bool finishedCoroutine;

	// Token: 0x0400317F RID: 12671
	public bool startJob;

	// Token: 0x04003180 RID: 12672
	public float switchTimeTaken;

	// Token: 0x04003181 RID: 12673
	public bool jobStarted;

	// Token: 0x04003182 RID: 12674
	public float lerpAmount;

	// Token: 0x04003183 RID: 12675
	public int currentRow;

	// Token: 0x04003184 RID: 12676
	public int currentColumn;

	// Token: 0x04003185 RID: 12677
	public int currentSubTexture;

	// Token: 0x04003186 RID: 12678
	public int currentRowInSubtexture;

	// Token: 0x020006D2 RID: 1746
	public struct LerpBakedLightingJob : IJob
	{
		// Token: 0x06002B89 RID: 11145 RVA: 0x000D6ADC File Offset: 0x000D4CDC
		public void Execute()
		{
			for (int i = 0; i < this.fromPixels.Length; i++)
			{
				this.mixedPixels[i] = Color.Lerp(this.fromPixels[i], this.toPixels[i], 0.5f);
			}
		}

		// Token: 0x04003187 RID: 12679
		public NativeArray<Color> fromPixels;

		// Token: 0x04003188 RID: 12680
		public NativeArray<Color> toPixels;

		// Token: 0x04003189 RID: 12681
		public NativeArray<Color> mixedPixels;

		// Token: 0x0400318A RID: 12682
		public float lerpValue;
	}
}
