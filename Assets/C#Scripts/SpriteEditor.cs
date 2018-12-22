using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteEditor
{
    public static Texture2D Texture2DFromFile(string path)
    {
        Texture2D texture = null;
        if (File.Exists(path))
        {
            //byte取得
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader bin = new BinaryReader(fileStream);
            byte[] readBinary = bin.ReadBytes((int) bin.BaseStream.Length);
            bin.Close();
            fileStream.Dispose();
            fileStream = null;
            if (readBinary != null)
            {
                //横サイズ
                int pos = 16;
                int width = 0;
                for (int i = 0; i < 4; i++)
                {
                    width = width * 256 + readBinary[pos++];
                }

                //縦サイズ
                int height = 0;
                for (int i = 0; i < 4; i++)
                {
                    height = height * 256 + readBinary[pos++];
                }

                //byteからTexture2D作成
                texture = new Texture2D(width, height);
                texture.LoadImage(readBinary);
            }

            readBinary = null;
        }

        return texture;
    }

    public static Sprite SpriteFromTexture2D(Texture2D texture)
    {
        Sprite sprite = null;
        if (texture)
        {
            //Texture2DからSprite作成
            sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        return sprite;
    }

    public static Sprite SpriteFromFile(string path)
    {
        Sprite sprite = null;
        Texture2D texture = Texture2DFromFile(path);
        if (texture)
        {
            //Texture2DからSprite作成
            sprite = SpriteFromTexture2D(texture);
        }

        texture = null;
        return sprite;
    }
}