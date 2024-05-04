using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Learning_Hub_QA_Project
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connectionString = "Data Source=LAPTOP-QF370PIT;Initial Catalog=LearningHubDB;Integrated Security=True";
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Redirect the user to register form
            Register register = new Register();
            register.Show();
            this.Hide();
        }

        private void login()
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            int count = (int)command.ExecuteScalar();

            if (count > 0)
            {
                MessageBox.Show("Login successful!");
                // Navigate to another form (user main page)
                Post post = new Post();
                post.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            login();
        }

    }
}
