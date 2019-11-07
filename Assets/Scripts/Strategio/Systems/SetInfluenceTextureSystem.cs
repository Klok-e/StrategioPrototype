using Strategio.AuthoringGameObjects;
using Strategio.Util;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Strategio.Systems
{
    [UpdateAfter(typeof(InfluenceSystem))]
    [AlwaysUpdateSystem]
    public class SetInfluenceTextureSystem : JobComponentSystem
    {
        public const float InfluenceCutoff = 20f;
        public JobHandle LatestJob { get; private set; }

        //cached stuff
        private InitDataSystem _dataSystem;
        private InfluenceSystem _influenceSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _dataSystem = World.GetOrCreateSystem<InitDataSystem>();
            _influenceSystem = World.GetOrCreateSystem<InfluenceSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var tex = _dataSystem.InfluenceTexture.GetRawTextureData<float>();
            var j1 = new SetTextureJob
            {
                texture = tex,
                influences = _dataSystem.Influences,
                cutoff = InfluenceCutoff * _dataSystem.MapResolution,
            };
            LatestJob = j1.Schedule(tex.Length, 128,
                JobHandle.CombineDependencies(inputDeps, _influenceSystem.LatestJob));
            return LatestJob;
        }

        [BurstCompile]
        private struct SetTextureJob : IJobParallelFor
        {
            public float cutoff;

            public NativeArray<float> texture;

            [ReadOnly] public NativeArray2D<float> influences;

            public void Execute(int index)
            {
                float clamped = math.clamp(influences[index], -cutoff, cutoff);
                float val = (clamped + cutoff) / (cutoff * 2);
                texture[index] = val;
            }
        }
    }
}