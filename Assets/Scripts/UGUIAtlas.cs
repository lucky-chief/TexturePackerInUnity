using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// added by ggr @2018/05/11
/// UGUI 图集
/// 实现类似 NGUI 的 UIAtlas
/// 使用指南：
/// 1、该类只是一个序列化类，在游戏运行前就已经序列化好了图集里所有的精灵，所以不要在程序里改变AllSprites列表里的值
/// 2、一般来讲，此类只给UIImage使用。
/// </summary>
public class UGUIAtlas : MonoBehaviour
{
    [SerializeField]
    protected Texture _atlas;
    [SerializeField]
    protected List<Sprite> _allSprites;

//#if UNITY_EDITOR
    public Texture Atlas { get { return _atlas; } set { _atlas = value; } }
//#endif

    public List<Sprite> AllSprites {
        get { return _allSprites; }
        set { _allSprites = value; }
    }

    public Sprite GetSprite(string spriteName)
    {
        for(int i = 0; i < _allSprites.Count; i++)
        {
            Sprite _spt = _allSprites[i];
            if (spriteName.Equals(_spt.name))
                return _spt;
        }
        return null;
    }

    void OnDestroy()
    {
        _allSprites.Clear();
        _atlas = null;
    }
}
