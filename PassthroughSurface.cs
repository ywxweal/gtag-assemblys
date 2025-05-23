using System;
using UnityEngine;

// Token: 0x02000345 RID: 837
public class PassthroughSurface : MonoBehaviour
{
	// Token: 0x060013CB RID: 5067 RVA: 0x0005FFB7 File Offset: 0x0005E1B7
	private void Start()
	{
		Object.Destroy(this.projectionObject.GetComponent<MeshRenderer>());
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, true);
	}

	// Token: 0x040015FE RID: 5630
	public OVRPassthroughLayer passthroughLayer;

	// Token: 0x040015FF RID: 5631
	public MeshFilter projectionObject;
}
