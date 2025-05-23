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
		// (get) Token: 0x06005380 RID: 21376 RVA: 0x0019584F File Offset: 0x00193A4F
		// (set) Token: 0x06005381 RID: 21377 RVA: 0x00195857 File Offset: 0x00193A57
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

		// Token: 0x06005382 RID: 21378 RVA: 0x00195857 File Offset: 0x00193A57
		public GTDirectAssetRef(T theObj)
		{
			this._obj = theObj;
			this.edAssetPath = null;
		}

		// Token: 0x06005383 RID: 21379 RVA: 0x00195867 File Offset: 0x00193A67
		public static implicit operator T(GTDirectAssetRef<T> refObject)
		{
			return refObject.obj;
		}

		// Token: 0x06005384 RID: 21380 RVA: 0x00195870 File Offset: 0x00193A70
		public static implicit operator GTDirectAssetRef<T>(T other)
		{
			return new GTDirectAssetRef<T>
			{
				obj = other
			};
		}

		// Token: 0x06005385 RID: 21381 RVA: 0x0019588E File Offset: 0x00193A8E
		public bool Equals(T other)
		{
			return this.obj == other;
		}

		// Token: 0x06005386 RID: 21382 RVA: 0x001958A8 File Offset: 0x00193AA8
		public override bool Equals(object other)
		{
			T t = other as T;
			return t != null && this.Equals(t);
		}

		// Token: 0x06005387 RID: 21383 RVA: 0x001958D2 File Offset: 0x00193AD2
		public override int GetHashCode()
		{
			if (!(this.obj != null))
			{
				return 0;
			}
			return this.obj.GetHashCode();
		}

		// Token: 0x06005388 RID: 21384 RVA: 0x001958F9 File Offset: 0x00193AF9
		public static bool operator ==(GTDirectAssetRef<T> left, T right)
		{
			return left.Equals(right);
		}

		// Token: 0x06005389 RID: 21385 RVA: 0x00195903 File Offset: 0x00193B03
		public static bool operator !=(GTDirectAssetRef<T> left, T right)
		{
			return !(left == right);
		}

		// Token: 0x04005695 RID: 22165
		[SerializeField]
		[HideInInspector]
		internal T _obj;

		// Token: 0x04005696 RID: 22166
		[FormerlySerializedAs("assetPath")]
		public string edAssetPath;
	}
}
