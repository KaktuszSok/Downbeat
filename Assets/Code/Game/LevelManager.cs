using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages Level data and communicates between the data and the game.
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public Level loadedLevel = null;

    public static PlayerEntity player;

    //Debug
    public string LevelToLoad = "Level0-0";

    private void Awake()
    {
        instance = this;

        //LoadLevel(LevelToLoad);
        EnterLevel(LevelToLoad, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Shift+R to load LevelToLoad, R alone to reload current level.
            if (Input.GetKey(KeyCode.LeftShift))
            {
                LoadLevel(LevelToLoad, false);
            }
            else
            {
                LoadLevel(loadedLevel.name, true);
            }
        }
    }

    #region Action Management

    public enum ActionResult
    {
        IGNORED,
        FAILED,
        SUCCESS
    }

    public ActionResult PlayerTakeAction(EntityAction action)
    {
        return action.Execute() ? ActionResult.SUCCESS : ActionResult.FAILED;
    }

    #endregion

    #region Level Management
    /// <returns>Successful?</returns>
    public bool MoveEntity(Entity entity, IntVec newPos, bool force = false)
    {
        if(force || CanMoveEntity(entity, newPos))
        {
            entity.pos = newPos;
            OnEntityMoved(entity);
            return true;
        }

        OnEntityFailedToMove(entity, newPos);
        return false;
    }

    public bool CanMoveEntity(Entity entity, IntVec newPos)
    {
        if (loadedLevel.IsPosOOB(newPos)) return false;

        Tile tileAtPos = loadedLevel.TileAtPos(newPos);
        if (tileAtPos == null || tileAtPos.isWalkable) //If it is air or walkable, allow movement
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Called whenever an entity moves to another tile
    /// </summary>
    public void OnEntityMoved(Entity entity)
    {
        if (entity == player && loadedLevel.map.exits.ContainsKey(entity.pos)) //Player stepped on an exit tile
        {
            MapExit exit = loadedLevel.map.exits[entity.pos];
            EnterLevel(exit.level, exit.entranceID);
        }
    }

    public void OnEntityFailedToMove(Entity entity, IntVec attemptTilePos)
    {
        if (entity == player && loadedLevel.map.exits.ContainsKey(entity.pos)) //Player stepped in an invalid direction while on an exit tile - Activate exit
        {
            MapExit exit = loadedLevel.map.exits[entity.pos];
            EnterLevel(exit.level, exit.entranceID);
        }
    }

    #endregion

    #region Loading
    public void EnterLevel(string levelName, int entrance)
    {
        Debug.Log("Entering level \"" + levelName + "\" through entrance with ID " + entrance + ".");
        LoadLevel(levelName);
        LevelLoader.SpawnPlayer(ref loadedLevel, entrance);
    }

    public void LoadLevel(string levelName, bool keepPlayer = false)
    {
        loadedLevel = LevelLoader.LoadLevel(levelName, keepPlayer);
        SpawnLevel();
    }

    void SpawnLevel()
    {
        System.DateTime startTime = System.DateTime.Now;
        ClearLevel();

        //Build Level Geometry
        foreach(int height in loadedLevel.map.mapLayers.Keys)
        {
            MapLayer layer = loadedLevel.map.mapLayers[height];
            for(int x=0;x<layer.tiles.GetLength(0);x++)
            {
                for(int z=0;z<layer.tiles.GetLength(1);z++)
                {
                    if (layer.tiles[x, z] == null) continue;

                    GameObject tileGO = new GameObject("Tile (" + x + "," + z + ", L" + height + ")");
                    tileGO.transform.SetParent(transform);
                    TileRenderer tileRend = (TileRenderer)tileGO.AddComponent(layer.tiles[x, z].renderer);
                    tileRend.LoadFromMap(loadedLevel.map, new IntVec(x,z,height));
                }
            }
        }

        Debug.Log("Spawned level \"" + loadedLevel.name + "\" geometry in " + System.DateTime.Now.Subtract(startTime).TotalMilliseconds + "ms");
    }

    void ClearLevel()
    {
        List<TileRenderer> tiles = new List<TileRenderer>(GetComponentsInChildren<TileRenderer>());
        while(tiles.Count > 0)
        {
            Destroy(tiles[0].gameObject);
            tiles.RemoveAt(0);
        }
    }
    #endregion
}

public static class LevelManagerHelper
{
    public static bool ToBool(this LevelManager.ActionResult actionResult)
    {
        switch (actionResult)
        {
            case LevelManager.ActionResult.SUCCESS:
                return true;
            default:
                return false;
        }
    }
}