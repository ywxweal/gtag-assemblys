using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200073B RID: 1851
public class VirtualStumpBarrierSFX : MonoBehaviour
{
	// Token: 0x06002E38 RID: 11832 RVA: 0x000E6CFC File Offset: 0x000E4EFC
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.PlaySFX();
			return;
		}
		VRRig vrrig;
		if (other.gameObject.TryGetComponent<VRRig>(out vrrig) && !vrrig.isLocal)
		{
			bool flag = other.gameObject.transform.position.z < base.gameObject.transform.position.z;
			this.trackedGameObjects.Add(other.gameObject, flag);
			this.OnTriggerStay(other);
		}
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x000E6D8C File Offset: 0x000E4F8C
	public void OnTriggerStay(Collider other)
	{
		bool flag;
		if (!this.trackedGameObjects.TryGetValue(other.gameObject, out flag))
		{
			return;
		}
		bool flag2 = other.gameObject.transform.position.z < base.gameObject.transform.position.z;
		if (flag != flag2)
		{
			this.PlaySFX();
			this.trackedGameObjects.Remove(other.gameObject);
		}
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x000E6DF8 File Offset: 0x000E4FF8
	public void OnTriggerExit(Collider other)
	{
		bool flag;
		if (this.trackedGameObjects.TryGetValue(other.gameObject, out flag))
		{
			bool flag2 = other.gameObject.transform.position.z < base.gameObject.transform.position.z;
			if (flag != flag2)
			{
				this.PlaySFX();
			}
			this.trackedGameObjects.Remove(other.gameObject);
		}
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x000E6E64 File Offset: 0x000E5064
	public void PlaySFX()
	{
		if (this.barrierAudioSource.IsNull())
		{
			return;
		}
		if (this.PassThroughBarrierSoundClips.IsNullOrEmpty<AudioClip>())
		{
			return;
		}
		this.barrierAudioSource.clip = this.PassThroughBarrierSoundClips[Random.Range(0, this.PassThroughBarrierSoundClips.Count)];
		this.barrierAudioSource.Play();
	}

	// Token: 0x040034CD RID: 13517
	[SerializeField]
	private AudioSource barrierAudioSource;

	// Token: 0x040034CE RID: 13518
	[FormerlySerializedAs("teleportingPlayerSoundClips")]
	[SerializeField]
	private List<AudioClip> PassThroughBarrierSoundClips = new List<AudioClip>();

	// Token: 0x040034CF RID: 13519
	private Dictionary<GameObject, bool> trackedGameObjects = new Dictionary<GameObject, bool>();
}
