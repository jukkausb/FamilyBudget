namespace FamilyBudget.v3.Models
{
    public class FormActionButtonsModel
    {
        public bool RenderSaveButton { get; set; }
        public bool RenderSaveAndCreateButton { get; set; }
        public bool RenderDeleteButton { get; set; }
        public bool RenderBackButton { get; set; }
        public string BackButtonLink { get; set; }
    }
}