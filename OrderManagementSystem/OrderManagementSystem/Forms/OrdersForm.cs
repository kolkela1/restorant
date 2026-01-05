using MySql.Data.MySqlClient;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Forms
{
	public partial class OrdersForm : Form
	{
		private User currentUser;
		private Button btnRefresh;
		private Button btnClose;
		private DataGridView dgvOrders;
		private DataGridViewTextBoxColumn OrderCode;
		private DataGridViewCheckBoxColumn Edit;
		private DataGridViewCheckBoxColumn Confirm;
		private DataGridViewCheckBoxColumn Delete;
		private DataGridViewTextBoxColumn CustomerName;
		private DataGridViewTextBoxColumn TotalAmount;
		private DataGridViewTextBoxColumn Status;
		private DataGridViewTextBoxColumn CreatedAt;
		private DatabaseService dbService;

		public OrdersForm(User user)
		{
			InitializeComponent();
			currentUser = user;
			dbService = new DatabaseService();
			LoadOrders();
			SetupDataGridView();
		}

		private void SetupDataGridView()
		{
			dgvOrders.AutoGenerateColumns = false;
			dgvOrders.Columns.Clear();

			// إضافة الأعمدة
			dgvOrders.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "OrderCode",
				HeaderText = "كود الطلب",
				DataPropertyName = "OrderCode"
			});

			dgvOrders.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "CustomerName",
				HeaderText = "اسم العميل",
				DataPropertyName = "CustomerName"
			});

			dgvOrders.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "TotalAmount",
				HeaderText = "المبلغ",
				DataPropertyName = "TotalAmount",
				DefaultCellStyle = new DataGridViewCellStyle() { Format = "C2" }
			});

			dgvOrders.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "Status",
				HeaderText = "الحالة",
				DataPropertyName = "Status"
			});

			dgvOrders.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "CreatedAt",
				HeaderText = "تاريخ الطلب",
				DataPropertyName = "CreatedAt"
			});

			// زر الحذف
			var deleteButton = new DataGridViewButtonColumn()
			{
				Text = "حذف",
				UseColumnTextForButtonValue = true,
				Name = "Delete"
			};
			dgvOrders.Columns.Add(deleteButton);

			// زر التعديل
			var editButton = new DataGridViewButtonColumn()
			{
				Text = "تعديل",
				UseColumnTextForButtonValue = true,
				Name = "Edit"
			};
			dgvOrders.Columns.Add(editButton);

			// زر تأكيد الطلب
			var confirmButton = new DataGridViewButtonColumn()
			{
				Text = "تأكيد",
				UseColumnTextForButtonValue = true,
				Name = "Confirm"
			};
			dgvOrders.Columns.Add(confirmButton);
		}

		private void LoadOrders()
		{
			try
			{
				using (var connection = dbService.GetConnection())
				{
					connection.Open();
					string query = @"SELECT o.*, u.FullName as EmployeeName 
                                   FROM Orders o 
                                   LEFT JOIN Users u ON o.EmployeeID = u.UserID 
                                   ORDER BY o.CreatedAt DESC";

					var orders = new List<Order>();

					using (var cmd = new MySqlCommand(query, connection))
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							orders.Add(new Order
							{
								OrderID = Convert.ToInt32(reader["OrderID"]),
								OrderCode = reader["OrderCode"].ToString(),
								CustomerName = reader["CustomerName"].ToString(),
								CustomerPhone = reader["CustomerPhone"].ToString(),
								CustomerAddress = reader["CustomerAddress"].ToString(),
								TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
								Status = reader["Status"].ToString(),
								EmployeeName = reader["EmployeeName"].ToString(),
								CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
							});
						}
					}

					dgvOrders.DataSource = orders;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"حدث خطأ في تحميل الطلبات: {ex.Message}", "خطأ",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void dgvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;

			var order = dgvOrders.Rows[e.RowIndex].DataBoundItem as Order;
			if (order == null) return;

			if (e.ColumnIndex == dgvOrders.Columns["Delete"].Index)
			{
				if (MessageBox.Show("هل تريد حذف هذا الطلب؟", "تأكيد الحذف",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					DeleteOrder(order.OrderID);
				}
			}
			else if (e.ColumnIndex == dgvOrders.Columns["Edit"].Index)
			{
				EditOrder(order);
			}
			else if (e.ColumnIndex == dgvOrders.Columns["Confirm"].Index)
			{
				ConfirmOrder(order.OrderID);
			}
		}

		private void DeleteOrder(int orderId)
		{
			try
			{
				using (var connection = dbService.GetConnection())
				{
					connection.Open();
					string query = "DELETE FROM Orders WHERE OrderID = @orderid";

					using (var cmd = new MySqlCommand(query, connection))
					{
						cmd.Parameters.AddWithValue("@orderid", orderId);
						cmd.ExecuteNonQuery();
					}
				}

				MessageBox.Show("تم حذف الطلب بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
				LoadOrders();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"حدث خطأ في حذف الطلب: {ex.Message}", "خطأ",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void EditOrder(Order order)
		{
			// يمكن إضافة نافذة تعديل الطلب هنا
			MessageBox.Show("ميزة تعديل الطلب قيد التطوير", "معلومة",
				MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void ConfirmOrder(int orderId)
		{
			try
			{
				using (var connection = dbService.GetConnection())
				{
					connection.Open();
					string query = "UPDATE Orders SET Status = 'Ready' WHERE OrderID = @orderid";

					using (var cmd = new MySqlCommand(query, connection))
					{
						cmd.Parameters.AddWithValue("@orderid", orderId);
						cmd.ExecuteNonQuery();
					}
				}

				MessageBox.Show("تم تأكيد الطلب بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
				LoadOrders();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"حدث خطأ في تأكيد الطلب: {ex.Message}", "خطأ",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			LoadOrders();
		}

		private void InitializeComponent()
		{
			btnRefresh = new Button();
			btnClose = new Button();
			dgvOrders = new DataGridView();
			OrderCode = new DataGridViewTextBoxColumn();
			Edit = new DataGridViewCheckBoxColumn();
			Confirm = new DataGridViewCheckBoxColumn();
			Delete = new DataGridViewCheckBoxColumn();
			CustomerName = new DataGridViewTextBoxColumn();
			TotalAmount = new DataGridViewTextBoxColumn();
			Status = new DataGridViewTextBoxColumn();
			CreatedAt = new DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)dgvOrders).BeginInit();
			SuspendLayout();
			// 
			// btnRefresh
			// 
			btnRefresh.Location = new Point(1150, 97);
			btnRefresh.Name = "btnRefresh";
			btnRefresh.Size = new Size(278, 47);
			btnRefresh.TabIndex = 0;
			btnRefresh.Text = "تحديث القائمة";
			btnRefresh.UseVisualStyleBackColor = true;
			btnRefresh.Click += btnRefresh_Click_1;
			// 
			// btnClose
			// 
			btnClose.Location = new Point(1151, 227);
			btnClose.Name = "btnClose";
			btnClose.Size = new Size(277, 55);
			btnClose.TabIndex = 1;
			btnClose.Text = "إغلاق النافذة";
			btnClose.UseVisualStyleBackColor = true;
			// 
			// dgvOrders
			// 
			dgvOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgvOrders.Columns.AddRange(new DataGridViewColumn[] { OrderCode, Edit, Confirm, Delete, CustomerName, TotalAmount, Status, CreatedAt });
			dgvOrders.Location = new Point(266, 23);
			dgvOrders.Name = "dgvOrders";
			dgvOrders.Size = new Size(842, 538);
			dgvOrders.TabIndex = 2;
			// 
			// OrderCode
			// 
			OrderCode.HeaderText = "كود الطلب";
			OrderCode.Name = "OrderCode";
			// 
			// Edit
			// 
			Edit.HeaderText = "تعديل";
			Edit.Name = "Edit";
			// 
			// Confirm
			// 
			Confirm.HeaderText = "تأكيد";
			Confirm.Name = "Confirm";
			// 
			// Delete
			// 
			Delete.HeaderText = "حذف";
			Delete.Name = "Delete";
			// 
			// CustomerName
			// 
			CustomerName.HeaderText = "اسم العميل";
			CustomerName.Name = "CustomerName";
			// 
			// TotalAmount
			// 
			TotalAmount.HeaderText = "المبلغ";
			TotalAmount.Name = "TotalAmount";
			// 
			// Status
			// 
			Status.HeaderText = "الحالة";
			Status.Name = "Status";
			// 
			// CreatedAt
			// 
			CreatedAt.HeaderText = "تاريخ الطلب";
			CreatedAt.Name = "CreatedAt";
			// 
			// OrdersForm
			// 
			ClientSize = new Size(1473, 630);
			Controls.Add(dgvOrders);
			Controls.Add(btnClose);
			Controls.Add(btnRefresh);
			Name = "OrdersForm";
			Text = "عرض الاورارات";
			Load += OrdersForm_Load;
			((System.ComponentModel.ISupportInitialize)dgvOrders).EndInit();
			ResumeLayout(false);

		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void OrdersForm_Load(object sender, EventArgs e)
		{

		}

		private void btnRefresh_Click_1(object sender, EventArgs e)
		{

		}
	}
}