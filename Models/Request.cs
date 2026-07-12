using System.ComponentModel.DataAnnotations;

namespace CachingProxy.Models;

class Request
{

    [Required]
    [MaxLength(256)]
    public string Url { get; set; } = string.Empty;

    [Required]
    public int ResponseStatusCode { get; set; }

    [Required]
    public string ResponseHeaders { get; set; } = string.Empty;

    [Required]
    public string ResponseBody { get; set; } = string.Empty;
}