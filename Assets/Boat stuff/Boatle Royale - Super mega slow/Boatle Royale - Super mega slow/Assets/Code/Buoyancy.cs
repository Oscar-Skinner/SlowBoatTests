using System;
using System.Collections;
using UnityEngine;

// Cams mostly hack buoyancy
public class Buoyancy : MonoBehaviour
{
	public byte splashVelocityThreshold;
	public float forceScalar;
	public byte waterLineHack; // HACK

	private byte underwaterVerts;
	public float dragScalar;

	public static event Action<GameObject, Vector3, Vector3> OnSplash;
	public static event Action<GameObject> OnDestroyed;

	private Vector3 worldVertPos;

	public Rigidbody rb;
	public Mesh meshFilterMesh;

	
	private Vector3[] meshVerticies;
	private Vector3[] meshNormals;
	private Vector3 rbVelocity;
	
	private Vector3 transformPos;
	
	private float rbDragValue;
	private Vector3 forceAmount;
	private Vector3 forcePosition;
	private byte meshNormalsLength;

	private void Awake()
	{
		meshVerticies = meshFilterMesh.vertices;
		meshNormals = meshFilterMesh.normals;
		rbVelocity = rb.velocity;
		meshNormalsLength = (byte)meshNormals.Length;
	}

	void Update()
	{
		transformPos = transform.position;
		CalculateForces();
	}

	
	private void CalculateForces()
	{
		underwaterVerts = 0;

		for (byte index = 0; index < meshNormalsLength; index++)
		{
			worldVertPos = transformPos + transform.TransformDirection(meshVerticies[index]);
			if (worldVertPos.y < waterLineHack)
			{
				// Splashes only on surface of water plane
				if (worldVertPos.y > waterLineHack - 0.1f)
				{
					if (rbVelocity.magnitude > splashVelocityThreshold || rb.angularVelocity.magnitude > splashVelocityThreshold)
					{
						//print(rbVelocity.magnitude);
						if (OnSplash != null)
						{
							OnSplash.Invoke(gameObject, worldVertPos, rbVelocity);
						}
					}
				}
				forceAmount = (transform.TransformDirection(-meshNormals[index]) * forceScalar) * Time.deltaTime;
				forcePosition = transformPos + transform.TransformDirection(meshVerticies[index]);
				rb.AddForceAtPosition(forceAmount, forcePosition, ForceMode.Force);
				underwaterVerts++;
			}
			// HACK to remove sunken boats
			if (worldVertPos.y < waterLineHack - 10f)
			{
				DestroyParentGO();
				break;
			}
			// Drag for percentage underwater
			rbDragValue = (underwaterVerts / (float)meshVerticies.Length) * dragScalar;
			rb.drag = rbDragValue;
			rb.angularDrag = rbDragValue;
		}
	}

	private void DestroyParentGO()
	{
		if (OnDestroyed != null)
		{
			OnDestroyed.Invoke(gameObject);
		}
		
		//maybe a coroutine to destroy it :shrug:
		Destroyer.boatsToDestroy.Add(gameObject);
		gameObject.SetActive(false);
	}
}
