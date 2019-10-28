using Strategio.AuthoringGameObjects;
using Strategio.Util;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Strategio.Systems
{
    [UpdateAfter(typeof(SetInfluenceTextureSystem))]
    public class ApplyTextureSystem : ComponentSystem
    {
        private static readonly int Property = Shader.PropertyToID("_InfluenceMap");

        private InitDataSystem _dataSystem;

        private TextureEditorView _testTex;

        protected override void OnCreate()
        {
            base.OnCreate();
            _dataSystem = World.GetOrCreateSystem<InitDataSystem>();
            _testTex=new GameObject("tex").AddComponent<TextureEditorView>();
        }

        protected override void OnUpdate()
        {
            _testTex.texture = _dataSystem.InfluenceTexture;
            _dataSystem.InfluenceTexture.Apply();
            Entities.ForEach((RenderMesh mesh, ref GameArenaTag _) =>
            {
                mesh.material.SetTexture(Property, _dataSystem.InfluenceTexture);
            });
        }
    }
}