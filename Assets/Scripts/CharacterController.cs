using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Karakterin hareket hızı
    public float rotationSpeed = 700f; // Karakterin dönme hızı

    private void Update()
    {
        // Yatay eksende (sağ-sol) hareket
        float horizontal = Input.GetAxis("Horizontal");
        // Düşey eksende (ileri-geri) hareket
        float vertical = Input.GetAxis("Vertical");

        // Hareket yönünü belirle
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Karakterin konumunu güncelle
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Karakterin yönünü belirle
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}