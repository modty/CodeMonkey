using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

/// <summary>
/// 网格X轴和Z轴
/// </summary>
/// <typeparam name="TGridObject"></typeparam>
public class GridXZ<TGridObject> {

    /// <summary>
    /// 网格对象属性改变后触发事件
    /// </summary>
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    /// <summary>
    /// 变化属性，X格子数和Z格子数
    /// </summary>
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int z;
    }
    /// <summary>
    /// 水平格子数
    /// </summary>
    private int width;
    /// <summary>
    /// 竖直格子数
    /// </summary>
    private int height;
    /// <summary>
    /// 每个格子大小
    /// </summary>
    private float cellSize;
    /// <summary>
    /// 初始位置
    /// </summary>
    private Vector3 originPosition;
    /// <summary>
    /// 网格内对象
    /// </summary>
    private TGridObject[,] gridArray;

    public GridXZ(int width, int height, float cellSize, Vector3 originPosition, Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        bool showDebug = false;
        // 测试用
        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int z = 0; z < gridArray.GetLength(1); z++) {
                    debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, cellSize) * .5f, 30, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
            };
        }
    }

    /// <summary>
    /// 获取水平格子数
    /// </summary>
    /// <returns>int</returns>
    public int GetWidth() {
        return width;
    }
    /// <summary>
    /// 竖直格子数
    /// </summary>
    /// <returns>int</returns>
    public int GetHeight() {
        return height;
    }
    /// <summary>
    /// 获取每个网格大小
    /// </summary>
    /// <returns>float</returns>
    public float GetCellSize() {
        return cellSize;
    }
    /// <summary>
    /// 转换为世界坐标
    /// </summary>
    /// <param name="x">输入X方向格子数</param>
    /// <param name="z">输入Z方向格子数</param>
    /// <returns>Vector3</returns>
    public Vector3 GetWorldPosition(int x, int z) {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }
    
    /// <summary>
    /// 将传入的X，Z转换为格子坐标
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <param name="x">输出的X方向格子数</param>
    /// <param name="z">输出的Z方向格子数</param>
    public void GetXZ(Vector3 worldPosition, out int x, out int z) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    /// <summary>
    /// 设置格子物体
    /// </summary>
    /// <param name="x">X方向格子坐标</param>
    /// <param name="z">Z方向格子坐标</param>
    /// <param name="value">游戏对象</param>
    public void SetGridObject(int x, int z, TGridObject value) {
        if (x >= 0 && z >= 0 && x < width && z < height) {
            gridArray[x, z] = value;
            // 触发事件
            TriggerGridObjectChanged(x, z);
        }
    }

    /// <summary>
    /// 事件触发唤醒，并传入对象参数
    /// </summary>
    /// <param name="x">X方向格子数</param>
    /// <param name="z">Z方向格子数</param>
    public void TriggerGridObjectChanged(int x, int z) {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
    }
    /// <summary>
    /// 设置游戏物体，
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <param name="value">游戏对象</param>
    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }
    /// <summary>
    /// 根据格子数获取游戏物体,如果越界，返回默认物体
    /// </summary>
    /// <param name="x">X方向格子数</param>
    /// <param name="z">Z方向格子数</param>
    /// <returns></returns>
    public TGridObject GetGridObject(int x, int z) {
        if (x >= 0 && z >= 0 && x < width && z < height) {
            return gridArray[x, z];
        } else {
            return default(TGridObject);
        }
    }
    /// <summary>
    /// 根据世界坐标获取游戏物体
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <returns>格子中的物体</returns>
    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

}
