// handles the setting of tiles and pathfinding

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
   [SerializeField]
   Transform arrow = default;

   GameTile north, east, south, west, nextOnPath;

   int distance;

    // quarternion roatations for the path arrows
    static Quaternion
		northRotation = Quaternion.Euler(90f, 0f, 0f),
		eastRotation = Quaternion.Euler(90f, 90f, 0f),
		southRotation = Quaternion.Euler(90f, 180f, 0f),
		westRotation = Quaternion.Euler(90f, 270f, 0f);

   public bool IsAlternative { get; set; }

   GameTileContent content;

   public GameTile NextTileOnPath => nextOnPath;

   public Direction PathDirection { get; private set; }

    // returns content, setter recycles prev content and positions new content
   public GameTileContent Content {
		get => content;
		set {
            Debug.Assert(value != null, "Null assigned to content!");
			if (content != null) {
				content.Recycle();
			}
			content = value;
			content.transform.localPosition = transform.localPosition;
		}
	}

    public void ShowPath() {
        if (distance == 0) {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
			nextOnPath == east ? eastRotation :
			nextOnPath == south ? southRotation :
			westRotation;
    }

    //Makes tiles to right eastern neigbors and left west
   public static void MakeEastWestNeighbors (GameTile east, GameTile west) {
       Debug.Assert(west.east == null && east.west == null, "Redefined neighbors!");
       west.east = east;
       east.west = west;
   }

    //same as MakeEastWestNeighors but for north south
   	public static void MakeNorthSouthNeighbors (GameTile north, GameTile south) {
		Debug.Assert(
			south.north == null && north.south == null, "Redefined neighbors!"
		);
		south.north = north;
		north.south = south;
	}

    //initializes path data to help decide paths 
    public void ClearPath() {
        //before a path is found the distance can be considered infinite
        distance = int.MaxValue;
        nextOnPath = null;
    }

    //turns tiles into the destination to find distance  
    public void BecomeDestination() {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

     //getter that finds if a tile has a path
   public bool HasPath => distance != int.MaxValue;

   public Vector3 ExitPoint { get; private set; }

    // Grows a path out from a tile finding dist to its neighbors
    GameTile GrowPathTo (GameTile neighbor, Direction direction) {
        // only invoked on tiles that have a path
        if (!HasPath || neighbor == null || neighbor.HasPath) {
			return null;
		}
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return neighbor.Content.BlocksPath ? null : neighbor;
    }

    public void HidePath () {
		arrow.gameObject.SetActive(false);
	}

    public GameTile GrowPathNorth () => GrowPathTo(north,Direction.South);

	public GameTile GrowPathEast () => GrowPathTo(east, Direction.West);

	public GameTile GrowPathSouth () => GrowPathTo(south, Direction.North);

	public GameTile GrowPathWest () => GrowPathTo(west, Direction.East);
}