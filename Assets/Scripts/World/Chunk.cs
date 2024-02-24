using System;
using System.Collections.Generic;
using UnityEngine;


// octree node has 8 cubes with 18 points a 3x3x3 point cube or 2x2x2 cubes
// each node will share 17 of its points with neighboring nodes
// store each point as a integer reference to a value in 
// while generating chunk points insert into octree



class OctreeNode{
	OctreeNode parent;
	OctreeNode[] children;

	byte[,,] voxels;
	
}


public class Chunk
{
    public byte[,,] Voxels;
    public Vector3Int ChunkPosition;
    public int chunkId;

    List<int> trianglesA = new List<int>();
	List<int> trianglesB = new List<int>();
	List<int> trianglesC = new List<int>();
    List<Vector3> vertices = new List<Vector3>();
    List<Color> colours = new List<Color>();
	//private byte[,,] lightValues;

    public ReUsableChunk chunk;
	private ChunkLoader loader;
	System.Random random = new System.Random();

	public bool loaded = false;
	private bool generated;
	private bool modified;
	public bool updated;
	public bool containsVoxels;


	//thread safe variables
	public bool waitingForUpdate = false;
	public bool beingUpdated = false;
	public bool buildingMesh = false;

    //get from GameData
    Color[] colourTypes { get { return GameData.VoxelTypes; } }
    byte chunkSize { get { return GameData.chunkSize; } }
	int sChunkSize { get { return GameData.actualChunkSize; } } 
	public int chunkScale = 2;

	#region Initialisation

		
		public Chunk(Vector3Int _position, voxelsSave[] save, ReUsableChunk _object, Vector3 _mid){
			chunkScale = 1;
			SetChunkObject(_position, _object, _mid);

			SetChunkPointsFromSave(save);
			BackgroundCreateMeshData();
			modified = false;
		}

		//
		/*public Chunk(vector3Int _position, Voxel voxel, ReUsableChunk _object, Vector3 _mid){
			SetChunkObject(_position, _object, _mid);
			Voxels[voxel.Position.x,voxel.Position.y,voxel.Position.z] = voxel.VoxelType;
			modified = false;
		}*/

		public Chunk(Vector3Int _position, ReUsableChunk _object, Vector3 _mid, int _chunkScale){
			chunkScale = _chunkScale;
			SetChunkObject(_position, _object, _mid);

			GenerateChunkPoints(chunk.loader);
			BackgroundCreateMeshData();
			modified = false;
		}

		public void GenerateAndLoad(){
			GenerateChunkPoints(chunk.loader);
			BackgroundCreateMeshData();
		}

		void SetChunkObject(Vector3Int _position, ReUsableChunk _object, Vector3 _mid){
			Voxels = new byte[sChunkSize,sChunkSize,sChunkSize];
			chunk = _object;
			chunkId = _object.id;
			ChunkPosition = _position;
			UpdateLOD(_mid);
			//chunk.chunkObject.transform.position = _position;
		}
	#endregion

	#region Create Mesh
		void CreateMeshData(){
			ClearMeshData(false);

			//MarchChunk();
			if(MarchOctree(new vector3Int(0), chunkSize) > 0)
				BuildMesh();
		}

		void BackgroundCreateMeshData(){
			beingUpdated = true;
			ClearMeshData(true);

			//MarchBruteForce();
			MarchOctree(new vector3Int(0), chunkSize);
			waitingForUpdate = true;
			beingUpdated = false;
		}

		public void UpdateMeshData(){
			if(!beingUpdated){
				chunk.ClearMesh();
				BuildMesh();
				chunk.chunkObject.transform.position = ChunkPosition;
				waitingForUpdate = false;
			}
		}

		void MarchBruteForce(){
			for(int x = 0; x < GameData.chunkSize; x++){
				for(int y = 0; y < GameData.chunkSize; y++){
					for(int z = 0; z < GameData.chunkSize; z++){
						MarchAllCubes(new vector3Int(x,y,z));
					}
				}
			}
		}

		int MarchOctree(vector3Int begin, int nodeSize){
			if(nodeSize == 1){
				MarchAllCubes(begin);
				return 1;
			}
			byte same = Voxels[begin.x, begin.y, begin.z];
			
			for(int x = 0; x < nodeSize + 1; x++){
				for(int y = 0; y < nodeSize + 1; y++){
					for(int z = 0; z < nodeSize + 1; z++){
						if(Voxels[begin.x + x, begin.y + y, begin.z + z] != same){
							int h = nodeSize/2;
							MarchOctree(new vector3Int(0,0,0) + begin, h);
							MarchOctree(new vector3Int(0,h,0) + begin, h);
							MarchOctree(new vector3Int(h,0,0) + begin, h);
							MarchOctree(new vector3Int(h,h,0) + begin, h);
							MarchOctree(new vector3Int(0,0,h) + begin, h);
							MarchOctree(new vector3Int(0,h,h) + begin, h);
							MarchOctree(new vector3Int(h,0,h) + begin, h);
							MarchOctree(new vector3Int(h,h,h) + begin, h);
							return 1;
						}
					}
				}
			}
			return 0;
		}

		void ClearMeshData(bool delayClear){
			if(!delayClear)
				chunk.ClearMesh();
			trianglesA.Clear();
			trianglesB.Clear();
			trianglesC.Clear();
			vertices.Clear();
			colours.Clear();
		}

		public void ResetChunkMesh(){
			CreateMeshData();
		}

		void BuildMesh(){
			buildingMesh = true;
			chunk.chunkObject.SetActive(true);

			chunk.mesh.vertices = vertices.ToArray();
			chunk.mesh.SetTriangles(trianglesA, 0);
			chunk.mesh.SetTriangles(trianglesB, 1);
			chunk.mesh.SetTriangles(trianglesC, 2);

			chunk.mesh.colors = colours.ToArray();
			chunk.mesh.RecalculateNormals();
			chunk.meshFilter.mesh = chunk.mesh;

			//if(chunkScale == 1){
				chunk.colliderMesh.vertices = vertices.ToArray();
				chunk.colliderMesh.SetTriangles(trianglesA,0);
				chunk.colliderMesh.SetTriangles(trianglesB,1);
				chunk.colliderMesh.RecalculateNormals();
				chunk.colliderMesh.Optimize();
				chunk.collider.sharedMesh = chunk.colliderMesh;
			//}
			

			chunk.mesh.Optimize();
			

			/*MeshUtility.Optimize(chunk.mesh);
			MeshUtility.Optimize(chunk.colliderMesh);*/
			buildingMesh = false;
		}
	#endregion

    #region Marching Cubes - mesh generation
		//
		void MarchCube(Vector3Int _position, bool[] _cube, int _submesh){
			//texture seed: add positions results in cool stripes
			//System.Random random = new System.Random((int)((ChunkPosition.x + 1 + position.x) * (ChunkPosition.y + 1 + position.y) * (ChunkPosition.z + 1 + position.z)));
			//random = new System.Random((int)((ChunkPosition.x + 1 + position.x) * (ChunkPosition.y + 1 + position.y) * (ChunkPosition.z + 1 + position.z)));

			float randomSeed = ((ChunkPosition.x + _position.x) * (ChunkPosition.y + _position.y) * (ChunkPosition.z + _position.z));

			byte configIndex = GetCubeConfiguration(_cube);
			if (configIndex == 0 || configIndex == 255)
				return;

			byte edgeIndex = 0;
			for (byte t = 0; t < 5; t++){ // for max amout of triangles
				//offset is used to change the colour of each triangle
				float offset = ((randomSeed*=3.14f) % 50) / 50;
				Vector3[] tri = new Vector3[3];
				for (byte j = 0; j < 3; j++){
					int indice = GameData.TriangleTable[configIndex, edgeIndex];
					if (indice == -1)
						return;
					//get the two points of the edge and the midpoint
					Vector3 vert1 = _position*chunkScale + (GameData.EdgeTable[indice, 0]*chunkScale);
					Vector3 vert2 = _position*chunkScale + (GameData.EdgeTable[indice, 1]*chunkScale);
					Vector3 vertPosition = (vert1 + vert2) / 2;
					//add the points to the list for mesh
					
					tri[j] = vertPosition;
					edgeIndex++;
				}

				Vector3[] midTri = new Vector3[]{
					(tri[0] + tri[1])/2,
					(tri[1] + tri[2])/2,
					(tri[2] + tri[0])/2
				};

				AddTri(tri, _position*chunkScale, _submesh, offset);
			}
		}

		void AddTri(Vector3[] tri, vector3Int position, int submesh, float offset){
			byte colour = GetColor(tri, position, submesh);
			for(int i = 0; i < 3; i++){
				vertices.Add(tri[i]);
				switch(submesh){
					case 0:
						trianglesA.Add(vertices.Count - 1);
						break;
					case 1:
						trianglesB.Add(vertices.Count - 1);
						break;
					case 2:
						trianglesC.Add(vertices.Count - 1);
						break;
				}
			}
			for(int v = 0; v < 3; v++){
				SetColour(colour, offset, position.y);
			}
		}
		byte GetColor(Vector3[] tri, vector3Int position, int submesh){
			//work out what the normal of triangle is
			Vector3 a = tri[1] - tri[0];
			Vector3 b = tri[2] - tri[0];
			Vector3 normal = Vector3.Cross(b,a);
			//add colours to mesh (each vertex stores a colour value)
			Vector3 midPoint = (tri[0] + tri[1] + tri[2])/3;
			float dist = Mathf.Infinity;
			vector3Int c = position;
			for(int i = 0; i < 8; i++){
				float d = Vector3.Distance(midPoint + normal, position + GameData.CornerTable[i]*chunkScale);
				if(d < dist){
					dist = d;
					c = position + GameData.CornerTable[i]*chunkScale;
				}
			}
			vector3Int _c = position/chunkScale;
			byte colour = Voxels[_c.x, _c.y, _c.z] <= 128 ? Voxels[_c.x, _c.y, _c.z] : (byte)(Voxels[_c.x, _c.y, _c.z] - 128);
				
			//submesh 2 is liquids
			if(submesh == 2){
				if(colour != 13 && colour != 141 && colour != 18 && colour != 146){
					for(int u = 0; u < 8; u++){
						int X = _c.x + GameData.cTable[u].x;
						int Y = _c.y + GameData.cTable[u].y;
						int Z = _c.z + GameData.cTable[u].z;
						X = X > sChunkSize ? _c.x : X;
						Y = Y > sChunkSize ? _c.y : Y;
						Z = Z > sChunkSize ? _c.z : Z;
						byte vox = Voxels[X,Y,Z];
						if(vox == 13 || vox == 141 || vox == 18 || vox == 146){
							colour = vox >= 128 ? (byte)(vox - 128) : vox;
						}
					}
				}
			}

			//submesh is ores
			if(submesh == 1){
				if((colour < 228 && colour > 128) || colour < 100 || colour == 0 || colour == 128){
					for(int u = 0; u < 8; u++){
						int X = _c.x + GameData.cTable[u].x;
						int Y = _c.y + GameData.cTable[u].y;
						int Z = _c.z + GameData.cTable[u].z;
						X = X > sChunkSize ? _c.x : X;
						Y = Y > sChunkSize ? _c.y : Y;
						Z = Z > sChunkSize ? _c.z : Z;
						byte vox = Voxels[X,Y,Z];
						if(vox >= 228 || (vox >= 100 && vox < 128)){
							colour = vox >= 128 ? (byte)(vox - 128) : vox;
						}
					}
				}
			}

			//if drawing liquid on non liquid mesh
			if(submesh == 0){
				if(colour == 13 || colour == 18 || colour >= 100 || colour == 0 || colour == 128){
					for(int u = 0; u < 8; u++){
						int X = _c.x + GameData.cTable[u].x;
						int Y = _c.y + GameData.cTable[u].y;
						int Z = _c.z + GameData.cTable[u].z;
						X = X > sChunkSize ? _c.x : X;
						Y = Y > sChunkSize ? _c.y : Y;
						Z = Z > sChunkSize ? _c.z : Z;
						byte vox = Voxels[X,Y,Z] >= 128 ? (byte)(Voxels[X,Y,Z] - 128) : Voxels[X,Y,Z];
						if(vox != 13 && vox != 0 && vox != 18 && vox < 100){
							colour = vox > 128 ? (byte)(vox - 128) : vox;
							break;
						}
					}
				}
			}
			return colour;
		}

		byte GetCubeConfiguration(bool[] cube){
			int ConfigurationIndex = 0;
			//for each point on the cube
			for (int i = 0; i < 8; i++){
				if (!cube[i]){
					//if the point is filled in set correspponding bit to true
					ConfigurationIndex |= 1 << i; //c = c | (1<<i)
				}
			}
			return (byte)ConfigurationIndex;
		}

		void SetColour(byte color, float offset, int y){
			offset /= 5;
			Color colour = new Color(0,0,0,0);
			Color Standard = new Color((offset / 7) - 0.1f, (offset / 7) - 0.1f, (offset / 7) - 0.1f);
			// no texture
			if (color == 8)               
				colour = colourTypes[color];

			
			//tungstone
			else if(color == 104)
				colour = colourTypes[color] + new Color(-offset/1.5f,-offset/2f,-offset/2.25f);

			// hellstone
			else if (color == 11)                        
				colour = (colourTypes[color] + new Color(-offset/1.5f, 0, 0));
						
			// wood planks
			else if (color == 10)                        
				colour = (colourTypes[color]
						+ new Color((float)(y % 2) / -7, (float)(y % 2) / -7, (float)(y % 2) / -7) + Standard / -5);

			// light standard texture
			else if(color == 109 || color == 107){                                       
				colour = (colourTypes[color] + Standard/5);
			}

			// dark standard texture
			else if(color == 17){                                       
				colour = (colourTypes[color] + Standard*1.5f);
			}

			// standard texture
			else{                                       
				if(color > colourTypes.Length)UnityEngine.Debug.Log(color);
					colour = (colourTypes[color] + Standard);
			}

			
			//Color32 colour32 = new Color32();
			colour.r = Mathf.Clamp(colour.r, 0, 255);
			colour.g = Mathf.Clamp(colour.g, 0, 255);
			colour.b = Mathf.Clamp(colour.b, 0, 255);
			colours.Add(colour);
		}

		void MarchAllCubes(vector3Int _pos){
			bool[] cubeT = new bool[8];
			bool[] cubeO = new bool[8];
			bool[] cubeL = new bool[8];
			for (int i = 0; i < 8; i++)
			{
				Vector3Int corner = _pos + GameData.CornerTable[i];
				byte voxel = Voxels[corner.x, corner.y, corner.z];
				//terrain
				cubeT[i] = ((voxel & 0b10000000) == 0b10000000); // all voxels except water and ores
				if(voxel >= 228 || voxel == 141 || voxel == 146) cubeT[i] = false;

				//ores
				cubeO[i] = ((voxel & 0b10000000) == 0b10000000); //>228 is ores
				if(voxel <= 227 || voxel == 141 || voxel == 146) cubeO[i] = false;

				//liquids
				cubeL[i] = ((voxel & 0b10000000) == 0b10000000); //liquids
				if(voxel != 141 && voxel != 146) cubeL[i] = false;
			}
			MarchCube(_pos, cubeT, 0);
			MarchCube(_pos, cubeO, 1);
			MarchCube(_pos, cubeL, 2);
		}
	#endregion

	#region ChunkModification
		public void PlaceTerrain(byte placeType, Vector3 pos){
			Vector3Int v3Int = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
			v3Int -= ChunkPosition;
			if(Voxels[v3Int.x, v3Int.y, v3Int.z] < 128)
				Voxels[v3Int.x, v3Int.y, v3Int.z] = (byte)(128 + placeType);
			modified = true;
		}

		public void PlaceTerrain(byte placeType, Vector3Int pos){
			vector3Int p = pos;
			p -= ChunkPosition;
			if(Voxels[p.x, p.y, p.z] < 128)
				Voxels[p.x, p.y, p.z] = (byte)(128 + placeType);
			modified = true;
		}

		public void RemoveTerrain(Vector3 pos){
			Vector3Int v3Int = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
			v3Int -= ChunkPosition;
			Voxels[v3Int.x, v3Int.y, v3Int.z] = (byte)0;
			modified = true;
		}

		public void RemoveTerrain(Vector3Int pos){
			pos -= ChunkPosition;
			Voxels[pos.x, pos.y, pos.z] = (byte)0;
			modified = true;
		}

		public byte GetVoxel(vector3Int p){
			return Voxels[p.x,p.y,p.z];
		}

		public byte GetVoxel(Vector3 p){
			return Voxels[(int)p.x,(int)p.y,(int)p.z];
		}

		public void SetVoxel(vector3Int pos, byte value){
			Voxels[pos.x,pos.y,pos.z] = value;
		}
	#endregion

	#region Chunk Generation
		void SetChunkPointsFromSave(voxelsSave[] save){
			containsVoxels = false;
			Voxels = new byte[chunkSize + 1, chunkSize + 1, chunkSize + 1];
			int currentVoxel = 0;
			int n = 0;
			byte type;
			for(int i = 0; i < save.Length; i++){
				n = save[i].amount;
				type = save[i].type;
				if(type >= 128) containsVoxels = true;
				for(int j = 0; j < n; j++){
					Vector3Int pos = WorldGenerator.intToXyz(currentVoxel);
					Voxels[pos.x,pos.y,pos.z] = type;
					currentVoxel++;
				}
			}
			loaded = true;
		}

		void GenerateChunkPoints(ChunkLoader loader){
			SetNoiseFrequency();
			containsVoxels = false;
			List<vector3Int> treeSeeds = new List<vector3Int>();

			CodeTimer.Start();
			float[] HillSet = WorldGenerator.instance.HillNoise.GetNoiseSet(ChunkPosition.x, 0, ChunkPosition.z, sChunkSize, 1, sChunkSize);
			float[] TreeSet = WorldGenerator.instance.TreeNoise.GetNoiseSet(ChunkPosition.x, 0, ChunkPosition.z, sChunkSize, 1, sChunkSize);


			float[] caveSet = new float[GameData.chunkVoxels];
			float[] DarkStoneSet = new float[GameData.chunkVoxels];

			float[] IronSet = new float[GameData.chunkVoxels];
			float[] CopperSet = new float[GameData.chunkVoxels];
			float[] GoldSet = new float[GameData.chunkVoxels];
			if(ChunkPosition.y < GameData.terrainAmplitude + GameData.layer3Height*chunkSize){
				caveSet = WorldGenerator.instance.CaveNoise.GetNoiseSet(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z, sChunkSize, sChunkSize, sChunkSize);
				
				CopperSet = WorldGenerator.instance.CopperNoise.GetNoiseSet(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z, sChunkSize, sChunkSize, sChunkSize);
				IronSet = WorldGenerator.instance.IronNoise.GetNoiseSet(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z, sChunkSize, sChunkSize, sChunkSize);
				GoldSet = WorldGenerator.instance.GoldNoise.GetNoiseSet(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z, sChunkSize, sChunkSize, sChunkSize);

				if(ChunkPosition.y < GameData.layer3Height*chunkSize)
					DarkStoneSet = WorldGenerator.instance.DarkStoneNoise.GetNoiseSet(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z, sChunkSize, sChunkSize, sChunkSize);
			}
			CodeTimer.StopAverage();

			for(int i = 0; i < GameData.chunkVoxels; i++){
				vector3Int pos = WorldGenerator.intToXyz(i);
				vector3Int iPos = pos + ChunkPosition;
				float hillHeight = (HillSet[pos.x*sChunkSize+pos.z]+1)*GameData.terrainAmplitude/2 + GameData.layer3Height*chunkSize;

				//set caves
				if(iPos.y < hillHeight - 32){
					//add Ore
					if(caveSet[i] > GameData.caveSurface - 0.07f){
						if(CopperSet[i] > 0.65f)
							Voxels[pos.x,pos.y,pos.z] = (byte)(VOXELS.copper + 128); //228 = 100 = copper
						else if(IronSet[i] > 0.65f)
							Voxels[pos.x,pos.y,pos.z] = (byte)(VOXELS.iron + 128); //229 = 101 = iron
						else if(GoldSet[i] > 0.7f)
							Voxels[pos.x,pos.y,pos.z] = (byte)(VOXELS.gold + 128);	//230 = 102 = gold

						//add darkstone
						else if(iPos.y < GameData.layer3Height*chunkSize && DarkStoneSet[i] > GameData.caveSurface)
							Voxels[pos.x,pos.y,pos.z] = (byte)(VOXELS.darkStone) | 0b10000000; //145 = 17 = darkstone

						//add stone
						else if(caveSet[i] > GameData.caveSurface)
							Voxels[pos.x,pos.y,pos.z] = (byte)(VOXELS.stone + 128); //147 = 19 = stone
					}
				}

				//set caves dirt
				else if(iPos.y < hillHeight - 2 && caveSet[i] > GameData.caveSurface){
					Voxels[pos.x,pos.y,pos.z] = (byte)(VOXELS.dirt  + 128); //2 : dirt
				}

				//set grass
				else if(iPos.y < hillHeight + 3 && caveSet[i] > GameData.caveSurface){
					Voxels[pos.x,pos.y,pos.z] = (byte)VOXELS.grass | 0b10000000; //1 : grass
				}

				//spawn trees
				if(pos.x > 0 && pos.y > 0 && pos.z > 0){
					if(TreeSet[pos.x*sChunkSize+pos.z] > 0.99f){
						treeSeeds.Add(iPos);
					}
				}
				if(Voxels[pos.x,pos.y,pos.z] >= 128)
					containsVoxels = true;
			}
			for(int i = 0; i < treeSeeds.Count; i++){
				//PasteStructure(GameStructures.GreenTrees[0], treeSeeds[i], loader);
			}

			//finalize
			loaded = true;
		}
		
		private void PasteStructure(structure _structure, vector3Int _pos, ChunkLoader loader){
			//get overlapping chunks


			//loop through all voxels and for each chunk check if it overlaps then place
			//if it is place voxel
		}

		private void SetNoiseFrequency(){
			WorldGenerator.instance.CaveNoise.SetFrequency(0.02f);
			WorldGenerator.instance.HillNoise.SetFrequency(0.0015f);
			WorldGenerator.instance.CopperNoise.SetFrequency(0.047f);
			WorldGenerator.instance.IronNoise.SetFrequency(0.047f);
			WorldGenerator.instance.GoldNoise.SetFrequency(0.04f);
			WorldGenerator.instance.DarkStoneNoise.SetFrequency(0.02f);
		}
	#endregion
	

	public void UpdateLOD(Vector3 midPos){
		//check position from player
		/*float playerDist = (ChunkPosition + new vector3Int(chunkSize/2) - midPos).magnitude;
		int scale = 1;
		if(playerDist < chunkSize*2)
			scale = 1;
		else if(playerDist < chunkSize* 4)
			scale = 2;
		else if(playerDist < chunkSize* 6)
			scale = 4;
		else if(playerDist < chunkSize* 8)
			scale = 8;
		else if(playerDist < chunkSize* 10)
			scale = 16;
		if(scale != chunkScale){
			chunkScale = scale;
			BackgroundCreateMeshData();
		}*/
	}

	public void Update(){
        //run water physics and other chunk physics using cellular automata 

        //loop through all voxels to find one that can be updated
        updated =  false;
        /*for (int x = 0; x < sChunkSize; x++){
            for (int y = 0; y < sChunkSize; y++){
                for (int z = 0; z < sChunkSize; z++){
                    if(Voxels[x,y,z] == 141){ //0b10001101 is voxel type 13(water)
						//find new water position
						//update old voxel
						//update new voxel
                    }
                }
            }
        }*/
        if(updated && !buildingMesh){BackgroundCreateMeshData(); modified = true;}
    }

	private bool voxelInChunk(vector3Int chunkPos, vector3Int voxelPos){
		if(voxelPos.x >= chunkPos.x && voxelPos.x <= chunkPos.x + chunkSize)
			return true;
		if(voxelPos.y >= chunkPos.y && voxelPos.y <= chunkPos.y + chunkSize)
			return true;
		if(voxelPos.z >= chunkPos.z && voxelPos.z <= chunkPos.z + chunkSize)
			return true;
		return false;
	}

    byte[] MakeSave(){
        byte[] VoxelArr = new byte[(chunkSize + 1)*(chunkSize + 1)*( chunkSize + 1)];
        int i = 0;
        for (int x = 0; x < chunkSize + 1; x++){
            for (int y = 0; y < chunkSize + 1; y++){
                for (int z = 0; z < chunkSize + 1; z++){
                    VoxelArr[i] = Voxels[x,y,z];
                    i++;
                }
            }
        }
		/*List<byte> chunkData = new List<byte>();
		int index = 0;
		byte type;
		byte n;
		while(i < GameData.chunkVoxels){
			n = 0;
			type = VoxelArr[index];
			while(type == VoxelArr[index]){
				n++;
				index++;
			}
			chunkData.Add(n);
			chunkData.Add(type);
		}
        return chunkData.ToArray();*/
		return VoxelArr;
    }

	public void unload(){
		if(modified)
			ChunkLoader.SaveChunk(ChunkPosition, MakeSave(), $"{Game.instance.WorldSavePath}/Chunks/");
		ClearMeshData(false);
		chunk.Clear();
	}
}

public enum VOXELS:byte{
	air = 0,
	grass = 1,
	dirt = 2,
	sand =3,
	snow = 4,
	ice = 5,
	wood = 6,
	leaves = 7,
	smoothStone = 8,
	obsidian = 9,
	woodPlanks = 10,
	hellstone = 11,
	cacti = 12,
	water = 13,
	jungleGrass = 14,
	mud = 15,
	hardendSand = 16,
	darkStone = 17,
	lava = 18,
	stone = 19,

	copper = 100,
	iron = 101,
	gold = 102,
	lead = 103,
	tungston = 104,
	silver = 105,
	titanium = 106,
	platnium = 107,
	colbalt = 108,
	palladium = 109,
	tin = 110,
	ruby = 111,
	topaz = 112,
	fluorite = 113,
	diamond = 114
}

    //Biomes: 0 = Deformity, 1 = snowyForrest, 2 = Forrest, 3 = Ocean, 4 = Snowy, 5 = Icy, 6 = Savana, 7 = rockyPlains, 8 = caves, 9 = underworld,
    //        10 = submeargedCave, 11 = sky island, 12 = mountains, 13 = snowyMountains, 14 = beach, 15 = rockyMountains, 16 = plains, 17 = snowyyPlains. 18 = desert


