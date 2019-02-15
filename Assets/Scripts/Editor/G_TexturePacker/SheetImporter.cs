using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEditor;

/// <summary>
/// added by ggr @2018/04/20
/// 
/// 图集导入设置,主要作用负责把由TP打出来的一张打图和配置分割成小的sprite
/// 并写入9宫信息边框等信息
/// </summary>
public class SheetImporter
{
    public SheetImporter(string selectPath, string tpOUT, Dictionary<string, SpriteMetaData> metaInfo)
    {
        _selectPath = selectPath;
        int __index_ = _selectPath.LastIndexOf('/') + 1;
        _folderName = _selectPath.Substring(__index_, _selectPath.Length - __index_);
        _sheetFullPath = Path.Combine(Path.Combine(Application.dataPath, tpOUT), _folderName);
        _sheetUnityPath = "Assets/" + tpOUT + "/" + _folderName;
        _savedMetaInfo = metaInfo;
        DeserialiseXML();
        WriteMetaData();
        SerialiseSprites();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void DeserialiseXML()
    {
        _spriteMetaDataList = new List<SpriteMetaData>();
        string __path = _sheetFullPath + ".xml";
        string __xmlString = File.ReadAllText(__path);
        XmlDocument __xmlDoc = new XmlDocument();
        __xmlDoc.LoadXml(__xmlString);
        XmlElement __elm = __xmlDoc.SelectSingleNode("TextureAtlas") as XmlElement;
        float __sheetHeight = float.Parse(__elm.GetAttribute("height"));
        for (int i = 0; i < __elm.ChildNodes.Count; i++)
        {
            XmlElement __child = __elm.ChildNodes[i] as XmlElement;
            string __sptName = __child.GetAttribute("name");
            SpriteMetaData __data = default(SpriteMetaData);
            if(_savedMetaInfo != null && _savedMetaInfo.ContainsKey(__sptName))
            {
                __data = _savedMetaInfo[__sptName];
            }
            else
            {
                __data.name = __sptName;
                __data.pivot = Vector2.one * 0.5f;
                float x = float.Parse(__child.GetAttribute("x"));
                float y = float.Parse(__child.GetAttribute("y"));
                float w = float.Parse(__child.GetAttribute("width"));
                float h = float.Parse(__child.GetAttribute("height"));
                __data.rect = new Rect(x, __sheetHeight - y - h, w, h);
            }

            _spriteMetaDataList.Add(__data);
        }
        File.Delete(__path);
    }

    private void WriteMetaData()
    {
        string __Path = _sheetUnityPath + ".png";
        TextureImporter asetImp = TextureImporter.GetAtPath(__Path) as TextureImporter;
        asetImp.spritesheet = _spriteMetaDataList.ToArray();
        asetImp.textureType = TextureImporterType.Sprite;
        asetImp.spriteImportMode = SpriteImportMode.Multiple;
        asetImp.mipmapEnabled = false;
        asetImp.SaveAndReimport();
    }

    private void SerialiseSprites()
    {
        string _Path = _sheetUnityPath + ".png";
        string _atlasPath = _sheetUnityPath + ".prefab";
        UGUIAtlas _atlasClass = null;
        if (File.Exists(_atlasPath))
        {
            GameObject basePrebab = AssetDatabase.LoadAssetAtPath(_atlasPath, typeof(GameObject)) as GameObject;
            GameObject prefabGameobject = PrefabUtility.InstantiatePrefab(basePrebab) as GameObject;
            if (prefabGameobject)
            {
                _atlasClass = prefabGameobject.GetComponent<UGUIAtlas>();
                _atlasClass.Atlas = AssetDatabase.LoadAssetAtPath<Texture>(_Path);
                _atlasClass.AllSprites = new List<Sprite>();
                Object[] _allAsset = AssetDatabase.LoadAllAssetsAtPath(_Path);
                for (int i = 0; i < _allAsset.Length; i++)
                {
                    if (!(_allAsset[i] is Sprite))
                        continue;
                    Sprite _spt = _allAsset[i] as Sprite;
                    _atlasClass.AllSprites.Add(_spt);
                }
            }
            PrefabUtility.ReplacePrefab(prefabGameobject, basePrebab, ReplacePrefabOptions.Default);
            MonoBehaviour.DestroyImmediate(prefabGameobject);
        }
        else
        {
            GameObject _prefab = new GameObject(_folderName);
            _atlasClass = _prefab.AddComponent<UGUIAtlas>();
            _atlasClass.Atlas = AssetDatabase.LoadAssetAtPath<Texture>(_Path);
            _atlasClass.AllSprites = new List<Sprite>();
            Object[] _allAsset = AssetDatabase.LoadAllAssetsAtPath(_Path);
            for (int i = 0; i < _allAsset.Length; i++)
            {
                if (!(_allAsset[i] is Sprite))
                    continue;
                Sprite _spt = _allAsset[i] as Sprite;
                _atlasClass.AllSprites.Add(_spt);
            }
            PrefabUtility.CreatePrefab(_atlasPath, _prefab, ReplacePrefabOptions.ConnectToPrefab);
            MonoBehaviour.DestroyImmediate(_prefab);
        }
    }

    private string _selectPath;
    private string _sheetFullPath;
    private string _sheetUnityPath;
    private string _folderName;
    private List<SpriteMetaData> _spriteMetaDataList;
    private Dictionary<string, SpriteMetaData> _savedMetaInfo;
}


