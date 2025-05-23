using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000CA1 RID: 3233
	[AddComponentMenu("")]
	public class MTAssetsMathematics : MonoBehaviour
	{
		// Token: 0x0600501F RID: 20511 RVA: 0x0017D998 File Offset: 0x0017BB98
		public static List<T> RandomizeThisList<T>(List<T> list)
		{
			int count = list.Count;
			int num = count - 1;
			for (int i = 0; i < num; i++)
			{
				int num2 = Random.Range(i, count);
				T t = list[i];
				list[i] = list[num2];
				list[num2] = t;
			}
			return list;
		}

		// Token: 0x06005020 RID: 20512 RVA: 0x0017D9E5 File Offset: 0x0017BBE5
		public static Vector3 GetHalfPositionBetweenTwoPoints(Vector3 pointA, Vector3 pointB)
		{
			return Vector3.Lerp(pointA, pointB, 0.5f);
		}
	}
}
