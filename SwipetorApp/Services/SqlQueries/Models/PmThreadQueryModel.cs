using System.Collections.Generic;
using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Services.SqlQueries.Models;

public class PmThreadQueryModel
{
    public PmThread PmThread { get; set; }
    public List<PmThreadUserQueryModel> PmThreadUserQueryModel { get; set; }
}