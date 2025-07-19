using System.ComponentModel.DataAnnotations;

namespace Networth.Options;

internal class NetworthOptions
{
    [Required]
    public bool IsSandbox { get; set; }
}
