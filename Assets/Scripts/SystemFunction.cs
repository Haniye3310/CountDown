using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SystemFunction
{
    public static IEnumerator StartTimerOftheGame(DataRepo dataRepo)
    {
        dataRepo.GameData.RemainingTimeInGame = dataRepo.GameData.TimeOftheGame;
        while (dataRepo.GameData.RemainingTimeInGame > 0)
        {
            dataRepo.GameData.RemainingTimeInGame -= Time.deltaTime;

            dataRepo.UIData.RemainingTimeText.text = ((int)dataRepo.GameData.RemainingTimeInGame).ToString();
            if (ShouldFinishGame(dataRepo))
                break;

            yield return null;
        }
        dataRepo.GameData.ShouldStopGame = true;
        foreach(PlayerData p in dataRepo.Players)
        {
            if (p.IsMainPlayer)
            {
                if(p.Rank == 1){ dataRepo.UIData.ResultBG.sprite = dataRepo.UIData.FirstPlayerSprite; }
                if(p.Rank == 2){ dataRepo.UIData.ResultBG.sprite = dataRepo.UIData.SecondPlayerSprite; }
                if(p.Rank == 3){ dataRepo.UIData.ResultBG.sprite = dataRepo.UIData.ThirdPlayerSprite; }
                if(p.Rank == 4){ dataRepo.UIData.ResultBG.sprite = dataRepo.UIData.ForthPlayerSprite; }
            }
        }
        dataRepo.UIData.ResultPanel.gameObject.SetActive(true);
        dataRepo.UIData.UIPanel.gameObject.SetActive(false);
    }
    public static bool ShouldFinishGame(DataRepo dataRepo)
    {
        int NumberOfEleminited = 0;
        foreach(PlayerData playerData in dataRepo.Players)
        {
            if (playerData.IsOutfGround||
                playerData.Player.transform.position.y <dataRepo.GameData.GroundTrigger.position.y)
            {
                NumberOfEleminited++;
            }
        }
        if(NumberOfEleminited == 4)
        {
            return true;
        }
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
                        dataRepo.Platforms[j].platform.Animator.SetBool("Substitute", true);
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
                            dataRepo.Platforms[j].platform.Animator.SetBool("Open", true);

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
    public static void OnAnimatorOpenEvent(DataRepo dataRepo,Platform platform)
    {
        foreach(PlatformData p in dataRepo.Platforms)
        {
            if(p.platform == platform)
            {
                p.IsInAnimatorOpen = true;
            }
        }
    }
    public static void OnAnimatorCloseEvent(DataRepo dataRepo, Platform platform)
    {
        foreach (PlatformData p in dataRepo.Platforms)
        {
            if (p.platform == platform)
            {
                p.IsInAnimatorOpen = false;
            }
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
            pushDirection.y = 0;

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
            foreach (PlatformData platformData in dataRepo.Platforms)
            {
                if (platformData.platform == playerData.CurrentPlatform)
                {
                    if (platformData.IsInAnimatorOpen)
                    {
                        playerData.IsPlayerFalling = true;
                        Vector3 dir = (platformData.platform.transform.position - playerData.Player.transform.position).normalized;
                        dir.y = 0;
                        playerData.PlayerRigidbody
                            .AddForce(dir * 5, ForceMode.Impulse);
                    }
                }
            }
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
    public static void OnFootTriggerEnter(DataRepo dataRepo, Player player, Collider other)
    {
        PlayerData playerData = null;
        foreach (PlayerData p in dataRepo.Players)
        {
            if (p.Player == player)
            {
                playerData = p;

            }
        }
        Player otherPlayer = other.gameObject.GetComponent<Player>();
        if (other.gameObject.tag == "Player")
        {
            if (otherPlayer != player)
                playerData.ShouldJumpOnCharacter = true;
        }
    }

    public static void OnFootTriggerExit(DataRepo dataRepo, Player player, Collider other)
    {
        PlayerData playerData = null;
        foreach (PlayerData p in dataRepo.Players)
        {
            if (p.Player == player)
            {
                playerData = p;

            }
        }
        Player otherPlayer = other.gameObject.GetComponent<Player>();
        if (other.gameObject.tag == "Player")
        {
            if (otherPlayer != player)
                playerData.ShouldJumpOnCharacter = false;
        }
    }

    public static void OnPlayerTriggerEnter(Player player, Collider collider,DataRepo dataRepo)
    {
        PlayerData playerData = null;
        foreach (PlayerData p in dataRepo.Players)
        {
            if (p.Player == player)
            {
                playerData = p;

            }
        }
        if (collider.gameObject.tag == "GroundTrigger")
        {
            playerData.JumpVFX.Play();
            Ray ray = new Ray(playerData.Player.transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                playerData.JumpVFX.transform.position = hit.point + Vector3.up * 0.01f; // Slightly above the ground
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
            foreach (PlatformData platformData in dataRepo.Platforms)
            {
                if (platformData.platform == playerData.CurrentPlatform)
                {
                    if (platformData.IsInAnimatorOpen)
                    {
                        playerData.IsPlayerFalling = true;
                        Vector3 dir = (platformData.platform.transform.position - playerData.Player.transform.position).normalized;
                        dir.y = 0;
                        playerData.PlayerRigidbody
                            .AddForce(dir * 5, ForceMode.Impulse);
                    }
                }
            }
        }

    }
    public static void Move(DataRepo dataRepo, PlayerData playerData, Vector3 direction)
    {
        if (playerData.IsOutfGround && !playerData.IsPlayerFalling) return;
        direction = direction.normalized;
        direction.y = 0;

        if (Mathf.Approximately(0, direction.magnitude))
        {
            playerData.SpeedMultiplier -= Time.deltaTime * 7000;
        }
        else
        {
            playerData.MoveDirection = direction;
            playerData.SpeedMultiplier += Time.deltaTime * 7000;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            playerData.PlayerRigidbody.
                MoveRotation(Quaternion.Slerp(playerData.PlayerRigidbody.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
        playerData.SpeedMultiplier = Mathf.Clamp(playerData.SpeedMultiplier, 0, 6000);
        playerData.PlayerRigidbody.AddForce
                    (playerData.MoveDirection * playerData.SpeedMultiplier * Time.fixedDeltaTime, ForceMode.Force);

        if (playerData.SpeedMultiplier < 0.1f)
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
            if (!p.IsMainPlayer && !p.PauseMovement)
                Move(dataRepo, p, (p.TargetMovement - p.Player.transform.position));
            if (p.ShouldJump && !p.PauseMovement)
            {
                Jump(dataRepo, p);
            }
            if (p.ShouldPunch)
            {
                AttemptPunch(mono,dataRepo, p);
                p.ShouldPunch = false;
            }
            if (p.ShouldJumpOnCharacter && !p.IsPlayerFalling)
            {
                JumpOnCharacter(dataRepo, p);
            }
            if (p.IsOutfGround && !p.IsPlayerFalling)
                p.PlayerRigidbody.AddForce
                    ((p.Player.transform.position - dataRepo.GameData.GroundTrigger.position).normalized * 60,
                    ForceMode.Force);
            p.PlayerAnimator.SetBool("Grounded", p.IsGrounded);
        }
        foreach (PlayerData p in dataRepo.Players)
        {
            if (p.PushForce.magnitude > 0.01f && !p.IsPlayerFalling) // Apply force smoothly
            {
                Vector3 horizontalForce = p.PushForce;
                horizontalForce.y = 0; // Remove vertical force just in case
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
    public static void Update(DataRepo dataRepo)
    {
        //Ranking the players
        foreach (PlayerData playerData in dataRepo.Players)
        {
            if (playerData.IsOutfGround ||
                playerData.Player.transform.position.y < dataRepo.GameData.GroundTrigger.position.y)
            {
                playerData.HasBeenRemoved = true;
                int numberOfRemoved = 0;
                foreach (PlayerData playerData2 in dataRepo.Players)
                {
                    if (playerData2.HasBeenRemoved)
                    {
                        numberOfRemoved++;
                    }
                }
                playerData.Rank = 5 - numberOfRemoved;
            }
        }
        //Setting all players platform
        foreach(PlayerData playerData in dataRepo.Players)
        {
            Dictionary<PlatformData,float> distancesToPlayer = new Dictionary<PlatformData, float>();
            foreach(PlatformData platformData in dataRepo.Platforms)
            {
                Vector3 platformPos = new Vector3
                    (platformData.platform.transform.position.x, 0, platformData.platform.transform.position.z);
                Vector3 playerPos = new Vector3
                    (playerData.Player.transform.position.x, 0, playerData.Player.transform.position.z);

                distancesToPlayer.Add(platformData,Vector3.Distance(platformPos,playerPos));
            }
            playerData.CurrentPlatform = distancesToPlayer
                                .OrderBy(pair => pair.Value)
                                .First().Key.platform;
        }
        foreach(PlayerData playerData in dataRepo.Players)
        {
            if (playerData.Player.transform.position.y < dataRepo.GameData.GroundTrigger.position.y)
            {
                playerData.Player.GetComponent<CapsuleCollider>().isTrigger = true;
            }
        }



    }
    public static IEnumerator FreezePlayer(PlayerData playerData)
    {

        playerData.IsFrozen = true;
        playerData.PauseMovement = true;
        float EndTimer = Time.time + 1.5f;
        float interval = Time.time + 0.01f;
        float endPausing = Time.time + 0.5f;

        while (true)
        {
            if (Time.time > endPausing)
            {
                playerData.PauseMovement = false;
            }

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
                        pushDir.y = 0;
                        ApplyPush(pushDir, 30f, enemyData);
                        enemyData.PlayerAnimator.SetTrigger("GetHit");
                        enemyData.PunchVFX.gameObject.SetActive(true);
                        enemyData.PunchVFX.Play();
                        mono.StartCoroutine(FreezePlayer(enemyData));
                    }
                }
            }
        }


    }

    public static void OnPunchClicked(DataRepo dataRepo, PlayerData playerData)
    {
        if (playerData.IsGrounded)
        {
            playerData.ShouldPunch = true;
        }
    }

    public static void JumpOnCharacter(DataRepo dataRepo, PlayerData playerData)
    {
        if (!playerData.IsPlayerFalling && !playerData.IsGrounded)
            playerData.PlayerRigidbody.AddForce
                ((playerData.Player.transform.forward * 10 + playerData.Player.transform.up * 2), ForceMode.Impulse);
    }
    public static void Jump(DataRepo dataRepo, PlayerData playerData)
    {

        if (playerData.IsGrounded && !playerData.IsPlayerFalling)
        {
            playerData.JumpVFX.Play();
            Ray ray = new Ray(playerData.Player.transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                playerData.JumpVFX.transform.position = hit.point + Vector3.up * 0.01f; // Slightly above the ground
            }
            playerData.PlayerRigidbody.AddForce(Vector3.up * 10, ForceMode.Impulse);
            playerData.ShouldJump = false;
        }
    }
    public static void OnJumpClicked(DataRepo dataRepo, PlayerData playerData)
    {
        playerData.ShouldJump = true;

    }
    public static void ApplyPush(Vector3 pushDirection, float forceAmount, PlayerData playerData)
    {

        pushDirection.y = 0;
        pushDirection.Normalize();
        playerData.PushForce = pushDirection * forceAmount * 5;
    }
    public static List<PlayerData> GetUnFrozenNearbyEnemies(DataRepo dataRepo,PlayerData playerData)
    {
        List<PlayerData> nearbyEnemies = new List<PlayerData>();
        foreach(PlayerData p in dataRepo.Players)
        {
            if(p!= playerData)
            {
                if (Vector3.Distance(p.Player.transform.position, playerData.Player.transform.position) < 1f)
                    if(!p.IsFrozen)
                        nearbyEnemies.Add(p);
            }
        }
        return nearbyEnemies;
    }
    public static bool ThreatDetected(DataRepo dataRepo,PlayerData playerData)
    {
        if(GetUnFrozenNearbyEnemies(dataRepo, playerData).Count>0)
            return true;
        return false;
    }
    public static BotAction EvaluateBestAction(PlayerData playerData, DataRepo dataRepo)
    {
        List<BotAction> actions = new List<BotAction>();

        foreach (PlatformData platform in dataRepo.Platforms)
        {
            float score = platform.platform.SecondOfPrefab - Vector3.Distance(playerData.Player.transform.position, platform.platform.gameObject.transform.position) * 0.5f;
            actions.Add(new BotAction { ActionType = BotActionType.Move, Score = score, TargetPosition = platform.platform.gameObject.transform.position });
        }

        foreach (PlayerData enemy in GetUnFrozenNearbyEnemies(dataRepo, playerData))
        {
            float score = (10 - enemy.CurrentPlatform.SecondOfPrefab) * 2f;
            actions.Add(new BotAction { ActionType = BotActionType.Attack, Score = score, TargetEnemy = enemy });
        }

        if (ThreatDetected(dataRepo, playerData))
        {
            float score = 10f / playerData.CurrentPlatform.SecondOfPrefab;
            actions.Add(new BotAction { ActionType = BotActionType.Dodge, Score = score });
        }

        var ordered = actions.OrderByDescending(a => a.Score);
        string debugText = "Action Points:\n";
        foreach (BotAction action in ordered) 
        {
            if (action.ActionType == BotActionType.Move)
            {
                debugText += $"{action.ActionType}:Pos({action.TargetPosition}):{action.Score}\n";
            }
            else if (action.ActionType == BotActionType.Attack)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(action.TargetEnemy.DebugText.color);
                debugText += $"{action.ActionType}:To(<color=#{colorHex}>{action.TargetEnemy.Player.name}</color>):{action.Score}\n";
            }
            else if (action.ActionType == BotActionType.Dodge) 
            {
                debugText += $"{action.ActionType}:{action.Score}\n";
            }
        }

        playerData.DebugText.text = debugText;

        return ordered.First();
    }
    public static IEnumerator StartRobot(PlayerData playerData, DataRepo dataRepo)
    {
        while (true)
        {
            BotAction botAction= EvaluateBestAction(playerData, dataRepo);
            if(botAction.ActionType == BotActionType.Move)
            {
                playerData.TargetMovement = botAction.TargetPosition;
            }
            if(botAction.ActionType == BotActionType.Attack)
            {
                OnPunchClicked(dataRepo, playerData);
            }
            if(botAction.ActionType == BotActionType.Dodge)
            {
                OnJumpClicked(dataRepo, playerData);
            }
            yield return null;
        }
    }
}
