using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000CFF RID: 3327
	[Serializable]
	public struct GTDirectAssetRef<T> : IEquatable<T> where T : Object
	{
		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x0600537F RID: 21375 RVA: 0x00195777 File Offset: 0x00193977
		// (set) Token: 0x06005380 RID: 21376 RVA: 0x0019577F File Offset: 0x0019397F
		public T obj
		{
			get
			{
				return this._obj;
			}
			set
			{
				this._obj = value;
				this.edAssetPath = null;
			}
		}

		// Token: 0x06005381 RID: 21377 RVA: 0x0019577F File Offset: 0x0019397F
		public GTDirectAssetRef(T theObj)
		{
			this._obj = theObj;
			this.edAssetPath = null;
		}

		// Token: 0x06005382 RID: 21378 RVA: 0x0019578F File Offset: 0x0019398F
		public static implicit operator T(GTDirectAssetRef<T> refObject)
		{
			return refObject.obj;
		}

		// Token: 0x06005383 RID: 21379 RVA: 0x00195798 File Offset: 0x00193998
		public static implicit operator GTDirectAssetRef<T>(T other)
		{
			return new GTDirectAssetRef<T>
			{
				obj = other
			};
		}

		// Token: 0x06005384 RID: 21380 RVA: 0x001957B6 File Offset: 0x001939B6
		public bool Equals(T other)
		{
			return this.obj == other;
		}

		// Token: 0x06005385 RID: 21381 RVA: 0x001957D0 File Offset: 0x001939D0
		public override bool Equals(object other)
		{
			T t = other as T;
			return t != null && this.Equals(t);
		}

		// Token: 0x06005386 RID: 21382 RVA: 0x001957FA File Offset: 0x001939FA
		public override int GetHashCode()
		{
			if (!(this.obj != null))
			{
				return 0;
			}
			return this.obj.GetHashCode();
		}

		// Token: 0x06005387 RID: 21383 RVA: 0x00195821 File Offset: 0x00193A21
		public static bool operator ==(GTDirectAssetRef<T> left, T right)
		{
			return left.Equals(right);
		}

		// Token: 0x06005388 RID: 21384 RVA: 0x0019582B File Offset: 0x00193A2B
		public static bool operator !=(GTDirectAssetRef<T> left, T right)
		{
			return !(left == right);
		}

		// Token: 0x04005694 RID: 22164
		[SerializeField]
		[HideInInspector]
		internal T _obj;

		// Token: 0x04005695 RID: 22165
		[FormerlySerializedAs("assetPath")]
		public string edAssetPath;
	}
}
