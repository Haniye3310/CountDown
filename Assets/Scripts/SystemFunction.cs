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
    public static IEnumerator DeleteCircleAroundMainChar(DataRepo dataRepo)
    {
        yield return new WaitForSeconds(2);
        dataRepo.GameData.CircleAroundMainCharacter.gameObject.SetActive(false);

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
        foreach(PlayerData p in dataRepo.Players)
        {
            if(p.IsMainPlayer)
                if (p.HasBeenRemoved)
                    return true;
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
        float deadZone = 0.2f;
        if (!playerData.IsMainPlayer)
            direction = direction.normalized;
        if (direction.magnitude < deadZone)
        {
            direction = Vector3.zero;
        }
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            playerData.PlayerRigidbody.AddForce
                                (direction * 9000 * Time.fixedDeltaTime, ForceMode.Force);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            playerData.PlayerRigidbody.
                MoveRotation(Quaternion.Slerp(playerData.PlayerRigidbody.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
        if (direction.magnitude < 0.2f)
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
            if (!p.IsMainPlayer && !p.PauseMovement && Vector3.Distance(p.TargetMovement, Vector3.zero) > 0.1f)
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
            if (p.ShouldJumpOnCharacter && !p.IsPlayerFalling && !p.IsMainPlayer)
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
        float EndTimer = Time.time + 1f;
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


            playerData.JumpVFX.Play();
            Ray ray = new Ray(playerData.Player.transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                playerData.JumpVFX.transform.position = hit.point + Vector3.up * 0.01f; // Slightly above the ground
            }
            playerData.PlayerRigidbody.AddForce(Vector3.up * 10, ForceMode.Impulse);
            playerData.ShouldJump = false;
        
    }
    public static void OnJumpClicked(DataRepo dataRepo, PlayerData playerData)
    {
        if (playerData.IsGrounded && !playerData.PauseMovement && !playerData.IsPlayerFalling)
            playerData.ShouldJump = true;

    }
    public static void ApplyPush(Vector3 pushDirection, float forceAmount, PlayerData playerData)
    {

        pushDirection.y = 0;
        pushDirection.Normalize();
        playerData.PushForce = pushDirection * forceAmount * 5;
    }


    public static IEnumerator StartRobot(MonoBehaviour mono,PlayerData playerData, DataRepo dataRepo)
    {
        while(true)
        {
            if (Time.time - playerData.LastDecisionTime >= playerData.DecisionInterval)
            {
                MakeDecision(mono,dataRepo,playerData);
                playerData.LastDecisionTime = Time.time;
            }
            yield return null;
        }
    }

    public static void MakeDecision(MonoBehaviour mono,DataRepo dataRepo,PlayerData playerData)
    {
        switch (playerData.BotDifficulty)
        {
            case BotDifficulty.Easy:
                mono.StartCoroutine( EasyBotLogic(dataRepo,playerData));
                break;
            case BotDifficulty.Medium:
                MediumBotLogic(dataRepo,playerData);
                break;
            case BotDifficulty.Hard:
                HardBotLogic(dataRepo,playerData);
                break;
        }
    }
    public static IEnumerator EasyBotLogic(DataRepo dataRepo,PlayerData playerData) 
    {
        if(playerData.CurrentPlatform == null)
        {
            yield break;
        }
        //Tile Escape Rule
        if (playerData.CurrentPlatform.SecondOfPrefab == 0)
        {
            playerData.TargetMovement = FindDifferenttileBiggerThanValue(dataRepo,playerData.CurrentPlatform,0,playerData.BotDifficulty);
        }

        //Punch Rule
        if (IsThereAnyEnemyNear(dataRepo, playerData) && IsThereChance(25))
        {
            yield return new WaitForSeconds(0.5f);
            OnPunchClicked(dataRepo,playerData);

        }
        //Tile Targeting
        else
        {
            playerData.TargetMovement = FindRandomPosOnWholeMap(dataRepo);
        }
    }
    public static void MediumBotLogic(DataRepo dataRepo, PlayerData playerData) 
    {
        if (playerData.CurrentPlatform == null)
        {
            return;
        }
        //Tile Escape Rule
        if (playerData.CurrentPlatform.SecondOfPrefab == 0)
        {
            playerData.TargetMovement = FindNearestTilePosBiggerThanCurrent(dataRepo,playerData.CurrentPlatform);

        }

        //player Proximity
        if (IsThereAnyEnemyNear(dataRepo, playerData, true))
        {
            playerData.TargetMovement = FindOneNearByEmptyTile(dataRepo,playerData.CurrentPlatform);

        }

        //Punch Rule
        if (IsThereAnyEnemyNear(dataRepo, playerData))
        {
            OnPunchClicked(dataRepo,playerData);
        
        }

        //Jumping (Threat)
        if (playerData.CurrentPlatform.SecondOfPrefab == 0 && IsThereAnyEnemyNear(dataRepo, playerData))

        {
            playerData.TargetMovement = FindDifferenttileBiggerThanValue(dataRepo,playerData.CurrentPlatform,1,playerData.BotDifficulty);
            if (Vector3.Distance(playerData.TargetMovement, playerData.Player.transform.position) > 0.1f)
                OnJumpClicked(dataRepo,playerData);
        }
        //Tile Targeting
        //Jumping (Movement)
        else
        {
            playerData.TargetMovement = FindAnyTileBiggerThanCurrentIncludingEdge(dataRepo,playerData.CurrentPlatform);
            if (IsThereChance(15))
                if (Vector3.Distance(playerData.TargetMovement, playerData.Player.transform.position) > 0.1f)
                    OnJumpClicked(dataRepo, playerData);
        }

    }
    public static void HardBotLogic(DataRepo dataRepo, PlayerData playerData) 
    {
        if (playerData.CurrentPlatform == null)
        {
            return;
        }
        //Tile Escape Rule
        if (playerData.CurrentPlatform.SecondOfPrefab <= 1) 
        {
            Debug.Log("Tile escape");
            playerData.TargetMovement = FindDifferenttileBiggerThanValue(dataRepo,playerData.CurrentPlatform,2,playerData.BotDifficulty);
            
        }

        //Player Proximity
        if (IsThereAnyEnemyNear(dataRepo, playerData, false))
        {
            Debug.Log("escape enemy");
            playerData.TargetMovement = FindDifferenttileBiggerThanValue(dataRepo,playerData.CurrentPlatform,2,playerData.BotDifficulty);

        }

        //Punch Rule
        List<PlayerData> enemies = GetUnFrozenNearbyEnemies(dataRepo,playerData);
        foreach(PlayerData enemy in enemies)
        {
            if (enemy.CurrentPlatform.SecondOfPrefab <= 1 && playerData.CurrentPlatform.SecondOfPrefab>= 2)
            {
                Debug.Log("punch");
                OnPunchClicked(dataRepo, playerData);
            }
        }
        //Jumping (Threat)
        if (playerData.CurrentPlatform.SecondOfPrefab <= 1 && IsThereAnyEnemyNear(dataRepo, playerData))
        {
            Debug.Log("Jump Threat");
            playerData.TargetMovement = FindDifferenttileBiggerThanValue(dataRepo, playerData.CurrentPlatform, 2, playerData.BotDifficulty);
            if(Vector3.Distance(playerData.TargetMovement, playerData.Player.transform.position) >0.1f)
                OnJumpClicked(dataRepo,playerData);
        }

        //Tile Targeting
        //Jumping (Movement)
        else
        {
            Debug.Log("Move");
            playerData.TargetMovement = FindHighestTileCenter(dataRepo, playerData.CurrentPlatform);
            if (IsThereChance(25))
                if (Vector3.Distance(playerData.TargetMovement, playerData.Player.transform.position) > 0.1f)
                    OnJumpClicked(dataRepo, playerData);
        }
    }
    public static List<PlayerData> GetUnFrozenNearbyEnemies(DataRepo dataRepo,PlayerData playerData,bool differentTile = false)
    {
        List<PlayerData> nearbyEnemies = new List<PlayerData>();
        foreach(PlayerData p in dataRepo.Players)
        {
            if(p!= playerData)
            {
                if (Vector3.Distance(p.Player.transform.position, playerData.Player.transform.position) < 1f)
                    if (!p.IsFrozen) 
                    {
                        if (differentTile)
                        {
                            if (p.CurrentPlatform != playerData.CurrentPlatform)
                            {
                                nearbyEnemies.Add(p);
                            }
                        }
                        else
                        {
                            nearbyEnemies.Add(p);
                        }
                    }
                        
            }
        }
        return nearbyEnemies;
    }
    public static bool IsThereAnyEnemyNear(DataRepo dataRepo,PlayerData playerData,bool differentTile = false)
    {
        if(GetUnFrozenNearbyEnemies(dataRepo, playerData,differentTile).Count>0)
            return true;
        return false;
    }

    //Medium bot: Tile Escape Rule
    public static Vector3 FindNearestTilePosBiggerThanCurrent(DataRepo dataRepo,Platform currentPlatform)
    {
        Dictionary<PlatformData, float> distances = new Dictionary<PlatformData, float>();

        // Store distances only for platforms bigger than the current one
        foreach (PlatformData p in dataRepo.Platforms)
        {
            if (p.platform.SecondOfPrefab > currentPlatform.SecondOfPrefab) 
            {
                float distance = Vector3.Distance(p.platform.transform.position, currentPlatform.transform.position);
                distances.Add(p, distance);
            }
        }

        // Sort by distance (ascending)
        var sorted = distances.OrderBy(pair => pair.Value);

        // Get the nearest one, if any
        PlatformData nearest = sorted.FirstOrDefault().Key;

        return nearest.platform.transform.position;
    }
    //Medium bot: Tile Targeting
    public static Vector3 FindAnyTileBiggerThanCurrentIncludingEdge(DataRepo dataRepo, Platform currentPlatform)
    {

        foreach (PlatformData p in dataRepo.Platforms)
        {
            if (p.platform.SecondOfPrefab > currentPlatform.SecondOfPrefab) 
            {
                Transform t = p.platform.transform;

                // Use scale as bounds (assumes platform is centered at origin in local space)
                Vector3 scale = t.localScale;

                // Generate a random local position within the scaled bounds (X-Z plane)
                float randomX = Random.Range(-0.5f * scale.x, 0.5f * scale.x);
                float randomZ = Random.Range(-0.5f * scale.z, 0.5f * scale.z);

                // Assuming platform is flat on the Y axis (like a floor), Y is constant
                Vector3 localOffset = new Vector3(randomX, t.position.y, randomZ);

                // Convert local offset to world space
                Vector3 randomWorldPosition = t.TransformPoint(localOffset);

                return randomWorldPosition;
            }
        }
        return Vector3.zero;
    }
    //Hard Bot : TileTargeting
    public static Vector3 FindHighestTileCenter(DataRepo dataRepo, Platform currentPlatform)
    {
        Vector3 highestTileCenter = Vector3.zero;
        float highestSecond = float.MinValue;
        foreach(PlatformData p in dataRepo.Platforms)
        {
            if (p.platform.SecondOfPrefab > highestSecond)
            {
                highestSecond = p.platform.SecondOfPrefab;
                highestTileCenter = p.platform.transform.position;
            }
        }
        return highestTileCenter; ;
    }
    //HardBot: Player Proximity
    //Medium Bot: Jumping(Threat)
    //Hard Bot : Jumping(Threat)
    //Easy Bot : Tile Escape Rule
    //Hard Bot : Tile Escape Rule
    public static Vector3 FindDifferenttileBiggerThanValue(DataRepo dataRepo, Platform currentPlatform,int value,BotDifficulty botDifficulty)
    {
        foreach (PlatformData p in dataRepo.Platforms)
        {
            if (p.platform.SecondOfPrefab >= value && p.platform.gameObject != currentPlatform.gameObject)
            {
                if(botDifficulty == BotDifficulty.Easy)
                {
                    Transform t = p.platform.transform;

                    // Use scale as bounds (assumes platform is centered at origin in local space)
                    Vector3 scale = t.localScale;

                    // Generate a random local position within the scaled bounds (X-Z plane)
                    float randomX = Random.Range(-0.5f * scale.x, 0.5f * scale.x);
                    float randomZ = Random.Range(-0.5f * scale.z, 0.5f * scale.z);

                    // Assuming platform is flat on the Y axis (like a floor), Y is constant
                    Vector3 localOffset = new Vector3(randomX, t.position.y, randomZ);

                    // Convert local offset to world space
                    Vector3 randomWorldPosition = t.TransformPoint(localOffset);
                    return randomWorldPosition;
                }
                else
                {
                    return p.platform.transform.position;
                }
            }
        }
        return Vector3.zero;
    }

    //Medium Bot: Player Proximity
    public static Vector3 FindOneNearByEmptyTile(DataRepo dataRepo,Platform currentPlatform) 
    {
        Dictionary<PlatformData, float> distances = new Dictionary<PlatformData, float>();

        // Store distances 
        foreach (PlatformData p in dataRepo.Platforms)
        {
            float distance = Vector3.Distance(p.platform.transform.position, currentPlatform.transform.position);
            distances.Add(p, distance);
        }

        // Sort by distance (ascending)
        var sorted = distances.OrderBy(pair => pair.Value);
        Dictionary<PlatformData, float> temp =new Dictionary<PlatformData, float>(sorted);

        foreach(var d in sorted)
        {
            foreach(PlayerData p in dataRepo.Players)
            {
                if(p.CurrentPlatform == d.Key.platform)
                {
                    temp.Remove(d.Key);
                }
            }
        }
        // Get the nearest one, if any
        PlatformData nearest = temp.FirstOrDefault().Key;

        return nearest.platform.transform.position;
    }

    //Easy Bot : Tile targeting
    public static Vector3 FindRandomPosOnWholeMap(DataRepo dataRepo)
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

    public static bool IsThereChance(float probability)
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        float chance = Random.Range(0f, 100f);

        if (chance < probability)
        {
            return true;
        }
        return false;
    }
}
