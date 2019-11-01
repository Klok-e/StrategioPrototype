using Strategio.AuthoringGameObjects;
using Strategio.Components;
using Strategio.GameConfigs;
using Strategio.Util;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Strategio.Systems
{
    [AlwaysUpdateSystem]
    public class InfluenceSystem : JobComponentSystem
    {
        public const int Falloff = 1;
        public JobHandle LatestJob { get; private set; }
        private InitDataSystem _dataSystem;
        private SetInfluenceTextureSystem _influenceTextureSystem;

        // cached stuff
        private NativeQueue<InflPosSide> _inflQueue;
        private NativeList<InflPosSide> _inflList;

        protected override void OnCreate()
        {
            base.OnCreate();
            _dataSystem = EntityManager.World.GetOrCreateSystem<InitDataSystem>();
            _influenceTextureSystem = EntityManager.World.GetOrCreateSystem<SetInfluenceTextureSystem>();

            _inflQueue = new NativeQueue<InflPosSide>(Allocator.Persistent);
            _inflList = new NativeList<InflPosSide>(Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _inflQueue.Dispose();
            _inflList.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var infls = _dataSystem.Influences;
            _inflList.Clear();
            var j1 = new GetInfluencersJob
            {
                influencers = _inflQueue.AsParallelWriter(),
                arenaSize = _dataSystem.MapSize,
                mapRes = _dataSystem.MapResolution,
            };
            var j2 = new AddInflToList
            {
                influencers = _inflQueue,
                inflList = _inflList,
            };
            var j3 = new SetInfluenceArrayJob
            {
                influencesMap = infls,
                influencers = _inflList,
            };
            var j4 = new ClearListJob
            {
                inflList = _inflList
            };
            LatestJob = j4.Schedule(
                j3.Schedule(infls.XMax * infls.YMax, 128,
                    j2.Schedule(
                        j1.Schedule(this,
                            JobHandle.CombineDependencies(inputDeps, _influenceTextureSystem.LatestJob)))));
            return LatestJob;
        }

        private struct InflPosSide
        {
            public InfluencerComponent infl;
            public int2 pos;
            public SideComponent side;
        }

        [BurstCompile]
        private struct GetInfluencersJob : IJobForEach_CCC<InfluencerComponent, Translation, SideComponent>
        {
            public NativeQueue<InflPosSide>.ParallelWriter influencers;
            public int2 arenaSize;
            public int mapRes;

            public void Execute([ReadOnly] ref InfluencerComponent infl,
                [ReadOnly] ref Translation transl,
                [ReadOnly] ref SideComponent side)
            {
                var inflCpy = infl;
                inflCpy.num *= mapRes;
                influencers.Enqueue(new InflPosSide
                {
                    infl = inflCpy,
                    pos = math.int2(math.floor((transl.Value.xy + math.float2(arenaSize) / 2) * mapRes)),
                    side = side
                });
            }
        }

        [BurstCompile]
        private struct AddInflToList : IJob
        {
            public NativeQueue<InflPosSide> influencers;
            public NativeList<InflPosSide> inflList;

            public void Execute()
            {
                int count = influencers.Count;
                for (int i = 0; i < count; i++)
                    inflList.Add(influencers.Dequeue());
            }
        }

        [BurstCompile]
        private struct SetInfluenceArrayJob : IJobParallelFor
        {
            [WriteOnly] public NativeArray2D<int> influencesMap;

            [ReadOnly] public NativeList<InflPosSide> influencers;

            public void Execute(int index)
            {
                influencesMap.At(index, out int x, out int y);
                int sum = 0;
                for (int i = 0; i < influencers.Length; i++)
                {
                    var t = influencers[i];
                    var infl = t.infl;
                    var pos = t.pos;
                    var side = t.side;
                    int sideMul = side.side == Side.Player1 ? 1 : -1;
                    float dist = math.distance(math.float2(x, y), math.float2(pos));
                    sum += math.max(0, (int) (infl.num - dist * Falloff)) * sideMul;
                    if ((infl.num - dist * Falloff) > 0)
                        continue;
                }

                influencesMap[index] = sum;
            }
        }

        [BurstCompile]
        private struct ClearListJob : IJob
        {
            public NativeList<InflPosSide> inflList;

            public void Execute()
            {
                inflList.Clear();
            }
        }
    }
}