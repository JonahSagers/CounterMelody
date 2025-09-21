using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour
{
    public float duration;
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        if(Song.gameState < 2){
            anim.Play("Judgement_Popup");
        } else {
            anim.Play("Judgement_Popup_Reverse");
        }
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
