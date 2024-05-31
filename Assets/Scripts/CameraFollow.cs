using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // �÷��̾��� Transform
    private float minX; // ī�޶� �̵��� �� �ִ� �ּ� X ��
    public float smoothTime = 0.5F; // ī�޶� �������� �������� �뷫���� �ð�
    private Vector3 velocity = Vector3.zero; // ���������� ���� �ӵ� ���� ����

    private void Start()
    {
        // �ʱ� minX ���� ���� ī�޶��� X ��ġ�� ����
        minX = transform.position.x;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            // �÷��̾ minX���� �����ʿ� ���� ��츸 minX ������Ʈ
            if (player.position.x > minX)
            {
                minX = player.position.x;
            }

            Vector3 targetPosition = new Vector3(minX, transform.position.y, transform.position.z);

            // ���� ��ġ���� ��ǥ ��ġ(targetPosition)�� �ε巴�� �̵�
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
