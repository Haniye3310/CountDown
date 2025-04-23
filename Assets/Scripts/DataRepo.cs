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
    public bool IsMainPlayer;
    public Animator PlayerAnimator;
    public Rigidbody PlayerRigidbody;
    public float Strength;
    [NonSerialized] public bool IsGrounded = true;
    [NonSerialized] public bool ShouldJump;
    [NonSerialized] public Vector3 PushForce;
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
    public List<Transform> PlatformsPosition;
    public Transform PlatformsParent;

}