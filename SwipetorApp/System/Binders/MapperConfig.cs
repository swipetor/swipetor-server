using System;
using System.Linq;
using AutoMapper;
using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.SqlQueries.Models;
using WebLibServer.Extensions;
using WebLibServer.SharedLogic.Fx;
using WebLibServer.Uploaders.R2;

namespace SwipetorApp.System.Binders;

[UsedImplicitly]
public class MapperConfig
{
    static MapperConfig()
    {
        //Mapper.Configuration.AssertConfigurationIsValid();
        Config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
    }

    public static MapperConfiguration Config { get; }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            AllowNullCollections = true;
            AllowNullDestinationValues = true;

            CreateMap<DateTime, long>().ConvertUsing(dt => dt.ToTimestamp());
            CreateMap<DateTimeOffset, long>().ConvertUsing(dt => dt.ToTimestamp());

            CreateMap<CustomDomain, CustomDomainDto>();
            CreateMap<Hub, HubDto>();

            CreateMap<Photo, PhotoDto>();

            CreateMap<Post, PostDto>()
                .ForMember(d => d.Hubs,
                    opt =>
                        opt.MapFrom(s => s.PostHubs.Select(c => c.Hub)));

            CreateMap<PostMedia, PostMediaDto>();
            CreateMap<Comment, CommentDto>();

            CreateMap<HubQueryModel, HubDto>().IncludeMembers(f => f.Hub);
            CreateMap<PostQueryModel, PostDto>().IncludeMembers(t => t.Post)
                .ForPath(d => d.User.Photo, opt =>
                {
                    opt.Condition(src => src.Source.User?.PhotoId != null);
                    opt.MapFrom(s => s.UserPhoto);
                })
                .ForMember(d => d.User, opt => opt.MapFrom(s => s.User))
                .ForMember(d => d.CommentsCount, opt => opt.MapFrom(s => s.Post.CommentsCount))
                .ForPath(d => d.User.UserFollows, opt => opt.MapFrom(s => s.UserFollows));

            CreateMap<UserQueryModel, UserDto>().IncludeMembers(q => q.User)
                .ForMember(d => d.Photo, o => o.MapFrom(s => s.Photo));

            CreateMap<UserQueryModel, PublicUserDto>().IncludeMembers(q => q.User)
                .ForMember(d => d.Photo, o => o.MapFrom(s => s.Photo));

            CreateMap<CommentQueryModel, CommentDto>().IncludeMembers(t => t.Comment)
                .ForMember(d => d.User, opt => opt.MapFrom(s => s.User))
                .ForPath(d => d.User.Photo, opt => opt.MapFrom(s => s.UserPhoto));

            CreateMap<Notif, NotifDto>();

            CreateMap<PmThread, PmThreadDto>();
            CreateMap<PmThreadUser, PmThreadUserDto>();
            CreateMap<PmMsg, PmMsgDto>();

            CreateMap<PmThreadUserQueryModel, PmThreadUserDto>().IncludeMembers(s => s.ThreadUser)
                .ForMember(d => d.User, opt => opt.MapFrom(s => s.UserQueryModel));
            CreateMap<PmThreadQueryModel, PmThreadDto>().IncludeMembers(s => s.PmThread)
                .ForMember(d => d.ThreadUsers, opt => opt.MapFrom(s => s.PmThreadUserQueryModel));
            CreateMap<NotifQueryModel, NotifDto>().IncludeMembers(t => t.Notif);

            CreateMap<User, UserDto>();
            CreateMap<User, PublicUserDto>();

            CreateMap<Video, VideoDto>();
            CreateMap<Sprite, SpriteDto>();

            CreateMap<Location, LocationDto>();

            CreateMap<R2Config, R2FileUploaderConfig>();

            CreateMap<Sub, SubDto>();
            CreateMap<SubPlan, SubPlanDto>().ForMember(dest => dest.CPrice,
                opt => opt.MapFrom(src => new CPrice(src.Currency, src.Price)));
        }
    }
}