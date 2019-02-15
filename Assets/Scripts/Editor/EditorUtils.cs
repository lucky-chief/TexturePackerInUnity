using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtils {
	/// <summary>
	/// 空贴图
	/// </summary>
	public static Texture2D BlankTexture{ get { return EditorGUIUtility.whiteTexture; } }

	//缓存棋盘图
	private static Texture2D contrastTexture=null;

	/// <summary>
	/// 绘制棋盘背景
	/// </summary>
	public static void DrawContrastBackground(Rect rect)
	{
		if (contrastTexture == null) {
			Texture2D tex = new Texture2D (16, 16);
			tex.name = "[Generated] Contrast Texture";
			tex.hideFlags = HideFlags.DontSave;
			Color c1 = new Color (0.1f, 0.1f, 0.1f, 0.5f);
			Color c2 = new Color (0.2f, 0.2f, 0.2f, 0.5f);
			for (int y = 0; y < 8; ++y)
				for (int x = 0; x < 8; ++x)
					tex.SetPixel (x, y, c1);
			for (int y = 8; y < 16; ++y)
				for (int x = 0; x < 8; ++x)
					tex.SetPixel (x, y, c2);
			for (int y = 0; y < 8; ++y)
				for (int x = 8; x < 16; ++x)
					tex.SetPixel (x, y, c2);
			for (int y = 8; y < 16; ++y)
				for (int x = 8; x < 16; ++x)
					tex.SetPixel (x, y, c1);				
			tex.Apply ();
			tex.filterMode = FilterMode.Point;
			contrastTexture = tex;
		}
		DrawTiledTexture (rect, contrastTexture);
	}

	/// <summary>
	/// 用指定贴图平铺指定矩形
	/// </summary>
	public static void DrawTiledTexture (Rect rect, Texture texture)
	{
		using (GUI.GroupScope groupScope = new GUI.GroupScope (rect)) {
			int width = Mathf.RoundToInt (rect.width);
			int height = Mathf.RoundToInt (rect.height);
			for (int y = 0; y < height; y += texture.height) {
				for (int x = 0; x < width; x += texture.width) {
					GUI.DrawTexture (new Rect (x, y, texture.width, texture.height), texture);
				}
			}
		}
	}

	/// <summary>
	/// 绘制分隔符
	/// </summary>
	public static void DrawSeparator ()
	{
		GUILayout.Space(12.0f);
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D texture = BlankTexture;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.25f);
			GUI.DrawTexture(new Rect(0.0f, rect.yMin + 6.0f, Screen.width, 4.0f), texture);
			GUI.DrawTexture(new Rect(0.0f, rect.yMin + 6.0f, Screen.width, 1.0f), texture);
			GUI.DrawTexture(new Rect(0.0f, rect.yMin + 9.0f, Screen.width, 1.0f), texture);
			GUI.color = Color.white;
		}
	}
    public static void DrawOutline(Rect rect, Color color)
    {
        DrawOutline(rect, color, 1.0f);
    }
    /// <summary>
    /// 绘制轮廓
    /// </summary>
    public static void DrawOutline (Rect rect, Color color, float outlineSize )
	{
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D texture = EditorUtils.BlankTexture;
			GUI.color = color;
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, outlineSize, rect.height), texture);
			GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, outlineSize, rect.height), texture);
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, outlineSize), texture);
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, outlineSize), texture);
			GUI.color = Color.white;
		}
	}

	/// <summary>
	/// 根据像素坐标绘制一张贴图的一部分
	/// </summary>
	public static void DrawTextureWithPixelCoords(Rect rect, Texture texture, Rect rectByPixel)
	{
		Rect rectByUV = rectByPixel;
		if (texture.width != 0f && texture.height != 0f) {
			rectByUV.xMin = rectByPixel.xMin / texture.width;
			rectByUV.xMax = rectByPixel.xMax / texture.width;
			rectByUV.yMin = rectByPixel.yMin / texture.height;
			rectByUV.yMax = rectByPixel.yMax / texture.height;
		}
		GUI.DrawTextureWithTexCoords (rect, texture, rectByUV);
	}

	/// <summary>
	/// 绘制搜索框
	/// </summary>
	public static string DrawSearchBox(string searchName)
	{
		using (EditorGUILayout.HorizontalScope horizontalScope = new EditorGUILayout.HorizontalScope ()) {
			GUILayout.Space (50.0f);
			searchName = EditorGUILayout.TextField ("", searchName, "SearchTextField");
			if (GUILayout.Button ("", "SearchCancelButton", GUILayout.Width (18f))) {
				searchName = "";
				GUIUtility.keyboardControl = 0;
			}
			GUILayout.Space (50.0f);
		}
		return searchName;
	}


//	public static string DrawList (string field, string[] list, string selection, params GUILayoutOption[] options)
//	{
//		if (list != null && list.Length > 0)
//		{
//			int index = 0;
//			if (string.IsNullOrEmpty(selection)) selection = list[0];
//
//			// We need to find the sprite in order to have it selected
//			if (!string.IsNullOrEmpty(selection))
//			{
//				for (int i = 0; i < list.Length; ++i)
//				{
//					if (selection.Equals(list[i], System.StringComparison.OrdinalIgnoreCase))
//					{
//						index = i;
//						break;
//					}
//				}
//			}
//
//			// Draw the sprite selection popup
//			index = string.IsNullOrEmpty(field) ? EditorGUILayout.Popup(index, list, options) : EditorGUILayout.Popup(field, index, list, options);
//			return list[index];
//		}
//		return null;
//	}

	public static bool DrawPrefixButton (string text)
	{
		return GUILayout.Button(text, "DropDown", GUILayout.Width(76f));
	}

	public static bool DrawPrefixButton (string text, params GUILayoutOption[] options)
	{
		return GUILayout.Button(text, "DropDown", options);
	}

	public static int DrawPrefixList (int index, string[] list, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(index, list, "DropDown", options);
	}

	public static int DrawPrefixList (string text, int index, string[] list, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(text, index, list, "DropDown", options);
	}
}