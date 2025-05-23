using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000738 RID: 1848
public abstract class CustomMapsTerminalScreen : MonoBehaviour
{
	// Token: 0x06002E30 RID: 11824
	public abstract void Initialize();

	// Token: 0x06002E31 RID: 11825 RVA: 0x000E6C63 File Offset: 0x000E4E63
	public virtual void Show()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
			GameEvents.OnModIOKeyboardButtonPressedEvent.AddListener(new UnityAction<CustomMapsTerminalButton.ModIOKeyboardBindings>(this.PressButton));
		}
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x000E6C95 File Offset: 0x000E4E95
	public virtual void Hide()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
			GameEvents.OnModIOKeyboardButtonPressedEvent.RemoveListener(new UnityAction<CustomMapsTerminalButton.ModIOKeyboardBindings>(this.PressButton));
		}
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void PressButton(CustomMapsTerminalButton.ModIOKeyboardBindings button)
	{
	}
}
