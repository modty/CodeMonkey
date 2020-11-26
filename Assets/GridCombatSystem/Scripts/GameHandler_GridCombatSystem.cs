using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using GridPathfindingSystem;
using Cinemachine;

/// <summary>
/// 网格对战系统
/// </summary>
public class GameHandler_GridCombatSystem : MonoBehaviour {

    public static GameHandler_GridCombatSystem Instance { get; private set; }

    /// <summary>
    /// 相机更随的物体
    /// </summary>
    [SerializeField] private Transform cinemachineFollowTransform;
    /// <summary>
    /// 可视化网格
    /// </summary>
    [SerializeField] private MovementTilemapVisual movementTilemapVisual;
    /// <summary>
    /// 可视化相机
    /// </summary>
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    /// <summary>
    /// 鼠标碰撞层级
    /// </summary>
    [SerializeField] private LayerMask mouseColliderLayerMask;
    /// <summary>
    /// 鼠标的位置
    /// </summary>
    [SerializeField] private Transform mouseVisualTransform;
    /// <summary>
    /// 网格中的所有物体
    /// </summary>
    private GridXZ<GridCombatSystem.GridObject> grid;
    /// <summary>
    /// 更随角色移动的网格
    /// </summary>
    private MovementTilemap movementTilemap;
    /// <summary>
    /// 网格寻路
    /// </summary>
    public GridPathfinding gridPathfinding;

    private void Awake() {
        Instance = this;
        // 水平格子数
        int mapWidth = 50;
        // 竖直格子数
        int mapHeight = 25;
        // 每个格子的大小
        float cellSize = 10f;
        Vector3 origin = new Vector3(0, 0);

        // 数组初始化
        grid = new GridXZ<GridCombatSystem.GridObject>(
            mapWidth, mapHeight, cellSize, origin, 
            (GridXZ<GridCombatSystem.GridObject> g, int x, int y) => new GridCombatSystem.GridObject(g, x, y)
        );

        // 寻路设置
        #region Pathfinding
        //gridPathfinding = new GridPathfinding(origin + new Vector3(1, 1) * cellSize * .5f, new Vector3(mapWidth, mapHeight) * cellSize, cellSize, GridPathfinding.Axis.XY);
        gridPathfinding = new GridPathfinding(origin + new Vector3(1, 0, 1) * cellSize * .5f, new Vector3(mapWidth, 0, mapHeight) * cellSize, cellSize, GridPathfinding.Axis.XZ);
        gridPathfinding.RaycastWalkable();
        // 输出地图
//        gridPathfinding.PrintMap((Vector3 vec, Vector3 size, Color color) => {
//            World_Sprite worldSprite = World_Sprite.Create(vec, size, color);
//            worldSprite.transform.eulerAngles = new Vector3(90, 0, 0);
//        });
        #endregion
        movementTilemap = new MovementTilemap(mapWidth, mapHeight, cellSize, origin);
    }

    private void Start() {
        movementTilemap.SetTilemapVisual(movementTilemapVisual);
    }

    private void Update() {
        HandleCameraMovement();
        // 鼠标处发射射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 如果鼠标移动到鼠标可以碰撞的地方，设置光标位置
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask)) {
            mouseVisualTransform.position = raycastHit.point;
        }
    }
    /// <summary>
    /// 控制相机移动
    /// </summary>
    private void HandleCameraMovement() {
        Vector3 moveDir = new Vector3(0, 0);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            moveDir.z = +1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            moveDir.z = -1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            moveDir.x = -1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            moveDir.x = +1;
        }
        moveDir.Normalize();

        float moveSpeed = 80f;
        cinemachineFollowTransform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 获取鼠标的世界坐标（射线）
    /// </summary>
    /// <returns></returns>
    public Vector3 GetMouseWorldPosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask)) {
            Vector3 mousePosition = raycastHit.point;
            mousePosition.y = 0f;
            return mousePosition;
        }
        return Vector3.zero;
    }
    /// <summary>
    /// 获取网格
    /// </summary>
    /// <returns></returns>
    public GridXZ<GridCombatSystem.GridObject> GetGrid() {
        return grid;
    }
    /// <summary>
    /// 获取更随角色移动的网格
    /// </summary>
    /// <returns></returns>
    public MovementTilemap GetMovementTilemap() {
        return movementTilemap;
    }
    /// <summary>
    /// 设置相机移动到的位置
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetCameraFollowPosition(Vector3 targetPosition) {
        cinemachineFollowTransform.position = targetPosition;
    }

    /// <summary>
    /// 屏幕抖动
    /// </summary>
    /// <returns></returns>
    public void ScreenShake() {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 1f;
        FunctionTimer.Create(() => { cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f; }, .1f);
    }
    
    /// <summary>
    /// 没有物体的网格
    /// </summary>
    public class EmptyGridObject {

        private Grid<EmptyGridObject> grid;
        private int x;
        private int y;
        public EmptyGridObject(Grid<EmptyGridObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            // 设置网格的四个坐标
            Vector3 worldPos00 = grid.GetWorldPosition(x, y);
            Vector3 worldPos10 = grid.GetWorldPosition(x + 1, y);
            Vector3 worldPos01 = grid.GetWorldPosition(x, y + 1);
            Vector3 worldPos11 = grid.GetWorldPosition(x + 1, y + 1);
            // 设置网格边缘颜色
            Debug.DrawLine(worldPos00, worldPos01, Color.white, 999f);
            Debug.DrawLine(worldPos00, worldPos10, Color.white, 999f);
            Debug.DrawLine(worldPos01, worldPos11, Color.white, 999f);
            Debug.DrawLine(worldPos10, worldPos11, Color.white, 999f);
        }

        public override string ToString() {
            return "";
        }
    }
}
