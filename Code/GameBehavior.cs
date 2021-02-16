
// Serves as a abstract gameUpdate function
using UnityEngine;

public abstract class GameBehavior : MonoBehaviour {

	public virtual bool GameUpdate () => true;

	public abstract void Recycle ();
	
}