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
[Serializable]
public class PlatformData
{
    public Platform platform;
    [NonSerialized] public bool IsOpen;
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
    [NonSerialized] public bool IsFrozen;
    [NonSerialized] public bool IsGrounded = true;
    [NonSerialized] public bool ShouldJump;
    [NonSerialized]public bool ShouldPunch;
    [NonSerialized] public Vector3 PushForce;
    [NonSerialized] public Vector3 TargetMovement;
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

}