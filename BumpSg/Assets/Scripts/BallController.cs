using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] float powerUpHeadTime;
    [SerializeField] float powerUpForcePower;
    Coroutine PowerUpCoroutine;
    void Awake()
    {
        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody>();
        }
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PowerUp(1.2f);
        }
    }
    public void PowerUp(float powerUpRate)
    {
        powerUpForcePower = powerUpRate;
        if(PowerUpCoroutine != null)
        {
            StopCoroutine(PowerUpCoroutine);
            PowerUpCoroutine = null;
        }

        PowerUpCoroutine = StartCoroutine(PowerUpEnumerator());
    }
    IEnumerator PowerUpEnumerator()
    {
        var headTime = powerUpHeadTime;
        var beforePos = gameObject.transform.position;

        while (headTime > 0)
        {
            headTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        var currentPos = gameObject.transform.position;
        var dir = currentPos - beforePos;
        dir.z = 0;
        dir *= powerUpForcePower;
        rigidbody.AddForce(dir, ForceMode.Impulse);
    }
}
