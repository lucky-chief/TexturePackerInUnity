using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

//[CustomEditor(typeof(UIImage))]
[CanEditMultipleObjects]
public class UIImageEditor : ImageEditor
{
    SerializedProperty m_atlas;

    UIImage _target;
    bool _warning = false;
    string _wrongName = "";
    string prompt1 = "图集为空！{0}";
    string prompt2 = "图集里没有名为{0}这个图！";
    string prompt = "";

    protected override void OnEnable()
    {
        base.OnEnable();
        m_atlas = serializedObject.FindProperty("_atlasTexture");
        _target = target as UIImage;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(m_atlas, new GUIContent("图集"));
        if(_warning)
        {
            EditorGUILayout.HelpBox(string.Format(prompt, _wrongName), MessageType.Warning);
        }
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
        if (_target._atlasTexture != null)
        {
            string _atlasPath = AssetDatabase.GetAssetPath(_target._atlasTexture);
            string _prefabPath = _atlasPath.Replace("png", "prefab");
            _target.UIAtlas = AssetDatabase.LoadAssetAtPath<UGUIAtlas>(_prefabPath);
            if (string.IsNullOrEmpty(_atlasPath))
            {
                _warning = true;
                prompt = prompt1;
                _wrongName = "";
                return;
            }

            Sprite[] _allSprites = _target.UIAtlas.AllSprites.ToArray();
            if (_allSprites.Length == 0)
                return;
            string _spriteName = "";
            if (_target.sprite != null)
            {
                _spriteName = _target.sprite.name;
                _wrongName = _spriteName;
            }
            if (_spriteName != "")
            {
                _warning = false;
                _target.SpriteName = _spriteName;
            }
            if(_target.sprite == null)
            {
                _warning = true;
                prompt = prompt2;
            }
        }
        else
        {
            _target.sprite = null;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
