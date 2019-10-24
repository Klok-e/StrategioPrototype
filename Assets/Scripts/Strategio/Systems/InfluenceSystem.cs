using Strategio.AuthoringGameObjects;
using Strategio.Components;
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
        public const int Falloff = 50;
        public JobHandle LatestJob { get; private set; }
        private InitDataSystem _dataSystem;
        private SetInfluenceTextureSystem _influenceTextureSystem;

        // cached stuff
        private NativeQueue<InflAndPos> _inflQueue;
        private NativeList<InflAndPos> _inflList;

        protected override void OnCreate()
        {
            base.OnCreate();
            _dataSystem = EntityManager.World.GetOrCreateSystem<InitDataSystem>();
            _influenceTextureSystem = EntityManager.World.GetOrCreateSystem<SetInfluenceTextureSystem>();

            _inflQueue = new NativeQueue<InflAndPos>(Allocator.Persistent);
            _inflList = new NativeList<InflAndPos>(Allocator.Persistent);
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
            var j1 = new GetInfluencersJob
            {
                influencers = _inflQueue.AsParallelWriter(),
                arenaSize = _dataSystem.MapSize,
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

        private struct InflAndPos
        {
            public Influencer infl;
            public int2 pos;
        }

        [BurstCompile]
        private struct GetInfluencersJob : IJobForEach_CC<Influencer, Translation>
        {
            public NativeQueue<InflAndPos>.ParallelWriter influencers;
            public int2 arenaSize;

            public void Execute(ref Influencer infl, ref Translation transl)
            {
                influencers.Enqueue(new InflAndPos
                {
                    infl = infl,
                    pos = math.int2(math.floor(transl.Value.xy)) + arenaSize / 2
                });
            }
        }

        [BurstCompile]
        private struct AddInflToList : IJob
        {
            public NativeQueue<InflAndPos> influencers;
            public NativeList<InflAndPos> inflList;

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
            [WriteOnly]
            public NativeArray2D<int> influencesMap;

            [ReadOnly]
            public NativeList<InflAndPos> influencers;

            public void Execute(int index)
            {
                influencesMap.At(index, out int x, out int y);
                int sum = 0;
                for (int i = 0; i < influencers.Length; i++)
                {
                    var t = influencers[i];
                    var infl = t.infl;
                    var pos = t.pos;
                    int sideMul = infl.side == Side.Friend ? 1 : -1;
                    int dist = (int) math.distance(math.float2(x, y), math.float2(pos));
                    sum += math.max(0, infl.num - dist * Falloff) * sideMul;
                }

                influencesMap[index] = sum;
            }
        }

        [BurstCompile]
        private struct ClearListJob : IJob
        {
            public NativeList<InflAndPos> inflList;

            public void Execute()
            {
                inflList.Clear();
            }
        }
    }
}