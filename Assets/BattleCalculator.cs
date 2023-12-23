namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A class for calculating and determining battle outcomes
    /// </summary>
    public class BattleCalculator : MonoBehaviour
    {
        private static int CalculatePhysicalAttackPower(UnitManager attacker, UnitManager defender)
        {
            int attackStrength = attacker.UnitData.StrengthBaseValue + attacker.UnitData.EquippedWeapon.WeaponMight;
            int defenseStrength = defender.UnitData.DefenseBaseValue + CalculateTerrainDefenseBonus(defender);

            int attackPower = attackStrength - defenseStrength;

            return attackPower;
        }

        private static int CalculateMagicAttackPower(UnitManager attacker, UnitManager defender)
        {
            int attackStrength = attacker.UnitData.MagicBaseValue + attacker.UnitData.EquippedWeapon.WeaponMight;
            int defenseStrength = defender.UnitData.ResistanceBaseValue + CalculateTerrainDefenseBonus(defender);

            int attackPower = attackStrength - defenseStrength;

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

            return (unitA_WeaponType, unitB_WeaponType) switch
            {
                // Axe bonuses
                (WeaponType.Axe, WeaponType.Lance) => +5,
                (WeaponType.Axe, WeaponType.Sword) => -5,
                (WeaponType.Axe, WeaponType.Tome) => -5,

                // Lance bonuses
                (WeaponType.Lance, WeaponType.Axe) => -5,
                (WeaponType.Lance, WeaponType.Sword) => +5,
                (WeaponType.Lance, WeaponType.Tome) => +5,

                // Sword bonuses
                (WeaponType.Sword, WeaponType.Axe) => +5,
                (WeaponType.Sword, WeaponType.Lance) => -5,

                // Tome bonuses
                (WeaponType.Tome, WeaponType.Axe) => +5,
                (WeaponType.Tome, WeaponType.Lance) => -5,

                // Default for same weapon types
                _ => 0
            };
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
            var roll = UnityEngine.Random.Range(0, 100);

            return roll <= percentage;
        }

    }
}