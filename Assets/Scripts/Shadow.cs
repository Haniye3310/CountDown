using UnityEngine;

public class Shadow : MonoBehaviour
{
    public GameObject shadowPrefab;
    public LayerMask groundLayer;
    public Vector3 ScaleOfShadow;
    private GameObject shadow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Instantiate the shadow prefab
        shadow = Instantiate(shadowPrefab, transform);
        shadow.transform.localScale = ScaleOfShadow;
        if (shadow.TryGetComponent<Collider>(out Collider col))
        {
            Destroy(col); // Remove any collider on the shadow prefab
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Adjust shadow to ground
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            shadow.transform.position = hit.point + Vector3.up * 0.03f; // Slightly above the ground
            shadow.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
