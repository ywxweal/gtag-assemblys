using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000462 RID: 1122
[RequireComponent(typeof(OnTriggerEventsCosmetic))]
public class OnTriggerEventsHandlerCosmetic : MonoBehaviour
{
	// Token: 0x06001B8F RID: 7055 RVA: 0x0008752E File Offset: 0x0008572E
	public void OnTriggerEntered()
	{
		if (this.toggleOnceOnly && this.triggerEntered)
		{
			return;
		}
		this.triggerEntered = true;
		UnityEvent<OnTriggerEventsHandlerCosmetic> unityEvent = this.onTriggerEntered;
		if (unityEvent != null)
		{
			unityEvent.Invoke(this);
		}
		this.ToggleEffects();
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x00087560 File Offset: 0x00085760
	public void ToggleEffects()
	{
		if (this.particleToPlay)
		{
			this.particleToPlay.Play();
		}
		if (this.soundBankPlayer)
		{
			this.soundBankPlayer.Play();
		}
		if (this.destroyOnTriggerEnter)
		{
			if (this.destroyDelay > 0f)
			{
				base.Invoke("Destroy", this.destroyDelay);
				return;
			}
			this.Destroy();
		}
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x000875CA File Offset: 0x000857CA
	private void Destroy()
	{
		this.triggerEntered = false;
		if (ObjectPools.instance.DoesPoolExist(base.gameObject))
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001E8E RID: 7822
	[SerializeField]
	private ParticleSystem particleToPlay;

	// Token: 0x04001E8F RID: 7823
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04001E90 RID: 7824
	[SerializeField]
	private bool destroyOnTriggerEnter;

	// Token: 0x04001E91 RID: 7825
	[SerializeField]
	private float destroyDelay = 1f;

	// Token: 0x04001E92 RID: 7826
	[SerializeField]
	private bool toggleOnceOnly;

	// Token: 0x04001E93 RID: 7827
	[HideInInspector]
	public UnityEvent<OnTriggerEventsHandlerCosmetic> onTriggerEntered;

	// Token: 0x04001E94 RID: 7828
	private bool triggerEntered;
}
