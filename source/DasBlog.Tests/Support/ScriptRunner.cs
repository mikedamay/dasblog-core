using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Tests.Support
{
	public class ScriptRunner : IScriptRunner
	{
		public IReadOnlyDictionary<string, string> DefaultEnv => defaultEnv;
		private readonly Dictionary<String, string> defaultEnv = new Dictionary<string, string>(); 
		private ILogger<ScriptRunner> logger;
		private readonly string scriptDirectory;
		private readonly int scriptTimeout = 5_000;
		private readonly int scriptExitTimeout = 10;

		public ScriptRunner(IOptions<ScriptRunnerOptions> opts, ILogger<ScriptRunner> logger)
		{
			this.scriptDirectory = opts.Value.ScriptDirectory;
			this.scriptTimeout = opts.Value.ScriptTimeout;
			this.scriptExitTimeout = opts.Value.ScriptExitTimeout;
			this.logger = logger;
		}

		/// <inheritdoc cref="DasBlog.Tests.Support.Interfaces"/>
		/// some unpleasantness with capturing output
		/// The following applied when only "echo" statements appeared in the script.
		/// cmd /C - no output was returned
		/// cmd /K with "exit" in the script - no output was returned
		/// cmd /K with "exit /b" in the script - output was returned but the process timed out.
		/// added the statement "set" on the line after "echo" and the echoed expressions appeared.
		/// conclusion: the script exits before the process has an opportunity to get the output
		/// DON'T have scripts that comprise only "echo" statements
		public (int exitCode, string[] output, string[] errors) Run(
			string scriptName, IReadOnlyDictionary<string, string> envirnmentVariables,
			params object[] arguments)
		{
			try
			{
				var cmdexe = GetCmdExe();
				var output = new List<string>();
				var errs = new List<string>();
				var sw = new Stopwatch();
				sw.Start();
				
				ProcessStartInfo psi = new ProcessStartInfo(cmdexe);
				var scriptPathAndFileName = Path.Combine(scriptDirectory, scriptName);
				psi.UseShellExecute = false;
				SetArguments(psi.ArgumentList
					, new string[]
					{
						"/K", scriptPathAndFileName, scriptExitTimeout.ToString()
					}.Concat(arguments).ToArray());
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardError = true;
				logger.LogDebug($"script timeout: {scriptTimeout}, script exit delay {scriptExitTimeout}ms for {scriptPathAndFileName}");

				var exitCode = RunCmdProcess(psi, output, errs);
				logger.LogDebug($"elapsed time {sw.ElapsedMilliseconds} on thread {Thread.CurrentThread.ManagedThreadId}");
				
				ThrowExceptionForBadExitCode(exitCode, scriptPathAndFileName, scriptTimeout, psi);
				ThrowExceptionForIncompleteOutput(output, errs, scriptName);
				return (exitCode, output.Where(o => o != null && !o.Contains("dasmeta")).ToArray(), errs.Where(e => e != null && !e.Contains("dasmeta")).ToArray());
			}
//			finally
			catch (Exception e)
			{
				throw new Exception(e.Message, e);
			}
		}

		/// <summary>
		/// start the process and collect std output until it dies or the timeout is run down
		/// </summary>
		/// <param name="psi">RedirectStandardOutput/Error set to true, Shell
		///   Execute=false, fully loaded arglist including /K to keep the command shell open</param>
		/// <param name="output">each line of output (terminated with a null line) or empty list</param>
		/// <param name="errs">each line of output (terminated with a null line) or empty list</param>
		/// <returns>exit code from script (which is whatever the last executed step exits with
		///   or 1 if the args are wrong) or int.MaxValue - 1 to indicate timeout</returns>
		private int RunCmdProcess(ProcessStartInfo psi, List<string> output, List<string> errs)
		{
			int exitCode = int.MaxValue;
			using (var ps = Process.Start(psi))
			{
				ps.OutputDataReceived += (sender, e) => output.Add(e.Data);
				ps.ErrorDataReceived += (sender, e) => errs.Add(e.Data);
				ps.BeginOutputReadLine();
				ps.BeginErrorReadLine();
				var result = ps.WaitForExit(scriptTimeout);
				exitCode = result ? ps.ExitCode : int.MaxValue - 1;
			}

			logger.LogDebug($"exit code: {exitCode}");
			return exitCode;
		}

		private int RunCmdProcess_Works(ProcessStartInfo psi, List<string> output, List<string> errs)
		{
			int exitCode = int.MaxValue;
			Process ps;
			using (ps = Process.Start(psi))
			{
				string s;
				do
				{
					s = ps.StandardOutput.ReadLine();
					if (s != null)
					{
						output.Add(s);
					}
				} while (s != null);

				var result = ps.WaitForExit(scriptTimeout);
				exitCode = result ? ps.ExitCode : int.MaxValue - 1;
			}
			logger.LogDebug($"exit code: {exitCode}");
			return exitCode;
		}

		private int RunCmdProcess_Old(ProcessStartInfo psi, List<string> output, List<string> errs)
		{
			int exitCode = int.MaxValue;
			Process ps;
			using (ps = Process.Start(psi))
			{
				string s;
				do
				{
					s = ps.StandardOutput.ReadLine();
					if (s != null)
					{
						output.Add(s);
					}
				} while (s != null);
				var result = ps.WaitForExit(scriptTimeout);
				exitCode = result ? ps.ExitCode : int.MaxValue - 1;
			}
			logger.LogDebug($"exit code: {exitCode}");
			return exitCode;
		}
		private int RunCmdProcess_latest(ProcessStartInfo psi, List<string> output, List<string> errs)
		{
			int exitCode = int.MaxValue;
			Process ps;
			bool readStdOut = true;
			bool readStdErr = true;
			bool stdOutFinished = false, stdErrFinished = false;
			Task<string> stdOutTask = null, stdErrTask = null;
			int outputTaskId = 0, errorsTaskId = 0;
			
			using (ps = Process.Start(psi))
			{
				string str;
				do
				{
					if (readStdOut)
					{
						stdOutTask = ps.StandardOutput.ReadLineAsync();
						outputTaskId = stdOutTask.Id;
						readStdOut = false;
					}

					if (readStdErr)
					{
						stdErrTask = ps.StandardError.ReadLineAsync();
						errorsTaskId = stdErrTask.Id;
						readStdErr = false;
					}
					if (stdOutTask.Id == stdErrTask.Id)
					{
						throw new Exception($"ScriptRunner: test aborted as task ids are not unique - {stdOutTask.Id}");
					}
					var tasks = Task.WhenAny(new[] {stdOutTask, stdErrTask});
					if (tasks.IsCanceled || tasks.IsFaulted)
					{
						throw new Exception("ScriptRunner: test aborted as TaskWhenAny() failed");
					}
					var completedTask = tasks.Result;
					if (!completedTask.IsCompletedSuccessfully)
					{
						throw new Exception($"ScriptRunner: test aborted as TaskWhenAny() failed - for task {completedTask.Id}");
					}
					str = completedTask.Result;
					if (completedTask.Id == outputTaskId)
					{
						output.Add(str);
						readStdOut = true;
						if (str == null)
						{
							stdOutFinished = true;
						}
					}
					else if (completedTask.Id == errorsTaskId)
					{
						errs.Add(str);
						readStdErr = true;
						if (str == null)
						{
							stdErrFinished = true;
						}
					}
				} while (!stdOutFinished && !stdErrFinished);
				var result = ps.WaitForExit(scriptTimeout);
				exitCode = result ? ps.ExitCode : int.MaxValue - 1;
			}

			logger.LogDebug($"exit code: {exitCode}");
			return exitCode;
		}

		private void ThrowExceptionForIncompleteOutput(List<string> output, List<string> errs, string scriptName)
		{
			output.Where(o => o != null).ToList().ForEach(o => logger.LogDebug(o));
			errs.Where(o => o != null).ToList().ForEach(o => logger.LogDebug(o));
			if (!(output.Contains("dasmeta_output_complete") || errs.Contains("dasmeta_errors_complete")))
			{
				throw new Exception($"incomplete output from script {scriptName}");
			}
		}

		private void ThrowExceptionForBadExitCode(int exitCode, string scriptName, int timeout, ProcessStartInfo psi)
		{
			string message = string.Empty;
			switch (exitCode)
			{
				case Constants.ScriptTimedOutCode:
					message =
						$"Execution of the script timed out after {timeout} milliseconds: {scriptName}" 
						+ Environment.NewLine + "This can be set (in milliseconds) through the environment variable "
						+ "DAS_BLOG_TEST_SCRIPT_TIMEOUT";
					break;
				case Constants.ScriptProcessFailedToRunCode:
					var cmdLine = psi.FileName + " " + string.Join(' ', psi.ArgumentList);
					message = $"failed to run the command line: {cmdLine}";
					break;
				default:
					// other exit codes are handled by the caller to Run
					return;
			}
			throw new Exception(message);
		}

		private void SetArguments(Collection<string> psiArgumentList, object[] arguments)
		{
			foreach (var arg in arguments)
			{
				psiArgumentList.Add((string)arg);
			}
		}

		private string GetCmdExe()
		{
			var cmdexe = Environment.GetEnvironmentVariable("ComSpec");
			if (string.IsNullOrWhiteSpace(cmdexe))
			{
				logger.LogInformation("comspec environment variable was empty - will use cmd.exe");
				return "cmd.exe";
			}

			return cmdexe;
		}
	}


	public class ScriptRunnerOptions
	{
		/// <summary>
		/// full path to the scripts directory
		/// </summary>
		public string ScriptDirectory { get; set; }
		/// <summary>
		/// timeout in milliseconds
		/// </summary>
		public int ScriptTimeout { get; set; }
		/// <summary>
		/// timeout in milliseconds
		/// </summary>
		public int ScriptExitTimeout { get; set; }
	}
}
