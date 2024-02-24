using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelController{

 	public LayerMask layerMask;
	private ChunkLoader loader;

	public GameObject cursorThingo;
  	public GameObject cursorThingo2;

	List<Chunk> updatedChunks = new List<Chunk>();

	public VoxelController(ChunkLoader _loader, GameObject c1, GameObject c2){
		cursorThingo = c1;
		cursorThingo2 = c2;
		layerMask = LayerMask.GetMask("terrain");
		loader = _loader;
	}

	public void LateUpdate(){
		//reset all updated chunks meshes
		for(int i = 0; i < updatedChunks.Count; i ++){
			updatedChunks[i].ResetChunkMesh();
		}
		updatedChunks.Clear();
	}

	public void Update(){
		
	}

	public void ShowPlace(Transform cam){
		//position pointers
		if(Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 10, layerMask)){
				cursorThingo.SetActive(true);
				cursorThingo.transform.position = closestVoxelPoint(true,hit, cam);

				cursorThingo2.SetActive(true);
				cursorThingo2.transform.position = closestVoxelPoint(false,hit, cam);
		}else{
			cursorThingo.SetActive(false);
			cursorThingo2.SetActive(false);
		}
	}

	vector3Int closestVoxelPoint(bool place, RaycastHit hit, Transform Cam){
		Vector3 point = hit.point;
		vector3Int hitPoint = new vector3Int(point - (Cam.forward/4));
		vector3Int closest = hitPoint;
		//float minDist = Mathf.Infinity;

		if(place){
			/*for(int i = 0; i < 27; i++){
				Vector3Int newPoint = hitPoint + GameData.AdjacentPoints[i];
				Chunk chunk = loader.GetChunkFromVector3(newPoint);
				if(chunk == null)
          			continue;
				float dist = Vector3.Distance(newPoint, point);
				byte voxel = chunk.GetVoxel(newPoint - chunk.ChunkPosition);

				if(dist < minDist && voxel < 128){
					closest = newPoint;
					minDist = dist;
				}
			}*/
			return new vector3Int(Mathf.RoundToInt(point.x + (hit.normal.x/2)),Mathf.RoundToInt(point.y + (hit.normal.y/2)),Mathf.RoundToInt(point.z + (hit.normal.z/2)));
		}else{
			return new vector3Int(Mathf.RoundToInt(point.x - (hit.normal.x/4)),Mathf.RoundToInt(point.y - (hit.normal.y/4)),Mathf.RoundToInt(point.z - (hit.normal.z/4)));
		}
		//return closest;
	}

	public void PlaceTerrain(byte PlaceType, float Reach, Transform cam){
		RaycastHit hit;
		if(Physics.Raycast(cam.position, cam.forward, out hit, Reach, layerMask)){
			if(hit.transform.tag == "terrain"){
				vector3Int pos = closestVoxelPoint(true,hit, cam);
				Addterrain(PlaceType, pos);
			}
		}
	}

	public void RemoveTerrain(int Size, float Reach, Transform cam){
		RaycastHit hit;
		if(Physics.Raycast(cam.position, cam.forward, out hit, Reach, layerMask)){
			if(hit.transform.tag == "terrain"){
				vector3Int CentrePoint = closestVoxelPoint(false,hit, cam);

				for(int x = -Size; x < Size; x++){
					for(int y = -Size; y < Size; y++){
						for(int z = -Size; z < Size; z++){
							if(Vector3.Distance(new Vector3Int(x,y,z) + CentrePoint, CentrePoint) < Size){
								Vector3Int pos = CentrePoint + new Vector3Int(x,y,z);
								Taketerrain(pos);
							}
						}
					}
				}

			}
		}
	}



	public void Addterrain(byte PlaceType, vector3Int PlacePoint){
		//update main chunk
		Chunk chunk = loader.GetChunkFromVector3(PlacePoint);
		chunk.PlaceTerrain(PlaceType, PlacePoint);
		AddChunkToUpdate(chunk);
		//loop through all adjacent chunks
		Vector3Int chunkPos = ChunkLoader.GetChunkPos(PlacePoint);
		for(int i = 0; i < 26; i++){
			Vector3Int adj = GameData.AdjacentPoint[i] * GameData.chunkSize;
			Vector3Int newChunkPos = adj+chunkPos;

			if((PlacePoint.x >= newChunkPos.x && PlacePoint.x <= newChunkPos.x+GameData.chunkSize)
			&& (PlacePoint.y >= newChunkPos.y && PlacePoint.y <= newChunkPos.y+GameData.chunkSize)
			&& (PlacePoint.z >= newChunkPos.z && PlacePoint.z <= newChunkPos.z+GameData.chunkSize)){
			Chunk _chunk = loader.GetChunkFromChunkPos(newChunkPos);
			if(_chunk != null){
				_chunk.PlaceTerrain(PlaceType, PlacePoint);
				AddChunkToUpdate(_chunk);
			}else
				Debug.Log($"error : no chunk at {newChunkPos}");
				//load chunk
			}
		}
	}

	public void Taketerrain(vector3Int PlacePoint){
		//update main chunk
		Chunk chunk = loader.GetChunkFromVector3(PlacePoint);
		chunk.RemoveTerrain(PlacePoint);
		AddChunkToUpdate(chunk);
		//loop through all adjacent chunks
		Vector3Int chunkPos = ChunkLoader.GetChunkPos(PlacePoint);
		for(int i = 0; i < 26; i++){
			Vector3Int adj = GameData.AdjacentPoint[i] * GameData.chunkSize;
			Vector3Int newChunkPos = adj+chunkPos;

			if((PlacePoint.x >= newChunkPos.x && PlacePoint.x <= newChunkPos.x+GameData.chunkSize)
			&& (PlacePoint.y >= newChunkPos.y && PlacePoint.y <= newChunkPos.y+GameData.chunkSize)
			&& (PlacePoint.z >= newChunkPos.z && PlacePoint.z <= newChunkPos.z+GameData.chunkSize)){
			Chunk _chunk = loader.GetChunkFromChunkPos(newChunkPos);
			if(_chunk != null){
				_chunk.RemoveTerrain(PlacePoint);
				AddChunkToUpdate(_chunk);
			}else
				Debug.Log($"error : no chunk at {newChunkPos}");
				//load chunk
			}
		}
	}


	void AddChunkToUpdate(Chunk chunk){
		if(!updatedChunks.Contains(chunk))
			updatedChunks.Add(chunk);
	}

}
