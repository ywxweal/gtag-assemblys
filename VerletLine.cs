using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006F6 RID: 1782
[DisallowMultipleComponent]
public class VerletLine : MonoBehaviour
{
	// Token: 0x06002C5D RID: 11357 RVA: 0x000DAD38 File Offset: 0x000D8F38
	private void Awake()
	{
		this._nodes = new VerletLine.LineNode[this.segmentNumber];
		this._positions = new Vector3[this.segmentNumber];
		for (int i = 0; i < this.segmentNumber; i++)
		{
			float num = (float)i / (float)(this.segmentNumber - 1);
			Vector3 vector = Vector3.Lerp(this.lineStart.position, this.lineEnd.position, num);
			this._nodes[i] = new VerletLine.LineNode
			{
				position = vector,
				lastPosition = vector,
				acceleration = this.gravity
			};
		}
		this.line.positionCount = this._nodes.Length;
		this.endRigidbody = this.lineEnd.GetComponent<Rigidbody>();
		if (this.endRigidbody)
		{
			this.endRigidbody.maxLinearVelocity = this.endMaxSpeed;
			this.endRigidbodyParent = this.endRigidbody.transform.parent;
			this.rigidBodyStartingLocalPosition = this.endRigidbody.transform.localPosition;
			this.endRigidbody.transform.parent = null;
			this.endRigidbody.gameObject.SetActive(false);
		}
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x000DAE78 File Offset: 0x000D9078
	private void OnEnable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(true);
			this.endRigidbody.transform.localPosition = this.endRigidbodyParent.TransformPoint(this.rigidBodyStartingLocalPosition);
		}
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x000DAEC4 File Offset: 0x000D90C4
	private void OnDisable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x000DAEE4 File Offset: 0x000D90E4
	public void SetLength(float total, float delay = 0f)
	{
		this.segmentTargetLength = total / (float)this.segmentNumber;
		if (this.segmentTargetLength < this.segmentMinLength)
		{
			this.segmentTargetLength = this.segmentMinLength;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x000DAF4C File Offset: 0x000D914C
	public void AddSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength + amount;
		if (this.segmentTargetLength <= 0f)
		{
			return;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06002C62 RID: 11362 RVA: 0x000DAFA8 File Offset: 0x000D91A8
	public void RemoveSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength - amount;
		if (this.segmentTargetLength <= this.segmentMinLength)
		{
			this.segmentTargetLength = (this.segmentLength = this.segmentMinLength);
			return;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06002C63 RID: 11363 RVA: 0x000DAFFD File Offset: 0x000D91FD
	private IEnumerator ResizeAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		yield break;
	}

	// Token: 0x06002C64 RID: 11364 RVA: 0x000DB00C File Offset: 0x000D920C
	private void Update()
	{
		if (this.segmentLength.Approx(this.segmentTargetLength, 0.1f))
		{
			this.segmentLength = this.segmentTargetLength;
			return;
		}
		this.segmentLength = Mathf.Lerp(this.segmentLength, this.segmentTargetLength, this.resizeSpeed * this.resizeScale * Time.deltaTime);
		if (this.scaleLineWidth)
		{
			this.line.widthMultiplier = base.transform.lossyScale.x;
		}
	}

	// Token: 0x06002C65 RID: 11365 RVA: 0x000DB08C File Offset: 0x000D928C
	public void ForceTotalLength(float totalLength)
	{
		float num = totalLength / (float)((this.segmentNumber < 1) ? 1 : this.segmentNumber);
		this.segmentLength = (this.segmentTargetLength = num);
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x06002C66 RID: 11366 RVA: 0x000DB0D4 File Offset: 0x000D92D4
	private void FixedUpdate()
	{
		for (int i = 0; i < this._nodes.Length; i++)
		{
			VerletLine.Simulate(ref this._nodes[i], Time.fixedDeltaTime);
		}
		for (int j = 0; j < this.simIterations; j++)
		{
			for (int k = 0; k < this._nodes.Length - 1; k++)
			{
				VerletLine.LimitDistance(ref this._nodes[k], ref this._nodes[k + 1], this.segmentLength);
			}
		}
		this._nodes[0].position = this.lineStart.position;
		if (this.endRigidbody)
		{
			if (this.onlyPullAtEdges)
			{
				if ((this.endRigidbody.transform.position - this.lineStart.position).IsLongerThan(this.totalLineLength))
				{
					Vector3 vector = this.lineStart.position + (this.endRigidbody.transform.position - this.lineStart.position).normalized * this.totalLineLength;
					this.endRigidbody.velocity += (vector - this.endRigidbody.transform.position) / Time.fixedDeltaTime;
					if (this.endRigidbody.velocity.IsLongerThan(this.endMaxSpeed))
					{
						this.endRigidbody.velocity = this.endRigidbody.velocity.normalized * this.endMaxSpeed;
					}
				}
			}
			else
			{
				VerletLine.LineNode[] nodes = this._nodes;
				Vector3 vector2 = (nodes[nodes.Length - 1].position - this.lineEnd.position) * (this.tension * this.tensionScale);
				Quaternion rotation = this.endRigidbody.rotation;
				VerletLine.LineNode[] nodes2 = this._nodes;
				Vector3 position = nodes2[nodes2.Length - 1].position;
				VerletLine.LineNode[] nodes3 = this._nodes;
				Quaternion.LookRotation(position - nodes3[nodes3.Length - 2].position);
				if (!this.endRigidbody.isKinematic)
				{
					this.endRigidbody.AddForceAtPosition(vector2, this.endRigidbody.transform.TransformPoint(this.endLineAnchorLocalPosition));
				}
			}
		}
		VerletLine.LineNode[] nodes4 = this._nodes;
		nodes4[nodes4.Length - 1].position = this.lineEnd.position;
		for (int l = 0; l < this._nodes.Length; l++)
		{
			this._positions[l] = this._nodes[l].position;
		}
		this.line.SetPositions(this._positions);
	}

	// Token: 0x06002C67 RID: 11367 RVA: 0x000DB394 File Offset: 0x000D9594
	private static void Simulate(ref VerletLine.LineNode p, float dt)
	{
		Vector3 position = p.position;
		p.position += p.position - p.lastPosition + p.acceleration * (dt * dt);
		p.lastPosition = position;
	}

	// Token: 0x06002C68 RID: 11368 RVA: 0x000DB3EC File Offset: 0x000D95EC
	private static void LimitDistance(ref VerletLine.LineNode p1, ref VerletLine.LineNode p2, float restLength)
	{
		Vector3 vector = p2.position - p1.position;
		float num = vector.magnitude + 1E-05f;
		float num2 = (num - restLength) / num;
		p1.position += vector * (num2 * 0.5f);
		p2.position -= vector * (num2 * 0.5f);
	}

	// Token: 0x040032A5 RID: 12965
	public Transform lineStart;

	// Token: 0x040032A6 RID: 12966
	public Transform lineEnd;

	// Token: 0x040032A7 RID: 12967
	[Space]
	public LineRenderer line;

	// Token: 0x040032A8 RID: 12968
	public Rigidbody endRigidbody;

	// Token: 0x040032A9 RID: 12969
	public Transform endRigidbodyParent;

	// Token: 0x040032AA RID: 12970
	public Vector3 endLineAnchorLocalPosition;

	// Token: 0x040032AB RID: 12971
	private Vector3 rigidBodyStartingLocalPosition;

	// Token: 0x040032AC RID: 12972
	[Space]
	public int segmentNumber = 10;

	// Token: 0x040032AD RID: 12973
	public float segmentLength = 0.03f;

	// Token: 0x040032AE RID: 12974
	public float segmentTargetLength = 0.03f;

	// Token: 0x040032AF RID: 12975
	public float segmentMaxLength = 0.03f;

	// Token: 0x040032B0 RID: 12976
	public float segmentMinLength = 0.03f;

	// Token: 0x040032B1 RID: 12977
	[Space]
	public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

	// Token: 0x040032B2 RID: 12978
	public int simIterations = 6;

	// Token: 0x040032B3 RID: 12979
	public float tension = 10f;

	// Token: 0x040032B4 RID: 12980
	public float tensionScale = 1f;

	// Token: 0x040032B5 RID: 12981
	public float endMaxSpeed = 48f;

	// Token: 0x040032B6 RID: 12982
	[FormerlySerializedAs("lerpSpeed")]
	[Space]
	public float resizeSpeed = 1f;

	// Token: 0x040032B7 RID: 12983
	public float resizeScale = 1f;

	// Token: 0x040032B8 RID: 12984
	[NonSerialized]
	private VerletLine.LineNode[] _nodes = new VerletLine.LineNode[0];

	// Token: 0x040032B9 RID: 12985
	[NonSerialized]
	private Vector3[] _positions = new Vector3[0];

	// Token: 0x040032BA RID: 12986
	private float totalLineLength;

	// Token: 0x040032BB RID: 12987
	[SerializeField]
	private bool onlyPullAtEdges;

	// Token: 0x040032BC RID: 12988
	[SerializeField]
	private bool scaleLineWidth = true;

	// Token: 0x020006F7 RID: 1783
	[Serializable]
	public struct LineNode
	{
		// Token: 0x040032BD RID: 12989
		public Vector3 position;

		// Token: 0x040032BE RID: 12990
		public Vector3 lastPosition;

		// Token: 0x040032BF RID: 12991
		public Vector3 acceleration;
	}
}
