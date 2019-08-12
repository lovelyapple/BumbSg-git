using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] Collider col;
    public bool IsDead;
    public bool IsPowerUpLine;
    [SerializeField] float deadEffectTime;
    [SerializeField] float desppearSpeed;
    [SerializeField] Vector3 defaultScale;
    const float pie = 3.1415926f;
    void Awake()
    {
        defaultScale = transform.localScale;
        col = GetComponent<Collider>();
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag == "Ball")
        {
            if (IsPowerUpLine)
                FieldManager.GetInstance().Ball.PowerUp();

            IsDead = true;
            col.enabled = false;

            Vector3 hitPos;
            foreach (ContactPoint point in collision.contacts)
            {
                if (point.thisCollider == col)
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
