using System.Diagnostics;

namespace Networth.Application.Diagnostics;

public static class AppActivitySource
{
    public const string Name = "Networth.Application";
    public static readonly ActivitySource Instance = new(Name);
}
