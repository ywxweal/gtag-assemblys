using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020009E8 RID: 2536
[Serializable]
public class GTScene : IEquatable<GTScene>
{
	// Token: 0x170005E5 RID: 1509
	// (get) Token: 0x06003C98 RID: 15512 RVA: 0x00121154 File Offset: 0x0011F354
	public string alias
	{
		get
		{
			return this._alias;
		}
	}

	// Token: 0x170005E6 RID: 1510
	// (get) Token: 0x06003C99 RID: 15513 RVA: 0x0012115C File Offset: 0x0011F35C
	public string name
	{
		get
		{
			return this._name;
		}
	}

	// Token: 0x170005E7 RID: 1511
	// (get) Token: 0x06003C9A RID: 15514 RVA: 0x00121164 File Offset: 0x0011F364
	public string path
	{
		get
		{
			return this._path;
		}
	}

	// Token: 0x170005E8 RID: 1512
	// (get) Token: 0x06003C9B RID: 15515 RVA: 0x0012116C File Offset: 0x0011F36C
	public string guid
	{
		get
		{
			return this._guid;
		}
	}

	// Token: 0x170005E9 RID: 1513
	// (get) Token: 0x06003C9C RID: 15516 RVA: 0x00121174 File Offset: 0x0011F374
	public int buildIndex
	{
		get
		{
			return this._buildIndex;
		}
	}

	// Token: 0x170005EA RID: 1514
	// (get) Token: 0x06003C9D RID: 15517 RVA: 0x0012117C File Offset: 0x0011F37C
	public bool includeInBuild
	{
		get
		{
			return this._includeInBuild;
		}
	}

	// Token: 0x170005EB RID: 1515
	// (get) Token: 0x06003C9E RID: 15518 RVA: 0x00121184 File Offset: 0x0011F384
	public bool isLoaded
	{
		get
		{
			return SceneManager.GetSceneByBuildIndex(this._buildIndex).isLoaded;
		}
	}

	// Token: 0x170005EC RID: 1516
	// (get) Token: 0x06003C9F RID: 15519 RVA: 0x001211A4 File Offset: 0x0011F3A4
	public bool hasAlias
	{
		get
		{
			return !string.IsNullOrWhiteSpace(this._alias);
		}
	}

	// Token: 0x06003CA0 RID: 15520 RVA: 0x001211B4 File Offset: 0x0011F3B4
	public GTScene(string name, string path, string guid, int buildIndex, bool includeInBuild)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentNullException("name");
		}
		if (string.IsNullOrWhiteSpace(path))
		{
			throw new ArgumentNullException("path");
		}
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this._name = name;
		this._path = path;
		this._guid = guid;
		this._buildIndex = buildIndex;
		this._includeInBuild = includeInBuild;
	}

	// Token: 0x06003CA1 RID: 15521 RVA: 0x00121225 File Offset: 0x0011F425
	public override int GetHashCode()
	{
		return this._guid.GetHashCode();
	}

	// Token: 0x06003CA2 RID: 15522 RVA: 0x00121232 File Offset: 0x0011F432
	public override string ToString()
	{
		return this.ToJson(false);
	}

	// Token: 0x06003CA3 RID: 15523 RVA: 0x0012123B File Offset: 0x0011F43B
	public bool Equals(GTScene other)
	{
		return this._guid.Equals(other._guid) && this._name == other._name && this._path == other._path;
	}

	// Token: 0x06003CA4 RID: 15524 RVA: 0x00121278 File Offset: 0x0011F478
	public override bool Equals(object obj)
	{
		GTScene gtscene = obj as GTScene;
		return gtscene != null && this.Equals(gtscene);
	}

	// Token: 0x06003CA5 RID: 15525 RVA: 0x00121298 File Offset: 0x0011F498
	public static bool operator ==(GTScene x, GTScene y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003CA6 RID: 15526 RVA: 0x001212A1 File Offset: 0x0011F4A1
	public static bool operator !=(GTScene x, GTScene y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06003CA7 RID: 15527 RVA: 0x001212AD File Offset: 0x0011F4AD
	public void LoadAsync()
	{
		if (this.isLoaded)
		{
			return;
		}
		SceneManager.LoadSceneAsync(this._buildIndex, LoadSceneMode.Additive);
	}

	// Token: 0x06003CA8 RID: 15528 RVA: 0x001212C5 File Offset: 0x0011F4C5
	public void UnloadAsync()
	{
		if (!this.isLoaded)
		{
			return;
		}
		SceneManager.UnloadSceneAsync(this._buildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
	}

	// Token: 0x06003CA9 RID: 15529 RVA: 0x00045F91 File Offset: 0x00044191
	public static GTScene FromAsset(object sceneAsset)
	{
		return null;
	}

	// Token: 0x06003CAA RID: 15530 RVA: 0x00045F91 File Offset: 0x00044191
	public static GTScene From(object editorBuildSettingsScene)
	{
		return null;
	}

	// Token: 0x04004082 RID: 16514
	[SerializeField]
	private string _alias;

	// Token: 0x04004083 RID: 16515
	[SerializeField]
	private string _name;

	// Token: 0x04004084 RID: 16516
	[SerializeField]
	private string _path;

	// Token: 0x04004085 RID: 16517
	[SerializeField]
	private string _guid;

	// Token: 0x04004086 RID: 16518
	[SerializeField]
	private int _buildIndex;

	// Token: 0x04004087 RID: 16519
	[SerializeField]
	private bool _includeInBuild;
}
