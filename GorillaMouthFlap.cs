using System;
using UnityEngine;

// Token: 0x02000623 RID: 1571
public class GorillaMouthFlap : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060026E4 RID: 9956 RVA: 0x000C11FF File Offset: 0x000BF3FF
	private void Start()
	{
		this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
		this.targetFaceRenderer = this.targetFace.GetComponent<Renderer>();
		this.facePropBlock = new MaterialPropertyBlock();
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000C1229 File Offset: 0x000BF429
	public void EnableLeafBlower()
	{
		this.leafBlowerActiveUntilTimestamp = Time.time + 0.1f;
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000C123C File Offset: 0x000BF43C
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTimeUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x00010F34 File Offset: 0x0000F134
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060026E8 RID: 9960 RVA: 0x000C125C File Offset: 0x000BF45C
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.lastTimeUpdated;
		this.lastTimeUpdated = Time.time;
		if (this.speaker == null)
		{
			this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
			return;
		}
		float num = 0f;
		if (this.speaker.IsSpeaking)
		{
			num = this.speaker.Loudness;
		}
		this.CheckMouthflapChange(this.speaker.IsMicEnabled, num);
		MouthFlapLevel mouthFlapLevel = this.noMicFace;
		if (this.leafBlowerActiveUntilTimestamp > Time.time)
		{
			mouthFlapLevel = this.leafBlowerFace;
		}
		else if (this.useMicEnabled)
		{
			mouthFlapLevel = this.mouthFlapLevels[this.activeFlipbookIndex];
		}
		this.UpdateMouthFlapFlipbook(mouthFlapLevel);
	}

	// Token: 0x060026E9 RID: 9961 RVA: 0x000C1310 File Offset: 0x000BF510
	private void CheckMouthflapChange(bool isMicEnabled, float currentLoudness)
	{
		if (isMicEnabled)
		{
			this.useMicEnabled = true;
			int i = this.mouthFlapLevels.Length - 1;
			while (i >= 0)
			{
				if (currentLoudness >= this.mouthFlapLevels[i].maxRequiredVolume)
				{
					return;
				}
				if (currentLoudness > this.mouthFlapLevels[i].minRequiredVolume)
				{
					if (this.activeFlipbookIndex != i)
					{
						this.activeFlipbookIndex = i;
						this.activeFlipbookPlayTime = 0f;
						return;
					}
					return;
				}
				else
				{
					i--;
				}
			}
			return;
		}
		if (this.useMicEnabled)
		{
			this.useMicEnabled = false;
			this.activeFlipbookPlayTime = 0f;
		}
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x000C139C File Offset: 0x000BF59C
	private void UpdateMouthFlapFlipbook(MouthFlapLevel mouthFlap)
	{
		Material material = this.targetFaceRenderer.material;
		this.activeFlipbookPlayTime += this.deltaTime;
		this.activeFlipbookPlayTime %= mouthFlap.cycleDuration;
		int num = Mathf.FloorToInt(this.activeFlipbookPlayTime * (float)mouthFlap.faces.Length / mouthFlap.cycleDuration);
		material.SetTextureOffset(this._MouthMap, mouthFlap.faces[num]);
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x000C1414 File Offset: 0x000BF614
	public void SetMouthTextureReplacement(Texture2D replacementMouthAtlas)
	{
		Material material = this.targetFaceRenderer.material;
		if (!this.hasDefaultMouthAtlas)
		{
			this.defaultMouthAtlas = material.GetTexture(this._MouthMap);
			this.hasDefaultMouthAtlas = true;
		}
		material.SetTexture(this._MouthMap, replacementMouthAtlas);
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x000C1465 File Offset: 0x000BF665
	public void ClearMouthTextureReplacement()
	{
		this.targetFaceRenderer.material.SetTexture(this._MouthMap, this.defaultMouthAtlas);
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x000C1488 File Offset: 0x000BF688
	public void SetFaceMaterialReplacement(Material replacementFaceMaterial)
	{
		if (!this.hasDefaultFaceMaterial)
		{
			this.defaultFaceMaterial = this.targetFaceRenderer.material;
			this.hasDefaultFaceMaterial = true;
		}
		this.targetFaceRenderer.material = replacementFaceMaterial;
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x000C14B6 File Offset: 0x000BF6B6
	public void ClearFaceMaterialReplacement()
	{
		if (this.hasDefaultFaceMaterial)
		{
			this.targetFaceRenderer.material = this.defaultFaceMaterial;
		}
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04002B67 RID: 11111
	public GameObject targetFace;

	// Token: 0x04002B68 RID: 11112
	public MouthFlapLevel[] mouthFlapLevels;

	// Token: 0x04002B69 RID: 11113
	public MouthFlapLevel noMicFace;

	// Token: 0x04002B6A RID: 11114
	public MouthFlapLevel leafBlowerFace;

	// Token: 0x04002B6B RID: 11115
	private bool useMicEnabled;

	// Token: 0x04002B6C RID: 11116
	private float leafBlowerActiveUntilTimestamp;

	// Token: 0x04002B6D RID: 11117
	private int activeFlipbookIndex;

	// Token: 0x04002B6E RID: 11118
	private float activeFlipbookPlayTime;

	// Token: 0x04002B6F RID: 11119
	private GorillaSpeakerLoudness speaker;

	// Token: 0x04002B70 RID: 11120
	private float lastTimeUpdated;

	// Token: 0x04002B71 RID: 11121
	private float deltaTime;

	// Token: 0x04002B72 RID: 11122
	private Renderer targetFaceRenderer;

	// Token: 0x04002B73 RID: 11123
	private MaterialPropertyBlock facePropBlock;

	// Token: 0x04002B74 RID: 11124
	private Texture defaultMouthAtlas;

	// Token: 0x04002B75 RID: 11125
	private Material defaultFaceMaterial;

	// Token: 0x04002B76 RID: 11126
	private bool hasDefaultMouthAtlas;

	// Token: 0x04002B77 RID: 11127
	private bool hasDefaultFaceMaterial;

	// Token: 0x04002B78 RID: 11128
	private ShaderHashId _MouthMap = "_MouthMap";

	// Token: 0x04002B79 RID: 11129
	private ShaderHashId _BaseMap = "_BaseMap";
}
