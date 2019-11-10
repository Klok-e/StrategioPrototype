using Strategio.Components;
using Strategio.Util;
using Unity.Entities;
using Unity.Jobs;

namespace Strategio.Systems
{
    public class AiSystem : JobComponentSystem
    {
        private InitDataSystem _dataSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _dataSystem = World.GetOrCreateSystem<InitDataSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }

        private struct DoAiStuff : IJobForEach_C<AiComponent>
        {
            public NativeArray2D<float> influenceMap;

            public void Execute(ref AiComponent c0)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}