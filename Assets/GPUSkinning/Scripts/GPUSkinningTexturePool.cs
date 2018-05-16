namespace chengkehan.GPUSkinning
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class GPUSkinningTexturePool 
    {
        private static Dictionary<string, Texture2D> textureDict;

        static GPUSkinningTexturePool()
        {
            textureDict = new Dictionary<string, Texture2D>();
        }

        public static Texture2D GetTexture(string key)
        {
            if (textureDict.ContainsKey(key))
                return textureDict[key];
            else
                return null;
        }

        public static Texture2D GetTextureOrAdd(string key, Func<Texture2D> makeTexture)
        {
            if (!textureDict.ContainsKey(key))
            {
                textureDict.Add(key, makeTexture());
            }
            else if (textureDict[key] == null)
            {
                textureDict[key] = makeTexture();
            }
                
            return textureDict[key];
        }

        public static void AddTexture(string key, Texture2D texture)
        {
            textureDict.Add(key, texture);
        }
    }
}