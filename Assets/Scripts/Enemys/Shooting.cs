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
    private Dir dir;
    private void Awake() 
    {
        // 부모의 DetectOption의 DetectType이 Up이면 Up, Down이면 Down
        dir = transform.parent.GetComponent<ShotEnemyTrigger>().dectType == DetectOption.DetectType.Up ? Dir.Up : Dir.Down; 
        
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
