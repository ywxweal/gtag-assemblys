using System;
using UnityEngine;

namespace GorillaTag.Rendering.Shaders
{
	// Token: 0x02000DA5 RID: 3493
	public class ShaderConfigData
	{
		// Token: 0x060056AE RID: 22190 RVA: 0x001A65B0 File Offset: 0x001A47B0
		public static ShaderConfigData.MatPropInt[] convertInts(string[] names, int[] vals)
		{
			ShaderConfigData.MatPropInt[] array = new ShaderConfigData.MatPropInt[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropInt
				{
					intName = names[i],
					intVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x060056AF RID: 22191 RVA: 0x001A65FC File Offset: 0x001A47FC
		public static ShaderConfigData.MatPropFloat[] convertFloats(string[] names, float[] vals)
		{
			ShaderConfigData.MatPropFloat[] array = new ShaderConfigData.MatPropFloat[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropFloat
				{
					floatName = names[i],
					floatVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x060056B0 RID: 22192 RVA: 0x001A6648 File Offset: 0x001A4848
		public static ShaderConfigData.MatPropMatrix[] convertMatrices(string[] names, Matrix4x4[] vals)
		{
			ShaderConfigData.MatPropMatrix[] array = new ShaderConfigData.MatPropMatrix[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropMatrix
				{
					matrixName = names[i],
					matrixVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x060056B1 RID: 22193 RVA: 0x001A6698 File Offset: 0x001A4898
		public static ShaderConfigData.MatPropVector[] convertVectors(string[] names, Vector4[] vals)
		{
			ShaderConfigData.MatPropVector[] array = new ShaderConfigData.MatPropVector[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropVector
				{
					vectorName = names[i],
					vectorVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x060056B2 RID: 22194 RVA: 0x001A66E8 File Offset: 0x001A48E8
		public static ShaderConfigData.MatPropTexture[] convertTextures(string[] names, Texture[] vals)
		{
			ShaderConfigData.MatPropTexture[] array = new ShaderConfigData.MatPropTexture[names.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ShaderConfigData.MatPropTexture
				{
					textureName = names[i],
					textureVal = vals[i]
				};
			}
			return array;
		}

		// Token: 0x060056B3 RID: 22195 RVA: 0x001A6734 File Offset: 0x001A4934
		public static string GetShaderPropertiesStringFromMaterial(Material mat, bool excludeMainTexData)
		{
			string text = "";
			string[] array = mat.GetPropertyNames(MaterialPropertyType.Int);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = mat.GetInteger(array[i]);
				text += array2[i].ToString();
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Float);
			float[] array3 = new float[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				if (excludeMainTexData || !array[j].Contains("_BaseMap"))
				{
					array3[j] = mat.GetFloat(array[j]);
					text += array3[j].ToString();
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Matrix);
			Matrix4x4[] array4 = new Matrix4x4[array.Length];
			for (int k = 0; k < array.Length; k++)
			{
				array4[k] = mat.GetMatrix(array[k]);
				text += array4[k].ToString();
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Vector);
			Vector4[] array5 = new Vector4[array.Length];
			for (int l = 0; l < array.Length; l++)
			{
				if (excludeMainTexData || !array[l].Contains("_BaseMap"))
				{
					array5[l] = mat.GetVector(array[l]);
					text += array5[l].ToString();
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Texture);
			Texture[] array6 = new Texture[array.Length];
			for (int m = 0; m < array.Length; m++)
			{
				if (!array[m].Contains("_BaseMap"))
				{
					array6[m] = mat.GetTexture(array[m]);
					if (array6[m] != null)
					{
						text += array6[m].ToString();
					}
				}
			}
			return text;
		}

		// Token: 0x060056B4 RID: 22196 RVA: 0x001A6900 File Offset: 0x001A4B00
		public static ShaderConfigData.ShaderConfig GetConfigDataFromMaterial(Material mat, bool includeMainTexData)
		{
			string[] array = mat.GetPropertyNames(MaterialPropertyType.Int);
			string[] array2 = array;
			int[] array3 = new int[array2.Length];
			bool flag = mat.IsKeywordEnabled("_WATER_EFFECT");
			bool flag2 = mat.IsKeywordEnabled("_MAINTEX_ROTATE");
			bool flag3 = mat.IsKeywordEnabled("_UV_WAVE_WARP");
			bool flag4 = mat.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
			bool flag5 = flag3 || flag4;
			bool flag6 = mat.IsKeywordEnabled("_LIQUID_CONTAINER");
			bool flag7 = mat.IsKeywordEnabled("_LIQUID_VOLUME") && !flag6;
			bool flag8 = mat.IsKeywordEnabled("_CRYSTAL_EFFECT");
			bool flag9 = mat.IsKeywordEnabled("_EMISSION") || flag8;
			bool flag10 = mat.IsKeywordEnabled("_REFLECTIONS");
			mat.IsKeywordEnabled("_REFLECTIONS_MATCAP");
			bool flag11 = mat.IsKeywordEnabled("_UV_SHIFT");
			for (int i = 0; i < array2.Length; i++)
			{
				array3[i] = mat.GetInteger(array[i]);
				if (!flag11 && (array[i] == "_UvShiftSteps" || array[i] == "_UvShiftOffset"))
				{
					array3[i] = 0;
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Float);
			string[] array4 = array;
			float[] array5 = new float[array4.Length];
			for (int j = 0; j < array.Length; j++)
			{
				if (includeMainTexData || !array[j].Contains("_BaseMap"))
				{
					array5[j] = mat.GetFloat(array[j]);
				}
				if ((!flag && array[j] == "_HeightBasedWaterEffect") || (!flag2 && array[j] == "_RotateSpeed") || (!flag5 && (array[j] == "_WaveAmplitude" || array[j] == "_WaveFrequency" || array[j] == "_WaveScale")) || (!flag7 && (array[j] == "_LiquidFill" || array[j] == "_LiquidSwayX" || array[j] == "_LiquidSwayY")) || (!flag8 && array[j] == "_CrystalPower") || (!flag9 && array[j].StartsWith("_Emission")) || (!flag10 && (array[j] == "_ReflectOpacity" || array[j] == "_ReflectExposure" || array[j] == "_ReflectRotate")) || (!flag11 && array[j] == "_UvShiftRate"))
				{
					array5[j] = 0f;
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Matrix);
			string[] array6 = array;
			Matrix4x4[] array7 = new Matrix4x4[array6.Length];
			for (int k = 0; k < array.Length; k++)
			{
				array7[k] = mat.GetMatrix(array[k]);
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Vector);
			string[] array8 = array;
			Vector4[] array9 = new Vector4[array8.Length];
			for (int l = 0; l < array.Length; l++)
			{
				if (includeMainTexData || !array[l].Contains("_BaseMap"))
				{
					array9[l] = mat.GetVector(array[l]);
				}
				if ((!flag7 && (array[l] == "_LiquidFillNormal" || array[l] == "_LiquidSurfaceColor")) || (!flag6 && (array[l] == "_LiquidPlanePosition" || array[l] == "_LiquidPlaneNormal")) || (!flag8 && array[l] == "_CrystalRimColor") || (!flag9 && array[l].StartsWith("_Emission")) || (!flag10 && (array[l] == "_ReflectTint" || array[l] == "_ReflectOffset" || array[l] == "_ReflectScale")))
				{
					array9[l] = Vector4.zero;
				}
			}
			array = mat.GetPropertyNames(MaterialPropertyType.Texture);
			string[] array10 = array;
			Texture[] array11 = new Texture[array10.Length];
			for (int m = 0; m < array.Length; m++)
			{
				if (!array[m].Contains("_BaseMap"))
				{
					array11[m] = mat.GetTexture(array[m]);
				}
			}
			return new ShaderConfigData.ShaderConfig(mat.shader.name, mat, array2, array3, array4, array5, array6, array7, array8, array9, array10, array11);
		}

		// Token: 0x02000DA6 RID: 3494
		[Serializable]
		public struct ShaderConfig
		{
			// Token: 0x060056B6 RID: 22198 RVA: 0x001A6D28 File Offset: 0x001A4F28
			public ShaderConfig(string shadName, Material fMat, string[] intNames, int[] intVals, string[] floatNames, float[] floatVals, string[] matrixNames, Matrix4x4[] matrixVals, string[] vectorNames, Vector4[] vectorVals, string[] textureNames, Texture[] textureVals)
			{
				this.shaderName = shadName;
				this.firstMat = fMat;
				this.ints = ShaderConfigData.convertInts(intNames, intVals);
				this.floats = ShaderConfigData.convertFloats(floatNames, floatVals);
				this.matrices = ShaderConfigData.convertMatrices(matrixNames, matrixVals);
				this.vectors = ShaderConfigData.convertVectors(vectorNames, vectorVals);
				this.textures = ShaderConfigData.convertTextures(textureNames, textureVals);
			}

			// Token: 0x04005A90 RID: 23184
			public string shaderName;

			// Token: 0x04005A91 RID: 23185
			public Material firstMat;

			// Token: 0x04005A92 RID: 23186
			public ShaderConfigData.MatPropInt[] ints;

			// Token: 0x04005A93 RID: 23187
			public ShaderConfigData.MatPropFloat[] floats;

			// Token: 0x04005A94 RID: 23188
			public ShaderConfigData.MatPropMatrix[] matrices;

			// Token: 0x04005A95 RID: 23189
			public ShaderConfigData.MatPropVector[] vectors;

			// Token: 0x04005A96 RID: 23190
			public ShaderConfigData.MatPropTexture[] textures;
		}

		// Token: 0x02000DA7 RID: 3495
		[Serializable]
		public struct MatPropInt
		{
			// Token: 0x04005A97 RID: 23191
			public string intName;

			// Token: 0x04005A98 RID: 23192
			public int intVal;
		}

		// Token: 0x02000DA8 RID: 3496
		[Serializable]
		public struct MatPropFloat
		{
			// Token: 0x04005A99 RID: 23193
			public string floatName;

			// Token: 0x04005A9A RID: 23194
			public float floatVal;
		}

		// Token: 0x02000DA9 RID: 3497
		[Serializable]
		public struct MatPropMatrix
		{
			// Token: 0x04005A9B RID: 23195
			public string matrixName;

			// Token: 0x04005A9C RID: 23196
			public Matrix4x4 matrixVal;
		}

		// Token: 0x02000DAA RID: 3498
		[Serializable]
		public struct MatPropVector
		{
			// Token: 0x04005A9D RID: 23197
			public string vectorName;

			// Token: 0x04005A9E RID: 23198
			public Vector4 vectorVal;
		}

		// Token: 0x02000DAB RID: 3499
		[Serializable]
		public struct MatPropTexture
		{
			// Token: 0x04005A9F RID: 23199
			public string textureName;

			// Token: 0x04005AA0 RID: 23200
			public Texture textureVal;
		}

		// Token: 0x02000DAC RID: 3500
		[Serializable]
		public struct RenderersForShaderWithSameProperties
		{
			// Token: 0x04005AA1 RID: 23201
			public MeshRenderer[] renderers;
		}
	}
}
