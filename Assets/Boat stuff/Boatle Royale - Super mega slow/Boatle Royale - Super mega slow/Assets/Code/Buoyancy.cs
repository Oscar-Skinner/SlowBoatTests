using System;
using UnityEngine;

// Cams mostly hack buoyancy
public class Buoyancy : MonoBehaviour
{
	public float splashVelocityThreshold;
	public float forceScalar;
	public float waterLineHack; // HACK

	public int underwaterVerts;
	public float dragScalar;

	public static event Action<GameObject, Vector3, Vector3> OnSplash;
	public static event Action<GameObject> OnDestroyed;

	Vector3 worldVertPos;

	public Rigidbody rb;
	public Mesh meshFilterMesh;

	public Vector3[] meshVerticies;
	public Vector3[] meshNormals;

	private void Awake()
	{
		meshVerticies = meshFilterMesh.vertices;
		meshNormals = meshFilterMesh.normals;
	}

	void Update()
	{
		CalculateForces();
	}

	private void CalculateForces()
	{
		underwaterVerts = 0;

		for (var index = 0; index < meshNormals.Length; index++)
		{
			worldVertPos = transform.position + transform.TransformDirection(meshVerticies[index]);
			if (worldVertPos.y < waterLineHack)
			{
				// Splashes only on surface of water plane
				if (worldVertPos.y > waterLineHack - 0.1f)
				{
					if (rb.velocity.magnitude > splashVelocityThreshold || rb.angularVelocity.magnitude > splashVelocityThreshold)
					{
						print(rb.velocity.magnitude);
						if (OnSplash != null)
						{
							OnSplash.Invoke(gameObject, worldVertPos, rb.velocity);
						}
					}
				}
				Vector3	forceAmount = (transform.TransformDirection(-meshNormals[index]) * forceScalar) * Time.deltaTime;
				Vector3 forcePosition = transform.position + transform.TransformDirection(meshVerticies[index]);
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
			rb.drag = (underwaterVerts / (float)meshVerticies.Length) * dragScalar;
			rb.angularDrag = (underwaterVerts / (float)meshVerticies.Length) * dragScalar;
		}
	}

	private void DestroyParentGO()
	{
		if (OnDestroyed != null)
		{
			OnDestroyed.Invoke(gameObject);
		}
		Destroy(gameObject);
	}
}
