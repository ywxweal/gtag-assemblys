using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200065F RID: 1631
public class HauntedObject : MonoBehaviour
{
	// Token: 0x060028C5 RID: 10437 RVA: 0x000CB1A4 File Offset: 0x000C93A4
	private void Awake()
	{
		this.lurkerGhost = GameObject.FindGameObjectWithTag("LurkerGhost");
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.wanderingGhost = GameObject.FindGameObjectWithTag("WanderingGhost");
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.animators = base.transform.GetComponentsInChildren<Animator>();
	}

	// Token: 0x060028C6 RID: 10438 RVA: 0x000CB260 File Offset: 0x000C9460
	private void OnDestroy()
	{
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
	}

	// Token: 0x060028C7 RID: 10439 RVA: 0x000CB2EB File Offset: 0x000C94EB
	private void Start()
	{
		this.initialPos = base.transform.position;
		this.passedTime = 0f;
		this.lightPassedTime = 0f;
	}

	// Token: 0x060028C8 RID: 10440 RVA: 0x000CB314 File Offset: 0x000C9514
	private void TriggerEffects(GameObject go)
	{
		if (base.gameObject != go)
		{
			return;
		}
		if (this.rattle)
		{
			base.StartCoroutine("Shake");
		}
		if (this.audioSource && this.hauntedSound)
		{
			this.audioSource.GTPlayOneShot(this.hauntedSound, 1f);
		}
		if (this.FBXprefab)
		{
			ObjectPools.instance.Instantiate(this.FBXprefab, base.transform.position, true);
		}
		if (this.TurnOffLight != null)
		{
			base.StartCoroutine("TurnOff");
		}
		foreach (Animator animator in this.animators)
		{
			if (animator)
			{
				animator.SetTrigger("Haunted");
			}
		}
	}

	// Token: 0x060028C9 RID: 10441 RVA: 0x000CB3E4 File Offset: 0x000C95E4
	private IEnumerator Shake()
	{
		while (this.passedTime < this.duration)
		{
			this.passedTime += Time.deltaTime;
			base.transform.position = new Vector3(this.initialPos.x + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.y + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.z);
			yield return null;
		}
		this.passedTime = 0f;
		yield break;
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x000CB3F3 File Offset: 0x000C95F3
	private IEnumerator TurnOff()
	{
		this.TurnOffLight.gameObject.SetActive(false);
		while (this.lightPassedTime < this.TurnOffDuration)
		{
			this.lightPassedTime += Time.deltaTime;
			yield return null;
		}
		this.TurnOffLight.SetActive(true);
		this.lightPassedTime = 0f;
		yield break;
	}

	// Token: 0x04002DC2 RID: 11714
	[Tooltip("If this box is checked, then object will rattle when hunted")]
	public bool rattle;

	// Token: 0x04002DC3 RID: 11715
	public float speed = 60f;

	// Token: 0x04002DC4 RID: 11716
	public float amount = 0.01f;

	// Token: 0x04002DC5 RID: 11717
	public float duration = 1f;

	// Token: 0x04002DC6 RID: 11718
	[FormerlySerializedAs("FBX")]
	public GameObject FBXprefab;

	// Token: 0x04002DC7 RID: 11719
	[Tooltip("Use to turn off a game object like candle flames when hunted")]
	public GameObject TurnOffLight;

	// Token: 0x04002DC8 RID: 11720
	public float TurnOffDuration = 2f;

	// Token: 0x04002DC9 RID: 11721
	private Vector3 initialPos;

	// Token: 0x04002DCA RID: 11722
	private float passedTime;

	// Token: 0x04002DCB RID: 11723
	private float lightPassedTime;

	// Token: 0x04002DCC RID: 11724
	private GameObject lurkerGhost;

	// Token: 0x04002DCD RID: 11725
	private GameObject wanderingGhost;

	// Token: 0x04002DCE RID: 11726
	private Animator[] animators;

	// Token: 0x04002DCF RID: 11727
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002DD0 RID: 11728
	[FormerlySerializedAs("rattlingSound")]
	public AudioClip hauntedSound;
}
