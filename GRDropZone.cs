using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200059F RID: 1439
public class GRDropZone : MonoBehaviour
{
	// Token: 0x0600231F RID: 8991 RVA: 0x000AFA4E File Offset: 0x000ADC4E
	private void Awake()
	{
		this.repelDirectionWorld = base.transform.TransformDirection(this.repelDirectionLocal.normalized);
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000AFA6C File Offset: 0x000ADC6C
	private void OnTriggerEnter(Collider other)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		GameEntity component = other.attachedRigidbody.GetComponent<GameEntity>();
		if (component != null)
		{
			GhostReactorManager.instance.EntityEnteredDropZone(component);
		}
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000AFAA1 File Offset: 0x000ADCA1
	public Vector3 GetRepelDirectionWorld()
	{
		return this.repelDirectionWorld;
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x000AFAAC File Offset: 0x000ADCAC
	public void PlayEffect()
	{
		if (this.vfxRoot != null && !this.playingEffect)
		{
			this.vfxRoot.SetActive(true);
			this.playingEffect = true;
			if (this.sfxPrefab != null)
			{
				ObjectPools.instance.Instantiate(this.sfxPrefab, base.transform.position, base.transform.rotation, true);
			}
			base.StartCoroutine(this.DelayedStopEffect());
		}
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x000AFB25 File Offset: 0x000ADD25
	private IEnumerator DelayedStopEffect()
	{
		yield return new WaitForSeconds(this.effectDuration);
		this.vfxRoot.SetActive(false);
		this.playingEffect = false;
		yield break;
	}

	// Token: 0x04002758 RID: 10072
	[SerializeField]
	private GameObject vfxRoot;

	// Token: 0x04002759 RID: 10073
	[SerializeField]
	private GameObject sfxPrefab;

	// Token: 0x0400275A RID: 10074
	public float effectDuration = 1f;

	// Token: 0x0400275B RID: 10075
	private bool playingEffect;

	// Token: 0x0400275C RID: 10076
	[SerializeField]
	private Vector3 repelDirectionLocal = Vector3.up;

	// Token: 0x0400275D RID: 10077
	private Vector3 repelDirectionWorld = Vector3.up;
}
