using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class BrushController : MonoBehaviour
{
	// Token: 0x0600133E RID: 4926 RVA: 0x0005BE7C File Offset: 0x0005A07C
	private void Start()
	{
		this.brush.controllerHand = OVRInput.Controller.None;
		if (!this.brush.lineContainer)
		{
			this.brush.lineContainer = new GameObject("LineContainer");
		}
		this.backgroundSphere.material.renderQueue = 3998;
		this.backgroundSphere.transform.parent = null;
		this.backgroundSphere.enabled = false;
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x0005BF49 File Offset: 0x0005A149
	private void Update()
	{
		this.backgroundSphere.transform.position = Camera.main.transform.position;
	}

	// Token: 0x06001340 RID: 4928 RVA: 0x0005BF6C File Offset: 0x0005A16C
	public void Grab(OVRInput.Controller grabHand)
	{
		this.brush.controllerHand = grabHand;
		this.brush.lineContainer.SetActive(true);
		this.backgroundSphere.enabled = true;
		if (this.grabRoutine != null)
		{
			base.StopCoroutine(this.grabRoutine);
		}
		if (this.releaseRoutine != null)
		{
			base.StopCoroutine(this.releaseRoutine);
		}
		this.grabRoutine = this.FadeSphere(Color.grey, 0.25f, false);
		base.StartCoroutine(this.grabRoutine);
	}

	// Token: 0x06001341 RID: 4929 RVA: 0x0005BFF0 File Offset: 0x0005A1F0
	public void Release()
	{
		this.brush.controllerHand = OVRInput.Controller.None;
		this.brush.lineContainer.SetActive(false);
		if (this.grabRoutine != null)
		{
			base.StopCoroutine(this.grabRoutine);
		}
		if (this.releaseRoutine != null)
		{
			base.StopCoroutine(this.releaseRoutine);
		}
		this.releaseRoutine = this.FadeSphere(new Color(0.5f, 0.5f, 0.5f, 0f), 0.25f, true);
		base.StartCoroutine(this.releaseRoutine);
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x0005C07A File Offset: 0x0005A27A
	private IEnumerator FadeCameraClearColor(Color newColor, float fadeTime)
	{
		float timer = 0f;
		Color currentColor = Camera.main.backgroundColor;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float num = Mathf.Clamp01(timer / fadeTime);
			Camera.main.backgroundColor = Color.Lerp(currentColor, newColor, num);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x0005C090 File Offset: 0x0005A290
	private IEnumerator FadeSphere(Color newColor, float fadeTime, bool disableOnFinish = false)
	{
		float timer = 0f;
		Color currentColor = this.backgroundSphere.material.GetColor("_Color");
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float num = Mathf.Clamp01(timer / fadeTime);
			this.backgroundSphere.material.SetColor("_Color", Color.Lerp(currentColor, newColor, num));
			if (disableOnFinish && timer >= fadeTime)
			{
				this.backgroundSphere.enabled = false;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400155E RID: 5470
	public PassthroughBrush brush;

	// Token: 0x0400155F RID: 5471
	public MeshRenderer backgroundSphere;

	// Token: 0x04001560 RID: 5472
	private IEnumerator grabRoutine;

	// Token: 0x04001561 RID: 5473
	private IEnumerator releaseRoutine;
}
