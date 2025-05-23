using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000349 RID: 841
public class BouncingBallLogic : MonoBehaviour
{
	// Token: 0x060013D6 RID: 5078 RVA: 0x00060262 File Offset: 0x0005E462
	private void OnCollisionEnter()
	{
		this.audioSource.PlayOneShot(this.bounce);
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x00060275 File Offset: 0x0005E475
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.PlayOneShot(this.loadball);
		this.centerEyeCamera = OVRManager.instance.GetComponentInChildren<OVRCameraRig>().centerEyeAnchor;
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x000602AC File Offset: 0x0005E4AC
	private void Update()
	{
		if (!this.isReleased)
		{
			return;
		}
		this.UpdateVisibility();
		this.timer += Time.deltaTime;
		if (!this.isReadyForDestroy && this.timer >= this.TTL)
		{
			this.isReadyForDestroy = true;
			float length = this.pop.length;
			this.audioSource.PlayOneShot(this.pop);
			base.StartCoroutine(this.PlayPopCallback(length));
		}
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x00060324 File Offset: 0x0005E524
	private void UpdateVisibility()
	{
		Vector3 vector = this.centerEyeCamera.position - base.transform.position;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position, vector), out raycastHit, vector.magnitude))
		{
			if (raycastHit.collider.gameObject != base.gameObject)
			{
				this.SetVisible(false);
				return;
			}
		}
		else
		{
			this.SetVisible(true);
		}
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x00060398 File Offset: 0x0005E598
	private void SetVisible(bool setVisible)
	{
		if (this.isVisible && !setVisible)
		{
			base.GetComponent<MeshRenderer>().material = this.hiddenMat;
			this.isVisible = false;
		}
		if (!this.isVisible && setVisible)
		{
			base.GetComponent<MeshRenderer>().material = this.visibleMat;
			this.isVisible = true;
		}
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x000603ED File Offset: 0x0005E5ED
	public void Release(Vector3 pos, Vector3 vel, Vector3 angVel)
	{
		this.isReleased = true;
		base.transform.position = pos;
		base.GetComponent<Rigidbody>().isKinematic = false;
		base.GetComponent<Rigidbody>().velocity = vel;
		base.GetComponent<Rigidbody>().angularVelocity = angVel;
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x00060426 File Offset: 0x0005E626
	private IEnumerator PlayPopCallback(float clipLength)
	{
		yield return new WaitForSeconds(clipLength);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04001607 RID: 5639
	[SerializeField]
	private float TTL = 5f;

	// Token: 0x04001608 RID: 5640
	[SerializeField]
	private AudioClip pop;

	// Token: 0x04001609 RID: 5641
	[SerializeField]
	private AudioClip bounce;

	// Token: 0x0400160A RID: 5642
	[SerializeField]
	private AudioClip loadball;

	// Token: 0x0400160B RID: 5643
	[SerializeField]
	private Material visibleMat;

	// Token: 0x0400160C RID: 5644
	[SerializeField]
	private Material hiddenMat;

	// Token: 0x0400160D RID: 5645
	private AudioSource audioSource;

	// Token: 0x0400160E RID: 5646
	private Transform centerEyeCamera;

	// Token: 0x0400160F RID: 5647
	private bool isVisible = true;

	// Token: 0x04001610 RID: 5648
	private float timer;

	// Token: 0x04001611 RID: 5649
	private bool isReleased;

	// Token: 0x04001612 RID: 5650
	private bool isReadyForDestroy;
}
