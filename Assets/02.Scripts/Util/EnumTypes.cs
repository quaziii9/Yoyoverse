using UnityEngine;

namespace EnumTypes
{
    public enum EnemyState { Idle, Patrol, Move, Trace, Attack, Die , AssassinationDie }
    
    public enum EnemyType { Sniper, Paladin }

    public enum Layers
    {
        Default,
        TransparentFX,
        IgnoreRaycast,
        Reserved1,
        Water,
        UI,
        Reserved2,
        Reserved3,
        Player,
        Enemy,
    }

    public enum PlayerEvents
    {
        PlayerDead,
        OnAttackEffect,
        Clear
    }

    public enum YoYoEvents
    {
        YoYoAttached,
    }

    public enum UIEvents
    {
        OnClickDiskListItem,
        OnClickWireListItem,
        StartDraggingSkillIcon,
        StopDraggingSkillIcon,
        UpdateSkillDescription,
    }

    public enum GameEvents
    {
        SelectedDisk,
        SelectedWire,
        IsEquipReady,
        IsSkillReady,
        StartGame,
        GameOver,
    }
    
    public enum EnemyEvents
    {
        PlayerDetected,
        PlayerLost
    }

    public enum DataEvents
    {
       
    }

    public class EnumTypes : MonoBehaviour { }
}