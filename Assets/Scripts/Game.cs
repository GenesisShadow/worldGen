using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.IO;
using System.Diagnostics; 
using UnityEngine.UI;
using Network;	
using Network.Enums;

public class Game : MonoBehaviour
{
	public static Game instance;

	public GameObject cursorThingo;
  	public GameObject cursorThingo2;
	public GameObject chunkParent;
	public Color sky;
	public Color skyDay;
	public Color skyNight;
	public int DayDurationSeconds;
	public AnimationCurve DayNightCycle;
	public Camera skyCam;

	public byte VoxelToPlace = 19;

    public string WorldSavePath;
	private string savePathLocation;
	public bool isRunning;
	public bool inGame;
	public Text VoxelType;

	public ChunkLoader loader;
	public VoxelController vCon;
	WorldGenerator gen;

	Player player;

	public UI_Manager UI;

	#region initilisation
		void Awake(){
			if (instance == null)
				instance = this;
			else if (instance != this)
				Destroy(this);
		}

		void Start(){	
			UI.SetMenuUI();	
			isRunning = true;
			Thread gameThread = new Thread(game);
			gameThread.Start();

			Thread gameThread2 = new Thread(Game2);
			gameThread2.Start();

			SetSky();
			Screen.fullScreen = false;
			Application.targetFrameRate = 100;

		}

		void GameStart(){
			isRunning = true;
			
		}

		public void StartGame(){
			loader = new ChunkLoader(chunkParent.transform, WorldSavePath);
			vCon = new VoxelController(loader, cursorThingo, cursorThingo2);
			//HostServer(1234);
			UI.SetGameUI();
			SetNewWorld("test world five", 5);
			player = new Player(loader, vCon);
			inGame = true;
			CodeTimer.Start();
			SetSky();


			//loader.loadChunks(3,3,new vector3Int(6144,800,6144));
			//loader.LoadChunk(new vector3Int(6144,832,6144));
		}
		void SetSky(){
			RenderSettings.fogColor = sky;
			RenderSettings.ambientLight = new Color(0.85f,0.85f,0.85f);
			skyCam.backgroundColor = sky;
		}
	#endregion

	#region Update functions
		//run at 60 ticks per second on main thread
		void UpdateGame(){
			vCon.Update();
			loader.SetMainChunks();
		}
		//runs once per second on the main thread
		void SlowUpdateGame(){
			loader.UpdateChunks();
		}

		//runs each tick on the secondary thread
		void UpdateGame2(){
			if(player != null)
				loader.LoadClosestChunk();
		}

		//runs 30 times per second on the secondary thread
		void UpdateGame2Slow(){
			if(player != null){
				loader.SetSecondaryChunks();
				//loader.UpdateChunkLOD();
			}
		}

		//runs once per frame on the unity thread
		void unityUpdate(){
			if(Input.GetKeyDown(KeyCode.P)){
				VoxelToPlace += 1;
				while(GameData.VoxelNames[VoxelToPlace] == "Unknown")
					VoxelToPlace++;
			}
			else if(Input.GetKeyDown(KeyCode.O)){
				VoxelToPlace -= 1;
				while(GameData.VoxelNames[VoxelToPlace] == "Unknown")
					VoxelToPlace--;
			}
			if(VoxelToPlace <= 0)
				VoxelToPlace = 1;
			if(VoxelToPlace >= GameData.VoxelNames.Length-1)
				VoxelToPlace = (byte)(GameData.VoxelNames.Length -1);
			
			VoxelType.text = GameData.VoxelNames[VoxelToPlace];

			loader.UpdateChunksMesh();
			//loader.LoadChunksInView();
			loader.SetUnityChunks();
		}

		//runs after unity update
		void LateUpdate(){
			if(inGame)
				vCon.LateUpdate();
			//loader.SetLoadSpeed();
		}

		//updates once per second on the unity thread
		void SlowUnityUpdate(){
			if(inGame){
				loader.UnloadChunks();
				sky = Color.Lerp(skyDay, skyNight, DayNightCycle.Evaluate(Mathf.PingPong(Time.time/DayDurationSeconds, 1)));
				SetSky();
			}
		}
	#endregion

	#region Game Loops	
		//Main thread
		void game(){
			UnityEngine.Debug.Log($"Main thread started. Running at {60} ticks per second.");
			DateTime _nextLoop = DateTime.Now;
			DateTime _nextSlowLoop = DateTime.Now;
			GameStart();
			while (isRunning){
				while(_nextLoop < DateTime.Now){
					if(inGame)
						UpdateGame();
					_nextLoop = _nextLoop.AddMilliseconds(1000/60);
					if(_nextSlowLoop < DateTime.Now){
						_nextSlowLoop = _nextSlowLoop.AddMilliseconds(1000);
						if(inGame)
							SlowUpdateGame();
					}

					if(_nextLoop > DateTime.Now)
						Thread.Sleep(_nextLoop - DateTime.Now);
				}
			}
			UnityEngine.Debug.Log($"Main thread closing");
			//if(hosting) server.Stop();
			//client.Dissconect();
		}

		//secondary thread
		void Game2(){
			UnityEngine.Debug.Log($"Secondary thread started. running as fast as possible");
			DateTime _nextLoop = DateTime.Now;
			while(isRunning){
				if(_nextLoop < DateTime.Now){
					UpdateGame2Slow();
					_nextLoop = _nextLoop.AddMilliseconds(1000/60);
				}
				
				UpdateGame2();
			}
			UnityEngine.Debug.Log($"Secondary thread closing");
		}

		// unity thread
		float elapsed = 0f;
		void Update(){
			elapsed += Time.deltaTime;
			if (elapsed >= 1f){
				elapsed = elapsed % 1f;
				SlowUnityUpdate();
			}
			if(inGame){
				if(player != null)
					player.Update();
				unityUpdate();
			}
			if(inGame){
				if(Input.GetKeyDown(KeyCode.Escape))
					SaveAndQuit();
			}
		}
	#endregion

	void CreateWorld(string name, int _seed){
        string Path = $"{savePathLocation}/Worlds/{name}/Chunks/";
        Directory.CreateDirectory(Path);
		gen = new WorldGenerator(_seed, Path);
        UnityEngine.Debug.Log("saving world at: " + Path);
    }

	void SetNewWorld(string name, int _seed){ //for generating as chunks load
		SetWorldSavePath(name);
		//save file in world save path containing world data like seed and such
		string Path = $"{WorldSavePath}/Chunks/";
        Directory.CreateDirectory(Path);
		gen = new WorldGenerator(_seed, Path);
		loader.SavePath = WorldSavePath;
        UnityEngine.Debug.Log("saving world at: " + Path);
	}

	public void SetWorldSavePath(string name){
		savePathLocation = Application.persistentDataPath;
        WorldSavePath = $"{savePathLocation}/Worlds/{name}/";
    }

	void CalculateStrcutureWidth(){
		int width = 0;
		int height = 0;
		int depth = 0;
		foreach(Voxel v in GameStructures.TreeTwelve){
			if(v.Position.x > width)
				width = v.Position.x;
			if(v.Position.y > height)
				height = v.Position.y;
			if(v.Position.z > depth)
				depth = v.Position.z;
		}
		UnityEngine.Debug.Log($"width: {width}, height: {height}, depth: {depth}");
	}

	#region quit
		void OnApplicationQuit(){
			isRunning = false;
			inGame = false;
		}
		public void SaveAndQuit(){
			UnityEngine.Debug.Log("returning to main menu");
			inGame = false;
			player.DeleteSelf();
			player = null;
			loader.UnloadAllChunks();
			UI.SetMenuUI();
			CodeTimer.CollectAverage("chunks mesh gen");
		}

		public void ExitGame(){
			Application.Quit();
		}
	#endregion
}
