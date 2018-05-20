namespace chengkehan.GPUSkinning
{
    using UnityEngine;
    using System.Collections;
    using System;

    [System.Serializable]
    public class GPUSkinningClip
    {
        public string name = null;

        public float length = 0.0f;

        public int fps = 0;

        public GPUSkinningWrapMode wrapMode = GPUSkinningWrapMode.Once;

        public GPUSkinningFrame[] frames = null;

        public int pixelSegmentation = 0;

        public bool rootMotionEnabled = false;

        public bool individualDifferenceEnabled = false;

        public GPUSkinningAnimEvent[] events = null;

        public int matrixStartIndex;
        public int matrixCount;
        
        public int SetMatrixFromTexture(byte[] matrixBytes, int accumByteIndex, int totalBoneCount)
        {
            int caculateIndexOffset = 0;

            for (int i = 0; i < frames.Length; i++)
            {
                GPUSkinningFrame frame = frames[i];
                caculateIndexOffset += frame.SetMatrixFromTexture(matrixBytes, accumByteIndex + caculateIndexOffset, totalBoneCount);
            }

            return caculateIndexOffset;
        }
        
        public int SetMatrixFromTexture(Color[] matrixColors, int accumByteIndex, int totalBoneCount)
        {
            int caculateIndexOffset = 0;

            for (int i = 0; i < frames.Length; i++)
            {
                GPUSkinningFrame frame = frames[i];
                caculateIndexOffset += frame.SetMatrixFromTexture(matrixColors, accumByteIndex + caculateIndexOffset, totalBoneCount);
            }

            return caculateIndexOffset;
        }

        [System.NonSerialized]
        public Texture2D matrixTexture;
        [System.NonSerialized]
        public Color[] colorsForMatrix = null;
        [System.NonSerialized]
        public int boneLength;

        public void SetTextureForMatrix(Texture2D texture, Color[] colors, int boneLength)
        {
            matrixTexture = texture;
            colorsForMatrix = colors;
            this.boneLength = boneLength;

            for (int i = 0; i < frames.Length; i++)
                frames[i].SetTextureForMatrix(texture, colors, matrixStartIndex + i * boneLength);
        }

        public Matrix4x4 GetMatrixInTexture(int frameIndex, int boneIndex)
        {
            if (colorsForMatrix == null)
                return GPUSkinningUtil.GetMatrixFromTexture(matrixTexture, matrixStartIndex + frameIndex * boneLength + boneIndex);
            else
                return GPUSkinningUtil.GetMatrixFromTexture(colorsForMatrix, matrixStartIndex + frameIndex * boneLength + boneIndex);
        }
    }
}