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

namespace ContactOrganizer
{
    //We are using the ADO.NET Connected Model(DataReader) as opposed to the Disconnected Model(DataSet). Easier to manage when Inserting, updating and deleting data.
    public partial class Form1 : Form
    {
        private SqlConnection sqlConn = null;
        private SqlCommand sqlCmd = null;
        private string origLName = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //FARAZ: in design mode corrected Length property of txtboxes

            //input validation
            txtLName.KeyPress += txtLName_KeyPress;
            txtFName.KeyPress += txtFName_KeyPress;
            txtPhone.KeyPress += txtPhone_KeyPress;
            txtEmail.KeyPress += txtEmail_KeyPress;

            //disable right click menu operations
            txtLName.ContextMenu = new System.Windows.Forms.ContextMenu();
            txtFName.ContextMenu = new System.Windows.Forms.ContextMenu();
            txtPhone.ContextMenu = new System.Windows.Forms.ContextMenu();
            txtEmail.ContextMenu = new System.Windows.Forms.ContextMenu();

            //Get data from table and fill dg1. If table is empty, a message box will pop up.
            retrieveTData();
            dg1.Click += dg1_Click;
            clearText();
            control("i");
        }

        void txtEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            int c = e.KeyChar;
            int len = ((TextBox)sender).Text.Length;
            ((TextBox)sender).SelectionStart = len;
            
            //backspace
            if (c == 8)
            {
                return;
            }

            //last char in the textfield (default to -1 if does not exist)
            int lastChar = -1;
            if (len > 0)
            {
                lastChar = ((TextBox)sender).Text[len - 1];
            }

            //ampersand '@' ASCII:64
            if (c == 64)
            {
                //no repeated ampersands
                if (((TextBox)sender).Text.IndexOf('@') >= 0)
                {
                    e.Handled = true;
                }
                //no initial ampersand
                if (len == 0)
                {
                    e.Handled = true;
                }

                //minimum domain name length
                //?

                return;
            }
                
            //numeric
            //ASCII:48-57
            if (c >= 48 && c <= 57)
            {
                return;
            }

            //letters
            //ASCII:65-90
            if (c >= 65 && c <= 90)
            {
                return;
            }
            //ASCII:97-122
            if (c >= 97 && c <= 122)
            {
                return;
            }

            //dot '.' ASCII:46
            if (c == 46)
            {
                //no repeated dots
                if (lastChar == 46)
                {
                    e.Handled = true;
                }
                //no initial dot
                if (len == 0)
                {
                    e.Handled = true;
                }

                return;
            }            

            //special characters:
            //ASCII: 33, 35-39, 42, 43, 45, 47, 61, 63, 94-96, 123-126
            //NOTE: if even these are not detected the char is rejected
            switch (c)
            {
                case 33:
                    //
                    break;
                case 35:
                    //
                    break;
                case 36:
                    //
                    break;
                case 37:
                    //
                    break;
                case 38:
                    //
                    break;
                case 39:
                    //
                    break;
                case 42:
                    //
                    break;
                case 43:
                    //
                    break;
                case 45:
                    //
                    break;
                case 47:
                    //
                    break;
                case 61:
                    //
                    break;
                case 63:
                    //
                    break;
                case 94:
                    //
                    break;
                case 95:
                    //
                    break;
                case 96:
                    //
                    break;
                case 123:
                    //
                    break;
                case 124:
                    //
                    break;
                case 126:
                    //
                    break;
                default:
                    e.Handled = true;
                    break;
            }

        }

        void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            int c = e.KeyChar;
            int len = ((TextBox)sender).Text.Length;
            ((TextBox)sender).SelectionStart = len;
            if (c == 8)
            {
                return;
            }
            if (len == 3 || len == 7)
            {
                //dash only
                if (c != 45)
                {
                    //kill char
                    e.Handled = true;
                }
            }
            else if (len == 12)
            {
                //max length
                e.Handled = true;
            }
            else
            {
                //number only
                if (c < 48 || c > 57)
                {
                    e.Handled = true;
                }
            }

        }

        void txtFName_KeyPress(object sender, KeyPressEventArgs e)
        {
            int c = e.KeyChar;
            int len = ((TextBox)sender).Text.Length;
            ((TextBox)sender).SelectionStart = len;
            if (c == 8)
            {
                return;
            }
            if (len == 0)
            {
                if (c >= 97 && c <= 122)
                {
                    //first char lower case
                    e.KeyChar = (char)(c - 32);
                    return;
                }
                if (c < 65 || c > 90)
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (c >= 65 && c <= 90)
                {
                    e.KeyChar = (char)(c + 32);
                    return;
                }
                if (c < 97 || c > 122)
                {
                    e.Handled = true;
                }
            }

        }

        void txtLName_KeyPress(object sender, KeyPressEventArgs e)
        {
            int c = e.KeyChar;
            int len = ((TextBox)sender).Text.Length;
            ((TextBox)sender).SelectionStart = len;
            if (c == 8)
            {
                return;
            }
            if (len == 0)
            {
                if (c >= 97 && c <= 122)
                {
                    //first char lower case
                    e.KeyChar = (char)(c - 32);
                    return;
                }
                if (c < 65 || c > 90)
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (c >= 65 && c <= 90)
                {
                    e.KeyChar = (char)(c + 32);
                    return;
                }
                if (c < 97 || c > 122)
                {
                    e.Handled = true;
                }
            }
        }

        void dg1_Click(object sender, EventArgs e)
        {
            dg1.CurrentRow.Selected = true;
            origLName = dg1.CurrentRow.Cells[1].Value.ToString();
            txtLName.Text = dg1.CurrentRow.Cells[1].Value.ToString();
            txtFName.Text = dg1.CurrentRow.Cells[2].Value.ToString();
            txtPhone.Text = dg1.CurrentRow.Cells[3].Value.ToString();
            txtEmail.Text = dg1.CurrentRow.Cells[4].Value.ToString();
            control("c");
        }


        //This method will be called on every button click. (Will rebind the data to dg1)
        //Gets data from the database and fills the dg1 with said data
        private void retrieveTData()
        {
            string connStr = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=C:\\PROG37721\\ContactOrganizer\\ContactOrganizer\\Clients.mdf;Integrated Security=True";
            string select = "SELECT * FROM [tContacts]";

            try
            {
                sqlConn = new SqlConnection(connStr);
                sqlConn.Open();
                sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = select;
                SqlDataReader sDr = sqlCmd.ExecuteReader();

                if (sDr.HasRows)
                {
                    bindingSource1.DataSource = sDr;
                    dg1.DataSource = bindingSource1;
                    dg1.ClearSelection();
                    lblCount.Text = "Number of Contacts: " + dg1.Rows.Count.ToString();
                }
                    
                else
                {
                    lblCount.Text = "Number of Contacts: 0";
                    MessageBox.Show("Table is empty", "Read Result");
                }

                sDr.Close();
                sqlConn.Close();
            }
            catch (SqlException ex)
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
                MessageBox.Show(ex.Message, "An error occurred while reading the Database");
            }
        }

        private void cmdInsert_Click(object sender, EventArgs e)
        {
            if (validInput())
            {
                if (validFullName("i"))
                {
                    string insert = "INSERT INTO [tContacts] ([lname], [fname], [phone], [email]) VALUES ('" + txtLName.Text + "','" + txtFName.Text + "','" + txtPhone.Text + "','" + txtEmail.Text + "')";

                    try
                    {
                        sqlConn.Open();
                        sqlCmd.CommandText = insert;
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();
                        retrieveTData();
                        clearText();
                    }
                    catch (SqlException ex)
                    {
                        if (sqlConn != null)
                        {
                            sqlConn.Close();
                        }
                        MessageBox.Show(ex.Message, "Error Inserting Record");
                    }
                }
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (validInput())
            {
                if (validFullName("u"))
                {
                    string update = 
                        "UPDATE [tContacts] SET [lname] = '" + txtLName.Text 
                        + "', [fname] = '" + txtFName.Text 
                        + "', [phone] = '" + txtPhone.Text 
                        + "', [email] = '" + txtEmail.Text
                        + "' WHERE [lname] = '" + origLName + "'";

                    try
                    {
                        sqlConn.Open();
                        sqlCmd.CommandText = update;
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();
                        retrieveTData();
                        clearText();
                    }
                    catch (SqlException ex)
                    {
                        if (sqlConn != null)
                        {
                            sqlConn.Close();
                        }
                        MessageBox.Show(ex.Message, "Error Updating Record");
                    }
                    control("i");
                }
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this record?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                string delete = "DELETE FROM [tContacts] WHERE [lname] = '" + origLName + "'";

                try
                {
                    sqlConn.Open();
                    sqlCmd.CommandText = delete;
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    //This removes the selected row if only one row remains in dg1. If there is only one row in the table, that row will persist after deletion because 
                    //if (dg1.Rows.Count < 2)
                    //{
                        dg1.Rows.RemoveAt(dg1.CurrentRow.Index);
                    //}
                    retrieveTData();
                    clearText();

                }
                catch (SqlException ex)
                {
                    if (sqlConn != null)
                    {
                        sqlConn.Close();
                    }

                    MessageBox.Show(ex.Message, "Error Deleting Record");
                }
            }
            control("i");
        }

        private bool validInput()
        {
            if (txtLName.Text.Length < 1)
            {
                MessageBox.Show("Last name field cannot be empty", "Missing Last Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtLName.Focus();
                return false;
            }

            if (txtLName.Text.Length < 2)
            {
                MessageBox.Show("Last name must have 2 or more characters", "Invalid Last Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtLName.Focus();
                return false;
            }

            if (txtFName.Text.Length < 1)
            {
                MessageBox.Show("First name field cannot be empty", "Missing First Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFName.Focus();
                return false;
            }

            if (txtFName.Text.Length < 2)
            {
                MessageBox.Show("First name must have 2 or more characters", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFName.Focus();
                return false;
            }

            //email validation
            //must be 3 chars long atleast
            if (txtEmail.Text.Length < 3)
            {
                MessageBox.Show("Email must have 3 or more characters", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return false;
            }
            //ampersand must exist and it cannot be the first or last char in textbox
            if (txtEmail.Text.IndexOf('@') < 1 || txtEmail.Text.IndexOf('@') == txtEmail.Text.Length - 1)
            {
                MessageBox.Show("Email must have legal ampersand placement", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return false;
            }
            
            //phone validation
            if (txtPhone.Text.Length < 12)
            {
                MessageBox.Show("Phone number must have 12 characters", "Invalid Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPhone.Focus();
                return false;
            }

            return true;
        }

        private bool validFullName(string state)
        {
            if (state.Equals("i"))
            {
                for (int i = 0; i < dg1.Rows.Count; i++)
                {
                    if (txtLName.Text.ToLower().Equals(dg1.Rows[i].Cells[1].Value.ToString().ToLower()) &&
                        txtFName.Text.ToLower().Equals(dg1.Rows[i].Cells[2].Value.ToString().ToLower()))
                    {
                        MessageBox.Show("This contact already exists. 2 contacts cannot share the same first and last name", "Entry Violation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtLName.Focus();
                        return false;
                    }
                }
            }
            else if (state.Equals("u"))
            {
                for (int i = 0; i < dg1.Rows.Count; i++)
                {
                    if (i != dg1.CurrentRow.Index)
                    {
                        if (txtLName.Text.ToLower().Equals(dg1.Rows[i].Cells[1].Value.ToString().ToLower()) &&
                            txtFName.Text.ToLower().Equals(dg1.Rows[i].Cells[2].Value.ToString().ToLower()))
                        {
                            MessageBox.Show("This contact already exists. 2 contacts cannot share the same first and last name", "Entry Violation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtLName.Focus();
                            return false;
                        }
                        if (txtEmail.Text.ToLower().Equals(dg1.Rows[i].Cells[4].Value.ToString().ToLower()))
                        {
                            MessageBox.Show("This email already exists. Multiple contacts cannot share the same email", "Entry Violation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtEmail.Focus();
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void clearText()
        {
            txtLName.Text = "";
            txtFName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtLName.Focus();
            dg1.ClearSelection();
        }

        private void control(String control)
        {
            if (control.Equals("i"))
            {
                cmdInsert.Enabled = true;
                cmdUpdate.Enabled = false;
                cmdDelete.Enabled = false;
            }
            else if (control.Equals("c"))
            {
                cmdInsert.Enabled = false;
                cmdUpdate.Enabled = true;
                cmdDelete.Enabled = true;
            }
        }
    }
}
