using System;
using UnityEngine;

// Token: 0x020001F5 RID: 501
public class PlantablePoint : MonoBehaviour
{
	// Token: 0x06000BA3 RID: 2979 RVA: 0x0003E1A2 File Offset: 0x0003C3A2
	private void OnTriggerEnter(Collider other)
	{
		if ((this.floorMask & (1 << other.gameObject.layer)) != 0)
		{
			this.plantableObject.SetPlanted(true);
		}
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x0003E1CE File Offset: 0x0003C3CE
	public void OnTriggerExit(Collider other)
	{
		if ((this.floorMask & (1 << other.gameObject.layer)) != 0)
		{
			this.plantableObject.SetPlanted(false);
		}
	}

	// Token: 0x04000E32 RID: 3634
	public bool shouldBeSet;

	// Token: 0x04000E33 RID: 3635
	public LayerMask floorMask;

	// Token: 0x04000E34 RID: 3636
	public PlantableObject plantableObject;
}
