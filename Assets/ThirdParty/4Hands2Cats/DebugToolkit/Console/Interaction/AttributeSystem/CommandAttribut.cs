using DebugToolkit.Console.Log;
using DebugToolkit.Interaction.Commands;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DebugToolkit.Console.Interaction.AttributeSystem
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribut : Attribute
    {
        public string CommandName { get; }

        public CommandAttribut(string commandName)
        {
            CommandName = commandName;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BooleanCommandAttribute : CommandAttribut
    {
        public string TrueWord { get; }
        public string FalseWord { get; }
        public BooleanCommandAttribute(string commandName, string trueWord = "Enable", string falseWord = "Disable") : base(commandName)
        {
            TrueWord = trueWord;
            FalseWord = falseWord;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VectorCommandAttribute : CommandAttribut
    {
        public VectorCommandAttribute(string commandName) : base(commandName) 
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SimpleCommandAttribute : CommandAttribut
    {
        public SimpleCommandAttribute(string commandName) : base(commandName) { }
    }
}
