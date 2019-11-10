using Strategio.GameConfigs;
using Unity.Entities;

namespace Strategio.Components
{
    public struct AiComponent : IComponentData
    {
        public AiState aiState;
    }
}