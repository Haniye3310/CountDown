using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataRepo : MonoBehaviour
{
    public List<PlayerData> Players;
    public UIData UIData;
    public GameData GameData;
    [NonSerialized]public List<PlatformData> Platforms = new List<PlatformData>();

}
public enum BotActionType { Move, Attack, Dodge, Idle }

public class BotAction
{
    public BotActionType ActionType;
    public float Score;
    public Vector3 TargetPosition;
    public PlayerData TargetEnemy;
}

[Serializable]
public class PlatformData
{
    public Platform platform;
    [NonSerialized] public bool IsOpen;
    [NonSerialized] public bool IsInAnimatorOpen;
}
[Serializable]
public class PlayerData
{
    public Player Player;
    public GameObject Visual;
    public bool IsMainPlayer;
    public Animator PlayerAnimator;
    public Rigidbody PlayerRigidbody;
    public float Strength;
    public Text DebugText;
    public ParticleSystem JumpVFX;
    public ParticleSystem PunchVFX;
    [NonSerialized]public bool IsPlayerFalling;
    [NonSerialized] public bool HasBeenRemoved;
    [NonSerialized] public int Rank;
    [NonSerialized] public bool IsFrozen;
    [NonSerialized] public bool PauseMovement = false;
    [NonSerialized] public bool IsGrounded = true;
    [NonSerialized] public bool ShouldJump;
    [NonSerialized] public bool ShouldJumpOnCharacter;
    [NonSerialized]public bool ShouldPunch;
    [NonSerialized] public Vector3 PushForce;
    [NonSerialized] public Vector3 TargetMovement;
    [NonSerialized] public bool IsOutfGround = false;
    [NonSerialized] public Platform CurrentPlatform;
}
[Serializable]
public class UIData
{
    public FixedJoystick Joystick;
    public TextMeshProUGUI RemainingTimeText;
    public GameObject UIPanel;
    public GameObject ResultPanel;
    public Image StartCountDownTimer;
    public Image GoImage;
    public Sprite NumberThreeSprite;
    public Sprite NumberTwoSprite;
    public Sprite NumberOneSprite;
    public Sprite FirstPlayerSprite;
    public Sprite SecondPlayerSprite;
    public Sprite ThirdPlayerSprite;
    public Sprite ForthPlayerSprite;
    public Image ResultBG;
}
[Serializable]
public class GameData
{
    [NonSerialized] public bool ShouldStopGame = false;
    [NonSerialized] public float RemainingTimeInGame;
    [NonSerialized] public int TimeOftheGame = 30;

    public List<Platform> PlatformsPrefab;
    public List<int> PlatformsNumberAtFirst;
    public List<Transform> PlatformsPosition;
    public Transform PlatformsParent;
    public Transform GroundTrigger;
    [NonSerialized] public float GroundRadius;

}