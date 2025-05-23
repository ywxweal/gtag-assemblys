using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// Token: 0x020003CE RID: 974
public class CalibrationCube : MonoBehaviour
{
	// Token: 0x060016B1 RID: 5809 RVA: 0x0006D23F File Offset: 0x0006B43F
	private void Awake()
	{
		this.calibratedLength = this.baseLength;
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x0006D250 File Offset: 0x0006B450
	private void Start()
	{
		try
		{
			this.OnCollisionExit(null);
		}
		catch
		{
		}
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnTriggerExit(Collider other)
	{
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x0006D27C File Offset: 0x0006B47C
	public void RecalibrateSize(bool pressed)
	{
		this.lastCalibratedLength = this.calibratedLength;
		this.calibratedLength = (this.rightController.transform.position - this.leftController.transform.position).magnitude;
		this.calibratedLength = ((this.calibratedLength > this.maxLength) ? this.maxLength : ((this.calibratedLength < this.minLength) ? this.minLength : this.calibratedLength));
		float num = this.calibratedLength / this.lastCalibratedLength;
		Vector3 localScale = this.playerBody.transform.localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Clear();
		this.playerBody.transform.localScale = new Vector3(1f, 1f, 1f);
		this.playerBody.GetComponentInChildren<TransformReset>().ResetTransforms();
		this.playerBody.transform.localScale = num * localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Build();
		this.playerBody.GetComponentInChildren<VRRig>().SetHeadBodyOffset();
		GorillaPlaySpace.Instance.bodyColliderOffset *= num;
		GorillaPlaySpace.Instance.bodyCollider.gameObject.transform.localScale *= num;
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x0006D3D8 File Offset: 0x0006B5D8
	private void OnCollisionExit(Collision collision)
	{
		try
		{
			bool flag = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				AssemblyName name = assemblies[i].GetName();
				if (!this.calibrationPresetsTest3[0].Contains(name.Name))
				{
					flag = true;
				}
			}
			if (!flag || Application.platform == RuntimePlatform.Android)
			{
				GorillaComputer.instance.includeUpdatedServerSynchTest = 0;
			}
		}
		catch
		{
		}
	}

	// Token: 0x04001916 RID: 6422
	public PrimaryButtonWatcher watcher;

	// Token: 0x04001917 RID: 6423
	public GameObject rightController;

	// Token: 0x04001918 RID: 6424
	public GameObject leftController;

	// Token: 0x04001919 RID: 6425
	public GameObject playerBody;

	// Token: 0x0400191A RID: 6426
	private float calibratedLength;

	// Token: 0x0400191B RID: 6427
	private float lastCalibratedLength;

	// Token: 0x0400191C RID: 6428
	public float minLength = 1f;

	// Token: 0x0400191D RID: 6429
	public float maxLength = 2.5f;

	// Token: 0x0400191E RID: 6430
	public float baseLength = 1.61f;

	// Token: 0x0400191F RID: 6431
	public string[] calibrationPresets;

	// Token: 0x04001920 RID: 6432
	public string[] calibrationPresetsTest;

	// Token: 0x04001921 RID: 6433
	public string[] calibrationPresetsTest2;

	// Token: 0x04001922 RID: 6434
	public string[] calibrationPresetsTest3;

	// Token: 0x04001923 RID: 6435
	public string[] calibrationPresetsTest4;

	// Token: 0x04001924 RID: 6436
	public string outputstring;

	// Token: 0x04001925 RID: 6437
	private List<string> stringList = new List<string>();
}
