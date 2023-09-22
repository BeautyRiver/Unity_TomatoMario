using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public float scrollSpeed;
    private Renderer renderer;
    private float targetOffset;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("GameScene");
        }
    }
    private void FixedUpdate()
    {
        targetOffset += Time.deltaTime * scrollSpeed;
        renderer.material.mainTextureOffset = new Vector2(targetOffset, 0);
    }
}
