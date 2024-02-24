using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledChunk
{
	public ChunkManager loader;
    public Mesh mesh;
	public Mesh colliderMesh;
	public GameObject chunkObject;
	public MeshCollider collider;
	public MeshRenderer renderer;
	public MeshFilter meshFilter;
    public bool inUse;
    public int id;

    public PooledChunk(bool _using, int _id, Transform _parent, ChunkManager _loader){
		loader = _loader;
		mesh = new Mesh();
		colliderMesh = new Mesh();
		chunkObject = new GameObject();
		chunkObject.transform.tag = "terrain";
		chunkObject.transform.parent = _parent;
		chunkObject.layer = 9;

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

public class ChunkManager
{
    private Transform chunkParent;
    public string savePath;
    public Vector3 loadPoint;

    //octree chunks
    private PooledChunk[] meshes = new PooledChunk[GameData.MaxChunks];



    public ChunkManager(Transform _chunkParent, string _savePath){
        chunkParent = _chunkParent;
        savePath = _savePath;
        SetMeshes();
    }


    public void Update(){

    }

    PooledChunk GetAvalibleChunk(){
        for (int i = 0; i < GameData.MaxChunks; i++){
            if (!meshes[i].inUse){
                meshes[i].inUse = true;
                meshes[i].id = i;
                return meshes[i];
            }
        }
        UnityEngine.Debug.Log("ERROR: trying to access Mesh that isnt available");
        return null;
    }

    public void SetMeshes(){
        for(int i = 0; i < GameData.MaxChunks; i++){
            if(meshes[i] == null) meshes[i] = new PooledChunk(false,i, chunkParent, this);
        }
    }


}
