using UnityEngine.UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("UI/17zuoye/UIImage")]
public class UIImage : Image
{
    public Texture _atlasTexture;
    [SerializeField]
    private UGUIAtlas _uiAtlas;
    public UGUIAtlas UIAtlas
    {
        get { return _uiAtlas; }
        set { _uiAtlas = value; }
    }

    private string _spriteName;

    public string SpriteName
    {
        get
        {
            return _spriteName;
        }
        set
        {
            if (null == _uiAtlas)
            {
                sprite = null;
                return;
            }

            _spriteName = value;
            sprite = _uiAtlas.GetSprite(_spriteName);
        }
    }

    public float Alpha
    {
        get
        {
            return color.a;
        }
        set
        {
            color = new Color(color.r, color.g, color.b, value);
        }
    }

}
