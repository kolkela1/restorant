namespace OrderManagementSystem.Models
{
	public class FoodItem
	{
		public int FoodID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string Category { get; set; }
		public bool IsAvailable { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}