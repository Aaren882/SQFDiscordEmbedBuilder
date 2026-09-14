using System.Text;
using ExtensionComponents.Entity;
using Microsoft.Extensions.Logging;

namespace ExtensionComponents;

public class LocalServices(ILogger<LocalServices> Logger, EntryDelegatesBase entryDelegates) : ILocalServices
{
	public unsafe void Output(nint destination, int outputSize, string data)
	{
		//- Execution Time: 0.1319 ms  |  Cycles: 7583/10000  (ORIGIN)
		//- Execution Time: 0.1290 ms  |  Cycles: 7751/10000  (Improved "output()")
		//- Execution Time: 0.0293 ms  |  Cycles: 10000/10000 (Improved "output()" + Improved Args parsing + "RVFeature_ArgumentNoEscapeString")
		try
		{
			//- less overhead
			Span<byte> output = new((byte*)destination, outputSize);

			//- Empty buffer (clean up previous output)
			output.Clear();

			//- Write data into buffer
			int bytesWritten = Encoding.UTF8.GetBytes(data, output);
			output[bytesWritten] = 0; //- add "/0" C-String
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error occurred while writing data to output buffer.");
		}
	}

	public int ExecuteArgsAction(IArgsAction argsAction)
	{
		Logger.LogDebug("ExecuteArgsAction(IArgsAction argsAction)");
		var (output, args, functionPtr) = argsAction.GetParams();
		Logger.LogDebug("{argsAction}", argsAction);
		return ExecuteArgsAction(output, args, functionPtr);
	}
	public unsafe int ExecuteArgsAction(IOutputBuilder Output, string[] Args, nint functionPtr)
	{
		try
		{
			var functionSpan = GetUtf8Span(functionPtr);
			var functionString = Encoding.UTF8.GetString(functionSpan);

			Logger.LogDebug("Calling Function : {FunctionName}", functionString);

			if (!entryDelegates.ActionsDict.TryGetValue(functionSpan, out var actionPtr))
				throw new NullReferenceException($"Function \"{functionString}\" is not exist.");

			Logger.LogDebug("Function Found! Passing arguments ({Output}, {Args}, {ArgsCount})", Output, Args, Args.Length);

			var action = (delegate* managed<IOutputBuilder, string[], int, int>)actionPtr;
			return action(Output, Args, Args.Length);
		}
		catch (Exception e)
		{
			Output.Append($"Error!! \"{e.Message}\"");
			Logger.LogError(e, "Error during {MethodName} execution.", nameof(ExecuteArgsAction));

			return -11;
		}
	}
	public unsafe ReadOnlySpan<byte> GetUtf8Span(nint pointer)
	{
		if (pointer == nint.Zero) return [];

		var bytePtr = (byte*)pointer;
		var current = bytePtr;

		// 1. Find the length by scanning for the null terminator (0)
		while (*current != 0)
		{
			current++;
		}
		int length = (int)(current - bytePtr);

		// 2. Create the byte span directly from the memory address
		return new(bytePtr, length);
	}

}
