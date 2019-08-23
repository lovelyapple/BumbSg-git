using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] Collider bodyCol;
    public bool IsDead;
    public float PowerUpPerRow = 0.1f;
    public float PowerUpStrength = 1.1f;
    [SerializeField] float deadEffectTime;
    [SerializeField] float desppearSpeed;
    [SerializeField] Vector3 defaultScale;

    [SerializeField] float chargeCheckRateFix;
    [SerializeField] float chargeCheckRange;
    public Vector3 startPoint;
    public Vector3 endPoint;

    const float pie = 3.1415926f;
    void Awake()
    {
        defaultScale = transform.localScale;
        bodyCol = GetComponent<Collider>();
    }
    public void Setup(Vector3 startP, Vector3 endP)
    {
        this.startPoint = startP;
        this.endPoint = endP;

        chargeCheckRange = (startP - endP).magnitude * chargeCheckRateFix;
    }
    public bool CheckPointRange(Vector3 point, bool isStart)
    {
        if (isStart)
        {
            return (point - startPoint).sqrMagnitude <= chargeCheckRange;
        }
        else
        {
            return (point - endPoint).sqrMagnitude <= chargeCheckRange;
        }
    }
    public void PowerUpOneRound()
    {
        PowerUpStrength += PowerUpPerRow;
        Debug.Log(gameObject.name + "power up! " + PowerUpStrength);
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag == "Ball")
        {
            if (PowerUpStrength != 1.0f)
                FieldManager.GetInstance().Ball.PowerUp(PowerUpStrength);

            IsDead = true;
            bodyCol.enabled = false;

            FieldManager.GetInstance().RemoveLine(this);

            Vector3 hitPos;
            foreach (ContactPoint point in collision.contacts)
            {
                if (point.thisCollider == bodyCol)
                {
                    hitPos = point.point;
                    transform.position = hitPos;
                }
            }
        }
    }
    void Update()
    {
        if (!IsDead)
        {
            deadEffectTime = 0;
            return;
        }

        if (deadEffectTime < 1.5f * pie)
        {
            deadEffectTime += Time.unscaledDeltaTime * desppearSpeed;
            float sin = (float)Mathf.Sin(deadEffectTime) * 0.3f + 1f;
            gameObject.transform.localScale = defaultScale * sin;
            return;
        }

        Destroy(this.gameObject);
    }
}
