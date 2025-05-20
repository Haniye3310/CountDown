using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private DataRepo dataRepo;
    private void Start()
    {
        dataRepo = FindAnyObjectByType<DataRepo>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        SystemFunction.OnPlayerCollisionEnter(this,this,collision,dataRepo);
    }
    private void OnCollisionExit(Collision collision)
    {
        SystemFunction.OnPlayerCollisionExit(this,collision,dataRepo);
    }
    private void OnCollisionStay(Collision collision)
    {
        SystemFunction.OnPlayerCollisionStay(this,collision,dataRepo);
    }
    private void OnTriggerEnter(Collider other)
    {
        SystemFunction.OnPlayerTriggerEnter(this,other,dataRepo);
    }
}
