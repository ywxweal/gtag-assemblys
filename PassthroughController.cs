﻿using System;
using UnityEngine;

// Token: 0x0200033F RID: 831
public class PassthroughController : MonoBehaviour
{
	// Token: 0x0600139E RID: 5022 RVA: 0x0005F5E4 File Offset: 0x0005D7E4
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
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x0005F634 File Offset: 0x0005D834
	private void Update()
	{
		Color color = Color.HSVToRGB(Time.time * 0.1f % 1f, 1f, 1f);
		this.passthroughLayer.edgeColor = color;
		float num = Mathf.Sin(Time.time);
		this.passthroughLayer.SetColorMapControls(num, 0f, 0f, null, OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor);
		base.transform.position = Camera.main.transform.position;
		base.transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized);
	}

	// Token: 0x040015D4 RID: 5588
	private OVRPassthroughLayer passthroughLayer;
}
