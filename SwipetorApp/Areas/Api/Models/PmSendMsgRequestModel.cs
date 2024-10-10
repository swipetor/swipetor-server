using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class PmSendMsgRequestModel
{
    [Required] public long ThreadId { get; set; }

    [Required] public string Txt { get; set; }
}