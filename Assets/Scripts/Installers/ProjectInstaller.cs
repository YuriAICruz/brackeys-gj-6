using UnityEngine;
using Zenject;

namespace Graphene.Installers
{
    [CreateAssetMenu(fileName = "ProjectInstaller", menuName = "Installers/ProjectInstaller")]
    public class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        }
    }
}