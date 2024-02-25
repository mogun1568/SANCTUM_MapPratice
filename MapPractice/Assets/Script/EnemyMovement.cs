using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Map map;
    [SerializeField] float moveSpeed = 3.0f;
    LinkedListNode<NodeInfo> currentRoad, nextRoad;

    void Start()
    {
        map = GameObject.Find("GameObject").GetComponent<Map>();

        currentRoad = map.roads.First;
        transform.position = new Vector3(currentRoad.Value.R, 1, currentRoad.Value.C);
        nextRoad = currentRoad.Next;
    }

    void Update()
    {
        // 현재 타겟 위치까지 이동
        if (nextRoad != null)
        {
            Vector3 targetPosition = new Vector3(nextRoad.Value.R, 1, nextRoad.Value.C);
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Translate 메서드를 사용하여 오브젝트를 이동
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            // 현재 타겟 위치에 도달하면 다음 타겟으로 변경
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // 다음 타겟 설정
                currentRoad = nextRoad;
                nextRoad = nextRoad.Next;
            }
        } else
        {
            Destroy(gameObject);
        }
    }
}
