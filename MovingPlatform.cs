using System;
using GTMathUtil;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000684 RID: 1668
public class MovingPlatform : BasePlatform
{
	// Token: 0x060029A7 RID: 10663 RVA: 0x000CE924 File Offset: 0x000CCB24
	public float InitTimeOffset()
	{
		return this.startPercentage * this.cycleLength;
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x000CE933 File Offset: 0x000CCB33
	private long InitTimeOffsetMs()
	{
		return (long)(this.InitTimeOffset() * 1000f);
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x000CE942 File Offset: 0x000CCB42
	private long NetworkTimeMs()
	{
		if (PhotonNetwork.InRoom)
		{
			return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue) + (ulong)this.InitTimeOffsetMs());
		}
		return (long)(Time.time * 1000f);
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x000CE96B File Offset: 0x000CCB6B
	private long CycleLengthMs()
	{
		return (long)(this.cycleLength * 1000f);
	}

	// Token: 0x060029AB RID: 10667 RVA: 0x000CE97C File Offset: 0x000CCB7C
	public double PlatformTime()
	{
		long num = this.NetworkTimeMs();
		long num2 = this.CycleLengthMs();
		return (double)(num - num / num2 * num2) / 1000.0;
	}

	// Token: 0x060029AC RID: 10668 RVA: 0x000CE9A7 File Offset: 0x000CCBA7
	public int CycleCount()
	{
		return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
	}

	// Token: 0x060029AD RID: 10669 RVA: 0x000CE9B8 File Offset: 0x000CCBB8
	public float CycleCompletionPercent()
	{
		float num = (float)(this.PlatformTime() / (double)this.cycleLength);
		num = Mathf.Clamp(num, 0f, 1f);
		if (this.startDelay > 0f)
		{
			float num2 = this.startDelay / this.cycleLength;
			if (num <= num2)
			{
				num = 0f;
			}
			else
			{
				num = (num - num2) / (1f - num2);
			}
		}
		return num;
	}

	// Token: 0x060029AE RID: 10670 RVA: 0x000CEA1A File Offset: 0x000CCC1A
	public bool CycleForward()
	{
		return (this.CycleCount() + (this.startNextCycle ? 1 : 0)) % 2 == 0;
	}

	// Token: 0x060029AF RID: 10671 RVA: 0x000CEA34 File Offset: 0x000CCC34
	private void Awake()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.initLocalRotation = base.transform.localRotation;
		if (this.pivot != null)
		{
			this.initOffset = this.pivot.transform.position - this.startXf.transform.position;
		}
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x060029B0 RID: 10672 RVA: 0x000CEB04 File Offset: 0x000CCD04
	private void OnEnable()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		base.transform.localRotation = this.initLocalRotation;
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x000CEB8D File Offset: 0x000CCD8D
	private Vector3 UpdatePointToPoint()
	{
		return Vector3.Lerp(this.startPos, this.endPos, this.smoothedPercent);
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x000CEBA8 File Offset: 0x000CCDA8
	private Vector3 UpdateArc()
	{
		float num = Mathf.Lerp(this.rotateStartAmt, this.rotateStartAmt + this.rotateAmt, this.smoothedPercent);
		Quaternion quaternion = this.initLocalRotation;
		Vector3 vector = Quaternion.AngleAxis(num, Vector3.forward) * this.initOffset;
		return this.pivot.transform.position + vector;
	}

	// Token: 0x060029B3 RID: 10675 RVA: 0x000CEC06 File Offset: 0x000CCE06
	private Quaternion UpdateRotation()
	{
		return Quaternion.Slerp(this.startRot, this.endRot, this.smoothedPercent);
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x000CEC1F File Offset: 0x000CCE1F
	private Quaternion UpdateContinuousRotation()
	{
		return Quaternion.AngleAxis(this.smoothedPercent * 360f, Vector3.up) * base.transform.parent.rotation;
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x000CEC4C File Offset: 0x000CCE4C
	private void SetupContext()
	{
		double time = PhotonNetwork.Time;
		if (this.lastServerTime == time)
		{
			this.dtSinceServerUpdate += Time.fixedDeltaTime;
		}
		else
		{
			this.dtSinceServerUpdate = 0f;
			this.lastServerTime = time;
		}
		float num = this.currT;
		this.currT = this.CycleCompletionPercent();
		this.currForward = this.CycleForward();
		this.percent = this.currT;
		if (this.reverseDirOnCycle)
		{
			this.percent = (this.currForward ? this.currT : (1f - this.currT));
		}
		if (this.reverseDir)
		{
			this.percent = 1f - this.percent;
		}
		this.smoothedPercent = this.percent;
		this.lastNT = time;
		this.lastT = Time.time;
	}

	// Token: 0x060029B6 RID: 10678 RVA: 0x000CED1C File Offset: 0x000CCF1C
	private void Update()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.SetupContext();
		Vector3 vector = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		bool flag = false;
		switch (this.platformType)
		{
		case MovingPlatform.PlatformType.PointToPoint:
			vector = this.UpdatePointToPoint();
			break;
		case MovingPlatform.PlatformType.Arc:
			vector = this.UpdateArc();
			flag = true;
			break;
		case MovingPlatform.PlatformType.Rotation:
			quaternion = this.UpdateRotation();
			flag = true;
			break;
		case MovingPlatform.PlatformType.ContinuousRotation:
			quaternion = this.UpdateContinuousRotation();
			flag = true;
			break;
		}
		if (!this.debugMovement)
		{
			this.lastPos = this.rb.position;
			this.lastRot = this.rb.rotation;
			if (this.platformType != MovingPlatform.PlatformType.Rotation)
			{
				this.rb.MovePosition(vector);
			}
			if (flag)
			{
				this.rb.MoveRotation(quaternion);
			}
		}
		else
		{
			this.lastPos = base.transform.position;
			this.lastRot = base.transform.rotation;
			base.transform.position = vector;
			if (flag)
			{
				base.transform.rotation = quaternion;
			}
		}
		this.deltaPosition = vector - this.lastPos;
	}

	// Token: 0x060029B7 RID: 10679 RVA: 0x000CEE3D File Offset: 0x000CD03D
	public Vector3 ThisFrameMovement()
	{
		return this.deltaPosition;
	}

	// Token: 0x04002EAD RID: 11949
	public MovingPlatform.PlatformType platformType;

	// Token: 0x04002EAE RID: 11950
	public float cycleLength;

	// Token: 0x04002EAF RID: 11951
	public float smoothingHalflife = 0.1f;

	// Token: 0x04002EB0 RID: 11952
	public float rotateStartAmt;

	// Token: 0x04002EB1 RID: 11953
	public float rotateAmt;

	// Token: 0x04002EB2 RID: 11954
	public bool reverseDirOnCycle = true;

	// Token: 0x04002EB3 RID: 11955
	public bool reverseDir;

	// Token: 0x04002EB4 RID: 11956
	private CriticalSpringDamper springCD = new CriticalSpringDamper();

	// Token: 0x04002EB5 RID: 11957
	private Rigidbody rb;

	// Token: 0x04002EB6 RID: 11958
	public Transform startXf;

	// Token: 0x04002EB7 RID: 11959
	public Transform endXf;

	// Token: 0x04002EB8 RID: 11960
	public Vector3 platformInitLocalPos;

	// Token: 0x04002EB9 RID: 11961
	private Vector3 startPos;

	// Token: 0x04002EBA RID: 11962
	private Vector3 endPos;

	// Token: 0x04002EBB RID: 11963
	private Quaternion startRot;

	// Token: 0x04002EBC RID: 11964
	private Quaternion endRot;

	// Token: 0x04002EBD RID: 11965
	public float startPercentage;

	// Token: 0x04002EBE RID: 11966
	public float startDelay;

	// Token: 0x04002EBF RID: 11967
	public bool startNextCycle;

	// Token: 0x04002EC0 RID: 11968
	public Transform pivot;

	// Token: 0x04002EC1 RID: 11969
	private Quaternion initLocalRotation;

	// Token: 0x04002EC2 RID: 11970
	private Vector3 initOffset;

	// Token: 0x04002EC3 RID: 11971
	private float currT;

	// Token: 0x04002EC4 RID: 11972
	private float percent;

	// Token: 0x04002EC5 RID: 11973
	private float smoothedPercent = -1f;

	// Token: 0x04002EC6 RID: 11974
	private bool currForward;

	// Token: 0x04002EC7 RID: 11975
	private float dtSinceServerUpdate;

	// Token: 0x04002EC8 RID: 11976
	private double lastServerTime;

	// Token: 0x04002EC9 RID: 11977
	public Vector3 currentVelocity;

	// Token: 0x04002ECA RID: 11978
	public Vector3 rotationalAxis;

	// Token: 0x04002ECB RID: 11979
	public float angularVelocity;

	// Token: 0x04002ECC RID: 11980
	public Vector3 rotationPivot;

	// Token: 0x04002ECD RID: 11981
	public Vector3 lastPos;

	// Token: 0x04002ECE RID: 11982
	public Quaternion lastRot;

	// Token: 0x04002ECF RID: 11983
	public Vector3 deltaPosition;

	// Token: 0x04002ED0 RID: 11984
	public bool debugMovement;

	// Token: 0x04002ED1 RID: 11985
	private double lastNT;

	// Token: 0x04002ED2 RID: 11986
	private float lastT;

	// Token: 0x02000685 RID: 1669
	public enum PlatformType
	{
		// Token: 0x04002ED4 RID: 11988
		PointToPoint,
		// Token: 0x04002ED5 RID: 11989
		Arc,
		// Token: 0x04002ED6 RID: 11990
		Rotation,
		// Token: 0x04002ED7 RID: 11991
		Child,
		// Token: 0x04002ED8 RID: 11992
		ContinuousRotation
	}
}
