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

	public static Action<GameObject, Vector3, Vector3> OnSplash;
	public static Action<GameObject> OnDestroyed;

	private Vector3 worldVertPos;

	public Rigidbody rb;
	public Mesh meshFilterMesh;
	
	private Vector3[] meshVerticies;
	private Vector3[] meshNormals;
	private Vector3 rbVelocity;
	
	private Vector3 transformPos;
	private Quaternion transformRotate;
	private float rbDragValue;
	public Vector3 forceAmount;
	private Vector3 forcePosition;
	private byte meshNormalsLength;
	private int meshVerticiesLength;

	private void Awake()
	{
		meshVerticies = meshFilterMesh.vertices;
		meshNormals = meshFilterMesh.normals;
		rbVelocity = rb.velocity;
		meshNormalsLength = (byte)meshNormals.Length;
		meshVerticiesLength = meshVerticies.Length;
	}

	void Update()
	{
		transformPos = transform.position;
		transformRotate = transform.rotation;
		CalculateForces();
	}
	
	private void CalculateForces()
	{
		underwaterVerts = 0;

		for (byte index = 0; index < meshNormalsLength; index++)
		{
			worldVertPos = transformPos + (transformRotate * meshVerticies[index]);

			// Splashes only on surface of water plane
			if (worldVertPos.y < waterLineHack - 0.1f)
			{
				// if (rbVelocity.magnitude > splashVelocityThreshold || rb.angularVelocity.magnitude > splashVelocityThreshold)
				// {
				// 	OnSplash.Invoke(gameObject, worldVertPos, rbVelocity);
				// }
				//
				forceAmount = -meshNormals[index] * (forceScalar * Time.deltaTime);
                forcePosition = transformPos + (transformRotate * meshVerticies[index]);
                rb.AddForceAtPosition(forceAmount, forcePosition, ForceMode.Force);
                underwaterVerts++;
			}
			
			if (worldVertPos.y < waterLineHack - 10f) // HACK to remove sunken boats
			{
				DestroyParentGO();
				break;
			}

			// Drag for percentage underwater
			rbDragValue = (underwaterVerts / (float)meshVerticiesLength) * dragScalar;
			rb.drag = rbDragValue;
			rb.angularDrag = rbDragValue;
		}
	}

	private void DestroyParentGO()
	{
		OnDestroyed.Invoke(gameObject);
		Destroyer.boatsToDestroy.Add(gameObject);
		gameObject.SetActive(false);
	}
}
