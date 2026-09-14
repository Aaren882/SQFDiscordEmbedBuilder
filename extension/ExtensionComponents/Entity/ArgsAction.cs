using System.Runtime.InteropServices;

namespace ExtensionComponents.Entity;

public interface IArgsAction
{
	public IOutputBuilder Output { get; init; }
	public IArgsBuilder Args { get; init; }
	public nint FunctionPtr { get; init; }
	public (IOutputBuilder, string[], nint) GetParams();
	public string[] GetArgsStringArray();
}

public readonly record struct ArgsAction(IOutputBuilder Output, IArgsBuilder Args, nint FunctionPtr) : IArgsAction
{
	public unsafe string[] GetArgsStringArray()
	{
		if (Args.SourcePtr == nint.Zero) return [];

		var source = (byte**)Args.SourcePtr;
		var argCount = Args.ArgCount;

		// Get the Buffer
		ReadOnlySpan<nint> pointerSpan = new(source, argCount);

		var result = new string[argCount];
		for (var i = 0; i < argCount; i++)
		{
			var rawString = Marshal.PtrToStringUTF8(pointerSpan[i]) ?? string.Empty;
			if (!string.IsNullOrEmpty(rawString))
			{
				//- Remove Arma quotations
				var cleanedSpan = rawString.AsSpan().Trim("\" ");
				result[i] = cleanedSpan.ToString().Replace("\"\"", "\"");
			}
		}

		return result;
	}
	public (IOutputBuilder, string[], nint) GetParams() => (Output, GetArgsStringArray(), FunctionPtr);
}
