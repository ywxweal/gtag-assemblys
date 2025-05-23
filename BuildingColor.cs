using System;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class BuildingColor : MonoBehaviour
{
	// Token: 0x060004E2 RID: 1250 RVA: 0x0001C6C0 File Offset: 0x0001A8C0
	private void Start()
	{
		Renderer component = base.GetComponent<Renderer>();
		Color color = new Color(this.Red, this.Green, this.Blue, 1f);
		component.material.SetColor("_Color", color);
	}

	// Token: 0x040005C4 RID: 1476
	[Range(0f, 178f)]
	public float Red;

	// Token: 0x040005C5 RID: 1477
	[Range(0f, 178f)]
	public float Green;

	// Token: 0x040005C6 RID: 1478
	[Range(0f, 178f)]
	public float Blue;
}
