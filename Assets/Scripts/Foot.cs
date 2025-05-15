using UnityEngine;

public class Foot : MonoBehaviour
{
    private DataRepo dataRepo;
    private void Start()
    {
        dataRepo = FindAnyObjectByType<DataRepo>();
    }
    private void OnTriggerEnter(Collider other)
    {
        SystemFunction.OnFootTriggerEnter(dataRepo, GetComponentInParent<Player>(), other);

    }
    private void OnTriggerStay(Collider other)
    {
        SystemFunction.OnFootTriggerStay(dataRepo, GetComponentInParent<Player>(), other);

    }
    private void OnTriggerExit(Collider other)
    {
        SystemFunction.OnFootTriggerExit(dataRepo, GetComponentInParent<Player>(), other);
    }
}
