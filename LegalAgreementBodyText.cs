using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000840 RID: 2112
public class LegalAgreementBodyText : MonoBehaviour
{
	// Token: 0x0600338C RID: 13196 RVA: 0x000FE3CC File Offset: 0x000FC5CC
	private void Awake()
	{
		this.textCollection.Add(this.textBox);
	}

	// Token: 0x0600338D RID: 13197 RVA: 0x000FE3E0 File Offset: 0x000FC5E0
	public void SetText(string text)
	{
		text = Regex.Unescape(text);
		string[] array = text.Split(new string[]
		{
			Environment.NewLine,
			"\\r\\n",
			"\n"
		}, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			Text text2;
			if (i >= this.textCollection.Count)
			{
				text2 = Object.Instantiate<Text>(this.textBox, base.transform);
				this.textCollection.Add(text2);
			}
			else
			{
				text2 = this.textCollection[i];
			}
			text2.text = array[i];
		}
	}

	// Token: 0x0600338E RID: 13198 RVA: 0x000FE470 File Offset: 0x000FC670
	public void ClearText()
	{
		foreach (Text text in this.textCollection)
		{
			text.text = string.Empty;
		}
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x0600338F RID: 13199 RVA: 0x000FE4CC File Offset: 0x000FC6CC
	public async Task<bool> UpdateTextFromPlayFabTitleData(string key, string version)
	{
		string text = key + "_" + version;
		this.state = LegalAgreementBodyText.State.Loading;
		PlayFabTitleDataCache.Instance.GetTitleData(text, new Action<string>(this.OnTitleDataReceived), new Action<PlayFabError>(this.OnPlayFabError));
		while (this.state == LegalAgreementBodyText.State.Loading)
		{
			await Task.Yield();
		}
		bool flag;
		if (this.cachedText != null)
		{
			this.SetText(this.cachedText.Substring(1, this.cachedText.Length - 2));
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	// Token: 0x06003390 RID: 13200 RVA: 0x000FE51F File Offset: 0x000FC71F
	private void OnPlayFabError(PlayFabError obj)
	{
		Debug.LogError("ERROR: " + obj.ErrorMessage);
		this.state = LegalAgreementBodyText.State.Error;
	}

	// Token: 0x06003391 RID: 13201 RVA: 0x000FE53D File Offset: 0x000FC73D
	private void OnTitleDataReceived(string text)
	{
		this.cachedText = text;
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x17000532 RID: 1330
	// (get) Token: 0x06003392 RID: 13202 RVA: 0x000FE550 File Offset: 0x000FC750
	public float Height
	{
		get
		{
			return this.rectTransform.rect.height;
		}
	}

	// Token: 0x04003A77 RID: 14967
	[SerializeField]
	private Text textBox;

	// Token: 0x04003A78 RID: 14968
	[SerializeField]
	private TextAsset textAsset;

	// Token: 0x04003A79 RID: 14969
	[SerializeField]
	private RectTransform rectTransform;

	// Token: 0x04003A7A RID: 14970
	private List<Text> textCollection = new List<Text>();

	// Token: 0x04003A7B RID: 14971
	private string cachedText;

	// Token: 0x04003A7C RID: 14972
	private LegalAgreementBodyText.State state;

	// Token: 0x02000841 RID: 2113
	private enum State
	{
		// Token: 0x04003A7E RID: 14974
		Ready,
		// Token: 0x04003A7F RID: 14975
		Loading,
		// Token: 0x04003A80 RID: 14976
		Error
	}
}
