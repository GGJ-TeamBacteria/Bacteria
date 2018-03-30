using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleSegment : MonoBehaviour {

    internal Tentacle rootTentacle;
    private bool isAnimating;
    private Vector3 lastPosition = Vector3.zero;
    private float speed;

    void FixedUpdate()
    {
        speed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Antibiotic"))
        {
            rootTentacle.buttonTrigger.VibrateForSomethingBad();

            // Tell player to take damage from Antibiotic
            rootTentacle.playerRef.TakeDamageAntibiotic();
        }
        else if (other.CompareTag("BadBacteria"))
        {
            rootTentacle.buttonTrigger.VibrateForSomethingBad();

            // Tell player to take damage from BadBacteria
            StartCoroutine("AttackedAnimation");
            rootTentacle.playerRef.TakeDamageBacteria();
			SelfDestroy otherScript = other.GetComponent<SelfDestroy>();
            otherScript.Death();
        }
        else if (other.CompareTag("GoodBacteria"))
        {
            rootTentacle.buttonTrigger.VibrateForSomethingGood();
            StartCoroutine("AbsorbingAnimation");

            //Call GainHealth() from Player
            rootTentacle.playerRef.GainHealth();
            SelfDestroy otherScript = other.GetComponent<SelfDestroy>();
            otherScript.Death();
        }
        else if (other.CompareTag("HeartWin"))
        {
            rootTentacle.buttonTrigger.VibrateForSomethingGood();
            Destroy(other.gameObject);
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

    IEnumerator AttackedAnimation()
    {
        isAnimating = true;

        gameObject.transform.localScale /= 2.0f;
        yield return new WaitForSeconds(0.3f);

        gameObject.transform.localScale *= 2.0f;

        isAnimating = false;
    }
    public float getSpeed()
    {
        return speed;
    }
}

