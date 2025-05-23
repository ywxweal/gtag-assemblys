using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x0200058B RID: 1419
public class GhostReactorShiftManager : MonoBehaviour
{
	// Token: 0x1700035C RID: 860
	// (get) Token: 0x060022C5 RID: 8901 RVA: 0x000AE602 File Offset: 0x000AC802
	public bool ShiftActive
	{
		get
		{
			return this.shiftStarted;
		}
	}

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x060022C6 RID: 8902 RVA: 0x000AE60A File Offset: 0x000AC80A
	public double ShiftStartNetworkTime
	{
		get
		{
			return this.shiftStartNetworkTime;
		}
	}

	// Token: 0x1700035E RID: 862
	// (get) Token: 0x060022C7 RID: 8903 RVA: 0x000AE612 File Offset: 0x000AC812
	public bool LocalPlayerInside
	{
		get
		{
			return this.localPlayerInside;
		}
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x000AE61C File Offset: 0x000AC81C
	public void RefreshShiftStatsDisplay()
	{
		this.shiftStatsText.text = string.Concat(new string[]
		{
			"\n\n",
			this.EnemyDeaths.ToString("D2"),
			"\n",
			this.CoresCollected.ToString("D2"),
			"\n",
			this.PlayerDeaths.ToString("D2")
		});
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x000AE690 File Offset: 0x000AC890
	public void StartShiftButtonPressed()
	{
		this.RequestShiftStart();
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x000AE698 File Offset: 0x000AC898
	public void RequestShiftStart()
	{
		GhostReactorManager.instance.RequestShiftStart();
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x000AE6A4 File Offset: 0x000AC8A4
	public void EndShift()
	{
		GhostReactorManager.instance.RequestShiftEnd();
	}

	// Token: 0x060022CC RID: 8908 RVA: 0x000AE6B0 File Offset: 0x000AC8B0
	public void ClearEntities()
	{
		Debug.LogError("Need to re-implement whatever this was doing");
	}

	// Token: 0x060022CD RID: 8909 RVA: 0x000AE6BC File Offset: 0x000AC8BC
	public void OnShiftStarted(double shiftStartTime)
	{
		this.shiftStarted = true;
		this.shiftStartNetworkTime = shiftStartTime;
		this.frontGate.OpenGate();
		this.ringTransform.gameObject.SetActive(false);
		this.gateBlockerTransform.gameObject.SetActive(false);
		this.prevCountDownTotal = this.shiftDurationMinutes * 60f;
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x000AE718 File Offset: 0x000AC918
	public void OnShiftEnded()
	{
		this.shiftStarted = false;
		this.shiftTimerText.text = Mathf.FloorToInt(this.shiftDurationMinutes).ToString("D2") + ":00";
		this.frontGate.CloseGate();
		this.ringTransform.gameObject.SetActive(false);
		if (this.localPlayerInside)
		{
			this.LocalPlayerOutOfBounds();
		}
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x000AE784 File Offset: 0x000AC984
	private void Update()
	{
		double num = PhotonNetwork.Time - this.shiftStartNetworkTime;
		float num2 = 60f * this.shiftDurationMinutes - (float)num;
		if (GameEntityManager.instance.IsAuthority())
		{
			this.AuthorityUpdate(num2);
		}
		num2 = Mathf.Clamp(num2, 0f, 60f * this.shiftDurationMinutes);
		this.SharedUpdate(num2);
		this.prevCountDownTotal = num2;
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x000AE7E9 File Offset: 0x000AC9E9
	private void AuthorityUpdate(float countDownTotal)
	{
		if (PhotonNetwork.InRoom && GameEntityManager.instance.IsAuthority() && this.shiftStarted && countDownTotal <= 0f)
		{
			GhostReactorManager.instance.RequestShiftEnd();
		}
	}

	// Token: 0x060022D1 RID: 8913 RVA: 0x000AE81C File Offset: 0x000ACA1C
	private void SharedUpdate(float countDownTotal)
	{
		if (this.shiftStarted)
		{
			if (this.debugFastForwarding)
			{
				float num = this.debugFastForwardRate * Time.deltaTime;
				this.shiftStartNetworkTime -= (double)num;
			}
			int num2 = Mathf.FloorToInt(countDownTotal / 60f);
			int num3 = Mathf.FloorToInt(countDownTotal % 60f);
			this.shiftTimerText.text = num2.ToString("D2") + ":" + num3.ToString("D2");
			for (int i = 0; i < this.warningClipPlayTimes.Count; i++)
			{
				if (countDownTotal < (float)this.warningClipPlayTimes[i] && this.prevCountDownTotal >= (float)this.warningClipPlayTimes[i])
				{
					this.warningAudioSource.PlayOneShot(this.warningAudio);
				}
			}
			if (this.localPlayerInside)
			{
				if (countDownTotal >= 0f && countDownTotal < this.ringClosingDuration * 60f)
				{
					this.ringTransform.gameObject.SetActive(true);
					float num4 = Mathf.Lerp(this.ringClosingMinRadius, this.ringClosingMaxRadius, countDownTotal / (this.ringClosingDuration * 60f));
					this.ringTransform.localScale = new Vector3(num4, 1f, num4);
					Vector3 vector = VRRig.LocalRig.bodyTransform.position - this.ringTransform.position;
					vector.y = 0f;
					if (vector.sqrMagnitude > num4 * num4)
					{
						this.LocalPlayerOutOfBounds();
						return;
					}
				}
			}
			else if (this.ringTransform.gameObject.activeSelf)
			{
				this.ringTransform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x000AE9C3 File Offset: 0x000ACBC3
	private void LocalPlayerOutOfBounds()
	{
		if (this.localPlayerInside)
		{
			GhostReactorManager.instance.ReportLocalPlayerHit();
			VRRig.LocalRig.GetComponent<GRPlayer>().ChangePlayerState(GRPlayer.GRPlayerState.Ghost);
			GTPlayer.Instance.TeleportTo(this.playerTeleportTransform, true, true);
			this.localPlayerInside = false;
		}
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x000AEA00 File Offset: 0x000ACC00
	public void RevealJudgment(int evaluation)
	{
		if (evaluation <= 0)
		{
			this.shiftJugmentText.text = "DON'T QUIT YOUR DAY JOB.";
			return;
		}
		switch (evaluation)
		{
		case 1:
			this.shiftJugmentText.text = "YOU'RE LEARNING. GOOD.";
			return;
		case 2:
			this.shiftJugmentText.text = "YOU MIGHT EARN A PROMOTION.";
			return;
		case 3:
			this.shiftJugmentText.text = "YOU DID A MANAGER-TIER JOB.";
			return;
		case 4:
			this.shiftJugmentText.text = "NICE. YOU GET EXTRA SHIFTS.";
			return;
		default:
			this.shiftJugmentText.text = "YOU WORK FOR US NOW.";
			this.wrongStumpGoo.SetActive(true);
			return;
		}
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x000AEA9C File Offset: 0x000ACC9C
	public void ResetJudgment()
	{
		this.shiftJugmentText.text = "";
		this.wrongStumpGoo.SetActive(false);
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x000AEABA File Offset: 0x000ACCBA
	private void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.localPlayerInside = true;
		}
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x000AEAD8 File Offset: 0x000ACCD8
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			bool flag = Vector3.Dot(other.transform.position - this.gatePlaneTransform.position, this.gatePlaneTransform.forward) < 0f;
			this.localPlayerInside = flag;
		}
	}

	// Token: 0x040026C9 RID: 9929
	[SerializeField]
	private GRMetalEnergyGate frontGate;

	// Token: 0x040026CA RID: 9930
	[SerializeField]
	private TMP_Text shiftTimerText;

	// Token: 0x040026CB RID: 9931
	[SerializeField]
	private TMP_Text shiftStatsText;

	// Token: 0x040026CC RID: 9932
	[SerializeField]
	private TMP_Text shiftJugmentText;

	// Token: 0x040026CD RID: 9933
	[SerializeField]
	private GameObject wrongStumpGoo;

	// Token: 0x040026CE RID: 9934
	[SerializeField]
	private float shiftDurationMinutes = 20f;

	// Token: 0x040026CF RID: 9935
	[SerializeField]
	private Transform playerTeleportTransform;

	// Token: 0x040026D0 RID: 9936
	[SerializeField]
	private Transform gatePlaneTransform;

	// Token: 0x040026D1 RID: 9937
	[SerializeField]
	private Transform gateBlockerTransform;

	// Token: 0x040026D2 RID: 9938
	[Header("Audio")]
	[SerializeField]
	private AudioSource warningAudioSource;

	// Token: 0x040026D3 RID: 9939
	[SerializeField]
	private AudioClip warningAudio;

	// Token: 0x040026D4 RID: 9940
	[SerializeField]
	[Tooltip("Must be ordered from largest time (first played) to smallest time (last played)")]
	private List<int> warningClipPlayTimes = new List<int>();

	// Token: 0x040026D5 RID: 9941
	[Header("Ring")]
	[SerializeField]
	private Transform ringTransform;

	// Token: 0x040026D6 RID: 9942
	[SerializeField]
	private float ringClosingDuration = 3f;

	// Token: 0x040026D7 RID: 9943
	[SerializeField]
	private float ringClosingMaxRadius = 100f;

	// Token: 0x040026D8 RID: 9944
	[SerializeField]
	private float ringClosingMinRadius = 7f;

	// Token: 0x040026D9 RID: 9945
	[Header("Debug")]
	[SerializeField]
	private float debugFastForwardRate = 30f;

	// Token: 0x040026DA RID: 9946
	[SerializeField]
	private bool debugFastForwarding;

	// Token: 0x040026DB RID: 9947
	private bool shiftStarted;

	// Token: 0x040026DC RID: 9948
	private double shiftStartNetworkTime;

	// Token: 0x040026DD RID: 9949
	private float prevCountDownTotal;

	// Token: 0x040026DE RID: 9950
	private bool localPlayerInside;

	// Token: 0x040026DF RID: 9951
	private static List<VRRig> tempRigs = new List<VRRig>(12);

	// Token: 0x040026E0 RID: 9952
	public int EnemyDeaths;

	// Token: 0x040026E1 RID: 9953
	public int PlayerDeaths;

	// Token: 0x040026E2 RID: 9954
	public int CoresCollected;

	// Token: 0x040026E3 RID: 9955
	[NonSerialized]
	public GhostReactorManager grManager;
}
