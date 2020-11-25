using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTilemap {

    /// <summary>
    /// 加载完成后事件
    /// </summary>
    public event EventHandler OnLoaded;
    /// <summary>
    /// 网格对象+ 
    /// </summary>
    private GridXZ<TilemapObject> grid;

    /// <summary>
    /// 移动网格（重新初始化一张网格）
    /// </summary>
    /// <param name="width">长度</param>
    /// <param name="height">高度</param>
    /// <param name="cellSize">大小</param>
    /// <param name="originPosition">初始坐标</param>
    public MovementTilemap(int width, int height, float cellSize, Vector3 originPosition) {
        grid = new GridXZ<TilemapObject>(width, height, cellSize, originPosition, (GridXZ<TilemapObject> g, int x, int y) => new TilemapObject(g, x, y));
    }
    /// <summary>
    /// 设置指定世界坐标上物体的图片精灵类型
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <param name="tilemapSprite">图片精灵类型</param>
    public void SetTilemapSprite(Vector3 worldPosition, TilemapObject.TilemapSprite tilemapSprite) {
        TilemapObject tilemapObject = grid.GetGridObject(worldPosition);
        if (tilemapObject != null) {
            tilemapObject.SetTilemapSprite(tilemapSprite);
        }
    }
    /// <summary>
    /// 设置指定格子中的物体的图片精灵类型
    /// </summary>
    /// <param name="x">格子中的X坐标</param>
    /// <param name="y">格子中的X坐标</param>
    /// <param name="tilemapSprite">图片精灵类型</param>
    public void SetTilemapSprite(int x, int y, TilemapObject.TilemapSprite tilemapSprite) {
        TilemapObject tilemapObject = grid.GetGridObject(x, y);
        if (tilemapObject != null) {
            tilemapObject.SetTilemapSprite(tilemapSprite);
        }
    }
    /// <summary>
    /// 用图片精灵初始化所有游戏物体
    /// </summary>
    /// <param name="tilemapSprite">图片精灵类型</param>
    public void SetAllTilemapSprite(TilemapObject.TilemapSprite tilemapSprite) {
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                SetTilemapSprite(x, y, tilemapSprite);
            }
        }
    }

    public void SetTilemapVisual(MovementTilemapVisual tilemapVisual) {
        tilemapVisual.SetGrid(this, grid);
    }



    /*
     * Save - Load
     * */
    public class SaveObject {
        public TilemapObject.SaveObject[] tilemapObjectSaveObjectArray;
    }

    public void Save() {
        List<TilemapObject.SaveObject> tilemapObjectSaveObjectList = new List<TilemapObject.SaveObject>();
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                TilemapObject tilemapObject = grid.GetGridObject(x, y);
                tilemapObjectSaveObjectList.Add(tilemapObject.Save());
            }
        }

        SaveObject saveObject = new SaveObject { tilemapObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() };

        SaveSystem.SaveObject(saveObject);
    }

    public void Load() {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
        foreach (TilemapObject.SaveObject tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray) {
            TilemapObject tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }



    /*
     * 在网格格子中的对象
     * */
    public class TilemapObject {
        
        /// <summary>
        /// 网格对象图片精灵的类型
        /// </summary>
        public enum TilemapSprite {
            None,
            Move,
        }
        /// <summary>
        /// 对象所在的网格
        /// </summary>
        private GridXZ<TilemapObject> grid;
        // 网格中X坐标
        private int x;
        // 网格中Y坐标
        private int y;
        // 当前图片精灵类型
        private TilemapSprite tilemapSprite;
        /// <summary>
        /// 初始化一个对象
        /// </summary>
        /// <param name="grid">网格地图</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        public TilemapObject(GridXZ<TilemapObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// 设置图片精灵类型，并触发改变事件
        /// </summary>
        /// <param name="tilemapSprite"></param>
        public void SetTilemapSprite(TilemapSprite tilemapSprite) {
            this.tilemapSprite = tilemapSprite;
            grid.TriggerGridObjectChanged(x, y);
        }
        /// <summary>
        /// 获取图片精灵类型
        /// </summary>
        /// <returns></returns>
        public TilemapSprite GetTilemapSprite() {
            return tilemapSprite;
        }

        public override string ToString() {
            return tilemapSprite.ToString();
        }


        
        [System.Serializable]
        public class SaveObject {
            public TilemapSprite tilemapSprite;
            public int x;
            public int y;
        }
        /*
         * 保存或加载
         * */
        public SaveObject Save() {
            return new SaveObject { 
                tilemapSprite = tilemapSprite,
                x = x,
                y = y,
            };
        }

        public void Load(SaveObject saveObject) {
            tilemapSprite = saveObject.tilemapSprite;
        }
    }

}
