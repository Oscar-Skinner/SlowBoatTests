using Unity.Entities; //Namespace for working with ECS components and systems.
using UnityEngine; //Namespace for Unity-specific functionality.

//Define a struct for the Wobbler component implementing IComponentData.
public struct Wobbler : IComponentData
{
    public float Speed; 
    public float intensity; 
    public float timeOffsetY; 
}

//Define a MonoBehaviour class for the WobblerAuthoring script.
public class WobblerAuthoring : MonoBehaviour
{
    //Public variables to specify the parameters for the wobbling movement in the Unity Inspector.
    public float speed = 1; 
    public float intensity = 1; 
    public float timeOffsetY = 0; 

    //Define a nested class Baker for baking the WobblerAuthoring component data into an entity.
    class Baker : Baker<WobblerAuthoring>
    {
        //Override the Bake method to convert and add the Wobbler component data to the entity.
        public override void Bake(WobblerAuthoring src)
        {
            //Create a new Wobbler component and assign the values from WobblerAuthoring.
            var data = new Wobbler
            {
                Speed = src.speed,
                intensity = src.intensity,
                timeOffsetY = src.timeOffsetY
            };

            //Add the Wobbler component to the entity with TransformUsageFlags.Dynamic flag.
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), data);
        }
    }
}