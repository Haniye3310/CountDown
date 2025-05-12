using System;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Animator Animator;
    public int SecondOfPrefab;
    private DataRepo dataRepo;
    private void Start()
    {
        dataRepo = FindAnyObjectByType<DataRepo>();
    }
    public void OnAnimatorOpenEvent()
    {
        SystemFunction.OnAnimatorOpenEvent(dataRepo,this);
    }
    public void OnAnimatorCloseEvent() 
    {
        SystemFunction.OnAnimatorCloseEvent(dataRepo, this);

    }
}
