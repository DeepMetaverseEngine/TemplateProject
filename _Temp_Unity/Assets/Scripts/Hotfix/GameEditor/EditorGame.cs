using _Temp_.Battle.Runtime;
using DeepCore.Game3D.Slave.Runtime;
using DeepMetaGame.Data.ZoneEditor;
using DeepMetaGame.Unity.Preview;

public class EditorGame : BaseBattleRoot
{
    protected override AbstractBattle CreateBattleRuntime(SceneData sceneData)
    {
        return new _Temp_BattleRuntime(
                    Templates,
                    sceneData);
    }
}
