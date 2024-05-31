using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
#if UNITY_EDITOR
[ExecuteInEditMode]  //<- �� �κ��� �����Ϳ��� �۵��ǵ��� �����ϴ� �κ�
#endif


public class NoCheatBoxScript : MonoBehaviour
{
    public GameObject[] Items; // �ڽ����� ������ ������ ���
    public Sprite brokenBoxImg; // �μ��� �ڽ� �̹���

    private bool isOnBroken = false; // �ڽ� �μ������� ����
    private int randomIndex; // �����ϰ� ������ ������ ��ȣ
    private bool isDetectedSensor = false; // ������ �����ƴ��� ����
    public Vector2 sensorBoxSize = new Vector2(1f, 0.5f); // �������� ũ��
    public float controlYLength;
    public LayerMask playerHeadLayer; // �÷��̾� ���̾�
    [Header("������Ʈ")]
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
        // ġƮ���� ����
        if (SensorDetectionCheck() == true && !isOnBroken) // �÷��̾ �����ǰ� �μ��� ���°� �ƴ϶��
        {
            isOnBroken = true; // �ڽ� �μ���(�ݰ�)
            spr.sprite = brokenBoxImg; // �ڽ��� �μ��� �̹�����
            // Items �迭�� ������� ���� ��쿡�� ó��
            if (Items != null)
            {
                randomIndex = Random.Range(0, Items.Length);
                GameObject selectedObject = Items[randomIndex];
                // ������ ������Ʈ�� ���� ������ ����
                spawnedObject = Instantiate(selectedObject, transform.position, Quaternion.identity);

                // ������ ������Ʈ�� ������ �ڷ�ƾ�� ����
                StartCoroutine(MoveObjectUp(spawnedObject));
            }
        }
            
    }
   
    // ������ ���� (�÷��̾� ����)
    private bool SensorDetectionCheck()
    {
        Bounds bounds = boxCollider2D.bounds;
        Vector2 sensorPos = new Vector2(bounds.center.x, bounds.min.y + controlYLength);
        isDetectedSensor = Physics2D.OverlapBox(sensorPos, sensorBoxSize, 0, playerHeadLayer);

        return isDetectedSensor;
    }

    // ������Ʈ�� õõ�� ���� �̵���Ű�� �ڷ�ƾ
    IEnumerator MoveObjectUp(GameObject obj)
    {
        // ��ũ��Ʈ�� �Ͻ������� ��Ȱ��ȭ
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();

        foreach (var script in scripts)
        {
            script.enabled = false;
        }

        obj.GetComponent<Collider2D>().enabled = false;
        obj.GetComponent<Rigidbody2D>().isKinematic = true;

        // ������ ó��
        float duration = 1.3f; // ������ ���� �ð�
        float speed = 1f; // ������ �ӵ�
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            obj.transform.Translate(Vector3.up * speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }        
        // ��ũ��Ʈ�� �ٽ� Ȱ��ȭ
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
