using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System;

public class AtlasSelector : ScriptableWizard
{
	private static AtlasSelector instance = null;

	//双击时间阈值
	private const float DoubleClickThreshold = 0.3f;
	//外观参数
	private const float CellSize = 160.0f;
	private const float Padding = 10.0f;
	private const float NameHeight = 40.0f;
	private const float PaddedCellWidth = CellSize + Padding;
	private const float PaddedCellHeight = CellSize + Padding + NameHeight;

	private Color backgroundColor = new Color (1f, 1f, 1f, 0.5f);
	private Color contentColor = new Color (1f, 1f, 1f, 1.0f);

	private Vector2 scrollPosition = Vector2.zero;
	private float timer = 0.0f;
	private string searchName = "";

	private UGUIAtlas selectedAtlas = null;
	private UnityAction<UGUIAtlas> onClickAtlas = null;
	private UnityAction<UGUIAtlas> onDoubleClickAtlas = null;

	List<UGUIAtlas> atlases = null;
	List<UGUIAtlas> atlasToRemove = null;

	private bool highResolution = false;

	void OnEnable ()
	{
		instance = this;
		atlasToRemove = new List<UGUIAtlas> ();
		atlases = new List<UGUIAtlas> (Resources.FindObjectsOfTypeAll<UGUIAtlas> ());
		SortAtlases ();
	}

	void OnDisable ()
	{
		instance = null;
	}

	void OnGUI ()
	{
		if (atlases == null || atlases.Count == 0) {
			GUILayout.Label ("No Atlas found, please create an Atlas first.", "LODLevelNotifyText");
		} else {
			GUILayout.Label ("Atlases in Project", "LODLevelNotifyText");
			EditorUtils.DrawSeparator ();
			GUILayout.BeginHorizontal ();
			highResolution = GUILayout.Toggle(highResolution, "HighResolution");
			searchName = EditorUtils.DrawSearchBox (searchName);
			GUILayout.EndHorizontal ();
			int columns = 0;
			if (highResolution == false) {
				columns = Mathf.FloorToInt (Screen.width / PaddedCellWidth);
			} else {
				columns = Mathf.FloorToInt (Screen.width * 0.5f / PaddedCellWidth);
			}
			columns = columns < 1 ? 1 : columns;
			int rows = (int)Mathf.CeilToInt ((float)atlases.Count / columns);
			rows = rows < 1 ? 1 : rows;

			GUILayout.Space (10.0f);

			using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope (scrollPosition)) {
				scrollPosition = scrollViewScope.scrollPosition;

				GUILayout.Space (rows * PaddedCellHeight);

				for (int i = 0; i < atlases.Count; i++) {
					UGUIAtlas atlas = atlases [i];
					Rect rect = new Rect (PaddedCellWidth * (i % columns) + Padding, PaddedCellHeight * (i / columns) + Padding, CellSize, CellSize);

					if (atlas == null || atlas.Atlas == null) {
						if (atlasToRemove != null) {
							atlasToRemove.Add (atlas);
						}
						continue;
					}

					if (GUI.Button (rect, "")) {
						if (Event.current.button == 0) {
							float delta = Time.realtimeSinceStartup - timer;
							timer = Time.realtimeSinceStartup;

							if (onClickAtlas != null) {
								onClickAtlas (atlas);
							}

							if (selectedAtlas == atlas) {
								if (delta < DoubleClickThreshold) {
									if (onDoubleClickAtlas != null) {
										onDoubleClickAtlas (atlas);
									}
								} 
							} else {
								selectedAtlas = atlas;
							}
						}
					}

					if (Event.current.type == EventType.Repaint) {
						Rect clipRect = rect;
						float aspect = (float)atlas.Atlas.width / atlas.Atlas.height;

						if (aspect != 1.0f) {
							if (aspect < 1.0f) {
								float padding = CellSize * (1.0f - aspect) * 0.5f;
								clipRect.xMin += padding;
								clipRect.xMax -= padding;
							} else {								
								float padding = CellSize * (1.0f - 1.0f / aspect) * 0.5f;
								clipRect.yMin += padding;
								clipRect.yMax -= padding;
							}
						}
				
						EditorUtils.DrawContrastBackground (clipRect);
						GUI.DrawTexture (rect, atlas.Atlas,ScaleMode.ScaleToFit);

						if (atlas == selectedAtlas) {
							EditorUtils.DrawOutline (rect, Color.green);
						}
						GUI.backgroundColor = backgroundColor;
						GUI.contentColor = contentColor;
						GUI.Label (new Rect (rect.x, rect.y + rect.height, rect.width, NameHeight), atlas.name, "ProgressBarBack");
						GUI.contentColor = Color.white;
						GUI.backgroundColor = Color.white;
					}

				}

				if (atlasToRemove != null && atlasToRemove.Count > 0) {
					foreach (var item in atlasToRemove) {
						atlases.Remove (item);
					}
				}
				atlasToRemove.Clear ();
			}
		}
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Search Atlas", GUILayout.Height(100)) == true) {
			LoadAllAtlases ();
		}
	}

	private void SortAtlases()
	{
		if (atlases != null && atlases.Count > 0) {
			atlases.Sort (
				new Comparison<UGUIAtlas> ((atlas1, atlas2) => {
					return String.CompareOrdinal (atlas1.gameObject.name, atlas2.gameObject.name);
				})
			);
		}
	}

	private void LoadAllAtlases()
	{
		if (atlases == null) {
			atlases = new List<UGUIAtlas> ();
		} else {
			atlases.Clear ();
		}

		string[] paths = AssetDatabase.GetAllAssetPaths ();
		List<string> assetList = new List<string> ();

		for (int i = 0; i < paths.Length; i++) {
			if (paths [i].EndsWith (".prefab", StringComparison.OrdinalIgnoreCase) == true) {
				assetList.Add (paths [i]);
			}
		}

		for (int i = 0; i < assetList.Count; i++) {
			EditorUtility.DisplayProgressBar ("Searching", "Searching atlases, please wait...", (float)i / assetList.Count);
			UnityEngine.Object target = AssetDatabase.LoadMainAssetAtPath (assetList [i]);
			if (target != null && target is GameObject) {
				UGUIAtlas atlas = (target as GameObject).GetComponent<UGUIAtlas> ();
				if (atlas != null) {
					atlases.Add (atlas);
				}
			}
		}
		EditorUtility.ClearProgressBar();

		SortAtlases ();
	}

	private void SearchAtlas(string searchString)
	{
	}

	public static void Show (UnityAction<UGUIAtlas> onClickAtlas=null, UnityAction<UGUIAtlas> onDoubleClickAtlas=null, UGUIAtlas selectedAtlas=null)
	{
		if (instance != null) {
			instance.Close ();
			instance = null;
		}

		instance = ScriptableWizard.DisplayWizard<AtlasSelector> ("Select an atlas");
		instance.onClickAtlas = onClickAtlas;
		instance.onDoubleClickAtlas = onDoubleClickAtlas;
		instance.selectedAtlas = selectedAtlas;
	}

	public static void Hide()
	{
		if (instance != null) {
			instance.Close ();
			instance = null;
		}
	}
}