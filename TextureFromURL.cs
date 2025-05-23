using System;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000A04 RID: 2564
public class TextureFromURL : MonoBehaviour
{
	// Token: 0x06003D48 RID: 15688 RVA: 0x00122DF6 File Offset: 0x00120FF6
	private void OnEnable()
	{
		if (this.data.Length == 0)
		{
			return;
		}
		if (this.source == TextureFromURL.Source.TitleData)
		{
			this.LoadFromTitleData();
			return;
		}
		this.applyRemoteTexture(this.data);
	}

	// Token: 0x06003D49 RID: 15689 RVA: 0x00122E24 File Offset: 0x00121024
	private async void LoadFromTitleData()
	{
		int attempt = 0;
		while (attempt < this.maxTitleDataAttempts && PlayFabTitleDataCache.Instance == null)
		{
			await Task.Delay(1000);
			attempt++;
		}
		if (PlayFabTitleDataCache.Instance != null)
		{
			PlayFabTitleDataCache.Instance.GetTitleData(this.data, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError));
		}
	}

	// Token: 0x06003D4A RID: 15690 RVA: 0x00122E5B File Offset: 0x0012105B
	private void OnDisable()
	{
		if (this.texture != null)
		{
			Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x06003D4B RID: 15691 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnPlayFabError(PlayFabError error)
	{
	}

	// Token: 0x06003D4C RID: 15692 RVA: 0x00122E80 File Offset: 0x00121080
	private void OnTitleDataRequestComplete(string imageUrl)
	{
		imageUrl = imageUrl.Replace("\\r", "\r").Replace("\\n", "\n");
		if (imageUrl[0] == '"' && imageUrl[imageUrl.Length - 1] == '"')
		{
			imageUrl = imageUrl.Substring(1, imageUrl.Length - 2);
		}
		this.applyRemoteTexture(imageUrl);
	}

	// Token: 0x06003D4D RID: 15693 RVA: 0x00122EE4 File Offset: 0x001210E4
	private async void applyRemoteTexture(string imageUrl)
	{
		Texture2D texture2D = await this.GetRemoteTexture(imageUrl);
		this.texture = texture2D;
		if (this.texture != null)
		{
			this._renderer.material.mainTexture = this.texture;
		}
	}

	// Token: 0x06003D4E RID: 15694 RVA: 0x00122F24 File Offset: 0x00121124
	private async Task<Texture2D> GetRemoteTexture(string url)
	{
		Texture2D texture2D;
		using (UnityWebRequest wr = UnityWebRequestTexture.GetTexture(url))
		{
			UnityWebRequestAsyncOperation asyncOp = wr.SendWebRequest();
			while (!asyncOp.isDone)
			{
				await Task.Delay(1000);
			}
			if (wr.result == UnityWebRequest.Result.Success)
			{
				texture2D = DownloadHandlerTexture.GetContent(wr);
			}
			else
			{
				texture2D = null;
			}
		}
		return texture2D;
	}

	// Token: 0x04004104 RID: 16644
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04004105 RID: 16645
	[SerializeField]
	private TextureFromURL.Source source;

	// Token: 0x04004106 RID: 16646
	[Tooltip("If Source is set to 'TitleData' Data should be the id of the title data entry that defines an image URL. If Source is set to 'URL' Data should be a URL that points to an image.")]
	[SerializeField]
	private string data;

	// Token: 0x04004107 RID: 16647
	private Texture2D texture;

	// Token: 0x04004108 RID: 16648
	private int maxTitleDataAttempts = 10;

	// Token: 0x02000A05 RID: 2565
	private enum Source
	{
		// Token: 0x0400410A RID: 16650
		TitleData,
		// Token: 0x0400410B RID: 16651
		URL
	}
}
