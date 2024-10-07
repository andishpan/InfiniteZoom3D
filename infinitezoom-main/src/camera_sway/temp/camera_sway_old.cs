using Godot;
using System;

public partial class camera_sway_old : Node3D
{
	[Export]
	Camera3D cam;

	[Export]
	RigidBody3D rig;

	[Export]
	Node3D container;

	[Export]
	float zoomEffectSpeed = 0.04f;

	float elapsedTime = 0;

	float spawnCooldown = 1.0f;

	float distanceBetween = 5.0f;

	MeshInstance3D obstacleMesh;

	float lastDuplicatePositionZ;


	bool dodge1 = false;

	bool dodge2 = false;

	float dodgeMovementSpeed = 0.2f; //distance the camera moves to avoid an obstacle

	float dodgeRotationSpeed = 0.01f;

	float dodgeRotationSpeedUpDown = 0.006f;

	int rotationFrames1 = 50; //frames it takes for the camera to perform the first dodge
	int rotationFrames2 = 50 * 2;

	int frameCounter = 0;

	bool obstacleDetected = false;

	bool shouldMoveCamera = false;
    private bool moveToOrigin;
    Vector3 targetCameraPosition;
    private bool moveToTarget;
    float dodgeRange = 10;

	enum DodgeDirection
	{
		Left,
		Right,
		Up,
		Down
	}

	enum ExperimentalDodgeDirection
	{
		LeftUp,
		RightUp,
		LeftDown,
		RightDown
	}

	DodgeDirection dodgeDirection;

	bool leftDetected = false;
	bool rightDetected = false;
	bool upDetected = false;
	bool downDetected = false;

	Area3D leftArea;
	Area3D rightArea;
	Area3D upArea;
	Area3D downArea;
    private Vector3 originalCameraPosition;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		obstacleMesh = container.GetChild<MeshInstance3D>(0);
		lastDuplicatePositionZ = obstacleMesh.Position.Z;
	}

	void AreaEntered(Area3D area)
{
    originalCameraPosition = cam.GlobalPosition;

    Vector3 objectPosition = area.GlobalPosition;
    Vector3 cameraPosition = cam.GlobalPosition;

    Vector3 relativePosition = objectPosition - cameraPosition;
    Vector3 moveDirection = -relativePosition.Normalized();

    targetCameraPosition = new Vector3(moveDirection.X * dodgeRange, moveDirection.Y * dodgeRange, cameraPosition.Z);
    moveToTarget = true;
    moveToOrigin = false;
}

	void LeftAreaEntered(Area3D area)
	{
		if(area.GetParent().GetParent().Name == "Container"){
			GD.Print("Left Area Detection! " + area);
			leftDetected = true;
			leftArea = area;
			obstacleDetected = true;
			dodgeDirection = DodgeDirection.Left;
		}
	}

	void RightAreaEntered(Area3D area)
	{
		if(area.GetParent().GetParent().Name == "Container"){
			GD.Print("Right Area Detection! " + area);
			rightDetected = true;
			rightArea = area;
			obstacleDetected = true;
			dodgeDirection = DodgeDirection.Right;
		}
	}

	void UpAreaEntered(Area3D area)
	{
		if(area.GetParent().GetParent().Name == "Container"){
			GD.Print("Up Area Detection! " + area);
			upDetected = true;
			upArea = area;
			obstacleDetected = true;
			dodgeDirection = DodgeDirection.Up;
		}
	}

	void DownAreaEntered(Area3D area)
	{
		if(area.GetParent().GetParent().Name == "Container"){
			GD.Print("Down Area Detection! " + area);
			downDetected = true;
			downArea = area;
			obstacleDetected = true;
			dodgeDirection = DodgeDirection.Down;
		}
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float deltaF = (float) delta;

		float moveSpeed = 5.0f;

		GD.Print("moveToTarget: " + moveToTarget + " moveToOrigin: " + moveToOrigin);

		if (moveToTarget)
		{
			cam.GlobalPosition = cam.GlobalPosition.Lerp(targetCameraPosition, moveSpeed * deltaF);

		
			if (cam.GlobalPosition.DistanceTo(targetCameraPosition) < 0.1f)
			{
				cam.GlobalPosition = targetCameraPosition; 
				moveToTarget = false;
				moveToOrigin = true; 
			}
		}
		else if (moveToOrigin)
		{
			Vector3 originPosition = new Vector3(0, 0, originalCameraPosition.Z);
			cam.GlobalPosition = cam.GlobalPosition.Lerp(originPosition, moveSpeed * deltaF);

			if (cam.GlobalPosition.DistanceTo(originPosition) < 0.1f)
			{
				cam.GlobalPosition = originPosition; 
				moveToOrigin = false; 
			}
		}

		//obstacle spawning
		elapsedTime += (float) delta;

		if (elapsedTime >= spawnCooldown)
		{

			MeshInstance3D duplicate = (MeshInstance3D)obstacleMesh.Duplicate();
			Random r = new Random();
			double scaleMax = 1;
			double scaleMin = 1;

			float scale = (float)(r.NextDouble() * (scaleMax - scaleMin) + scaleMin);
			duplicate.Scale = new Vector3(scale, scale, scale);
			container.AddChild(duplicate);

            OrmMaterial3D material = new OrmMaterial3D
            {
                AlbedoColor = new Color((float)r.NextDouble(),
            (float)r.NextDouble(),
            (float)r.NextDouble())
            };

            duplicate.SetSurfaceOverrideMaterial(0, material);

			double max = 1;
			double min = -1;


			float randomX = (float)(duplicate.Position.X + (r.NextDouble() * (max - min) + min));
			float randomY = (float)(duplicate.Position.Y + (r.NextDouble() * (max - min) + min));

			duplicate.Position = new Vector3(randomX, randomY,
				lastDuplicatePositionZ - distanceBetween);
			lastDuplicatePositionZ -= distanceBetween;
			elapsedTime = 0;
		}

		// move all obstacles in "container" to the camera
		foreach(Node node in container.GetChildren())
		{
			MeshInstance3D obstacleMesh = (MeshInstance3D)node;

			float x = obstacleMesh.Position.X;
			float y = obstacleMesh.Position.Y;
			float z = obstacleMesh.Position.Z + zoomEffectSpeed;

			obstacleMesh.Position = new Vector3(x, y, z);

			Random r = new Random();
			obstacleMesh.RotateX(r.Next(-1, 1) * 0.05f);
			obstacleMesh.RotateY(r.Next(-1, 1) * 0.05f);
			obstacleMesh.RotateZ(r.Next(-1, 1) * 0.05f);
		}


		//obstacle avoidance
		if (obstacleDetected && !dodge1 && !dodge2)
		{			
			dodge1 = true;
		}

		//make sure only one of the four detection booleans is true
		if(leftDetected && rightDetected)
		{
			GD.Print(Math.Abs(leftArea.GlobalPosition.X) + " " + Math.Abs(rightArea.GlobalPosition.X));

			if(Math.Abs(leftArea.GlobalPosition.X) < Math.Abs(rightArea.GlobalPosition.X))
			{
				rightDetected = false;
			}
			else
			{
				leftDetected = false;
			}
		}

		if(upDetected && downDetected)
		{
			GD.Print(Math.Abs(upArea.GlobalPosition.Y) + " " + Math.Abs(downArea.GlobalPosition.Y));

			if(Math.Abs(upArea.GlobalPosition.Y) < Math.Abs(downArea.GlobalPosition.Y))
			{
				downDetected = false;
			}
			else
			{
				upDetected = false;
			}
		}

		if(leftDetected && upDetected){
			if(Math.Abs(leftArea.GlobalPosition.X) < Math.Abs(upArea.GlobalPosition.Y))
			{
				upDetected = false;
			}
			else
			{
				leftDetected = false;
			}
		}else if(leftDetected && downDetected){
			if(Math.Abs(leftArea.GlobalPosition.X) < Math.Abs(downArea.GlobalPosition.Y))
			{
				downDetected = false;
			}
			else
			{
				leftDetected = false;
			}
		}else if(rightDetected && upDetected){
			if(Math.Abs(rightArea.GlobalPosition.X) < Math.Abs(upArea.GlobalPosition.Y))
			{
				upDetected = false;
			}
			else
			{
				rightDetected = false;
			}
		}else if(rightDetected && downDetected){
			if(Math.Abs(rightArea.GlobalPosition.X) < Math.Abs(downArea.GlobalPosition.Y))
			{
				downDetected = false;
			}
			else
			{
				rightDetected = false;
			}
		}


		if(leftDetected){
			//Dodge(-1 , deltaF);
			DodgeExperimental(-1, deltaF);
		}else if(rightDetected){
			//Dodge(1, deltaF);
		}else if(upDetected){
			//DodgeUpDown(-1, deltaF);
		}else if(downDetected){
			//DodgeUpDown(1, deltaF);
		}



	}

	void resetDetections()
	{
		leftDetected = false;
		rightDetected = false;
		upDetected = false;
		downDetected = false;
	}


	//--------------------------------------------------------------------------------------------
	// Obstacles Avoidance
	void DodgeExperimental(int direction, float delta){
		float t;
		float easedRotationSpeed;

		if (dodge1 && frameCounter <= rotationFrames1)
		{
			t = (float)frameCounter / rotationFrames1;
			easedRotationSpeed = dodgeRotationSpeed * MakeMovementCubic(t) * delta;
			cam.RotateZ(easedRotationSpeed * direction);
			cam.RotateX(easedRotationSpeed * direction);
			rig.ApplyImpulse(new Vector3(-1 * direction, 0, 0) * dodgeMovementSpeed);
			rig.ApplyImpulse(new Vector3(0, direction, 0) * dodgeMovementSpeed);
		}
		if (dodge1 && frameCounter == rotationFrames1 + 1)
		{
			dodge1 = false;
			dodge2 = true;
			rig.LinearVelocity = Vector3.Zero;
            rig.AngularVelocity = Vector3.Zero;
		}
		if (dodge2 && frameCounter <= rotationFrames2)
		{
			t = (float)(frameCounter - rotationFrames1 - 1) / (rotationFrames2 - rotationFrames1 - 1);
			easedRotationSpeed = dodgeRotationSpeed * MakeMovementCubic(t) * delta;
			cam.RotateZ(-easedRotationSpeed * direction);
			rig.ApplyImpulse(new Vector3(1 * direction, 0, 0) * dodgeMovementSpeed);
		}
		if (dodge2 && frameCounter == rotationFrames2 + 1)
		{
			dodge2 = false;
			obstacleDetected = false;
			frameCounter = 0;
			rig.LinearVelocity = Vector3.Zero;
            rig.AngularVelocity = Vector3.Zero;
			rig.GlobalPosition = new Vector3(0, rig.GlobalPosition.Y, rig.GlobalPosition.Z);
			resetDetections();
		}
	}


	void Dodge(int leftRight, float delta) {
		float t;
		float easedRotationSpeed;


		if (dodge1 && frameCounter <= rotationFrames1)
		{
			t = (float)frameCounter / rotationFrames1;
			easedRotationSpeed = dodgeRotationSpeed * MakeMovementCubic(t) * delta;
			cam.RotateZ(easedRotationSpeed * leftRight);
			rig.ApplyImpulse(new Vector3(-1 * leftRight, 0, 0) * dodgeMovementSpeed);
		}
		if (dodge1 && frameCounter == rotationFrames1 + 1)
		{
			dodge1 = false;
			dodge2 = true;
			rig.LinearVelocity = Vector3.Zero;
            rig.AngularVelocity = Vector3.Zero;
		}
		if (dodge2 && frameCounter <= rotationFrames2)
		{
			t = (float)(frameCounter - rotationFrames1 - 1) / (rotationFrames2 - rotationFrames1 - 1);
			easedRotationSpeed = dodgeRotationSpeed * MakeMovementCubic(t) * delta;
			cam.RotateZ(-easedRotationSpeed * leftRight);
			rig.ApplyImpulse(new Vector3(1 * leftRight, 0, 0) * dodgeMovementSpeed);
		}

		if (dodge2 && frameCounter == rotationFrames2 + 1)
		{
			dodge2 = false;
			obstacleDetected = false;
			frameCounter = 0;
			rig.LinearVelocity = Vector3.Zero;
            rig.AngularVelocity = Vector3.Zero;
			rig.GlobalPosition = new Vector3(0, rig.GlobalPosition.Y, rig.GlobalPosition.Z);
			resetDetections();
		}
		

		if (dodge1 || dodge2)
		{
			frameCounter++;
		}
	}

	void DodgeUpDown(int upDown, float delta){
		float t;
		float easedRotationSpeed;

		if (dodge1 && frameCounter <= rotationFrames1)
		{
			t = (float)frameCounter / rotationFrames1;
			easedRotationSpeed = dodgeRotationSpeedUpDown * MakeMovementCubic(t) * delta;
			cam.RotateX(easedRotationSpeed * upDown);
			rig.ApplyImpulse(new Vector3(0, upDown, 0) * dodgeMovementSpeed);
		}
		if (dodge1 && frameCounter == rotationFrames1 + 1)
		{
			dodge1 = false;
			dodge2 = true;
			rig.LinearVelocity = Vector3.Zero;
            rig.AngularVelocity = Vector3.Zero;
		}
		if (dodge2 && frameCounter <= rotationFrames2)
		{
			t = (float)(frameCounter - rotationFrames1 - 1) / (rotationFrames2 - rotationFrames1 - 1);
			easedRotationSpeed = dodgeRotationSpeedUpDown * MakeMovementCubic(t) * delta;
			cam.RotateX(-easedRotationSpeed * upDown);
			rig.ApplyImpulse(new Vector3(0, -upDown, 0) * dodgeMovementSpeed);
		}
		if (dodge2 && frameCounter == rotationFrames2 + 1)
		{
			dodge2 = false;
			obstacleDetected = false;
			frameCounter = 0;
			rig.LinearVelocity = Vector3.Zero;
            rig.AngularVelocity = Vector3.Zero;
			rig.GlobalPosition = new Vector3(rig.GlobalPosition.X, 0, rig.GlobalPosition.Z);
			resetDetections();
		}
		

		if (dodge1 || dodge2)
		{
			frameCounter++;
		}
	}


	float MakeMovementCubic(float t)
	{
		if (t < 0.5f)
			return 4 * t * t * t;
		return 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
	}


}

