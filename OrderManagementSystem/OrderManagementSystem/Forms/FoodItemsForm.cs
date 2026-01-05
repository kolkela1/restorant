using System.Windows.Forms;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Forms
{
	public partial class FoodItemsForm : Form
	{
		private User currentUser;

		public FoodItemsForm(User user)
		{
			currentUser = user;
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = "عرض المأكولات";
			this.ClientSize = new System.Drawing.Size(800, 500);
			// هنا ممكن تضيف DataGridView لعرض الأطعمة
		}
	}
}
