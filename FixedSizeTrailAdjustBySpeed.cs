using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000547 RID: 1351
public class FixedSizeTrailAdjustBySpeed : MonoBehaviour
{
	// Token: 0x060020BC RID: 8380 RVA: 0x000A451A File Offset: 0x000A271A
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x060020BD RID: 8381 RVA: 0x000A4524 File Offset: 0x000A2724
	private void Setup()
	{
		this._lastPosition = base.transform.position;
		this._rawVelocity = Vector3.zero;
		this._rawSpeed = 0f;
		this._speed = 0f;
		if (this.trail)
		{
			this._initGravity = this.trail.gravity;
			this.trail.applyPhysics = this.adjustPhysics;
		}
		this.LerpTrailColors(0.5f);
	}

	// Token: 0x060020BE RID: 8382 RVA: 0x000A45A0 File Offset: 0x000A27A0
	private void LerpTrailColors(float t = 0.5f)
	{
		GradientColorKey[] colorKeys = this._mixGradient.colorKeys;
		int num = colorKeys.Length;
		for (int i = 0; i < num; i++)
		{
			float num2 = (float)i / (float)(num - 1);
			Color color = this.minColors.Evaluate(num2);
			Color color2 = this.maxColors.Evaluate(num2);
			Color color3 = Color.Lerp(color, color2, t);
			colorKeys[i].color = color3;
			colorKeys[i].time = num2;
		}
		this._mixGradient.colorKeys = colorKeys;
		if (this.trail)
		{
			this.trail.renderer.colorGradient = this._mixGradient;
		}
	}

	// Token: 0x060020BF RID: 8383 RVA: 0x000A4640 File Offset: 0x000A2840
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		this._rawVelocity = (position - this._lastPosition) / deltaTime;
		this._rawSpeed = this._rawVelocity.magnitude;
		if (this._rawSpeed > this.retractMin)
		{
			this._speed += this.expandSpeed * deltaTime;
		}
		if (this._rawSpeed <= this.retractMin)
		{
			this._speed -= this.retractSpeed * deltaTime;
		}
		if (this._speed > this.maxSpeed)
		{
			this._speed = this.maxSpeed;
		}
		this._speed = Mathf.Lerp(this._lastSpeed, this._speed, 0.5f);
		if (this._speed < 0.01f)
		{
			this._speed = 0f;
		}
		this.AdjustTrail();
		this._lastSpeed = this._speed;
		this._lastPosition = position;
	}

	// Token: 0x060020C0 RID: 8384 RVA: 0x000A4738 File Offset: 0x000A2938
	private void AdjustTrail()
	{
		if (!this.trail)
		{
			return;
		}
		float num = MathUtils.Linear(this._speed, this.minSpeed, this.maxSpeed, 0f, 1f);
		float num2 = MathUtils.Linear(num, 0f, 1f, this.minLength, this.maxLength);
		this.trail.length = num2;
		this.LerpTrailColors(num);
		if (this.adjustPhysics)
		{
			Transform transform = base.transform;
			Vector3 vector = transform.forward * this.gravityOffset.z + transform.right * this.gravityOffset.x + transform.up * this.gravityOffset.y;
			Vector3 vector2 = (this._initGravity + vector) * (1f - num);
			this.trail.gravity = Vector3.Lerp(Vector3.zero, vector2, 0.5f);
		}
	}

	// Token: 0x040024D2 RID: 9426
	public FixedSizeTrail trail;

	// Token: 0x040024D3 RID: 9427
	public bool adjustPhysics = true;

	// Token: 0x040024D4 RID: 9428
	private Vector3 _rawVelocity;

	// Token: 0x040024D5 RID: 9429
	private float _rawSpeed;

	// Token: 0x040024D6 RID: 9430
	private float _speed;

	// Token: 0x040024D7 RID: 9431
	private float _lastSpeed;

	// Token: 0x040024D8 RID: 9432
	private Vector3 _lastPosition;

	// Token: 0x040024D9 RID: 9433
	private Vector3 _initGravity;

	// Token: 0x040024DA RID: 9434
	public Vector3 gravityOffset = Vector3.zero;

	// Token: 0x040024DB RID: 9435
	[Space]
	public float retractMin = 0.5f;

	// Token: 0x040024DC RID: 9436
	[Space]
	[FormerlySerializedAs("sizeIncreaseSpeed")]
	public float expandSpeed = 16f;

	// Token: 0x040024DD RID: 9437
	[FormerlySerializedAs("sizeDecreaseSpeed")]
	public float retractSpeed = 4f;

	// Token: 0x040024DE RID: 9438
	[Space]
	public float minSpeed;

	// Token: 0x040024DF RID: 9439
	public float minLength = 1f;

	// Token: 0x040024E0 RID: 9440
	public Gradient minColors = GradientHelper.FromColor(new Color(0f, 1f, 1f, 1f));

	// Token: 0x040024E1 RID: 9441
	[Space]
	public float maxSpeed = 10f;

	// Token: 0x040024E2 RID: 9442
	public float maxLength = 8f;

	// Token: 0x040024E3 RID: 9443
	public Gradient maxColors = GradientHelper.FromColor(new Color(1f, 1f, 0f, 1f));

	// Token: 0x040024E4 RID: 9444
	[Space]
	[SerializeField]
	private Gradient _mixGradient = new Gradient
	{
		colorKeys = new GradientColorKey[8],
		alphaKeys = Array.Empty<GradientAlphaKey>()
	};

	// Token: 0x02000548 RID: 1352
	[Serializable]
	public struct GradientKey
	{
		// Token: 0x060020C2 RID: 8386 RVA: 0x000A490D File Offset: 0x000A2B0D
		public GradientKey(Color color, float time)
		{
			this.color = color;
			this.time = time;
		}

		// Token: 0x040024E5 RID: 9445
		public Color color;

		// Token: 0x040024E6 RID: 9446
		public float time;
	}
}
