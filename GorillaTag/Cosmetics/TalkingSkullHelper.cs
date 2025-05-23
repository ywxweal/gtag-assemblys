using System;
using GorillaTag.Audio;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DAD RID: 3501
	public class TalkingSkullHelper : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060056B8 RID: 22200 RVA: 0x001A6E65 File Offset: 0x001A5065
		public void Awake()
		{
			this._materialPropertyBlock = new MaterialPropertyBlock();
			this.SetEyeColor(this.EyeColorOff);
		}

		// Token: 0x060056B9 RID: 22201 RVA: 0x001A6E7E File Offset: 0x001A507E
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this._speakerLoudness = base.GetComponentInParent<GorillaSpeakerLoudness>();
		}

		// Token: 0x060056BA RID: 22202 RVA: 0x00010F34 File Offset: 0x0000F134
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x060056BB RID: 22203 RVA: 0x001A6E93 File Offset: 0x001A5093
		public void ToggleIsPlaced(bool isPlaced)
		{
			this._isPlaced = isPlaced;
			if (!this._isPlaced)
			{
				this.CanTalk(false);
			}
		}

		// Token: 0x060056BC RID: 22204 RVA: 0x001A6EAC File Offset: 0x001A50AC
		public void CanTalk(bool toggle)
		{
			if (this._isActive == toggle)
			{
				return;
			}
			this._isActive = toggle;
			if (toggle)
			{
				this._activator.StartLocalBroadcast();
				this.SetEyeColor(this.EyeColorOn);
				return;
			}
			this._activator.StopLocalBroadcast();
			this.SetEyeColor(this.EyeColorOff);
		}

		// Token: 0x060056BD RID: 22205 RVA: 0x001A6EFC File Offset: 0x001A50FC
		public void SliceUpdate()
		{
			this._deltaTime = Time.time - this._timeLastUpdated;
			this._timeLastUpdated = Time.time;
			if (!this._isPlaced)
			{
				if (this._currentAntennaWeight > 0f)
				{
					this._currentAntennaWeight -= this._deltaTime * this.AntennaExtendSpeed;
					this._skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Clamp(this._currentAntennaWeight, 0f, 100f));
				}
			}
			else if (this._currentAntennaWeight < 100f)
			{
				this._currentAntennaWeight += this._deltaTime * this.AntennaExtendSpeed;
				this._skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Clamp(this._currentAntennaWeight, 0f, 100f));
			}
			if (!this._isPlaced || !this._speakerLoudness.IsSpeaking)
			{
				this._attack = 0f;
				if (!this._animation.isPlaying)
				{
					this._skullTransform.localPosition = this.SkullOffsetPosition;
				}
				return;
			}
			float num = 0f;
			if (this._speakerLoudness != null)
			{
				num = this._speakerLoudness.LoudnessNormalized;
			}
			if (num >= this.LoudnessThreshold)
			{
				this._attack += this._deltaTime;
				if (this._attack >= this.LoudnessAttack && !this._animation.isPlaying)
				{
					this._animation.Play();
					return;
				}
			}
			else
			{
				this._attack = 0f;
			}
		}

		// Token: 0x060056BE RID: 22206 RVA: 0x001A7071 File Offset: 0x001A5271
		private void SetEyeColor(Color eyeColor)
		{
			this._materialPropertyBlock.SetColor("_BaseColor", eyeColor);
			this._skinnedMeshRenderer.SetPropertyBlock(this._materialPropertyBlock, 0);
		}

		// Token: 0x060056BF RID: 22207 RVA: 0x001A7096 File Offset: 0x001A5296
		private void ResetToFirstFrame()
		{
			this._animation.Rewind();
			this._animation.Play();
			this._animation.Sample();
			this._animation.Stop();
		}

		// Token: 0x060056C1 RID: 22209 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x04005AA3 RID: 23203
		[Tooltip("How loud the Gorilla voice should be before detecting as talking.")]
		public float LoudnessThreshold = 0.1f;

		// Token: 0x04005AA4 RID: 23204
		[Tooltip("How long the initial speaking section needs to last to trigger the talking animation.")]
		public float LoudnessAttack = 0.15f;

		// Token: 0x04005AA5 RID: 23205
		[Tooltip("How fast the antenna should extend (with the range of the blend shape from 0-100).")]
		public float AntennaExtendSpeed = 100f;

		// Token: 0x04005AA6 RID: 23206
		public Color EyeColorOff = Color.black;

		// Token: 0x04005AA7 RID: 23207
		public Color EyeColorOn = Color.white;

		// Token: 0x04005AA8 RID: 23208
		private float _attack;

		// Token: 0x04005AA9 RID: 23209
		private float _deltaTime;

		// Token: 0x04005AAA RID: 23210
		private float _timeLastUpdated;

		// Token: 0x04005AAB RID: 23211
		private bool _isPlaced;

		// Token: 0x04005AAC RID: 23212
		private bool _isActive;

		// Token: 0x04005AAD RID: 23213
		private float _currentAntennaWeight;

		// Token: 0x04005AAE RID: 23214
		[SerializeField]
		private Animation _animation;

		// Token: 0x04005AAF RID: 23215
		[SerializeField]
		private SkinnedMeshRenderer _skinnedMeshRenderer;

		// Token: 0x04005AB0 RID: 23216
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04005AB1 RID: 23217
		[SerializeField]
		private LoudSpeakerActivator _activator;

		// Token: 0x04005AB2 RID: 23218
		[SerializeField]
		private GorillaSpeakerLoudness _speakerLoudness;

		// Token: 0x04005AB3 RID: 23219
		[SerializeField]
		private Transform _skullTransform;

		// Token: 0x04005AB4 RID: 23220
		public Vector3 SkullOffsetPosition = new Vector3(0f, -0.15f, 0f);

		// Token: 0x04005AB5 RID: 23221
		private MaterialPropertyBlock _materialPropertyBlock;
	}
}
