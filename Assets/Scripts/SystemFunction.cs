using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DataRepo;

public class SystemFunction
{
    public static IEnumerator StartTimerOftheGame(DataRepo dataRepo)
    {
        dataRepo.GameData.RemainingTimeInGame = dataRepo.GameData.TimeOftheGame;
        while (dataRepo.GameData.RemainingTimeInGame > 0)
        {
            dataRepo.GameData.RemainingTimeInGame -= Time.deltaTime;

            dataRepo.UIData.RemainingTimeText.text = ((int)dataRepo.GameData.RemainingTimeInGame).ToString();
            yield return null;
        }
        dataRepo.GameData.ShouldStopGame = true;
        dataRepo.UIData.ResultPanel.gameObject.SetActive(true);
        dataRepo.UIData.UIPanel.gameObject.SetActive(false);
    }
    public static bool ShouldFinishGame(DataRepo dataRepo)
    {
        
        return false;
    }
    public static void CreateMap(DataRepo dataRepo)
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        //Creates the map randomly
        foreach (Transform t in dataRepo.GameData.PlatformsPosition)
        {
            int r = Random.Range(0, dataRepo.GameData.PlatformsPrefab.Count);
            while (dataRepo.GameData.PlatformsNumberAtFirst[r] == 0)
            {
                r++;
                if (r == dataRepo.GameData.PlatformsNumberAtFirst.Count)
                    r = 0;
            }
            Platform platform
                = GameObject.Instantiate
                (dataRepo.GameData.PlatformsPrefab[r],
                t.position,
                Quaternion.identity,
                dataRepo.GameData.PlatformsParent);
            platform.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            dataRepo.Platforms.Add(new PlatformData { platform = platform });
            dataRepo.GameData.PlatformsNumberAtFirst[r]--;
        }

        //increase number of 3second tiles if they are less than 3
        if (GetNumberOfTileInMap(3, dataRepo) < 2) 
        {
            foreach (PlatformData pl in dataRepo.Platforms)
            {
                if (pl.platform.SecondOfPrefab != 3)
                {
                    Vector3 pos = pl.platform.transform.position;
                    GameObject.DestroyImmediate(pl.platform.gameObject);
                    int r = 3;
                    Platform p
                        = GameObject.Instantiate
                        (dataRepo.GameData.PlatformsPrefab[r],
                        pos,
                        Quaternion.identity,
                        dataRepo.GameData.PlatformsParent);
                    p.transform.eulerAngles = new Vector3(-90f, 0f, 0f);

                    pl.platform = p;
                }
                if (GetNumberOfTileInMap(3, dataRepo) >= 2)
                    break;

            }
        }

    }
    public static int GetNumberOfTileInMap(int SecondOfTile,DataRepo dataRepo)
    {
        int n = 0;
        foreach (PlatformData pld in dataRepo.Platforms)
        {
            if (pld.platform.SecondOfPrefab == SecondOfTile)
            {
                n++;
            }
        }
        return n;
    }
    public static IEnumerator CountDown(DataRepo dataRepo)
    {
        while (true)
        {

            //Counts down the tiles
            for (int j = 0; j < dataRepo.Platforms.Count; j++)
            {
                for (int i = 0; i < dataRepo.GameData.PlatformsPrefab.Count; i++)
                {
                    if (dataRepo.GameData.PlatformsPrefab[i].SecondOfPrefab == dataRepo.Platforms[j].platform.SecondOfPrefab)
                    {
                        if (dataRepo.GameData.PlatformsPrefab[i].SecondOfPrefab >= 1)
                        {
                            Vector3 pos = dataRepo.Platforms[j].platform.transform.position;
                            GameObject.DestroyImmediate(dataRepo.Platforms[j].platform.gameObject);
                            foreach (Platform platform in dataRepo.GameData.PlatformsPrefab)
                            {
                                if (platform.SecondOfPrefab == dataRepo.GameData.PlatformsPrefab[i].SecondOfPrefab - 1)
                                {
                                    Platform p = GameObject.Instantiate(platform,
                                        pos,
                                        Quaternion.identity,
                                        dataRepo.GameData.PlatformsParent);

                                    p.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
                                    dataRepo.Platforms[j].platform = p;
                                }
                            }
                        }
                    }

                }
            }
            // Deletes the open platform and replaces them
            for (int j = 0; j < dataRepo.Platforms.Count; j++)
            {
                for (int i = 0; i < dataRepo.GameData.PlatformsPrefab.Count; i++)
                {
                    if (dataRepo.Platforms[j].IsOpen)
                    {
                        dataRepo.Platforms[j].IsOpen = false;
                        Vector3 pos = dataRepo.Platforms[j].platform.transform.position;
                        GameObject.DestroyImmediate(dataRepo.Platforms[j].platform.gameObject);
                        int r = Random.Range(2, dataRepo.GameData.PlatformsPrefab.Count);
                        while (GetNumberOfTileInMap(dataRepo.GameData.PlatformsPrefab[r].SecondOfPrefab, dataRepo) >= 3)
                        {
                            r++;
                            if (r == dataRepo.GameData.PlatformsPrefab.Count)
                                r = 2;

                        }
                        Platform p
                            = GameObject.Instantiate
                            (dataRepo.GameData.PlatformsPrefab[r],
                            pos,
                            Quaternion.identity,
                            dataRepo.GameData.PlatformsParent);
                        p.transform.eulerAngles = new Vector3(-90f, 0f, 0f);

                        dataRepo.Platforms[j].platform = p;
                    }
                }
            }


            //Opens the tiles less than 1
            for (int j = 0; j < dataRepo.Platforms.Count; j++)
            {
                for (int i = 0; i < dataRepo.GameData.PlatformsPrefab.Count; i++)
                {
                    if (dataRepo.GameData.PlatformsPrefab[i].SecondOfPrefab == dataRepo.Platforms[j].platform.SecondOfPrefab)
                    {
                        if (dataRepo.Platforms[j].platform.SecondOfPrefab < 1)
                        {
                            dataRepo.Platforms[j].IsOpen = true;
                            dataRepo.Platforms[j].platform.Animator.SetBool("Open", dataRepo.Platforms[j].IsOpen);

                        }
                    }

                }
            }

            float timeToWait = 0;
            if ((int)dataRepo.GameData.RemainingTimeInGame >= 20) timeToWait = 1.3f;

            if ((int)dataRepo.GameData.RemainingTimeInGame > 10
                && (int)dataRepo.GameData.RemainingTimeInGame < 20) timeToWait = 1f;

            if ((int)dataRepo.GameData.RemainingTimeInGame <= 10) timeToWait = 0.7f;
            yield return new WaitForSeconds(timeToWait);
        }

    }
    public static void OnPlayerCollisionStay(Player player, Collision collision, DataRepo dataRepo)
    {
        Player otherPlayer = null;
        if (collision.gameObject.tag == "Player")
        {
            otherPlayer = collision.gameObject.GetComponent<Player>();
        }
        PlayerData otherPlayerData = null;
        PlayerData playerData = null;
        foreach (PlayerData p in dataRepo.Players)
        {
            if (p.Player == otherPlayer)
            {
                otherPlayerData = p;
            }
            if (p.Player == player)
            {
                playerData = p;
            }
        }
        if (otherPlayer != null && playerData.IsGrounded && otherPlayerData.IsGrounded)
        {
            Vector3 pushDirection = (otherPlayer.transform.position - player.transform.position).normalized;
            float strengthDifference = playerData.Strength - otherPlayerData.Strength;

            if (strengthDifference > 0) // This character is stronger
            {
                ApplyPush(pushDirection, strengthDifference, otherPlayerData);
            }
            else if (strengthDifference < 0) // The other character is stronger
            {
                ApplyPush(-pushDirection, Mathf.Abs(strengthDifference), playerData);
            }
        }
        if (collision.gameObject.tag == "Ground")
        {
            playerData.IsGrounded = true;
        }
    }
    public static void OnPlayerCollisionExit(Player player, Collision collision, DataRepo dataRepo)
    {

        if (collision.gameObject.tag == "Ground")
        {
            foreach (PlayerData p in dataRepo.Players)
            {
                if (p.Player == player)
                {
                    p.IsGrounded = false;
                }
            }
        }
    }
    public static void OnPlayerCollisionEnter(MonoBehaviour mono, Player player, Collision collision, DataRepo dataRepo)
    {
       
        PlayerData playerData = null;
        foreach (PlayerData p in dataRepo.Players)
        {
            if (p.Player == player)
            {
                playerData = p;
                
            }
        }
        if (collision.gameObject.CompareTag("OutOfGround"))
        {
            playerData.IsOutfGround = true;
            Vector3 bounceDir = Vector3.up;
            playerData.PlayerRigidbody.AddForce(bounceDir * 2f, ForceMode.Impulse);
        }
        if (collision.gameObject.tag == "Ground")
        {
            playerData.IsGrounded = true;
        }
    }
    public static void Move(DataRepo dataRepo, PlayerData playerData, Vector3 direction)
    {
        if (playerData.IsOutfGround)return;
        direction = direction.normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            playerData.PlayerRigidbody.AddForce
                                (direction * 85, ForceMode.Force);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            playerData.PlayerRigidbody.
                MoveRotation(Quaternion.Slerp(playerData.PlayerRigidbody.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
        if (direction.magnitude < 0.1f)
        {
            playerData.PlayerAnimator.SetFloat("MoveSpeed", 0);
        }
        else
        {
            playerData.PlayerAnimator.SetFloat("MoveSpeed", 1);
        }
    }
    public static void FixedUpdate(MonoBehaviour mono,DataRepo dataRepo)
    {
        float v = dataRepo.UIData.Joystick.Vertical;
        float h = dataRepo.UIData.Joystick.Horizontal;

        Vector3 direction = new Vector3(h, 0, v) * -1;
        foreach (PlayerData p in dataRepo.Players)
        {
            if (p.IsMainPlayer)
            {
                Move(dataRepo, p, direction);
            }
            if (!p.IsMainPlayer)
                Move(dataRepo, p, (p.TargetMovement - p.Player.transform.position));
            if (p.ShouldJump)
            {
                Jump(dataRepo, p);
            }
            if (p.ShouldPunch)
            {
                AttemptPunch(mono,dataRepo, p);
                p.ShouldPunch = false;
            }
            if(p.IsOutfGround)
                p.PlayerRigidbody.AddForce(p.Player.transform.forward * 60, ForceMode.Force);
            p.PlayerAnimator.SetBool("Grounded", p.IsGrounded);
        }
        foreach (PlayerData p in dataRepo.Players)
        {
            if (p.PushForce.magnitude > 0.01f) // Apply force smoothly
            {
                p.PlayerRigidbody.AddForce(p.PushForce, ForceMode.Acceleration);
                p.PushForce *= 0.9f; // Gradually reduce the push force to prevent infinite sliding
            }

            else
            {
                p.PushForce = Vector3.zero;
            }
        }
        if (dataRepo.GameData.ShouldStopGame)
        {
            foreach (PlayerData p in dataRepo.Players)
            {
                Move(dataRepo, p, Vector3.zero);
            }
        }
    }
    public static IEnumerator FreezePlayer(PlayerData playerData)
    {

        playerData.IsFrozen = true;
        float EndTimer = Time.time + 3;
        float interval = Time.time + 0.1f;

        while (true)
        {
            if (Time.time > EndTimer)
            {
                playerData.Visual.gameObject.SetActive(false);
                break;
            }
            if (Time.time > interval)
            {
                playerData.Visual.gameObject.SetActive(!playerData.Visual.gameObject.activeSelf);
                interval = Time.time + 0.1f;
            }
            yield return null;
        }
        playerData.Visual.gameObject.SetActive(true);
        playerData.IsFrozen = false;
    }
    public static void AttemptPunch(MonoBehaviour mono,DataRepo dataRepo, PlayerData playerData)
    {
        Vector3 punchOrigin = playerData.Player.transform.position;
        Vector3 punchDirection = playerData.Player.transform.forward;

        float punchRange = 0.5f;
        float punchRadius = 0.5f;
        float maxAngle = 180f; // only punch if within 45 degrees in front
        playerData.PlayerAnimator.SetTrigger("Attack");
        RaycastHit[] hits = Physics.SphereCastAll(punchOrigin, punchRadius, punchDirection, punchRange);

        foreach (RaycastHit hit in hits)
        {
            Player enemyPlayer = hit.collider.GetComponent<Player>();
            if (enemyPlayer != null && enemyPlayer != playerData.Player)
            {
                PlayerData enemyData = dataRepo.Players.Find(p => p.Player == enemyPlayer);
                if (enemyData != null)
                {
                    Vector3 toEnemy = (enemyPlayer.transform.position - punchOrigin).normalized;
                    float angle = Vector3.Angle(punchDirection, toEnemy);

                    if (angle <= maxAngle) // Enemy is within punch cone
                    {
                        if (enemyData.IsFrozen) return;
                        Vector3 pushDir = toEnemy;
                        ApplyPush(pushDir, 50f, enemyData);
                        enemyData.PlayerAnimator.SetTrigger("GetHit");
                        mono.StartCoroutine(FreezePlayer(enemyData));
                    }
                }
            }
        }


    }

    public static void OnPunchClicked(DataRepo dataRepo, PlayerData playerData)
    {
        playerData.ShouldPunch = true;
    }


    public static void Jump(DataRepo dataRepo, PlayerData playerData)
    {

        if (playerData.IsGrounded)
        {
            playerData.PlayerRigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
            playerData.ShouldJump = false;

        }
    }
    public static void OnJumpClicked(DataRepo dataRepo, PlayerData playerData)
    {
        playerData.ShouldJump = true;
    }
    public static void ApplyPush(Vector3 pushDirection, float forceAmount, PlayerData playerData)
    {
        playerData.PushForce = pushDirection * forceAmount * 5;
    }
    public void OnRestartClicked()
    {
        SceneManager.LoadScene("FinalSceneWithBothArtAndLogic");
    }
    public void OnHomeClicked()
    {
        Application.OpenURL("https://tobi.gg");
    }
    public static IEnumerator StartRobot(PlayerData playerData, DataRepo dataRepo)
    {
        while (true)
        {
            if (playerData.TargetMovement == Vector3.zero)
            {
                playerData.TargetMovement = GetRandomPositionOnGround(dataRepo, playerData);

            }
            if
            (
            Vector3.Distance
                            (
                                playerData.TargetMovement,
                                new Vector3
                                        (
                                        playerData.Player.transform.position.x,
                                        dataRepo.GameData.GroundTrigger.position.y + (dataRepo.GameData.GroundTrigger.localScale.y),
                                        playerData.Player.transform.position.z
                                        )
                            )
            < 0.3f
            )
            {
                playerData.TargetMovement = Vector3.zero;

            }
            yield return null;
        }
    }
    public static Vector3 GetRandomPositionOnGround(DataRepo dataRepo, PlayerData playerData)
    {
        // Calculate the effective radius of the ground
        float radius = dataRepo.GameData.GroundTrigger.localScale.x * 0.3f;

        // Generate a random angle in radians
        float angle = Random.Range(0f, Mathf.PI * 2);

        // Calculate x and z coordinates relative to the center of the ground
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Adjust the position to be relative to the ground's actual position
        float adjustedY = dataRepo.GameData.GroundTrigger.position.y + (dataRepo.GameData.GroundTrigger.localScale.y); // Top surface of the ground

        return new Vector3(
            x + dataRepo.GameData.GroundTrigger.position.x, // Adjust x based on the ground's center position
            adjustedY, // Set Y to the ground's top surface
            z + dataRepo.GameData.GroundTrigger.position.z  // Adjust z based on the ground's center position
        );
    }
}
