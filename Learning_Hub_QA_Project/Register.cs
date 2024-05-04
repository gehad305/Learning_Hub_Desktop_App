using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Learning_Hub_QA_Project
{
    public partial class Register : Form
    {
        private SqlConnection sqlConnection;
        public Register()
        {
            InitializeComponent();
            string connectionString = "Data Source=LAPTOP-QF370PIT;Initial Catalog=LearningHubDB;Integrated Security=True";
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }

        private string ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return "Username field is required.";
            }

            if (username[0] == ' ')
            {
                return "Username should not start with a space.";
            }

            if (username.Any(c => !char.IsLetter(c)))
            {
                return "Username should not contain numbers or special characters.";
            }

            // Check if the username already exists in the database
            string selectQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            using (SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection))
            {
                selectCommand.Parameters.AddWithValue("@Username", username);

                int count = Convert.ToInt32(selectCommand.ExecuteScalar());
                if (count > 0)
                {
                    return "Please, Choose another name.";
                }
            }

            return string.Empty;
        }

        private string ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Email field is required.";
            }

            // Check if the email already exists in the database
            string selectQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            using (SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection))
            {
                selectCommand.Parameters.AddWithValue("@Email", email);

                int count = Convert.ToInt32(selectCommand.ExecuteScalar());
                if (count > 0)
                {
                    return "Sorry, Email already exists.";
                }
            }

            try
            {
                var mailAddress = new MailAddress(email);
                return string.Empty;
            }
            catch (FormatException)
            {
                return "Invalid email format.";
            }
        }

        private string ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Password field is required.";
            }

            if (password.Length < 8)
            {
                return "Password should be at least 8 characters long.";
            }

            if (!password.Any(char.IsUpper))
            {
                return "Password should contain at least one uppercase character.";
            }

            if (!password.Any(char.IsDigit))
            {
                return "Password should contain at least one digit.";
            }

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                return "Password should contain at least one special character.";
            }

            return string.Empty;
        }

        private string ValidateConfirmPassword(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                return "Confirm password field is required.";
            }

            if (password != confirmPassword)
            {
                return "Passwords do not match.";
            }

            return string.Empty;
        }

        private void register()
        {
            string username = txtUsername.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            string usernameError = ValidateUsername(username);
            string emailError = ValidateEmail(email);
            string passwordError = ValidatePassword(password);
            string confirmPasswordError = ValidateConfirmPassword(password, confirmPassword);

            if (string.IsNullOrEmpty(usernameError) &&
                string.IsNullOrEmpty(emailError) &&
                string.IsNullOrEmpty(passwordError) &&
                string.IsNullOrEmpty(confirmPasswordError))
            {
                // Insert user's information into the database
                string insertQuery = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection))
                {
                    insertCommand.Parameters.AddWithValue("@Username", username);
                    insertCommand.Parameters.AddWithValue("@Email", email);
                    insertCommand.Parameters.AddWithValue("@Password", password);

                    // Execute the insert query
                    insertCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Registration successful!");
                // Redirect the user to another form (user main page)
                Form1 login = new Form1();
                login.Show();
                this.Hide();
            }
            else
            {
                // Determine the error message with the highest priority
                string errorMessage = string.Empty;
                if (!string.IsNullOrEmpty(usernameError))
                {
                    errorMessage = usernameError;
                }
                else if (!string.IsNullOrEmpty(emailError))
                {
                    errorMessage = emailError;
                }
                else if (!string.IsNullOrEmpty(passwordError))
                {
                    errorMessage = passwordError;
                }
                else 
                {
                    errorMessage = confirmPasswordError;
                }

                MessageBox.Show(errorMessage);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            register();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            // Redirect the user to login form
            Form1 login = new Form1();
            login.Show();
            this.Hide();
        }
    }
}
