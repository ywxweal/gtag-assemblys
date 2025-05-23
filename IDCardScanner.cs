using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000D0 RID: 208
public class IDCardScanner : MonoBehaviour
{
	// Token: 0x0600051F RID: 1311 RVA: 0x0001D648 File Offset: 0x0001B848
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<ScannableIDCard>() != null)
		{
			UnityEvent unityEvent = this.onCardSwiped;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			GameEntity gameEntity = other.GetComponent<GameEntity>();
			if (gameEntity == null && other.attachedRigidbody != null)
			{
				gameEntity = other.attachedRigidbody.GetComponent<GameEntity>();
			}
			if (gameEntity != null && gameEntity.heldByActorNumber != -1)
			{
				UnityEvent<int> unityEvent2 = this.onCardSwipedByPlayer;
				if (unityEvent2 == null)
				{
					return;
				}
				unityEvent2.Invoke(gameEntity.heldByActorNumber);
			}
		}
	}

	// Token: 0x040005F2 RID: 1522
	public UnityEvent onCardSwiped;

	// Token: 0x040005F3 RID: 1523
	public UnityEvent<int> onCardSwipedByPlayer;

	// Token: 0x040005F4 RID: 1524
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onSucceeded;

	// Token: 0x040005F5 RID: 1525
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onFailed;
}
