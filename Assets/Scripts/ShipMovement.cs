using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 5.0f;
    public float waterLevel = 0.0f;
    public float floatHeight = 2.0f;
    public float bounceDamp = 0.05f;
    public Vector3 buoyancyCentreOffset;

    public float acceleration = 2.0f; // İvmelenme hızı
    public float maxSpeed = 20.0f; // Maksimum hız

    public GameObject rudder; // Dümen referansı
    public float rudderRotationSpeed = 30.0f; // Dümen dönüş hızı
    public float maxRudderAngle = 45.0f; // Dümen maksimum açı
    public float returnSpeed = 5f;

    private float currentSpeed = 0.0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbody ayarları
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ; // X ve Z rotasyonlarını kısıtla
    }

    void FixedUpdate()
    {
        Vector3 actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);
        float forceFactor = 1.0f - ((actionPoint.y - waterLevel) / floatHeight);

        if (forceFactor > 0.0f)
        {
            Vector3 uplift = -Physics.gravity * (forceFactor - rb.velocity.y * bounceDamp);
            rb.AddForceAtPosition(uplift, actionPoint);
        }

        // İleri hareket ve ivmelenme
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime);
        }

        Vector3 forwardMovement = -transform.right * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forwardMovement);

        // Yönlendirme
        float turnDirection = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnDirection, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // Rotasyon sınırlamaları ve sallanma
       // ApplyNaturalSway();

        // Dümen kontrolü
        ControlRudder();
    }

    /*void ApplyNaturalSway()
    {
        float swayAmount = Mathf.Sin(Time.time) * 2.0f; // Sallantı miktarı
        float tiltAmount = Mathf.Cos(Time.time) * 2.0f; // Yatay sallantı miktarı

        Quaternion swayRotation = Quaternion.Euler(tiltAmount, transform.eulerAngles.y, swayAmount);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, swayRotation, Time.deltaTime * 0.5f));
    }*/

    void ControlRudder()
    {
        if (rudder == null)
        {
            return; // Rudder object is not assigned
        }

        // Mevcut dümen açısını al
        float rudderAngle = rudder.transform.localEulerAngles.x;
        if (rudderAngle > 180f) rudderAngle -= 360f; // -180 ile 180 arasında tutmak için

        // Girdi kontrolü
        if (Input.GetKey(KeyCode.A))
        {
            rudderAngle -= rudderRotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rudderAngle += rudderRotationSpeed * Time.deltaTime;
        }
        else
        {
            // A veya D tuşları bırakıldığında, dümeni merkeze döndürme
            rudderAngle = Mathf.Lerp(rudderAngle, 0f, returnSpeed * Time.deltaTime);
        }

        // Hedef açıyı sınırlama
        rudderAngle = Mathf.Clamp(rudderAngle, -maxRudderAngle, maxRudderAngle);

        // Dümeni güncelle
        Vector3 rudderEulerAngles = rudder.transform.localEulerAngles;
        rudderEulerAngles.x = rudderAngle;
        rudder.transform.localEulerAngles = rudderEulerAngles;
    }
}
