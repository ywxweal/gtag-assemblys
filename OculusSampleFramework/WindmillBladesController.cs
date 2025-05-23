using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BFF RID: 3071
	public class WindmillBladesController : MonoBehaviour
	{
		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x06004BE3 RID: 19427 RVA: 0x00167B42 File Offset: 0x00165D42
		// (set) Token: 0x06004BE4 RID: 19428 RVA: 0x00167B4A File Offset: 0x00165D4A
		public bool IsMoving { get; private set; }

		// Token: 0x06004BE5 RID: 19429 RVA: 0x00167B53 File Offset: 0x00165D53
		private void Start()
		{
			this._originalRotation = base.transform.localRotation;
		}

		// Token: 0x06004BE6 RID: 19430 RVA: 0x00167B68 File Offset: 0x00165D68
		private void Update()
		{
			this._rotAngle += this._currentSpeed * Time.deltaTime;
			if (this._rotAngle > 360f)
			{
				this._rotAngle = 0f;
			}
			base.transform.localRotation = this._originalRotation * Quaternion.AngleAxis(this._rotAngle, Vector3.forward);
		}

		// Token: 0x06004BE7 RID: 19431 RVA: 0x00167BCC File Offset: 0x00165DCC
		public void SetMoveState(bool newMoveState, float goalSpeed)
		{
			this.IsMoving = newMoveState;
			if (this._lerpSpeedCoroutine != null)
			{
				base.StopCoroutine(this._lerpSpeedCoroutine);
			}
			this._lerpSpeedCoroutine = base.StartCoroutine(this.LerpToSpeed(goalSpeed));
		}

		// Token: 0x06004BE8 RID: 19432 RVA: 0x00167BFC File Offset: 0x00165DFC
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

		// Token: 0x06004BE9 RID: 19433 RVA: 0x00167C12 File Offset: 0x00165E12
		private IEnumerator PlaySoundDelayed(AudioClip initial, AudioClip clip, float timeDelayAfterInitial)
		{
			this.PlaySound(initial, false);
			yield return new WaitForSeconds(timeDelayAfterInitial);
			this.PlaySound(clip, true);
			yield break;
		}

		// Token: 0x06004BEA RID: 19434 RVA: 0x00167C36 File Offset: 0x00165E36
		private void PlaySound(AudioClip clip, bool loop = false)
		{
			this._audioSource.loop = loop;
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		// Token: 0x04004E97 RID: 20119
		private const float MAX_TIME = 1f;

		// Token: 0x04004E98 RID: 20120
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04004E99 RID: 20121
		[SerializeField]
		private AudioClip _windMillRotationSound;

		// Token: 0x04004E9A RID: 20122
		[SerializeField]
		private AudioClip _windMillStartSound;

		// Token: 0x04004E9B RID: 20123
		[SerializeField]
		private AudioClip _windMillStopSound;

		// Token: 0x04004E9D RID: 20125
		private float _currentSpeed;

		// Token: 0x04004E9E RID: 20126
		private Coroutine _lerpSpeedCoroutine;

		// Token: 0x04004E9F RID: 20127
		private Coroutine _audioChangeCr;

		// Token: 0x04004EA0 RID: 20128
		private Quaternion _originalRotation;

		// Token: 0x04004EA1 RID: 20129
		private float _rotAngle;
	}
}
