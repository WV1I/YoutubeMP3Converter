using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;

namespace Mp3_Converter
{
    public partial class Form1 : Form
    {

        char[] invalidFileChars = Path.GetInvalidFileNameChars();
        string path;
        public Form1()
        {
            InitializeComponent();

        }

        private  void konvert(string fileName, string directory)
        {
            var type = types.SelectedItem;
            if (type == "mp4") return;

            var Convert = new NReco.VideoConverter.FFMpegConverter();
            string fullname = directory + fileName;
            String SaveMP3File = fullname.Replace(".mp4", $".{type}");
            Convert.ConvertMedia(fullname, SaveMP3File, $"{type}");
            //Delete the MP4 file after conversion
            File.Delete(fullname);
        }



        private async void Download(string SaveToFolder, string VideoURL, char [] charArray)
        {
            label1.Text = "Download Started";

            var source = @SaveToFolder += "/";
            var youtube = YouTube.Default;
            var video = youtube.GetVideo(VideoURL);
            var client = new HttpClient();

            long? totalByte = 0;

            string fileExtension = video.FileExtension;

            var fileName = $"{video.Title}{fileExtension}";
            foreach (char x in charArray)
            {
                if (fileName.Contains(x))
                {
                    // Sprawdzanie czy nazwa nie zawiera niewłaściwych znaków
                    fileName = fileName.Replace(x.ToString(), "");

                }
            }
            label1.Text = "Downloading...";
            using (Stream output = File.OpenWrite(source + fileName))
            {
                using (var request = new HttpRequestMessage(HttpMethod.Head, video.Uri))
                {
                    totalByte = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result.Content.Headers.ContentLength;
                }
                using (var input = await client.GetStreamAsync(video.Uri))
                {
                    byte[] buffer = new byte[16 * 1024];
                    int read;
                    int totalRead = 0;
                    int size = Convert.ToInt32(totalByte);
                    progressBar1.Maximum = size;
                    
                    
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                        totalRead += read;
                        progressBar1.Value = totalRead;
                        
                    }

                    
                }
                
            }



            konvert(fileName, source);
            label1.Text = "Download Complete";

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void downbutton_Click(object sender, EventArgs e)
        {
            if (path == "")
            {
                patherror.Visible = true;
                return;
            }
            else
                patherror.Visible = false;
            Download(path, textBox1.Text, invalidFileChars);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Contains("www.youtube.com"))
            {

            }
        }
    }
}
