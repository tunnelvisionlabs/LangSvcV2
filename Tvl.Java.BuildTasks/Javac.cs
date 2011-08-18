namespace Tvl.Java.BuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Build.Utilities;
    using Microsoft.Build.Tasks;
    using Microsoft.Build.Framework;
    using System.Text.RegularExpressions;
    using Path = System.IO.Path;
    using System.Diagnostics.Contracts;

    public class Javac : ToolTask
    {
        private readonly List<ITaskItem> _generatedClassFiles = new List<ITaskItem>();
        private readonly List<string> _previousErrorLines = new List<string>();

        public Javac()
        {
            ShowDeprecation = false;
            NoWarnings = false;
            Verbose = false;
        }

        public bool ShowDeprecation
        {
            get;
            set;
        }

        public bool NoWarnings
        {
            get;
            set;
        }

        public bool Verbose
        {
            get;
            set;
        }

        public string OutputPath
        {
            get;
            set;
        }

        public string Encoding
        {
            get;
            set;
        }

        public string SourceRelease
        {
            get;
            set;
        }

        public string TargetRelease
        {
            get;
            set;
        }

        public string[] ClassPath
        {
            get;
            set;
        }

        public ITaskItem[] References
        {
            get;
            set;
        }

        public ITaskItem[] ResponseFiles
        {
            get;
            set;
        }

        public ITaskItem[] Sources
        {
            get;
            set;
        }

        [Output]
        public ITaskItem[] GeneratedClassFiles
        {
            get
            {
                return _generatedClassFiles.ToArray();
            }
        }

        protected override Encoding ResponseFileEncoding
        {
            get
            {
                return new UTF8Encoding(false);
            }
        }

        protected override string ToolName
        {
            get
            {
                return "javac.exe";
            }
        }

        protected override string GenerateFullPathToTool()
        {
            return @"C:\Program Files (x86)\Java\jdk1.6.0_26\bin\javac.exe";
        }

        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilderExtension commandLine = new CommandLineBuilderExtension();
            AddCommandLineCommands(commandLine);
            return commandLine.ToString();
        }

        protected override string GenerateResponseFileCommands()
        {
            CommandLineBuilderExtension commandLine = new CommandLineBuilderExtension();
            AddResponseFileCommands(commandLine);
            return commandLine.ToString();
        }

        protected void AddCommandLineCommands(CommandLineBuilderExtension commandLine)
        {
        }

        protected void AddResponseFileCommands(CommandLineBuilderExtension commandLine)
        {
            if (ShowDeprecation)
                commandLine.AppendSwitch("-deprecation");
            if (NoWarnings)
                commandLine.AppendSwitch("-nowarn");

            commandLine.AppendSwitch("-verbose");
            commandLine.AppendSwitchIfNotNull("-encoding ", Encoding);
            commandLine.AppendSwitch("-g");
            commandLine.AppendSwitchIfNotNull("-source ", SourceRelease);
            commandLine.AppendSwitchIfNotNull("-target ", TargetRelease);
            commandLine.AppendSwitchIfNotNull("-d ", OutputPath);
            commandLine.AppendSwitch("-Xlint:unchecked");
            commandLine.AppendSwitchIfNotNull("-classpath ", ClassPath, ";");

            commandLine.AppendFileNamesIfNotNull(Sources, " ");
        }

        private static readonly Regex CompileMessageFormat = new Regex(@"^(?<File>[\w\\/\.]+):(?<Line>[0-9]+):(?<Warning> warning:)? (?:\[(?<Category>\w+)\] )?(?<Message>.*)$", RegexOptions.Compiled);

        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            if (_previousErrorLines.Count > 0)
            {
                if (singleLine.Trim() == "^")
                {
                    Match result = CompileMessageFormat.Match(_previousErrorLines[0]);
                    Contract.Assert(result.Success);

                    string subcategory = null;
                    string warningCode = null;
                    string helpKeyword = null;
                    string file = null;

                    int lineNumber = 0;
                    int columnNumber = 0;
                    int endLineNumber = 0;
                    int endColumnNumber = 0;

                    string message = null;
                    object[] messageArgs = null;

                    Group fileGroup = result.Groups["File"];
                    Group lineGroup = result.Groups["Line"];
                    Group warningGroup = result.Groups["Warning"];
                    Group categoryGroup = result.Groups["Category"];
                    Group messageGroup = result.Groups["Message"];

                    file = Path.GetFullPath(fileGroup.Value);

                    message = messageGroup.Value;
                    if (_previousErrorLines.Count > 2)
                        message += ", " + string.Join(", ", _previousErrorLines.Skip(1).Take(_previousErrorLines.Count - 2));

                    if (categoryGroup.Success)
                        subcategory = categoryGroup.Value;

                    if (!int.TryParse(lineGroup.Value, out lineNumber))
                        lineNumber = 0;
                    endLineNumber = lineNumber;

                    columnNumber = singleLine.IndexOf('^');
                    endColumnNumber = columnNumber;

                    //Log.LogWarning(subcategory, warningCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, messageArgs);
                    Action<string, string, string, string, int, int, int, int, string, object[]> logFunction;
                    if (warningGroup.Success)
                        logFunction = Log.LogWarning;
                    else
                        logFunction = Log.LogError;

                    logFunction(subcategory, warningCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, messageArgs);

                    _previousErrorLines.Clear();
                }
                else
                {
                    _previousErrorLines.Add(singleLine);
                }

                return;
            }

            if (Verbose && singleLine.StartsWith("[") && singleLine.EndsWith("]"))
            {
                base.LogEventsFromTextOutput(singleLine, messageImportance);
            }

            if (singleLine.StartsWith("Note: "))
            {
                Log.LogWarning("{0}", singleLine);
            }
            else if (singleLine.StartsWith("[wrote "))
            {
                int startIndex = "[wrote ".Length;
                string outputFile = singleLine.Substring(startIndex, singleLine.Length - startIndex - 1);
                _generatedClassFiles.Add(new TaskItem(outputFile));
            }
            else if (CompileMessageFormat.IsMatch(singleLine))
            {
                _previousErrorLines.Add(singleLine);
            }
            else if (!singleLine.StartsWith("[") || !singleLine.EndsWith("]"))
            {
                base.LogEventsFromTextOutput(singleLine, messageImportance);
            }
        }
    }
}
