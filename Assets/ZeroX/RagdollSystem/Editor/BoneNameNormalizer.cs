using System.Linq;
using System.Text;
using UnityEngine;

namespace ZeroX.RagdollSystem.Editors
{
    public static class BoneNameNormalizer
    {
        public static string NormalizeBoneName(Transform bone, Avatar avatar)
        {
            string normalizeBoneName = "";
            foreach (var humanBone in avatar.humanDescription.human)
            {
                if (humanBone.boneName == bone.name)
                {
                    normalizeBoneName = humanBone.humanName;
                    break;
                }
            }

            
            if (string.IsNullOrEmpty(normalizeBoneName))
                normalizeBoneName = NormalizeBoneNameAddition(bone.name);
            

            normalizeBoneName = ConvertToSnakeCase(normalizeBoneName);
            return normalizeBoneName;
        }

        private static string ConvertToSnakeCase(string s)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (i == s.Length - 1 && c == 's')
                    continue;

                if (char.IsUpper(c))
                {
                    if (i > 0)
                        sb.Append("_");
                }

                sb.Append(char.ToLower(c));
            }

            return sb.ToString();
        }


        private static string NormalizeBoneNameAddition(string boneName)
        {
            if (TryNormalizeBoneNameAddition(boneName, hip_Variant))
                return hip_Normalized;
            
            if (TryNormalizeBoneNameAddition(boneName, spine_Variant))
                return spine_Normalized;
            
            if (TryNormalizeBoneNameAddition(boneName, chest_Variant))
                return chest_Normalized;
            
            if (TryNormalizeBoneNameAddition(boneName, upperChest_Variant))
                return upperChest_Normalized;


            return "";
        }

        private static bool TryNormalizeBoneNameAddition(string boneName, string[] variantNames)
        {
            boneName = boneName.ToLower().Replace(" ", "").Replace("_", "");
            if (variantNames.Any(v => boneName.Contains(v)))
                return true;

            return false;
        }


        private static string hip_Normalized = "hip";
        private static string[] hip_Variant = {"hip", "pelvis"};
        
        private static string spine_Normalized = "spine";
        private static string[] spine_Variant = new string[] {"spine", "spine1"};
        
        private static string chest_Normalized = "chest";
        private static string[] chest_Variant = new string[] {"chest", "spine2"};
        
        private static string upperChest_Normalized = "upper_chest";
        private static string[] upperChest_Variant = new string[] {"upper_chest", "spine3"};
    }
}