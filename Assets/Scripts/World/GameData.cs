﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Voxel{
	public Vector3Int Position;
	public byte VoxelType;

	public Voxel(Vector3Int pos, byte voxelType){
		this.Position = pos;
		this.VoxelType = voxelType;
	}
}

public struct VoxelStructure{
	Voxel[] voxels;
	Vector3 position;
}

public class ReUsableChunk{
	public ChunkLoader loader;
    public Mesh mesh;
	public Mesh colliderMesh;
	public GameObject chunkObject;
	public MeshCollider collider;
	public MeshRenderer renderer;
	public MeshFilter meshFilter;
    public bool inUse;
    public int id;

    public ReUsableChunk(bool _using, int _id, Transform _parent, ChunkLoader _loader){
		loader = _loader;
		mesh = new Mesh();
		colliderMesh = new Mesh();
		chunkObject = new GameObject();
		chunkObject.transform.tag = "terrain";
		chunkObject.transform.parent = _parent;
		chunkObject.layer = 9;
		chunkObject.isStatic = true;

        collider = chunkObject.AddComponent<MeshCollider>();
        renderer = chunkObject.AddComponent<MeshRenderer>();
		meshFilter = chunkObject.AddComponent<MeshFilter>();

		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		renderer.receiveShadows = false;
		inUse = _using;
		id = _id;
		chunkObject.SetActive(false);


		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.subMeshCount = 3;
		colliderMesh.subMeshCount = 2;
        renderer.materials = new Material[]{Resources.Load<Material>("Materials/Voxels/Terrain"),
												Resources.Load<Material>("Materials/Voxels/Ores"),
												Resources.Load<Material>("Materials/Voxels/Liquid")};

    }

    public void Clear(){
		inUse = false;
		mesh.Clear();
		colliderMesh.Clear();
		chunkObject.SetActive(false);
    }

    public void ClearMesh(){
        mesh.Clear();
		colliderMesh.Clear();
		mesh.subMeshCount = 3;
		colliderMesh.subMeshCount = 2;
    }
}

public static class GameData{
    public static int RenderDistance = 5;
    public static int MaxChunks = 10000;
	

    public static byte chunkSize = 32; //+1 for edges
	public static int actualChunkSize = chunkSize + 1;
    public static int chunkVoxels = actualChunkSize * actualChunkSize * actualChunkSize;

    public static float surfaceLevel = -0.27f;
	public static float caveSurface = -0.29f;
    public static float noiseScale = 0.05f;
    public static float terrainAmplitude = 160;

    public static float GravitationalPull = -100;


	public static int	worldWidth		= 256;
    public static int   WorldHeight   	= 64; //in chunks


    public static int 	layer1Height  	= WorldHeight / 16; /*top of underworld*/ 				//4 chunks from base
    public static int 	layer2Height  	= WorldHeight / 16 * 3; /*top of lower caves*/			//8 chunks from underworld
    public static int 	layer3Height  	= WorldHeight / 16 * 6; /*top of mid caves*/			//12 chunks from caves
    public static int 	layer4Height  	= WorldHeight/4 * 3; /*top of underground - surface */	//24 chunks from mid caves
	public static int 	seaLevel		= layer3Height + 4; 							//4 chunks above mid caves
    public static int 	layer5Height  	= WorldHeight; //sky							//26 chunks from top of surface

    public static float[] treeDensity = new float[]{
		0, // deformity
		0.6f, //snowyForrest
		1.2f, // Forrest
		0, //ocean
		0.4f, // snowy
		0, //icy
		0.2f, //savana
		0, //rockyPlains
		0, // caves
		0, //underworld
		0, //submeargedCave
		0, // skyisland
		0.1f, // mountains
		0.1f, //snowyMountains
		0.3f, //beach
		0, //rockyMountains
		0.3f, //plains
		0, //snowyyPlains
		0.5f // desert
    };
    public static float BiomeSize = 1f;
    public static float BiomeRoughness = 0.0035f;


	public static Color[] VoxelTypes = new Color[]{
		new Color(0,0,0), 						//	air				0
		new Color(0.48f,0.98f,0,1), 			//	grass          	1
		new Color(0.38f, 0.267f, 0.17f,1), 		//	dirt    		2
		new Color(1, 0.9f, 0.5f, 1), 			//	sand          	3
		new Color(1,1,1,1), 					//	snow            4
		new Color(0.7f,1,1,0.5f), 				//	ice             5
		new Color(0.52f, 0.24f, 0.051f, 1), 	//	wood   			6
		new Color(0.034042f, 0.424528f, 0.093464f, 0.6f), // leaves 7
		new Color(0.4f,0.4f,0.4f,1), 			//	smooth stone  	8
		new Color(0.1f,0.1f,0.1f,1), 			//	obsidian		9
		new Color(0.52f, 0.24f, 0.051f, 1), 	//	wood planks		10
		new Color(0.9f,0,0), 					//	hellstone		11
		new Color(0.1098f, 0.2588f, 0.074f), 	//	cacti			12
		new Color(0.467f, 0.995f,1f), 			//	water        	13
		new Color(0.02118f,0.64151f,0.24968f), 	//	jungle grass  	14
		new Color(0.361f,0.21917f,0.158864f), 	//	mud         	15
		new Color(0.76415f,0.420822f,0.092f), 	// desertSand 		16
		new Color(0.1f,0.1f,0.1f),				//	darkstone 		17
		new Color(1, 0.3f, 0),						//	lava			18
		new Color(0.4f,0.4f,0.4f,1),			//	stone			19
		new Color(0,0,0),						//		20
		new Color(0,0,0),						//		21
		new Color(0,0,0),						//		22
		new Color(0,0,0),						//		23
		new Color(0,0,0),						//		24
		new Color(0,0,0),						//		25
		new Color(0,0,0),						//		26
		new Color(0,0,0),						//		27
		new Color(0,0,0),						//		28
		new Color(0,0,0),						//		29
		new Color(0,0,0),						//		30
		new Color(0,0,0),						//		31
		new Color(0,0,0),						//		32
		new Color(0,0,0),						//		33
		new Color(0,0,0),						//		34
		new Color(0,0,0),						//		35
		new Color(0,0,0),						//		36
		new Color(0,0,0),						//		37
		new Color(0,0,0),						//		38
		new Color(0,0,0),						//		39
		new Color(0,0,0),						//		40
		new Color(0,0,0),						//		41
		new Color(0,0,0),						//		42
		new Color(0,0,0),						//		43
		new Color(0,0,0),						//		44
		new Color(0,0,0),						//		45
		new Color(0,0,0),						//		46
		new Color(0,0,0),						//		47
		new Color(0,0,0),						//		48
		new Color(0,0,0),						//		49
		new Color(0,0,0),						//		50
		new Color(0,0,0),						//		51
		new Color(0,0,0),						//		52
		new Color(0,0,0),						//		53
		new Color(0,0,0),						//		54
		new Color(0,0,0),						//		55
		new Color(0,0,0),						//		56
		new Color(0,0,0),						//		57
		new Color(0,0,0),						//		58
		new Color(0,0,0),						//		59
		new Color(0,0,0),						//		60
		new Color(0,0,0),						//		61
		new Color(0,0,0),						//		62
		new Color(0,0,0),						//		63
		new Color(0,0,0),						//		64
		new Color(0,0,0),						//		65
		new Color(0,0,0),	//	66
		new Color(0,0,0),	//	67
		new Color(0,0,0),	//	68
		new Color(0,0,0),	//	69
		new Color(0,0,0),	//	70
		new Color(0,0,0),	//	71
		new Color(0,0,0),	//	72
		new Color(0,0,0),	//	73
		new Color(0,0,0),	//	74
		new Color(0,0,0),	//	75
		new Color(0,0,0),	//	76
		new Color(0,0,0),	//	77
		new Color(0,0,0),	//	78
		new Color(0,0,0),	//	79
		new Color(0,0,0),	//	80
		new Color(0,0,0),	//	81
		new Color(0,0,0),	//	82
		new Color(0,0,0),	//	83
		new Color(0,0,0),	//	84
		new Color(0,0,0),	//	85
		new Color(0,0,0),	//	86
		new Color(0,0,0),	//	87
		new Color(0,0,0),	//	88
		new Color(0,0,0),	//	89
		//gems
		new Color(0,0,0),	//ruby	90
		new Color(0,0,0),	//topaz	91
		new Color(0,0,0),	//fluorite	92
		new Color(0,0,0),	//diamond	93
		new Color(0,0,0),	//	94
		new Color(0,0,0),	//	95
		new Color(0,0,0),	//	96
		new Color(0,0,0),	//	97
		new Color(0,0,0),	//	98
		new Color(0,0,0),	//	99
		//ores
		new Color(0.68f, 0.35f, 0.21f, 0),//			copper		100
		new Color(0.49f, 0.49f, 0.53f),// 			iron		101
		new Color(0.97f, 0.83f, 0.13f),//			gold		102
		new Color(0.25f, 0.29f, 0.33f),//			lead		103
		new Color(0.90f, 0.91f, 0.94f),//			tungston	104
		new Color(0.65f, 0.70f, 0.76f),//			silver		105
		new Color(0.16f, 0.17f, 0.20f),//			taitanium	106
		new Color(0.22f, 0.21f, 0.22f),//			platnium	107
		new Color(0.03f, 0.29f, 0.38f),//			colbalt		108
		new Color(0.60f, 0.27f, 0.10f),//			palladium	109
		new Color(0.49f, 0.48f, 0.39f),// 			tin			110
		new Color(0,0,0),//		111
		new Color(0,0,0),// 	112
		new Color(0,0,0),// 	113
		new Color(0,0,0)// 	114
	};

	public static string[] VoxelNames = new string[]{
		"air",//			0
		"grass",//         	1
		"dirt",//    		2
		"sand",// 		  	3
		"snow",//           4
		"ice",//            5
		"wood",//  			6
		"leaves",//			7
		"smooth stone",//	8
		"obsidian",//		9
		"wood planks",//	10
		"hellstone",//		11
		"cacti",//			12
		"water",//			13
		"jungle grass",//	14
		"mud",//			15
		"desert sand",//	16
		"darkstone",//		17
		"lava",//			18
		"stone",//			19
		"Unknown",//		20
		"Unknown",//		21
		"Unknown",//		22
		"Unknown",//		23
		"Unknown",//		24
		"Unknown",//		25
		"Unknown",//		26
		"Unknown",//		27
		"Unknown",//		28
		"Unknown",//		29
		"Unknown",//		30
		"Unknown",//		31
		"Unknown",//		32
		"Unknown",//		33
		"Unknown",//		34
		"Unknown",//		35
		"Unknown",//		36
		"Unknown",//		37
		"Unknown",//		38
		"Unknown",//		39
		"Unknown",//		40
		"Unknown",//		41
		"Unknown",//		42
		"Unknown",//		43
		"Unknown",//		44
		"Unknown",//		45
		"Unknown",//		46
		"Unknown",//		47
		"Unknown",//		48
		"Unknown",//		49
		"Unknown",//		50
		"Unknown",//		51
		"Unknown",//		52
		"Unknown",//		53
		"Unknown",//		54
		"Unknown",//		55
		"Unknown",//		56
		"Unknown",//		57
		"Unknown",//		58
		"Unknown",//		59
		"Unknown",//		60
		"Unknown",//		61
		"Unknown",//		62
		"Unknown",//		63
		"Unknown",//		64
		"Unknown",//		65
		"Unknown",//		66
		"Unknown",//		67
		"Unknown",//		68
		"Unknown",//		69
		"Unknown",//		70
		"Unknown",//		71
		"Unknown",//		72
		"Unknown",//		73
		"Unknown",//		74
		"Unknown",//		75
		"Unknown",//		76
		"Unknown",//		77
		"Unknown",//		78
		"Unknown",//		79
		"Unknown",//		80
		"Unknown",//		81
		"Unknown",//		82
		"Unknown",//		83
		"Unknown",//		84
		"Unknown",//		85
		"Unknown",//		86
		"Unknown",//		87
		"Unknown",//		88
		"Unknown",//		89
		//gems
		"Ruby",//			90
		"Topaz",//			91
		"Fluorite",//		92
		"Diamond",//		93
		"Unknown",//		94
		"Unknown",//		95
		"Unknown",//		96
		"Unknown",//		97
		"Unknown",//		98
		"Unknown",//		99
		//ores
		"Copper",//			100
		"Iron",// 			101
		"Gold",//			102
		"Lead",//			103
		"Tungston",//		104
		"Silver",//			105
		"Taitanium",//		106
		"Platnium",//		107
		"Colbalt",//		108
		"Palladium",//		109
		"Tin",// 			110
		"Unknown",//		111
		"Unknown",// 		112
		"Unknown",// 		113
		"Unknown"// 		114
	};

    //biome voxel types
    public static byte[] BiomeGrass = new byte[]{
        0,4,14,3,4,5,16,0,0,11,0,5,1,4,3,0,1,4,3
    };
	
    public static byte[] BiomeDirt = new byte[]{
        0,2,15,3,2,0,16,0,0,11,0,5,2,2,3,0,2,2,3
    };


	//used for marching cubes - ORDER MATTERS
    public static Vector3Int[] CornerTable = new Vector3Int[8] {
		new Vector3Int(0, 0, 0), // left, base, back		//corner #1
		new Vector3Int(1, 0, 0), // right, base, back		//corner #2
		new Vector3Int(1, 1, 0), // right, top, back		//corner #3
		new Vector3Int(0, 1, 0), // left, top, back			//corner #4
		new Vector3Int(0, 0, 1), //left, base, front		//corner #5
		new Vector3Int(1, 0, 1), //right, base, front		//corner #6
		new Vector3Int(1, 1, 1), //right, top, front		//corner #7
		new Vector3Int(0, 1, 1)  //left, top, front			//corner #8
    };

	public static Vector3Int[] cTable = new Vector3Int[8] {
		new Vector3Int(0, 0, 0), // left, base, back
		new Vector3Int(1, 0, 0), // right, base, back

		new Vector3Int(0, 1, 0), // left, top, back
		new Vector3Int(1, 1, 0), // right, top, back

		new Vector3Int(0, 0, 1), //left, base, front
		new Vector3Int(1, 0, 1), //right, base, front

		new Vector3Int(0, 1, 1),  //left, top, front
		new Vector3Int(1, 1, 1) //right, top, front
	};

	public static int[][] cubeFace = new int[6][] {
		new int[]{5,4,7,7,6,5}, // front
		new int[]{1,0,3,3,2,1}, // back
		new int[]{4,0,3,3,7,4}, // left
		new int[]{1,5,6,6,2,1}, // right
		new int[]{6,7,3,3,2,6}, // top
		new int[]{1,0,4,4,5,1}  //base
	};

	public static Vector3Int[] adjacentCube = new Vector3Int[6] {
		new Vector3Int(0,0,1),
		new Vector3Int(0,0,-1),
		new Vector3Int(-1,0,0),
		new Vector3Int(1,0,0),
		new Vector3Int(0,1,0),
		new Vector3Int(0,-1,0)
	};

	//adjacent points not including corners
	public static Vector3Int[] adjacentPoints = new Vector3Int[18] {
		//new Vector3Int(-1, 1,  -1),
		new Vector3Int(0,  1,  -1),
		//new Vector3Int(1,  1,  -1),
		new Vector3Int(-1, 1,  0),
		new Vector3Int(0,  1,  0),     //top
		new Vector3Int(1,  1,  0),
		// new Vector3Int(-1, 1,  1),
		new Vector3Int(0,  1,  1),
		//new Vector3Int(1,  1,  1),

		new Vector3Int(-1, 0,  -1),
		new Vector3Int(0,  0,  -1),
		new Vector3Int(1,  0,  -1),
		new Vector3Int(-1, 0,  0),     // mid
		new Vector3Int(1,  0,  0),
		new Vector3Int(-1, 0,  1),
		new Vector3Int(0,  0,  1),
		new Vector3Int(1,  0,  1),

		//new Vector3Int(-1, -1,  -1),
		new Vector3Int(0,  -1,  -1),
		//new Vector3Int(1,  -1,  -1),
		new Vector3Int(-1, -1,  0),
		new Vector3Int(0,  -1,  0),    //base
		new Vector3Int(1,  -1,  0),
		//new Vector3Int(-1, -1,  1),
		new Vector3Int(0,  -1,  1)
		//  new Vector3Int(1,  -1,  1)
	};

	//adjacent points including corners
	public static Vector3Int[] AdjacentPoints = new Vector3Int[27] {
		new Vector3Int(-1, 1,  -1),
		new Vector3Int(0,  1,  -1),
		new Vector3Int(1,  1,  -1),
		new Vector3Int(-1, 1,  0),
		new Vector3Int(0,  1,  0),     //top
		new Vector3Int(1,  1,  0),
		new Vector3Int(-1, 1,  1),
		new Vector3Int(0,  1,  1),
		new Vector3Int(1,  1,  1),

		new Vector3Int(-1, 0,  -1),
		new Vector3Int(0,  0,  -1),
		new Vector3Int(1,  0,  -1),
		new Vector3Int(-1, 0,  0),     // mid
		new Vector3Int(0,0,0),
		new Vector3Int(1,  0,  0),
		new Vector3Int(-1, 0,  1),
		new Vector3Int(0,  0,  1),
		new Vector3Int(1,  0,  1),

		new Vector3Int(-1, -1,  -1),
		new Vector3Int(0,  -1,  -1),
		new Vector3Int(1,  -1,  -1),
		new Vector3Int(-1, -1,  0),
		new Vector3Int(0,  -1,  0),    //base
		new Vector3Int(1,  -1,  0),
		new Vector3Int(-1, -1,  1),
		new Vector3Int(0,  -1,  1),
		new Vector3Int(1,  -1,  1)
	};

	//adjacent points including corners
	public static Vector3Int[] AdjacentPoint = new Vector3Int[26] {
		new Vector3Int(-1, 1,  -1),
		new Vector3Int(0,  1,  -1),
		new Vector3Int(1,  1,  -1),
		new Vector3Int(-1, 1,  0),
		new Vector3Int(0,  1,  0),     //top
		new Vector3Int(1,  1,  0),
		new Vector3Int(-1, 1,  1),
		new Vector3Int(0,  1,  1),
		new Vector3Int(1,  1,  1),

		new Vector3Int(-1, 0,  -1),
		new Vector3Int(0,  0,  -1),
		new Vector3Int(1,  0,  -1),
		new Vector3Int(-1, 0,  0),     // mid
		new Vector3Int(1,  0,  0),
		new Vector3Int(-1, 0,  1),
		new Vector3Int(0,  0,  1),
		new Vector3Int(1,  0,  1),

		new Vector3Int(-1, -1,  -1),
		new Vector3Int(0,  -1,  -1),
		new Vector3Int(1,  -1,  -1),
		new Vector3Int(-1, -1,  0),
		new Vector3Int(0,  -1,  0),    //base
		new Vector3Int(1,  -1,  0),
		new Vector3Int(-1, -1,  1),
		new Vector3Int(0,  -1,  1),
		new Vector3Int(1,  -1,  1)
	};

	public static Vector3[,] EdgeTable = new Vector3[12, 2] {
		{ new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f) },
		{ new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 0.0f) },
		{ new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 1.0f, 0.0f) },
		{ new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f) },
		{ new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 1.0f) },
		{ new Vector3(1.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f) },
		{ new Vector3(0.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f) },
		{ new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 1.0f) },
		{ new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f) },
		{ new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f) },
		{ new Vector3(1.0f, 1.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f) },
		{ new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f) }
	};

	public static int[,] TriangleTable = new int[,] {											//index 2, 5, 6, 8, 9, 10, 11, 12 and 14 need to be changed
		{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, 	// 0 =   00000000-  index 0	
		{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 1 =   00000001-	index 1
		{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 2 =   00000010-	index 1	
		{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 3 =   00000011-	index 2
		{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 4 =   00000100-	index 1
		{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 5 =   00000101-	index 3
		{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 6 =   00000110	index 2
		{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},			// 7 =   00000111	index 5
		{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 8 =   00001000-	index 1
		{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 9 =   00001001	index 2
		{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 10 =  00001010	index 3
		{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},			// 11 =  00001011	index 5
		{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 12 =  00001100-	index 2
		{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},		// 13 =  00001101	index 5
		{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},			// 14 =  00001110	index 5
		{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 15 =  00001111	index 8
		{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 16 =  00010000-	index 1
		{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 17 =  00010001	index 2
		{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 18 =  00010010-	index 3
		{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},			// 19 =  00010011	index 5
		{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 20 =  00010100-	index 4
		{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},			// 21 =  00010101	index 6
		{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},			// 22 =  00010110	index 6
		{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},				// 23 =  00010111	index 11
		{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 24 =  00011000	index 3
		{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},			// 25 =  00011001	index 5
		{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},			// 26 =  00011010	index 7
		{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},			// 27 =  00011011	
		{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},			// 28 =  00011100	
		{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},			// 29 =  00011101	
		{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},			// 30 =  00011110	
		{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},		// 31 =  00011111	
		{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 32 =  00100000-	index 1
		{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 33 =  00100001
		{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 34 =  00100010
		{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},			// 35 =  00100011
		{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 36 =  00100100
		{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},			// 37 =  00100101
		{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},			// 38 =  00100110
		{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},				// 39 =  00100111
		{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 40 =  00101000
		{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},			// 41 =  00101001
		{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},			// 42 =  00101010
		{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},				// 43 =  00101011
		{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},			// 44 =  00101100
		{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},			// 45 =  00101101
		{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},			// 46 =  00101110
		{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},			// 47 =  00101111
		{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 48 =  00110000
		{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},			// 49 =  00110001
		{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},			// 50 =  00110010
		{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 51 =  00110011
		{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},			// 52 =  00110100
		{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},				// 53 =  00110101
		{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},				// 54 =  00110110
		{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},			// 55 =  00110111
		{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},			// 56 =  00111000
		{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},				// 57 =  00111001
		{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},				// 58 =  00111010
		{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},			// 59 =  00111011
		{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},			// 60 =  00111100
		{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},				// 61 =  00111101
		{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},				// 62 =  00111110
		{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 63 =  00111111
		{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 64 =  01000000-	index 1
		{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 65 =  01000001
		{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 66 =  01000010
		{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},			// 67 =  01000011
		{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 68 =  01000100
		{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},			// 69 =  01000101
		{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},			// 70 =  01000110
		{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},				// 71 =  01000111
		{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 72 =  01001000
		{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},			// 73 =  01001001
		{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},			// 74 =  01001010
		{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},			// 75 =  01001011
		{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},			// 76 =  01001100
		{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},			// 77 =  01001101
		{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},				// 78 =  01001110
		{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},			// 79 =  01001111
		{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 80 =  01010000
		{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},			// 81 =  01010001
		{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},			// 82 =  01010010
		{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},				// 83 =  01010011
		{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},			// 84 =  01010100
		{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},				// 85 =  01010101
		{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},				// 86 =  01010110
		{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},					// 87 =  01010111
		{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},			// 88 =  01011000
		{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},				// 89 =  01011001
		{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},				// 90 =  01011010
		{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},				// 91 =  01011011
		{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},				// 92 =  01011100
		{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},				// 93 =  01011101
		{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},					// 94 =  01011110
		{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},				// 95 =  01011111
		{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 96 =  01100000
		{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},			// 97 =  01100001
		{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},			// 98 =  01100010
		{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},				// 99 =  01100011
		{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},			// 100 = 01100100
		{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},				// 101 = 01100101
		{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 102 = 01100110
		{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},			// 103 = 01100111
		{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},			// 104 = 01101000
		{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},			// 105 = 01101001
		{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},				// 106 = 01101010
		{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},				// 107 = 01101011
		{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},				// 108 = 01101100
		{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},				// 109 = 01101101
		{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},			// 110 = 01101110
		{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 111 = 01101111
		{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},			// 112 = 01110000
		{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},			// 113 = 01110001
		{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},				// 114 = 01110010
		{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},			// 115 = 01110011
		{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},				// 116 = 01110100
		{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},					// 117 = 01110101	index 6
		{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},			// 118 = 01110110	index 5
		{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 119 = 01110111	index 2
		{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},			// 120 = 01111000	index 12
		{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},				// 121 = 01111001	index 6
		{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},				// 122 = 01111010	index 7
		{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},			// 123 = 01111011	index 3
		{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},					// 124 = 01111100	index 6
		{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 125 = 01111101	index 4
		{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},				// 126 = 01111110	index 3
		{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 127 = 01111111-	index 1
		{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 128 = 10000000-	index 1
		{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 129 = 10000001	index 3
		{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 130 = 10000010	index 4
		{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},			// 131 = 10000011	index 6
		{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 132 = 10000100	index 3
		{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},			// 133 = 10000101	index 7
		{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},			// 134 = 10000110	index 6
		{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},			// 135 = 10000111	index 12
		{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 136 = 10001000	index 2
		{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},			// 137 = 10001001	index 5
		{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},			// 138 = 10001010	index 6
		{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},				// 139 = 10001011
		{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},			// 140 = 10001100
		{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},				// 141 = 10001101
		{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},			// 142 = 10001110
		{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},			// 143 = 10001111
		{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 144 = 10010000
		{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},			// 145 = 10010001
		{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},			// 146 = 10010010
		{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},				// 147 = 10010011
		{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},			// 148 = 10010100
		{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},			// 149 = 10010101
		{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},			// 150 = 10010110
		{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},				// 151 = 10010111
		{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},			// 152 = 10011000
		{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 153 = 10011001
		{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},				// 154 = 10011010
		{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},			// 155 = 10011011
		{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},				// 156 = 10011100
		{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},			// 157 = 10011101
		{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},				// 158 = 10011110
		{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 159 = 10011111
		{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 160 = 10100000
		{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},			// 161 = 10100001
		{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},			// 162 = 10100010
		{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},				// 163 = 10100011
		{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},			// 164 = 10100100
		{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},				// 165 = 10100101
		{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},			// 166 = 10100110
		{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},				// 167 = 10100111
		{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},			// 168 = 10101000
		{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},				// 169 = 10101001
		{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},				// 170 = 10101010
		{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},					// 171 = 10101011
		{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},				// 172 = 10101100
		{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},					// 173 = 10101101
		{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},				// 174 = 10101110
		{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},			// 175 = 10101111
		{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},			// 176 = 10110000
		{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},				// 177 = 10110001
		{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},			// 178 = 10110010
		{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},			// 179 = 10110011
		{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},			// 180 = 10110100
		{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},				// 181 = 10110101
		{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},				// 182 = 10110110
		{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},			// 183 = 10110111
		{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},				// 184 = 10111000
		{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},			// 185 = 10111001
		{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},					// 186 = 10111010
		{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 187 = 10111011
		{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},					// 188 = 10111100
		{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},				// 189 = 10111101
		{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 190 = 10111110
		{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 191 = 10111111	index 1
		{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 192 = 11000000
		{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},			// 193 = 11000001
		{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},			// 194 = 11000010
		{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},			// 195 = 11000011
		{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},			// 196 = 11000100
		{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},				// 197 = 11000101
		{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},				// 198 = 11000110
		{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},					// 200 = 11000111
		{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},			// 201 = 11001000
		{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},				// 202 = 11001001
		{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},				// 203 = 11001010
		{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},					// 204 = 11001011
		{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 205 = 11001100
		{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},			// 206 = 11001101
		{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},			// 207 = 11001110
		{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 208 = 11001111
		{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},			// 209 = 11010000
		{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},			// 210 = 11010001
		{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},			// 211 = 11010010
		{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},				// 212 = 11010011
		{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},				// 213 = 11010100
		{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},				// 214 = 11010101
		{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},				// 215 = 11010110
		{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 216 = 11010111
		{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},				// 217 = 11011000
		{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},			// 218 = 11011001
		{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},				// 219 = 11011010
		{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},				// 220 = 11011011
		{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},			// 221 = 11011100
		{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 222 = 11011101
		{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},				// 223 = 11011110
		{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 224 = 11011111	index 1
		{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},		// 225 = 11100000
		{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},			// 226 = 11100001
		{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},			// 227 = 11100010
		{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},				// 228 = 11100011
		{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},			// 229 = 11100100
		{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},				// 230 = 11100101	index 7
		{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},			// 231 = 11100110	index 5
		{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},				// 232 = 11100111	index 3
		{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},				// 233 = 11101000	index 11
		{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},				// 234 = 11101001	index 6
		{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},				// 235 = 11101010	index 6
		{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 236 = 11101011	index 4
		{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},			// 237 = 11101100	index 5
		{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},				// 238 = 11101101	index 3
		{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 239 = 11101110	index 2
		{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 240 = 11101111	index 1 
		{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 241 = 11110000	index 8
		{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},			// 242 = 11110001	index 5
		{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},		// 243 = 11110010	index 5
		{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 244 = 11110011	index 2
		{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},			// 245 = 11110100	index 5
		{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},				// 246 = 11110101	index 3
		{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 247 = 11110110	index 2
		{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 248 = 11110111	index 1
		{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},			// 249 = 11111000	index 5
		{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 250 = 11111001	index 2
		{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},				// 251 = 11111010	index 3
		{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 252 = 11111011	index 1
		{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},			// 253 = 11111100	index 2
		{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 254 = 11111101	index 1
		{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},		// 255 = 11111110	index 1
		{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}	// 256 = 11111111	index 0
	};
}
