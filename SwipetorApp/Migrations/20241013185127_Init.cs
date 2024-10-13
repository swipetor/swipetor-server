using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace SwipetorApp.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "english_words",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    word = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_english_words", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "key_values",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    value = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    modified_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_key_values", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    name_ascii = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    full_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    search_vector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name", "name_ascii", "full_name" }),
                    lat = table.Column<double>(type: "double precision", nullable: false),
                    lng = table.Column<double>(type: "double precision", nullable: false),
                    population = table.Column<int>(type: "integer", nullable: false),
                    capital = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    iso2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    iso3 = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    simplemaps_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    parent_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                    table.ForeignKey(
                        name: "fk_locations_locations_parent_id",
                        column: x => x.parent_id,
                        principalTable: "locations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "photos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    ext = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    sizes = table.Column<List<int>>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_photos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "remote_video_infos",
                columns: table => new
                {
                    ref_domain = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ref_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    extra_info = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_remote_video_infos", x => new { x.ref_domain, x.ref_id });
                });

            migrationBuilder.CreateTable(
                name: "videos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ext = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    size_in_bytes = table.Column<long>(type: "bigint", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    duration = table.Column<double>(type: "double precision", nullable: false),
                    checksum = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    captions = table.Column<string>(type: "character varying(102400)", maxLength: 102400, nullable: true),
                    reference_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    reference_domain = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    reference_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    reference_title = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    reference_desc = table.Column<string>(type: "character varying(102400)", maxLength: 102400, nullable: true),
                    reference_json = table.Column<string>(type: "character varying(1024000)", maxLength: 1024000, nullable: true),
                    formats = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_videos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hubs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    last_post_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ordering = table.Column<int>(type: "integer", nullable: false),
                    post_count = table.Column<int>(type: "integer", nullable: false),
                    photo_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hubs", x => x.id);
                    table.ForeignKey(
                        name: "fk_hubs_photos_photo_id",
                        column: x => x.photo_id,
                        principalTable: "photos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    username = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    secret = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    comment_count = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    last_pm_check_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_notif_check_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_notif_email_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_online_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_online_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    pm_email_interval_hours = table.Column<int>(type: "integer", nullable: true),
                    notif_email_interval_hours = table.Column<int>(type: "integer", nullable: true),
                    photo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    robot_source = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    premium_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    modified_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    browser_agent = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_photos_photo_id",
                        column: x => x.photo_id,
                        principalTable: "photos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sprites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<double>(type: "double precision", nullable: false),
                    interval = table.Column<double>(type: "double precision", nullable: false),
                    thumbnail_count = table.Column<int>(type: "integer", nullable: false),
                    thumbnail_width = table.Column<int>(type: "integer", nullable: false),
                    thumbnail_height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sprites", x => x.id);
                    table.ForeignKey(
                        name: "fk_sprites_videos_video_id",
                        column: x => x.video_id,
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    entity_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    entity_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "text", nullable: true),
                    log = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    browser_agent = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "custom_domains",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    domain_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    recaptcha_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    recaptcha_secret = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_custom_domains", x => x.id);
                    table.ForeignKey(
                        name: "fk_custom_domains_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "login_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    email_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    browser_agent = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_login_requests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pm_permissions",
                columns: table => new
                {
                    receiver_user_id = table.Column<int>(type: "integer", nullable: false),
                    sender_user_id = table.Column<int>(type: "integer", nullable: false),
                    is_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pm_permissions", x => new { x.receiver_user_id, x.sender_user_id });
                    table.ForeignKey(
                        name: "fk_pm_permissions_users_receiver_user_id",
                        column: x => x.receiver_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pm_permissions_users_sender_user_id",
                        column: x => x.sender_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    comments_count = table.Column<int>(type: "integer", nullable: false),
                    fav_count = table.Column<int>(type: "integer", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_posts", x => x.id);
                    table.ForeignKey(
                        name: "fk_posts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "push_devices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    last_used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    token = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_push_devices", x => x.id);
                    table.ForeignKey(
                        name: "fk_push_devices_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sub_plans",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    currency = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: true),
                    owner_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sub_plans", x => x.id);
                    table.ForeignKey(
                        name: "fk_sub_plans_users_owner_user_id",
                        column: x => x.owner_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_follows",
                columns: table => new
                {
                    follower_user_id = table.Column<int>(type: "integer", nullable: false),
                    followed_user_id = table.Column<int>(type: "integer", nullable: false),
                    last_new_post_notif_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_follows", x => new { x.follower_user_id, x.followed_user_id });
                    table.ForeignKey(
                        name: "fk_user_follows_users_followed_user_id",
                        column: x => x.followed_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_follows_users_follower_user_id",
                        column: x => x.follower_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "login_attempts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    tried_email_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    browser_agent = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_login_attempts_login_requests_login_request_id",
                        column: x => x.login_request_id,
                        principalTable: "login_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_login_attempts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    txt = table.Column<string>(type: "character varying(65536)", maxLength: 65536, nullable: false),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    like_count = table.Column<int>(type: "integer", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_comments_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fav_posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fav_posts", x => x.id);
                    table.ForeignKey(
                        name: "fk_fav_posts_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_fav_posts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_hubs",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    hub_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_hubs", x => new { x.post_id, x.hub_id });
                    table.ForeignKey(
                        name: "fk_post_hubs_hubs_hub_id",
                        column: x => x.hub_id,
                        principalTable: "hubs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_post_hubs_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_notif_batches",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    processed_count = table.Column<int>(type: "integer", nullable: false),
                    is_done = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_notif_batches", x => x.post_id);
                    table.ForeignKey(
                        name: "fk_post_notif_batches_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_views",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    viewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_views", x => new { x.user_id, x.post_id });
                    table.ForeignKey(
                        name: "fk_post_views_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_post_views_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_medias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    photo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    video_id = table.Column<Guid>(type: "uuid", nullable: true),
                    clipped_video_id = table.Column<Guid>(type: "uuid", nullable: true),
                    preview_photo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    preview_photo_time = table.Column<double>(type: "double precision", nullable: true),
                    clip_times = table.Column<string>(type: "text", nullable: true),
                    is_followers_only = table.Column<bool>(type: "boolean", nullable: false),
                    sub_plan_id = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    article = table.Column<string>(type: "character varying(1024000)", maxLength: 1024000, nullable: true),
                    is_instant = table.Column<bool>(type: "boolean", nullable: false),
                    ordering = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_medias", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_medias_photos_photo_id",
                        column: x => x.photo_id,
                        principalTable: "photos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_post_medias_photos_preview_photo_id",
                        column: x => x.preview_photo_id,
                        principalTable: "photos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_post_medias_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_post_medias_sub_plans_sub_plan_id",
                        column: x => x.sub_plan_id,
                        principalTable: "sub_plans",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_post_medias_videos_clipped_video_id",
                        column: x => x.clipped_video_id,
                        principalTable: "videos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_post_medias_videos_video_id",
                        column: x => x.video_id,
                        principalTable: "videos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    plan_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    browser_agent = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    created_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscriptions_sub_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "sub_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    receiver_user_id = table.Column<int>(type: "integer", nullable: false),
                    related_post_id = table.Column<int>(type: "integer", nullable: true),
                    related_comment_id = table.Column<int>(type: "integer", nullable: true),
                    sender_user_id = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    data = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    is_viewed = table.Column<bool>(type: "boolean", nullable: false),
                    push_notif_sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifs", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifs_comments_related_comment_id",
                        column: x => x.related_comment_id,
                        principalTable: "comments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_notifs_posts_related_post_id",
                        column: x => x.related_post_id,
                        principalTable: "posts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_notifs_users_receiver_user_id",
                        column: x => x.receiver_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notifs_users_sender_user_id",
                        column: x => x.sender_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pm_msgs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    thread_id = table.Column<long>(type: "bigint", nullable: false),
                    thread_user_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    txt = table.Column<string>(type: "character varying(65536)", maxLength: 65536, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pm_msgs", x => x.id);
                    table.ForeignKey(
                        name: "fk_pm_msgs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pm_threads",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_count = table.Column<int>(type: "integer", nullable: false),
                    last_msg_id = table.Column<long>(type: "bigint", nullable: true),
                    last_msg_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pm_threads", x => x.id);
                    table.ForeignKey(
                        name: "fk_pm_threads_pm_msgs_last_msg_id",
                        column: x => x.last_msg_id,
                        principalTable: "pm_msgs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pm_thread_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    thread_id = table.Column<long>(type: "bigint", nullable: false),
                    first_unread_msg_id = table.Column<long>(type: "bigint", nullable: true),
                    last_read_msg_id = table.Column<long>(type: "bigint", nullable: true),
                    unread_msg_count = table.Column<int>(type: "integer", nullable: false),
                    is_initiator = table.Column<bool>(type: "boolean", nullable: false),
                    email_sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_msg_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pm_thread_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_pm_thread_users_pm_msgs_first_unread_msg_id",
                        column: x => x.first_unread_msg_id,
                        principalTable: "pm_msgs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pm_thread_users_pm_msgs_last_read_msg_id",
                        column: x => x.last_read_msg_id,
                        principalTable: "pm_msgs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pm_thread_users_pm_threads_thread_id",
                        column: x => x.thread_id,
                        principalTable: "pm_threads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pm_thread_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_action",
                table: "audit_logs",
                column: "action");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_created_at",
                table: "audit_logs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_created_ip",
                table: "audit_logs",
                column: "created_ip");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_entity_id",
                table: "audit_logs",
                column: "entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_entity_name",
                table: "audit_logs",
                column: "entity_name");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_like_count",
                table: "comments",
                column: "like_count");

            migrationBuilder.CreateIndex(
                name: "ix_comments_post_id",
                table: "comments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_custom_domains_domain_name",
                table: "custom_domains",
                column: "domain_name");

            migrationBuilder.CreateIndex(
                name: "ix_custom_domains_user_id",
                table: "custom_domains",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fav_posts_post_id",
                table: "fav_posts",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_fav_posts_user_id",
                table: "fav_posts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_hubs_last_post_at",
                table: "hubs",
                column: "last_post_at");

            migrationBuilder.CreateIndex(
                name: "ix_hubs_ordering",
                table: "hubs",
                column: "ordering");

            migrationBuilder.CreateIndex(
                name: "ix_hubs_photo_id",
                table: "hubs",
                column: "photo_id");

            migrationBuilder.CreateIndex(
                name: "ix_hubs_post_count",
                table: "hubs",
                column: "post_count");

            migrationBuilder.CreateIndex(
                name: "ix_locations_full_name",
                table: "locations",
                column: "full_name");

            migrationBuilder.CreateIndex(
                name: "ix_locations_iso2",
                table: "locations",
                column: "iso2");

            migrationBuilder.CreateIndex(
                name: "ix_locations_iso3",
                table: "locations",
                column: "iso3");

            migrationBuilder.CreateIndex(
                name: "ix_locations_lat",
                table: "locations",
                column: "lat");

            migrationBuilder.CreateIndex(
                name: "ix_locations_lng",
                table: "locations",
                column: "lng");

            migrationBuilder.CreateIndex(
                name: "ix_locations_name",
                table: "locations",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_locations_name_ascii",
                table: "locations",
                column: "name_ascii");

            migrationBuilder.CreateIndex(
                name: "ix_locations_parent_id",
                table: "locations",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_locations_search_vector",
                table: "locations",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_locations_type",
                table: "locations",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_login_attempts_created_at",
                table: "login_attempts",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_login_attempts_created_ip",
                table: "login_attempts",
                column: "created_ip");

            migrationBuilder.CreateIndex(
                name: "ix_login_attempts_login_request_id",
                table: "login_attempts",
                column: "login_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_login_attempts_user_id",
                table: "login_attempts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_login_requests_created_at",
                table: "login_requests",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_login_requests_created_ip",
                table: "login_requests",
                column: "created_ip");

            migrationBuilder.CreateIndex(
                name: "ix_login_requests_email",
                table: "login_requests",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_login_requests_email_code",
                table: "login_requests",
                column: "email_code");

            migrationBuilder.CreateIndex(
                name: "ix_login_requests_is_used",
                table: "login_requests",
                column: "is_used");

            migrationBuilder.CreateIndex(
                name: "ix_login_requests_user_id",
                table: "login_requests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifs_created_at",
                table: "notifs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_notifs_is_read",
                table: "notifs",
                column: "is_read");

            migrationBuilder.CreateIndex(
                name: "ix_notifs_is_viewed",
                table: "notifs",
                column: "is_viewed");

            migrationBuilder.CreateIndex(
                name: "ix_notifs_push_notif_sent_at",
                table: "notifs",
                column: "push_notif_sent_at");

            migrationBuilder.CreateIndex(
                name: "ix_notifs_receiver_user_id",
                table: "notifs",
                column: "receiver_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifs_related_comment_id",
                table: "notifs",
                column: "related_comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifs_related_post_id",
                table: "notifs",
                column: "related_post_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifs_sender_user_id",
                table: "notifs",
                column: "sender_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifs_type",
                table: "notifs",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_pm_msgs_thread_id",
                table: "pm_msgs",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "ix_pm_msgs_thread_user_id",
                table: "pm_msgs",
                column: "thread_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pm_msgs_user_id",
                table: "pm_msgs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pm_permissions_is_allowed",
                table: "pm_permissions",
                column: "is_allowed");

            migrationBuilder.CreateIndex(
                name: "ix_pm_permissions_is_blocked",
                table: "pm_permissions",
                column: "is_blocked");

            migrationBuilder.CreateIndex(
                name: "ix_pm_permissions_sender_user_id",
                table: "pm_permissions",
                column: "sender_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pm_thread_users_email_sent_at",
                table: "pm_thread_users",
                column: "email_sent_at");

            migrationBuilder.CreateIndex(
                name: "ix_pm_thread_users_first_unread_msg_id",
                table: "pm_thread_users",
                column: "first_unread_msg_id");

            migrationBuilder.CreateIndex(
                name: "ix_pm_thread_users_is_initiator",
                table: "pm_thread_users",
                column: "is_initiator");

            migrationBuilder.CreateIndex(
                name: "ix_pm_thread_users_last_msg_at",
                table: "pm_thread_users",
                column: "last_msg_at");

            migrationBuilder.CreateIndex(
                name: "ix_pm_thread_users_last_read_msg_id",
                table: "pm_thread_users",
                column: "last_read_msg_id");

            migrationBuilder.CreateIndex(
                name: "ix_pm_thread_users_thread_id",
                table: "pm_thread_users",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "ix_pm_thread_users_unread_msg_count",
                table: "pm_thread_users",
                column: "unread_msg_count");

            migrationBuilder.CreateIndex(
                name: "ix_pm_thread_users_user_id",
                table: "pm_thread_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pm_threads_created_at",
                table: "pm_threads",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_pm_threads_last_msg_at",
                table: "pm_threads",
                column: "last_msg_at");

            migrationBuilder.CreateIndex(
                name: "ix_pm_threads_last_msg_id",
                table: "pm_threads",
                column: "last_msg_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_hubs_hub_id",
                table: "post_hubs",
                column: "hub_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_clipped_video_id",
                table: "post_medias",
                column: "clipped_video_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_is_instant",
                table: "post_medias",
                column: "is_instant");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_ordering",
                table: "post_medias",
                column: "ordering");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_photo_id",
                table: "post_medias",
                column: "photo_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_post_id",
                table: "post_medias",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_preview_photo_id",
                table: "post_medias",
                column: "preview_photo_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_sub_plan_id",
                table: "post_medias",
                column: "sub_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_type",
                table: "post_medias",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_video_id",
                table: "post_medias",
                column: "video_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_notif_batches_created_at",
                table: "post_notif_batches",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_post_notif_batches_is_done",
                table: "post_notif_batches",
                column: "is_done");

            migrationBuilder.CreateIndex(
                name: "ix_post_views_post_id",
                table: "post_views",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_posts_created_at",
                table: "posts",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_posts_created_ip",
                table: "posts",
                column: "created_ip");

            migrationBuilder.CreateIndex(
                name: "ix_posts_fav_count",
                table: "posts",
                column: "fav_count");

            migrationBuilder.CreateIndex(
                name: "ix_posts_is_published",
                table: "posts",
                column: "is_published");

            migrationBuilder.CreateIndex(
                name: "ix_posts_is_removed",
                table: "posts",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "ix_posts_user_id",
                table: "posts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_push_devices_last_used_at",
                table: "push_devices",
                column: "last_used_at");

            migrationBuilder.CreateIndex(
                name: "ix_push_devices_token",
                table: "push_devices",
                column: "token");

            migrationBuilder.CreateIndex(
                name: "ix_push_devices_user_id",
                table: "push_devices",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_remote_video_infos_action",
                table: "remote_video_infos",
                column: "action");

            migrationBuilder.CreateIndex(
                name: "ix_sprites_video_id",
                table: "sprites",
                column: "video_id");

            migrationBuilder.CreateIndex(
                name: "ix_sub_plans_owner_user_id",
                table: "sub_plans",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_created_ip",
                table: "subscriptions",
                column: "created_ip");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_plan_id",
                table: "subscriptions",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_follows_followed_user_id",
                table: "user_follows",
                column: "followed_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_follows_last_new_post_notif_at",
                table: "user_follows",
                column: "last_new_post_notif_at");

            migrationBuilder.CreateIndex(
                name: "ix_users_created_ip",
                table: "users",
                column: "created_ip");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_users_last_notif_check_at",
                table: "users",
                column: "last_notif_check_at");

            migrationBuilder.CreateIndex(
                name: "ix_users_last_notif_email_at",
                table: "users",
                column: "last_notif_email_at");

            migrationBuilder.CreateIndex(
                name: "ix_users_last_online_at",
                table: "users",
                column: "last_online_at");

            migrationBuilder.CreateIndex(
                name: "ix_users_last_pm_check_at",
                table: "users",
                column: "last_pm_check_at");

            migrationBuilder.CreateIndex(
                name: "ix_users_modified_ip",
                table: "users",
                column: "modified_ip");

            migrationBuilder.CreateIndex(
                name: "ix_users_notif_email_interval_hours",
                table: "users",
                column: "notif_email_interval_hours");

            migrationBuilder.CreateIndex(
                name: "ix_users_photo_id",
                table: "users",
                column: "photo_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_pm_email_interval_hours",
                table: "users",
                column: "pm_email_interval_hours");

            migrationBuilder.CreateIndex(
                name: "ix_users_role",
                table: "users",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_videos_checksum",
                table: "videos",
                column: "checksum");

            migrationBuilder.CreateIndex(
                name: "ix_videos_reference_domain",
                table: "videos",
                column: "reference_domain");

            migrationBuilder.CreateIndex(
                name: "ix_videos_reference_id",
                table: "videos",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_size_in_bytes",
                table: "videos",
                column: "size_in_bytes");

            migrationBuilder.AddForeignKey(
                name: "fk_pm_msgs_pm_thread_users_thread_user_id",
                table: "pm_msgs",
                column: "thread_user_id",
                principalTable: "pm_thread_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_pm_msgs_pm_threads_thread_id",
                table: "pm_msgs",
                column: "thread_id",
                principalTable: "pm_threads",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pm_msgs_users_user_id",
                table: "pm_msgs");

            migrationBuilder.DropForeignKey(
                name: "fk_pm_thread_users_users_user_id",
                table: "pm_thread_users");

            migrationBuilder.DropForeignKey(
                name: "fk_pm_msgs_pm_thread_users_thread_user_id",
                table: "pm_msgs");

            migrationBuilder.DropForeignKey(
                name: "fk_pm_msgs_pm_threads_thread_id",
                table: "pm_msgs");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "custom_domains");

            migrationBuilder.DropTable(
                name: "english_words");

            migrationBuilder.DropTable(
                name: "fav_posts");

            migrationBuilder.DropTable(
                name: "key_values");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "login_attempts");

            migrationBuilder.DropTable(
                name: "notifs");

            migrationBuilder.DropTable(
                name: "pm_permissions");

            migrationBuilder.DropTable(
                name: "post_hubs");

            migrationBuilder.DropTable(
                name: "post_medias");

            migrationBuilder.DropTable(
                name: "post_notif_batches");

            migrationBuilder.DropTable(
                name: "post_views");

            migrationBuilder.DropTable(
                name: "push_devices");

            migrationBuilder.DropTable(
                name: "remote_video_infos");

            migrationBuilder.DropTable(
                name: "sprites");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "user_follows");

            migrationBuilder.DropTable(
                name: "login_requests");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "hubs");

            migrationBuilder.DropTable(
                name: "videos");

            migrationBuilder.DropTable(
                name: "sub_plans");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "photos");

            migrationBuilder.DropTable(
                name: "pm_thread_users");

            migrationBuilder.DropTable(
                name: "pm_threads");

            migrationBuilder.DropTable(
                name: "pm_msgs");
        }
    }
}
