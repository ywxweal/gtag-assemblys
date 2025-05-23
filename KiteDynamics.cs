using System;
using UnityEngine;

// Token: 0x020003A1 RID: 929
public class KiteDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x060015BA RID: 5562 RVA: 0x00069DC4 File Offset: 0x00067FC4
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtPosition = this.grabPt.position;
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x00069E21 File Offset: 0x00068021
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x00069E4C File Offset: 0x0006804C
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x00069E9C File Offset: 0x0006809C
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!enable)
			{
				this.rb.velocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x00069F06 File Offset: 0x00068106
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.rb.useGravity = !enable;
		this.balloonScale = scale;
		this.grabPtPosition = this.grabPt.position;
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x060015BF RID: 5567 RVA: 0x00069F2F File Offset: 0x0006812F
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x00069F4C File Offset: 0x0006814C
	private void FixedUpdate()
	{
		if (this.rb.isKinematic || this.rb.useGravity)
		{
			return;
		}
		if (this.enableDynamics)
		{
			Vector3 vector = (this.grabPt.position - this.grabPtPosition) * 100f;
			vector = Matrix4x4.Rotate(this.ctrlRotation).MultiplyVector(vector);
			this.rb.AddForce(vector, ForceMode.Force);
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			this.rb.velocity = velocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
			base.transform.LookAt(base.transform.position - this.rb.velocity);
		}
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x00002628 File Offset: 0x00000828
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060015C2 RID: 5570 RVA: 0x00067937 File Offset: 0x00065B37
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x0006A026 File Offset: 0x00068226
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		transferOwnership = false;
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x0006A02C File Offset: 0x0006822C
	public bool ReturnStep()
	{
		this.rb.isKinematic = true;
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.grabPt.position, Time.deltaTime * this.returnSpeed);
		return base.transform.position == this.grabPt.position;
	}

	// Token: 0x04001829 RID: 6185
	private Rigidbody rb;

	// Token: 0x0400182A RID: 6186
	private Collider balloonCollider;

	// Token: 0x0400182B RID: 6187
	private Bounds bounds;

	// Token: 0x0400182C RID: 6188
	[SerializeField]
	private float bouyancyMinHeight = 10f;

	// Token: 0x0400182D RID: 6189
	[SerializeField]
	private float bouyancyMaxHeight = 20f;

	// Token: 0x0400182E RID: 6190
	private float bouyancyActualHeight = 20f;

	// Token: 0x0400182F RID: 6191
	[SerializeField]
	private float airResistance = 0.01f;

	// Token: 0x04001830 RID: 6192
	public GameObject knot;

	// Token: 0x04001831 RID: 6193
	private Rigidbody knotRb;

	// Token: 0x04001832 RID: 6194
	public Transform grabPt;

	// Token: 0x04001833 RID: 6195
	private Transform grabPtInitParent;

	// Token: 0x04001834 RID: 6196
	[SerializeField]
	private float maximumVelocity = 2f;

	// Token: 0x04001835 RID: 6197
	private bool enableDynamics;

	// Token: 0x04001836 RID: 6198
	[SerializeField]
	private float balloonScale = 1f;

	// Token: 0x04001837 RID: 6199
	private Vector3 grabPtPosition;

	// Token: 0x04001838 RID: 6200
	[SerializeField]
	private Quaternion ctrlRotation;

	// Token: 0x04001839 RID: 6201
	[SerializeField]
	private float returnSpeed = 50f;
}
