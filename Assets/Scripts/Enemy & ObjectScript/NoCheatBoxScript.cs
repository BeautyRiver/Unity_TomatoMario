using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
#if UNITY_EDITOR
[ExecuteInEditMode]  //<- 이 부분이 에디터에서 작동되도록 선언하는 부분
#endif


public class NoCheatBoxScript : MonoBehaviour
{
    public GameObject[] Items; // 박스에서 등장할 아이템 목록
    public Sprite brokenBoxImg; // 부서진 박스 이미지

    private bool isOnBroken = false; // 박스 부서졌는지 여부
    private int randomIndex; // 랜덤하게 등장할 아이템 번호
    private bool isDetectedSensor = false; // 센서에 감지됐는지 여부
    public Vector2 sensorBoxSize = new Vector2(1f, 0.5f); // 센서감지 크기
    public float controlYLength;
    public LayerMask playerHeadLayer; // 플레이어 레이어
    [Header("컴포넌트")]
    private BoxCollider2D boxCollider2D;
    private GameObject spawnedObject;
    private SpriteRenderer spr;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // 치트센서 감지
        if (SensorDetectionCheck() == true && !isOnBroken) // 플레이어가 감지되고 부서진 상태가 아니라면
        {
            isOnBroken = true; // 박스 부서짐(금감)
            spr.sprite = brokenBoxImg; // 박스를 부서진 이미지로
            // Items 배열이 비어있지 않은 경우에만 처리
            if (Items != null)
            {
                randomIndex = Random.Range(0, Items.Length);
                GameObject selectedObject = Items[randomIndex];
                // 생성된 오브젝트에 대한 참조를 저장
                spawnedObject = Instantiate(selectedObject, transform.position, Quaternion.identity);

                // 생성된 오브젝트에 움직임 코루틴을 시작
                StartCoroutine(MoveObjectUp(spawnedObject));
            }
        }
            
    }
   
    // 센서로 감지 (플레이어 감지)
    private bool SensorDetectionCheck()
    {
        Bounds bounds = boxCollider2D.bounds;
        Vector2 sensorPos = new Vector2(bounds.center.x, bounds.min.y + controlYLength);
        isDetectedSensor = Physics2D.OverlapBox(sensorPos, sensorBoxSize, 0, playerHeadLayer);

        return isDetectedSensor;
    }

    // 오브젝트를 천천히 위로 이동시키는 코루틴
    IEnumerator MoveObjectUp(GameObject obj)
    {
        // 스크립트를 일시적으로 비활성화
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();

        foreach (var script in scripts)
        {
            script.enabled = false;
        }

        obj.GetComponent<Collider2D>().enabled = false;
        obj.GetComponent<Rigidbody2D>().isKinematic = true;

        // 움직임 처리
        float duration = 1.3f; // 움직임 지속 시간
        float speed = 1f; // 움직임 속도
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            obj.transform.Translate(Vector3.up * speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }        
        // 스크립트를 다시 활성화
        foreach (var script in scripts)
        {
            script.enabled = true;
        }

        obj.GetComponent<Collider2D>().enabled = true;
        obj.GetComponent<Rigidbody2D>().isKinematic = false;
        
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (boxCollider2D != null)
        {
            Gizmos.color = Color.green;
            Bounds bounds = boxCollider2D.bounds;
            Vector2 sensorPos = new Vector2(bounds.center.x, bounds.min.y + controlYLength);
            Gizmos.DrawWireCube(sensorPos, sensorBoxSize);
        }
    }
#endif

}
