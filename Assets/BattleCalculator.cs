namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A class for calculating and determining battle outcomes
    /// </summary>
    public class BattleCalculator : MonoBehaviour
    {
        private static int CalculatePhysicalAttackPower(UnitManager unitA, UnitManager unitB)
        {
            int attackPower = (unitA.UnitData.StrengthBaseValue + unitA.UnitData.EquippedWeapon.WeaponMight) - (unitB.UnitData.DefenseBaseValue + CalculateTerrainDefenseBonus(unitB));

            if (CanDoubleAttack(unitA, unitB))
            {
                Debug.Log("[BATTLE]: Unit can double attack!");
                //attackPower *= 2;
            }

            return attackPower;
        }

        private static int CalculateMagicAttackPower(UnitManager unitA, UnitManager unitB)
        {
            int attackPower = (unitA.UnitData.MagicBaseValue + unitA.UnitData.EquippedWeapon.WeaponMight) - (unitB.UnitData.ResistanceBaseValue + CalculateTerrainDefenseBonus(unitB));

            if (CanDoubleAttack(unitA, unitB))
            {
                Debug.Log("[BATTLE]: Unit can double attack!");
                //attackPower *= 2;
            }

            return attackPower;
        }

        private static int CalculateTerrainDefenseBonus(UnitManager unit)
        {
            return unit.StoodNode.NodeData.TerrainType.DefenseBoost;
        }

        private static int CalculateWeaponTriangeFactor(UnitData unitA, UnitData unitB)
        {
            var unitA_WeaponType = unitA.EquippedWeapon.WeaponType;
            var unitB_WeaponType = unitB.EquippedWeapon.WeaponType;

            if (unitA_WeaponType == WeaponType.Axe)
            {
                if (unitB_WeaponType == WeaponType.Axe)
                {
                    // Axe vs Axe. Hit rate 0
                    return 0;
                }
                if (unitB_WeaponType == WeaponType.Lance)
                {
                    // Axe beats Lance. Hit rate +5
                    return +5;
                }
                if (unitB_WeaponType == WeaponType.Sword)
                {
                    // Axe loses against Sword. Hit rate -5
                    return -5;
                }
                if (unitB_WeaponType == WeaponType.Tome)
                {
                    // Axe loses against Tome. Hit rate -5
                    return -5;
                }
            }

            if (unitA_WeaponType == WeaponType.Lance)
            {
                if (unitB_WeaponType == WeaponType.Axe)
                {
                    // Lance loses against Axe. Hit rate -5
                    return -5;
                }
                if (unitB_WeaponType == WeaponType.Lance)
                {
                    // Lance vs Lance. Hit rate 0
                    return 0;
                }
                if (unitB_WeaponType == WeaponType.Sword)
                {
                    // Lance beats Sword. Hit rate +5
                    return 5;
                }
                if (unitB_WeaponType == WeaponType.Tome)
                {
                    // Lance beats Tome. Hit rate +5
                    return 5;
                }
            }

            if (unitA_WeaponType == WeaponType.Sword)
            {
                if (unitB_WeaponType == WeaponType.Axe)
                {
                    // Sword beats Axe. Hit rate +5
                    return 5;
                }
                if (unitB_WeaponType == WeaponType.Lance)
                {
                    // Sword loses against Lance. Hit rate -5
                    return -5;
                }
                if (unitB_WeaponType == WeaponType.Sword)
                {
                    // Sword vs Sword. Hit rate 0
                    return 0;
                }
                if (unitB_WeaponType == WeaponType.Tome)
                {
                    // Sword vs Tome. Hit rate 0
                    return 0;
                }
            }

            if (unitA_WeaponType == WeaponType.Tome)
            {
                if (unitB_WeaponType == WeaponType.Axe)
                {
                    // Tome beats Axe. Hit rate +5
                    return 5;
                }
                if (unitB_WeaponType == WeaponType.Lance)
                {
                    // Tome loses against Lance. Hit rate -5
                    return -5;
                }
                if (unitB_WeaponType == WeaponType.Sword)
                {
                    // Tome vs Sword. Hit rate 0
                    return 0;
                }
                if (unitB_WeaponType == WeaponType.Tome)
                {
                    // Tome vs Tome. Hit rate 0
                    return 0;
                }
            }

            return 0;
        }

        public static bool CanDoubleAttack(UnitManager unitA, UnitManager unitB)
        {
            return (unitB.UnitData.SpeedBaseValue + 5) <= unitA.UnitData.SpeedBaseValue;
        }

        public static int CalculateAttackPower(UnitManager unitA, UnitManager unitB)
        {
            var weaponType = unitA.UnitData.EquippedWeapon.WeaponType;

            if (weaponType == WeaponType.Tome)
            {
                return CalculateMagicAttackPower(unitA, unitB);
            }
            else
            {
                return CalculatePhysicalAttackPower(unitA, unitB);
            }
        }

        public static int CalculateCriticalRatePercentage(UnitManager unitA, UnitManager unitB)
        {           
            var critRate = (unitA.UnitData.CriticalRateBaseValue + (unitA.UnitData.SkillBaseValue / 2)) - unitB.UnitData.LuckBaseValue;

            if (critRate < 0)
            {
                critRate = 0;
            }

            return critRate;
        }

        public static int CalculateHitRatePercentage(UnitManager unitA, UnitManager unitB)
        {
            int weaponTriangleFactor = CalculateWeaponTriangeFactor(unitA.UnitData, unitB.UnitData);
            int hitRate = (unitA.UnitData.HitRateBaseValue + weaponTriangleFactor) - unitB.UnitData.AvoidRateBaseValue;

            if (hitRate > 100)
            {
                hitRate = 100;
            }

            return hitRate;
        }

        public static int CalculateAvoidRate(UnitData unit)
        {
            return (unit.SpeedBaseValue * 2) + unit.LuckBaseValue;
        }

        public static int CalculateRemainingHPForecast(UnitManager unit, int attackAmount, bool canDoubleAttack)
        {
            var attackValue = attackAmount;

            if (canDoubleAttack)
            {
                attackValue *= 2;
            }

            return unit.UnitStatsManager.HealthPoints - attackValue;
        }

        public static bool CriticalRoll(int criticalPercentage)
        {
            bool critHit = Roll(criticalPercentage);

            if (critHit)
            {
                Debug.Log("[BATTLE]: Got crit hit!");
            }

            return critHit;
        }

        public static bool HitRoll(int hitPercentage)
        {
            bool hit = Roll(hitPercentage);

            if (hit)
            {
                Debug.Log("[BATTLE]: Got hit!");
            }

            return hit;
        }

        public static bool Roll(int percentage)
        {
            var roll = Random.Range(0, 100);

            return roll <= percentage;
        }

    }
}