using System;
using UnityEngine;

// Token: 0x02000340 RID: 832
public class PassthroughProjectionSurface : MonoBehaviour
{
	// Token: 0x060013A1 RID: 5025 RVA: 0x0005F6F8 File Offset: 0x0005D8F8
	private void Start()
	{
		GameObject gameObject = GameObject.Find("OVRCameraRig");
		if (gameObject == null)
		{
			Debug.LogError("Scene does not contain an OVRCameraRig");
			return;
		}
		this.passthroughLayer = gameObject.GetComponent<OVRPassthroughLayer>();
		if (this.passthroughLayer == null)
		{
			Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
		}
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, true);
		this.quadOutline = this.projectionObject.GetComponent<MeshRenderer>();
		this.quadOutline.enabled = false;
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x0005F77C File Offset: 0x0005D97C
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			this.passthroughLayer.RemoveSurfaceGeometry(this.projectionObject.gameObject);
			this.quadOutline.enabled = true;
		}
		if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			OVRInput.Controller controller = OVRInput.Controller.RTouch;
			base.transform.position = OVRInput.GetLocalControllerPosition(controller);
			base.transform.rotation = OVRInput.GetLocalControllerRotation(controller);
		}
		if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
			this.quadOutline.enabled = false;
		}
	}

	// Token: 0x040015D5 RID: 5589
	private OVRPassthroughLayer passthroughLayer;

	// Token: 0x040015D6 RID: 5590
	public MeshFilter projectionObject;

	// Token: 0x040015D7 RID: 5591
	private MeshRenderer quadOutline;
}
