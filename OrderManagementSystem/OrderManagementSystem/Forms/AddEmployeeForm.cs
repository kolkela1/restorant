using System.Windows.Forms;

namespace OrderManagementSystem.Forms
{
	public partial class AddEmployeeForm : Form
	{
		public AddEmployeeForm()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = "إضافة موظف جديد";
			this.ClientSize = new System.Drawing.Size(600, 400);
			// هنا ممكن تضيف TextBoxes, Buttons, Labels لكل بيانات الموظف
		}
	}
}
