using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BFF RID: 3071
	public class WindmillBladesController : MonoBehaviour
	{
		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x06004BE2 RID: 19426 RVA: 0x00167A6A File Offset: 0x00165C6A
		// (set) Token: 0x06004BE3 RID: 19427 RVA: 0x00167A72 File Offset: 0x00165C72
		public bool IsMoving { get; private set; }

		// Token: 0x06004BE4 RID: 19428 RVA: 0x00167A7B File Offset: 0x00165C7B
		private void Start()
		{
			this._originalRotation = base.transform.localRotation;
		}

		// Token: 0x06004BE5 RID: 19429 RVA: 0x00167A90 File Offset: 0x00165C90
		private void Update()
		{
			this._rotAngle += this._currentSpeed * Time.deltaTime;
			if (this._rotAngle > 360f)
			{
				this._rotAngle = 0f;
			}
			base.transform.localRotation = this._originalRotation * Quaternion.AngleAxis(this._rotAngle, Vector3.forward);
		}

		// Token: 0x06004BE6 RID: 19430 RVA: 0x00167AF4 File Offset: 0x00165CF4
		public void SetMoveState(bool newMoveState, float goalSpeed)
		{
			this.IsMoving = newMoveState;
			if (this._lerpSpeedCoroutine != null)
			{
				base.StopCoroutine(this._lerpSpeedCoroutine);
			}
			this._lerpSpeedCoroutine = base.StartCoroutine(this.LerpToSpeed(goalSpeed));
		}

		// Token: 0x06004BE7 RID: 19431 RVA: 0x00167B24 File Offset: 0x00165D24
		private IEnumerator LerpToSpeed(float goalSpeed)
		{
			float totalTime = 0f;
			float startSpeed = this._currentSpeed;
			if (this._audioChangeCr != null)
			{
				base.StopCoroutine(this._audioChangeCr);
			}
			if (this.IsMoving)
			{
				this._audioChangeCr = base.StartCoroutine(this.PlaySoundDelayed(this._windMillStartSound, this._windMillRotationSound, this._windMillStartSound.length * 0.95f));
			}
			else
			{
				this.PlaySound(this._windMillStopSound, false);
			}
			for (float num = Mathf.Abs(this._currentSpeed - goalSpeed); num > Mathf.Epsilon; num = Mathf.Abs(this._currentSpeed - goalSpeed))
			{
				this._currentSpeed = Mathf.Lerp(startSpeed, goalSpeed, totalTime / 1f);
				totalTime += Time.deltaTime;
				yield return null;
			}
			this._lerpSpeedCoroutine = null;
			yield break;
		}

		// Token: 0x06004BE8 RID: 19432 RVA: 0x00167B3A File Offset: 0x00165D3A
		private IEnumerator PlaySoundDelayed(AudioClip initial, AudioClip clip, float timeDelayAfterInitial)
		{
			this.PlaySound(initial, false);
			yield return new WaitForSeconds(timeDelayAfterInitial);
			this.PlaySound(clip, true);
			yield break;
		}

		// Token: 0x06004BE9 RID: 19433 RVA: 0x00167B5E File Offset: 0x00165D5E
		private void PlaySound(AudioClip clip, bool loop = false)
		{
			this._audioSource.loop = loop;
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		// Token: 0x04004E96 RID: 20118
		private const float MAX_TIME = 1f;

		// Token: 0x04004E97 RID: 20119
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04004E98 RID: 20120
		[SerializeField]
		private AudioClip _windMillRotationSound;

		// Token: 0x04004E99 RID: 20121
		[SerializeField]
		private AudioClip _windMillStartSound;

		// Token: 0x04004E9A RID: 20122
		[SerializeField]
		private AudioClip _windMillStopSound;

		// Token: 0x04004E9C RID: 20124
		private float _currentSpeed;

		// Token: 0x04004E9D RID: 20125
		private Coroutine _lerpSpeedCoroutine;

		// Token: 0x04004E9E RID: 20126
		private Coroutine _audioChangeCr;

		// Token: 0x04004E9F RID: 20127
		private Quaternion _originalRotation;

		// Token: 0x04004EA0 RID: 20128
		private float _rotAngle;
	}
}
