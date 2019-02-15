using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class AtlasSpriteSelector : ScriptableWizard {
	private static AtlasSpriteSelector instance = null;

	//双击时间阈值
	private const float DoubleClickThreshold = 0.3f;
	//单元格尺寸
	private const float CellSize = 80.0f;
	private const float Padding = 10.0f;
	private const float NameHeight = 40.0f;
	private const float PaddedCellWidth = CellSize + Padding;
	private const float PaddedCellHeight = CellSize + Padding + NameHeight;

	private Color backgroundColor = new Color (1f, 1f, 1f, 0.5f);
	private Color contentColor = new Color (1f, 1f, 1f, 1.0f);

	private Vector2 scrollPosition = Vector2.zero;
	//记录点击时间，用来模拟双击
	private float timer = 0;
	private string searchName = "";
	//目标图集
	private UGUIAtlas targetAtlas;

	//已选择的Sprite
	private Sprite currentSprite = null;
	private UnityAction<Sprite> onClickSprite = null;
	private UnityAction<Sprite> onDoubleClickSprite = null;

	private bool highResolution = false;

	void OnEnable(){
		instance = this;
		searchName = "";

	}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       

	void OnDisable(){
		instance = null;
	}

	void OnGUI()
	{
		if (targetAtlas == null) {
			GUILayout.Label ("No Atlas selected.", "LODLevelNotifyText");
		} else {
			GUILayout.Label (targetAtlas.name + " Sprites", "LODLevelNotifyText");
			EditorUtils.DrawSeparator ();

			GUILayout.BeginHorizontal ();
			highResolution = GUILayout.Toggle(highResolution, "HighResolution");
			searchName = EditorUtils.DrawSearchBox (searchName);
			GUILayout.EndHorizontal ();

			var sprites = targetAtlas.AllSprites;

			int columns = 0;
			if (highResolution == false) {
				columns = Mathf.FloorToInt (Screen.width / PaddedCellWidth);
			} else {
				columns = Mathf.FloorToInt (Screen.width * 0.5f / PaddedCellWidth);
			}
			columns = columns < 1 ? 1 : columns;
			int rows = (int)Mathf.CeilToInt ((float)sprites.Count / columns);
			rows = rows < 1 ? 1 : rows;

			GUILayout.Space (10.0f);

			using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope (scrollPosition)) {
				scrollPosition = scrollViewScope.scrollPosition;

				GUILayout.Space (rows * PaddedCellHeight);

				for(int i=0;i<sprites.Count; i++){
					
					Rect rect = new Rect (PaddedCellWidth * (i % columns) + Padding, PaddedCellHeight * (i / columns) + Padding, CellSize, CellSize);

					Sprite sprite = sprites [i];
					if (sprite == null)
						continue;

					if (GUI.Button (rect, "")) {
						if (Event.current.button == 0) {
							float delta = Time.realtimeSinceStartup - timer;
							timer = Time.realtimeSinceStartup;

							if (onClickSprite != null) {
								onClickSprite (sprite);
							}

							if (currentSprite == sprite) {
								if (delta < DoubleClickThreshold) {
									if (onDoubleClickSprite != null) {
										onDoubleClickSprite (sprite);
									}
								}
							} else {
								currentSprite = sprite;
							}

						}
					}

					if (Event.current.type == EventType.Repaint) {
						
						EditorUtils.DrawContrastBackground (rect);

						Rect clipRect = rect;
						float aspect = sprite.rect.width/sprite.rect.height;
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

						Texture targetTexture = targetAtlas.Atlas;

						EditorUtils.DrawTextureWithPixelCoords (clipRect, targetAtlas.Atlas, sprite.rect);
						if (currentSprite == sprite) {
							EditorUtils.DrawOutline (rect, Color.green);
						}

						GUI.backgroundColor = backgroundColor;
						GUI.contentColor = contentColor;
						GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width,NameHeight), sprite.name, "ProgressBarBack");
						GUI.contentColor = Color.white;
						GUI.backgroundColor = Color.white;
					}
				}
			}
		}	
	}

	public static void Show(UGUIAtlas atlas, UnityAction<Sprite> onClickSprite=null, UnityAction<Sprite> onDoubleClickSprite=null, Sprite currentSprite=null)
	{
		if (instance != null) {
			instance.Close ();
			instance = null;
		}
		instance = ScriptableWizard.DisplayWizard<AtlasSpriteSelector> ("Select a Sprite");
		instance.targetAtlas = atlas;
		instance.onClickSprite = onClickSprite;
		instance.onDoubleClickSprite = onDoubleClickSprite;
		instance.currentSprite = currentSprite;
	}

	public static void Hide()
	{
		if (instance != null) {
			instance.Close ();
			instance = null;
		}
	}
}