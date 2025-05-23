using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000738 RID: 1848
public abstract class CustomMapsTerminalScreen : MonoBehaviour
{
	// Token: 0x06002E2F RID: 11823
	public abstract void Initialize();

	// Token: 0x06002E30 RID: 11824 RVA: 0x000E6BBF File Offset: 0x000E4DBF
	public virtual void Show()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
			GameEvents.OnModIOKeyboardButtonPressedEvent.AddListener(new UnityAction<CustomMapsTerminalButton.ModIOKeyboardBindings>(this.PressButton));
		}
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x000E6BF1 File Offset: 0x000E4DF1
	public virtual void Hide()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
			GameEvents.OnModIOKeyboardButtonPressedEvent.RemoveListener(new UnityAction<CustomMapsTerminalButton.ModIOKeyboardBindings>(this.PressButton));
		}
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void PressButton(CustomMapsTerminalButton.ModIOKeyboardBindings button)
	{
	}
}
