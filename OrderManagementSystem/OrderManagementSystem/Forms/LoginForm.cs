using System;
using System.Drawing;
using System.Windows.Forms;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Forms
{
	public partial class LoginForm : Form
	{
		private AuthService authService;

		public LoginForm()
		{
			InitializeComponent();
			authService = new AuthService();
		}

		private void btnLogin_Click(object sender, EventArgs e)
		{
			string username = txtUsername.Text.Trim();
			string password = txtPassword.Text;

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				MessageBox.Show(
					"يرجى إدخال اسم المستخدم وكلمة المرور",
					"خطأ",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				return;
			}

			try
			{
				User user = authService.Login(username, password);

				if (user != null)
				{
					MainForm mainForm = new MainForm(user);
					mainForm.Show();
					this.Hide();
				}
				else
				{
					MessageBox.Show(
						"اسم المستخدم أو كلمة المرور غير صحيحة",
						"خطأ في تسجيل الدخول",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"حدث خطأ: {ex.Message}",
					"خطأ",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void InitializeComponent()
		{
			btnLogin = new Button();
			btnExit = new Button();
			label1 = new Label();
			txtUsername = new TextBox();
			label2 = new Label();
			txtPassword = new TextBox();
			SuspendLayout();

			// btnLogin
			btnLogin.BackColor = Color.White;
			btnLogin.Location = new Point(973, 100);
			btnLogin.Name = "btnLogin";
			btnLogin.Size = new Size(375, 83);
			btnLogin.TabIndex = 0;
			btnLogin.Text = "تسجيل دخول";
			btnLogin.UseVisualStyleBackColor = false;
			btnLogin.Click += btnLogin_Click;

			// btnExit
			btnExit.Location = new Point(973, 242);
			btnExit.Name = "btnExit";
			btnExit.Size = new Size(375, 71);
			btnExit.TabIndex = 1;
			btnExit.Text = "خروج";
			btnExit.UseVisualStyleBackColor = true;
			btnExit.Click += btnExit_Click;

			// label1
			label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
			label1.Location = new Point(260, 151);
			label1.Name = "label1";
			label1.Size = new Size(150, 36);
			label1.TabIndex = 2;
			label1.Text = "اسم المستخدم";

			// txtUsername
			txtUsername.Location = new Point(421, 151);
			txtUsername.Name = "txtUsername";
			txtUsername.Size = new Size(442, 23);
			txtUsername.TabIndex = 3;

			// label2
			label2.AutoSize = true;
			label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
			label2.Location = new Point(260, 226);
			label2.Name = "label2";
			label2.Size = new Size(90, 21);
			label2.TabIndex = 4;
			label2.Text = "كلمة المرور";

			// txtPassword
			txtPassword.Location = new Point(421, 224);
			txtPassword.Name = "txtPassword";
			txtPassword.Size = new Size(444, 23);
			txtPassword.TabIndex = 5;
			txtPassword.PasswordChar = '*';

			// LoginForm
			ClientSize = new Size(1488, 631);
			Controls.Add(txtPassword);
			Controls.Add(label2);
			Controls.Add(txtUsername);
			Controls.Add(label1);
			Controls.Add(btnExit);
			Controls.Add(btnLogin);
			Name = "LoginForm";
			Text = "تسجيل الدخول";
			ResumeLayout(false);
			PerformLayout();
		}

		private Button btnLogin;
		private Button btnExit;
		private Label label1;
		private TextBox txtUsername;
		private Label label2;
		private TextBox txtPassword;
	}
}
