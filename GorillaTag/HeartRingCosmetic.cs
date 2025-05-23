using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000CFE RID: 3326
	[DefaultExecutionOrder(1250)]
	public class HeartRingCosmetic : MonoBehaviour
	{
		// Token: 0x0600537A RID: 21370 RVA: 0x00195517 File Offset: 0x00193717
		protected void Awake()
		{
			Application.quitting += delegate
			{
				base.enabled = false;
			};
		}

		// Token: 0x0600537B RID: 21371 RVA: 0x0019552C File Offset: 0x0019372C
		protected void OnEnable()
		{
			this.particleSystem = this.effects.GetComponentInChildren<ParticleSystem>(true);
			this.audioSource = this.effects.GetComponentInChildren<AudioSource>(true);
			this.ownerRig = base.GetComponentInParent<VRRig>();
			bool flag = this.ownerRig != null && this.ownerRig.head != null && this.ownerRig.head.rigTarget != null;
			base.enabled = flag;
			this.effects.SetActive(flag);
			if (!flag)
			{
				Debug.LogError("Disabling HeartRingCosmetic. Could not find owner head. Scene path: " + base.transform.GetPath(), this);
				return;
			}
			this.ownerHead = ((this.ownerRig != null) ? this.ownerRig.head.rigTarget.transform : base.transform);
			this.maxEmissionRate = this.particleSystem.emission.rateOverTime.constant;
			this.maxVolume = this.audioSource.volume;
		}

		// Token: 0x0600537C RID: 21372 RVA: 0x00195634 File Offset: 0x00193834
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num = this.effectActivationRadius * this.effectActivationRadius * x * x;
			bool flag = (this.ownerHead.TransformPoint(this.headToMouthOffset) - position).sqrMagnitude < num;
			ParticleSystem.EmissionModule emission = this.particleSystem.emission;
			emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, flag ? this.maxEmissionRate : 0f, Time.deltaTime / 0.1f);
			this.audioSource.volume = Mathf.Lerp(this.audioSource.volume, flag ? this.maxVolume : 0f, Time.deltaTime / 2f);
			this.ownerRig.UsingHauntedRing = this.isHauntedVoiceChanger && flag;
			if (this.ownerRig.UsingHauntedRing)
			{
				this.ownerRig.HauntedRingVoicePitch = this.hauntedVoicePitch;
			}
		}

		// Token: 0x04005687 RID: 22151
		public GameObject effects;

		// Token: 0x04005688 RID: 22152
		[SerializeField]
		private bool isHauntedVoiceChanger;

		// Token: 0x04005689 RID: 22153
		[SerializeField]
		private float hauntedVoicePitch = 0.75f;

		// Token: 0x0400568A RID: 22154
		[AssignInCorePrefab]
		public float effectActivationRadius = 0.15f;

		// Token: 0x0400568B RID: 22155
		private readonly Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x0400568C RID: 22156
		private VRRig ownerRig;

		// Token: 0x0400568D RID: 22157
		private Transform ownerHead;

		// Token: 0x0400568E RID: 22158
		private ParticleSystem particleSystem;

		// Token: 0x0400568F RID: 22159
		private AudioSource audioSource;

		// Token: 0x04005690 RID: 22160
		private float maxEmissionRate;

		// Token: 0x04005691 RID: 22161
		private float maxVolume;

		// Token: 0x04005692 RID: 22162
		private const float emissionFadeTime = 0.1f;

		// Token: 0x04005693 RID: 22163
		private const float volumeFadeTime = 2f;
	}
}
