using UnityEngine;

namespace CT6GAMAI
{
    /// <summary>
    /// Contains constants and enums used throughout the game.
    /// </summary>
    public static class Constants
    {
        #region Game Flow

        public enum GameStates { Map, Battle }

        public enum CameraStates { Map, Battle }

        public enum BattleSequenceStates
        {
            PreBattle,
            PlayerMoveForward,
            PlayerAttack,
            PlayerMoveBack,
            EnemyMoveForward,
            EnemyAttack,
            EnemyMoveBack,
            CheckAdditionalAttacks,
            BattleEnd
        }

        #endregion // Game Flow

        #region Battle Calculator Values

        /// <summary>
        /// A value representing the speed advantage required to do a double attack.
        /// </summary>
        public static int DOUBLE_ATK_SPEED_THRESHOLD = 5;

        /// <summary>
        /// A value representing what to multiply attack by when calculating double attack damage.
        /// </summary>
        public static int DOUBLE_ATK_MULTIPLIER = 2;

        /// <summary>
        /// A value representing the 'Skill' divider when skill is added to critical strength for when critical rate is calculated.
        /// </summary>
        public static int CRIT_SKILL_DIVIDER = 2;

        /// <summary>
        /// A value representing the 'Speed' multiplier when speed is added to avoid rate when calculated.
        /// </summary>
        public static int AVOID_SPEED_MULTIPLIER = 2;

        #endregion // Battle Calculator Values

        #region Nodes
        /// <summary>
        /// Enumerations representing the different visual states of a node.
        /// </summary>
        public enum NodeVisualState { Default, HoveredBlue, HoveredRed, HoveredGreen, SelectedBlue, SelectedRed, SelectedGreen, AllEnemyRange, SingularEnemyRange, PointOfInterest }

        /// <summary>
        /// Enumerations representing the different color states of a node.
        /// </summary>
        public enum NodeVisualColorState { Blue, Red, Green }

        /// <summary>
        /// Enumerations representing the different enemy color states of a node.
        /// </summary>
        public enum NodeVisualEnemyColorState { SingularEnemy, AllEnemy }

        /// <summary>
        /// Enumerations representing the different states of a cursor.
        /// </summary>
        public enum NodeCursorState { NoSelection, DefaultSelected, PlayerSelected, EnemySelected }

        /// <summary>
        /// Enumerations representing different directions.
        /// </summary>
        public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }

        /// <summary>
        /// Enumerations representing different terrain types.
        /// </summary>
        public enum Terrain { Plain, Forest, River, Fort }

        /// <summary>
        /// Enumerations representing the current state of the grid cursor in regards to unit actions.
        /// </summary>
        public enum CurrentState { Idle, Moving, ActionSelected, ConfirmingMove };

        /// <summary>
        /// Constant string for the tag string used on nodes.
        /// </summary>
        public const string NODE_TAG_REFERENCE = "Node";

        /// <summary>
        /// A cost value to set on nodes which should be unpassable.
        /// Reason being is that using int.MaxValue leads to int overflow.
        /// </summary>
        public const int MAX_NODE_COST = 99999;

        /// <summary>
        /// The default Y position of the cursor's world space canvas.
        /// </summary>
        public const int CURSOR_WSC_DEFAULT_Y_POS = 15;
        #endregion //Nodes

        #region Units
        /// <summary>
        /// Enumerations representing different unit classes.
        /// </summary>
        public enum Class { Knight, Druid, Barbarian, Archer, Mage };

        /// <summary>
        /// Which team a unit is on.
        /// </summary>
        public enum Team { Player, Enemy };

        /// <summary>
        /// What side a team is on in the battle map
        /// </summary>
        public enum Side { Left, Right };

        public enum UnitHealthState { Alive, Dead }

        /// <summary>
        /// The different types of weapons
        /// </summary>
        public enum WeaponType { Sword, Lance, Axe, Tome }

        /// <summary>
        /// The main weapon a unit has equipped.
        /// </summary>
        public enum Weapons { BronzeSword, SilverSword, WoodSpear, MetalSpear, Fire, Ice };

        /// <summary>
        /// The player team colour (FOR UI)
        /// </summary>
        public static Color32 UI_PlayerColour = new Color32(93, 149, 185, 255);

        /// <summary>
        /// The enemy team colour (FOR UI)
        /// </summary>
        public static Color32 UI_EnemyColour = new Color32(185, 94, 93, 255);

        /// <summary>
        /// The ally team colour (FOR UI)
        /// </summary>
        public static Color32 UI_AllyColour = new Color32(132, 185, 93, 255);

        /// <summary>
        /// Constant string for the tag string used on units.
        /// </summary>
        public const string UNIT_TAG_REFERENCE = "Unit";

        /// <summary>
        /// A value representing the amount of time a unit waits between moving tiles
        /// </summary>
        public const float MOVEMENT_DELAY = 0.3f;

        /// <summary>
        /// A value representing the amount of time the coroutine waits after cancelling a move.
        /// </summary>
        public const float MOVEMENT_DELAY_CANCEL = 1f;

        /// <summary>
        /// A value representing the speed a unit moves from one tile to the next
        /// </summary>
        public const float MOVEMENT_SPEED = 0.2f;

        /// <summary>
        /// A value representing the speed a unit rotates toward their look rotation
        /// </summary>
        public const float LOOK_ROTATION_SPEED = 0.1f;

        /// <summary>
        /// The Y value that the unit should go to when they're walking on river tiles
        /// </summary>
        public const float UNIT_Y_VALUE_RIVER = -0.2f;

        /// <summary>
        /// The Y value that the unit should go to when they're walking on land tiles
        /// </summary>
        public const float UNIT_Y_VALUE_LAND = 0.1f;

        /// <summary>
        /// The speed that the unit should adjust their Y value
        /// </summary>
        public const float UNIT_Y_ADJUSTMENT_SPEED = 0.1f;
        #endregion //Units                           

        #region Animation Values

        public const float BATTLE_SEQUENCE_ANIM_DELAY = 0.5f;

        public const float BATTLE_SEQUENCE_MOVEMENT_SPEED = 0.2f;

        #endregion // Animation Values

        #region Animation Parameter Strings

        /// <summary>
        /// The string value for the moving parameter in the animator
        /// </summary>
        public const string MOVING_ANIM_PARAM = "Moving";

        /// <summary>
        /// The string value for the ready parameter in the animator
        /// </summary>
        public const string READY_ANIM_PARAM = "Ready";

        public const string ATTACKING_ANIM_PARAM = "Attack";

        public const string ATTACKING_ANIM_IDX_PARAM = "AttackingAnimIndex";

        public const string HIT_ANIM_PARAM = "Hit";

        public const string HIT_ANIM_IDX_PARAM = "HitAnimIndex";

        public const string DODGE_ANIM_PARAM = "Dodge";

        public const string DEAD_ANIM_PARAM = "Dead";

        public const string DEAD_ANIM_IDX_PARAM = "DeadAnimIndex";

        #endregion // Animation Parameter Strings

        #region User Interface

        /// <summary>
        /// The left X position to move the left battle forecast to when disabled.
        /// </summary>
        public const float BATTLE_FORECAST_LEFT_X_POS_TO = -400;

        /// <summary>
        /// The right x position to move the right battle forecast to when disabled.
        /// </summary>
        public const float BATTLE_FORECAST_RIGHT_X_POS_TO = 400;

        /// <summary>
        /// The speed that the cursor pointer on the actions list should point
        /// </summary>
        public const float POINTER_X_YOYO_SPEED = 0.5f;

        /// <summary>
        /// The amount of time to wait between cancelling a move and moving state to idle.
        /// </summary>
        public const float IDLE_DELAY = 0.5f;

        /// <summary>
        /// The speed that it takes for the vignette for UI to fade in/out
        /// </summary>
        public const float VIGNETTE_FADE_SPEED = 0.5f;

        /// <summary>
        /// The number of priority for the active camera
        /// </summary>
        public const int ACTIVE_CAMERA_PRIORITY = 10;

        /// <summary>
        /// The number of priority for the inactive camera
        /// </summary>
        public const int INACTIVE_CAMERA_PRIORITY = 0;

        #endregion // User Interface
    }
}