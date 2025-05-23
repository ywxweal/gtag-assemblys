using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000392 RID: 914
public class BalloonDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x0600152A RID: 5418 RVA: 0x00067508 File Offset: 0x00065708
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x00067554 File Offset: 0x00065754
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x0600152C RID: 5420 RVA: 0x00067580 File Offset: 0x00065780
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x0600152D RID: 5421 RVA: 0x000675D0 File Offset: 0x000657D0
	private void ApplyBouyancyForce()
	{
		float num = this.bouyancyActualHeight + Mathf.Sin(Time.time) * this.varianceMaxheight;
		float num2 = (num - base.transform.position.y) / num;
		float num3 = this.bouyancyForce * num2 * this.balloonScale;
		this.rb.AddForce(new Vector3(0f, num3, 0f), ForceMode.Acceleration);
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x00067638 File Offset: 0x00065838
	private void ApplyUpRightForce()
	{
		Vector3 vector = Vector3.Cross(base.transform.up, Vector3.up) * this.upRightTorque * this.balloonScale;
		this.rb.AddTorque(vector);
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x00067680 File Offset: 0x00065880
	private void ApplyAntiSpinForce()
	{
		Vector3 vector = this.rb.transform.InverseTransformDirection(this.rb.angularVelocity);
		this.rb.AddRelativeTorque(0f, -vector.y * this.antiSpinTorque, 0f);
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x000676CC File Offset: 0x000658CC
	private void ApplyAirResistance()
	{
		this.rb.velocity *= 1f - this.airResistance;
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x000676F0 File Offset: 0x000658F0
	private void ApplyDistanceConstraint()
	{
		this.knot.transform.position - base.transform.position;
		Vector3 vector = this.grabPt.transform.position - this.knot.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		float num = this.stringLength * this.balloonScale;
		if (magnitude > num)
		{
			Vector3 vector2 = Vector3.Dot(this.knotRb.velocity, normalized) * normalized;
			float num2 = magnitude - num;
			float num3 = num2 / Time.fixedDeltaTime;
			if (vector2.magnitude < num3)
			{
				float num4 = num3 - vector2.magnitude;
				float num5 = Mathf.Clamp01(num2 / this.stringStretch);
				Vector3 vector3 = Mathf.Lerp(0f, num4, num5 * num5) * normalized * this.stringStrength;
				this.rb.AddForceAtPosition(vector3, this.knot.transform.position, ForceMode.VelocityChange);
			}
		}
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x000677FC File Offset: 0x000659FC
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		bool flag = !this.enableDynamics && enable;
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!kinematic && flag)
			{
				this.rb.velocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x00067877 File Offset: 0x00065A77
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.enableDistanceConstraints = enable;
		this.balloonScale = scale;
	}

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06001534 RID: 5428 RVA: 0x00067887 File Offset: 0x00065A87
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x000678A4 File Offset: 0x00065AA4
	private void FixedUpdate()
	{
		if (this.enableDynamics && !this.rb.isKinematic)
		{
			this.ApplyBouyancyForce();
			if (this.antiSpinTorque > 0f)
			{
				this.ApplyAntiSpinForce();
			}
			this.ApplyUpRightForce();
			this.ApplyAirResistance();
			if (this.enableDistanceConstraints)
			{
				this.ApplyDistanceConstraint();
			}
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			this.rb.velocity = velocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
		}
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x00002628 File Offset: 0x00000828
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x00067937 File Offset: 0x00065B37
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x00067940 File Offset: 0x00065B40
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		if (!other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
		{
			return;
		}
		if (!this.rb)
		{
			return;
		}
		transferOwnership = true;
		TransformFollow component = other.gameObject.GetComponent<TransformFollow>();
		if (!component)
		{
			return;
		}
		Vector3 vector = (component.transform.position - component.prevPos) / Time.deltaTime;
		force = vector * this.bopSpeed;
		force = Mathf.Min(this.maximumVelocity, force.magnitude) * force.normalized * this.balloonScale;
		if (this.bopSpeedCap > 0f && force.IsLongerThan(this.bopSpeedCap))
		{
			force = force.normalized * this.bopSpeedCap;
		}
		collisionPt = other.ClosestPointOnBounds(base.transform.position);
		this.rb.AddForceAtPosition(force, collisionPt, ForceMode.VelocityChange);
		if (this.balloonBopSource != null)
		{
			this.balloonBopSource.GTPlay();
		}
		GorillaTriggerColliderHandIndicator component2 = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component2 != null)
		{
			float num = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			GorillaTagger.Instance.StartVibration(component2.isLeftHand, num, fixedDeltaTime);
		}
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x00047642 File Offset: 0x00045842
	public bool ReturnStep()
	{
		return true;
	}

	// Token: 0x0400178C RID: 6028
	private Rigidbody rb;

	// Token: 0x0400178D RID: 6029
	private Collider balloonCollider;

	// Token: 0x0400178E RID: 6030
	private Bounds bounds;

	// Token: 0x0400178F RID: 6031
	public float bouyancyForce = 1f;

	// Token: 0x04001790 RID: 6032
	public float bouyancyMinHeight = 10f;

	// Token: 0x04001791 RID: 6033
	public float bouyancyMaxHeight = 20f;

	// Token: 0x04001792 RID: 6034
	private float bouyancyActualHeight = 20f;

	// Token: 0x04001793 RID: 6035
	public float varianceMaxheight = 5f;

	// Token: 0x04001794 RID: 6036
	public float airResistance = 0.01f;

	// Token: 0x04001795 RID: 6037
	public GameObject knot;

	// Token: 0x04001796 RID: 6038
	private Rigidbody knotRb;

	// Token: 0x04001797 RID: 6039
	public Transform grabPt;

	// Token: 0x04001798 RID: 6040
	private Transform grabPtInitParent;

	// Token: 0x04001799 RID: 6041
	public float stringLength = 2f;

	// Token: 0x0400179A RID: 6042
	public float stringStrength = 0.9f;

	// Token: 0x0400179B RID: 6043
	public float stringStretch = 0.1f;

	// Token: 0x0400179C RID: 6044
	public float maximumVelocity = 2f;

	// Token: 0x0400179D RID: 6045
	public float upRightTorque = 1f;

	// Token: 0x0400179E RID: 6046
	public float antiSpinTorque;

	// Token: 0x0400179F RID: 6047
	private bool enableDynamics;

	// Token: 0x040017A0 RID: 6048
	private bool enableDistanceConstraints;

	// Token: 0x040017A1 RID: 6049
	public float balloonScale = 1f;

	// Token: 0x040017A2 RID: 6050
	public float bopSpeed = 1f;

	// Token: 0x040017A3 RID: 6051
	public float bopSpeedCap;

	// Token: 0x040017A4 RID: 6052
	[SerializeField]
	private AudioSource balloonBopSource;
}
