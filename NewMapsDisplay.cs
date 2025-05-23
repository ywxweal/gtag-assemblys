using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using ModIO;
using TMPro;
using UnityEngine;

// Token: 0x02000727 RID: 1831
public class NewMapsDisplay : MonoBehaviour
{
	// Token: 0x06002D94 RID: 11668 RVA: 0x000E1E48 File Offset: 0x000E0048
	public void OnEnable()
	{
		if (ModIOManager.GetNewMapsModId() == ModId.Null)
		{
			return;
		}
		this.mapImage.gameObject.SetActive(false);
		this.modNameText.text = "";
		this.modNameText.gameObject.SetActive(false);
		this.modCreatorLabelText.gameObject.SetActive(false);
		this.modCreatorText.text = "";
		this.modCreatorText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
		if (GorillaServer.Instance == null || !GorillaServer.Instance.FeatureFlagsReady)
		{
			this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
			return;
		}
		this.Initialize();
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x000E1F14 File Offset: 0x000E0114
	public void OnDisable()
	{
		if (this.initCoroutine != null)
		{
			base.StopCoroutine(this.initCoroutine);
			this.initCoroutine = null;
		}
		this.newMapsModProfile = default(ModProfile);
		this.newMapDatas.Clear();
		this.slideshowActive = false;
		this.slideshowIndex = 0;
		this.lastSlideshowUpdate = 0f;
		this.mapImage.gameObject.SetActive(false);
		this.modNameText.text = "";
		this.modNameText.gameObject.SetActive(false);
		this.modCreatorLabelText.gameObject.SetActive(false);
		this.modCreatorText.text = "";
		this.modCreatorText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x000E1FE1 File Offset: 0x000E01E1
	private IEnumerator DelayedInitialize()
	{
		bool flag = GorillaServer.Instance != null && GorillaServer.Instance.FeatureFlagsReady;
		while (!flag)
		{
			yield return new WaitForSecondsRealtime(3f);
			flag = GorillaServer.Instance != null && GorillaServer.Instance.FeatureFlagsReady;
		}
		this.Initialize();
		this.initCoroutine = null;
		yield break;
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x000E1FF0 File Offset: 0x000E01F0
	private void Initialize()
	{
		if (!this.requestingNewMapsModProfile && !this.downloadingImages)
		{
			ModIOManager.Initialize(delegate(ModIORequestResult result)
			{
				if (result.success)
				{
					if (!base.isActiveAndEnabled)
					{
						return;
					}
					this.requestingNewMapsModProfile = true;
					ModIOManager.GetModProfile(ModIOManager.GetNewMapsModId(), new Action<ModIORequestResultAnd<ModProfile>>(this.OnGetNewMapsModProfile));
				}
			});
		}
	}

	// Token: 0x06002D98 RID: 11672 RVA: 0x000E2014 File Offset: 0x000E0214
	private async void OnGetNewMapsModProfile(ModIORequestResultAnd<ModProfile> resultAndProfile)
	{
		this.requestingNewMapsModProfile = false;
		if (resultAndProfile.result.success)
		{
			if (base.isActiveAndEnabled)
			{
				this.newMapsModProfile = resultAndProfile.data;
				this.newMapDatas.Clear();
				string text = "";
				string text2 = "";
				foreach (KeyValuePair<string, string> keyValuePair in resultAndProfile.data.metadataKeyValuePairs)
				{
					if (keyValuePair.Key.Equals("mapNames"))
					{
						text = keyValuePair.Value;
					}
					else if (keyValuePair.Key.Equals("mapCreators"))
					{
						text2 = keyValuePair.Value;
					}
				}
				string[] mapNamesList = (text.IsNullOrEmpty() ? null : text.Split(',', StringSplitOptions.None));
				string[] mapCreatorsList = (text2.IsNullOrEmpty() ? null : text2.Split(',', StringSplitOptions.None));
				this.downloadingImages = true;
				int j;
				for (int i = 0; i < this.newMapsModProfile.galleryImages320x180.Length; i = j)
				{
					DownloadReference downloadReference = this.newMapsModProfile.galleryImages320x180[i];
					string mapName = ((mapNamesList != null && mapNamesList.Length > i) ? mapNamesList[i] : "UNKNOWN MAP");
					string mapCreator = ((mapCreatorsList != null && mapCreatorsList.Length > i) ? mapCreatorsList[i] : "UNKNOWN CREATOR");
					ResultAnd<Texture2D> resultAnd = await ModIOUnityAsync.DownloadTexture(downloadReference);
					NewMapsDisplay.NewMapData newMapData = new NewMapsDisplay.NewMapData
					{
						image = (resultAnd.result.Succeeded() ? resultAnd.value : null),
						name = mapName,
						creator = mapCreator
					};
					this.newMapDatas.Add(newMapData);
					mapName = null;
					mapCreator = null;
					j = i + 1;
				}
				this.downloadingImages = false;
				this.StartSlideshow();
			}
		}
	}

	// Token: 0x06002D99 RID: 11673 RVA: 0x000E2053 File Offset: 0x000E0253
	private void StartSlideshow()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			return;
		}
		this.slideshowIndex = 0;
		this.slideshowActive = true;
		this.UpdateSlideshow();
	}

	// Token: 0x06002D9A RID: 11674 RVA: 0x000E2077 File Offset: 0x000E0277
	public void Update()
	{
		if (!this.slideshowActive || Time.time - this.lastSlideshowUpdate < this.slideshowUpdateInterval)
		{
			return;
		}
		this.UpdateSlideshow();
	}

	// Token: 0x06002D9B RID: 11675 RVA: 0x000E209C File Offset: 0x000E029C
	private void UpdateSlideshow()
	{
		this.loadingText.gameObject.SetActive(false);
		this.lastSlideshowUpdate = Time.time;
		Texture2D image = this.newMapDatas[this.slideshowIndex].image;
		if (image != null)
		{
			this.mapImage.sprite = Sprite.Create(image, new Rect(0f, 0f, (float)image.width, (float)image.height), new Vector2(0.5f, 0.5f));
			this.mapImage.gameObject.SetActive(true);
		}
		else
		{
			this.mapImage.gameObject.SetActive(false);
		}
		this.modNameText.text = this.newMapDatas[this.slideshowIndex].name;
		this.modCreatorText.text = this.newMapDatas[this.slideshowIndex].creator;
		this.modNameText.gameObject.SetActive(true);
		this.modCreatorLabelText.gameObject.SetActive(true);
		this.modCreatorText.gameObject.SetActive(true);
		this.slideshowIndex++;
		if (this.slideshowIndex >= this.newMapDatas.Count)
		{
			this.slideshowIndex = 0;
		}
	}

	// Token: 0x040033D3 RID: 13267
	[SerializeField]
	private SpriteRenderer mapImage;

	// Token: 0x040033D4 RID: 13268
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x040033D5 RID: 13269
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x040033D6 RID: 13270
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x040033D7 RID: 13271
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x040033D8 RID: 13272
	[SerializeField]
	private float slideshowUpdateInterval = 1f;

	// Token: 0x040033D9 RID: 13273
	private ModProfile newMapsModProfile;

	// Token: 0x040033DA RID: 13274
	private List<NewMapsDisplay.NewMapData> newMapDatas = new List<NewMapsDisplay.NewMapData>();

	// Token: 0x040033DB RID: 13275
	private bool slideshowActive;

	// Token: 0x040033DC RID: 13276
	private int slideshowIndex;

	// Token: 0x040033DD RID: 13277
	private float lastSlideshowUpdate;

	// Token: 0x040033DE RID: 13278
	private bool requestingNewMapsModProfile;

	// Token: 0x040033DF RID: 13279
	private bool downloadingImages;

	// Token: 0x040033E0 RID: 13280
	private Coroutine initCoroutine;

	// Token: 0x02000728 RID: 1832
	private struct NewMapData
	{
		// Token: 0x040033E1 RID: 13281
		public Texture2D image;

		// Token: 0x040033E2 RID: 13282
		public string name;

		// Token: 0x040033E3 RID: 13283
		public string creator;
	}
}
