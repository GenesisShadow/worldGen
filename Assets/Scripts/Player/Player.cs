using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{

	//debug
	public bool fly = false;


	private float speedMultiplyer = 1;
  	private float walkSpeed = 0.1f;
	private float runMultiplyer = 1;
	private float maxRunSpeed = 1.5f;
	private float jumpPower = 0.3f;
	private float GravitationalForce = 0.98f;
	public bool dashing;
	
	private float dashTime = 0.2f;
	private float dashSpeed = 5;
	private float currentDashTime;

	private ChunkLoader cLoader;
	private VoxelController vxControl;
	private InputManager input;
	private GameObject playerObject;
	private CharacterController controller;
	public GameObject CamObject;
	private Inventory inventory;

	public bool isGrounded;
	private float airTime;
	private Vector3 PlayerForces;
	public vector3Int chunkPos;

	public Player(ChunkLoader _loader, VoxelController v){
		playerObject = new GameObject();
		playerObject.name = "player";
		playerObject.layer = 8;

		input = playerObject.AddComponent<InputManager>();

		CamObject = new GameObject();
		CamObject.transform.parent = playerObject.transform;
		//GameObject Cam2 = new GameObject();
		Camera camera = CamObject.AddComponent<Camera>();
		//Camera camera2 = Cam2.AddComponent<Camera>();
		//Cam.transform.parent = CamObject.transform;
		//Cam2.transform.parent = CamObject.transform;
		CamObject.transform.position = new Vector3(0,1.3f,0);
		//Cam.transform.position = new Vector3(0,0,0);
		//Cam2.transform.position = new Vector3(0,0,0);
		camera.clearFlags = CameraClearFlags.Depth;
		camera.depth = 1;
		camera.cullingMask &= ~(1 << LayerMask.NameToLayer("sky"));
		camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
		camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
		//camera.farClipPlane = 100;

		//camera2.clearFlags = CameraClearFlags.Depth;
		/*camera2.depth = 1;
		camera2.cullingMask &= ~(1 << LayerMask.NameToLayer("sky"));
		camera2.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
		camera2.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
		Cam2.layer = 8;*/
		CamObject.layer = 8;
		//Cam.layer = 8;

		/*AnaglyphEffect a = Cam.AddComponent<AnaglyphEffect>();
		a.cam2 = camera2;
		a.fxShader = Resources.Load<Shader>("Shaders/AnaglyphShader");
		a.enabled = true;*/
		
		//camera.backgroundColor = new Color(23.0f/255.0f,26.0f/255.0f,31.0f/255.0f);

		CameraController c = CamObject.AddComponent<CameraController>();
		c.player = playerObject;
		c.input = input;
		c.skyCam = GameObject.Find("sky");

		playerObject.transform.position = new Vector3(6144,GameData.terrainAmplitude + GameData.layer3Height*GameData.chunkSize,6144);
		controller = playerObject.AddComponent<CharacterController>();
		controller.slopeLimit = 60;
		controller.height = 3;
		controller.radius = 1;
		playerObject.layer = 8;

		cLoader = _loader;
		cLoader.midPosition = playerObject.transform.position;
		cLoader.SetChunksToLoad();
		cLoader.loadChunks(2,2,playerObject.transform.position - new Vector3(0,32,0));
		cLoader.cam = camera;

		CamObject.AddComponent<MeshRenderer>();
		MeshFilter m = CamObject.AddComponent<MeshFilter>();
		m.mesh = Resources.Load<Mesh>("Mesh/Dude");
		
		//camera.farClipPlane = 100;

		vxControl = v;

		/*Light l = CamObject.AddComponent<Light>();
		l.type = LightType.Spot;
		l.spotAngle = 85;
		l.intensity = 0.6f;
		l.range = 50;*/
	}

	public void DeleteSelf(){
		input.UnlockCursor();
		MonoBehaviour.Destroy(CamObject);
		MonoBehaviour.Destroy(playerObject);
	}


	
	public void Update(){
		move();
		if(input.OnPlace){
			PlaceVoxel(Game.instance.VoxelToPlace,10);
		}
		if(input.OnMine){
				MineVoxel(1,10);
		}
		vxControl.ShowPlace(CamObject.transform);
	}

	private void move(){
		UpdateGrounded();
		if(!fly)
			Gravity(CheckGrounded());
		if(input.OnJump && isGrounded)
			Jump();
		Vector3 foward = CamObject.transform.forward - new Vector3(0,CamObject.transform.forward.y,0);
        Vector3 right = CamObject.transform.right - new Vector3(0,CamObject.transform.right.y,0);
        Vector3 Force = (foward * input.Yaxis())
                      + (right * input.Xaxis());
		Force.Normalize();
		Force *= walkSpeed * runMultiplyer * speedMultiplyer;
		Force.y = 0;

		controller.Move((Force + PlayerForces)*Time.deltaTime*100);
		cLoader.midPosition = CamObject.transform.position;

			if(input.Yaxis() == 0 && input.Xaxis() == 0)
				runMultiplyer = 1;
			else 
				runMultiplyer += runMultiplyer < maxRunSpeed ? Time.deltaTime*maxRunSpeed/3 : 0;

		if(input.dash()){
			speedMultiplyer = dashSpeed;
			dashing = true;
		} 
		if(dashing){
			currentDashTime += Time.deltaTime;
			if(currentDashTime > dashTime){
				speedMultiplyer = 1;
				dashing = false;
				currentDashTime = 0;
			}
		}
	}

	vector3Int GetChunkPosition(){
		vector3Int pos = new vector3Int();

		return pos;
	}

	private void Gravity(bool ground){
		//check that the player isnt grounded
		// apply a downward force
		if(!ground){
			if(PlayerForces.y > -GravitationalForce)
				PlayerForces.y -= GravitationalForce * Time.deltaTime;
			else
				PlayerForces.y = -GravitationalForce;
		}else if(PlayerForces.y < 0){
			PlayerForces.y = 0;
		}
    }

	public void Jump(){
		PlayerForces.y = jumpPower;
		isGrounded = false;
    }

	private void UpdateGrounded(){
		bool ground = CheckGrounded();
		if(!ground)
			airTime+=Time.deltaTime;
		else
			airTime = 0;
		if(!ground && airTime > 0.2f)
			isGrounded = false;
		if(ground)
			isGrounded = true;
	}

	public bool CheckGrounded(){
		RaycastHit hit;
		if(Physics.Raycast(playerObject.transform.position - new Vector3(0,1.5f,0), playerObject.transform.up*-1, out hit, 0.8f)){
			return true;
		}else if(
				Physics.Raycast(playerObject.transform.position - new Vector3(0.5f,1.5f,0.5f), playerObject.transform.up*-1, 1f)
			|| 	Physics.Raycast(playerObject.transform.position - new Vector3(0.5f,1.5f,-0.5f), playerObject.transform.up*-1, 1f)
			|| 	Physics.Raycast(playerObject.transform.position - new Vector3(-0.5f,1.5f,0.5f), playerObject.transform.up*-1, 1f)
			|| 	Physics.Raycast(playerObject.transform.position - new Vector3(-0.5f,1.5f,-0.5f), playerObject.transform.up*-1, 1f)){
				return true;
		}else{
			return false;
		}
    }

	public void PlaceVoxel(byte type, int reach){
		vxControl.PlaceTerrain(type,reach,CamObject.transform);
	}

	public void MineVoxel(int size, int reach){
		vxControl.RemoveTerrain(size,reach,CamObject.transform);
	}

	public void RemoveVoxel(){

	}
}
