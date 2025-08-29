using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DebugToolkit.Console.Interaction.AttributeSystem
{
    [System.Obsolete("This class is in preview and not ready for use.", true)]
    public class CommandAttributManager : MonoBehaviour
    {
        private static Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();
        private static Dictionary<string, object> commandInstances = new Dictionary<string, object>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RegisterCommands()
        {
            commands.Clear();
            commandInstances.Clear();

            foreach (var mono in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Type type = mono.GetType();
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var attribute = method.GetCustomAttribute<CommandAttribut>();
                    if (attribute != null)
                    {
                        string commandName = attribute.CommandName.ToLower();
                        commands[commandName] = method;
                        commandInstances[commandName] = mono;
                        Debug.Log($"[Console] Registered Command: '{commandName}' -> {method.Name} in {type.Name}");
                    }
                }
            }
        }

        public static void ExecuteCommand(string command)
        {
            string[] parts = command.Split(' ');
            string commandName = parts[0].ToLower();

            if (commands.TryGetValue(commandName, out MethodInfo method))
            {
                object instance = commandInstances[commandName];
                method.Invoke(instance, null);
                Debug.Log($"[Console] Executed command: {commandName}");
            }
            else
            {
                Debug.Log($"[Console] Unknown command: {commandName}");
            }
        }
    }
}

