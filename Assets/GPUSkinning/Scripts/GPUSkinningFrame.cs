namespace chengkehan.GPUSkinning
{
    using UnityEngine;
    using System.Collections;
    using System;

    [System.Serializable]
    public class GPUSkinningFrame
    {
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
                rootMotionInv = matrices[rootBoneIndex].inverse;
                rootMotionInvInit = true;
            }
            return rootMotionInv;
        }

        private byte[] single = new byte[4];
        private float HalfToSingle(byte expAndFractByte, byte fractByte)
        {
            byte    expHalf = (byte)((expAndFractByte & 124) >> 2),
                    expSingle = expHalf != 0 ? (byte)(expHalf - 15 + 127) : (byte)0;

            single[3] = (byte)((expAndFractByte & 128) | (expSingle >> 1));
            single[2] = (byte)(expSingle << 7 | ((expAndFractByte & 3) << 5) | ((fractByte & 248) >> 3));
            single[1] = (byte)((fractByte & 7) << 5);
            single[0] = 0;

            return BitConverter.ToSingle(single, 0);
        }

        public int SetMatrixFromTexture(byte[] matrixBytes, int accumByteIndex, int totalBoneCount)
        {
            int matrixSize = 3 /*rows*/ * 4 /*rowitems*/ * 1 /*bytesize*/ * 2 /*half-2byte*/;

            if (matrices == null)   matrices = new Matrix4x4[totalBoneCount];
            else                    Array.Resize(ref matrices, totalBoneCount);
            
            for (int i = 0; i < matrices.Length; i++)
            {
                int byteIndex = accumByteIndex + matrixSize * i;

                matrices[i] =
                    new Matrix4x4(
                        new Vector4(HalfToSingle(matrixBytes[byteIndex + 0 * 2 + 1], matrixBytes[byteIndex + 0 * 2]), HalfToSingle(matrixBytes[byteIndex + 4 * 2 + 1], matrixBytes[byteIndex + 4 * 2]), HalfToSingle(matrixBytes[byteIndex + 8 * 2 + 1], matrixBytes[byteIndex + 8 * 2]), 0),
                        new Vector4(HalfToSingle(matrixBytes[byteIndex + 1 * 2 + 1], matrixBytes[byteIndex + 1 * 2]), HalfToSingle(matrixBytes[byteIndex + 5 * 2 + 1], matrixBytes[byteIndex + 5 * 2]), HalfToSingle(matrixBytes[byteIndex + 9 * 2 + 1], matrixBytes[byteIndex + 9 * 2]), 0),
                        new Vector4(HalfToSingle(matrixBytes[byteIndex + 2 * 2 + 1], matrixBytes[byteIndex + 2 * 2]), HalfToSingle(matrixBytes[byteIndex + 6 * 2 + 1], matrixBytes[byteIndex + 6 * 2]), HalfToSingle(matrixBytes[byteIndex + 10 * 2 + 1], matrixBytes[byteIndex + 10 * 2]), 0),
                        new Vector4(HalfToSingle(matrixBytes[byteIndex + 3 * 2 + 1], matrixBytes[byteIndex + 3 * 2]), HalfToSingle(matrixBytes[byteIndex + 7 * 2 + 1], matrixBytes[byteIndex + 7 * 2]), HalfToSingle(matrixBytes[byteIndex + 11 * 2 + 1], matrixBytes[byteIndex + 11 * 2]), 1)
                        );
            }

            return matrices.Length * matrixSize;
        }

        public int SetMatrixFromTexture(Color[] matrixColors, int accumColorIndex, int totalBoneCount)
        {
            int stepSize = 3; // Matrix composed three Vector4
            
            if (matrices == null) matrices = new Matrix4x4[totalBoneCount];
            else Array.Resize(ref matrices, totalBoneCount);

            Color color0, color1, color2;

            for (int i = 0; i < matrices.Length; i++)
            {
                int colorIndex = accumColorIndex + i * stepSize;

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

            return matrices.Length * stepSize;
        }
    }
}