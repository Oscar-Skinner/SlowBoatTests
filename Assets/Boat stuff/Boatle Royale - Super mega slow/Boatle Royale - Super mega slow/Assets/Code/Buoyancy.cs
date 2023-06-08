using System;
using System.Collections;
using UnityEngine;

// Cams mostly hack buoyancy
public class Buoyancy : MonoBehaviour
{
	public float splashVelocityThreshold;
	public float forceScalar;
	public float waterLineHack; // HACK

	private int underwaterVerts;
	public float dragScalar;

	public static event Action<GameObject, Vector3, Vector3> OnSplash;
	public static event Action<GameObject> OnDestroyed;

	private Vector3 worldVertPos;

	public Rigidbody rb;
	public Mesh meshFilterMesh;

	
	private Vector3[] meshVerticies;
	private Vector3[] meshNormals;
	private Vector3 rbVelocity;
	//private Vector3 transPos;
	private float rbDragValue;
	private Vector3 forceAmount;
	private Vector3 forcePosition;
	private int meshNormalsLength;

	private void Awake()
	{
		meshVerticies = meshFilterMesh.vertices;
		meshNormals = meshFilterMesh.normals;
		rbVelocity = rb.velocity;
		//transPos = transform.position;
		meshNormalsLength = meshNormals.Length;
	}

	void Update()
	{
		CalculateForces();
	}

	private void CalculateForces()
	{
		underwaterVerts = 0;

		for (var index = 0; index < meshNormalsLength; index++)
		{
			worldVertPos = transform.position + transform.TransformDirection(meshVerticies[index]);
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
				forcePosition = transform.position + transform.TransformDirection(meshVerticies[index]);
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
		gameObject.SetActive(false);
		DestroyTheObject();
	}

	private IEnumerator DestroyTheObject()
	{
		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}
}
