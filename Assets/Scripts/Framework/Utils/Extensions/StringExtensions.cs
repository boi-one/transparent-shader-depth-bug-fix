using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public static class StringExtensions
    {

        public static bool ContainsAny(this string target, List<string> targetList)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                if (target.Contains(targetList[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CompareFirstCharacter(this string target, char targetChar)
        {
            return target[0] == targetChar;
        }


        public static string RemoveFirstCharacter(this string target)
        {
            return target.Remove(0, 1);;
        }
    
    }