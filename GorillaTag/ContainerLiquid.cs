using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D03 RID: 3331
	[AddComponentMenu("GorillaTag/ContainerLiquid (GTag)")]
	[ExecuteInEditMode]
	public class ContainerLiquid : MonoBehaviour
	{
		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x0600538F RID: 21391 RVA: 0x00195930 File Offset: 0x00193B30
		[DebugReadout]
		public bool isEmpty
		{
			get
			{
				return this.fillAmount <= this.refillThreshold;
			}
		}

		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x06005390 RID: 21392 RVA: 0x00195943 File Offset: 0x00193B43
		// (set) Token: 0x06005391 RID: 21393 RVA: 0x0019594B File Offset: 0x00193B4B
		public Vector3 cupTopWorldPos { get; private set; }

		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x06005392 RID: 21394 RVA: 0x00195954 File Offset: 0x00193B54
		// (set) Token: 0x06005393 RID: 21395 RVA: 0x0019595C File Offset: 0x00193B5C
		public Vector3 bottomLipWorldPos { get; private set; }

		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x06005394 RID: 21396 RVA: 0x00195965 File Offset: 0x00193B65
		// (set) Token: 0x06005395 RID: 21397 RVA: 0x0019596D File Offset: 0x00193B6D
		public Vector3 liquidPlaneWorldPos { get; private set; }

		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x06005396 RID: 21398 RVA: 0x00195976 File Offset: 0x00193B76
		// (set) Token: 0x06005397 RID: 21399 RVA: 0x0019597E File Offset: 0x00193B7E
		public Vector3 liquidPlaneWorldNormal { get; private set; }

		// Token: 0x06005398 RID: 21400 RVA: 0x00195988 File Offset: 0x00193B88
		protected bool IsValidLiquidSurfaceValues()
		{
			return this.meshRenderer != null && this.meshFilter != null && this.spillParticleSystem != null && !string.IsNullOrEmpty(this.liquidColorShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlaneNormalShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlanePositionShaderPropertyName);
		}

		// Token: 0x06005399 RID: 21401 RVA: 0x001959EC File Offset: 0x00193BEC
		protected void InitializeLiquidSurface()
		{
			this.liquidColorShaderProp = Shader.PropertyToID(this.liquidColorShaderPropertyName);
			this.liquidPlaneNormalShaderProp = Shader.PropertyToID(this.liquidPlaneNormalShaderPropertyName);
			this.liquidPlanePositionShaderProp = Shader.PropertyToID(this.liquidPlanePositionShaderPropertyName);
			this.localMeshBounds = this.meshFilter.sharedMesh.bounds;
		}

		// Token: 0x0600539A RID: 21402 RVA: 0x00195A44 File Offset: 0x00193C44
		protected void InitializeParticleSystem()
		{
			this.spillParticleSystem.main.startColor = this.liquidColor;
		}

		// Token: 0x0600539B RID: 21403 RVA: 0x00195A6F File Offset: 0x00193C6F
		protected void Awake()
		{
			this.matPropBlock = new MaterialPropertyBlock();
			this.topVerts = this.GetTopVerts();
		}

		// Token: 0x0600539C RID: 21404 RVA: 0x00195A88 File Offset: 0x00193C88
		protected void OnEnable()
		{
			if (Application.isPlaying)
			{
				base.enabled = this.useLiquidShader && this.IsValidLiquidSurfaceValues();
				if (base.enabled)
				{
					this.InitializeLiquidSurface();
				}
				this.InitializeParticleSystem();
				this.useFloater = this.floater != null;
			}
		}

		// Token: 0x0600539D RID: 21405 RVA: 0x00195ADC File Offset: 0x00193CDC
		protected void LateUpdate()
		{
			this.UpdateRefillTimer();
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			Bounds bounds = this.meshRenderer.bounds;
			Vector3 vector = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
			Vector3 vector2 = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
			this.liquidPlaneWorldPos = Vector3.Lerp(vector, vector2, this.fillAmount);
			Vector3 vector3 = transform.InverseTransformPoint(this.liquidPlaneWorldPos);
			float deltaTime = Time.deltaTime;
			this.temporalWobbleAmp = Vector2.Lerp(this.temporalWobbleAmp, Vector2.zero, deltaTime * this.recovery);
			float num = 6.2831855f * this.wobbleFrequency;
			float num2 = Mathf.Lerp(this.lastSineWave, Mathf.Sin(num * Time.realtimeSinceStartup), deltaTime * Mathf.Clamp(this.lastVelocity.magnitude + this.lastAngularVelocity.magnitude, this.thickness, 10f));
			Vector2 vector4 = this.temporalWobbleAmp * num2;
			this.liquidPlaneWorldNormal = new Vector3(vector4.x, -1f, vector4.y).normalized;
			Vector3 vector5 = transform.InverseTransformDirection(this.liquidPlaneWorldNormal);
			if (this.useLiquidShader)
			{
				this.matPropBlock.SetVector(this.liquidPlaneNormalShaderProp, vector5);
				this.matPropBlock.SetVector(this.liquidPlanePositionShaderProp, vector3);
				this.matPropBlock.SetVector(this.liquidColorShaderProp, this.liquidColor.linear);
				if (this.useLiquidVolume)
				{
					float num3 = MathUtils.Linear(this.fillAmount, 0f, 1f, this.liquidVolumeMinMax.x, this.liquidVolumeMinMax.y);
					this.matPropBlock.SetFloat(Shader.PropertyToID("_LiquidFill"), num3);
				}
				this.meshRenderer.SetPropertyBlock(this.matPropBlock);
			}
			if (this.useFloater)
			{
				float num4 = Mathf.Lerp(this.localMeshBounds.min.y, this.localMeshBounds.max.y, this.fillAmount);
				this.floater.localPosition = this.floater.localPosition.WithY(num4);
			}
			Vector3 vector6 = (this.lastPos - position) / deltaTime;
			Vector3 angularVelocity = GorillaMath.GetAngularVelocity(this.lastRot, rotation);
			this.temporalWobbleAmp.x = this.temporalWobbleAmp.x + Mathf.Clamp((vector6.x + vector6.y * 0.2f + angularVelocity.z + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.temporalWobbleAmp.y = this.temporalWobbleAmp.y + Mathf.Clamp((vector6.z + vector6.y * 0.2f + angularVelocity.x + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.lastPos = position;
			this.lastRot = rotation;
			this.lastSineWave = num2;
			this.lastVelocity = vector6;
			this.lastAngularVelocity = angularVelocity;
			this.meshRenderer.enabled = !this.keepMeshHidden && !this.isEmpty;
			float x = transform.lossyScale.x;
			float num5 = this.localMeshBounds.extents.x * x;
			float y = this.localMeshBounds.extents.y;
			Vector3 vector7 = this.localMeshBounds.center + new Vector3(0f, y, 0f);
			this.cupTopWorldPos = transform.TransformPoint(vector7);
			Vector3 up = transform.up;
			Vector3 vector8 = transform.InverseTransformDirection(Vector3.down);
			float num6 = float.MinValue;
			Vector3 vector9 = Vector3.zero;
			for (int i = 0; i < this.topVerts.Length; i++)
			{
				float num7 = Vector3.Dot(this.topVerts[i], vector8);
				if (num7 > num6)
				{
					num6 = num7;
					vector9 = this.topVerts[i];
				}
			}
			this.bottomLipWorldPos = transform.TransformPoint(vector9);
			float num8 = Mathf.Clamp01((this.liquidPlaneWorldPos.y - this.bottomLipWorldPos.y) / (num5 * 2f));
			bool flag = num8 > 1E-05f;
			ParticleSystem.EmissionModule emission = this.spillParticleSystem.emission;
			emission.enabled = flag;
			if (flag)
			{
				if (!this.spillSoundBankPlayer.isPlaying)
				{
					this.spillSoundBankPlayer.Play();
				}
				this.spillParticleSystem.transform.position = Vector3.Lerp(this.bottomLipWorldPos, this.cupTopWorldPos, num8);
				this.spillParticleSystem.shape.radius = num5 * num8;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				float num9 = num8 * this.maxSpillRate;
				rateOverTime.constant = num9;
				emission.rateOverTime = rateOverTime;
				this.fillAmount -= num9 * deltaTime * 0.01f;
			}
			if (this.isEmpty && !this.wasEmptyLastFrame && !this.emptySoundBankPlayer.isPlaying)
			{
				this.emptySoundBankPlayer.Play();
			}
			else if (!this.isEmpty && this.wasEmptyLastFrame && !this.refillSoundBankPlayer.isPlaying)
			{
				this.refillSoundBankPlayer.Play();
			}
			this.wasEmptyLastFrame = this.isEmpty;
		}

		// Token: 0x0600539E RID: 21406 RVA: 0x0019606C File Offset: 0x0019426C
		public void UpdateRefillTimer()
		{
			if (this.refillDelay < 0f || !this.isEmpty)
			{
				return;
			}
			if (this.refillTimer < 0f)
			{
				this.refillTimer = this.refillDelay;
				this.fillAmount = this.refillAmount;
				return;
			}
			this.refillTimer -= Time.deltaTime;
		}

		// Token: 0x0600539F RID: 21407 RVA: 0x001960C8 File Offset: 0x001942C8
		private Vector3[] GetTopVerts()
		{
			Vector3[] vertices = this.meshFilter.sharedMesh.vertices;
			List<Vector3> list = new List<Vector3>(vertices.Length);
			float num = float.MinValue;
			foreach (Vector3 vector in vertices)
			{
				if (vector.y > num)
				{
					num = vector.y;
				}
			}
			foreach (Vector3 vector2 in vertices)
			{
				if (Mathf.Abs(vector2.y - num) < 0.001f)
				{
					list.Add(vector2);
				}
			}
			return list.ToArray();
		}

		// Token: 0x04005698 RID: 22168
		[Tooltip("Used to determine the world space bounds of the container.")]
		public MeshRenderer meshRenderer;

		// Token: 0x04005699 RID: 22169
		[Tooltip("Used to determine the local space bounds of the container.")]
		public MeshFilter meshFilter;

		// Token: 0x0400569A RID: 22170
		[Tooltip("If you are only using the liquid mesh to calculate the volume of the container and do not need visuals then set this to true.")]
		public bool keepMeshHidden;

		// Token: 0x0400569B RID: 22171
		[Tooltip("The object that will float on top of the liquid.")]
		public Transform floater;

		// Token: 0x0400569C RID: 22172
		public bool useLiquidShader = true;

		// Token: 0x0400569D RID: 22173
		public bool useLiquidVolume;

		// Token: 0x0400569E RID: 22174
		public Vector2 liquidVolumeMinMax = Vector2.up;

		// Token: 0x0400569F RID: 22175
		public string liquidColorShaderPropertyName = "_BaseColor";

		// Token: 0x040056A0 RID: 22176
		public string liquidPlaneNormalShaderPropertyName = "_LiquidPlaneNormal";

		// Token: 0x040056A1 RID: 22177
		public string liquidPlanePositionShaderPropertyName = "_LiquidPlanePosition";

		// Token: 0x040056A2 RID: 22178
		[Tooltip("Emits drips when pouring.")]
		public ParticleSystem spillParticleSystem;

		// Token: 0x040056A3 RID: 22179
		[SoundBankInfo]
		public SoundBankPlayer emptySoundBankPlayer;

		// Token: 0x040056A4 RID: 22180
		[SoundBankInfo]
		public SoundBankPlayer refillSoundBankPlayer;

		// Token: 0x040056A5 RID: 22181
		[SoundBankInfo]
		public SoundBankPlayer spillSoundBankPlayer;

		// Token: 0x040056A6 RID: 22182
		public Color liquidColor = new Color(0.33f, 0.25f, 0.21f, 1f);

		// Token: 0x040056A7 RID: 22183
		[Tooltip("The amount of liquid currently in the container. This value is passed to the shader.")]
		[Range(0f, 1f)]
		public float fillAmount = 0.85f;

		// Token: 0x040056A8 RID: 22184
		[Tooltip("This is what fillAmount will be after automatic refilling.")]
		public float refillAmount = 0.85f;

		// Token: 0x040056A9 RID: 22185
		[Tooltip("Set to a negative value to disable.")]
		public float refillDelay = 10f;

		// Token: 0x040056AA RID: 22186
		[Tooltip("The point that the liquid should be considered empty and should be auto refilled.")]
		public float refillThreshold = 0.1f;

		// Token: 0x040056AB RID: 22187
		public float wobbleMax = 0.2f;

		// Token: 0x040056AC RID: 22188
		public float wobbleFrequency = 1f;

		// Token: 0x040056AD RID: 22189
		public float recovery = 1f;

		// Token: 0x040056AE RID: 22190
		public float thickness = 1f;

		// Token: 0x040056AF RID: 22191
		public float maxSpillRate = 100f;

		// Token: 0x040056B4 RID: 22196
		[DebugReadout]
		private bool wasEmptyLastFrame;

		// Token: 0x040056B5 RID: 22197
		private int liquidColorShaderProp;

		// Token: 0x040056B6 RID: 22198
		private int liquidPlaneNormalShaderProp;

		// Token: 0x040056B7 RID: 22199
		private int liquidPlanePositionShaderProp;

		// Token: 0x040056B8 RID: 22200
		private float refillTimer;

		// Token: 0x040056B9 RID: 22201
		private float lastSineWave;

		// Token: 0x040056BA RID: 22202
		private float lastWobble;

		// Token: 0x040056BB RID: 22203
		private Vector2 temporalWobbleAmp;

		// Token: 0x040056BC RID: 22204
		private Vector3 lastPos;

		// Token: 0x040056BD RID: 22205
		private Vector3 lastVelocity;

		// Token: 0x040056BE RID: 22206
		private Vector3 lastAngularVelocity;

		// Token: 0x040056BF RID: 22207
		private Quaternion lastRot;

		// Token: 0x040056C0 RID: 22208
		private MaterialPropertyBlock matPropBlock;

		// Token: 0x040056C1 RID: 22209
		private Bounds localMeshBounds;

		// Token: 0x040056C2 RID: 22210
		private bool useFloater;

		// Token: 0x040056C3 RID: 22211
		private Vector3[] topVerts;
	}
}
