using FluentIL.Common;
using Mono.Cecil.Cil;
using System;

namespace FluentIL.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly string _toolName;

        public ConsoleLogger(string toolName)
        {
	        this._toolName = toolName;
        }

        public virtual bool IsErrorThrown { get; private set; }

        public void Log(Rule rule, SequencePoint sp, params string[] messages)
        {
            var location = sp?.Document == null ? this._toolName :
                $"{sp.Document.Url}({sp.StartLine},{sp.StartColumn},{sp.EndLine},{sp.EndColumn})";

            var message = string.Format(rule.Message.ToString(), messages ?? new string[] { });

            switch (rule.Severity)
            {
                case RuleSeverity.Error:
	                this.WriteError(rule.Id, location, message);
	                this.IsErrorThrown = true; break;
                case RuleSeverity.Warning:
	                this.WriteWarning(rule.Id, location, message); break;
                case RuleSeverity.Info:
	                this.WriteInfo(location, message); break;
            }
        }

        private void WriteInfo(string location, string message)
        {
            Console.WriteLine($"{location}: {message}");
        }

        private void WriteWarning(string id, string location, string message)
        {
            Console.WriteLine($"{location}: warning {id}: {message}");
        }

        private void WriteError(string id, string location, string message)
        {
            Console.WriteLine($"{location}: error {id}: {message}");
        }
    }
}