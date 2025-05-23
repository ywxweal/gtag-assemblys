using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020009E8 RID: 2536
[Serializable]
public class GTScene : IEquatable<GTScene>
{
	// Token: 0x170005E5 RID: 1509
	// (get) Token: 0x06003C99 RID: 15513 RVA: 0x0012122C File Offset: 0x0011F42C
	public string alias
	{
		get
		{
			return this._alias;
		}
	}

	// Token: 0x170005E6 RID: 1510
	// (get) Token: 0x06003C9A RID: 15514 RVA: 0x00121234 File Offset: 0x0011F434
	public string name
	{
		get
		{
			return this._name;
		}
	}

	// Token: 0x170005E7 RID: 1511
	// (get) Token: 0x06003C9B RID: 15515 RVA: 0x0012123C File Offset: 0x0011F43C
	public string path
	{
		get
		{
			return this._path;
		}
	}

	// Token: 0x170005E8 RID: 1512
	// (get) Token: 0x06003C9C RID: 15516 RVA: 0x00121244 File Offset: 0x0011F444
	public string guid
	{
		get
		{
			return this._guid;
		}
	}

	// Token: 0x170005E9 RID: 1513
	// (get) Token: 0x06003C9D RID: 15517 RVA: 0x0012124C File Offset: 0x0011F44C
	public int buildIndex
	{
		get
		{
			return this._buildIndex;
		}
	}

	// Token: 0x170005EA RID: 1514
	// (get) Token: 0x06003C9E RID: 15518 RVA: 0x00121254 File Offset: 0x0011F454
	public bool includeInBuild
	{
		get
		{
			return this._includeInBuild;
		}
	}

	// Token: 0x170005EB RID: 1515
	// (get) Token: 0x06003C9F RID: 15519 RVA: 0x0012125C File Offset: 0x0011F45C
	public bool isLoaded
	{
		get
		{
			return SceneManager.GetSceneByBuildIndex(this._buildIndex).isLoaded;
		}
	}

	// Token: 0x170005EC RID: 1516
	// (get) Token: 0x06003CA0 RID: 15520 RVA: 0x0012127C File Offset: 0x0011F47C
	public bool hasAlias
	{
		get
		{
			return !string.IsNullOrWhiteSpace(this._alias);
		}
	}

	// Token: 0x06003CA1 RID: 15521 RVA: 0x0012128C File Offset: 0x0011F48C
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

	// Token: 0x06003CA2 RID: 15522 RVA: 0x001212FD File Offset: 0x0011F4FD
	public override int GetHashCode()
	{
		return this._guid.GetHashCode();
	}

	// Token: 0x06003CA3 RID: 15523 RVA: 0x0012130A File Offset: 0x0011F50A
	public override string ToString()
	{
		return this.ToJson(false);
	}

	// Token: 0x06003CA4 RID: 15524 RVA: 0x00121313 File Offset: 0x0011F513
	public bool Equals(GTScene other)
	{
		return this._guid.Equals(other._guid) && this._name == other._name && this._path == other._path;
	}

	// Token: 0x06003CA5 RID: 15525 RVA: 0x00121350 File Offset: 0x0011F550
	public override bool Equals(object obj)
	{
		GTScene gtscene = obj as GTScene;
		return gtscene != null && this.Equals(gtscene);
	}

	// Token: 0x06003CA6 RID: 15526 RVA: 0x00121370 File Offset: 0x0011F570
	public static bool operator ==(GTScene x, GTScene y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003CA7 RID: 15527 RVA: 0x00121379 File Offset: 0x0011F579
	public static bool operator !=(GTScene x, GTScene y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06003CA8 RID: 15528 RVA: 0x00121385 File Offset: 0x0011F585
	public void LoadAsync()
	{
		if (this.isLoaded)
		{
			return;
		}
		SceneManager.LoadSceneAsync(this._buildIndex, LoadSceneMode.Additive);
	}

	// Token: 0x06003CA9 RID: 15529 RVA: 0x0012139D File Offset: 0x0011F59D
	public void UnloadAsync()
	{
		if (!this.isLoaded)
		{
			return;
		}
		SceneManager.UnloadSceneAsync(this._buildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
	}

	// Token: 0x06003CAA RID: 15530 RVA: 0x00045F91 File Offset: 0x00044191
	public static GTScene FromAsset(object sceneAsset)
	{
		return null;
	}

	// Token: 0x06003CAB RID: 15531 RVA: 0x00045F91 File Offset: 0x00044191
	public static GTScene From(object editorBuildSettingsScene)
	{
		return null;
	}

	// Token: 0x04004083 RID: 16515
	[SerializeField]
	private string _alias;

	// Token: 0x04004084 RID: 16516
	[SerializeField]
	private string _name;

	// Token: 0x04004085 RID: 16517
	[SerializeField]
	private string _path;

	// Token: 0x04004086 RID: 16518
	[SerializeField]
	private string _guid;

	// Token: 0x04004087 RID: 16519
	[SerializeField]
	private int _buildIndex;

	// Token: 0x04004088 RID: 16520
	[SerializeField]
	private bool _includeInBuild;
}
