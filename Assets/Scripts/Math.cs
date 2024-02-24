using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct vector3Int{
	public int x,y,z;
	
	public vector3Int(int _x, int _y, int _z){
		x = _x;
		y = _y;
		z = _z;
	}
	public vector3Int(int _s){
		x = _s;
		y = _s;
		z = _s;
	}

	public vector3Int(Vector3 v){
		x = Mathf.RoundToInt(v.x);
		y = Mathf.RoundToInt(v.y);
		z = Mathf.RoundToInt(v.z);
	}
	
	public static implicit operator vector3Int(UnityEngine.Vector3Int v){
		return new vector3Int { x = v.x, y = v.y, z = v.z };
	}

	public static implicit operator UnityEngine.Vector3Int(vector3Int v){
		return new UnityEngine.Vector3Int(v.x, v.y, v.z);
	}

	public static implicit operator UnityEngine.Vector3(vector3Int v){
		return new UnityEngine.Vector3(v.x, v.y, v.z);
	}
	
	public static vector3Int operator +(vector3Int a, vector3Int b)
			=> new vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);

	public static vector3Int operator -(vector3Int a, vector3Int b)
			=> new vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
			
	public static vector3Int operator +(vector3Int a, UnityEngine.Vector3Int b)
			=> new vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);

	public static vector3Int operator -(vector3Int a, UnityEngine.Vector3Int b)
			=> new vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
			
	public static vector3Int operator +(UnityEngine.Vector3Int a, vector3Int b)
			=> new vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);

	public static vector3Int operator -(UnityEngine.Vector3Int a, vector3Int b)
			=> new vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
			
	public static vector3Int operator *(vector3Int a, byte b)
		=> new vector3Int(a.x * b, a.y * b, a.z * b);

	public static vector3Int operator /(vector3Int a, int b)
		=> new vector3Int(a.x / b, a.y / b, a.z / b);

	public static bool operator ==(vector3Int a, vector3Int b)
		=> (a.x == b.x && a.y == b.y && a.z == b.z);

	public static bool operator ==(vector3Int a, Vector3Int b)
		=> (a.x == b.x && a.y == b.y && a.z == b.z);

	public static bool operator !=(vector3Int a, Vector3Int b)
		=> (a.x != b.x || a.y != b.y || a.z != b.z);
              
	public static bool operator !=(vector3Int a, vector3Int b)
		=> (a.x != b.x || a.y != b.y || a.z != b.z);
	
	public static vector3Int zero = new vector3Int(0,0,0);

	public float magnitude{
		get{return (float)Math.Sqrt(x*x + y*y + z*z);}
	}
}


public class VoxelOctree{
	//contains 32x32x32 leaf nodes
	VoxelOctreeNode root;

	public VoxelOctree(){
		root.size = GameData.chunkSize;
	}

	public void InsertVoxel(vector3Int _pos, byte _type){
		
	}
}

public class VoxelOctreeNode{
	VoxelOctree parent;
	public VoxelOctreeNode[] children = null; 
	//children order
	//bottom
	// 2 , 3
	// 0 , 1
	//top
	// 6 , 7
	// 4 , 5

	public int size;
	public vector3Int position;

	byte activeNodes = 0; //bitmask containing active children nodes
	public byte type;


	public void Split(){
		if(size > 1){
			children = new VoxelOctreeNode[8];
			for(int i = 0; i < 8; i++){
				children[i].type = type;
				children[i].size = size/2;
			}
		}else
			UnityEngine.Debug.Log("ERROR: trying to split octree node smaller than 1");
	}

	public void Chop(){
		Array.Clear(children, 0, 8);
	}

	public void InsertVoxel(vector3Int _pos, byte _type){
		if(size == 1)
			type = _type;
		else{
			if(children == null)
				Split();
			if(_pos.x < 2){
				//if(pos.y < size)
					//children[].InsertVoxel
				//else
			}
		}
	}

	public void SetChildType(int _index, byte _type){
		if(children != null){
			children[_index].type = _type;
			bool leaf = true;
			for(int i = 0; i < 8; i++){
				if(children[i].type != _type)
					leaf = false;
			}
			if(leaf)
				Chop();
		}else if(_type != type){
			Split();
			children[_index].type = _type;
		}
	}
}