using UnityEngine;

namespace Fusion.Frameworks.Assets.Editor
{
    [CreateAssetMenu]
    public class AtlasProperty : ScriptableObject
    {
        // 图集粒度  -1 整个文件夹打一个图集  0  不打图集  +n n张sprite打一个图集
        // 一个图集会打成单独的AssetBundle包
        [SerializeField]
        public int packUnit = 0;

        // 过滤分辨率  大于等于这个分辨率的sprite会忽略打图集
        [SerializeField]
        public Vector2 ignoreSize = new Vector2(512, 512);
    }
}
