using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000314 RID: 788
public class ButtonDownListener : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	// Token: 0x1400003F RID: 63
	// (add) Token: 0x060012C6 RID: 4806 RVA: 0x000584BC File Offset: 0x000566BC
	// (remove) Token: 0x060012C7 RID: 4807 RVA: 0x000584F4 File Offset: 0x000566F4
	public event Action onButtonDown;

	// Token: 0x060012C8 RID: 4808 RVA: 0x00058529 File Offset: 0x00056729
	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onButtonDown != null)
		{
			this.onButtonDown();
		}
	}
}
