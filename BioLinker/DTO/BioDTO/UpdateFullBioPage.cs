namespace BioLinker.DTO.BioDTO
{
    public class UpdateFullBioPage
    {
        public UpdateBioPage? BioPage { get; set; }
        public BackgroundUpdate? Background { get; set; }
        public UpdateStyle? Style { get; set; }
        public UpdateStyleSettings? StyleSettings { get; set; }
        public List<UpdateContent>? Contents { get; set; }
    }
}
