using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindingMonster : MonoBehaviour
{
    [Header("刷怪设置")]
    // 刷怪的最小距离
    public float MinSpawnDistance = 5f;
    // 刷怪的最大距离
    public float MaxSpawnDistance = 10f;
    // 刷怪间隔（秒）
    public float SpawnInterval = 1f;
    // 刷怪的敌人池名称
    public string EnemyPoolName = "NormalEnemy";

    [Header("刷怪加速设置")]
    // 每次刷怪后间隔缩短的量
    public float SpawnIntervalDecrement = 0.01f;
    // 最小刷怪间隔
    public float MinSpawnInterval = 0.3f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnInterval);
            if (!PoolManager.Instance.CanGetObj) continue;
            SpawnEnemy();
            // 每次刷怪后缩短间隔，不低于最小值
            SpawnInterval = Mathf.Max(MinSpawnInterval, SpawnInterval - SpawnIntervalDecrement);
        }
    }

    private void SpawnEnemy()
    {
        // 随机距离（在 Min ~ Max 之间）
        float distance = Random.Range(MinSpawnDistance, MaxSpawnDistance);

        // 随机选择 X 或 Y 轴偏移方向，正负随机
        Vector2 spawnOffset;
        float sign = Random.value > 0.5f ? 1f : -1f;
        if (Random.value > 0.5f)
            spawnOffset = new Vector2(sign * distance, Random.Range(-distance, distance));
        else
            spawnOffset = new Vector2(Random.Range(-distance, distance), sign * distance);

        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

        GameObject enemy = PoolManager.Instance.GetObj(EnemyPoolName);
        GameObject bat = PoolManager.Instance.GetObj("Bat");
        if (enemy != null)
            enemy.transform.position = spawnPos;

        if (bat != null)
        {
            // Bat 单独随机一个坐标
            float batDistance = Random.Range(MinSpawnDistance, MaxSpawnDistance);
            float batSign = Random.value > 0.5f ? 1f : -1f;
            Vector2 batOffset;
            if (Random.value > 0.5f)
                batOffset = new Vector2(batSign * batDistance, Random.Range(-batDistance, batDistance));
            else
                batOffset = new Vector2(Random.Range(-batDistance, batDistance), batSign * batDistance);
            bat.transform.position = transform.position + (Vector3)batOffset;
        }
    }

    // 在 Scene 视图中可视化刷怪范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        DrawCircle(transform.position, MinSpawnDistance);
        Gizmos.color = Color.red;
        DrawCircle(transform.position, MaxSpawnDistance);
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        int segments = 36;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        for (int i = 1; i <= segments; i++)
        {
            float rad = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
