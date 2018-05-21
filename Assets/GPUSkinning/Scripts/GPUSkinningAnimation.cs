namespace chengkehan.GPUSkinning
{
    using UnityEngine;
    using System.Collections;
    using System;

    [ExecuteInEditMode]
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

        public Texture2D matrixTexture;
        [System.NonSerialized]
        public Color[] colorsForMatrix = null;

        public bool preloadMatrixAsColor;

        public Matrix4x4 GetMatrixInTexture(int clipIndex, int frameIndex, int boneIndex)
        {
            if (colorsForMatrix == null)
                return
                    GPUSkinningUtil.GetMatrixFromTexture(
                        matrixTexture,
                        clips[clipIndex].matrixStartIndex + frameIndex * bones.Length + boneIndex
                        );
            else
                return 
                    GPUSkinningUtil.GetMatrixFromTexture(
                        colorsForMatrix,
                        clips[clipIndex].matrixStartIndex + frameIndex * bones.Length + boneIndex
                        );
        }

        private void OnEnable()
        {
            if (matrixTexture != null)
                if (preloadMatrixAsColor)
                    colorsForMatrix = matrixTexture.GetPixels();

            if (clips != null)
                Array.ForEach(clips, (clip) => { clip.SetTextureForMatrix(matrixTexture, colorsForMatrix, bones.Length); });
        }

        [ContextMenu("Set Matrices from Texture2D.GetPixels")]
        public void SetMatrixFromTexture()
        {
            Color[] colors = matrixTexture.GetPixels();
            int caculateIndexOffset = 0;

            for (int i = 0; i < clips.Length; i++)
                caculateIndexOffset += clips[i].SetMatrixFromTexture(colors, caculateIndexOffset, bones.Length);
        }

    }
}
