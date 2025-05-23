using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000C2 RID: 194
public class MetroSpotlight : MonoBehaviour
{
	// Token: 0x060004DD RID: 1245 RVA: 0x0001C48C File Offset: 0x0001A68C
	public void Tick()
	{
		if (!this._light)
		{
			return;
		}
		if (!this._target)
		{
			return;
		}
		this._time += this.speed * Time.deltaTime * Time.deltaTime;
		Vector3 position = this._target.position;
		Vector3 normalized = (position - this._light.position).normalized;
		Vector3 vector = Vector3.Cross(normalized, this._blimp.forward);
		Vector3 vector2 = Vector3.Cross(normalized, vector);
		Vector3 vector3 = MetroSpotlight.Figure8(position, vector, vector2, this._radius, this._time, this._offset, this._theta);
		this._light.LookAt(vector3);
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0001C540 File Offset: 0x0001A740
	private static Vector3 Figure8(Vector3 origin, Vector3 xDir, Vector3 yDir, float scale, float t, float offset, float theta)
	{
		float num = 2f / (3f - Mathf.Cos(2f * (t + offset)));
		float num2 = scale * num * Mathf.Cos(t + offset);
		float num3 = scale * num * Mathf.Sin(2f * (t + offset)) / 2f;
		Vector3 vector = Vector3.Cross(xDir, yDir);
		Quaternion quaternion = Quaternion.AngleAxis(theta, vector);
		xDir = quaternion * xDir;
		yDir = quaternion * yDir;
		Vector3 vector2 = xDir * num2 + yDir * num3;
		return origin + vector2;
	}

	// Token: 0x040005B5 RID: 1461
	[SerializeField]
	private Transform _blimp;

	// Token: 0x040005B6 RID: 1462
	[SerializeField]
	private Transform _light;

	// Token: 0x040005B7 RID: 1463
	[SerializeField]
	private Transform _target;

	// Token: 0x040005B8 RID: 1464
	[FormerlySerializedAs("_scale")]
	[SerializeField]
	private float _radius = 1f;

	// Token: 0x040005B9 RID: 1465
	[SerializeField]
	private float _offset;

	// Token: 0x040005BA RID: 1466
	[SerializeField]
	private float _theta;

	// Token: 0x040005BB RID: 1467
	public float speed = 16f;

	// Token: 0x040005BC RID: 1468
	[Space]
	private float _time;
}
