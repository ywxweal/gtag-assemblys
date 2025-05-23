using System;
using UnityEngine;

// Token: 0x020009EE RID: 2542
[Serializable]
public class SceneObject : IEquatable<SceneObject>
{
	// Token: 0x06003CE3 RID: 15587 RVA: 0x00121765 File Offset: 0x0011F965
	public Type GetObjectType()
	{
		if (string.IsNullOrWhiteSpace(this.typeString))
		{
			return null;
		}
		if (this.typeString.Contains("ProxyType"))
		{
			return ProxyType.Parse(this.typeString);
		}
		return Type.GetType(this.typeString);
	}

	// Token: 0x06003CE4 RID: 15588 RVA: 0x0012179F File Offset: 0x0011F99F
	public SceneObject(int classID, ulong fileID)
	{
		this.classID = classID;
		this.fileID = fileID;
		this.typeString = UnityYaml.ClassIDToType[classID].AssemblyQualifiedName;
	}

	// Token: 0x06003CE5 RID: 15589 RVA: 0x001217CB File Offset: 0x0011F9CB
	public bool Equals(SceneObject other)
	{
		return this.fileID == other.fileID && this.classID == other.classID;
	}

	// Token: 0x06003CE6 RID: 15590 RVA: 0x001217EC File Offset: 0x0011F9EC
	public override bool Equals(object obj)
	{
		SceneObject sceneObject = obj as SceneObject;
		return sceneObject != null && this.Equals(sceneObject);
	}

	// Token: 0x06003CE7 RID: 15591 RVA: 0x0012180C File Offset: 0x0011FA0C
	public override int GetHashCode()
	{
		int num = this.classID;
		int num2 = StaticHash.Compute((long)this.fileID);
		return StaticHash.Compute(num, num2);
	}

	// Token: 0x06003CE8 RID: 15592 RVA: 0x00121831 File Offset: 0x0011FA31
	public static bool operator ==(SceneObject x, SceneObject y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003CE9 RID: 15593 RVA: 0x0012183A File Offset: 0x0011FA3A
	public static bool operator !=(SceneObject x, SceneObject y)
	{
		return !x.Equals(y);
	}

	// Token: 0x0400408F RID: 16527
	public int classID;

	// Token: 0x04004090 RID: 16528
	public ulong fileID;

	// Token: 0x04004091 RID: 16529
	[SerializeField]
	public string typeString;

	// Token: 0x04004092 RID: 16530
	public string json;
}
