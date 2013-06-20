using System;
using System.Globalization;
using System.Text;
using Microsoft.Build.Framework;

namespace Microsoft.VisualStudio.Project
{
	partial class DesignTimeAssemblyResolution
	{
		/// <summary>
		/// Engine required by RAR, primarily for collecting logs
		/// </summary>
		private class MockEngine : IBuildEngine
		{
			private int messages = 0;
			private int warnings = 0;
			private int errors = 0;
			private StringBuilder log = new StringBuilder();
			private readonly bool enableLog = false;

			internal MockEngine(bool enableLog)
			{
				this.enableLog = enableLog;
			}

			public void RecordRARExecutionException(Exception ex)
			{
				if (!enableLog) return;

				log.Append(String.Format(CultureInfo.InvariantCulture, "{0}", ex.ToString()));
			}

			public void LogErrorEvent(BuildErrorEventArgs eventArgs)
			{
				if (eventArgs == null)
				{
					throw new ArgumentNullException("eventArgs");
				}

				if (!enableLog) return;

				if (eventArgs.File != null && eventArgs.File.Length > 0)
				{
					log.Append(String.Format(CultureInfo.InvariantCulture, "{0}({1},{2}): ", eventArgs.File, eventArgs.LineNumber, eventArgs.ColumnNumber));
				}

				log.Append("ERROR ");
				log.Append(eventArgs.Code);
				log.Append(": ");
				++errors;

				log.AppendLine(eventArgs.Message);
			}

			public void LogWarningEvent(BuildWarningEventArgs eventArgs)
			{
				if (eventArgs == null)
				{
					throw new ArgumentNullException("eventArgs");
				}

				if (!enableLog) return;

				if (eventArgs.File != null && eventArgs.File.Length > 0)
				{
					log.Append(String.Format(CultureInfo.InvariantCulture, "{0}({1},{2}): ", eventArgs.File, eventArgs.LineNumber, eventArgs.ColumnNumber));
				}

				log.Append("WARNING ");
				log.Append(eventArgs.Code);
				log.Append(": ");
				++warnings;

				log.AppendLine(eventArgs.Message);
			}

			public void LogCustomEvent(CustomBuildEventArgs eventArgs)
			{
				if (eventArgs == null)
				{
					throw new ArgumentNullException("eventArgs");
				}

				if (!enableLog) return;

				log.Append(eventArgs.Message);
				log.Append("\n");
			}

			public void LogMessageEvent(BuildMessageEventArgs eventArgs)
			{
				if (eventArgs == null)
				{
					throw new ArgumentNullException("eventArgs");
				}

				log.Append(eventArgs.Message);
				log.Append("\n");

				++messages;
			}

			public bool ContinueOnError
			{
				get { return false; }
			}

			public string ProjectFileOfTaskNode
			{
				get { return String.Empty; }
			}

			public int LineNumberOfTaskNode
			{
				get { return 0; }
			}

			public int ColumnNumberOfTaskNode
			{
				get { return 0; }
			}

			internal string Log
			{
				get { return log.ToString(); }
			}

			public bool BuildProjectFile(string projectFileName, string[] targetNames, System.Collections.IDictionary globalProperties, System.Collections.IDictionary targetOutputs)
			{
				throw new NotImplementedException();
			}
		}
	}
}
