using System;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class AngryBeeAnimator : MonoBehaviour
{
	// Token: 0x0600064C RID: 1612 RVA: 0x00024028 File Offset: 0x00022228
	private void Awake()
	{
		this.bees = new GameObject[this.numBees];
		this.beeOrbits = new GameObject[this.numBees];
		this.beeOrbitalRadii = new float[this.numBees];
		this.beeOrbitalAxes = new Vector3[this.numBees];
		for (int i = 0; i < this.numBees; i++)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = base.transform;
			Vector2 vector = Random.insideUnitCircle * this.orbitMaxCenterDisplacement;
			gameObject.transform.localPosition = new Vector3(vector.x, Random.Range(-this.orbitMaxHeightDisplacement, this.orbitMaxHeightDisplacement), vector.y);
			gameObject.transform.localRotation = Quaternion.Euler(Random.Range(-this.orbitMaxTilt, this.orbitMaxTilt), (float)Random.Range(0, 360), 0f);
			this.beeOrbitalAxes[i] = gameObject.transform.up;
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.beePrefab, gameObject.transform);
			float num = Random.Range(this.orbitMinRadius, this.orbitMaxRadius);
			this.beeOrbitalRadii[i] = num;
			gameObject2.transform.localPosition = Vector3.forward * num;
			gameObject2.transform.localRotation = Quaternion.Euler(-90f, 90f, 0f);
			gameObject2.transform.localScale = Vector3.one * this.beeScale;
			this.bees[i] = gameObject2;
			this.beeOrbits[i] = gameObject;
		}
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x000241C4 File Offset: 0x000223C4
	private void Update()
	{
		float num = this.orbitSpeed * Time.deltaTime;
		for (int i = 0; i < this.numBees; i++)
		{
			this.beeOrbits[i].transform.Rotate(this.beeOrbitalAxes[i], num);
		}
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00024210 File Offset: 0x00022410
	public void SetEmergeFraction(float fraction)
	{
		for (int i = 0; i < this.numBees; i++)
		{
			this.bees[i].transform.localPosition = Vector3.forward * fraction * this.beeOrbitalRadii[i];
		}
	}

	// Token: 0x0400077E RID: 1918
	[SerializeField]
	private GameObject beePrefab;

	// Token: 0x0400077F RID: 1919
	[SerializeField]
	private int numBees;

	// Token: 0x04000780 RID: 1920
	[SerializeField]
	private float orbitMinRadius;

	// Token: 0x04000781 RID: 1921
	[SerializeField]
	private float orbitMaxRadius;

	// Token: 0x04000782 RID: 1922
	[SerializeField]
	private float orbitMaxHeightDisplacement;

	// Token: 0x04000783 RID: 1923
	[SerializeField]
	private float orbitMaxCenterDisplacement;

	// Token: 0x04000784 RID: 1924
	[SerializeField]
	private float orbitMaxTilt;

	// Token: 0x04000785 RID: 1925
	[SerializeField]
	private float orbitSpeed;

	// Token: 0x04000786 RID: 1926
	[SerializeField]
	private float beeScale;

	// Token: 0x04000787 RID: 1927
	private GameObject[] beeOrbits;

	// Token: 0x04000788 RID: 1928
	private GameObject[] bees;

	// Token: 0x04000789 RID: 1929
	private Vector3[] beeOrbitalAxes;

	// Token: 0x0400078A RID: 1930
	private float[] beeOrbitalRadii;
}
