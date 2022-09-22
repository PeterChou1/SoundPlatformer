using UnityEngine;
using UnityEngine.Tilemaps;

public class TileLoader
{
    private const string PathHorizontal = "Tiles/rpg_maker_wooden_floor_by_ayene_chan_d587fmi_0";

    public static Tile GetTestTile()
    {
        return GetTileByName(PathHorizontal);
    }
    private static Tile GetTileByName(string name)
    {
        return (Tile) Resources.Load(name, typeof(Tile));
    }
}