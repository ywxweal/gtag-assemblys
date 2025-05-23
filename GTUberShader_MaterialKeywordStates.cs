using System;
using UnityEngine;

// Token: 0x02000A27 RID: 2599
public struct GTUberShader_MaterialKeywordStates
{
	// Token: 0x06003DE5 RID: 15845 RVA: 0x00125624 File Offset: 0x00123824
	public GTUberShader_MaterialKeywordStates(Material mat)
	{
		this.material = mat;
		this.STEREO_INSTANCING_ON = mat.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.UNITY_SINGLE_PASS_STEREO = mat.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.STEREO_MULTIVIEW_ON = mat.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.STEREO_CUBEMAP_RENDER_ON = mat.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._ZONE_LIQUID_SHAPE__CYLINDER = mat.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = mat.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._USE_TEXTURE = mat.IsKeywordEnabled("_USE_TEXTURE");
		this.USE_TEXTURE__AS_MASK = mat.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
		this._UV_SOURCE__UV0 = mat.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = mat.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._USE_VERTEX_COLOR = mat.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = mat.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._ALPHA_DETAIL_MAP = mat.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._HALF_LAMBERT_TERM = mat.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._WATER_EFFECT = mat.IsKeywordEnabled("_WATER_EFFECT");
		this._HEIGHT_BASED_WATER_EFFECT = mat.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._WATER_CAUSTICS = mat.IsKeywordEnabled("_WATER_CAUSTICS");
		this._ALPHATEST_ON = mat.IsKeywordEnabled("_ALPHATEST_ON");
		this._MAINTEX_ROTATE = mat.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._UV_WAVE_WARP = mat.IsKeywordEnabled("_UV_WAVE_WARP");
		this._LIQUID_VOLUME = mat.IsKeywordEnabled("_LIQUID_VOLUME");
		this._LIQUID_CONTAINER = mat.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._GT_RIM_LIGHT = mat.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = mat.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = mat.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._SPECULAR_HIGHLIGHT = mat.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._EMISSION = mat.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = mat.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._USE_DEFORM_MAP = mat.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_DAY_NIGHT_LIGHTMAP = mat.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_TEX_ARRAY_ATLAS = mat.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._CRYSTAL_EFFECT = mat.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._EYECOMP = mat.IsKeywordEnabled("_EYECOMP");
		this._MOUTHCOMP = mat.IsKeywordEnabled("_MOUTHCOMP");
		this._ALPHA_BLUE_LIVE_ON = mat.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._GRID_EFFECT = mat.IsKeywordEnabled("_GRID_EFFECT");
		this._REFLECTIONS = mat.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_BOX_PROJECT = mat.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_ALBEDO_TINT = mat.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_USE_NORMAL_TEX = mat.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._VERTEX_ROTATE = mat.IsKeywordEnabled("_VERTEX_ROTATE");
		this._VERTEX_ANIM_FLAP = mat.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._GRADIENT_MAP_ON = mat.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._PARALLAX = mat.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = mat.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = mat.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._MASK_MAP_ON = mat.IsKeywordEnabled("_MASK_MAP_ON");
		this._FX_LAVA_LAMP = mat.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._INNER_GLOW = mat.IsKeywordEnabled("_INNER_GLOW");
		this._STEALTH_EFFECT = mat.IsKeywordEnabled("_STEALTH_EFFECT");
		this._UV_SHIFT = mat.IsKeywordEnabled("_UV_SHIFT");
		this._TEXEL_SNAP_UVS = mat.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = mat.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._GT_EDITOR_TIME = mat.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._DEBUG_PAWN_DATA = mat.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._COLOR_GRADE_PROTANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_DEUTERANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_TRITANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._COLOR_GRADE_ACHROMATOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this.LIGHTMAP_ON = mat.IsKeywordEnabled("LIGHTMAP_ON");
		this.DIRLIGHTMAP_COMBINED = mat.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = mat.IsKeywordEnabled("INSTANCING_ON");
	}

	// Token: 0x06003DE6 RID: 15846 RVA: 0x00125B24 File Offset: 0x00123D24
	public void Refresh()
	{
		Material material = this.material;
		this.STEREO_INSTANCING_ON = material.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.UNITY_SINGLE_PASS_STEREO = material.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.STEREO_MULTIVIEW_ON = material.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.STEREO_CUBEMAP_RENDER_ON = material.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._ZONE_LIQUID_SHAPE__CYLINDER = material.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = material.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._USE_TEXTURE = material.IsKeywordEnabled("_USE_TEXTURE");
		this.USE_TEXTURE__AS_MASK = material.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
		this._UV_SOURCE__UV0 = material.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = material.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._USE_VERTEX_COLOR = material.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = material.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._ALPHA_DETAIL_MAP = material.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._HALF_LAMBERT_TERM = material.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._WATER_EFFECT = material.IsKeywordEnabled("_WATER_EFFECT");
		this._HEIGHT_BASED_WATER_EFFECT = material.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._WATER_CAUSTICS = material.IsKeywordEnabled("_WATER_CAUSTICS");
		this._ALPHATEST_ON = material.IsKeywordEnabled("_ALPHATEST_ON");
		this._MAINTEX_ROTATE = material.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._UV_WAVE_WARP = material.IsKeywordEnabled("_UV_WAVE_WARP");
		this._LIQUID_VOLUME = material.IsKeywordEnabled("_LIQUID_VOLUME");
		this._LIQUID_CONTAINER = material.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._GT_RIM_LIGHT = material.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = material.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = material.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._SPECULAR_HIGHLIGHT = material.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._EMISSION = material.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = material.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._USE_DEFORM_MAP = material.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_DAY_NIGHT_LIGHTMAP = material.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_TEX_ARRAY_ATLAS = material.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._CRYSTAL_EFFECT = material.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._EYECOMP = material.IsKeywordEnabled("_EYECOMP");
		this._MOUTHCOMP = material.IsKeywordEnabled("_MOUTHCOMP");
		this._ALPHA_BLUE_LIVE_ON = material.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._GRID_EFFECT = material.IsKeywordEnabled("_GRID_EFFECT");
		this._REFLECTIONS = material.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_BOX_PROJECT = material.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = material.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = material.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_ALBEDO_TINT = material.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_USE_NORMAL_TEX = material.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._VERTEX_ROTATE = material.IsKeywordEnabled("_VERTEX_ROTATE");
		this._VERTEX_ANIM_FLAP = material.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._GRADIENT_MAP_ON = material.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._PARALLAX = material.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = material.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = material.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._MASK_MAP_ON = material.IsKeywordEnabled("_MASK_MAP_ON");
		this._FX_LAVA_LAMP = material.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._INNER_GLOW = material.IsKeywordEnabled("_INNER_GLOW");
		this._STEALTH_EFFECT = material.IsKeywordEnabled("_STEALTH_EFFECT");
		this._UV_SHIFT = material.IsKeywordEnabled("_UV_SHIFT");
		this._TEXEL_SNAP_UVS = material.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = material.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._GT_EDITOR_TIME = material.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._DEBUG_PAWN_DATA = material.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._COLOR_GRADE_PROTANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_DEUTERANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_TRITANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._COLOR_GRADE_ACHROMATOMALY = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this.LIGHTMAP_ON = material.IsKeywordEnabled("LIGHTMAP_ON");
		this.DIRLIGHTMAP_COMBINED = material.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = material.IsKeywordEnabled("INSTANCING_ON");
	}

	// Token: 0x04004205 RID: 16901
	public Material material;

	// Token: 0x04004206 RID: 16902
	public bool STEREO_INSTANCING_ON;

	// Token: 0x04004207 RID: 16903
	public bool UNITY_SINGLE_PASS_STEREO;

	// Token: 0x04004208 RID: 16904
	public bool STEREO_MULTIVIEW_ON;

	// Token: 0x04004209 RID: 16905
	public bool STEREO_CUBEMAP_RENDER_ON;

	// Token: 0x0400420A RID: 16906
	public bool _GLOBAL_ZONE_LIQUID_TYPE__WATER;

	// Token: 0x0400420B RID: 16907
	public bool _GLOBAL_ZONE_LIQUID_TYPE__LAVA;

	// Token: 0x0400420C RID: 16908
	public bool _ZONE_LIQUID_SHAPE__CYLINDER;

	// Token: 0x0400420D RID: 16909
	public bool _ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;

	// Token: 0x0400420E RID: 16910
	public bool _USE_TEXTURE;

	// Token: 0x0400420F RID: 16911
	public bool USE_TEXTURE__AS_MASK;

	// Token: 0x04004210 RID: 16912
	public bool _UV_SOURCE__UV0;

	// Token: 0x04004211 RID: 16913
	public bool _UV_SOURCE__WORLD_PLANAR_Y;

	// Token: 0x04004212 RID: 16914
	public bool _USE_VERTEX_COLOR;

	// Token: 0x04004213 RID: 16915
	public bool _USE_WEATHER_MAP;

	// Token: 0x04004214 RID: 16916
	public bool _ALPHA_DETAIL_MAP;

	// Token: 0x04004215 RID: 16917
	public bool _HALF_LAMBERT_TERM;

	// Token: 0x04004216 RID: 16918
	public bool _WATER_EFFECT;

	// Token: 0x04004217 RID: 16919
	public bool _HEIGHT_BASED_WATER_EFFECT;

	// Token: 0x04004218 RID: 16920
	public bool _WATER_CAUSTICS;

	// Token: 0x04004219 RID: 16921
	public bool _ALPHATEST_ON;

	// Token: 0x0400421A RID: 16922
	public bool _MAINTEX_ROTATE;

	// Token: 0x0400421B RID: 16923
	public bool _UV_WAVE_WARP;

	// Token: 0x0400421C RID: 16924
	public bool _LIQUID_VOLUME;

	// Token: 0x0400421D RID: 16925
	public bool _LIQUID_CONTAINER;

	// Token: 0x0400421E RID: 16926
	public bool _GT_RIM_LIGHT;

	// Token: 0x0400421F RID: 16927
	public bool _GT_RIM_LIGHT_FLAT;

	// Token: 0x04004220 RID: 16928
	public bool _GT_RIM_LIGHT_USE_ALPHA;

	// Token: 0x04004221 RID: 16929
	public bool _SPECULAR_HIGHLIGHT;

	// Token: 0x04004222 RID: 16930
	public bool _EMISSION;

	// Token: 0x04004223 RID: 16931
	public bool _EMISSION_USE_UV_WAVE_WARP;

	// Token: 0x04004224 RID: 16932
	public bool _USE_DEFORM_MAP;

	// Token: 0x04004225 RID: 16933
	public bool _USE_DAY_NIGHT_LIGHTMAP;

	// Token: 0x04004226 RID: 16934
	public bool _USE_TEX_ARRAY_ATLAS;

	// Token: 0x04004227 RID: 16935
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY;

	// Token: 0x04004228 RID: 16936
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;

	// Token: 0x04004229 RID: 16937
	public bool _CRYSTAL_EFFECT;

	// Token: 0x0400422A RID: 16938
	public bool _EYECOMP;

	// Token: 0x0400422B RID: 16939
	public bool _MOUTHCOMP;

	// Token: 0x0400422C RID: 16940
	public bool _ALPHA_BLUE_LIVE_ON;

	// Token: 0x0400422D RID: 16941
	public bool _GRID_EFFECT;

	// Token: 0x0400422E RID: 16942
	public bool _REFLECTIONS;

	// Token: 0x0400422F RID: 16943
	public bool _REFLECTIONS_BOX_PROJECT;

	// Token: 0x04004230 RID: 16944
	public bool _REFLECTIONS_MATCAP;

	// Token: 0x04004231 RID: 16945
	public bool _REFLECTIONS_MATCAP_PERSP_AWARE;

	// Token: 0x04004232 RID: 16946
	public bool _REFLECTIONS_ALBEDO_TINT;

	// Token: 0x04004233 RID: 16947
	public bool _REFLECTIONS_USE_NORMAL_TEX;

	// Token: 0x04004234 RID: 16948
	public bool _VERTEX_ROTATE;

	// Token: 0x04004235 RID: 16949
	public bool _VERTEX_ANIM_FLAP;

	// Token: 0x04004236 RID: 16950
	public bool _VERTEX_ANIM_WAVE;

	// Token: 0x04004237 RID: 16951
	public bool _VERTEX_ANIM_WAVE_DEBUG;

	// Token: 0x04004238 RID: 16952
	public bool _GRADIENT_MAP_ON;

	// Token: 0x04004239 RID: 16953
	public bool _PARALLAX;

	// Token: 0x0400423A RID: 16954
	public bool _PARALLAX_AA;

	// Token: 0x0400423B RID: 16955
	public bool _PARALLAX_PLANAR;

	// Token: 0x0400423C RID: 16956
	public bool _MASK_MAP_ON;

	// Token: 0x0400423D RID: 16957
	public bool _FX_LAVA_LAMP;

	// Token: 0x0400423E RID: 16958
	public bool _INNER_GLOW;

	// Token: 0x0400423F RID: 16959
	public bool _STEALTH_EFFECT;

	// Token: 0x04004240 RID: 16960
	public bool _UV_SHIFT;

	// Token: 0x04004241 RID: 16961
	public bool _TEXEL_SNAP_UVS;

	// Token: 0x04004242 RID: 16962
	public bool _UNITY_EDIT_MODE;

	// Token: 0x04004243 RID: 16963
	public bool _GT_EDITOR_TIME;

	// Token: 0x04004244 RID: 16964
	public bool _DEBUG_PAWN_DATA;

	// Token: 0x04004245 RID: 16965
	public bool _COLOR_GRADE_PROTANOMALY;

	// Token: 0x04004246 RID: 16966
	public bool _COLOR_GRADE_PROTANOPIA;

	// Token: 0x04004247 RID: 16967
	public bool _COLOR_GRADE_DEUTERANOMALY;

	// Token: 0x04004248 RID: 16968
	public bool _COLOR_GRADE_DEUTERANOPIA;

	// Token: 0x04004249 RID: 16969
	public bool _COLOR_GRADE_TRITANOMALY;

	// Token: 0x0400424A RID: 16970
	public bool _COLOR_GRADE_TRITANOPIA;

	// Token: 0x0400424B RID: 16971
	public bool _COLOR_GRADE_ACHROMATOMALY;

	// Token: 0x0400424C RID: 16972
	public bool _COLOR_GRADE_ACHROMATOPSIA;

	// Token: 0x0400424D RID: 16973
	public bool LIGHTMAP_ON;

	// Token: 0x0400424E RID: 16974
	public bool DIRLIGHTMAP_COMBINED;

	// Token: 0x0400424F RID: 16975
	public bool INSTANCING_ON;
}
