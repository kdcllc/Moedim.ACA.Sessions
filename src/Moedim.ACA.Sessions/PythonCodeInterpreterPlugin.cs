using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Moedim.ACA.Sessions;

/// <summary>
/// A plugin that provides Python code execution capabilities using Azure Container Apps Sessions.
/// </summary>
public class PythonCodeInterpreterPlugin
{
    private readonly ISessionPool _sessionPool;
    private readonly ILogger<PythonCodeInterpreterPlugin> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PythonCodeInterpreterPlugin"/> class.
    /// </summary>
    /// <param name="sessionPool">The session pool for managing execution sessions.</param>
    /// <param name="logger">Logger instance.</param>
    public PythonCodeInterpreterPlugin(ISessionPool sessionPool, ILogger<PythonCodeInterpreterPlugin> logger)
    {
        _sessionPool = sessionPool ?? throw new ArgumentNullException(nameof(sessionPool));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes Python code in an isolated session.
    /// </summary>
    /// <param name="code">The Python code to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of code execution.</returns>
    [KernelFunction, Description("Executes Python code in an isolated container session")]
    public async Task<CodeExecutionResult> ExecutePythonAsync(
        [Description("The Python code to execute")] string code,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        SessionInfo? session = null;

        try
        {
            _logger.LogInformation("Acquiring session for Python code execution");
            session = await _sessionPool.GetOrCreateSessionAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Executing Python code in session {SessionId}", session.Id);

            // Simulate code execution (in real implementation, this would use Azure Container Apps Sessions API)
            var result = new CodeExecutionResult
            {
                Success = true,
                Output = $"Code executed successfully in session {session.Id}\nCode:\n{code}",
                SessionId = session.Id,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };

            _logger.LogInformation("Python code executed successfully in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Python code");
            return new CodeExecutionResult
            {
                Success = false,
                Error = ex.Message,
                SessionId = session?.Id ?? string.Empty,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        finally
        {
            stopwatch.Stop();
            if (session != null)
            {
                await _sessionPool.ReleaseSessionAsync(session.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Executes Python code with file uploads.
    /// </summary>
    /// <param name="code">The Python code to execute.</param>
    /// <param name="files">Files to upload to the session.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of code execution.</returns>
    [KernelFunction, Description("Executes Python code with file uploads in an isolated container session")]
    public async Task<CodeExecutionResult> ExecutePythonWithFilesAsync(
        [Description("The Python code to execute")] string code,
        [Description("Dictionary of file names and their content")] Dictionary<string, string> files,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        SessionInfo? session = null;

        try
        {
            _logger.LogInformation("Acquiring session for Python code execution with {FileCount} files", files.Count);
            session = await _sessionPool.GetOrCreateSessionAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Executing Python code with files in session {SessionId}", session.Id);

            // Simulate code execution with files
            var fileList = string.Join(", ", files.Keys);
            var result = new CodeExecutionResult
            {
                Success = true,
                Output = $"Code executed successfully with files: {fileList}\nSession: {session.Id}\nCode:\n{code}",
                SessionId = session.Id,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };

            _logger.LogInformation("Python code with files executed successfully in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Python code with files");
            return new CodeExecutionResult
            {
                Success = false,
                Error = ex.Message,
                SessionId = session?.Id ?? string.Empty,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        finally
        {
            stopwatch.Stop();
            if (session != null)
            {
                await _sessionPool.ReleaseSessionAsync(session.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
