using System.Text.Json;
using ExtensionComponents.Entity;
using ExtensionComponents.Tools;
using Microsoft.Extensions.Logging;
using static ServiceConnection.ServiceStartup;
using ServiceConnectionUtil = ServiceConnection.Tools.Util;

#pragma warning disable CA1822
namespace ServiceConnection;

public sealed class EntryDelegates : EntryDelegatesBase
{
	protected override ILogger<EntryDelegatesBase> Logger { get; set; }
	public EntryDelegates(ILogger<EntryDelegates> Logger)
	{
		this.Logger = Logger;
		ActionsDict = GetActionsMap(typeof(EntryDelegates));
	}

	internal int GetDirectoryFileNames(IOutputBuilder output, string[] args, int argCount)
	{
		var path = args[0];
		var fileNames = ServiceConnectionUtil.GetDirectoryFileNames(path);
		output.Append($"[\"{string.Join("\",\"", fileNames)}\"]");

		return fileNames.Count;
	}

	internal int GetDirectoryFilesDateTime(IOutputBuilder output, string[] args, int argCount)
	{
		var fileInfos = ServiceConnectionUtil.GetFilesFileInfos(args)
			.Select(x =>
				((DateTimeOffset)x.LastWriteTime).ToUnixTimeSeconds()
			).ToList();

		output.Append($"[\"{string.Join("\",\"", fileInfos)}\"]");
		return fileInfos.Count;
	}
	internal int UpdateRptDirectory(IOutputBuilder output, string[] args, int argCount)
	{
		var dir = args[0];
		ServiceInteractions.RPTDirectory = dir;
		RptFileDirectory = ServiceConnectionUtil.GetCurrentRpt();
		Logger.LogInformation("Update RPT File : {RptFileDirectory}", RptFileDirectory);

		return 1;
	}
	internal int GetCurrentRpt(IOutputBuilder output, string[] args, int argCount)
	{
		output.Append(ServiceConnectionUtil.GetCurrentRpt());
		return 1;
	}

	/// <summary>
	/// Setup Websocket Connection to backend service
	/// </summary>
	/// <param name="output"></param>
	/// <param name="args"></param>
	/// <param name="argCount"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	internal int ConnectWebSocket(IOutputBuilder output, string[] args, int argCount)
	{
		var accessName = args[0];
		var profilePayload = args[1];
		if (string.IsNullOrEmpty(accessName))
			throw new Exception("NO ACCESS NAME PROVIDED.");

		_ = InitializeAsync(accessName, profilePayload);

		return 1;
	}
	/// <summary>
	/// Disrupt current WebSocket connection
	/// </summary>
	/// <param name="output"></param>
	/// <param name="args"></param>
	/// <param name="argCount"></param>
	/// <returns></returns>
	internal int DisconnectWebSocket(IOutputBuilder output, string[] args, int argCount)
	{
		_ = ShutdownAsync();
		return 1;
	}
	/// <summary>
	/// Reconnect Websocket relay
	/// </summary>
	/// <param name="output"></param>
	/// <param name="args"></param>
	/// <param name="argCount"></param>
	/// <returns></returns>
	internal int ReconnectWebSocket(IOutputBuilder output, string[] args, int argCount)
	{
		var profilePayload = args[0];

		_ = ServiceInteractions?.ReconnectWebSocket(profilePayload);
		return 1;
	}

	/// <summary>
	/// Sends a message via WebSocket to the backend service.
	/// </summary>
	/// <param name="output"></param>
	/// <param name="args"></param>
	/// <param name="argCount"></param>
	/// <returns></returns>
	internal int SendWebSocketMessage(IOutputBuilder output, string[] args, int argCount)
	{
		var message = args[0];

		ServiceInteractions?.SendWebSocketMessage(message);

		return 1;
	}
	/*internal int SendWebSocketRPT(IOutputBuilder output, string[] args, int argCount)
	{
		var lastestRpt= Util.GetLatestFile(ServiceInteractions.RPTDirectory);
		output.Append(lastestRpt); //- Return lastest Rpt directory

		ServiceInteractions.SendWebSocketBinary(lastestRpt, args[0]);
		ServiceInteractions.WebSocketTrafficWriter(task);

		return 1;
	}*/
	internal int SendWebSocketBinaries(IOutputBuilder output, string[] args, int argCount)
	{
		var binaryDict = JsonSerializer.Deserialize(args[0], ExtensionSerializable.Default.DictionaryStringString);
		ServiceInteractions?.SendWebSocketBinaries(binaryDict!);

		return 1;
	}
	internal int SendWebSocketRptLines(IOutputBuilder output, string[] args, int argCount)
	{
		if (!int.TryParse(args[0], out var linesCount))
			throw new Exception("INCORRECT NUMBER OF ARGUMENTS");

		ServiceInteractions?.SendWebSocketRptLines(RptFileDirectory, linesCount);

		return 1;
	}
	internal int SendWebSocketBinariesFromAssemblyDirectory(IOutputBuilder output, string[] args, int argCount)
	{
		var binaryDict = JsonSerializer.Deserialize(args[0], ExtensionSerializable.Default.DictionaryStringString);

		if (binaryDict is null)
			throw new Exception("INVALID ARGUMENT. (Dictionary for binaries is null)");

		foreach (var (key, value) in binaryDict)
			binaryDict[key] = Path.Combine(Util.AssemblyPath, value);

		ServiceInteractions?.SendWebSocketBinaries(binaryDict);

		return 1;
	}
}
