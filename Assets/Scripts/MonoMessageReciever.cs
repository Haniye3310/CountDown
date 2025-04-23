using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonoMessageReciever : MonoBehaviour
{
    public DataRepo DataRepo;
    bool start;
    IEnumerator Start()
    {
        SystemFunction.CreateMap(DataRepo);
        foreach (PlayerData p in DataRepo.Players)
        {
            p.PlayerAnimator.SetBool("Grounded", p.IsGrounded);
        }
        DataRepo.UIData.UIPanel.gameObject.SetActive(false);
        while (Mathf.Abs(Camera.main.fieldOfView - 25) > 0.1)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 25, 0.1f);
            yield return null;
        }

        DataRepo.UIData.StartCountDownTimer.gameObject.SetActive(true);
        float remaingStartTime = 4;
        while (remaingStartTime > 0)
        {
            remaingStartTime -= Time.deltaTime;
            if (((int)remaingStartTime + 1) == 4)
            {
                DataRepo.UIData.StartCountDownTimer.sprite = DataRepo.UIData.NumberThreeSprite;
            }
            if (((int)remaingStartTime + 1) == 3)
            {
                DataRepo.UIData.StartCountDownTimer.sprite = DataRepo.UIData.NumberTwoSprite;
            }
            if (((int)remaingStartTime + 1) == 2)
            {
                DataRepo.UIData.StartCountDownTimer.sprite = DataRepo.UIData.NumberOneSprite;
            }
            if (((int)remaingStartTime + 1) == 1)
            {
                DataRepo.UIData.StartCountDownTimer.gameObject.SetActive(false);
                DataRepo.UIData.GoImage.gameObject.SetActive(true);
            }
            yield return null;
        }
        DataRepo.UIData.GoImage.gameObject.SetActive(false);
        DataRepo.UIData.UIPanel.gameObject.SetActive(true);
        start = true;
        StartCoroutine(SystemFunction.StartTimerOftheGame(DataRepo));
        StartCoroutine(SystemFunction.CountDown(DataRepo));
    }
    private void FixedUpdate()
    {
        if (start)
            SystemFunction.FixedUpdate(DataRepo);
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
    public void OnRestartClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void OnHomeClicked()
    {
        Application.OpenURL("https://tobi.gg");
    }
}
