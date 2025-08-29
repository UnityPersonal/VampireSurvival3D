using DebugToolkit.Console.Log;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace DebugToolkit.Interaction.Commands
{
    [CreateAssetMenu(fileName = "BooleanCommand", menuName = "DebugToolkit/Command/Command")]
    public class Command : ScriptableObject
    {
        [Header("KeyWord")]
        [SerializeField] protected List<string> keyword = new List<string>();
        protected CommandDto commandDto;
        public CommandDto DTO => commandDto;

        internal virtual CommandDto Init()
        {
            commandDto = new CommandDto(keyword);
            commandDto.OnCommandValid += () => OnCommandValid?.Invoke();
            return commandDto;
        }


        public virtual event Action OnCommandValid;

        public bool Evaluate(string prompt)
        {
            return commandDto.Evaluate(prompt);
        }

        public void PrintHelp()
        {
            commandDto.PrintHelp();
        }

        public class CommandDto
        {
            protected List<string> _keyword = new List<string>();
            public List<string> Keyword => _keyword;
            public event Action OnCommandValid;

            protected int _requestedLength = 0;
            protected MethodInfo _bindedMethod;
            public CommandDto(List<string> keyword)
            {
                this._keyword = keyword;
            }

            public CommandDto(List<string> keyword, MethodInfo bindedMethod)
            {
                this._keyword = keyword;
                this._bindedMethod = bindedMethod;
            }

            public virtual string GetKeyword()
            {
                return string.Join(" ", _keyword).ToLower();
            }

            public virtual bool Evaluate(string prompt)
            {
                string[] keywords = prompt.Split(" ");
                if (keywords.Length < _keyword.Count)
                    return false;

                _requestedLength = GetRequestedLength();
                bool valid = CheckValidity(keywords) && keywords.Length == _requestedLength;

                if (valid)
                {
                    OnCommandValid?.Invoke();
                    if (_bindedMethod != null)
                        _bindedMethod.Invoke(null, null);
                }

                return valid;
            }

            protected virtual int GetRequestedLength()
            {
                return _keyword.Count;
            }

            protected bool CheckValidity(string[] keywords)
            {
                for (int i = 0; i < _keyword.Count; i++)
                {
                    if (String.IsNullOrEmpty(keywords[i])) return false;

                    if (_keyword[i].ToLower() == keywords[i].ToLower())
                        //keyword[i].ToLower().Substring(0, 1) == keywords[i].ToLower().Substring(0, 1))
                        continue;
                    else
                        return false;
                }
                return true;
            }

            public virtual void PrintHelp()
            {
                DebugLog.Log("Thanks for using this toolkit \n" +
                    "To use the different commands of the toolkit please read the following instructions \n",
                    DebugLog.LogColor.White);
            }
        }
    }
}

