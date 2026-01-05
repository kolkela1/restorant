using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Forms
{
	public partial class CreateOrderForm : Form
	{
		private User currentUser;
		private DatabaseService dbService;
		private List<OrderItem> orderItems = new List<OrderItem>();
		private List<FoodItem> availableFoods = new List<FoodItem>();

		// عناصر التحكم
		private Button btnAddItem;
		private Button btnCreateOrder;
		private Button btnClear;
		private Button btnClose;
		private DataGridView dgvOrderItems;
		private TextBox txtOrderCode;
		private TextBox txtCustomerName;
		private TextBox txtCustomerPhone;
		private TextBox txtCustomerAddress;
		private ComboBox comboFoodItems;
		private TextBox txtQuantity;
		private Label lblTotalAmount;

		public CreateOrderForm(User user)
		{
			currentUser = user;
			dbService = new DatabaseService();
			InitializeComponent();
			LoadFoodItems();
			GenerateOrderCode();
		}

		private void GenerateOrderCode()
		{
			txtOrderCode.Text = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss");
		}

		private void LoadFoodItems()
		{
			try
			{
				using (var connection = dbService.GetConnection())
				{
					connection.Open();
					string query = "SELECT * FROM FoodItems WHERE IsAvailable = 1 ORDER BY Category, Name";

					availableFoods.Clear();
					comboFoodItems.Items.Clear();

					using (var cmd = new MySqlCommand(query, connection))
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var food = new FoodItem
							{
								FoodID = Convert.ToInt32(reader["FoodID"]),
								Name = reader["Name"].ToString(),
								Price = Convert.ToDecimal(reader["Price"]),
								Category = reader["Category"].ToString()
							};
							availableFoods.Add(food);
							comboFoodItems.Items.Add($"{food.Name} - {food.Price:C}");
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"حدث خطأ في تحميل الأصناف: {ex.Message}", "خطأ",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnAddItem_Click(object sender, EventArgs e)
		{
			if (comboFoodItems.SelectedIndex < 0)
			{
				MessageBox.Show("يرجى اختيار صنف", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
			{
				MessageBox.Show("يرجى إدخال كمية صحيحة", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var selectedFood = availableFoods[comboFoodItems.SelectedIndex];
			var existingItem = orderItems.FirstOrDefault(i => i.FoodID == selectedFood.FoodID);

			if (existingItem != null)
			{
				existingItem.Quantity += quantity;
				existingItem.SubTotal = existingItem.Quantity * existingItem.UnitPrice;
			}
			else
			{
				orderItems.Add(new OrderItem
				{
					FoodID = selectedFood.FoodID,
					FoodName = selectedFood.Name,
					Quantity = quantity,
					UnitPrice = selectedFood.Price,
					SubTotal = quantity * selectedFood.Price
				});
			}

			UpdateOrderItemsGrid();
			CalculateTotal();
		}

		private void UpdateOrderItemsGrid()
		{
			dgvOrderItems.Rows.Clear();
			foreach (var item in orderItems)
			{
				dgvOrderItems.Rows.Add(
					item.FoodName,
					item.Quantity,
					item.UnitPrice,
					item.SubTotal,
					"حذف"
				);
			}
		}

		private void CalculateTotal()
		{
			decimal total = orderItems.Sum(item => item.SubTotal);
			lblTotalAmount.Text = $"المبلغ الإجمالي: {total:C}";
		}

		private void dgvOrderItems_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || dgvOrderItems.Columns[e.ColumnIndex].Name != "Delete") return;

			orderItems.RemoveAt(e.RowIndex);
			UpdateOrderItemsGrid();
			CalculateTotal();
		}

		private void btnCreateOrder_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtCustomerName.Text) ||
				string.IsNullOrEmpty(txtCustomerPhone.Text) ||
				string.IsNullOrEmpty(txtCustomerAddress.Text))
			{
				MessageBox.Show("يرجى إدخال جميع بيانات العميل", "تحذير",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (orderItems.Count == 0)
			{
				MessageBox.Show("يرجى إضافة أصناف إلى الطلب", "تحذير",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				decimal totalAmount = orderItems.Sum(item => item.SubTotal);
				int orderId = 0;

				using (var connection = dbService.GetConnection())
				{
					connection.Open();
					using (var transaction = connection.BeginTransaction())
					{
						try
						{
							string orderQuery = @"INSERT INTO Orders 
                                (OrderCode, CustomerName, CustomerPhone, CustomerAddress, 
                                 TotalAmount, Status, EmployeeID) 
                                VALUES (@code, @name, @phone, @address, @total, 'Pending', @empid);
                                SELECT LAST_INSERT_ID();";

							using (var cmd = new MySqlCommand(orderQuery, connection, transaction))
							{
								cmd.Parameters.AddWithValue("@code", txtOrderCode.Text);
								cmd.Parameters.AddWithValue("@name", txtCustomerName.Text);
								cmd.Parameters.AddWithValue("@phone", txtCustomerPhone.Text);
								cmd.Parameters.AddWithValue("@address", txtCustomerAddress.Text);
								cmd.Parameters.AddWithValue("@total", totalAmount);
								cmd.Parameters.AddWithValue("@empid", currentUser.UserID);

								orderId = Convert.ToInt32(cmd.ExecuteScalar());
							}

							foreach (var item in orderItems)
							{
								string itemQuery = @"INSERT INTO OrderItems 
                                    (OrderID, FoodID, Quantity, UnitPrice, SubTotal) 
                                    VALUES (@orderid, @foodid, @qty, @price, @subtotal)";

								using (var cmd = new MySqlCommand(itemQuery, connection, transaction))
								{
									cmd.Parameters.AddWithValue("@orderid", orderId);
									cmd.Parameters.AddWithValue("@foodid", item.FoodID);
									cmd.Parameters.AddWithValue("@qty", item.Quantity);
									cmd.Parameters.AddWithValue("@price", item.UnitPrice);
									cmd.Parameters.AddWithValue("@subtotal", item.SubTotal);
									cmd.ExecuteNonQuery();
								}
							}

							transaction.Commit();

							MessageBox.Show($"تم إنشاء الطلب بنجاح!\nرقم الطلب: {txtOrderCode.Text}",
								"نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

							ResetForm();
						}
						catch
						{
							transaction.Rollback();
							throw;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"حدث خطأ في إنشاء الطلب: {ex.Message}", "خطأ",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ResetForm()
		{
			txtCustomerName.Clear();
			txtCustomerPhone.Clear();
			txtCustomerAddress.Clear();
			orderItems.Clear();
			UpdateOrderItemsGrid();
			CalculateTotal();
			GenerateOrderCode();
			LoadFoodItems();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void InitializeComponent()
		{
			btnAddItem = new Button();
			btnCreateOrder = new Button();
			btnClear = new Button();
			btnClose = new Button();
			dgvOrderItems = new DataGridView();
			txtOrderCode = new TextBox();
			txtCustomerName = new TextBox();
			txtCustomerPhone = new TextBox();
			txtCustomerAddress = new TextBox();
			comboFoodItems = new ComboBox();
			txtQuantity = new TextBox();
			lblTotalAmount = new Label();

			((System.ComponentModel.ISupportInitialize)dgvOrderItems).BeginInit();
			SuspendLayout();

			// DataGridView إعداد الأعمدة
			dgvOrderItems.ColumnCount = 4;
			dgvOrderItems.Columns[0].Name = "FoodName";
			dgvOrderItems.Columns[1].Name = "Quantity";
			dgvOrderItems.Columns[2].Name = "UnitPrice";
			dgvOrderItems.Columns[3].Name = "SubTotal";

			var deleteButton = new DataGridViewButtonColumn()
			{
				Text = "حذف",
				UseColumnTextForButtonValue = true,
				Name = "Delete"
			};
			dgvOrderItems.Columns.Add(deleteButton);
			dgvOrderItems.CellClick += dgvOrderItems_CellClick;

			// ربط الأحداث
			btnAddItem.Click += btnAddItem_Click;
			btnCreateOrder.Click += btnCreateOrder_Click;
			btnClose.Click += btnClose_Click;

			// مثال مواقع العناصر (يمكنك تعديلها حسب التصميم)
			dgvOrderItems.Location = new Point(20, 20);
			dgvOrderItems.Size = new Size(600, 400);

			comboFoodItems.Location = new Point(650, 50);
			comboFoodItems.Size = new Size(300, 25);

			txtQuantity.Location = new Point(650, 90);
			txtQuantity.Size = new Size(100, 25);

			btnAddItem.Location = new Point(650, 130);
			btnAddItem.Size = new Size(150, 40);
			btnAddItem.Text = "إضافة صنف";

			txtCustomerName.Location = new Point(650, 200);
			txtCustomerName.PlaceholderText = "اسم العميل";
			txtCustomerPhone.Location = new Point(650, 240);
			txtCustomerPhone.PlaceholderText = "رقم الهاتف";
			txtCustomerAddress.Location = new Point(650, 280);
			txtCustomerAddress.PlaceholderText = "العنوان";

			btnCreateOrder.Location = new Point(650, 330);
			btnCreateOrder.Size = new Size(150, 40);
			btnCreateOrder.Text = "إنشاء الطلب";

			btnClose.Location = new Point(650, 380);
			btnClose.Size = new Size(150, 40);
			btnClose.Text = "إغلاق";

			txtOrderCode.Location = new Point(650, 420);
			txtOrderCode.Size = new Size(200, 25);
			txtOrderCode.ReadOnly = true;

			lblTotalAmount.Location = new Point(650, 460);
			lblTotalAmount.Size = new Size(200, 25);
			lblTotalAmount.Text = "المبلغ الإجمالي: 0";

			Controls.Add(dgvOrderItems);
			Controls.Add(comboFoodItems);
			Controls.Add(txtQuantity);
			Controls.Add(btnAddItem);
			Controls.Add(txtCustomerName);
			Controls.Add(txtCustomerPhone);
			Controls.Add(txtCustomerAddress);
			Controls.Add(btnCreateOrder);
			Controls.Add(btnClose);
			Controls.Add(txtOrderCode);
			Controls.Add(lblTotalAmount);

			ClientSize = new Size(1000, 550);
			Name = "CreateOrderForm";
			Text = "إنشاء طلب";
			((System.ComponentModel.ISupportInitialize)dgvOrderItems).EndInit();
			ResumeLayout(false);
		}
	}
}
