using System;
using System.Collections.Generic;
using Cysharp.Text;
using GorillaExtensions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000A2 RID: 162
public class TextTyperAnimatorMono : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000407 RID: 1031 RVA: 0x00017CE1 File Offset: 0x00015EE1
	public void EdRestartAnimation()
	{
		this.m_textMesh.maxVisibleCharacters = 0;
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00017CF0 File Offset: 0x00015EF0
	protected void Awake()
	{
		this._has_typingSoundBank = this.m_typingSoundBank != null;
		this._has_beginEntrySoundBank = this.m_beginEntrySoundBank != null;
		this._waitTime = this._random.NextFloat(this.m_typingSpeedMinMax.x, this.m_typingSpeedMinMax.y);
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00010F2B File Offset: 0x0000F12B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00010F34 File Offset: 0x0000F134
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00017D48 File Offset: 0x00015F48
	public void SliceUpdate()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		int num = this.m_textMesh.maxVisibleCharacters;
		if (num < 0 || num >= this._charCount || this._timeOfLastTypedChar + this._waitTime > realtimeSinceStartup)
		{
			return;
		}
		num = (this.m_textMesh.maxVisibleCharacters = num + 1);
		this._timeOfLastTypedChar = realtimeSinceStartup;
		if (this._has_beginEntrySoundBank && num == 1)
		{
			this.m_beginEntrySoundBank.Play();
		}
		else if (this._has_typingSoundBank)
		{
			this.m_typingSoundBank.Play();
		}
		this._waitTime = this._random.NextFloat(this.m_typingSpeedMinMax.x, this.m_typingSpeedMinMax.y);
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x00017DEF File Offset: 0x00015FEF
	public void SetText(string text, IList<int> entryIndexes, int nonRichTextTagsCharCount)
	{
		this._charCount = nonRichTextTagsCharCount;
		this.m_textMesh.SetText(text, true);
		this.m_textMesh.maxVisibleCharacters = 0;
		this._SetEntryIndexes(entryIndexes);
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00017E18 File Offset: 0x00016018
	public void SetText(string text, IList<int> entryIndexes)
	{
		this.SetText(text, entryIndexes, text.Length);
		this.m_textMesh.SetText(text, true);
		this.m_textMesh.maxVisibleCharacters = 0;
		this._SetEntryIndexes(entryIndexes);
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00017E48 File Offset: 0x00016048
	public void SetText(string text)
	{
		this.SetText(text, Array.Empty<int>());
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00017E56 File Offset: 0x00016056
	public void SetText(Utf16ValueStringBuilder zStringBuilder, IList<int> entryIndexes, int nonRichTextTagsCharCount)
	{
		this._charCount = nonRichTextTagsCharCount;
		this.m_textMesh.SetTextToZString(zStringBuilder);
		this.m_textMesh.maxVisibleCharacters = 0;
		this._SetEntryIndexes(entryIndexes);
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00017E7E File Offset: 0x0001607E
	public void SetText(Utf16ValueStringBuilder zStringBuilder)
	{
		this.SetText(zStringBuilder, Array.Empty<int>(), zStringBuilder.Length);
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00017E93 File Offset: 0x00016093
	private void _SetEntryIndexes(IList<int> entryIndexes)
	{
		this._entryIndexes.Clear();
		this._entryIndexes.AddRange(entryIndexes);
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00017EAC File Offset: 0x000160AC
	public void UpdateText(Utf16ValueStringBuilder zStringBuilder, int nonRichTextTagsCharCount)
	{
		TMP_Text textMesh = this.m_textMesh;
		this._charCount = nonRichTextTagsCharCount;
		textMesh.maxVisibleCharacters = nonRichTextTagsCharCount;
		this.m_textMesh.SetTextToZString(zStringBuilder);
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x0400047F RID: 1151
	[FormerlySerializedAs("_textMesh")]
	[Tooltip("Text Mesh Pro component.")]
	[SerializeField]
	private TMP_Text m_textMesh;

	// Token: 0x04000480 RID: 1152
	[Tooltip("Delay between characters in seconds")]
	[SerializeField]
	private Vector2 m_typingSpeedMinMax = new Vector2(0.05f, 0.1f);

	// Token: 0x04000481 RID: 1153
	[Header("Audio")]
	[Tooltip("AudioClips to play while typing.")]
	[SerializeField]
	private SoundBankPlayer m_typingSoundBank;

	// Token: 0x04000482 RID: 1154
	private bool _has_typingSoundBank;

	// Token: 0x04000483 RID: 1155
	[Tooltip("AudioClips to play when a ")]
	[SerializeField]
	private SoundBankPlayer m_beginEntrySoundBank;

	// Token: 0x04000484 RID: 1156
	private bool _has_beginEntrySoundBank;

	// Token: 0x04000485 RID: 1157
	private int _charCount;

	// Token: 0x04000486 RID: 1158
	private readonly List<int> _entryIndexes = new List<int>(16);

	// Token: 0x04000487 RID: 1159
	private float _waitTime;

	// Token: 0x04000488 RID: 1160
	private float _timeOfLastTypedChar = -1f;

	// Token: 0x04000489 RID: 1161
	private Unity.Mathematics.Random _random = new Unity.Mathematics.Random(6746U);
}
