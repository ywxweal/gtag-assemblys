using System;

// Token: 0x02000132 RID: 306
public class PlayerGameEvents
{
	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06000800 RID: 2048 RVA: 0x0002C750 File Offset: 0x0002A950
	// (remove) Token: 0x06000801 RID: 2049 RVA: 0x0002C784 File Offset: 0x0002A984
	public static event Action<string> OnGameModeObjectiveTrigger;

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06000802 RID: 2050 RVA: 0x0002C7B8 File Offset: 0x0002A9B8
	// (remove) Token: 0x06000803 RID: 2051 RVA: 0x0002C7EC File Offset: 0x0002A9EC
	public static event Action<string> OnGameModeCompleteRound;

	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06000804 RID: 2052 RVA: 0x0002C820 File Offset: 0x0002AA20
	// (remove) Token: 0x06000805 RID: 2053 RVA: 0x0002C854 File Offset: 0x0002AA54
	public static event Action<string> OnGrabbedObject;

	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06000806 RID: 2054 RVA: 0x0002C888 File Offset: 0x0002AA88
	// (remove) Token: 0x06000807 RID: 2055 RVA: 0x0002C8BC File Offset: 0x0002AABC
	public static event Action<string> OnDroppedObject;

	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06000808 RID: 2056 RVA: 0x0002C8F0 File Offset: 0x0002AAF0
	// (remove) Token: 0x06000809 RID: 2057 RVA: 0x0002C924 File Offset: 0x0002AB24
	public static event Action<string> OnEatObject;

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x0600080A RID: 2058 RVA: 0x0002C958 File Offset: 0x0002AB58
	// (remove) Token: 0x0600080B RID: 2059 RVA: 0x0002C98C File Offset: 0x0002AB8C
	public static event Action<string> OnTapObject;

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x0600080C RID: 2060 RVA: 0x0002C9C0 File Offset: 0x0002ABC0
	// (remove) Token: 0x0600080D RID: 2061 RVA: 0x0002C9F4 File Offset: 0x0002ABF4
	public static event Action<string> OnLaunchedProjectile;

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x0600080E RID: 2062 RVA: 0x0002CA28 File Offset: 0x0002AC28
	// (remove) Token: 0x0600080F RID: 2063 RVA: 0x0002CA5C File Offset: 0x0002AC5C
	public static event Action<float, float> OnPlayerMoved;

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06000810 RID: 2064 RVA: 0x0002CA90 File Offset: 0x0002AC90
	// (remove) Token: 0x06000811 RID: 2065 RVA: 0x0002CAC4 File Offset: 0x0002ACC4
	public static event Action<float, float> OnPlayerSwam;

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x06000812 RID: 2066 RVA: 0x0002CAF8 File Offset: 0x0002ACF8
	// (remove) Token: 0x06000813 RID: 2067 RVA: 0x0002CB2C File Offset: 0x0002AD2C
	public static event Action<string> OnTriggerHandEffect;

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06000814 RID: 2068 RVA: 0x0002CB60 File Offset: 0x0002AD60
	// (remove) Token: 0x06000815 RID: 2069 RVA: 0x0002CB94 File Offset: 0x0002AD94
	public static event Action<string> OnEnterLocation;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06000816 RID: 2070 RVA: 0x0002CBC8 File Offset: 0x0002ADC8
	// (remove) Token: 0x06000817 RID: 2071 RVA: 0x0002CBFC File Offset: 0x0002ADFC
	public static event Action<string> OnMiscEvent;

	// Token: 0x1400001F RID: 31
	// (add) Token: 0x06000818 RID: 2072 RVA: 0x0002CC30 File Offset: 0x0002AE30
	// (remove) Token: 0x06000819 RID: 2073 RVA: 0x0002CC64 File Offset: 0x0002AE64
	public static event Action<string> OnCritterEvent;

	// Token: 0x0600081A RID: 2074 RVA: 0x0002CC98 File Offset: 0x0002AE98
	public static void GameModeObjectiveTriggered()
	{
		string text = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeObjectiveTrigger = PlayerGameEvents.OnGameModeObjectiveTrigger;
		if (onGameModeObjectiveTrigger == null)
		{
			return;
		}
		onGameModeObjectiveTrigger(text);
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0002CCC0 File Offset: 0x0002AEC0
	public static void GameModeCompleteRound()
	{
		string text = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeCompleteRound = PlayerGameEvents.OnGameModeCompleteRound;
		if (onGameModeCompleteRound == null)
		{
			return;
		}
		onGameModeCompleteRound(text);
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0002CCE8 File Offset: 0x0002AEE8
	public static void GrabbedObject(string objectName)
	{
		Action<string> onGrabbedObject = PlayerGameEvents.OnGrabbedObject;
		if (onGrabbedObject == null)
		{
			return;
		}
		onGrabbedObject(objectName);
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0002CCFA File Offset: 0x0002AEFA
	public static void DroppedObject(string objectName)
	{
		Action<string> onDroppedObject = PlayerGameEvents.OnDroppedObject;
		if (onDroppedObject == null)
		{
			return;
		}
		onDroppedObject(objectName);
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0002CD0C File Offset: 0x0002AF0C
	public static void EatObject(string objectName)
	{
		Action<string> onEatObject = PlayerGameEvents.OnEatObject;
		if (onEatObject == null)
		{
			return;
		}
		onEatObject(objectName);
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0002CD1E File Offset: 0x0002AF1E
	public static void TapObject(string objectName)
	{
		Action<string> onTapObject = PlayerGameEvents.OnTapObject;
		if (onTapObject == null)
		{
			return;
		}
		onTapObject(objectName);
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0002CD30 File Offset: 0x0002AF30
	public static void LaunchedProjectile(string objectName)
	{
		Action<string> onLaunchedProjectile = PlayerGameEvents.OnLaunchedProjectile;
		if (onLaunchedProjectile == null)
		{
			return;
		}
		onLaunchedProjectile(objectName);
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0002CD42 File Offset: 0x0002AF42
	public static void PlayerMoved(float distance, float speed)
	{
		Action<float, float> onPlayerMoved = PlayerGameEvents.OnPlayerMoved;
		if (onPlayerMoved == null)
		{
			return;
		}
		onPlayerMoved(distance, speed);
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0002CD55 File Offset: 0x0002AF55
	public static void PlayerSwam(float distance, float speed)
	{
		Action<float, float> onPlayerSwam = PlayerGameEvents.OnPlayerSwam;
		if (onPlayerSwam == null)
		{
			return;
		}
		onPlayerSwam(distance, speed);
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0002CD68 File Offset: 0x0002AF68
	public static void TriggerHandEffect(string effectName)
	{
		Action<string> onTriggerHandEffect = PlayerGameEvents.OnTriggerHandEffect;
		if (onTriggerHandEffect == null)
		{
			return;
		}
		onTriggerHandEffect(effectName);
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0002CD7A File Offset: 0x0002AF7A
	public static void TriggerEnterLocation(string locationName)
	{
		Action<string> onEnterLocation = PlayerGameEvents.OnEnterLocation;
		if (onEnterLocation == null)
		{
			return;
		}
		onEnterLocation(locationName);
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0002CD8C File Offset: 0x0002AF8C
	public static void MiscEvent(string eventName)
	{
		Action<string> onMiscEvent = PlayerGameEvents.OnMiscEvent;
		if (onMiscEvent == null)
		{
			return;
		}
		onMiscEvent(eventName);
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0002CD9E File Offset: 0x0002AF9E
	public static void CritterEvent(string eventName)
	{
		Action<string> onCritterEvent = PlayerGameEvents.OnCritterEvent;
		if (onCritterEvent == null)
		{
			return;
		}
		onCritterEvent(eventName);
	}

	// Token: 0x02000133 RID: 307
	public enum EventType
	{
		// Token: 0x0400099A RID: 2458
		NONE,
		// Token: 0x0400099B RID: 2459
		GameModeObjective,
		// Token: 0x0400099C RID: 2460
		GameModeCompleteRound,
		// Token: 0x0400099D RID: 2461
		GrabbedObject,
		// Token: 0x0400099E RID: 2462
		DroppedObject,
		// Token: 0x0400099F RID: 2463
		EatObject,
		// Token: 0x040009A0 RID: 2464
		TapObject,
		// Token: 0x040009A1 RID: 2465
		LaunchedProjectile,
		// Token: 0x040009A2 RID: 2466
		PlayerMoved,
		// Token: 0x040009A3 RID: 2467
		PlayerSwam,
		// Token: 0x040009A4 RID: 2468
		TriggerHandEfffect,
		// Token: 0x040009A5 RID: 2469
		EnterLocation,
		// Token: 0x040009A6 RID: 2470
		MiscEvent
	}
}
