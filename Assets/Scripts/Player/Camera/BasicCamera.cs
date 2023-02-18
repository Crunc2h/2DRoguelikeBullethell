using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCamera : MonoBehaviour
{
    private Transform playerTransform = null;
    private bool isCameraShakeActive = false;
    private void Update()
    {
        if(GameObject.FindGameObjectWithTag("Player") != null && !isCameraShakeActive)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
    }

    public IEnumerator Shake(float shakeIntensity, float shakeDuration, float timerDecreasePerLoop)
    {
        isCameraShakeActive = true;

            while (shakeDuration > 0)
            {
                transform.position = new Vector3(playerTransform.position.x + Random.Range(-shakeIntensity, shakeIntensity),
                    playerTransform.position.y + Random.Range(-shakeIntensity, shakeIntensity), transform.position.z);
                shakeDuration -= timerDecreasePerLoop;
                yield return new WaitForSeconds(timerDecreasePerLoop);
            }
        isCameraShakeActive = false;
    }
}
