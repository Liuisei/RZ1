using UnityEngine;

public class FreeFlyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float ascendSpeed = 5f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;

        // 上昇/下降（Q/E）
        if (Input.GetKey(KeyCode.E)) move += Vector3.up * ascendSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q)) move += Vector3.down * ascendSpeed * Time.deltaTime;

        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
