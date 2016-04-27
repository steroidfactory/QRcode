using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QRCoder;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Printing;

namespace QRCoderDemo
{
    public partial class Form1 : Form
    {
        public Dictionary<string, List<string>> printTemp = new Dictionary<string, List<string>>();
        public string text = "";
        public Image qrImage;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxECC.SelectedIndex = 0; //Pre-select ECC level "L"
            RenderQrCode();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            RenderQrCode();
            standardPrint();
        }

        private void RenderQrCode()
        {
            string level = comboBoxECC.SelectedItem.ToString();
            QRCodeGenerator.ECCLevel eccLevel = (QRCodeGenerator.ECCLevel)(level == "L" ? 0 : level == "M" ? 1 : level == "Q" ? 2 : 3);
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(textBoxQRCode.Text, eccLevel))
                {
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        qrImage = qrCode.GetGraphic(20, Color.Black, Color.White,
                            GetIconBitmap(), (int)iconSize.Value);
                        pictureBoxQRCode.BackgroundImage = qrImage;
                        
                        this.pictureBoxQRCode.Size = new System.Drawing.Size(pictureBoxQRCode.Width, pictureBoxQRCode.Height);
                        //Set the SizeMode to center the image.
                        this.pictureBoxQRCode.SizeMode = PictureBoxSizeMode.CenterImage;

                        pictureBoxQRCode.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
            text = "";
            string tempText = textBoxQRCode.Text;
            int counter = 0;
            foreach (char character in tempText)
            {
                text += character;
                counter++;
                if (counter == 20)
                {
                    text += Environment.NewLine;
                    counter = 0;
                }
            }
            labelText.Text = text;
        }
        
        private Bitmap GetIconBitmap()
        {
            Bitmap img = null;
            if (iconPath.Text.Length > 0)
            {
                try
                {
                    img = new Bitmap(iconPath.Text);
                }
                catch (Exception) 
                { 
                }
            }
            return img;
        }

        private void selectIconBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "Select icon";
            openFileDlg.Multiselect = false;
            openFileDlg.CheckFileExists = true;
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                iconPath.Text = openFileDlg.FileName;
                if (iconSize.Value == 0)
                {
                    iconSize.Value = 15;
                }
            }
            else
            {
                iconPath.Text = "";
            }
        }
        

        private void btn_save_Click(object sender, EventArgs e)
        {

            // Displays a SaveFileDialog so the user can save the Image
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Bitmap Image|*.bmp|PNG Image|*.png|JPeg Image|*.jpg|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                using (FileStream fs = (System.IO.FileStream) saveFileDialog1.OpenFile())
                {
                    // Saves the Image in the appropriate ImageFormat based upon the
                    // File type selected in the dialog box.
                    // NOTE that the FilterIndex property is one-based.

                    ImageFormat imageFormat = null;
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            imageFormat = ImageFormat.Bmp;
                            break;
                        case 2:
                            imageFormat = ImageFormat.Png;
                            break;
                        case 3:
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        case 4:
                            imageFormat = ImageFormat.Gif;
                            break;
                        default:
                            throw new NotSupportedException("File extension is not supported");
                    }

                    pictureBoxQRCode.BackgroundImage.Save(fs, imageFormat);
                    fs.Close();
                }
            }





        }

        public void ExportToBmp(string path)
        {
            
        }



        private void standardPrint()
        {
            PrintDocument pd = new PrintDocument();
            PrintController printController = new StandardPrintController();
            PaperSize pageSize = new PaperSize("label", 400, 100);
            pd.PrintController = printController;
            pd.PrinterSettings.DefaultPageSettings.PaperSize = pageSize;
            pd.DefaultPageSettings.Margins.Left = 0;
            pd.DefaultPageSettings.Margins.Top = 0;
            pd.DefaultPageSettings.Margins.Right = 0;
            pd.DefaultPageSettings.Margins.Bottom = 0;
            pd.DefaultPageSettings.PaperSize = pageSize;

            pd.OriginAtMargins = true;
            pd.DocumentName = "";
            printTemp[pd.DocumentName] = new List<string>();
            try
            {  
                printTemp[pd.DocumentName].Add(text);
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                pd.Print();
                printTemp[pd.DocumentName].Clear();
            }
            catch
            {
                MessageBox.Show("Error in standard print");
            }
        }

        void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            try
            {
                Font printFont = new Font("Lucida Console", 12, FontStyle.Regular);
                SolidBrush br = new SolidBrush(Color.Black);

                PrintDocument pd = sender as PrintDocument;

                float yPos = 0f;
                int count = 0;
                //float leftMargin = ev.MarginBounds.Left;
                //float topMargin = ev.MarginBounds.Top;
                float leftMargin = 20;
                float topMargin = 0;
                string line = null;
                //float linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);
                int linesPerPage = 90;
                ev.Graphics.DrawString(pd.DocumentName, printFont, Brushes.Black, 10, 5, new StringFormat());

                while (count < linesPerPage)
                {

                    if (count < printTemp[pd.DocumentName].Count())
                    {
                        line = printTemp[pd.DocumentName][count];
                    }
                    if (count == printTemp[pd.DocumentName].Count())
                    {
                        line = null;
                        break;
                    }
                    if (count == linesPerPage - 1)
                    {
                        printTemp[pd.DocumentName].RemoveRange(0, count + 1);
                    }

                    if (line == null)
                    {
                        break;
                    }

                    yPos = topMargin + count * printFont.GetHeight(ev.Graphics);
                    ev.Graphics.DrawImage(qrImage, 250, -5, 110, 110);
                    ev.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
                    count++;
                    

                }
                if (line != null)
                {
                    ev.HasMorePages = true;
                }
            }
            catch
            {
                MessageBox.Show("Error in pd_Printpage");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
           // textBoxQRCode.Text
        }

        private void textBoxQRCode_TextChanged(object sender, EventArgs e)
        {
            RenderQrCode();
        }

    }
}
