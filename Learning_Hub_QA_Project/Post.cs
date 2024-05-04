using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace Learning_Hub_QA_Project
{
    public partial class Post : Form
    {
        private WaveInEvent waveIn;
        private WaveFileWriter waveWriter;
        private string outputFilePath;
        public string categoryName;
        private string connectionString = "Data Source=LAPTOP-QF370PIT;Initial Catalog=LearningHubDB;Integrated Security=True";
        public Post()
        {
            InitializeComponent();
        }

        private void Post_Load(object sender, EventArgs e)
        {
            textBoxArticle.Visible = true;
            submitArticle.Visible = true;
            buttonUpload.Visible = false;
            recordButton.Visible = false;
            stopButton.Visible = false;
            labelStatus.Visible = false;

            comboBoxOptions.SelectedIndex = 0;
            categories.SelectedIndex = 0;
            categoryName = categories.SelectedItem.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedOption = comboBoxOptions.SelectedItem.ToString();

            // Show/hide controls in the GroupBox based on the selected option
            if (selectedOption == "Add Article")
            {
                textBoxArticle.Visible = true;
                submitArticle.Visible = true;
                buttonUpload.Visible = false;
                recordButton.Visible = false;
                stopButton.Visible = false;
                labelStatus.Visible = false;
            }
            else if (selectedOption == "Upload Video")
            {
                textBoxArticle.Visible = false;
                submitArticle.Visible = false;
                buttonUpload.Visible = true;
                recordButton.Visible = false;
                stopButton.Visible = false;
                labelStatus.Visible = false;
            }
            else if (selectedOption == "Record Voice")
            {
                textBoxArticle.Visible = false;
                submitArticle.Visible = false;
                buttonUpload.Visible = false;
                recordButton.Visible = true;
                stopButton.Visible = true;
                labelStatus.Visible = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            outputFilePath = "output.wav";  // Set the output file path for the recorded audio
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(waveIn.DeviceNumber).Channels);
            waveWriter = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.StartRecording();
            labelStatus.Text = "                            Recording...                             ";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            waveIn.StopRecording();
            // Clean up resources
            waveIn.Dispose();
            waveWriter.Dispose();
            labelStatus.Text = "     Recording stopped and submitted for approval!";

            StoreRecordInDatabase(outputFilePath);
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            waveWriter.Write(e.Buffer, 0, e.BytesRecorded); // Write the audio data to the WaveFileWriter
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files (*.mp4, *.avi)|*.mp4;*.avi";
            openFileDialog.Title = "Select a Video File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string videoFilePath = openFileDialog.FileName;

                StoreVideoInDatabase(videoFilePath);

                string videoFileName = Path.GetFileName(videoFilePath);
                MessageBox.Show($"Video file {videoFileName} uploaded successfully and Waiting for approval!");
            }
        }

        private void submitArticle_Click(object sender, EventArgs e)
        {
            string articleText = textBoxArticle.Text.Trim();

            if (string.IsNullOrWhiteSpace(articleText))
            {
                MessageBox.Show("Article text cannot be empty.");
                return;
            }

            StoreArticleInDatabase(articleText);

            textBoxArticle.Text = string.Empty;
            MessageBox.Show("Article submitted successfully and Waiting for approval!");
            textBoxArticle.Focus();
        }

        private void categories_SelectedIndexChanged(object sender, EventArgs e)
        {
            categoryName = categories.SelectedItem.ToString();
        }

        private void StoreArticleInDatabase(string articleText)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO " + categoryName + " (Articles) VALUES (@ArticleText)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@ArticleText", SqlDbType.NVarChar).Value = articleText;

                    command.ExecuteNonQuery();
                }
            }
        }

        private void StoreVideoInDatabase(string videoFilePath)
        {
            byte[] videoData = File.ReadAllBytes(videoFilePath);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO " + categoryName + " (Videos) VALUES (@VideoData)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@VideoData", SqlDbType.VarBinary).Value = videoData;

                    command.ExecuteNonQuery();
                }
            }
        }

        private void StoreRecordInDatabase(string recordFilePath)
        {
            byte[] recordData = File.ReadAllBytes(recordFilePath);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO " + categoryName + " (Audios) VALUES (@AudioData)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@AudioData", SqlDbType.VarBinary).Value = recordData;

                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
