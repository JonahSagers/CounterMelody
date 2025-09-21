using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float mana;
    public float health;
    public float maxMana;
    public Slider healthBar;
    public Image healthImage;
    public Slider manaBar;
    public Image manaImage;
    private Coroutine healthBarRoutine;
    private Coroutine manaBarRoutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 10;
        mana = 4;
        maxMana = 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(healthBarRoutine != null){
            StopCoroutine(healthBarRoutine);
        }
        healthBarRoutine = StartCoroutine(HealthBarUpdate());
    }

    public bool SpendMana(float spent)
    {
        if(mana < spent){
            return false;
        }
        mana -= spent;
        if(manaBarRoutine != null){
            StopCoroutine(manaBarRoutine);
        }
        manaBarRoutine = StartCoroutine(ManaBarUpdate());
        return true;
    }

    IEnumerator HealthBarUpdate()
    {
        float elapsed = 0;
        while(elapsed < 0.25f){
            healthBar.value = Mathf.Lerp(healthBar.value, health, Time.deltaTime * 20);
            elapsed += Time.deltaTime;
            if(healthImage.color == Color.white){
                healthImage.color = Color.red;
            } else {
                healthImage.color = Color.white;
            }
            yield return 0;
        }
        healthBar.value = health;
        healthImage.color = Color.red;
    }

    public IEnumerator ManaBarUpdate()
    {
        //I ain't writin all that
        manaBar.value = mana;
        yield return 0;
    }

    public void ManaBarScale()
    {
        RectTransform manaRect = manaBar.gameObject.GetComponent<RectTransform>();
        manaRect.sizeDelta = new Vector2(maxMana / 4, 0.6f);
        manaRect.anchoredPosition = new Vector2(-1.5f, 1.5f + maxMana / 8);
        manaBar.maxValue = maxMana;
    }
}
