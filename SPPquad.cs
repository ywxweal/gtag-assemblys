using System;
using UnityEngine;

// Token: 0x02000348 RID: 840
public class SPPquad : MonoBehaviour
{
	// Token: 0x060013D2 RID: 5074 RVA: 0x00060198 File Offset: 0x0005E398
	private void Start()
	{
		this.passthroughLayer = base.GetComponent<OVRPassthroughLayer>();
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x00060223 File Offset: 0x0005E423
	public void Grab(OVRInput.Controller grabHand)
	{
		this.passthroughLayer.RemoveSurfaceGeometry(this.projectionObject.gameObject);
		this.controllerHand = grabHand;
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x00060242 File Offset: 0x0005E442
	public void Release()
	{
		this.controllerHand = OVRInput.Controller.None;
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
	}

	// Token: 0x04001604 RID: 5636
	private OVRPassthroughLayer passthroughLayer;

	// Token: 0x04001605 RID: 5637
	public MeshFilter projectionObject;

	// Token: 0x04001606 RID: 5638
	private OVRInput.Controller controllerHand;
}
