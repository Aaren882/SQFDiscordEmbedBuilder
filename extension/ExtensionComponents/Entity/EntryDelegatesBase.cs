using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Logging;
using static ExtensionComponents.LocalServices;

namespace ExtensionComponents.Entity;

public abstract class EntryDelegatesBase
{
	protected abstract ILogger<EntryDelegatesBase> Logger { get; set; }
	public required Dictionary<string, InitAction> ActionsDict;

	public Dictionary<string, InitAction> GetActionsMap(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] Type actionType
	)
	{
		var methods = actionType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
			.Where(m => m.ReturnType == typeof(int));

#if DEBUG
		foreach (var method in methods)
			Logger.LogDebug("{ActionsMapName} : {MethodName}", nameof(GetActionsMap), method.Name);
#endif

		return methods.ToDictionary(
			prop => prop.Name,
			prop => (InitAction)Delegate.CreateDelegate(
				typeof(InitAction),
				null,
				prop
			)!
		);
	}
}
