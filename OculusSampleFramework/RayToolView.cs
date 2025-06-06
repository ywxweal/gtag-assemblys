﻿using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BEA RID: 3050
	public class RayToolView : MonoBehaviour, InteractableToolView
	{
		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x06004B51 RID: 19281 RVA: 0x00165917 File Offset: 0x00163B17
		// (set) Token: 0x06004B52 RID: 19282 RVA: 0x00165924 File Offset: 0x00163B24
		public bool EnableState
		{
			get
			{
				return this._lineRenderer.enabled;
			}
			set
			{
				this._targetTransform.gameObject.SetActive(value);
				this._lineRenderer.enabled = value;
			}
		}

		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x06004B53 RID: 19283 RVA: 0x00165943 File Offset: 0x00163B43
		// (set) Token: 0x06004B54 RID: 19284 RVA: 0x0016594B File Offset: 0x00163B4B
		public bool ToolActivateState
		{
			get
			{
				return this._toolActivateState;
			}
			set
			{
				this._toolActivateState = value;
				this._lineRenderer.colorGradient = (this._toolActivateState ? this._highLightColorGradient : this._oldColorGradient);
			}
		}

		// Token: 0x06004B55 RID: 19285 RVA: 0x00165978 File Offset: 0x00163B78
		private void Awake()
		{
			this._lineRenderer.positionCount = 25;
			this._oldColorGradient = this._lineRenderer.colorGradient;
			this._highLightColorGradient = new Gradient();
			this._highLightColorGradient.SetKeys(new GradientColorKey[]
			{
				new GradientColorKey(new Color(0.9f, 0.9f, 0.9f), 0f),
				new GradientColorKey(new Color(0.9f, 0.9f, 0.9f), 1f)
			}, new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			});
		}

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x06004B56 RID: 19286 RVA: 0x00165A3B File Offset: 0x00163C3B
		// (set) Token: 0x06004B57 RID: 19287 RVA: 0x00165A43 File Offset: 0x00163C43
		public InteractableTool InteractableTool { get; set; }

		// Token: 0x06004B58 RID: 19288 RVA: 0x00165A4C File Offset: 0x00163C4C
		public void SetFocusedInteractable(Interactable interactable)
		{
			if (interactable == null)
			{
				this._focusedTransform = null;
				return;
			}
			this._focusedTransform = interactable.transform;
		}

		// Token: 0x06004B59 RID: 19289 RVA: 0x00165A6C File Offset: 0x00163C6C
		private void Update()
		{
			Vector3 position = this.InteractableTool.ToolTransform.position;
			Vector3 forward = this.InteractableTool.ToolTransform.forward;
			Vector3 vector = ((this._focusedTransform != null) ? this._focusedTransform.position : (position + forward * 3f));
			float magnitude = (vector - position).magnitude;
			Vector3 vector2 = position;
			Vector3 vector3 = position + forward * magnitude * 0.3333333f;
			Vector3 vector4 = position + forward * magnitude * 0.6666667f;
			Vector3 vector5 = vector;
			for (int i = 0; i < 25; i++)
			{
				this.linePositions[i] = RayToolView.GetPointOnBezierCurve(vector2, vector3, vector4, vector5, (float)i / 25f);
			}
			this._lineRenderer.SetPositions(this.linePositions);
			this._targetTransform.position = vector;
		}

		// Token: 0x06004B5A RID: 19290 RVA: 0x00165B64 File Offset: 0x00163D64
		public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			float num2 = num * num;
			float num3 = t * t;
			return num * num2 * p0 + 3f * num2 * t * p1 + 3f * num * num3 * p2 + t * num3 * p3;
		}

		// Token: 0x04004DEE RID: 19950
		private const int NUM_RAY_LINE_POSITIONS = 25;

		// Token: 0x04004DEF RID: 19951
		private const float DEFAULT_RAY_CAST_DISTANCE = 3f;

		// Token: 0x04004DF0 RID: 19952
		[SerializeField]
		private Transform _targetTransform;

		// Token: 0x04004DF1 RID: 19953
		[SerializeField]
		private LineRenderer _lineRenderer;

		// Token: 0x04004DF2 RID: 19954
		private bool _toolActivateState;

		// Token: 0x04004DF3 RID: 19955
		private Transform _focusedTransform;

		// Token: 0x04004DF4 RID: 19956
		private Vector3[] linePositions = new Vector3[25];

		// Token: 0x04004DF5 RID: 19957
		private Gradient _oldColorGradient;

		// Token: 0x04004DF6 RID: 19958
		private Gradient _highLightColorGradient;
	}
}
