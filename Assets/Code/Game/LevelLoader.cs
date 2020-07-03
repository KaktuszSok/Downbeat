using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public static class LevelLoader
{
    static List<KeyValuePair<Color32, IMapComponent>> colourBehaviours = new List<KeyValuePair<Color32, IMapComponent>>(); //Gets converted to mapLookup once set
    static ILookup<Color32, IMapComponent> mapLookup = null;

    public static Level LoadLevel(string name, bool keepPlayer = false)
    {
        System.DateTime startTime = System.DateTime.Now;
        Level level = new Level(name);
        if(keepPlayer)
        {
            level.entities.Add(LevelManager.player);
        }

        //Load map from file
        Dictionary<int, Texture2D> mapPNGs = GetMapPNGs(name);

        //Interpret level data
        ProcessLevelTxt(name);

        //Generate Map
        mapLookup = colourBehaviours.ToLookup(kvp => kvp.Key, kvp => kvp.Value);
        foreach (int height in mapPNGs.Keys)
        {
            ProcessLayer(ref level, mapPNGs[height].GetColours2D(), height);
        }

        Debug.Log("Loaded level \"" + name + "\" in " + System.DateTime.Now.Subtract(startTime).TotalMilliseconds + "ms");
        return level;
    }

    static void ProcessLayer(ref Level level, Color32[,] layerMap, int layerHeight)
    {
        //Set up layer
        MapLayer layer = new MapLayer();
        layer.height = layerHeight;
        level.map.AddLayer(layer);

        //Set up tile map
        int width = layerMap.GetLength(0);
        int length = layerMap.GetLength(1);
        Tile[,] tileMap = new Tile[width, length];
        layer.tiles = tileMap;

        //Populate layer
        for (int z = 0; z < length; z++)
        {
            for(int x = 0; x < width; x++)
            {
                Color32 pixelColour = layerMap[x, z];
                if (mapLookup.Contains(pixelColour))
                {
                    foreach(IMapComponent pixelBehaviour in mapLookup[pixelColour])
                    {
                        pixelBehaviour.AddToLevel(ref level, new IntVec(x,z,layerHeight));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Spawns the player (as an entity, data-side, not as a GO) at a specific entrance of the level.
    /// </summary>
    public static void SpawnPlayer(ref Level level, int entranceToSpawnAt)
    {
        if (level.map.entrances.ContainsKey(entranceToSpawnAt))
        {
            if (LevelManager.player == null) LevelManager.player = new PlayerEntity();
            LevelManager.player.pos = level.map.entrances[entranceToSpawnAt];
        }
        else
            Debug.LogError("Could not spawn player in level " + level.name + " at requested entrance " + entranceToSpawnAt + " as it does not exist.");

        LevelManager.player.justTeleported = true;
        level.entities.Add(LevelManager.player);
    }

    #region PNGs
    static Dictionary<int, Texture2D> GetMapPNGs(string name)
    {
        Dictionary<int, Texture2D> layers = new Dictionary<int, Texture2D>();

        //Search for PNG with just map name, so single-layer maps don't need to all end in "L0".
        string searchPath = Application.streamingAssetsPath + "/Maps/" + name + ".png";
        if(File.Exists(searchPath))
        {
            layers.Add(0, GetLayerPNG(searchPath));
        }

        //Search for any additional layers
        string[] allLayers = Directory.GetFiles(Application.streamingAssetsPath + "/Maps/", name + "L*.png");
        foreach(string layerMap in allLayers)
        {
            int layerNumber = GetLayerNumber(layerMap);
            layers.Add(layerNumber, GetLayerPNG(layerMap));
        }

        return layers;
    }
    static Texture2D GetLayerPNG(string path)
    {
        Texture2D map = SaveLoadUtil.LoadPNGFixed(path);
        return map;
    }
    static int GetLayerNumber(string layerFilePath)
    {
        string layerFileName = Path.GetFileNameWithoutExtension(layerFilePath);
        //Start with last character of the name and work backwards until it encounters an L.
        string layerNumberString = "";
        for(int i=layerFileName.Length-1; i>=0; i--)
        {
            if (layerFileName[i] != 'L') layerNumberString = layerNumberString.Insert(0, layerFileName[i].ToString());
            else break;
        }
        return int.Parse(layerNumberString);
    }
    #endregion

    #region Level.Txt Parsing
    static void ProcessLevelTxt(string name)
    {
        colourBehaviours.Clear();

        string[] lines = File.ReadAllLines(Application.streamingAssetsPath + "/Maps/" + name + ".txt");
        int lineIndex = -1;
        foreach(string line in lines)
        {
            lineIndex++;

            string[] args = GetLineArguments(line);
            if (args.Length == 0) continue; //No arguments on this line, continue to next

            switch(args[0])
            {
                case "TILE":
                    AddTileToLookup(args);
                    break;
                case "ENTRANCE":
                    AddEntrance(args);
                    break;
                case "EXIT":
                    AddExit(args);
                    break;
                default:
                    Debug.Log("Unknown line initial argument: \"" + args[0] + "\" at line " + lineIndex);
                    continue;
            }
        }
    }

    /// <summary> Process a TILE line from the level.txt and add it to the tile dictionary. </summary>
    // TILE ColourR,G,B,A TileVariant
    static void AddTileToLookup(string[] args)
    {
        Color32 colour = ParseColour(args[1]);
        TileVariant tile = Resources.Load<TileVariant>("Tiles/" + ParseString(args[2]));
        if(tile  == null)
        {
            Debug.LogWarning("Could not find TileVariant " + ParseString(args[2]) + "!");
            return;
        }
        colourBehaviours.Add(new KeyValuePair<Color32, IMapComponent>(colour, tile));
    }

    // ENTRANCE ColourR,G,B,A ID
    static void AddEntrance(string[] args)
    {
        Color32 colour = ParseColour(args[1]);
        MapEntrance entrance = new MapEntrance(int.Parse(args[2]));
        colourBehaviours.Add(new KeyValuePair<Color32, IMapComponent>(colour, entrance));
    }

    // EXIT ColourR,G,B,A Level entranceID
    static void AddExit(string[] args)
    {
        Color32 colour = ParseColour(args[1]);
        string toLevel = ParseString(args[2]);
        int entranceToUse = int.Parse(args[3]);
        MapExit exit = new MapExit(toLevel, entranceToUse);
        colourBehaviours.Add(new KeyValuePair<Color32, IMapComponent>(colour, exit));
    }

    #region Line Parsing
    static string[] GetLineArguments(string line)
    {
        List<string> args = new List<string>();

        if (line.Length == 0) return args.ToArray(); //Empty line. Return empty array.

        string[] words = line.Split(' ');
        foreach (string word in words)
        {
            if (words.Length >= 2 && word.StartsWith("//")) break; //We're starting a comment. Ignore this and onwards as rest of line is also part of the comment.

            args.Add(word);
        }

        return args.ToArray();
    }

    static Color32 ParseColour(string argument)
    {
        string[] rgba = argument.Split(',');
        return new Color32(byte.Parse(rgba[0]), byte.Parse(rgba[1]), byte.Parse(rgba[2]), byte.Parse(rgba[3]));
    }

    static string ParseString(string argument)
    {
        return argument.Replace('_', ' '); //Replace underscores with spaces
    }
    #endregion
    #endregion
}
