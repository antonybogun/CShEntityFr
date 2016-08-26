using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Learner;
using System.Data.Entity;
using System.Runtime.InteropServices;

namespace AntonBogunM16_Lab3_Ex2
{
    public partial class Form1 : Form
    {
        private bool IsFound;
        public Form1()
        {
            InitializeComponent();
            IsFound = false;
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                TonyDBEntities dbcontext = new TonyDBEntities();
                // load Authors table ordered by LastNamethen FirstName
                dbcontext.TonyTBs
                .OrderBy(TonyTB => TonyTB.learnerId)
                .ThenBy(TonyTB => TonyTB.learnerName)
                .Load();
                // specify DataSourcefor authorBindingSource
                tonyTBBindingSource.DataSource = dbcontext.TonyTBs.Local;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab.Text == "Edit View")
                {
                    bindingNavigatorAddNewItem.Enabled = false;
                    bindingNavigatorDeleteItem.Enabled = false;
                    bindingNavigatorMoveFirstItem.Enabled = false;
                    bindingNavigatorMoveLastItem.Enabled = false;
                    bindingNavigatorMoveNextItem.Enabled = false;
                    bindingNavigatorMovePreviousItem.Enabled = false;
                    bindingNavigatorCountItem.Enabled = false;
                    bindingNavigatorPositionItem.Enabled = false;
                }
                else if (tabControl1.SelectedTab.Text == "Table View")
                {
                    bindingNavigatorAddNewItem.Enabled = true;
                    bindingNavigatorDeleteItem.Enabled = true;
                    bindingNavigatorMoveFirstItem.Enabled = true;
                    bindingNavigatorMoveLastItem.Enabled = true;
                    bindingNavigatorMoveNextItem.Enabled = true;
                    bindingNavigatorMovePreviousItem.Enabled = true;
                    bindingNavigatorCountItem.Enabled = true;
                    bindingNavigatorPositionItem.Enabled = true;

                    TonyDBEntities dbcontext = new TonyDBEntities();
                    dbcontext.TonyTBs
                    .OrderBy(TonyTB => TonyTB.learnerId)
                    .ThenBy(TonyTB => TonyTB.learnerName)
                    .Load();
                    // specify DataSourcefor authorBindingSource
                    tonyTBBindingSource.DataSource = dbcontext.TonyTBs.Local;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dbContext = new TonyDBEntities())
                {
                    var query = from a in dbContext.TonyTBs

                                where a.learnerId.ToString() == txtLearnerId.Text
                                select new
                                {
                                    learnerId = a.learnerId,
                                    learnerName = a.learnerName,
                                    favouriteSubject = a.favouriteSubject,
                                    strongestSkill = a.strongestSkill,
                                    numberOfLanguages = a.numberOfLanguages
                                };
                    if (query.Any())
                    {
                        foreach (var item in query)
                        {
                            txtLearnerId.Text = item.learnerId.ToString();
                            txtLearnerName.Text = item.learnerName;
                            txtFavouriteSubject.Text = item.favouriteSubject;
                            txtStrongestSkill.Text = item.strongestSkill;
                            txtNumberOfLanguages.Text = item.numberOfLanguages.ToString();
                        }
                        IsFound = true;
                    }
                    else
                    {
                        var temp = txtLearnerId.Text;
                        ClearFields();
                        txtLearnerId.Text = temp;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsFound)
                {
                    TonyTB learner;

                    //Get customer for the given customerId
                    using (var dbContext = new TonyDBEntities())
                    {
                        learner = dbContext.TonyTBs.Where(s => s.learnerId.ToString() == txtLearnerId.Text).FirstOrDefault<TonyTB>();
                    }
                    // Update customer info in disconnected mode (out of dbContext scope)
                    if (learner != null)
                    {
                        learner.learnerName = txtLearnerName.Text;
                        learner.favouriteSubject = txtFavouriteSubject.Text;
                        learner.strongestSkill = txtStrongestSkill.Text;
                        learner.numberOfLanguages = Int32.Parse(txtNumberOfLanguages.Text);
                    }

                    // Save modified entity using new Context
                    using (var dbContext = new TonyDBEntities())
                    {
                        // Mark entity as modified
                        dbContext.Entry(learner).State = EntityState.Modified;

                        // Save changes to database
                        dbContext.SaveChanges();
                    }
                    MessageBox.Show(@"Entry Updated Successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtLearnerId.Text != "")
                {
                    using (var dbContext = new TonyDBEntities())
                    {
                        //create an instance of the entity object
                        TonyTB learner = new TonyTB
                        {
                            learnerId = Int32.Parse(txtLearnerId.Text),
                            learnerName = txtLearnerName.Text,
                            favouriteSubject = txtFavouriteSubject.Text,
                            strongestSkill = txtStrongestSkill.Text,
                            numberOfLanguages = Int32.Parse(txtNumberOfLanguages.Text)
                        };
                        //add the entity to the data context
                        //dbContext.Authors.Add(author);
                        dbContext.TonyTBs.Attach(learner);
                        dbContext.Entry(learner).State = EntityState.Added;
                        // save the data context - it will also save the data to the database
                        dbContext.SaveChanges();
                        MessageBox.Show("Entry Inserted Succesfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsFound)
                {
                    TonyTB learnerToDelete;
                    //Get student for the given studentId
                    using (var dbContext = new TonyDBEntities())
                    {
                        learnerToDelete = dbContext.TonyTBs.Where(s => s.learnerId.ToString() == txtLearnerId.Text).FirstOrDefault<TonyTB>();
                    }

                    //Create new context for disconnected scenario
                    using (var newContext = new TonyDBEntities())
                    {
                        newContext.Entry(learnerToDelete).State = EntityState.Deleted;
                        newContext.SaveChanges();
                    }
                    MessageBox.Show("Entry Deleted Succesfully!");
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void ClearFields()
        {
            txtLearnerId.Text =
                txtLearnerName.Text = txtFavouriteSubject.Text = txtNumberOfLanguages.Text = txtStrongestSkill.Text = "";
        }
    }
}
