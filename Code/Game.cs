// Handles most of aspects of the gameplay and controls
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField]
    GameBoard board = default;

    [SerializeField]
	GameTileContentFactory tileContentFactory = default;

    // gets where player clicked via rays
    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    GameBehaviorCollection enemies = new GameBehaviorCollection();
	GameBehaviorCollection nonEnemies = new GameBehaviorCollection();

    TowerType selectedTowerType;

    [SerializeField]
	WarFactory warFactory = default;

	static Game instance;

	[SerializeField]
	GameScenario scenario = default;

	[SerializeField, Range(0, 100)]
	int startingPlayerHealth = 10;

	GameScenario.State activeScenario;

	int playerHealth;

	const float pausedTimeScale = 0f;

	[SerializeField, Range(1f, 10f)]
	float playSpeed = 1f;

    public static Explosion SpawnExplosion () {
		Explosion explosion = instance.warFactory.Explosion;
		instance.nonEnemies.Add(explosion);
		return explosion;
	}

    public static Shell SpawnShell () {
		Shell shell = instance.warFactory.Shell;
		instance.nonEnemies.Add(shell);
		return shell;
	}

	void OnEnable () {
		instance = this;
	}

    void Awake()
    {
		playerHealth = startingPlayerHealth;
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
		activeScenario = scenario.Begin();
    }

    void BeginNewGame() {
        enemies.Clear();
        nonEnemies.Clear();
        board.Clear();
        activeScenario = scenario.Begin();
        playerHealth = startingPlayerHealth;
    }

	public static void EnemyReachedDestination () {
		instance.playerHealth -= 1;
	}

    void OnValidate() {
        if (boardSize.x < 2)
        {
            boardSize.x = 2;
        }
        if (boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }

    // checks if mouse was clicked
    void Update () {
		if (Input.GetMouseButtonDown(0)) {
			HandleTouch();
		}
        else if (Input.GetMouseButtonDown(1)) {
			HandleAlternativeTouch();
		}

        // toggles visualization arrows on 'V' key
        if (Input.GetKeyDown(KeyCode.V)) {
			board.ShowPaths = !board.ShowPaths;
		}

        // toggles grid on 'G' key
        if (Input.GetKeyDown(KeyCode.G)) {
			board.ShowGrid = !board.ShowGrid;
		}
		
		// Switches to laser towers on '1'
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
			selectedTowerType = TowerType.Laser;
		}
		// Switches to mortar towers on '2'
		else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			selectedTowerType = TowerType.Mortar;
		}

		if (playerHealth <= 0 && startingPlayerHealth > 0) {
			Debug.Log("Defeat!");
			BeginNewGame();
		}

		if (!activeScenario.Progress() && enemies.IsEmpty) {
			Debug.Log("Victory!");
			BeginNewGame();
			activeScenario.Progress();
		}

        enemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpdate();
        nonEnemies.GameUpdate();
		
		// pauses game on 'space' press
		if (Input.GetKeyDown(KeyCode.Space)) {
			Time.timeScale = Time.timeScale > pausedTimeScale ? pausedTimeScale : playSpeed;
		} else if (Time.timeScale > pausedTimeScale) {
			Time.timeScale = playSpeed;
		}

		// starts a new game on 'b' press
		if (Input.GetKeyDown(KeyCode.B)) {
			BeginNewGame();
		}
	}

    public static void SpawnEnemy (EnemyFactory factory, EnemyType type) {
		GameTile spawnPoint = instance.board.GetSpawnPoint(Random.Range(0, instance.board.SpawnPointCount));
		Enemy enemy = factory.Get(type);
		enemy.SpawnOn(spawnPoint);
        instance.enemies.Add(enemy);
	}

    // Handles right click for setting walls   
	void HandleTouch () {
		GameTile tile = board.GetTile(TouchRay);
		if (tile != null) {
			if (Input.GetKey(KeyCode.LeftShift)) {
				board.ToggleTower(tile, selectedTowerType);
			} else{
           board.ToggleWall(tile);
            }
		}
	}

    // If clicked get tile destination
    void HandleAlternativeTouch () {
		GameTile tile = board.GetTile(TouchRay);
		if (tile != null) {
            if (Input.GetKey(KeyCode.LeftShift)) {
			    board.ToggleDestination(tile);
            }
            else{
            	board.ToggleSpawnPoint(tile);
            }
		}
	}

}

