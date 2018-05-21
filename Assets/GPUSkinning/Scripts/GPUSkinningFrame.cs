namespace chengkehan.GPUSkinning
{
    using UnityEngine;
    using System.Collections;
    using System;

    [System.Serializable]
    public class GPUSkinningFrame
    {
        //[System.NonSerialized]
        public Matrix4x4[] matrices = null;

        public Quaternion rootMotionDeltaPositionQ;

        public float rootMotionDeltaPositionL;

        public Quaternion rootMotionDeltaRotation;

        [System.NonSerialized]
        private bool rootMotionInvInit = false;
        [System.NonSerialized]
        private Matrix4x4 rootMotionInv;
        public Matrix4x4 RootMotionInv(int rootBoneIndex)
        {
            if (!rootMotionInvInit)
            {
                if (matrixTexture != null || colorsForMatrix != null)
                {
                    rootMotionInv = GetMatrixInTexture(rootBoneIndex).inverse;
                    rootMotionInvInit = true;
                }
                else
                {
                    rootMotionInv = matrices[rootBoneIndex].inverse;
                    rootMotionInvInit = true;
                }
            }
            return rootMotionInv;
        }

        [System.NonSerialized]
        public Texture2D matrixTexture;
        [System.NonSerialized]
        public Color[] colorsForMatrix = null;
        [System.NonSerialized]
        public int matrixStartIndex;

        public void SetTextureForMatrix(Texture2D texture, Color[] colors, int matrixStartIndex)
        {
            matrixTexture = texture;
            colorsForMatrix = colors;
            this.matrixStartIndex = matrixStartIndex;
        }

        public Matrix4x4 GetMatrixInTexture(int boneIndex)
        {
            if (colorsForMatrix == null)
                return GPUSkinningUtil.GetMatrixFromTexture(matrixTexture, matrixStartIndex + boneIndex);
            else
                return GPUSkinningUtil.GetMatrixFromTexture(colorsForMatrix, matrixStartIndex + boneIndex);
        }

        public int SetMatrixFromTexture(byte[] matrixBytes, int accumByteIndex, int totalBoneCount)
        {
            if (matrices == null)   matrices = new Matrix4x4[totalBoneCount];
            else                    Array.Resize(ref matrices, totalBoneCount);
            
            for (int i = 0; i < matrices.Length; i++)
            {
                int byteIndex = accumByteIndex + GPUSkinningUtil.matrixByteSize * i;

                matrices[i] =
                    new Matrix4x4(
                        new Vector4(GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 0 * 2 + 1], matrixBytes[byteIndex + 0 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 4 * 2 + 1], matrixBytes[byteIndex + 4 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 8 * 2 + 1], matrixBytes[byteIndex + 8 * 2]), 0),
                        new Vector4(GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 1 * 2 + 1], matrixBytes[byteIndex + 1 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 5 * 2 + 1], matrixBytes[byteIndex + 5 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 9 * 2 + 1], matrixBytes[byteIndex + 9 * 2]), 0),
                        new Vector4(GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 2 * 2 + 1], matrixBytes[byteIndex + 2 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 6 * 2 + 1], matrixBytes[byteIndex + 6 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 10 * 2 + 1], matrixBytes[byteIndex + 10 * 2]), 0),
                        new Vector4(GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 3 * 2 + 1], matrixBytes[byteIndex + 3 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 7 * 2 + 1], matrixBytes[byteIndex + 7 * 2]), GPUSkinningUtil.HalfToSingle(matrixBytes[byteIndex + 11 * 2 + 1], matrixBytes[byteIndex + 11 * 2]), 1)
                        );
            }

            return matrices.Length * GPUSkinningUtil.matrixByteSize;
        }
        public int SetMatrixFromTexture(Color[] matrixColors, int accumColorIndex, int totalBoneCount)
        {           
            if (matrices == null) matrices = new Matrix4x4[totalBoneCount];
            else Array.Resize(ref matrices, totalBoneCount);

            Color color0, color1, color2;

            for (int i = 0; i < matrices.Length; i++)
            {
                int colorIndex = accumColorIndex + i * GPUSkinningUtil.matrixColorSize;

                color0 = matrixColors[colorIndex + 0];
                color1 = matrixColors[colorIndex + 1];
                color2 = matrixColors[colorIndex + 2];

                matrices[i] =
                    new Matrix4x4(
                        new Vector4(color0.r, color1.r, color2.r, 0),
                        new Vector4(color0.g, color1.g, color2.g, 0),
                        new Vector4(color0.b, color1.b, color2.b, 0),
                        new Vector4(color0.a, color1.a, color2.a, 1)
                        );
            }

            return matrices.Length * GPUSkinningUtil.matrixColorSize;
        }
    }
}
