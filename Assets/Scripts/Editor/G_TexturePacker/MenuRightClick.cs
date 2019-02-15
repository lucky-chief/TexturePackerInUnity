using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// added by ggr @2018/04/20
/// 
/// 实现在 project 试图下右键有 Pack Sprites 的菜单
/// 注意只有在 Assets/Resources/unpack/Image 的子目录下右键才会执行打图集操作
/// 
/// 1、选中哪个文件夹就会打出哪个文件夹下的小图图集。
/// 2、打出来的大图放到了 TPConfig.txt 配置的 OUT 参数目录下，名字是上一步选中的文件夹的名字
/// </summary>
public class MenuRightClick
{
    [MenuItem("Tools/Pack Sprites")]
    private static void PackSprite()
    {
        DirectoryInfo __dir = new DirectoryInfo(Application.dataPath);
        string _path = __dir.Parent.Parent.FullName + "/res/";
        string __selectedPath = EditorUtility.OpenFolderPanel("请选择要打图集的文件夹",_path, "");
        if (!string.IsNullOrEmpty(__selectedPath))
        {
            TPPacker __packer = new TPPacker(__selectedPath);
            if (__packer.ValidateTPExePath())
            {
                if (__packer.SaveMetaInfo() != -1 && __packer.ExcuteTPCommand())
                {
                    AssetDatabase.Refresh();
                    new SheetImporter(__selectedPath, __packer.tpOUT, __packer.spriteInfoMap);
                }
            }
        }
    }

   
}
