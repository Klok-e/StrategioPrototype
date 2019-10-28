using Strategio.Components.Ui;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = System.Diagnostics.Debug;

namespace Strategio.Ui
{
    public class UiManager : MonoBehaviour
    {
        private void Update()
        {
            bool leftMouse = Input.GetMouseButtonDown(0);
            bool rightMouse = Input.GetMouseButtonDown(1);
            float3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            if (leftMouse)
            {
                var manager = World.Active.EntityManager;
                var ent = manager.CreateEntity();
                manager.AddComponentData(ent, new ClickEventComponent
                {
                    button = MouseButton.LeftMouse,
                    pos = mousePos.xy,
                });
            }
            if (rightMouse)
            {
                var manager = World.Active.EntityManager;
                var ent = manager.CreateEntity();
                manager.AddComponentData(ent, new ClickEventComponent
                {
                    button = MouseButton.RightMouse,
                    pos = mousePos.xy,
                });
            }
        }
    }
}