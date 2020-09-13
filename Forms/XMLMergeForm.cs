using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace Forex.Forms
{
    public partial class XMLMergeForm : Form
    {
        public Models m_models = null;

        public Session CurrentSession = null;

        public List<Tuple<Models,string>> ModelsArray = new List<Tuple<Models,string>>();

        public List<Model> ModelList = new List<Model>();

        public XMLMergeForm()
        {
            InitializeComponent();
        }

        public XMLMergeForm(Session m_session, Models models)
        {
            InitializeComponent();
            m_models = models;
            CurrentSession = m_session;

            ReadModel1();
            ReadOtherModels();
            FillComboBox();
            this.Show();
        }

        public void FillComboBox()
        {
            foreach (Tuple<Models,string> CurrentModels in ModelsArray)
            {
               foreach(Model CurrentModel in CurrentModels.Item1.ModelsList)
               {
                   ccbModels.Items.Add(CurrentModel.ModelName + "," + CurrentModels.Item2);
               }
            }
        }

        public void ReadModel1()
        {
            m_models = Models.LoadModelsList(m_models);
            Tuple<Models,string> CurrentTuple = new Tuple<Models,string>(m_models,"Current");

            ModelsArray.Add(CurrentTuple);
        }

        public void ReadOtherModels()
        {
            string[] files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");

            foreach(string Current in files)
            {
                if (Current != AppDomain.CurrentDomain.BaseDirectory + "XMLModels.xml")
                {
                    string FileName = Path.GetFileName(Current);

                    m_models = LoadModelsList(m_models, Current);
                    Tuple<Models, string> CurrentTuple = new Tuple<Models, string>(m_models, FileName);

                    ModelsArray.Add(CurrentTuple);
                }
            }
        }


        public Models LoadModelsList(Models Current, string file)
        {
            try
            {
                // Deserialize
                FileStream fs = new FileStream(file, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(Models), Current.ModelTypes);
                Current = (Models)serializer.Deserialize(fs);
                fs.Close();
                return Current;
            }
            catch (Exception e)
            {
                LogDirector.DoAction(4, e);
                return Current;
            }
        }

        private void buttonMerge_Click(object sender, EventArgs e)
        {
            DialogResult dlg = MessageBox.Show("This will overwrite the original XML Models file, Are you sure?",
                "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dlg == DialogResult.Yes)
            {
                Merge();
                Export();
                MessageBox.Show("Merge Complete.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void Merge()
        {
            foreach (string CurrentItem in ccbModels.CheckedItems)
            {
                string[] split = CurrentItem.Split(',');

                foreach (Tuple<Models, string> CurrentModels in ModelsArray)
                {
                    foreach (Model CurrentModel in CurrentModels.Item1.ModelsList)
                    {
                        if (CurrentModel.ModelName == split[0] && CurrentModels.Item2 == split[1])
                        {
                            foreach (Model ListModel in ModelList)
                            {
                                if (ListModel.ModelName == CurrentModel.ModelName)
                                {
                                    CurrentModel.ModelName = CurrentModel.ModelName + "(2)";
                                }
                            }

                            ModelList.Add(CurrentModel);
                            break;
                        }
                    }
                }
            }
        }

        public void Export()
        {
            try
            {
                m_models.ModelsList = ModelList;

                m_models.SaveModelsList();
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
