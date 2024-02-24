using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Diagnostics; 
using System.Linq;

using Unity.Jobs;

public class ChunkLoader
{
	private Transform chunkParent;

	public string SavePath;
	public Vector3 midPosition;
	public vector3Int chunkMidPos;

	private ReUsableChunk[] Meshes = new ReUsableChunk[GameData.MaxChunks];
	private ReUsableChunk errorChunk; //shouldnt ever get used

	
	public int RenderDistance;//in chunks
	public int RenderDistanceLOD = 100;
	public int numChunksToLoad = 1;

	public bool unloadingChunks;

	public Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

	//chunk list for each thread
	private List<Chunk> unityChunks = new List<Chunk>();
	private List<Chunk> mainThreadChunks = new List<Chunk>();
	private List<Chunk> secondaryThreadChunks = new List<Chunk>();


	private vector3Int[] chunksToBeLoaded;
	private List<Vector3Int> unloadedChunks = new List<Vector3Int>();
	public Camera cam;

	public static int scale = 1;


	#region initilisation
		public ChunkLoader(Transform _chunkParent, string _savePath){
			RenderDistance = GameData.RenderDistance;
			chunkParent = _chunkParent;
			SavePath = _savePath;
			errorChunk = new ReUsableChunk(false, -1, chunkParent, this);
			SetMeshes();
		}

		//creates an array of chunk coords in order from closest to furthest
		public void SetChunksToLoad(){
			List<vector3Int> C = new List<vector3Int>();
			chunkMidPos = GetChunkPos(midPosition);
			for(int x = -RenderDistance; x < RenderDistance; x++){
				for(int y = -RenderDistance; y < RenderDistance; y++){
					for(int z = -RenderDistance; z < RenderDistance; z++){
						if(Vector3.Distance(new Vector3(x,y,z), Vector3.zero) <= RenderDistance){
							vector3Int pos = GetChunkPos(x*GameData.chunkSize*scale, y*GameData.chunkSize*scale, z*GameData.chunkSize*scale);
							C.Add(pos);
						}
					}
				}
			}
			chunksToBeLoaded = C.ToArray();

			Array.Sort(chunksToBeLoaded, (c1, c2) => c1.magnitude.CompareTo(c2.magnitude));
		}
	#endregion

	#region load chunks
		//load new chunk
		public void LoadChunk(Vector3Int _pos){
			//if chunk has been created but not generated
			if(chunks.ContainsKey(_pos)) 
				chunks[_pos].GenerateAndLoad();
			//if chunk has been saved
			else if(File.Exists($"{SavePath}\\Chunks\\{_pos.x},{_pos.y},{_pos.z}.cxyz"))
				chunks.Add(_pos, new Chunk(_pos, GetSave(_pos), GetAvalibleChunk(), midPosition));
			//if chunk does not exist
			else{
				chunks.Add(_pos, new Chunk(_pos, GetAvalibleChunk(), midPosition, scale));
			}
		}
		
		//load new chunk with voxel start
		/*public Chunk LoadChunk(vector3Int _pos, Voxel _voxel){
			if(!chunks.ContainsKey(_pos) && !File.Exists($"{SavePath}\\Chunks\\{_pos.x},{_pos.y},{_pos.z}.cxyz"))
					chunks.Add(_pos, new Chunk(_pos, _voxel, GetAvalibleChunk(), midPosition));
			return chunks[_pos];
		}*/

		//load the closest chunk
		public void LoadClosestChunk(){
			chunkMidPos = GetChunkPos(midPosition);
			for(int i = 0; i < chunksToBeLoaded.Length; i++){
				vector3Int pos = chunksToBeLoaded[i] + chunkMidPos;
				if(pos.x < 0 || pos.y < 0 || pos.z < 0)
					continue;
				if(!chunks.ContainsKey(pos)){
					LoadChunk(pos);
					return;
				}
				else if(!chunks[pos].loaded){
					LoadChunk(pos);
					return;
				}
			}
		}

		//load chunks in an area
		public void loadChunks(int _size, int _height, Vector3 _pos){
			for(int x = 0; x < _size; x++){
				for(int y = 0; y < _height; y++){
					for(int z = 0; z < _size; z++){
						Vector3Int pos = new Vector3Int(x*GameData.chunkSize*scale, y*GameData.chunkSize*scale, z*GameData.chunkSize*scale) + GetChunkPos(_pos);
						LoadChunk(pos);
					}
				}
			}
		}

		//loads all meshes of chunks that are generated
		public void UpdateChunksMesh(){
			chunkMidPos = GetChunkPos(midPosition);
			
			//select chunks
			for(int i = 0; i < chunksToBeLoaded.Length; i++){//chunksToBeLoaded is sorted from closest to furthest
				vector3Int _pos = chunksToBeLoaded[i] + chunkMidPos;
				//skip over negative chunks
				if(_pos.x < 0 || _pos.y < 0 || _pos.z < 0)
					continue;
				if(!chunks.ContainsKey(_pos))
					continue;
				else if(chunks[_pos].waitingForUpdate && !chunks[_pos].beingUpdated && chunks[_pos].containsVoxels){
					chunks[_pos].UpdateMeshData();
				}
			}
		}

		public void UpdateChunkLOD(){
			foreach(Chunk chunk in mainThreadChunks){
				chunk.UpdateLOD(midPosition);
			}
		}

		bool ChunkInCam(Vector3Int _chunkPos) {
			if(!chunks.ContainsKey(_chunkPos))
				return false;
			if(!chunks[_chunkPos].containsVoxels)
				return false;
			
			Rect _rect = new Rect(0, 0, 1, 1);
			for(int i = 0; i < 8; i++){
				Vector3 viewportPoint = cam.WorldToViewportPoint(GameData.CornerTable[i]*16 + _chunkPos);
				if(viewportPoint.z > 0  && (_rect.Contains(viewportPoint)))
					return true;
			}
			return false;
		}

	#endregion

	#region unload chunks
		//unload all chunks that are too far away
		public void UnloadChunks(){ 
			unloadingChunks = true;
			chunkMidPos = GetChunkPos(midPosition);
			foreach(Chunk chunk in unityChunks){
				if(!chunk.loaded) continue;
				if(Vector3.Distance(chunk.ChunkPosition, midPosition) > RenderDistance*GameData.chunkSize*1.3f){
					chunk.unload();
					unloadedChunks.Add(chunk.ChunkPosition);
				}
			}
			foreach(Vector3Int chunk in unloadedChunks){
				chunks.Remove(chunk);
			}
			unloadedChunks.Clear();
			unloadingChunks = false;
		}

		//unload all chunks
		public void UnloadAllChunks(){
			List<Chunk> UnloadChunks = chunks.Values.ToList();
			foreach(Chunk chunk in UnloadChunks){
				chunk.unload();
				unloadedChunks.Add(chunk.ChunkPosition);
			}
			foreach(Vector3Int chunk in unloadedChunks){
				chunks.Remove(chunk);
			}
			unloadedChunks.Clear();
		}
	#endregion

	//update chunks (main thread)
	public void UpdateChunks(){
		foreach(Chunk chunk in mainThreadChunks){
			chunk.Update();
		}	
	}

	public void SetUnityChunks(){
		try{
			unityChunks = chunks.Values.ToList();
		}catch{
			UnityEngine.Debug.Log("thread conflict");
		}
	}
	public void SetMainChunks(){
		try{
			mainThreadChunks = chunks.Values.ToList();
		}catch{
			UnityEngine.Debug.Log("thread conflict");
		}
	}
	public void SetSecondaryChunks(){
		try{
			secondaryThreadChunks = chunks.Values.ToList();
		}catch{
			UnityEngine.Debug.Log("thread conflict");
		}
	}



	#region chunkObject pooling
		ReUsableChunk GetAvalibleChunk(){
			for (int i = 0; i < GameData.MaxChunks; i++)
			{
				if (!Meshes[i].inUse)
				{
					Meshes[i].inUse = true;
					Meshes[i].id = i;
					return Meshes[i];
				}
			}
			UnityEngine.Debug.Log("ERROR: trying to access Mesh that isnt available");
			return errorChunk;
		}

		public void SetMeshes(){
			for(int i = 0; i < GameData.MaxChunks; i++){
				if(Meshes[i] == null) Meshes[i] = new ReUsableChunk(false,i, chunkParent, this);
			}
		}
	#endregion

	#region saves
		voxelsSave[] GetSave(Vector3Int _pos){
			List<voxelsSave> save = new List<voxelsSave>();	
			int i = 0;
			
			using(BinaryReader reader = new BinaryReader(File.Open($"{SavePath}\\Chunks\\{_pos.x},{_pos.y},{_pos.z}.cxyz", FileMode.Open))){
				while(i < GameData.chunkVoxels){
					voxelsSave s = new voxelsSave();
					s.amount = reader.ReadUInt16();
					s.type = reader.ReadByte();
					i+= s.amount;
					save.Add(s);
				}
			}
			return save.ToArray();
		}

		public static void SaveChunk(vector3Int _chunkPos, byte[] _save, string _SavePath){ //byte[] save containing all voxels in a chunk
			using(BinaryWriter writer = new BinaryWriter(File.Open($"{_SavePath}{_chunkPos.x},{_chunkPos.y},{_chunkPos.z}.cxyz", FileMode.Create)))
			{
				int i = 0;
				byte type;
				UInt16 n;
				while(i < GameData.chunkVoxels){
					n = 0;
					type = _save[i];
					while(i < GameData.chunkVoxels && type == _save[i]){
						n++;
						i++;
					}
					writer.Write(n);
					writer.Write(type);
				}
			}
		}
	#endregion

	#region coordniate stuff
		public Chunk GetChunkFromVector3(Vector3 pos){
			if(chunks.ContainsKey(GetChunkPos(pos)))
				return chunks[GetChunkPos(pos)];
			return null;
		}

		public Chunk GetChunkFromChunkPos(vector3Int pos){
			return chunks[pos];
		}

		public static vector3Int GetChunkPos(Vector3 pos){
			return new vector3Int(
			((int)pos.x/GameData.chunkSize)*GameData.chunkSize,
			((int)pos.y/GameData.chunkSize)*GameData.chunkSize,
			((int)pos.z/GameData.chunkSize)*GameData.chunkSize);
		}
		
		public static vector3Int GetChunkPos(int x, int y, int z){
			return new vector3Int(
			(x/GameData.chunkSize)*GameData.chunkSize,
			(y/GameData.chunkSize)*GameData.chunkSize,
			(z/GameData.chunkSize)*GameData.chunkSize);
		}

		public static vector3Int GetChunkPos(vector3Int pos){
			return new vector3Int(
			(pos.x/GameData.chunkSize)*GameData.chunkSize,
			(pos.y/GameData.chunkSize)*GameData.chunkSize,
			(pos.z/GameData.chunkSize)*GameData.chunkSize);
		}
	#endregion
}
