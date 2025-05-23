using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class GameModeSpecificObjectRegistry : MonoBehaviour
{
	// Token: 0x060003AF RID: 943 RVA: 0x000169B6 File Offset: 0x00014BB6
	private void OnEnable()
	{
		GameModeSpecificObject.OnAwake += this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed += this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode += this.GameMode_OnStartGameMode;
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x000169EB File Offset: 0x00014BEB
	private void OnDisable()
	{
		GameModeSpecificObject.OnAwake -= this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed -= this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode -= this.GameMode_OnStartGameMode;
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x00016A20 File Offset: 0x00014C20
	private void GameModeSpecificObject_OnAwake(GameModeSpecificObject obj)
	{
		foreach (GameModeType gameModeType in obj.GameModes)
		{
			if (!this.gameModeSpecificObjects.ContainsKey(gameModeType))
			{
				this.gameModeSpecificObjects.Add(gameModeType, new List<GameModeSpecificObject>());
			}
			this.gameModeSpecificObjects[gameModeType].Add(obj);
		}
		if (GameMode.ActiveGameMode == null)
		{
			obj.gameObject.SetActive(obj.Validation == GameModeSpecificObject.ValidationMethod.Exclusion);
			return;
		}
		obj.gameObject.SetActive(obj.CheckValid(GameMode.ActiveGameMode.GameType()));
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x00016ADC File Offset: 0x00014CDC
	private void GameModeSpecificObject_OnDestroyed(GameModeSpecificObject obj)
	{
		foreach (GameModeType gameModeType in obj.GameModes)
		{
			if (this.gameModeSpecificObjects.ContainsKey(gameModeType))
			{
				this.gameModeSpecificObjects[gameModeType].Remove(obj);
			}
		}
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00016B4C File Offset: 0x00014D4C
	private void GameMode_OnStartGameMode(GameModeType newGameModeType)
	{
		if (this.currentGameType == newGameModeType)
		{
			return;
		}
		if (this.gameModeSpecificObjects.ContainsKey(this.currentGameType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject in this.gameModeSpecificObjects[this.currentGameType])
			{
				gameModeSpecificObject.gameObject.SetActive(gameModeSpecificObject.CheckValid(newGameModeType));
			}
		}
		if (this.gameModeSpecificObjects.ContainsKey(newGameModeType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject2 in this.gameModeSpecificObjects[newGameModeType])
			{
				gameModeSpecificObject2.gameObject.SetActive(gameModeSpecificObject2.CheckValid(newGameModeType));
			}
		}
		this.currentGameType = newGameModeType;
	}

	// Token: 0x0400042D RID: 1069
	private Dictionary<GameModeType, List<GameModeSpecificObject>> gameModeSpecificObjects = new Dictionary<GameModeType, List<GameModeSpecificObject>>();

	// Token: 0x0400042E RID: 1070
	private GameModeType currentGameType = GameModeType.Count;
}
