using System.Windows.Forms;

namespace OrderManagementSystem.Forms
{
	public partial class AddFoodForm : Form
	{
		public AddFoodForm()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = "إضافة مأكولات جديدة";
			this.ClientSize = new System.Drawing.Size(600, 400);
			// هنا تضيف TextBoxes, ComboBoxes, Buttons لكل بيانات الطعام
		}
	}
}
