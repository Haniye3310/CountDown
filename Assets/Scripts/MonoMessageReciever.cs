using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonoMessageReciever : MonoBehaviour
{
    public DataRepo DataRepo;
    private void Start()
    {
        StartCoroutine(SystemFunction.Start(DataRepo,this));
    }
    private void FixedUpdate()
    {
        SystemFunction.FixedUpdate(this, DataRepo);
    }
    private void Update()
    {
        SystemFunction.Update(DataRepo);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpClicked();
        }
    }
    public void OnJumpClicked()
    {
        foreach (PlayerData playerData in DataRepo.Players)
        {
            if (playerData.IsMainPlayer)
            {
                SystemFunction.OnJumpClicked(DataRepo, playerData);

            }
        }

    }
    public void OnPunchClicked()
    {
        foreach (PlayerData playerData in DataRepo.Players)
        {
            if (playerData.IsMainPlayer)
            {
                SystemFunction.OnPunchClicked(DataRepo, playerData);

            }
        }
    }
    public void OnRestartClicked()
    {
        StartCoroutine(SystemFunction.OnRestartClicked(DataRepo));
    }
    public void OnHomeClicked()
    {
        Application.OpenURL("https://tobi.gg");
    }
    public void OnNextTutorialClicked()
    {
        StartCoroutine( SystemFunction.OnNextTutorialClicked(DataRepo));
    }
}
