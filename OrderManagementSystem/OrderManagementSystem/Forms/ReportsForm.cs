using System.Windows.Forms;

namespace OrderManagementSystem.Forms
{
	public partial class ReportsForm : Form
	{
		public ReportsForm()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = "التقارير";
			this.ClientSize = new System.Drawing.Size(800, 500);
			// هنا تضيف DataGridView أو Charts للتقارير
		}
	}
}
