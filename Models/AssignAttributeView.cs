namespace CultureWeb.Models
{
    public class AssignAttributeView
    {
        public int ProductId { get; set; }
        public List<Attribute> AvailableAttributes { get; set; }
        public List<int> AssignedAttributeIds { get; set; }
        public int SelectedAttributeId { get; set; }
        public List<int> AvailableAttributeIds { get; set; }
    }
}
