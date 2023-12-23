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

        private static int CalculateWeaponTriangeFactor(UnitData attacker, UnitData defender)
        {
            var unitA_WeaponType = attacker.EquippedWeapon.WeaponType;
            var unitB_WeaponType = defender.EquippedWeapon.WeaponType;

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

        public static bool CanDoubleAttack(UnitManager attacker, UnitManager defender)
        {
            return (defender.UnitData.SpeedBaseValue + DOUBLE_ATK_SPEED_THRESHOLD) <= attacker.UnitData.SpeedBaseValue;
        }

        public static int CalculateAttackPower(UnitManager attacker, UnitManager defender)
        {
            var weaponType = attacker.UnitData.EquippedWeapon.WeaponType;

            if (weaponType == WeaponType.Tome)
            {
                return CalculateMagicAttackPower(attacker, defender);
            }

            return CalculatePhysicalAttackPower(attacker, defender);
        }

        public static int CalculateCriticalRatePercentage(UnitManager attacker, UnitManager defender)
        {
            int criticalStrength = attacker.UnitData.CriticalRateBaseValue + (attacker.UnitData.SkillBaseValue / CRIT_SKILL_DIVIDER);
            int defenseStrength = defender.UnitData.LuckBaseValue;

            int critRate = criticalStrength - defenseStrength;
            critRate = Mathf.Clamp(critRate, 0, 100);

            return critRate;
        }

        public static int CalculateHitRatePercentage(UnitManager attacker, UnitManager defender)
        {
            int weaponTriangleFactor = CalculateWeaponTriangeFactor(attacker.UnitData, defender.UnitData);
            int hitStrength = attacker.UnitData.HitRateBaseValue + weaponTriangleFactor;
            int defenseStrength = defender.UnitData.AvoidRateBaseValue;

            int hitRate = hitStrength - defenseStrength;
            hitRate = Mathf.Clamp(hitRate, 0, 100);

            return hitRate;
        }

        public static int CalculateAvoidRate(UnitData unit)
        {
            return (unit.SpeedBaseValue * AVOID_SPEED_MULTIPLIER) + unit.LuckBaseValue;
        }

        public static int CalculateRemainingHPForecast(UnitManager unit, int attackAmount, bool canDoubleAttack)
        {
            int attackValue = canDoubleAttack ? attackAmount * DOUBLE_ATK_MULTIPLIER : attackAmount;

            return unit.UnitStatsManager.HealthPoints - attackValue;
        }

        public static bool CriticalRoll(int criticalPercentage)
        {
            bool critHit = Roll(criticalPercentage);

            return critHit;
        }

        public static bool HitRoll(int hitPercentage)
        {
            bool hit = Roll(hitPercentage);

            return hit;
        }

        public static bool Roll(int percentage)
        {
            int roll = Random.Range(0, 100);

            return roll <= percentage;
        }
    }
}