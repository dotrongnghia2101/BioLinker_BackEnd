using BioLinker.DTO;
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
            var entity = new BioPage
            {
                UserId = dto.UserId,
                TemplateId = dto.TemplateId,
                Title = dto.Title,
                Description = dto.Description,
                Avatar = dto.Avatar,
                BackgroundId = dto.BackgroundId,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(entity);
            return new BioPageResponse
            {
                BioPageId = entity.BioPageId,
                Title = entity.Title,
                Description = entity.Description,
                Avatar = entity.Avatar,
                CustomerDomain = entity.CustomerDomain,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<BioPageResponse?> CreateFromTemplateAsync(string templateId, CreateBioPageFromTemplate dto)
        {
            var template = await _templateRepo.GetByIdAsync(templateId);
            if (template == null) return null;

            var newBio = new BioPage
            {
                BioPageId = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                TemplateId = template.TemplateId,
                Title = dto.Title ?? template.Name,
                Description = dto.Description ?? template.Description,
                Avatar = null,
                CustomerDomain = dto.CustomerDomain ?? "",
                Status = dto.Status ?? "Active",
                CreatedAt = DateTime.UtcNow
            };

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
            var newBios = new BioPage
            {
                BioPageId = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                TemplateId = template.TemplateId,
                Title = dto.Title ?? template.Name,
                Description = dto.Description ?? template.Description,
                Avatar = null,
                CustomerDomain = dto.CustomerDomain ?? "",
                Status = dto.Status ?? "Active",
                CreatedAt = DateTime.UtcNow,
                StyleId = styleId,
                BackgroundId = backgroundId,
                StyleSettingsId = styleSettingsId
            };

            await _repo.AddAsync(newBios);

            // template detail
            if (template.TemplateDetails != null && template.TemplateDetails.Any())
            {
                foreach (var item in template.TemplateDetails)
                {
                    var newContent = new CreateContent
                    {
                        BioPageId = newBios.BioPageId,
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
                CustomerDomain = newBio.CustomerDomain,
                Status = newBio.Status,
                CreatedAt = newBio.CreatedAt,

                Template = new TemplateDTO
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
                CustomerDomain = b.CustomerDomain,
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
                CustomerDomain = bio.CustomerDomain,
                Status = bio.Status,
                CreatedAt = bio.CreatedAt,
                Background = background,
                Style = style,
                StyleSettings = settings,
                Contents = contents
            };
        }

        public async Task<BioPageResponse?> UpdateAsync(string id, UpdateBioPage dto)
        {
            var bio = await _repo.GetByIdAsync(id);
            if (bio == null) return null;

            bio.Title = dto.Title ?? bio.Title;
            bio.Description = dto.Description ?? bio.Description;
            bio.Avatar = dto.Avatar ?? bio.Avatar;
            bio.CustomerDomain = dto.CustomerDomain ?? bio.CustomerDomain;
            bio.Status = dto.Status ?? bio.Status;

            await _repo.UpdateAsync(bio);

            return new BioPageResponse
            {
                BioPageId = bio.BioPageId,
                Title = bio.Title,
                Description = bio.Description,
                Avatar = bio.Avatar,
                CustomerDomain = bio.CustomerDomain,
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
                bio.CustomerDomain = dto.BioPage.CustomerDomain ?? bio.CustomerDomain;
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
