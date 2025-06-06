﻿using System;
using System.Collections;
using UnityEngine;

namespace UnityChan
{
	// Token: 0x02000E46 RID: 3654
	public class AutoBlink : MonoBehaviour
	{
		// Token: 0x06005B7F RID: 23423 RVA: 0x000023F4 File Offset: 0x000005F4
		private void Awake()
		{
		}

		// Token: 0x06005B80 RID: 23424 RVA: 0x001C174E File Offset: 0x001BF94E
		private void Start()
		{
			this.ResetTimer();
			base.StartCoroutine("RandomChange");
		}

		// Token: 0x06005B81 RID: 23425 RVA: 0x001C1762 File Offset: 0x001BF962
		private void ResetTimer()
		{
			this.timeRemining = this.timeBlink;
			this.timerStarted = false;
		}

		// Token: 0x06005B82 RID: 23426 RVA: 0x001C1778 File Offset: 0x001BF978
		private void Update()
		{
			if (!this.timerStarted)
			{
				this.eyeStatus = AutoBlink.Status.Close;
				this.timerStarted = true;
			}
			if (this.timerStarted)
			{
				this.timeRemining -= Time.deltaTime;
				if (this.timeRemining <= 0f)
				{
					this.eyeStatus = AutoBlink.Status.Open;
					this.ResetTimer();
					return;
				}
				if (this.timeRemining <= this.timeBlink * 0.3f)
				{
					this.eyeStatus = AutoBlink.Status.HalfClose;
				}
			}
		}

		// Token: 0x06005B83 RID: 23427 RVA: 0x001C17EC File Offset: 0x001BF9EC
		private void LateUpdate()
		{
			if (this.isActive && this.isBlink)
			{
				switch (this.eyeStatus)
				{
				case AutoBlink.Status.Close:
					this.SetCloseEyes();
					return;
				case AutoBlink.Status.HalfClose:
					this.SetHalfCloseEyes();
					return;
				case AutoBlink.Status.Open:
					this.SetOpenEyes();
					this.isBlink = false;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06005B84 RID: 23428 RVA: 0x001C183E File Offset: 0x001BFA3E
		private void SetCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Close);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Close);
		}

		// Token: 0x06005B85 RID: 23429 RVA: 0x001C1864 File Offset: 0x001BFA64
		private void SetHalfCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
		}

		// Token: 0x06005B86 RID: 23430 RVA: 0x001C188A File Offset: 0x001BFA8A
		private void SetOpenEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Open);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Open);
		}

		// Token: 0x06005B87 RID: 23431 RVA: 0x001C18B0 File Offset: 0x001BFAB0
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				float num = Random.Range(0f, 1f);
				if (!this.isBlink && num > this.threshold)
				{
					this.isBlink = true;
				}
				yield return new WaitForSeconds(this.interval);
			}
			yield break;
		}

		// Token: 0x04005F43 RID: 24387
		public bool isActive = true;

		// Token: 0x04005F44 RID: 24388
		public SkinnedMeshRenderer ref_SMR_EYE_DEF;

		// Token: 0x04005F45 RID: 24389
		public SkinnedMeshRenderer ref_SMR_EL_DEF;

		// Token: 0x04005F46 RID: 24390
		public float ratio_Close = 85f;

		// Token: 0x04005F47 RID: 24391
		public float ratio_HalfClose = 20f;

		// Token: 0x04005F48 RID: 24392
		[HideInInspector]
		public float ratio_Open;

		// Token: 0x04005F49 RID: 24393
		private bool timerStarted;

		// Token: 0x04005F4A RID: 24394
		private bool isBlink;

		// Token: 0x04005F4B RID: 24395
		public float timeBlink = 0.4f;

		// Token: 0x04005F4C RID: 24396
		private float timeRemining;

		// Token: 0x04005F4D RID: 24397
		public float threshold = 0.3f;

		// Token: 0x04005F4E RID: 24398
		public float interval = 3f;

		// Token: 0x04005F4F RID: 24399
		private AutoBlink.Status eyeStatus;

		// Token: 0x02000E47 RID: 3655
		private enum Status
		{
			// Token: 0x04005F51 RID: 24401
			Close,
			// Token: 0x04005F52 RID: 24402
			HalfClose,
			// Token: 0x04005F53 RID: 24403
			Open
		}
	}
}
