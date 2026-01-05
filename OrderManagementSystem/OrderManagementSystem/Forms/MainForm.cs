using System;
using System.Drawing;
using System.Windows.Forms;
using OrderManagementSystem.Models;
using OrderManagementSystem.Forms;

namespace OrderManagementSystem
{
	public partial class MainForm : Form
	{
		private User currentUser;

		// Controls
		private Label lblWelcome;
		private Button menuViewOrders;
		private Button menuCreateOrder;
		private Button menuViewFoodItems;
		private Button menuAddFood;
		private Button menuAddEmployee;
		private Button menuReports;
		private Button menuLogout;
		private Button menuExit;

		public MainForm(User user)
		{
			currentUser = user;
			InitializeComponent();

			// عرض رسالة الترحيب
			lblWelcome.Text = $"مرحباً {user.FullName} ({user.UserType})";

			// إخفاء القوائم حسب نوع المستخدم
			SetupMenuBasedOnUserType();
		}

		private void SetupMenuBasedOnUserType()
		{
			if (currentUser.UserType == "Employee")
			{
				menuAddEmployee.Visible = false;
				menuAddFood.Visible = false;
				menuReports.Visible = false;
			}
		}

		private void menuViewOrders_Click(object sender, EventArgs e)
		{
			OrdersForm ordersForm = new OrdersForm(currentUser);
			ordersForm.ShowDialog();
		}

		private void menuCreateOrder_Click(object sender, EventArgs e)
		{
			CreateOrderForm createOrderForm = new CreateOrderForm(currentUser);
			createOrderForm.ShowDialog();
		}

		private void menuViewFoodItems_Click(object sender, EventArgs e)
		{
			FoodItemsForm foodForm = new FoodItemsForm(currentUser);
			foodForm.ShowDialog();
		}

		private void menuAddFood_Click(object sender, EventArgs e)
		{
			AddFoodForm addFoodForm = new AddFoodForm();
			addFoodForm.ShowDialog();
		}

		private void menuAddEmployee_Click(object sender, EventArgs e)
		{
			AddEmployeeForm addEmpForm = new AddEmployeeForm();
			addEmpForm.ShowDialog();
		}

		private void menuReports_Click(object sender, EventArgs e)
		{
			ReportsForm reportsForm = new ReportsForm();
			reportsForm.ShowDialog();
		}

		private void menuLogout_Click(object sender, EventArgs e)
		{
			LoginForm loginForm = new LoginForm();
			loginForm.Show();
			this.Close();
		}

		private void menuExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void InitializeComponent()
		{
			SuspendLayout();

			// Label الترحيب
			lblWelcome = new Label();
			lblWelcome.Location = new Point(50, 20);
			lblWelcome.Size = new Size(400, 30);
			lblWelcome.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
			lblWelcome.Text = "مرحباً";
			Controls.Add(lblWelcome);

			// زر عرض الطلبات
			menuViewOrders = new Button();
			menuViewOrders.Location = new Point(558, 50);
			menuViewOrders.Size = new Size(282, 43);
			menuViewOrders.Text = "عرض الطلبات";
			menuViewOrders.Click += menuViewOrders_Click;
			Controls.Add(menuViewOrders);

			// زر إنشاء طلب
			menuCreateOrder = new Button();
			menuCreateOrder.Location = new Point(556, 123);
			menuCreateOrder.Size = new Size(284, 47);
			menuCreateOrder.Text = "طلب جديد";
			menuCreateOrder.Click += menuCreateOrder_Click;
			Controls.Add(menuCreateOrder);

			// زر عرض المأكولات
			menuViewFoodItems = new Button();
			menuViewFoodItems.Location = new Point(555, 213);
			menuViewFoodItems.Size = new Size(285, 48);
			menuViewFoodItems.Text = "عرض المأكولات";
			menuViewFoodItems.Click += menuViewFoodItems_Click;
			Controls.Add(menuViewFoodItems);

			// زر إضافة مأكولات
			menuAddFood = new Button();
			menuAddFood.Location = new Point(558, 303);
			menuAddFood.Size = new Size(289, 51);
			menuAddFood.Text = "إضافة مأكولات (للمسؤول)";
			menuAddFood.Click += menuAddFood_Click;
			Controls.Add(menuAddFood);

			// زر إضافة موظف
			menuAddEmployee = new Button();
			menuAddEmployee.Location = new Point(553, 402);
			menuAddEmployee.Size = new Size(287, 50);
			menuAddEmployee.Text = "إضافة موظف (للمسؤول)";
			menuAddEmployee.Click += menuAddEmployee_Click;
			Controls.Add(menuAddEmployee);

			// زر التقارير
			menuReports = new Button();
			menuReports.Location = new Point(553, 489);
			menuReports.Size = new Size(287, 60);
			menuReports.Text = "التقارير (للمسؤول)";
			menuReports.Click += menuReports_Click;
			Controls.Add(menuReports);

			// زر تسجيل الخروج
			menuLogout = new Button();
			menuLogout.Location = new Point(1172, 456);
			menuLogout.Size = new Size(251, 52);
			menuLogout.Text = "تسجيل الخروج";
			menuLogout.Click += menuLogout_Click;
			Controls.Add(menuLogout);

			// زر إغلاق التطبيق
			menuExit = new Button();
			menuExit.Location = new Point(1173, 529);
			menuExit.Size = new Size(250, 53);
			menuExit.Text = "إغلاق التطبيق";
			menuExit.Click += menuExit_Click;
			Controls.Add(menuExit);

			// إعدادات الفورم
			ClientSize = new Size(1483, 607);
			Name = "MainForm";
			Text = "النظام الرئيسي";
			ResumeLayout(false);
		}
	}
}
