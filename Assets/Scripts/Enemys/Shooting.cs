using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public float speed;
    public enum Dir{
        Up = 1,
        Down = -1
    }
    public Dir dir;
    private void Awake() 
    {
        if (dir == Dir.Up)
        {
            transform.GetChild(0).transform.localEulerAngles = new Vector3(0,0,-90);
        }
        
        else if (dir == Dir.Down)
        {
            transform.GetChild(0).transform.localEulerAngles = new Vector3(0,0,90);
        }

    }
    private void Update() 
    {
        if (transform.position.y > 10f)
        {
            Destroy(gameObject);            
        }
        transform.Translate(new Vector2(0,(float)dir) * speed * Time.deltaTime);    
    }
}
