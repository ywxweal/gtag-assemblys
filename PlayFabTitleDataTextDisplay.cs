using System;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006ED RID: 1773
public class PlayFabTitleDataTextDisplay : MonoBehaviour, IBuildValidation
{
	// Token: 0x17000455 RID: 1109
	// (get) Token: 0x06002C27 RID: 11303 RVA: 0x000D9A8E File Offset: 0x000D7C8E
	public string playFabKeyValue
	{
		get
		{
			return this.playfabKey;
		}
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x000D9A98 File Offset: 0x000D7C98
	private void Start()
	{
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError));
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x000D9B13 File Offset: 0x000D7D13
	private void OnPlayFabError(PlayFabError error)
	{
		if (this.textBox != null)
		{
			this.textBox.text = this.fallbackText;
		}
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x000D9B34 File Offset: 0x000D7D34
	private void OnTitleDataRequestComplete(string titleDataResult)
	{
		if (this.textBox != null)
		{
			string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
			if (text[0] == '"' && text[text.Length - 1] == '"')
			{
				text = text.Substring(1, text.Length - 2);
			}
			this.textBox.text = text;
		}
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x000D9BA8 File Offset: 0x000D7DA8
	private void OnNewTitleDataAdded(string key)
	{
		if (key == this.playfabKey && this.textBox != null)
		{
			this.textBox.color = this.newUpdateColor;
		}
	}

	// Token: 0x06002C2C RID: 11308 RVA: 0x000D9BD7 File Offset: 0x000D7DD7
	private void OnDestroy()
	{
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnNewTitleDataAdded));
	}

	// Token: 0x06002C2D RID: 11309 RVA: 0x000D9BF4 File Offset: 0x000D7DF4
	public bool BuildValidationCheck()
	{
		if (this.textBox == null)
		{
			Debug.LogError("text reference is null! sign text will be broken");
			return false;
		}
		return true;
	}

	// Token: 0x06002C2E RID: 11310 RVA: 0x000D9C14 File Offset: 0x000D7E14
	public void ChangeTitleDataAtRuntime(string newTitleDataKey)
	{
		this.playfabKey = newTitleDataKey;
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError));
	}

	// Token: 0x0400325A RID: 12890
	[SerializeField]
	private TextMeshPro textBox;

	// Token: 0x0400325B RID: 12891
	[SerializeField]
	private Color newUpdateColor = Color.magenta;

	// Token: 0x0400325C RID: 12892
	[SerializeField]
	private Color defaultTextColor = Color.white;

	// Token: 0x0400325D RID: 12893
	[Tooltip("PlayFab Title Data key from where to pull display text")]
	[SerializeField]
	private string playfabKey;

	// Token: 0x0400325E RID: 12894
	[Tooltip("Text to display when error occurs during fetch")]
	[TextArea(3, 5)]
	[SerializeField]
	private string fallbackText;
}
