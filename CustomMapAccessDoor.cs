using System;
using UnityEngine;

// Token: 0x020006F9 RID: 1785
public class CustomMapAccessDoor : MonoBehaviour
{
	// Token: 0x06002C70 RID: 11376 RVA: 0x000DB586 File Offset: 0x000D9786
	public void OpenDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(true);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(false);
		}
	}

	// Token: 0x06002C71 RID: 11377 RVA: 0x000DB5BC File Offset: 0x000D97BC
	public void CloseDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(false);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(true);
		}
	}

	// Token: 0x040032C3 RID: 12995
	public GameObject openDoorObject;

	// Token: 0x040032C4 RID: 12996
	public GameObject closedDoorObject;
}
