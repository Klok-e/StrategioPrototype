using System;
using UnityEngine;

namespace Strategio.GameConfigs
{
    public struct UnitStats
    {
        public float moveSpeed;
        public float attack;
        public float requiredProgress;
        public float health;
    }

    public static class UnitTypeConverter
    {
        public static UnitStats GetStats(this UnitType unit)
        {
            Debug.Assert(unit != UnitType.Invalid);
            var x = new UnitStats();
            switch (unit)
            {
                case UnitType.Simple:
                    x = new UnitStats
                    {
                        attack = 1f,
                        health = 5f,
                        moveSpeed = 0.5f,
                        requiredProgress = 0.5f,
                    };
                    break;
                case UnitType.Spawner:
                    x = new UnitStats
                    {
                        attack = 1f,
                        health = 100f,
                        moveSpeed = 0.1f,
                        requiredProgress = 50f,
                    };
                    break;
            }
            return x;
        }
    }
}