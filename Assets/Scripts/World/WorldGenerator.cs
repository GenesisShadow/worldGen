using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics; 
using System.IO;
using System.Threading;

public class WorldGenerator{
	public static WorldGenerator instance;
	private int Seed;
	private string SavePath;
	private float voxelSurfaceLevel { get { return GameData.surfaceLevel; } }

	static byte chunkSize { get { return GameData.chunkSize; } }
	static int sChunkSize { get { return GameData.actualChunkSize; } }

	public FastNoiseSIMD CaveNoise = new FastNoiseSIMD();
	public FastNoiseSIMD UndergroundNoise = new FastNoiseSIMD();
	public FastNoiseSIMD HillNoise = new FastNoiseSIMD();
	public FastNoiseSIMD BiomeNoise = new FastNoiseSIMD();
	public FastNoiseSIMD TreeNoise = new FastNoiseSIMD();

	public FastNoiseSIMD DarkStoneNoise = new FastNoiseSIMD();

	public FastNoiseSIMD GemNoise = new FastNoiseSIMD();
	public FastNoiseSIMD CopperNoise = new FastNoiseSIMD();
	public FastNoiseSIMD IronNoise = new FastNoiseSIMD();
	public FastNoiseSIMD GoldNoise = new FastNoiseSIMD();

	//public FastNoiseSIMD 


	bool[] islandsGenerated = new bool[8];

	Stopwatch CodeTimer = new Stopwatch();
	private void CodeTimerStop(string name){
		//call CodeTimer.Start(); before calling this function
		CodeTimer.Stop();
		TimeSpan ts = CodeTimer.Elapsed;
		string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
		ts.Hours, ts.Minutes, ts.Seconds,
		ts.Milliseconds / 10);
		UnityEngine.Debug.Log(name + " RunTime: " + elapsedTime);
		CodeTimer.Reset();
	}
	
	public WorldGenerator(int _seed, string _savePath){
		instance = this;
		SavePath = _savePath;
		Seed = _seed;
		SetNoise();
	}
/*
	public void Generate(){
		//generate small islands
		//for(int i = 0; i < 4; i++){
			GenerateIslandTerrain(0);
		//}
		//generate abyss
		//generate sky islands
	}

	public void GenerateIslandTerrain(int island){
		if(island < 4){
			GenerateSmallIsland(island);
		}else{
			GenerateLargeIsland(island);
		}
	}

	private void GenerateSmallIsland(int island){
		//underground is like caves but with less caves and is made of dirt with a few patches of stone
		//mid caves are generated using the cave noise and are made of stone with patches of dirt
		//lower caves are the same as mid caves but with no dirt and a lot more ore and lava patches, and pathces of darkstone
		//underworld is generated using two height maps and is made with a mix of hell stone and darkstone
		int sectionsGenerated = 0;
		int islandSectionSize = GameData.islandWidth*chunkSize/GameData.islandSections;

		//get island height map
		float[] HillSet = HillNoise.GetNoiseSet(0, 0, 0, chunkSize*GameData.islandWidth + 1, 1, chunkSize*GameData.islandWidth + 1);

		for(int sx = 0; sx < GameData.islandSections; sx++){
			for(int sz = 0; sz < GameData.islandSections; sz++){
				vector3Int pos = new vector3Int(sx*islandSectionSize,0,sz*islandSectionSize);
				Thread thread = new Thread(() => GenerateSmallIslandSection(island, pos, ref sectionsGenerated, HillSet));
				thread.Priority = ThreadPriority.Highest;
				thread.Start();
			}
		}
		while(sectionsGenerated < GameData.islandSections*GameData.islandSections){
			//wait for all sections to generate
		}

		int ws = chunkSize*GameData.islandWidth + 1;
		//spawn trees
		/*for(int x = 0; x < GameData.islandWidth; x++){
			for(int z = 0; z < GameData.islandWidth; z++){
				float hillHeight = (HillSet[x*chunkSize*ws+z*chunkSize]+1)*GameData.terrainAmplitude/2 + GameData.layer3Height*chunkSize;
				vector3Int pos = new vector3Int(x*chunkSize,(int)hillHeight,z*chunkSize);
				PasteStructure(GameStructures.GreenTrees[0], pos);
				//get offest for trees based on noise
				//get new position
				//get hill height for that position
				//if hillheigth voxel is grass or dirt spawn tree

				//when spawning tree check if tree overlaps other chunks
				//if it does load all those chunks from file then spawn
				// the tree in and re save chunks
			}
		}
	}

	private void GenerateSmallIslandSection(int island, vector3Int sectionPos, ref int wait, float[] HillSet){
		int ws = chunkSize*GameData.islandWidth + 1;
		
		//for each chunk within the section
		for(int x = 0; x < GameData.islandWidth/GameData.islandSections; x++){
			for(int z = 0; z < GameData.islandWidth/GameData.islandSections; z++){
				for(int y = 0; y < GameData.WorldHeight; y++){

					vector3Int chunkPos = new vector3Int(x*chunkSize,y*chunkSize,z*chunkSize) + sectionPos;
					//if chunk is below hill layer get cave noise //section pos is always 0
					float[] caveSet = new float[GameData.chunkVoxels];

					if(chunkPos.y < GameData.terrainAmplitude + GameData.layer3Height*chunkSize)
						caveSet = CaveNoise.GetNoiseSet(chunkPos.x, chunkPos.y, chunkPos.z, sChunkSize, sChunkSize, sChunkSize);
					//set chunk data
					byte[] chunkData = new byte[GameData.chunkVoxels];

					//this loop in total will be run over 1,048,576 times for each island so must be kept efficent as possible
						for(int i = 0; i < GameData.chunkVoxels; i++){
							vector3Int pos = intToXyz(i);
							vector3Int iPos = pos + chunkPos;
							float hillHeight = (HillSet[iPos.x*ws+iPos.z]+1)*GameData.terrainAmplitude/2 + GameData.layer3Height*chunkSize;
							//set caves rock
							if(iPos.y < hillHeight - 32){
								chunkData[i] = caveSet[i] > GameData.caveSurface ? (byte)128 : (byte)127;
							}
							//set caves dirt
							else if(iPos.y < hillHeight - 3){
								chunkData[i] = caveSet[i] > GameData.caveSurface ? (byte)130 : (byte)127;
							}
							//set grass
							else if(iPos.y < hillHeight){
								chunkData[i] = caveSet[i] > GameData.caveSurface ? (byte)129 : (byte)127;
							}
							else chunkData[i] = (byte)127;
						}
					ChunkLoader.SaveChunk(chunkPos, chunkData, SavePath);
				}
			}
		}
		wait++;
	}

	private void GenerateLargeIsland(int island){
		
	}

	private void GenerateLargeIslandSection(int island, vector3Int sectionPos, ref int wait){
		//check if section is on the edge if so set the voxels on the edge to air
	}	*/

	public void SetNoise(){
		//cave
		CaveNoise.SetNoiseType(FastNoiseSIMD.NoiseType.SimplexFractal);
		CaveNoise.SetFractalType(FastNoiseSIMD.FractalType.FBM);
		CaveNoise.SetFrequency(0.02f);
		CaveNoise.SetFractalOctaves(3);
		CaveNoise.SetFractalLacunarity(2);
		CaveNoise.SetFractalGain(0.65f);
		CaveNoise.SetSeed(Seed);

		//hills
		HillNoise.SetNoiseType(FastNoiseSIMD.NoiseType.SimplexFractal);
		HillNoise.SetFractalType(FastNoiseSIMD.FractalType.FBM);
		HillNoise.SetFrequency(0.0015f);
		HillNoise.SetFractalOctaves(4);
		HillNoise.SetFractalLacunarity(3);
		HillNoise.SetFractalGain(0.4f);
		HillNoise.SetSeed(Seed);

		//biome noise
		BiomeNoise.SetNoiseType(FastNoiseSIMD.NoiseType.Cellular);
		BiomeNoise.SetCellularDistanceFunction(FastNoiseSIMD.CellularDistanceFunction.Natural);
		BiomeNoise.SetCellularReturnType(FastNoiseSIMD.CellularReturnType.CellValue);
		BiomeNoise.SetCellularJitter(0.8f);
		BiomeNoise.SetSeed(Seed);

		//tree density noise
		TreeNoise.SetNoiseType(FastNoiseSIMD.NoiseType.WhiteNoise);

		//set ore noise
		CopperNoise.SetNoiseType(FastNoiseSIMD.NoiseType.SimplexFractal);
		CopperNoise.SetFrequency(0.047f);
		CopperNoise.SetSeed(Seed + 120);

		IronNoise.SetNoiseType(FastNoiseSIMD.NoiseType.SimplexFractal);
		IronNoise.SetFrequency(0.047f);
		IronNoise.SetSeed(Seed*2);

		GoldNoise.SetNoiseType(FastNoiseSIMD.NoiseType.SimplexFractal);
		GoldNoise.SetFrequency(0.04f);
		GoldNoise.SetSeed(Seed*3);

		DarkStoneNoise.SetNoiseType(FastNoiseSIMD.NoiseType.SimplexFractal);
		DarkStoneNoise.SetFractalType(FastNoiseSIMD.FractalType.Billow);
		DarkStoneNoise.SetFrequency(0.02f);
		DarkStoneNoise.SetSeed(Seed*3);
		DarkStoneNoise.SetFractalOctaves(2);
		DarkStoneNoise.SetFractalLacunarity(2);
		DarkStoneNoise.SetFractalGain(0.65f);
	}

	public static int xyzToInt(int x, int y, int z){
		return z + sChunkSize*(y+(sChunkSize*x));
	}
	public static vector3Int intToXyz(int index){
		int i = index;
		int x = i / (sChunkSize*sChunkSize);
		i -= (x*sChunkSize*sChunkSize);
		int y = i / sChunkSize;
		int z = i % sChunkSize;
		return new vector3Int(x,y,z);
	}
}

public struct voxelsSave{
	public voxelsSave(UInt16 a, byte t){
		amount = a;
		type = t;
	}
	public UInt16 amount;
	public byte type;
}
