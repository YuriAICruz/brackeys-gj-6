using System.Gameplay;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    public class Actor : MonoBehaviour
    {
        [Inject] private IPhysics _physics;
        
        
    }
}
