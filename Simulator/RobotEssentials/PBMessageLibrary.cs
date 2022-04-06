using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simulator.RobotEssentials
{
    class PBMessageLibrary
    {
        private static Dictionary<LlsfMsgs.AttentionMessage.Types.CompType, Type> MessageDict = new Dictionary<LlsfMsgs.AttentionMessage.Types.CompType, Type>();
        static Dictionary<LlsfMsgs.AttentionMessage.Types.CompType, Type> GetDictionary()
        {
            return MessageDict;
        }

        public static void CreateDictionary()
        {

            var typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "LlsfMsgs");
            /*for (int i = 0; i < typelist.Length; i++)
            {
                if(typelist[i].Name.Equals("<>c")|| typelist[i].Name.Equals("Types") || typelist[i].Name.Equals("CompType"))
                {
                    continue;
                }
                var element = typelist[i].Assembly;
                var newins = element.CreateInstance(typelist[i].Name);
                //MessageDict.Add(typelist[i].GetMethods(), typelist[i]);
                Console.WriteLine(typelist[i].Name);
            }*/
        }


        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }
    }
}

