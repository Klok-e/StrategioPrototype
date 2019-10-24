using Strategio.AuthoringGameObjects;
using Strategio.Util;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Strategio.Systems
{
    [UpdateAfter(typeof(InfluenceSystem))]
    [AlwaysUpdateSystem]
    public class SetInfluenceTextureSystem : JobComponentSystem
    {
        public const int InfluenceCutoff = 1000;
        public JobHandle LatestJob { get; private set; }

        //cached stuff
        private static readonly int Property = Shader.PropertyToID("_InfluenceMap");

        private InitDataSystem _dataSystem;
        private InfluenceSystem _influenceSystem;

        //private TextureEditorView _texView;

        protected override void OnCreate()
        {
            base.OnCreate();
            _dataSystem = World.GetOrCreateSystem<InitDataSystem>();
            _influenceSystem = World.GetOrCreateSystem<InfluenceSystem>();
            //_texView = new GameObject("TextureView").AddComponent<TextureEditorView>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            _dataSystem.InfluenceTexture.Apply();
            //_texView.texture = _dataSystem.InfluenceTexture;
            _dataSystem.SpriteRenderer.material.SetTexture(Property, _dataSystem.InfluenceTexture);
            var tex = _dataSystem.InfluenceTexture.GetRawTextureData<float>();

            var j1 = new SetTextureJob
            {
                texture = tex,
                influences = _dataSystem.Influences,
            };
            LatestJob = j1.Schedule(tex.Length, 128,
                JobHandle.CombineDependencies(inputDeps, _influenceSystem.LatestJob));
            return LatestJob;
        }

        [BurstCompile]
        private struct SetTextureJob : IJobParallelFor
        {
            public NativeArray<float> texture;

            [ReadOnly]
            public NativeArray2D<int> influences;

            public void Execute(int index)
            {
                int clamped = math.clamp(influences[index], -InfluenceCutoff, InfluenceCutoff);
                float val = (float) (clamped + InfluenceCutoff) / (InfluenceCutoff * 2);
                texture[index] = val;
            }
        }
    }
}