namespace chengkehan.GPUSkinning
{
    using UnityEngine;
    using System.Collections;

    public class GPUSkinningAnimation : ScriptableObject
    {
        public string guid = null;

        public string name = null;

        public GPUSkinningBone[] bones = null;

        public int rootBoneIndex = 0;

        public GPUSkinningClip[] clips = null;

        public Bounds bounds;

        public int textureWidth = 0;

        public int textureHeight = 0;

        public float[] lodDistances = null;

        public Mesh[] lodMeshes = null;

        public float sphereRadius = 1.0f;

        public TextAsset vertexMatrixPerClipBytes;

        [ContextMenu("Set Matrices from TextAsset.bytes")]
        public void SetMatrixFromTextAsset()
        {
            byte[] bytes = vertexMatrixPerClipBytes.bytes;
            int caculateIndexOffset = 0;
            
            for (int i = 0; i < clips.Length; i++)
                caculateIndexOffset += clips[i].SetMatrixFromTexture(bytes, caculateIndexOffset, bones.Length);
        }

        public Texture2D vertexMatrixTexturePerClipBytes;

        [ContextMenu("Set Matrices from Texture2D.GetPixels")]
        public void SetMatrixFromTexture()
        {
            Color[] colors = vertexMatrixTexturePerClipBytes.GetPixels(); 
            int caculateIndexOffset = 0;

            for (int i = 0; i < clips.Length; i++)
                caculateIndexOffset += clips[i].SetMatrixFromTexture(colors, caculateIndexOffset, bones.Length);
        }
    }
}
