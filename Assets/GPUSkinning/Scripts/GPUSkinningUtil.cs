namespace chengkehan.GPUSkinning
{
    using UnityEngine;
    using System.Collections;
    using System.Security.Cryptography;
    using System;

    public class GPUSkinningUtil
    {
        public static void MarkAllScenesDirty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.CallbackFunction DelayCall = null;
                DelayCall = () =>
                {
                    UnityEditor.EditorApplication.delayCall -= DelayCall;
                    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                };
                UnityEditor.EditorApplication.delayCall += DelayCall;
            }
#endif
        }

        public static Texture2D CreateTexture2D(TextAsset textureRawData, GPUSkinningAnimation anim)
        {
            if (textureRawData == null || anim == null)
            {
                return null;
            }

            Texture2D texture = new Texture2D(anim.textureWidth, anim.textureHeight, TextureFormat.RGBAHalf, false, true);
            texture.name = "GPUSkinningTextureMatrix";
            texture.filterMode = FilterMode.Point;
            texture.LoadRawTextureData(textureRawData.bytes);
            texture.Apply(false, true);

            return texture;
        }

        public static string BonesHierarchyTree(GPUSkinningAnimation gpuSkinningAnimation)
        {
            if (gpuSkinningAnimation == null || gpuSkinningAnimation.bones == null)
            {
                return null;
            }

            string str = string.Empty;
            BonesHierarchy_Internal(gpuSkinningAnimation, gpuSkinningAnimation.bones[gpuSkinningAnimation.rootBoneIndex], string.Empty, ref str);
            return str;
        }

        public static void BonesHierarchy_Internal(GPUSkinningAnimation gpuSkinningAnimation, GPUSkinningBone bone, string tabs, ref string str)
        {
            str += tabs + bone.name + "\n";

            int numChildren = bone.childrenBonesIndices == null ? 0 : bone.childrenBonesIndices.Length;
            for (int i = 0; i < numChildren; ++i)
            {
                BonesHierarchy_Internal(gpuSkinningAnimation, gpuSkinningAnimation.bones[bone.childrenBonesIndices[i]], tabs + "    ", ref str);
            }
        }

        public static string BoneHierarchyPath(GPUSkinningBone[] bones, int boneIndex)
        {
            if (bones == null || boneIndex < 0 || boneIndex >= bones.Length)
            {
                return null;
            }

            GPUSkinningBone bone = bones[boneIndex];
            string path = bone.name;
            while (bone.parentBoneIndex != -1)
            {
                bone = bones[bone.parentBoneIndex];
                path = bone.name + "/" + path;
            }
            return path;
        }

        public static string BoneHierarchyPath(GPUSkinningAnimation gpuSkinningAnimation, int boneIndex)
        {
            if (gpuSkinningAnimation == null)
            {
                return null;
            }

            return BoneHierarchyPath(gpuSkinningAnimation.bones, boneIndex);
        }

        public static string MD5(string input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(input);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = string.Empty;
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();
        }

        public static int NormalizeTimeToFrameIndex(GPUSkinningClip clip, float normalizedTime)
        {
            if (clip == null)
            {
                return 0;
            }

            normalizedTime = Mathf.Clamp01(normalizedTime);
            return (int)(normalizedTime * (clip.length * clip.fps - 1));
        }

        public static float FrameIndexToNormalizedTime(GPUSkinningClip clip, int frameIndex)
        {
            if (clip == null)
            {
                return 0;
            }

            int totalFrams = (int)(clip.fps * clip.length);
            frameIndex = Mathf.Clamp(frameIndex, 0, totalFrams - 1);
            return (float)frameIndex / (float)(totalFrams - 1);
        }

        /// <summary>
        /// half-precision bytes to single-precision bytes.
        /// defination in IEEE 754.
        /// </summary>
        private static byte[] single = new byte[4];
        public static float HalfToSingle(byte expAndFractByte, byte fractByte)
        {
            byte expHalf = (byte)((expAndFractByte & 124) >> 2),
                    expSingle = expHalf != 0 ? (byte)(expHalf - 15 + 127) : (byte)0;

            single[3] = (byte)((expAndFractByte & 128) | (expSingle >> 1));
            single[2] = (byte)(expSingle << 7 | ((expAndFractByte & 3) << 5) | ((fractByte & 248) >> 3));
            single[1] = (byte)((fractByte & 7) << 5);
            single[0] = 0;

            return BitConverter.ToSingle(single, 0);
        }

        public const int matrixByteSize = 3 /*rows*/ * 4 /*rowitems*/ * 1 /*bytesize*/ * 2 /*half-2byte*/;
        public const int matrixColorSize = 3; // Matrix composed three Vector4, Color substitute Vector4.
        public const int matrixPixelSize = 3; // Matrix composed three Vector4, Color substitute Vector4.

        public static Matrix4x4 GetMatrixFromTexture(byte[] matrixBytes, int matrixIndex)
        {
            int byteIndex = GPUSkinningUtil.matrixByteSize * matrixIndex;

            return
                new Matrix4x4(
                    new Vector4(GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 0 * 2 + 1], matrixBytes[byteIndex + 0 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 4 * 2 + 1], matrixBytes[byteIndex + 4 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 8 * 2 + 1], matrixBytes[byteIndex + 8 * 2]), 0),
                    new Vector4(GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 1 * 2 + 1], matrixBytes[byteIndex + 1 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 5 * 2 + 1], matrixBytes[byteIndex + 5 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 9 * 2 + 1], matrixBytes[byteIndex + 9 * 2]), 0),
                    new Vector4(GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 2 * 2 + 1], matrixBytes[byteIndex + 2 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 6 * 2 + 1], matrixBytes[byteIndex + 6 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 10 * 2 + 1], matrixBytes[byteIndex + 10 * 2]), 0),
                    new Vector4(GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 3 * 2 + 1], matrixBytes[byteIndex + 3 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 7 * 2 + 1], matrixBytes[byteIndex + 7 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 11 * 2 + 1], matrixBytes[byteIndex + 11 * 2]), 1)
                    );
        }

        public static Matrix4x4 GetMatrixFromTexture(Color[] matrixColors, int matrixIndex)
        {
            int colorIndex = matrixIndex * GPUSkinningUtil.matrixColorSize;
            Color 
                color0 = matrixColors[colorIndex + 0], 
                color1 = matrixColors[colorIndex + 1], 
                color2 = matrixColors[colorIndex + 2];
            
            return
                new Matrix4x4(
                    new Vector4(color0.r, color1.r, color2.r, 0),
                    new Vector4(color0.g, color1.g, color2.g, 0),
                    new Vector4(color0.b, color1.b, color2.b, 0),
                    new Vector4(color0.a, color1.a, color2.a, 1)
                    );
        }

        /// <summary>
        /// pixels in Texture2D has composed by row-major.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="accumPixelIndex"></param>
        /// <param name="matrixIndex"></param>
        /// <returns></returns>
        public static Matrix4x4 GetMatrixFromTexture(Texture2D texture, int matrixIndex)
        {
            int pixelIndex = matrixIndex * GPUSkinningUtil.matrixPixelSize;

            Color 
                color0 = texture.GetPixel((pixelIndex + 0) % texture.width, (pixelIndex + 0) / texture.width), 
                color1 = texture.GetPixel((pixelIndex + 1) % texture.width, (pixelIndex + 1) / texture.width), 
                color2 = texture.GetPixel((pixelIndex + 2) % texture.width, (pixelIndex + 2) / texture.width);

            return
                new Matrix4x4(
                    new Vector4(color0.r, color1.r, color2.r, 0),
                    new Vector4(color0.g, color1.g, color2.g, 0),
                    new Vector4(color0.b, color1.b, color2.b, 0),
                    new Vector4(color0.a, color1.a, color2.a, 1)
                    );
        }
    }
}