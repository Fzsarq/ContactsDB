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
            //Get data from table and fill dg1. If table is empty, a message box will pop up.
            retrieveTData();
            dg1.Click += dg1_Click;
            clearText();
            control("i");
            txtSearch.KeyPress += txtSearch_KeyPress;
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
            string connStr = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=C:\\Users\\Zukaro\\Documents\\GitHub\\ContactsDB\\ContactOrganizer\\Clients.mdf;Integrated Security=True"; //TO-DO: make the connection string relative, so that it will work on any machine
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

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This method searches the database as you type.  It is designed such that it will show you all records which contain what you've typed.
            //For example, typing the letter "e" will show all records which contain the letter "e".  This is case insensitive, so will return both lower and upper case.

            //TO-DO:
            //1. Disable context menu (so no one can copy and paste)
            //2. If the field is highlighted and all characters deleted, it wont instantly revert to the retrieveTData() method the way it does when no records are found
            //3. When the table is empty, search functionality needs to be greyed out; otherwise the user will consistently get the "Table is empty" message on every keypress


            char c = e.KeyChar;
            String searchSQL = "SELECT * FROM [tContacts] WHERE [lname] LIKE @search OR [fname] LIKE @search OR [phone] LIKE @search OR [email] LIKE @search";
            SqlDataReader readSearch = null;
            sqlCmd = new SqlCommand();
            StringBuilder buildSearch = new StringBuilder();
            String searchFor = "";

            if (c == 8)
            {
                searchFor = txtSearch.Text;
                buildSearch.Append(searchFor);
                if (buildSearch.Length > 0)
                {
                    buildSearch.Remove(buildSearch.Length - 1, 1);
                }
                searchFor = buildSearch.ToString();
            }
            else
            {
                searchFor = txtSearch.Text + c;
            }

            try
            {
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = searchSQL;
                sqlCmd.Parameters.Add("@search", System.Data.SqlDbType.VarChar, 15).Value = "%" + searchFor + "%";
                readSearch = sqlCmd.ExecuteReader();
                if (readSearch.HasRows)
                {
                    bindingSource1.DataSource = readSearch;
                    dg1.DataSource = bindingSource1;
                    dg1.ClearSelection();
                }
                else
                {
                    retrieveTData();
                }
                readSearch.Close();
                sqlConn.Close();
            }
            catch (SqlException ex)
            {
                if (readSearch != null)
                {
                    readSearch.Close();
                }
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
                MessageBox.Show(ex.Message, "SQL Error");//Debug message
            }
        }
    }
}
