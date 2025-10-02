using BioLinker.DTO;
using BioLinker.Enities;
using BioLinker.Respository.BioPageRepo;

namespace BioLinker.Service
{
    public class StyleService : IStyleService
    {
        private readonly IStyleRepository _repo;

        public StyleService(IStyleRepository repo)
        {
            _repo = repo;
        }

        public async Task<StyleResponse> CreateAsync(CreateStyle dto)
        {
            var style = new Style
            {
                StyleId = Guid.NewGuid().ToString(),
                Preset = dto.Preset ?? string.Empty,
                LayoutMode = dto.LayoutMode,
                ButtonShape = dto.ButtonShape,
                ButtonColor = dto.ButtonColor,
                IconColor = dto.IconColor,
                BackgroundColor = dto.BackgroundColor
            };

            await _repo.AddAsync(style);

            return new StyleResponse
            {
                StyleId = style.StyleId,
                Preset = style.Preset,
                LayoutMode = style.LayoutMode,
                ButtonShape = style.ButtonShape,
                ButtonColor = style.ButtonColor,
                IconColor = style.IconColor,
                BackgroundColor = style.BackgroundColor
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var style = await _repo.GetByIdAsync(id);
            if (style == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<StyleResponse>> GetAllAsync()
        {
            var styles = await _repo.GetAllAsync();
            return styles.Select(s => new StyleResponse
            {
                StyleId = s.StyleId,
                Preset = s.Preset,
                LayoutMode = s.LayoutMode,
                ButtonShape = s.ButtonShape,
                ButtonColor = s.ButtonColor,
                IconColor = s.IconColor,
                BackgroundColor = s.BackgroundColor
            });
        }

        public async Task<StyleResponse?> GetByIdAsync(string id)
        {
            var s = await _repo.GetByIdAsync(id);
            if (s == null) return null;

            return new StyleResponse
            {
                StyleId = s.StyleId,
                Preset = s.Preset,
                LayoutMode = s.LayoutMode,
                ButtonShape = s.ButtonShape,
                ButtonColor = s.ButtonColor,
                IconColor = s.IconColor,
                BackgroundColor = s.BackgroundColor
            };
        }

        public async Task<bool> UpdateAsync(string id, UpdateStyle dto)
        {
            var style = await _repo.GetByIdAsync(id);
            if (style == null) return false;

            // update field nào duoc gui
            if (dto.LayoutMode != null) style.LayoutMode = dto.LayoutMode;
            if (dto.ButtonShape != null) style.ButtonShape = dto.ButtonShape;
            if (dto.ButtonColor != null) style.ButtonColor = dto.ButtonColor;
            if (dto.IconColor != null) style.IconColor = dto.IconColor;
            if (dto.BackgroundColor != null) style.BackgroundColor = dto.BackgroundColor;

            await _repo.UpdateAsync(style);
            return true;
        }
    }
}
