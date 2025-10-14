using BioLinker.DTO.BioDTO;
using BioLinker.DTO.TemplateDTO;
using BioLinker.Enities;
using BioLinker.Respository.BioPageRepo;
using BioLinker.Respository.TemplateRepo;

namespace BioLinker.Service
{
    public class BioPageService : IBioPageService
    {
        private readonly IBioPageRepository _repo;
        private readonly IBackgroundService _backgroundService;
        private readonly IStyleService _styleService;
        private readonly IStyleSettingsService _styleSettingsService;
        private readonly IContentService _contentService;
        private readonly ITemplateRepository _templateRepo;

        public BioPageService(IBioPageRepository repo,
          IBackgroundService backgroundService,
          IStyleService styleService,
          IStyleSettingsService styleSettingsService,
          IContentService contentService,
          ITemplateRepository templateRepo)
        {
            _repo = repo;
            _backgroundService = backgroundService;
            _styleService = styleService;
            _styleSettingsService = styleSettingsService;
            _contentService = contentService;
            _templateRepo = templateRepo;
        }

        public async Task<BioPageResponse> CreateAsync(CreateBioPage dto)
        {
            using var transaction = await _repo.BeginTransactionAsync();
            try
            {
                //  Tao Background
                string? backgroundId = null;
                if (dto.Background != null)
                {
                    var bg = await _backgroundService.CreateAsync(dto.Background);
                    backgroundId = bg?.BackgroundId;
                }

                //  Tao Style
                string? styleId = null;
                if (dto.Style != null)
                {
                    var st = await _styleService.CreateAsync(dto.Style);
                    styleId = st?.StyleId;
                }

                // Tao StyleSettings
                string? styleSettingsId = null;
                if (dto.StyleSettings != null)
                {
                    var ss = await _styleSettingsService.CreateAsync(dto.StyleSettings);
                    styleSettingsId = ss?.StyleSettingsId;
                }

                //  Tao BioPage
                var bio = new BioPage
                {
                    BioPageId = Guid.NewGuid().ToString(),
                    UserId = dto.UserId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Avatar = dto.Avatar,
                    Status = dto.Status,
                    CreatedAt = DateTime.UtcNow,
                    BackgroundId = backgroundId,
                    StyleId = styleId,
                    StyleSettingsId = styleSettingsId
                };

                await _repo.AddAsync(bio);

                //  Tao Content
                if (dto.Contents != null && dto.Contents.Any())
                {
                    foreach (var content in dto.Contents)
                    {
                        content.BioPageId = bio.BioPageId;
                        await _contentService.CreateContent(content);
                    }
                }

                await transaction.CommitAsync();

                //  response
                return new BioPageResponse
                {
                    BioPageId = bio.BioPageId,
                    UserId = bio.UserId,
                    Title = bio.Title,
                    Description = bio.Description,
                    Avatar = bio.Avatar,
                    Status = bio.Status,
                    CreatedAt = bio.CreatedAt,
                    Background = dto.Background == null ? null : new BackgroundResponse
                    {
                        BackgroundId = backgroundId,
                        Type = dto.Background.Type,
                        Value = dto.Background.Value
                    },

                    Template = null,

                    Style = dto.Style == null ? null : new StyleResponse
                    {
                        StyleId = styleId,
                        Preset = dto.Style.Preset,
                        LayoutMode = dto.Style.LayoutMode,
                        ButtonShape = dto.Style.ButtonShape,
                        ButtonColor = dto.Style.ButtonColor,
                        IconColor = dto.Style.IconColor,
                        BackgroundColor = dto.Style.BackgroundColor
                    },
                    StyleSettings = dto.StyleSettings == null ? null : new StyleSettingsResponse
                    {
                        StyleSettingsId = styleSettingsId,
                        Thumbnail = dto.StyleSettings.Thumbnail,
                        MetaTitle = dto.StyleSettings.MetaTitle,
                        MetaDescription = dto.StyleSettings.MetaDescription,
                        CookieBanner = dto.StyleSettings.CookieBanner
                    },
                    Contents = dto.Contents?.Select(c => new ContentResponse
                    {
                        ElementType = c.ElementType,
                        Alignment = c.Alignment,
                        Visible = c.Visible,
                        Position = c.Position,
                        Size = c.Size,
                        Style = c.Style,
                        Element = c.Element
                    }).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BioPageResponse?> CreateFromTemplateAsync(string templateId, CreateBioPageFromTemplate dto)
        {
            var template = await _templateRepo.GetByIdAsync(templateId);
            if (template == null) return null;

            // === Clone Style ===
            string? styleId = null;
            if (template.Style != null)
            {
                var createdStyle = await _styleService.CreateAsync(new CreateStyle
                {
                    Preset = template.Style.Preset,
                    LayoutMode = template.Style.LayoutMode,
                    ButtonShape = template.Style.ButtonShape,
                    ButtonColor = template.Style.ButtonColor,
                    IconColor = template.Style.IconColor,
                    BackgroundColor = template.Style.BackgroundColor
                });

                // đảm bảo lấy đúng ID đã lưu trong DB
                styleId = createdStyle.StyleId;
            }

            // === Clone background ===
            string? backgroundId = null;
            if (template.Background != null)
            {
                // Tạo background mới
                var bg = await _backgroundService.CreateAsync(new BackgroundCreate
                {
                    Type = template.Background.Type,
                    Value = template.Background.Value
                });

                // Đảm bảo EF đã commit entity mới vào DB
                if (bg != null && !string.IsNullOrEmpty(bg.BackgroundId))
                {
                    backgroundId = bg.BackgroundId;
                }
            }

            // === Clone style setting ===
            string? styleSettingsId = null;
            if (template.StyleSettings != null)
            {
                var newSetting = await _styleSettingsService.CreateAsync(new CreateStyleSettings
                {
                    Thumbnail = template.StyleSettings.Thumbnail,
                    MetaTitle = template.StyleSettings.MetaTitle,
                    MetaDescription = template.StyleSettings.MetaDescription,
                    CookieBanner = template.StyleSettings.CookieBanner
                });

                styleSettingsId = newSetting.StyleSettingsId;
            }
            // luu bio
            var newBio = new BioPage
            {
                BioPageId = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                TemplateId = template.TemplateId,
                Title = dto.Title ?? template.Name,
                Description = dto.Description ?? template.Description,
                Avatar = null,
                Status = dto.Status ?? "Active",
                CreatedAt = DateTime.UtcNow,
                StyleId = styleId,
                BackgroundId = backgroundId,
                StyleSettingsId = styleSettingsId
            };

            await _repo.AddAsync(newBio);

            // template detail
            if (template.TemplateDetails != null && template.TemplateDetails.Any())
            {
                foreach (var item in template.TemplateDetails)
                {
                    var newContent = new CreateContent
                    {
                        BioPageId = newBio.BioPageId,
                        ElementType = item.ElementType,
                        Alignment = "center",
                        Visible = true,
                        Position = item.Position,
                        Size = item.Size,
                        Style = item.Style,
                        Element = item.Element
                    };
                    await _contentService.CreateContent(newContent);
                }
            }
            //response
            return new BioPageResponse
            {
                BioPageId = newBio.BioPageId,
                UserId = newBio.UserId,
                TemplateId = newBio.TemplateId,
                Title = newBio.Title,
                Description = newBio.Description,
                Avatar = newBio.Avatar,
                Status = newBio.Status,
                CreatedAt = newBio.CreatedAt,

                Template = new TemplateResponse
                {
                    TemplateId = template.TemplateId,
                    Name = template.Name,
                    Description = template.Description,
                    Category = template.Category,
                    IsPremium = template.IsPremium,
                    Status = template.Status
                },
                Style = template.Style == null ? null : new StyleResponse
                {
                    StyleId = newBio.StyleId,
                    Preset = template.Style.Preset,
                    LayoutMode = template.Style.LayoutMode,
                    ButtonShape = template.Style.ButtonShape,
                    ButtonColor = template.Style.ButtonColor,
                    IconColor = template.Style.IconColor,
                    BackgroundColor = template.Style.BackgroundColor
                },
                Background = template.Background == null ? null : new BackgroundResponse
                {
                    BackgroundId = newBio.BackgroundId,
                    Type = template.Background.Type,
                    Value = template.Background.Value
                },
                StyleSettings = template.StyleSettings == null ? null : new StyleSettingsResponse
                {
                    StyleSettingsId = newBio.StyleSettings?.StyleSettingsId,
                    Thumbnail = template.StyleSettings.Thumbnail,
                    MetaTitle = template.StyleSettings.MetaTitle,
                    MetaDescription = template.StyleSettings.MetaDescription,
                    CookieBanner = template.StyleSettings.CookieBanner
                },
                Contents = template.TemplateDetails.Select(td => new ContentResponse
                {
                    ElementType = td.ElementType,
                    Position = td.Position,
                    Size = td.Size,
                    Style = td.Style,
                    Element = td.Element
                }).ToList()
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var bio = await _repo.GetByIdAsync(id);
            if (bio == null) return false;
            await _repo.DeleteAsync(bio);
            return true;
        }

        public async Task<IEnumerable<BioPageResponse>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(b => new BioPageResponse
            {
                BioPageId = b.BioPageId,
                Title = b.Title,
                Description = b.Description,
                Avatar = b.Avatar,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            });
        }

        public async Task<BioPageResponse?> GetByIdAsync(string id)
        {
            var bio = await _repo.GetByIdAsync(id);
            if (bio == null) return null;

            var background = bio.Background == null ? null : new BackgroundResponse
            {
                BackgroundId = bio.Background.BackgroundId,
                Type = bio.Background.Type,
                Value = bio.Background.Value
            };

            var style = bio.Style == null ? null : new StyleResponse
            {
                StyleId = bio.Style.StyleId,
                Preset = bio.Style.Preset,
                LayoutMode = bio.Style.LayoutMode,
                ButtonColor = bio.Style.ButtonColor,
                IconColor = bio.Style.IconColor,
                BackgroundColor = bio.Style.BackgroundColor
            };

            var settings = bio.StyleSettings == null ? null : new StyleSettingsResponse
            {
                StyleSettingsId = bio.StyleSettings.StyleSettingsId,
                Thumbnail = bio.StyleSettings.Thumbnail,
                MetaTitle = bio.StyleSettings.MetaTitle,
                MetaDescription = bio.StyleSettings.MetaDescription,
                CookieBanner = bio.StyleSettings.CookieBanner
            };

            var contents = bio.Contents.Select(c => new ContentResponse
            {
                ContentId = c.ContentId,
                ElementType = c.ElementType,
                Alignment = c.Alignment,
                Visible = c.Visible,
                Position = c.Position,
                Size = c.Size,
                Style = c.Style,
                Element = c.Element
            }).ToList();

            return new BioPageResponse
            {
                BioPageId = bio.BioPageId,
                UserId = bio.UserId,
                TemplateId = bio.TemplateId,
                Title = bio.Title,
                Description = bio.Description,
                Avatar = bio.Avatar,
                Status = bio.Status,
                CreatedAt = bio.CreatedAt,
                Background = background,
                Style = style,
                StyleSettings = settings,
                Contents = contents
            };
        }

        public async Task<IEnumerable<BioPageResponse>> GetByUserIdAsync(string userId)
        {
            var bios = await _repo.GetByUserIdAsync(userId);
            return bios.Select(b => new BioPageResponse
            {
                BioPageId = b.BioPageId,
                UserId = b.UserId,
                Title = b.Title,
                Description = b.Description,
                Avatar = b.Avatar,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                Background = b.Background == null ? null : new BackgroundResponse
                {
                    BackgroundId = b.Background.BackgroundId,
                    Type = b.Background.Type,
                    Value = b.Background.Value
                },
                Style = b.Style == null ? null : new StyleResponse
                {
                    StyleId = b.Style.StyleId,
                    Preset = b.Style.Preset,
                    LayoutMode = b.Style.LayoutMode,
                    ButtonColor = b.Style.ButtonColor,
                    IconColor = b.Style.IconColor,
                    BackgroundColor = b.Style.BackgroundColor
                },
                StyleSettings = b.StyleSettings == null ? null : new StyleSettingsResponse
                {
                    StyleSettingsId = b.StyleSettings.StyleSettingsId,
                    Thumbnail = b.StyleSettings.Thumbnail,
                    MetaTitle = b.StyleSettings.MetaTitle,
                    MetaDescription = b.StyleSettings.MetaDescription,
                    CookieBanner = b.StyleSettings.CookieBanner
                },
                Contents = b.Contents.Select(c => new ContentResponse
                {
                    ContentId = c.ContentId,
                    ElementType = c.ElementType,
                    Position = c.Position,
                    Size = c.Size,
                    Style = c.Style,
                    Element = c.Element
                }).ToList()
            });
        }
        

        public async Task<BioPageResponse?> UpdateAsync(string id, UpdateBioPage dto)
        {
            var bio = await _repo.GetByIdAsync(id);
            if (bio == null) return null;

            bio.Title = dto.Title ?? bio.Title;
            bio.Description = dto.Description ?? bio.Description;
            bio.Avatar = dto.Avatar ?? bio.Avatar;
            bio.Status = dto.Status ?? bio.Status;

            await _repo.UpdateAsync(bio);

            return new BioPageResponse
            {
                BioPageId = bio.BioPageId,
                Title = bio.Title,
                Description = bio.Description,
                Avatar = bio.Avatar,
                Status = bio.Status,
                CreatedAt = bio.CreatedAt
            };
        }

        public async Task<BioPageResponse?> UpdateFullAsync(string id, UpdateFullBioPage dto)
        {
            var bio = await _repo.GetByIdAsync(id);
            if (bio == null) return null;

            if (dto.BioPage != null)
            {
                bio.Title = dto.BioPage.Title ?? bio.Title;
                bio.Description = dto.BioPage.Description ?? bio.Description;
                bio.Avatar = dto.BioPage.Avatar ?? bio.Avatar;
                bio.Status = dto.BioPage.Status ?? bio.Status;
            }

            if (dto.Background != null && bio.BackgroundId != null)
            {
                await _backgroundService.UpdateAsync(bio.BackgroundId, dto.Background);
            }

            if (dto.Style != null && bio.StyleId != null)
            {
                await _styleService.UpdateAsync(bio.StyleId, dto.Style);
            }

            if (dto.StyleSettings != null && bio.StyleSettings != null)
            {
                await _styleSettingsService.UpdateAsync(bio.StyleSettings.StyleSettingsId!, dto.StyleSettings);
            }

            if (dto.Contents != null)
            {
                foreach (var content in dto.Contents)
                {
                    await _contentService.UpdateContent(content.ContentId!, content);
                }
            }

            await _repo.UpdateAsync(bio);

            return await GetByIdAsync(bio.BioPageId!);
        }
    }
}
