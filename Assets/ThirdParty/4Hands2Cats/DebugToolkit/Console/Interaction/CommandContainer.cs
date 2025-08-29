using DebugToolkit.Console.Interaction.AttributeSystem;
using DebugToolkit.Interaction.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace DebugToolkit.Interaction.Commands
{
    [CreateAssetMenu(fileName = "CommandContainer", menuName = "DebugToolkit/Command/CommandContainer")]
    internal class CommandContainer : ScriptableObject
    {
        [SerializeField] private List<Command> commands;
        private static List<Command.CommandDto> commandsDtos;
        private static List<Command.CommandDto> staticCommandsDtos;

        private static bool STATIC_COMMAND_READY;

        public async Task<List<Command.CommandDto>> GetCommands()
        {
            while (!STATIC_COMMAND_READY)
            {
                await Awaitable.WaitForSecondsAsync(0.1f);
            }

            List<Command.CommandDto> result = new();
            result.AddRange(staticCommandsDtos);
            result.AddRange(commandsDtos);

            return result;
        }

        internal void Init()
        {
            commandsDtos = new List<Command.CommandDto>();

            for (int i = 0; i < commands.Count; i++)
            {
                commandsDtos.Add(commands[i].Init());
            }
        }

        internal void Eval(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            for (int i = 0; i < commandsDtos.Count; i++)
            {
                if (commandsDtos[i].Evaluate(text))
                    return;
            }
            for(int i = 0;i < staticCommandsDtos.Count;i++)
            {
                if (staticCommandsDtos[i].Evaluate(text))
                    return;
            }
        }

        internal void PrintHelp(List<Command.CommandDto> toIgnore)
        {
            for (int i = 0; i < commandsDtos.Count; i++)
            {
                if (toIgnore.Contains(commandsDtos[i]))
                    continue;

                commands[i].PrintHelp();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private async static void RegisterAttributCommands()
        {
            STATIC_COMMAND_READY = false;
            staticCommandsDtos = new List<Command.CommandDto>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                if (assembly.FullName.Contains("Unity") || assembly.FullName.Contains("mscorlib"))
                    continue;

                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        var attribute = (CommandAttribut)Attribute.GetCustomAttribute(method, typeof(CommandAttribut));
                        if (attribute != null)
                        {
                            switch(attribute)
                            { 
                                case SimpleCommandAttribute simpleCommand:
                                    staticCommandsDtos.Add(new Command.CommandDto(new List<string>() { simpleCommand.CommandName.ToLower() }, method));
                                    break;

                                case BooleanCommandAttribute booleanCommand:
                                    staticCommandsDtos.Add(new BooleanCommand.BooleanCommandDto(new List<string>() { booleanCommand.CommandName.ToLower() },
                                        new List<string>() { booleanCommand.TrueWord },
                                        new List<string>() { booleanCommand.FalseWord },
                                        method));
                                    break;

                                case VectorCommandAttribute vectorCommand:
                                    staticCommandsDtos.Add(new VectorCommand.VectorCommandDto(new List<string>() { vectorCommand.CommandName.ToLower() },
                                        method));
                                    break;
                            }

                            string commandName = attribute.CommandName.ToLower();
                            Debug.Log($"[Console] Registered Command: '{commandName}' -> {method.Name} in {type.Name}");
                        }
                    }
                }

                await Awaitable.WaitForSecondsAsync(0.05f);
            }
            STATIC_COMMAND_READY = true;
        }
    }
}
