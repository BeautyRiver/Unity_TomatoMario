using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    private float minX; // 카메라가 이동할 수 있는 최소 X 값
    public float smoothTime = 0.5F; // 카메라가 따라잡을 때까지의 대략적인 시간
    private Vector3 velocity = Vector3.zero; // 내부적으로 사용될 속도 참조 변수

    private void Start()
    {
        transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
        // 초기 minX 값을 현재 카메라의 X 위치로 설정
        minX = transform.position.x;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            // 플레이어가 minX보다 오른쪽에 있을 경우만 minX 업데이트
            if (player.position.x > minX)
            {
                minX = player.position.x;
            }

            Vector3 targetPosition = new Vector3(minX, transform.position.y, transform.position.z);

            // 현재 위치에서 목표 위치(targetPosition)로 부드럽게 이동
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
