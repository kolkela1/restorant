namespace OrderManagementSystem.Models
{
	public class Order
	{
		public int OrderID { get; set; }
		public string OrderCode { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhone { get; set; }
		public string CustomerAddress { get; set; }
		public decimal TotalAmount { get; set; }
		public string Status { get; set; }
		public int EmployeeID { get; set; }
		public string EmployeeName { get; set; }
		public DateTime CreatedAt { get; set; }
		public List<OrderItem> Items { get; set; } = new List<OrderItem>();
	}

	public class OrderItem
	{
		public int OrderItemID { get; set; }
		public int OrderID { get; set; }
		public int FoodID { get; set; }
		public string FoodName { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal SubTotal { get; set; }
	}
}