using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using fxcore2;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Xml;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace Forex.Forms
{
    public partial class ModelsForm : Form
    {
        private string m_selectedmodelname = null;
        private Session m_currentsession;
        private System.ComponentModel.BackgroundWorker m_backgroundworker;



        public string SelectedModelName { get { return m_selectedmodelname; } set { m_selectedmodelname = value; } }



        public ModelsForm()
        {
            InitializeComponent();
        }

        public ModelsForm(Session CurrentSession) : this()
        {
            m_currentsession = CurrentSession;
            DisplayModels(m_currentsession.Models);
            SetGridEvents();
            this.Show();
        }

        private void buttonCreateModel_Click(object sender, EventArgs e)
        {
            ModelCreateForm CurrentCreateForm = new ModelCreateForm(m_currentsession.Models);
                
            CurrentCreateForm.ShowDialog();

            DisplayModels(m_currentsession.Models);
        }

        private void DisplayModels(Models ModelsToDisplay)
        {
            // Clear the Grid before filling it
            dataGridView.DataSource = null;
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            dataGridView.DataSource = ModelsToDisplay.ModelsList;
            dataGridView.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Hide all properties except ModelName and Status
            foreach (DataGridViewColumn CurrentColumn in dataGridView.Columns)
            {
                if (CurrentColumn.Name != "ModelName" & CurrentColumn.Name != "Status")
                {
                    CurrentColumn.Visible = false;
                }
            }

            dataGridView.Columns[0].Width = 150;

            AddStatusColumn();
            SetStatus();
        }

        private void AddStatusColumn()
        { 
            DataGridViewButtonColumn EditColumn = new DataGridViewButtonColumn();
            EditColumn.Name = "Status";
            EditColumn.DataPropertyName = "Status";
            EditColumn.Text = "Status";
            EditColumn.UseColumnTextForButtonValue = true;
            EditColumn.Width = 150;

            dataGridView.Columns.Add(EditColumn);
        }

        private void SetGridEvents()
        {
            dataGridView.CellClick += new DataGridViewCellEventHandler(dataGridView_CellClick);        
        }

        private void SetStatus()
        {
            DataGridViewButtonCell dgbtn = null;
            string CurrentRowModelName = null;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                CurrentRowModelName = row.Cells["ModelName"].Value.ToString();

                dgbtn = (DataGridViewButtonCell)(row.Cells["Status"]);

                dgbtn.UseColumnTextForButtonValue = false;
                dataGridView.CurrentCell = row.Cells["Status"];

                dataGridView.CurrentCell.ReadOnly = false;

                Model CurrentRowModel = (Model)m_currentsession.Models.GetModelByName(CurrentRowModelName);
                if (CurrentRowModel.IsRunning)
                    dgbtn.Value = "Running";
                else
                    dgbtn.Value = "Status";

                dataGridView.CurrentCell.ReadOnly = true;
            }            
        }

        private void dataGridView_CellClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            DataGridViewButtonCell dgbtn = null;

            if (dataGridView.Columns[e.ColumnIndex].Name == "Status")
            {
                dgbtn = (DataGridViewButtonCell)(dataGridView.Rows[e.RowIndex].Cells["Status"]);

                if (dgbtn.Value.ToString() == "Status")
                {
                    dgbtn.UseColumnTextForButtonValue = false;
                    dataGridView.CurrentCell = dataGridView.Rows[e.RowIndex].Cells["Status"];
                    dataGridView.CurrentCell.ReadOnly = false;

                    dgbtn.Value = "Running";
                    dataGridView.CurrentCell.ReadOnly = true;

                    DataGridViewRow selectedRow = dataGridView.Rows[e.RowIndex];

                    SelectedModelName = selectedRow.Cells["ModelName"].Value.ToString();

                    m_backgroundworker = new BackgroundWorker();

                    InitializeBackgroundWorker();
                    m_backgroundworker.WorkerReportsProgress = true;
                    m_backgroundworker.WorkerSupportsCancellation = true;
                    m_backgroundworker.RunWorkerAsync();
                }
                else if (dgbtn.Value.ToString() == "Running")
                { 
                    // stop model
                    DataGridViewRow selectedRow = dataGridView.Rows[e.RowIndex];

                    SelectedModelName = selectedRow.Cells["ModelName"].Value.ToString();

                    m_currentsession.StopModel(SelectedModelName, dgbtn);
                }
            }
        }



        #region Backgroundworker

        private void InitializeBackgroundWorker()
        {
            m_backgroundworker.DoWork +=
                new DoWorkEventHandler(backgroundWorker_DoWork);

            m_backgroundworker.ProgressChanged +=
               new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);

            m_backgroundworker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;            
            m_currentsession.StartModel(SelectedModelName, worker, e);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            labelLogDetails.Text = labelLogDetails.Text + e.UserState as string + "\r\n";
            labelLogDetails.SelectionStart = labelLogDetails.Text.Length;
            labelLogDetails.ScrollToCaret();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }
        }

        #endregion Backgroundworker
    }
}
