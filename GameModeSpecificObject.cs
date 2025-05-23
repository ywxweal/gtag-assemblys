using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class GameModeSpecificObject : MonoBehaviour
{
	// Token: 0x14000007 RID: 7
	// (add) Token: 0x0600039F RID: 927 RVA: 0x00016778 File Offset: 0x00014978
	// (remove) Token: 0x060003A0 RID: 928 RVA: 0x000167AC File Offset: 0x000149AC
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnAwake;

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x060003A1 RID: 929 RVA: 0x000167E0 File Offset: 0x000149E0
	// (remove) Token: 0x060003A2 RID: 930 RVA: 0x00016814 File Offset: 0x00014A14
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnDestroyed;

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060003A3 RID: 931 RVA: 0x00016847 File Offset: 0x00014A47
	public GameModeSpecificObject.ValidationMethod Validation
	{
		get
		{
			return this.validationMethod;
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060003A4 RID: 932 RVA: 0x0001684F File Offset: 0x00014A4F
	public List<GameModeType> GameModes
	{
		get
		{
			return this.gameModes;
		}
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x00016858 File Offset: 0x00014A58
	private async void Awake()
	{
		this.gameModes = new List<GameModeType>(this._gameModes);
		await Task.Yield();
		if (GameModeSpecificObject.OnAwake != null)
		{
			GameModeSpecificObject.OnAwake(this);
		}
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x0001688F File Offset: 0x00014A8F
	private void OnDestroy()
	{
		if (GameModeSpecificObject.OnDestroyed != null)
		{
			GameModeSpecificObject.OnDestroyed(this);
		}
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x000168A3 File Offset: 0x00014AA3
	public bool CheckValid(GameModeType gameMode)
	{
		if (this.validationMethod == GameModeSpecificObject.ValidationMethod.Exclusion)
		{
			return !this.gameModes.Contains(gameMode);
		}
		return this.gameModes.Contains(gameMode);
	}

	// Token: 0x04000423 RID: 1059
	[SerializeField]
	private GameModeSpecificObject.ValidationMethod validationMethod;

	// Token: 0x04000424 RID: 1060
	[SerializeField]
	private GameModeType[] _gameModes;

	// Token: 0x04000425 RID: 1061
	private List<GameModeType> gameModes;

	// Token: 0x02000091 RID: 145
	// (Invoke) Token: 0x060003AA RID: 938
	public delegate void GameModeSpecificObjectDelegate(GameModeSpecificObject gameModeSpecificObject);

	// Token: 0x02000092 RID: 146
	[Serializable]
	public enum ValidationMethod
	{
		// Token: 0x04000427 RID: 1063
		Inclusion,
		// Token: 0x04000428 RID: 1064
		Exclusion
	}
}
