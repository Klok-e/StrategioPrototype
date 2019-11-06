using System;
using Strategio.Components;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Strategio.GameConfigs
{
    [Serializable]
    public struct UnitCommonConfig
    {
        [SerializeField]
        public float colliderRadius;

        [SerializeField]
        public Texture2D mainTex;

        [SerializeField]
        public Color tint;

        [SerializeField]
        public RenderMesh mesh;

        [SerializeField]
        public float z;

        [SerializeField]
        public InfluencerComponent influencerComponent;

        [SerializeField]
        public Scale scale;
    }
}