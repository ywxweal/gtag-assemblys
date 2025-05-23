using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200053E RID: 1342
public class ElderGorilla : MonoBehaviour
{
	// Token: 0x0600208C RID: 8332 RVA: 0x000A3398 File Offset: 0x000A1598
	private void Update()
	{
		if (GTPlayer.Instance == null)
		{
			return;
		}
		if (GTPlayer.Instance.inOverlay || !GTPlayer.Instance.isUserPresent)
		{
			return;
		}
		this.tHMD = GTPlayer.Instance.headCollider.transform;
		this.tLeftHand = GTPlayer.Instance.leftControllerTransform;
		this.tRightHand = GTPlayer.Instance.rightControllerTransform;
		if (Time.time - this.timeLastValidArmDist > 1f)
		{
			this.CheckHandDistance(this.tLeftHand);
			this.CheckHandDistance(this.tRightHand);
		}
		this.CheckHeight();
		this.CheckMicVolume();
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x000A3438 File Offset: 0x000A1638
	private void CheckHandDistance(Transform hand)
	{
		float num = Vector3.Distance(hand.localPosition, this.tHMD.localPosition);
		if (num >= 1f)
		{
			return;
		}
		if (num >= 0.75f)
		{
			this.countValidArmDists++;
			this.timeLastValidArmDist = Time.time;
		}
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x000A3488 File Offset: 0x000A1688
	private void CheckHeight()
	{
		float y = this.tHMD.localPosition.y;
		if (!this.trackingHeadHeight)
		{
			this.trackedHeadHeight = y - 0.05f;
			this.timerTrackedHeadHeight = 0f;
		}
		else if (this.trackedHeadHeight < y)
		{
			this.trackingHeadHeight = false;
		}
		if (this.trackingHeadHeight)
		{
			if (this.timerTrackedHeadHeight >= 1f)
			{
				this.savedHeadHeight = y;
				this.trackingHeadHeight = false;
				return;
			}
			this.timerTrackedHeadHeight += Time.deltaTime;
		}
	}

	// Token: 0x0600208F RID: 8335 RVA: 0x000A350E File Offset: 0x000A170E
	private void CheckMicVolume()
	{
		float currentPeakAmp = GorillaTagger.Instance.myRecorder.LevelMeter.CurrentPeakAmp;
	}

	// Token: 0x0400247B RID: 9339
	private const float MAX_HAND_DIST = 1f;

	// Token: 0x0400247C RID: 9340
	private const float COOLDOWN_HAND_DIST = 1f;

	// Token: 0x0400247D RID: 9341
	private const float VALID_HAND_DIST = 0.75f;

	// Token: 0x0400247E RID: 9342
	private const float TIME_VALID_HEAD_HEIGHT = 1f;

	// Token: 0x0400247F RID: 9343
	private Transform tHMD;

	// Token: 0x04002480 RID: 9344
	private Transform tLeftHand;

	// Token: 0x04002481 RID: 9345
	private Transform tRightHand;

	// Token: 0x04002482 RID: 9346
	private int countValidArmDists;

	// Token: 0x04002483 RID: 9347
	private float timeLastValidArmDist;

	// Token: 0x04002484 RID: 9348
	private bool trackingHeadHeight;

	// Token: 0x04002485 RID: 9349
	private float trackedHeadHeight;

	// Token: 0x04002486 RID: 9350
	private float timerTrackedHeadHeight;

	// Token: 0x04002487 RID: 9351
	private float savedHeadHeight = 1.5f;
}
