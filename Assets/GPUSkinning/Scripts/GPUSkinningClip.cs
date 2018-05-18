namespace chengkehan.GPUSkinning
{
    using UnityEngine;
    using System.Collections;

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
    }
}