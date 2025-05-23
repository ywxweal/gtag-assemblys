using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000130 RID: 304
public class PlayerGameEventListener : MonoBehaviour
{
	// Token: 0x060007F7 RID: 2039 RVA: 0x0002C432 File Offset: 0x0002A632
	private void OnEnable()
	{
		this.SubscribeToEvents();
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0002C43A File Offset: 0x0002A63A
	private void OnDisable()
	{
		this.UnsubscribeFromEvents();
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0002C444 File Offset: 0x0002A644
	private void SubscribeToEvents()
	{
		switch (this.eventType)
		{
		case PlayerGameEvents.EventType.NONE:
			return;
		case PlayerGameEvents.EventType.GameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GameModeCompleteRound:
			PlayerGameEvents.OnGameModeCompleteRound += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GrabbedObject:
			PlayerGameEvents.OnGrabbedObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.DroppedObject:
			PlayerGameEvents.OnDroppedObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EatObject:
			PlayerGameEvents.OnEatObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.TapObject:
			PlayerGameEvents.OnTapObject += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.LaunchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerMoved:
			PlayerGameEvents.OnPlayerMoved += this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerSwam:
			PlayerGameEvents.OnPlayerSwam += this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.TriggerHandEfffect:
			PlayerGameEvents.OnTriggerHandEffect += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EnterLocation:
			PlayerGameEvents.OnEnterLocation += this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.MiscEvent:
			PlayerGameEvents.OnMiscEvent += this.OnGameEventTriggered;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0002C578 File Offset: 0x0002A778
	private void UnsubscribeFromEvents()
	{
		switch (this.eventType)
		{
		case PlayerGameEvents.EventType.NONE:
			return;
		case PlayerGameEvents.EventType.GameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GameModeCompleteRound:
			PlayerGameEvents.OnGameModeCompleteRound -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.GrabbedObject:
			PlayerGameEvents.OnGrabbedObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.DroppedObject:
			PlayerGameEvents.OnDroppedObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EatObject:
			PlayerGameEvents.OnEatObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.TapObject:
			PlayerGameEvents.OnTapObject -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.LaunchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerMoved:
			PlayerGameEvents.OnPlayerMoved -= this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.PlayerSwam:
			PlayerGameEvents.OnPlayerSwam -= this.OnGameMoveEventTriggered;
			return;
		case PlayerGameEvents.EventType.TriggerHandEfffect:
			PlayerGameEvents.OnTriggerHandEffect -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.EnterLocation:
			PlayerGameEvents.OnEnterLocation -= this.OnGameEventTriggered;
			return;
		case PlayerGameEvents.EventType.MiscEvent:
			PlayerGameEvents.OnMiscEvent -= this.OnGameEventTriggered;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002C6A9 File Offset: 0x0002A8A9
	private void OnGameMoveEventTriggered(float distance, float speed)
	{
		Debug.LogError("Movement events not supported - please implement");
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0002C6B8 File Offset: 0x0002A8B8
	public void OnGameEventTriggered(string eventName)
	{
		if (!string.IsNullOrEmpty(this.filter) && !eventName.StartsWith(this.filter))
		{
			return;
		}
		if (this._cooldownEnd > Time.time)
		{
			return;
		}
		this._cooldownEnd = Time.time + this.cooldown;
		UnityEvent unityEvent = this.onGameEvent;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04000986 RID: 2438
	[SerializeField]
	private PlayerGameEvents.EventType eventType;

	// Token: 0x04000987 RID: 2439
	[Tooltip("Cooldown in seconds")]
	[SerializeField]
	private string filter;

	// Token: 0x04000988 RID: 2440
	[SerializeField]
	private float cooldown = 1f;

	// Token: 0x04000989 RID: 2441
	[SerializeField]
	private UnityEvent onGameEvent;

	// Token: 0x0400098A RID: 2442
	private float _cooldownEnd;
}
