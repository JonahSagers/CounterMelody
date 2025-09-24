using UnityEngine;

public class NoteIcon : MonoBehaviour
{
    public KeyCode key;
    private SpriteRenderer render;
    public Sprite pressed;
    public Sprite unpressed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(key)){
            render.sprite = pressed;
        } else {
            render.sprite = unpressed;
        }
    }
}
