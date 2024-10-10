using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.SqlQueries;
using SwipetorApp.Services.SqlQueries.Models;

namespace SwipetorApp.Services.Posts;

public class PostsQuerier([CanBeNull] User currentUser) : IDisposable
{
    private readonly int _takeCount = 10;
    private readonly int? _userIdOrNull = currentUser?.Id;

    private DbCx _db;

    private IQueryable<Post> _query;

    public int? FirstPostId { get; set; }
    public int? UserId { get; set; }
    public List<int> HubIds { get; set; }

    public void Dispose()
    {
        _db?.Dispose();
    }

    public List<PostQueryModel> Run(DbCx db)
    {
        _db = db;
        _query = _db.Posts.AsNoTracking().AsQueryable();

        SetDefaultFilters();

        if (UserId != null) _query = _query.Where(p => p.UserId == UserId);

        // Show not-viewed posts firsts
        _query = _query.OrderBy(p => p.PostViews.Any(pv => pv.UserId == _userIdOrNull)).ThenBy(p => Guid.NewGuid());

        if (HubIds is { Count: > 0 })
            SetHubs();

        var posts = _query.SelectForUser(_userIdOrNull).Take(_takeCount).ToList();

        // Prepend the first post if exists
        if (FirstPostId != null)
        {
            var firstPost = db.Posts.Where(p => p.Id == FirstPostId).SelectForUser(_userIdOrNull).SingleOrDefault();
            if (firstPost != null)
            {
                posts = posts.Where(p => p.Post.Id != FirstPostId).ToList();
                posts.Insert(0, firstPost);
            }
        }

        posts.ForEach(p =>
        {
            if (p.User != null && p.User.PhotoId == null)
            {
                p.User.Photo = null;
                p.UserPhoto = null;
            }
        });

        return posts;
    }

    private void SetHubs()
    {
        _query = _query.Where(p => p.PostHubs.Any(pl => HubIds.Contains(pl.HubId)));
    }

    private void SetDefaultFilters()
    {
        _query = _query.Where(p => p.IsPublished && !p.IsRemoved && p.Medias.Count > 0);
    }
}