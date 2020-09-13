using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Forex.Forms
{
    public partial class ModelCreateForm : Form
    {
        private Models m_models;



        # region Constructors

        public ModelCreateForm()
        { 
            InitializeComponent();
        }

        public ModelCreateForm(Models CurrentModelsList) : this()
        {
            m_models = CurrentModelsList;

            ShowList(); 
        }

        # endregion Constructors



        # region Events

        private void buttonSave_Click(object sender, EventArgs e)
        {
            bool Validated = Validation();
            bool FlagAddModel = true;

            if (Validated)
            {
                switch (tbModelClass.Text)
                {
                    // @ HERE GL add new implemented Model Classes

                    case "EMAShort_EMALong_Filter_CCI":

                        bool ModelExist = m_models.ModelExist(tbModelName.Text);

                        if (ModelExist) // Model Exist, remove and then replace it
                        {
                            if (!m_models.GetModelByName(tbModelName.Text).IsRunning)
                            {
                                m_models.RemoveModel(m_models.GetModelByName(tbModelName.Text));
                            }
                            else // Model is running; you cannot modify a running model
                            {
                                FlagAddModel = false;
                                DialogResult dr = MessageBox.Show("Model is running; you cannot modify a running model", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }

                        if (FlagAddModel)
                        {
                            EMAShort_EMALong_Filter_CCI SaveModel = new EMAShort_EMALong_Filter_CCI(this);

                            m_models.AddModel(SaveModel);

                            m_models.SaveModelsList();
                        }

                        break;

                    // new model class
                    // case "NewModelClass":
                        // do...
                        // break;
                }

                ShowList();
            }
            else
            {
                MessageBox.Show("Some fields are not filled; please check", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (cbListOfModels.Items.Count > 0 && cbListOfModels.Text != "")
            {
                DialogResult dr = MessageBox.Show("Are you sure you want delete?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (dr == DialogResult.Yes)
                {
                    Model ModelToDelete = m_models.GetModelByName(cbListOfModels.Text);
                    if (!ModelToDelete.IsRunning)
                    {
                        m_models.RemoveModel(m_models.GetModelByName(cbListOfModels.Text));
                        m_models.SaveModelsList();

                        ShowList();
                    }
                    else // Model is running; you cannot delete a running model
                    {
                        dr = MessageBox.Show("Model is running; you cannot delete a running model", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void cbListOfModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ModelClass = m_models.GetModelClassByName(cbListOfModels.Text);

            switch (ModelClass)
            {
                // HERE GL add new implemented Model Classes
                case "EMAShort_EMALong_Filter_CCI":

                    EMAShort_EMALong_Filter_CCI CurrentModel = (EMAShort_EMALong_Filter_CCI)m_models.GetModelByName(cbListOfModels.Text);

                    tbModelName.Text = CurrentModel.ModelName;

                    tbModelClass.Text = CurrentModel.ModelClass;

                    cbType.Text = CurrentModel.CrossType;

                    cbInstrument.Text = CurrentModel.Instrument;

                    tbMinutes.Text = Convert.ToString(CurrentModel.Minutes);
                    tbTimeFrame.Text = CurrentModel.TimeFrame;
            
                    tbAmount.Text = Convert.ToString(CurrentModel.Amount);
            
                    tbTakeProfit.Text = Convert.ToString(CurrentModel.TakeProfitValue);
            
                    tbStopLoss.Text = Convert.ToString(CurrentModel.StopLossValue);

                    tbStartWeekendMinute.Text = Convert.ToString(CurrentModel.WeekeendStartMinute);
                    tbStartWeekendHour.Text = Convert.ToString(CurrentModel.WeekeendStartETHour);
                    tbStartWeekendDay.Text = Convert.ToString(CurrentModel.WeekendStartDayOfWeek);

                    tbEndWeekendMinute.Text = Convert.ToString(CurrentModel.WeekeendEndMinute);
                    tbEndWeekendHour.Text = Convert.ToString(CurrentModel.WeekeendEndETHour);
                    tbEndWeekendDay.Text = Convert.ToString(CurrentModel.WeekendEndDayOfWeek);

                    tbCCIDays.Text = Convert.ToString(CurrentModel.CCIDays);
                    tbema40days.Text = Convert.ToString(CurrentModel.EMA40Days);
                    tbema80days.Text = Convert.ToString(CurrentModel.EMA80Days);

                    tbCCICond1.Text = Convert.ToString(CurrentModel.CCICond1);
                    tbCCICond2.Text = Convert.ToString(CurrentModel.CCICond2);
                    tbCCIVar.Text = Convert.ToString(CurrentModel.CCIVar);
                    tbCCIVal1.Text = Convert.ToString(CurrentModel.CCIVal1);
                    tbCCIVal2.Text = Convert.ToString(CurrentModel.CCIVal2);

                    break;

                // a different model class
                // case "MyModelClass":
                //    MyModelClass CurrentModel = m_models.GetModelByName(cbListOfModels.Text);
                // break;
            }
        }

        # endregion Events



        # region Methods

        private void ShowList()
        {
            cbListOfModels.Items.Clear();

            foreach (Model Current in m_models.ModelsList)
            {
                cbListOfModels.Items.Add(Current.ModelName);
            }

            if (m_models.ModelsList.Count > 0)
            {
                cbListOfModels.SelectedIndex = 0;
            }
        }

        private bool Validation()
        {
            bool Complete = true;

            foreach (Control C in this.Controls)
            {
                if (C.GetType() == typeof(System.Windows.Forms.TextBox) || 
                    C.GetType() == typeof(System.Windows.Forms.ComboBox))
                {
                    if(C.Text == "" && C.Name != "cbListOfModels")
                    {
                        Complete = false;
                    }
                }
            }
            return Complete;
        }

        # endregion Methods
    }
}
