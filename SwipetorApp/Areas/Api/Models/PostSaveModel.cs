using System.Collections.Generic;
using JetBrains.Annotations;

namespace SwipetorApp.Areas.Api.Models;

public class PostSaveModel
{
    public IList<PostMediaItemModel> Items { get; set; }
    public bool IsPublished { get; set; }

    public int[] HubIds { get; set; }

    public int? PosterUserId { get; set; }

    [UsedImplicitly]
    public class PostMediaItemModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsFollowersOnly { get; set; }
        public int? SubPlanId { get; set; }
        public List<List<double>> ClipTimes { get; set; }
    }
}
