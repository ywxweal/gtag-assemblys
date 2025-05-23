using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BFA RID: 3066
	public class TrainLocomotive : TrainCarBase
	{
		// Token: 0x06004BC4 RID: 19396 RVA: 0x001672B0 File Offset: 0x001654B0
		private void Start()
		{
			this._standardRateOverTimeMultiplier = this._smoke1.emission.rateOverTimeMultiplier;
			this._standardMaxParticles = this._smoke1.main.maxParticles;
			base.Distance = 0f;
			this._speedDiv = 2.5f / (float)this._accelerationSounds.Length;
			this._currentSpeed = this._initialSpeed;
			base.UpdateCarPosition();
			this._smoke1.Stop();
			this._startStopTrainCr = base.StartCoroutine(this.StartStopTrain(true));
		}

		// Token: 0x06004BC5 RID: 19397 RVA: 0x0016733F File Offset: 0x0016553F
		private void Update()
		{
			this.UpdatePosition();
		}

		// Token: 0x06004BC6 RID: 19398 RVA: 0x00167348 File Offset: 0x00165548
		public override void UpdatePosition()
		{
			if (!this._isMoving)
			{
				return;
			}
			if (this._trainTrack != null)
			{
				this.UpdateDistance();
				base.UpdateCarPosition();
				base.RotateCarWheels();
			}
			TrainCarBase[] childCars = this._childCars;
			for (int i = 0; i < childCars.Length; i++)
			{
				childCars[i].UpdatePosition();
			}
		}

		// Token: 0x06004BC7 RID: 19399 RVA: 0x0016739B File Offset: 0x0016559B
		public void StartStopStateChanged()
		{
			if (this._startStopTrainCr == null)
			{
				this._startStopTrainCr = base.StartCoroutine(this.StartStopTrain(!this._isMoving));
			}
		}

		// Token: 0x06004BC8 RID: 19400 RVA: 0x001673C0 File Offset: 0x001655C0
		private IEnumerator StartStopTrain(bool startTrain)
		{
			float endSpeed = (startTrain ? this._initialSpeed : 0f);
			float timePeriodForSpeedChange = 3f;
			if (startTrain)
			{
				this._smoke1.Play();
				this._isMoving = true;
				ParticleSystem.EmissionModule emission = this._smoke1.emission;
				ParticleSystem.MainModule main = this._smoke1.main;
				emission.rateOverTimeMultiplier = this._standardRateOverTimeMultiplier;
				main.maxParticles = this._standardMaxParticles;
				timePeriodForSpeedChange = this.PlayEngineSound(TrainLocomotive.EngineSoundState.Start);
			}
			else
			{
				timePeriodForSpeedChange = this.PlayEngineSound(TrainLocomotive.EngineSoundState.Stop);
			}
			this._engineAudioSource.loop = false;
			timePeriodForSpeedChange *= 0.9f;
			float startTime = Time.time;
			float endTime = Time.time + timePeriodForSpeedChange;
			float startSpeed = this._currentSpeed;
			while (Time.time < endTime)
			{
				float num = (Time.time - startTime) / timePeriodForSpeedChange;
				this._currentSpeed = startSpeed * (1f - num) + endSpeed * num;
				this.UpdateSmokeEmissionBasedOnSpeed();
				yield return null;
			}
			this._currentSpeed = endSpeed;
			this._startStopTrainCr = null;
			this._isMoving = startTrain;
			if (!this._isMoving)
			{
				this._smoke1.Stop();
			}
			else
			{
				this._engineAudioSource.loop = true;
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
			yield break;
		}

		// Token: 0x06004BC9 RID: 19401 RVA: 0x001673D8 File Offset: 0x001655D8
		private float PlayEngineSound(TrainLocomotive.EngineSoundState engineSoundState)
		{
			AudioClip audioClip;
			if (engineSoundState == TrainLocomotive.EngineSoundState.Start)
			{
				audioClip = this._startUpSound;
			}
			else
			{
				AudioClip[] array = ((engineSoundState == TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed) ? this._accelerationSounds : this._decelerationSounds);
				int num = array.Length;
				int num2 = (int)Mathf.Round((this._currentSpeed - 0.2f) / this._speedDiv);
				audioClip = array[Mathf.Clamp(num2, 0, num - 1)];
			}
			if (this._engineAudioSource.clip == audioClip && this._engineAudioSource.isPlaying && engineSoundState == TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed)
			{
				return 0f;
			}
			this._engineAudioSource.clip = audioClip;
			this._engineAudioSource.timeSamples = 0;
			this._engineAudioSource.Play();
			return audioClip.length;
		}

		// Token: 0x06004BCA RID: 19402 RVA: 0x00167484 File Offset: 0x00165684
		private void UpdateDistance()
		{
			float num = (this._reverse ? (-this._currentSpeed) : this._currentSpeed);
			base.Distance = (base.Distance + num * Time.deltaTime) % this._trainTrack.TrackLength;
		}

		// Token: 0x06004BCB RID: 19403 RVA: 0x001674CC File Offset: 0x001656CC
		public void DecreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed - this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		// Token: 0x06004BCC RID: 19404 RVA: 0x0016751C File Offset: 0x0016571C
		public void IncreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed + this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		// Token: 0x06004BCD RID: 19405 RVA: 0x0016756C File Offset: 0x0016576C
		private void UpdateSmokeEmissionBasedOnSpeed()
		{
			this._smoke1.emission.rateOverTimeMultiplier = this.GetCurrentSmokeEmission();
			this._smoke1.main.maxParticles = (int)Mathf.Lerp((float)this._standardMaxParticles, (float)(this._standardMaxParticles * 3), this._currentSpeed / 2.5f);
		}

		// Token: 0x06004BCE RID: 19406 RVA: 0x001675C7 File Offset: 0x001657C7
		private float GetCurrentSmokeEmission()
		{
			return Mathf.Lerp(this._standardRateOverTimeMultiplier, this._standardRateOverTimeMultiplier * 8f, this._currentSpeed / 2.5f);
		}

		// Token: 0x06004BCF RID: 19407 RVA: 0x001675EC File Offset: 0x001657EC
		public void SmokeButtonStateChanged()
		{
			if (this._isMoving)
			{
				this._smokeStackAudioSource.clip = this._smokeSound;
				this._smokeStackAudioSource.timeSamples = 0;
				this._smokeStackAudioSource.Play();
				this._smoke2.time = 0f;
				this._smoke2.Play();
			}
		}

		// Token: 0x06004BD0 RID: 19408 RVA: 0x00167644 File Offset: 0x00165844
		public void WhistleButtonStateChanged()
		{
			if (this._whistleSound != null)
			{
				this._whistleAudioSource.clip = this._whistleSound;
				this._whistleAudioSource.timeSamples = 0;
				this._whistleAudioSource.Play();
			}
		}

		// Token: 0x06004BD1 RID: 19409 RVA: 0x0016767C File Offset: 0x0016587C
		public void ReverseButtonStateChanged()
		{
			this._reverse = !this._reverse;
		}

		// Token: 0x04004E63 RID: 20067
		private const float MIN_SPEED = 0.2f;

		// Token: 0x04004E64 RID: 20068
		private const float MAX_SPEED = 2.7f;

		// Token: 0x04004E65 RID: 20069
		private const float SMOKE_SPEED_MULTIPLIER = 8f;

		// Token: 0x04004E66 RID: 20070
		private const int MAX_PARTICLES_MULTIPLIER = 3;

		// Token: 0x04004E67 RID: 20071
		[SerializeField]
		[Range(0.2f, 2.7f)]
		protected float _initialSpeed;

		// Token: 0x04004E68 RID: 20072
		[SerializeField]
		private GameObject _startStopButton;

		// Token: 0x04004E69 RID: 20073
		[SerializeField]
		private GameObject _decreaseSpeedButton;

		// Token: 0x04004E6A RID: 20074
		[SerializeField]
		private GameObject _increaseSpeedButton;

		// Token: 0x04004E6B RID: 20075
		[SerializeField]
		private GameObject _smokeButton;

		// Token: 0x04004E6C RID: 20076
		[SerializeField]
		private GameObject _whistleButton;

		// Token: 0x04004E6D RID: 20077
		[SerializeField]
		private GameObject _reverseButton;

		// Token: 0x04004E6E RID: 20078
		[SerializeField]
		private AudioSource _whistleAudioSource;

		// Token: 0x04004E6F RID: 20079
		[SerializeField]
		private AudioClip _whistleSound;

		// Token: 0x04004E70 RID: 20080
		[SerializeField]
		private AudioSource _engineAudioSource;

		// Token: 0x04004E71 RID: 20081
		[SerializeField]
		private AudioClip[] _accelerationSounds;

		// Token: 0x04004E72 RID: 20082
		[SerializeField]
		private AudioClip[] _decelerationSounds;

		// Token: 0x04004E73 RID: 20083
		[SerializeField]
		private AudioClip _startUpSound;

		// Token: 0x04004E74 RID: 20084
		[SerializeField]
		private AudioSource _smokeStackAudioSource;

		// Token: 0x04004E75 RID: 20085
		[SerializeField]
		private AudioClip _smokeSound;

		// Token: 0x04004E76 RID: 20086
		[SerializeField]
		private ParticleSystem _smoke1;

		// Token: 0x04004E77 RID: 20087
		[SerializeField]
		private ParticleSystem _smoke2;

		// Token: 0x04004E78 RID: 20088
		[SerializeField]
		private TrainCarBase[] _childCars;

		// Token: 0x04004E79 RID: 20089
		private bool _isMoving = true;

		// Token: 0x04004E7A RID: 20090
		private bool _reverse;

		// Token: 0x04004E7B RID: 20091
		private float _currentSpeed;

		// Token: 0x04004E7C RID: 20092
		private float _speedDiv;

		// Token: 0x04004E7D RID: 20093
		private float _standardRateOverTimeMultiplier;

		// Token: 0x04004E7E RID: 20094
		private int _standardMaxParticles;

		// Token: 0x04004E7F RID: 20095
		private Coroutine _startStopTrainCr;

		// Token: 0x02000BFB RID: 3067
		private enum EngineSoundState
		{
			// Token: 0x04004E81 RID: 20097
			Start,
			// Token: 0x04004E82 RID: 20098
			AccelerateOrSetProperSpeed,
			// Token: 0x04004E83 RID: 20099
			Stop
		}
	}
}
