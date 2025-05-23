using System;
using UnityEngine;

// Token: 0x020006F9 RID: 1785
public class CustomMapAccessDoor : MonoBehaviour
{
	// Token: 0x06002C6F RID: 11375 RVA: 0x000DB4E2 File Offset: 0x000D96E2
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

	// Token: 0x06002C70 RID: 11376 RVA: 0x000DB518 File Offset: 0x000D9718
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

	// Token: 0x040032C1 RID: 12993
	public GameObject openDoorObject;

	// Token: 0x040032C2 RID: 12994
	public GameObject closedDoorObject;
}
