namespace ExtensionComponents.Entity;

public interface ILocalServices
{
	void Output(nint destination, int outputSize, string data);
	int ExecuteArgsAction(IArgsAction argsAction);
	ReadOnlySpan<byte> GetUtf8Span(nint pointer);
}
