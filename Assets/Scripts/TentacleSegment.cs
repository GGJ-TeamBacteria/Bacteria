using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleSegment : PoolableBehaviour
{

    internal Tentacle rootTentacle;
    private bool isAnimating;
    Vector3 lastPosition = Vector3.zero;
    float speed;

    void FixedUpdate()
    {
        speed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            PowerUp gainedPowerUp = other.GetComponent<PowerUp>();
            
            // Get the gained power up effect
            gainedPowerUp.AffectPlayer(rootTentacle);

            StartCoroutine("AbsorbingAnimation");

            SelfDestroy otherScript = other.GetComponent<SelfDestroy>();
            otherScript.Death();
        }
        else if (other.CompareTag("BadBacteria"))
        {
            StartCoroutine("AbsorbingAnimation");
			SelfDestroy otherScript = other.GetComponent<SelfDestroy>();
            otherScript.Death();
        }
        else if (other.CompareTag("GoodBacteria"))
        {
            StartCoroutine("AbsorbingAnimation");

            SelfDestroy otherScript = other.GetComponent<SelfDestroy>();
            otherScript.Death();
        }
    }

    IEnumerator AbsorbingAnimation()
    {
        isAnimating = true;

        gameObject.transform.localScale *= 1.2f;
        yield return new WaitForSeconds(0.05f);
        gameObject.transform.localScale *= 1.25f;
        yield return new WaitForSeconds(0.05f);
        gameObject.transform.localScale *= 1.3f;
        yield return new WaitForSeconds(0.05f);
        gameObject.transform.localScale *= 1.35f;
        yield return new WaitForSeconds(0.05f);

        gameObject.transform.localScale /= 1.35f;
        yield return new WaitForSeconds(0.08f);
        gameObject.transform.localScale /= 1.3f;
        yield return new WaitForSeconds(0.08f);
        gameObject.transform.localScale /= 1.25f;
        yield return new WaitForSeconds(0.08f);
        gameObject.transform.localScale /= 1.2f;

        isAnimating = false;
    }

    public float getSpeed()
    {
        return speed;
    }

    protected override void OnPoolableEnable()
    {
    }

    // to reset the velocity the object still have
    protected override void OnPoolableDisable()
    {
    }
}

