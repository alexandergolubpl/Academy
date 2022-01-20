using UnityEngine;

namespace AlexDoveTools
{
    public static class AssetToolsExt
    {
        public static void RoundPos(this Transform t, int roundFactor)
        {
            UnityEditor.Undo.RecordObject(t, "RoundPos");
            Vector3 v3 = t.position;
            v3.x = (float) System.Math.Round(v3.x, roundFactor);
            v3.y = (float) System.Math.Round(v3.y, roundFactor);
            v3.z = (float) System.Math.Round(v3.y, roundFactor);
            t.position = v3;
        }
    }
}