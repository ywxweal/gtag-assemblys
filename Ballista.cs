using System;
using System.Collections;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class Ballista : MonoBehaviourPun
{
	// Token: 0x06000568 RID: 1384 RVA: 0x0001F70D File Offset: 0x0001D90D
	public void TriggerLoad()
	{
		this.animator.SetTrigger(this.loadTriggerHash);
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0001F720 File Offset: 0x0001D920
	public void TriggerFire()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x0600056A RID: 1386 RVA: 0x0001F733 File Offset: 0x0001D933
	private float LaunchSpeed
	{
		get
		{
			if (!this.useSpeedOptions)
			{
				return this.launchSpeed;
			}
			return this.speedOptions[this.currentSpeedIndex];
		}
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0001F754 File Offset: 0x0001D954
	private void Awake()
	{
		this.launchDirection = this.launchEnd.position - this.launchStart.position;
		this.launchRampDistance = this.launchDirection.magnitude;
		this.launchDirection /= this.launchRampDistance;
		this.collidingLayer = LayerMask.NameToLayer("Default");
		this.notCollidingLayer = LayerMask.NameToLayer("Prop");
		this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
		this.animator.SetFloat(this.pitchParamHash, this.pitch);
		this.appliedAnimatorPitch = this.pitch;
		this.RefreshButtonColors();
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0001F804 File Offset: 0x0001DA04
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
		{
			if (this.prevStateHash == this.fireStateHash)
			{
				this.fireCompleteTime = Time.time;
			}
			if (Time.time - this.fireCompleteTime > this.reloadDelay)
			{
				this.animator.SetTrigger(this.loadTriggerHash);
				this.loadStartTime = Time.time;
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash)
		{
			if (Time.time - this.loadStartTime > this.loadTime)
			{
				if (this.playerInTrigger)
				{
					GTPlayer instance = GTPlayer.Instance;
					Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance);
					Vector3 vector = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
					Vector3 vector2 = playerBodyCenterPosition - vector;
					Vector3 vector3 = Vector3.Lerp(Vector3.zero, vector2, Mathf.Exp(-this.playerPullInRate * deltaTime));
					instance.transform.position = instance.transform.position + (vector3 - vector2);
					this.playerReadyToFire = vector3.sqrMagnitude < this.playerReadyToFireDist * this.playerReadyToFireDist;
				}
				else
				{
					this.playerReadyToFire = false;
				}
				if (this.playerReadyToFire)
				{
					if (PhotonNetwork.InRoom)
					{
						base.photonView.RPC("FireBallistaRPC", RpcTarget.Others, Array.Empty<object>());
					}
					this.FireLocal();
				}
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.fireStateHash && !this.playerLaunched && (this.playerReadyToFire || this.playerInTrigger))
		{
			float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			GTPlayer instance2 = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition2 = this.GetPlayerBodyCenterPosition(instance2);
			float num2 = Vector3.Dot(playerBodyCenterPosition2 - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			float num3 = 0.25f / this.launchRampDistance;
			float num4 = Mathf.Max(num + num3, num2);
			float num5 = num4 * this.launchRampDistance;
			Vector3 vector4 = this.launchDirection * num5 + this.launchStart.position;
			instance2.transform.position + (vector4 - playerBodyCenterPosition2);
			instance2.transform.position = instance2.transform.position + (vector4 - playerBodyCenterPosition2);
			instance2.SetPlayerVelocity(Vector3.zero);
			if (num4 >= 1f)
			{
				this.playerLaunched = true;
				instance2.SetPlayerVelocity(this.LaunchSpeed * this.launchDirection);
				instance2.SetMaximumSlipThisFrame();
			}
		}
		this.prevStateHash = currentAnimatorStateInfo.shortNameHash;
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0001FAF9 File Offset: 0x0001DCF9
	private void FireLocal()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
		this.playerLaunched = false;
		if (this.debugDrawTrajectoryOnLaunch)
		{
			this.DebugDrawTrajectory(8f);
		}
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0001FB28 File Offset: 0x0001DD28
	private Vector3 GetPlayerBodyCenterPosition(GTPlayer player)
	{
		return player.headCollider.transform.position + Quaternion.Euler(0f, player.headCollider.transform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, -0.15f) + Vector3.down * 0.4f;
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0001FBA4 File Offset: 0x0001DDA4
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = true;
		}
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0001FBD8 File Offset: 0x0001DDD8
	private void OnTriggerExit(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = false;
		}
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0001FC09 File Offset: 0x0001DE09
	[PunRPC]
	public void FireBallistaRPC(PhotonMessageInfo info)
	{
		this.FireLocal();
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0001FC14 File Offset: 0x0001DE14
	private void UpdatePredictionLine()
	{
		float num = 0.033333335f;
		Vector3 vector = this.launchEnd.position;
		Vector3 vector2 = (this.launchEnd.position - this.launchStart.position).normalized * this.LaunchSpeed;
		for (int i = 0; i < 240; i++)
		{
			this.predictionLinePoints[i] = vector;
			vector += vector2 * num;
			vector2 += Vector3.down * 9.8f * num;
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0001FCAE File Offset: 0x0001DEAE
	private IEnumerator DebugDrawTrajectory(float duration)
	{
		this.UpdatePredictionLine();
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
			DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x0001FCC4 File Offset: 0x0001DEC4
	private void OnDrawGizmosSelected()
	{
		if (this.launchStart != null && this.launchEnd != null)
		{
			this.UpdatePredictionLine();
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.launchStart.position, this.launchEnd.position);
			Gizmos.DrawLineList(this.predictionLinePoints);
		}
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0001FD28 File Offset: 0x0001DF28
	public void RefreshButtonColors()
	{
		this.speedZeroButton.isOn = this.currentSpeedIndex == 0;
		this.speedZeroButton.UpdateColor();
		this.speedOneButton.isOn = this.currentSpeedIndex == 1;
		this.speedOneButton.UpdateColor();
		this.speedTwoButton.isOn = this.currentSpeedIndex == 2;
		this.speedTwoButton.UpdateColor();
		this.speedThreeButton.isOn = this.currentSpeedIndex == 3;
		this.speedThreeButton.UpdateColor();
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0001FDB1 File Offset: 0x0001DFB1
	public void SetSpeedIndex(int index)
	{
		this.currentSpeedIndex = index;
		this.RefreshButtonColors();
	}

	// Token: 0x0400064C RID: 1612
	public Animator animator;

	// Token: 0x0400064D RID: 1613
	public Transform launchStart;

	// Token: 0x0400064E RID: 1614
	public Transform launchEnd;

	// Token: 0x0400064F RID: 1615
	public Transform launchBone;

	// Token: 0x04000650 RID: 1616
	public float reloadDelay = 1f;

	// Token: 0x04000651 RID: 1617
	public float loadTime = 1.933f;

	// Token: 0x04000652 RID: 1618
	public float playerMagnetismStrength = 3f;

	// Token: 0x04000653 RID: 1619
	public float launchSpeed = 20f;

	// Token: 0x04000654 RID: 1620
	[Range(0f, 1f)]
	public float pitch;

	// Token: 0x04000655 RID: 1621
	private bool useSpeedOptions;

	// Token: 0x04000656 RID: 1622
	public float[] speedOptions = new float[] { 10f, 15f, 20f, 25f };

	// Token: 0x04000657 RID: 1623
	public int currentSpeedIndex;

	// Token: 0x04000658 RID: 1624
	public GorillaPressableButton speedZeroButton;

	// Token: 0x04000659 RID: 1625
	public GorillaPressableButton speedOneButton;

	// Token: 0x0400065A RID: 1626
	public GorillaPressableButton speedTwoButton;

	// Token: 0x0400065B RID: 1627
	public GorillaPressableButton speedThreeButton;

	// Token: 0x0400065C RID: 1628
	private bool debugDrawTrajectoryOnLaunch;

	// Token: 0x0400065D RID: 1629
	private int loadTriggerHash = Animator.StringToHash("Load");

	// Token: 0x0400065E RID: 1630
	private int fireTriggerHash = Animator.StringToHash("Fire");

	// Token: 0x0400065F RID: 1631
	private int pitchParamHash = Animator.StringToHash("Pitch");

	// Token: 0x04000660 RID: 1632
	private int idleStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000661 RID: 1633
	private int loadStateHash = Animator.StringToHash("Load");

	// Token: 0x04000662 RID: 1634
	private int fireStateHash = Animator.StringToHash("Fire");

	// Token: 0x04000663 RID: 1635
	private int prevStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000664 RID: 1636
	private float fireCompleteTime;

	// Token: 0x04000665 RID: 1637
	private float loadStartTime;

	// Token: 0x04000666 RID: 1638
	private bool playerInTrigger;

	// Token: 0x04000667 RID: 1639
	private bool playerReadyToFire;

	// Token: 0x04000668 RID: 1640
	private bool playerLaunched;

	// Token: 0x04000669 RID: 1641
	private float playerReadyToFireDist = 0.1f;

	// Token: 0x0400066A RID: 1642
	private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

	// Token: 0x0400066B RID: 1643
	private Vector3 launchDirection;

	// Token: 0x0400066C RID: 1644
	private float launchRampDistance;

	// Token: 0x0400066D RID: 1645
	private int collidingLayer;

	// Token: 0x0400066E RID: 1646
	private int notCollidingLayer;

	// Token: 0x0400066F RID: 1647
	private float playerPullInRate;

	// Token: 0x04000670 RID: 1648
	private float appliedAnimatorPitch;

	// Token: 0x04000671 RID: 1649
	private const int predictionLineSamples = 240;

	// Token: 0x04000672 RID: 1650
	private Vector3[] predictionLinePoints = new Vector3[240];
}
