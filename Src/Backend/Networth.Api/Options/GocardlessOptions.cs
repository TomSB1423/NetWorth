using System.ComponentModel.DataAnnotations;

namespace Networth.Options;

internal class GocardlessOptions
{
    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string SecretId { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string SecretKey { get; set; } = string.Empty;
}
