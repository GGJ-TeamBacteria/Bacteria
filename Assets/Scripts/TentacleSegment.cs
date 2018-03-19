using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleSegment : MonoBehaviour {

    internal Tentacle rootTentacle;
    private bool isAnimating;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Antibiotic"))
        {
            // Tell player to take damage from Antibiotic
            rootTentacle.playerRef.TakeDamageAntibiotic();
        }
        else if (other.CompareTag("BadBacteria"))
        {
            // Tell player to take damage from BadBacteria
            rootTentacle.playerRef.TakeDamageBacteria();
			SelfDestroy otherScript = other.GetComponent<SelfDestroy>();
            otherScript.Death();
            //Destroy(other.gameObject);
        }
        else if (other.CompareTag("GoodBacteria"))
        {
            rootTentacle.buttonTrigger.Vibrate(500);

            //Call GainHealth() from Player
            rootTentacle.playerRef.GainHealth();
            SelfDestroy otherScript = other.GetComponent<SelfDestroy>();
            otherScript.Death();
            //Destroy(other.gameObject);
        }
        else if (other.CompareTag("HeartWin"))
        {
            rootTentacle.buttonTrigger.Vibrate(500);
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
}

