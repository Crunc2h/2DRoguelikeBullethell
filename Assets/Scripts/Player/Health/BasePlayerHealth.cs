using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BasePlayerHealth : MonoBehaviour
{
    public Image[] heartsArray;
    public int maxHeartCount = 5;
    public int currentHealth = 5;
    public bool isDead = false;

    private void Awake()
    {
        for (int i = 0; i < heartsArray.Length; i++)
        {
            if (i < maxHeartCount)
            {
                heartsArray[i].enabled = true;
            }
            else
            {
                heartsArray[i].enabled = false;
            }
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage();
        }

        if(isDead)
        {
            StartCoroutine(temporaryDeath());
        }
    }

    public void TakeDamage()
    {
        currentHealth--;
        
        for (int i = 0; i < heartsArray.Length; i++)
        {
            if (i < currentHealth)
            {
                heartsArray[i].enabled = true;
            }
            else
            {
                heartsArray[i].enabled = false;
            }
        }

        if(currentHealth <= 0)
        {
            isDead = true;
        }
    }

    private IEnumerator temporaryDeath()
    {
        isDead = false;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<BasePlayerMovement>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
