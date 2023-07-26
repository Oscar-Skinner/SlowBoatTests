using Unity.Entities; //Namespace for working with ECS components and systems.
using Unity.Mathematics; //Namespace for math functions.
using Unity.Transforms; //Namespace for accessing Transform components.
using UnityEngine; //Namespace for Unity-specific functionality.


//Define a partial struct for the WobblerSystem implementing ISystem.
public partial struct WobblerSystem : ISystem 
{
    //The OnUpdate method will be called on each frame.
    public void OnUpdate(ref SystemState state) 
    {
        //Iterate over each entity that has both a Wobbler (read-only) and LocalTransform (read-write) component.
        foreach (var (wobbler, xform) in SystemAPI.Query<RefRO<Wobbler>, RefRW<LocalTransform>>())
        {
            //Calculate the movement offset along the Y-axis using coherent noise (cnoise) at the current time and wobbler parameters.
            //The coherent noise function (cnoise) generates a smooth noise value based on a 2D position. (couldnt use perlinNoise function)
            float offsetY = noise.cnoise(new float2(Time.time * wobbler.ValueRO.Speed + wobbler.ValueRO.timeOffsetY, 0)) * wobbler.ValueRO.intensity;

            //Apply the calculated movement offset to the cube's position along the Y-axis.
            xform.ValueRW.Position.y = offsetY;
        }
    }
}