using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ComputerVision;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Numerics;

namespace Aplicațíe_pentru_prelucrarea_de_fotografii
{
    public partial class Form1 : Form
    {
        bool sidebarExpend;
        private bool isPresetesSidebarVisible = false;
        private bool isCropSidebarVisible = false;
        private bool isEditSidebarVisible = false;
        private bool isBrushSidebarVisible = false;
        private bool isTextSidebarVisible = false;
        private bool isFramesSidebarVisible = false;
        private bool isHealingSidebarVisible = false;
        private bool workInCurrentForm = true; //   Initially, work is done on the image from the current form
        private string sSourceFileName = "";
        // Dictionary to store the original Bitmap associated with each child form
        private Dictionary<Form, Bitmap> imaginiOriginale = new Dictionary<Form, Bitmap>();
        // Dictionary to store the previous image for each child form
        private Dictionary<Form, Bitmap> imaginiAnterioare = new Dictionary<Form, Bitmap>();
        // Dictionary to store the modified images for each form
        private Dictionary<Photo, Bitmap> imaginiModificate = new Dictionary<Photo, Bitmap>();
        // Dictionary to store the modified images for each form, along with darknessValue
        private Dictionary<Photo, int> darknessValues = new Dictionary<Photo, int>();
        // Dictionary to store the modified images for each form, along with exposureValue
        private Dictionary<Photo, int> exposureValues = new Dictionary<Photo, int>();
        // Dictionary to store the modified images for each form, along with contrastValue
        private Dictionary<Photo, float> contrastValues = new Dictionary<Photo, float>();
        // Dictionary to store the modified images for each form, along with highlightsValue
        private Dictionary<Photo, float> highlightsValues = new Dictionary<Photo, float>();
        // Dictionary to store the modified images for each form, along with shadowsValue
        private Dictionary<Photo, float> shadowsValues = new Dictionary<Photo, float>();
        // Dictionary to store the modified images for each form, along with saturationValue
        private Dictionary<Photo, int> saturationValues = new Dictionary<Photo, int>();
        // Dictionary to store the modified images for each form, along with tempValue
        private Dictionary<Photo,float> tempValues = new Dictionary<Photo, float>();
        private bool boldActivated = false;
        private bool italicActivated = false;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MdiClient ctlMDI;

            foreach (Control ctl in this.Controls)
            {
                try
                {
                    ctlMDI = (MdiClient)ctl;
                    ctlMDI.BackColor = System.Drawing.Color.Black;
                    menuStrip1.Enabled = true;
                }
                catch (InvalidCastException exc)
                {

                }

            }
            sidebar_presetes.Hide();
            sidebar_crop.Hide();
            sidebar_edit.Hide();
            sidebar_brush.Hide();
            sidebar_text.Hide();
            sidebar_frame.Hide();
            sidebar_healing.Hide();

            ChangeWorkLocationButton.Text = "Edit in Current Window";


            InstalledFontCollection installedFont = new InstalledFontCollection();
            FontFamily[] fontFamilies = installedFont.Families;
            for (int i = 0; i < fontFamilies.Length; i++)
            {
                comboBox_font_f.Items.Add(fontFamilies[i].Name);
            }
            comboBox_font_f.SelectedItem = "Arial";
            this.KeyPreview = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Ești sigur că vrei să închizi aplicația?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open dialog to select an image
            openFileDialog.ShowDialog();
            sSourceFileName = openFileDialog.FileName;

            // Create a child form for the newly loaded image
            Photo newPhotoForm = new Photo();
            newPhotoForm.MdiParent = this;
            newPhotoForm.Show();

            // Save a reference to the original image as a Bitmap
            Bitmap imagineOriginala = new Bitmap(sSourceFileName);
            imaginiOriginale.Add(newPhotoForm, imagineOriginala);

            // Load the image into the PictureBox of the new child form 
            newPhotoForm.LoadImage(sSourceFileName);

            if (!darknessValues.ContainsKey(newPhotoForm))
            {
                darknessValues[newPhotoForm] = 0;
            }
            if (!exposureValues.ContainsKey(newPhotoForm))
            {
                exposureValues[newPhotoForm] = 0;
            }
            if (!contrastValues.ContainsKey(newPhotoForm))
            {
                contrastValues[newPhotoForm] = 0;
            }
            if (!highlightsValues.ContainsKey(newPhotoForm))
            {
                highlightsValues[newPhotoForm] = 0;
            }
            if (!shadowsValues.ContainsKey(newPhotoForm))
            {
                shadowsValues[newPhotoForm] = 0;
            }
            if (!saturationValues.ContainsKey(newPhotoForm))
            {
                saturationValues[newPhotoForm] = 0;
            }
            if (!tempValues.ContainsKey(newPhotoForm))
            {
                tempValues[newPhotoForm] = 0;
            }
            newPhotoForm.EnablePictureBoxMouseDown(false);
            newPhotoForm.EnablePictureBoxMouseMove(false);
            newPhotoForm.EnablePictureBoxMouseUp(false);
        }



        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if the child form is open
            if (ActiveMdiChild is Photo activeChildForm)
            {
                // Check if the PictureBox of the child form has an image
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Create a Bitmap from the PictureBox image of the child form
                    Bitmap bmp = new Bitmap(activeChildForm.pictureBox_photo.Image);

                    // Display a save dialog to choose the file path and format
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp";
                    saveFileDialog.Title = "Save an Image File";
                    saveFileDialog.ShowDialog();

                    // If a file name is chosen
                    if (saveFileDialog.FileName != "")
                    {
                        // Get the file extension
                        string extension = System.IO.Path.GetExtension(saveFileDialog.FileName);

                        // Save the image based on the chosen format
                        switch (extension.ToLower())
                        {
                            case ".jpg":
                                bmp.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;
                            case ".png":
                                bmp.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                                break;
                            case ".bmp":
                                bmp.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                break;
                            default:
                                // If the format is not recognized, save as PNG
                                bmp.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                                break;
                        }

                        MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Dispose of the bitmap object
                    bmp.Dispose();
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void sidebarTimer_Tick(object sender, EventArgs e)
        {
            //Setam  min si max size  al sidebar Panel


            if (sidebarExpend)
            {
                // daca sidebar-ul este expend , minimizam
                sidebar.Width -= 10;
                if (sidebar.Width == sidebar.MinimumSize.Width)
                {
                    sidebarExpend = false;
                    sidebarTimer.Stop();
                }
            }
            else
            {
                sidebar.Width += 10;
                if (sidebar.Width == sidebar.MaximumSize.Width)
                {
                    sidebarExpend = true;
                    sidebarTimer.Stop();
                }
            }
        }

        private void ToolsButton_Click(object sender, EventArgs e)
        {
            // set timer interval to lowest  to make it smoother
            sidebarTimer.Start();
        }

        private void AddPhotoButton_Click(object sender, EventArgs e)
        {
            // Open dialog to select an image
            openFileDialog.ShowDialog();
            sSourceFileName = openFileDialog.FileName;

            // Create a child form for the newly loaded image
            Photo newPhotoForm = new Photo();
            newPhotoForm.MdiParent = this;
            newPhotoForm.Show();

            // Save a reference to the original image as a Bitmap
            Bitmap imagineOriginala = new Bitmap(sSourceFileName);
            imaginiOriginale.Add(newPhotoForm, imagineOriginala);

            // Load the image into the PictureBox of the new child form 
            newPhotoForm.LoadImage(sSourceFileName);

            if (!darknessValues.ContainsKey(newPhotoForm))
            {
                darknessValues[newPhotoForm] = 0;
            }
            if (!exposureValues.ContainsKey(newPhotoForm))
            {
                exposureValues[newPhotoForm] = 0;
            }
            if (!contrastValues.ContainsKey(newPhotoForm))
            {
                contrastValues[newPhotoForm] = 0;
            }
            if (!highlightsValues.ContainsKey(newPhotoForm))
            {
                highlightsValues[newPhotoForm] = 0;
            }
            if (!shadowsValues.ContainsKey(newPhotoForm))
            {
                shadowsValues[newPhotoForm] = 0;
            }
            if (!saturationValues.ContainsKey(newPhotoForm))
            {
                saturationValues[newPhotoForm] = 0;
            }
            if (!tempValues.ContainsKey(newPhotoForm))
            {
                tempValues[newPhotoForm] = 0;
            }

            newPhotoForm.EnablePictureBoxMouseDown(false);
            newPhotoForm.EnablePictureBoxMouseMove(false);
            newPhotoForm.EnablePictureBoxMouseUp(false);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if(pictureBox.Image != null) 
            {
                pictureBox.Image.Dispose();// elibereaza resursele imaginii
                pictureBox.Image = null;
            }
            pictureBox.Visible = false;*/
        }


        private void presetesButton_Click(object sender, EventArgs e)
        {
            // check if the presetes sidebar is visible or not
            if (!isPresetesSidebarVisible)
            {
                // close other sidebar
                sidebar_crop.Hide();
                isCropSidebarVisible = false;
                sidebar_edit.Hide();
                isEditSidebarVisible = false;
                sidebar_brush.Hide();
                isBrushSidebarVisible = false;
                sidebar_text.Hide();
                isTextSidebarVisible = false;
                sidebar_frame.Hide();
                isFramesSidebarVisible = false;
                sidebar_healing.Hide();
                isHealingSidebarVisible = false;
                // show sidebar_presets
                sidebar_presetes.Show();
                isPresetesSidebarVisible = true;
            }
            else
            {
                // hide sidebar_presetes
                sidebar_presetes.Hide();
                isPresetesSidebarVisible = false;
            }
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(false); 
                activeChildForm.EnablePictureBoxMouseMove(false);
                activeChildForm.EnablePictureBoxMouseUp(false);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // check if the crop sidebar is visible or not
            if (!isCropSidebarVisible)
            {
                // close other sidebar
                sidebar_presetes.Hide();
                isPresetesSidebarVisible = false;
                sidebar_edit.Hide();
                isEditSidebarVisible = false;
                sidebar_brush.Hide();
                isBrushSidebarVisible = false;
                sidebar_text.Hide();
                isTextSidebarVisible = false;
                sidebar_frame.Hide();
                isFramesSidebarVisible = false;
                sidebar_healing.Hide();
                isHealingSidebarVisible = false;
                // show sidebar_crop
                sidebar_crop.Show();
                isCropSidebarVisible = true;
            }
            else
            {
                // hide sidebar_crop
                sidebar_crop.Hide();
                isCropSidebarVisible = false;
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            // check if the edit sidebar is visible or not
            if (!isEditSidebarVisible)
            {
                // close other sidebar
                sidebar_presetes.Hide();
                isPresetesSidebarVisible = false;
                sidebar_crop.Hide();
                isCropSidebarVisible = false;
                sidebar_brush.Hide();
                isBrushSidebarVisible = false;
                sidebar_text.Hide();
                isTextSidebarVisible = false;
                sidebar_frame.Hide();
                isFramesSidebarVisible = false;
                sidebar_healing.Hide();
                isHealingSidebarVisible = false;
                // show sidebar_edit
                sidebar_edit.Show();
                isEditSidebarVisible = true;

                revertToLastVersionToolStripMenuItem.Visible = false;
            }
            else
            {
                // hide sidebar_edit
                sidebar_edit.Hide();
                isEditSidebarVisible = false;
                revertToLastVersionToolStripMenuItem.Visible = true;
            }
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(false);
                activeChildForm.EnablePictureBoxMouseMove(false);
                activeChildForm.EnablePictureBoxMouseUp(false);
            }

        }

        private void brushButton_Click(object sender, EventArgs e)
        {
            // check if the brush sidebar is visible or not
            if (!isBrushSidebarVisible)
            {
                // close other sidebar
                sidebar_presetes.Hide();
                isPresetesSidebarVisible = false;
                sidebar_crop.Hide();
                isCropSidebarVisible = false;
                sidebar_edit.Hide();
                isEditSidebarVisible = false;
                sidebar_text.Hide();
                isTextSidebarVisible = false;
                sidebar_frame.Hide();
                isFramesSidebarVisible = false;
                sidebar_healing.Hide();
                isHealingSidebarVisible = false;
                // show sidebar_brush
                sidebar_brush.Show();
                isBrushSidebarVisible = true;
            }
            else
            {
                // hide sidebar_brush
                sidebar_brush.Hide();
                isBrushSidebarVisible = false;
            }
        }

        private void textButton_Click(object sender, EventArgs e)
        {
            // check if the text sidebar is visible or not
            if (!isTextSidebarVisible)
            {
                // close other sidebar
                sidebar_presetes.Hide();
                isPresetesSidebarVisible = false;
                sidebar_crop.Hide();
                isCropSidebarVisible = false;
                sidebar_edit.Hide();
                isEditSidebarVisible = false;
                sidebar_brush.Hide();
                isBrushSidebarVisible = false;
                sidebar_frame.Hide();
                isFramesSidebarVisible = false;
                sidebar_healing.Hide();
                isHealingSidebarVisible = false;
                // show sidebar_text
                sidebar_text.Show();
                isTextSidebarVisible = true;
            }
            else
            {
                // hide sidebar_text
                sidebar_text.Hide();
                isTextSidebarVisible = false;
            }
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(false);
                activeChildForm.EnablePictureBoxMouseMove(false);
                activeChildForm.EnablePictureBoxMouseUp(false);
            }
        }

        private void frameButton_Click(object sender, EventArgs e)
        {
            // check if the frame sidebar is visible or not
            if (!isFramesSidebarVisible)
            {
                // close other sidebar
                sidebar_presetes.Hide();
                isPresetesSidebarVisible = false;
                sidebar_crop.Hide();
                isCropSidebarVisible = false;
                sidebar_edit.Hide();
                isEditSidebarVisible = false;
                sidebar_brush.Hide();
                isBrushSidebarVisible = false;
                sidebar_text.Hide();
                isTextSidebarVisible = false;
                sidebar_healing.Hide();
                isHealingSidebarVisible = false;
                // show sidebar_frame
                sidebar_frame.Show();
                isFramesSidebarVisible = true;
            }
            else
            {
                // hide sidebar_frame
                sidebar_frame.Hide();
                isFramesSidebarVisible = false;
            }
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(false);
                activeChildForm.EnablePictureBoxMouseMove(false);
                activeChildForm.EnablePictureBoxMouseUp(false);
            }
        }

        private void healingButton_Click(object sender, EventArgs e)
        {
            // check if the healing sidebar is visible or not
            if (!isHealingSidebarVisible)
            {
                // close other sidebar
                sidebar_presetes.Hide();
                isPresetesSidebarVisible = false;
                sidebar_crop.Hide();
                isCropSidebarVisible = false;
                sidebar_edit.Hide();
                isEditSidebarVisible = false;
                sidebar_brush.Hide();
                isBrushSidebarVisible = false;
                sidebar_text.Hide();
                isTextSidebarVisible = false;
                sidebar_frame.Hide();
                isFramesSidebarVisible = false;
                // show sidebar_healing
                sidebar_healing.Show();
                isHealingSidebarVisible = true;
            }
            else
            {
                // hide sidebar_healing
                sidebar_healing.Hide();
                isHealingSidebarVisible = false;
            }
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(false);
                activeChildForm.EnablePictureBoxMouseMove(false);
                activeChildForm.EnablePictureBoxMouseUp(false);
            }
        }



        private void CopyImageButton_Click(object sender, EventArgs e)
        {
            // Check if there is an active child form and a Bitmap associated with it to make a copy
            if (this.ActiveMdiChild != null && imaginiOriginale.ContainsKey(this.ActiveMdiChild))
            {
                // Create a copy of the Bitmap associated with the active form and set it to the PictureBox of that form
                Bitmap copiedImage = new Bitmap(imaginiOriginale[this.ActiveMdiChild]);
            }
        }

        private void revertToOriginalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null && imaginiOriginale.ContainsKey(this.ActiveMdiChild))
            {
                // Retrieve the original image associated with the active form from the dictionary
                Bitmap originalImage = imaginiOriginale[this.ActiveMdiChild];

                // Set the original image for the PictureBox of the active form
                ((Photo)this.ActiveMdiChild).pictureBox_photo.Image = originalImage;

                // Reset the modified image in imaginiModificate to the original image
                imaginiModificate[(Photo)this.ActiveMdiChild] = new Bitmap(originalImage);

                // Reset the values in darknessValues, exposureValues, etc., to 0
                darknessValues[(Photo)this.ActiveMdiChild] = 0;
                exposureValues[(Photo)this.ActiveMdiChild] = 0;
                contrastValues[(Photo)this.ActiveMdiChild] = 0;
                highlightsValues[(Photo)this.ActiveMdiChild] = 0;
                shadowsValues[(Photo)this.ActiveMdiChild] = 0;
                saturationValues[(Photo)this.ActiveMdiChild] = 0;
                tempValues[(Photo)this.ActiveMdiChild] = 0;
                // ... other values reset to 0 ...

                // Update the UI to display these reset values
                label_Darkness_val.Text = darknessValues[(Photo)this.ActiveMdiChild].ToString();
                trackBar_Darkness.Value = darknessValues[(Photo)this.ActiveMdiChild];

                label_Exposure_val.Text = exposureValues[(Photo)this.ActiveMdiChild].ToString();
                trackBar_Exposure.Value = exposureValues[(Photo)this.ActiveMdiChild];

                label_Contrast_val.Text = contrastValues[(Photo)this.ActiveMdiChild].ToString();
                trackBar_Contrast.Value = (int)contrastValues[(Photo)this.ActiveMdiChild];

                label_Highlights_val.Text = highlightsValues[(Photo)this.ActiveMdiChild].ToString();
                trackBar_Highlights.Value = (int)highlightsValues[(Photo)this.ActiveMdiChild];

                label_Shadows_val.Text = shadowsValues[(Photo)this.ActiveMdiChild].ToString();
                trackBar_Shadows.Value = (int)shadowsValues[(Photo)this.ActiveMdiChild];

                label_Saturation_val.Text = saturationValues[(Photo)this.ActiveMdiChild].ToString();
                trackBar_Saturation.Value = saturationValues[(Photo)this.ActiveMdiChild];

                label_Temp_val.Text = tempValues[(Photo)this.ActiveMdiChild].ToString();
                trackBar_Temp.Value = (int)tempValues[(Photo)this.ActiveMdiChild];

            }
            else
            {
                MessageBox.Show("No original image to revert to.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void RevenireLaUltimaVersiuneAImaginii()
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                // Check if there is a previous image associated with the active form
                if (imaginiAnterioare.ContainsKey(activeChildForm))
                {
                    // Restore the previous image in the PictureBox
                    activeChildForm.pictureBox_photo.Image = new Bitmap(imaginiAnterioare[activeChildForm]);
                }
                else
                {
                    MessageBox.Show("No previous image to revert to.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void revertToLastVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RevenireLaUltimaVersiuneAImaginii();
        }


        private void ChangeWorkLocationButton_Click(object sender, EventArgs e)
        {
            workInCurrentForm = !workInCurrentForm; // We invert the value of the variable each time the button is pressed

            if (!workInCurrentForm)
            {
                ChangeWorkLocationButton.Text = "Edit in New Window"; // Button text "Edit in New Window"
                revertToOriginalToolStripMenuItem.Visible = false;
                revertToLastVersionToolStripMenuItem.Visible = false;
            }
            else
            {
                ChangeWorkLocationButton.Text = "Edit in Current Window"; // Button text "Edit in Current Window"
                revertToOriginalToolStripMenuItem.Visible = true;
                revertToLastVersionToolStripMenuItem.Visible = true;
            }
        }

        /// <summary>
        /// Filtre / Preseturi
        /// </summary>

        static Bitmap FiltruSepia(Bitmap imagineOriginala)
        {
            Bitmap imagineSepia = new Bitmap(imagineOriginala.Width, imagineOriginala.Height);

            for (int y = 0; y < imagineOriginala.Height; y++)
            {
                for (int x = 0; x < imagineOriginala.Width; x++)
                {
                    Color pixel = imagineOriginala.GetPixel(x, y);

                    int red = (int)(pixel.R * 0.393 + pixel.G * 0.769 + pixel.B * 0.189);
                    int green = (int)(pixel.R * 0.349 + pixel.G * 0.686 + pixel.B * 0.168);
                    int blue = (int)(pixel.R * 0.272 + pixel.G * 0.534 + pixel.B * 0.131);

                    red = Math.Min(red, 255);
                    green = Math.Min(green, 255);
                    blue = Math.Min(blue, 255);

                    imagineSepia.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return imagineSepia;
        }

        public void AplicaFiltruSepia()
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuSepia = FiltruSepia(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuSepia;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Sepia Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

  

        static Bitmap FiltruAlbNegru(Bitmap imagineOriginala)
        {
            Bitmap imagineAlbNegru = new Bitmap(imagineOriginala.Width, imagineOriginala.Height);

            for (int y = 0; y < imagineOriginala.Height; y++)
            {
                for (int x = 0; x < imagineOriginala.Width; x++)
                {
                    Color pixel = imagineOriginala.GetPixel(x, y);

                    int medie = (int)((pixel.R + pixel.G + pixel.B) / 3);

                    Color pixelNou = Color.FromArgb(medie, medie, medie);

                    imagineAlbNegru.SetPixel(x, y, pixelNou);
                }
            }

            return imagineAlbNegru;
        }

        public void AplicaFiltruAlbNegru()
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuAlbNegru = FiltruAlbNegru(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuAlbNegru;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Black And White Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruDuotone_Albastru_Portocaliu(Bitmap originalImage)
        {
            Bitmap duotoneImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    Color color1 = Color.Blue;
                    Color color2 = Color.Orange;
                    int average = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114); // Calculating average color intensity

                    // Applying the duotone effect using the two specified colors
                    Color newPixel = Color.FromArgb(
                        (color1.R * average) / 255 + (color2.R * (255 - average)) / 255,
                        (color1.G * average) / 255 + (color2.G * (255 - average)) / 255,
                        (color1.B * average) / 255 + (color2.B * (255 - average)) / 255
                    );

                    duotoneImage.SetPixel(x, y, newPixel); // Setting the new pixel in the duotone image
                }
            }

            return duotoneImage;
        }

        public void AplicaFiltruDuotone_Albastru_Portocaliu()//onli one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuDuotoneAlbastruPortocaliu = FiltruDuotone_Albastru_Portocaliu(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuDuotoneAlbastruPortocaliu;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Duotone_Blue_Orange Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruDuotone_Rosu_Verde(Bitmap originalImage)
        {
            Bitmap duotoneImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    Color color1 = Color.Red;
                    Color color2 = Color.Green;
                    int average = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114); // Calculating average color intensity

                    // Applying the duotone effect using the two specified colors
                    Color newPixel = Color.FromArgb(
                        (color1.R * average) / 255 + (color2.R * (255 - average)) / 255,
                        (color1.G * average) / 255 + (color2.G * (255 - average)) / 255,
                        (color1.B * average) / 255 + (color2.B * (255 - average)) / 255
                    );

                    duotoneImage.SetPixel(x, y, newPixel); // Setting the new pixel in the duotone image
                }
            }

            return duotoneImage;
        }

        public void AplicaFiltruDuotone_Rosu_Verde()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuDuotoneRosuVerde = FiltruDuotone_Rosu_Verde(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuDuotoneRosuVerde;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Duotone_Red_Green Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruDuotone_Rosu_Galben(Bitmap originalImage)
        {
            Bitmap duotoneImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    Color color1 = Color.Red;
                    Color color2 = Color.Yellow;
                    int average = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114); // Calculating average color intensity

                    // Applying the duotone effect using the two specified colors
                    Color newPixel = Color.FromArgb(
                        (color1.R * average) / 255 + (color2.R * (255 - average)) / 255,
                        (color1.G * average) / 255 + (color2.G * (255 - average)) / 255,
                        (color1.B * average) / 255 + (color2.B * (255 - average)) / 255
                    );

                    duotoneImage.SetPixel(x, y, newPixel); // Setting the new pixel in the duotone image
                }
            }

            return duotoneImage;
        }

        public void AplicaFiltruDuotone_Rosu_Galben()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuDuotoneRosuGalben = FiltruDuotone_Rosu_Galben(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuDuotoneRosuGalben;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Duotone_Red_Yellow Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruDuotone_Mov_Turcoaz(Bitmap originalImage)
        {
            Bitmap duotoneImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    Color color1 = Color.Purple;
                    Color color2 = Color.Turquoise;
                    int average = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114); // Calculating average color intensity

                    // Applying the duotone effect using the two specified colors
                    Color newPixel = Color.FromArgb(
                        (color1.R * average) / 255 + (color2.R * (255 - average)) / 255,
                        (color1.G * average) / 255 + (color2.G * (255 - average)) / 255,
                        (color1.B * average) / 255 + (color2.B * (255 - average)) / 255
                    );

                    duotoneImage.SetPixel(x, y, newPixel); // Setting the new pixel in the duotone image
                }
            }

            return duotoneImage;
        }

        public void AplicaFiltruDuotone_Mov_Turcoaz()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuDuotoneMovTurcoaz = FiltruDuotone_Mov_Turcoaz(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuDuotoneMovTurcoaz;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Duotone_Purple_Turquoise Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        static Bitmap FiltruDuotone_Gri_Alb(Bitmap originalImage)
        {
            Bitmap duotoneImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    Color color1 = Color.Gray;
                    Color color2 = Color.White;
                    int average = (int)((pixel.R + pixel.G + pixel.B )/3); // Calculating average color intensity

                    // Applying the duotone effect using the two specified colors
                    Color newPixel = Color.FromArgb(
                        (color1.R * average) / 255 + (color2.R * (255 - average)) / 255,
                        (color1.G * average) / 255 + (color2.G * (255 - average)) / 255,
                        (color1.B * average) / 255 + (color2.B * (255 - average)) / 255
                    );

                    duotoneImage.SetPixel(x, y, newPixel); // Setting the new pixel in the duotone image
                }
            }

            return duotoneImage;
        }

        public void AplicaFiltruDuotone_Gri_Alb()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuDuotoneGriAlb = FiltruDuotone_Gri_Alb(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuDuotoneGriAlb;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Duotone_Gray_White Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruDuotone_Maro_Verde(Bitmap originalImage)
        {
            Bitmap duotoneImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    Color color1 = Color.GreenYellow;
                    Color color2 = Color.Brown;
                    int average = (int)((pixel.R + pixel.G + pixel.B) / 3); // Calculating average color intensity

                    // Applying the duotone effect using the two specified colors
                    Color newPixel = Color.FromArgb(
                        (color1.R * average) / 255 + (color2.R * (255 - average)) / 255,
                        (color1.G * average) / 255 + (color2.G * (255 - average)) / 255,
                        (color1.B * average) / 255 + (color2.B * (255 - average)) / 255
                    );

                    duotoneImage.SetPixel(x, y, newPixel); // Setting the new pixel in the duotone image
                }
            }

            return duotoneImage;
        }

        public void AplicaFiltruDuotone_Maro_Verde()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuDuotoneMaroVerde = FiltruDuotone_Maro_Verde(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuDuotoneMaroVerde;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Duotone_GreenYellow_Brown Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruToneCaldeReci(Bitmap originalImage)
        {
            Bitmap imagineCalda = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);                  
                    int red = pixel.R;
                    int green = pixel.G;
                    int blue = pixel.B;

                    // Warm tones: red, orange, yello
                    red += 20; 
                    green += 5; 

                    // Cool tones: blue, green, purple
                    blue += 20; 

                    // Ensure the values stay within the correct range (0-255)
                    red = Math.Min(255, Math.Max(0, red));
                    green = Math.Min(255, Math.Max(0, green));
                    blue = Math.Min(255, Math.Max(0, blue));

                    imagineCalda.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return imagineCalda;
        }

        public void AplicaFiltruToneCaldeReci()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuToneCaldeReci = FiltruToneCaldeReci(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuToneCaldeReci;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Warm_Cool_Tone Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        static Bitmap FiltruCrossProcessing(Bitmap originalImage)
        {
            Bitmap imagineFiltrata= new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    int red = (int)(pixel.R * 0.5); 
                    int green = (int)(pixel.G * 1.5); 
                    int blue = (int)(pixel.B * 1.2);

                    red = Math.Min(255, Math.Max(0, red));
                    green = Math.Min(255, Math.Max(0, green));
                    blue = Math.Min(255, Math.Max(0, blue));

                    imagineFiltrata.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return imagineFiltrata;
        }

        public void AplicaFiltruCrossProcessing()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuCrossProcessing = FiltruCrossProcessing(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuCrossProcessing;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply CrossProcessing Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        static Bitmap FiltruVintage(Bitmap originalImage)
        {
            Bitmap imagineFiltrata = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    int red = (int)(pixel.R * 0.9); 
                    int green = (int)(pixel.G * 0.7); 
                    int blue = (int)(pixel.B * 0.5); 

                    red = Math.Min(255, Math.Max(0, red));
                    green = Math.Min(255, Math.Max(0, green));
                    blue = Math.Min(255, Math.Max(0, blue));

                    imagineFiltrata.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return imagineFiltrata;
        }

        public void AplicaFiltruVintage()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuVintage = FiltruVintage(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuVintage;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Vintage Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        static Bitmap FiltruHDR(Bitmap imagine)
        {
            int latime = imagine.Width;
            int inaltime = imagine.Height;

            // Imaginea rezultată va avea o gamă dinamică mai largă
            Bitmap imagineHDR = new Bitmap(latime, inaltime);

            for (int y = 0; y < inaltime; y++)
            {
                for (int x = 0; x < latime; x++)
                {
                    Color pixel = imagine.GetPixel(x, y);

                    // Simulăm un efect HDR aplicând un ton-mapping liniar pentru a extinde gama dinamică
                    int red = (int)(pixel.R * 1.2); // exemplu de extindere a intensității culorilor
                    int green = (int)(pixel.G * 1.2);
                    int blue = (int)(pixel.B * 1.2);

                    // Asigurăm că valorile nu depășesc intervalul RGB
                    red = Math.Min(255, Math.Max(0, red));
                    green = Math.Min(255, Math.Max(0, green));
                    blue = Math.Min(255, Math.Max(0, blue));

                   imagineHDR.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            // Aplicăm ton-mapping pentru a ajusta gama dinamică

            return imagineHDR;
        }

       
        public void AplicaFiltruHDR()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuCaricatura = FiltruHDR(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuCaricatura;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply HDR Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        static Bitmap FiltruVignette(Bitmap originalImage)
        {
            int width = originalImage.Width;
            int height = originalImage.Height;

            Bitmap vignetteImage = new Bitmap(width, height);

            // Determinăm centrul imaginii
            int centerX = width / 2;
            int centerY = height / 2;

            // Calculăm distanța de la fiecare pixel la centrul imaginii și aplicăm efectul de vignette
            double maxDistance = Math.Sqrt(centerX * centerX + centerY * centerY);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double distanceToCenter = Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));

                    // Calculăm un factor de opacitate bazat pe distanța față de centrul imaginii
                    // Cu cât este mai mare distanța, cu atât este mai mică opacitatea
                    double opacity = 1.0 - (distanceToCenter / maxDistance);

                    // Obținem pixelul original
                    Color originalPixel = originalImage.GetPixel(x, y);

                    // Modificăm pixelul aplicând factorul de opacitate
                    int red = (int)(originalPixel.R * opacity);
                    int green = (int)(originalPixel.G * opacity);
                    int blue = (int)(originalPixel.B * opacity);

                    Color vignetteColor = Color.FromArgb(red, green, blue);

                    vignetteImage.SetPixel(x, y, vignetteColor);
                }
            }

            return vignetteImage;
        }


        public void AplicaFiltrulVignette()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuVignette = FiltruVignette(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuVignette;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Vignette Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruClarendon(Bitmap originalImage)
        {
            Bitmap imagineFiltrata = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    int red = (int)(pixel.R * 1.2); // Ajustează roșu
                    int green = (int)(pixel.G * 1.1); // Ajustează verde
                    int blue = (int)(pixel.B * 1.1); // Ajustează albastru

                    // Asigură că valorile nu depășesc 255
                    red = Math.Min(red, 255);
                    green = Math.Min(green, 255);
                    blue = Math.Min(blue, 255);

                    imagineFiltrata.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return imagineFiltrata;
        }


        public void AplicaFiltruClarendon()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuClarendo = FiltruClarendon(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuClarendo;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Clarendon Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        static Bitmap FiltruRetro(Bitmap originalImage)
        {
            Bitmap imagineFiltrata = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    int red = (int)(pixel.R * 0.9); // Reducerea intensității roșului
                    int green = (int)(pixel.G * 0.9); // Reducerea intensității verdelui
                    int blue = (int)(pixel.B * 1.1); // Creșterea intensității albastrului

                    red = Math.Min(255, Math.Max(0, red));
                    green = Math.Min(255, Math.Max(0, green));
                    blue = Math.Min(255, Math.Max(0, blue));

                    imagineFiltrata.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return imagineFiltrata;
        }

        public void AplicaFiltruRetro()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuRetro = FiltruRetro(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuRetro;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Retro Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruGlitch(Bitmap originalImage)
        {
            Random rnd = new Random();
            Bitmap glitchedImage = new Bitmap(originalImage.Width, originalImage.Height);

            // Aplică pixelation - reducerea rezoluției imaginii
            int pixelationFactor = 10;
            for (int y = 0; y < originalImage.Height; y += pixelationFactor)
            {
                for (int x = 0; x < originalImage.Width; x += pixelationFactor)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    // Setează un grup de pixeli cu aceeași culoare pentru a crea efectul de pixelation
                    for (int py = 0; py < pixelationFactor; py++)
                    {
                        for (int px = 0; px < pixelationFactor; px++)
                        {
                            int newX = x + px;
                            int newY = y + py;

                            if (newX < originalImage.Width && newY < originalImage.Height)
                            {
                                glitchedImage.SetPixel(newX, newY, pixel);
                            }
                        }
                    }
                }
            }

            // Aplică deplasarea aleatoare a pixelilor (jittering)
            int maxJitter = 10;
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = glitchedImage.GetPixel(x, y);

                    int xOffset = rnd.Next(-maxJitter, maxJitter + 1);
                    int yOffset = rnd.Next(-maxJitter, maxJitter + 1);

                    int newX = x + xOffset;
                    int newY = y + yOffset;

                    if (newX >= 0 && newX < originalImage.Width && newY >= 0 && newY < originalImage.Height)
                    {
                        glitchedImage.SetPixel(newX, newY, pixel);
                    }
                }
            }

            return glitchedImage;
        }

        public void AplicaFiltruGlitch()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuGlitch = FiltruGlitch(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuGlitch;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Glitch Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap EfectFisheye(Bitmap originalImage)
        {
            int latime = originalImage.Width;
            int inaltime = originalImage.Height;
            int centruX = latime / 2;
            int centruY = inaltime / 2;
            double distantaMaxima = Math.Sqrt(centruX * centruX + centruY * centruY);

            Bitmap rezultat = new Bitmap(latime, inaltime);

            for (int y = 0; y < inaltime; y++)
            {
                for (int x = 0; x < latime; x++)
                {
                    double distanta = Math.Sqrt((x - centruX) * (x - centruX) + (y - centruY) * (y - centruY));

                    if (distanta < distantaMaxima)
                    {
                        double r = distanta / distantaMaxima;
                        double theta = Math.Atan2(y - centruY, x - centruX);
                        double newDistance = r * r * distantaMaxima;
                        int newX = (int)(centruX + newDistance * Math.Cos(theta));
                        int newY = (int)(centruY + newDistance * Math.Sin(theta));

                        if (newX >= 0 && newX < latime && newY >= 0 && newY < inaltime)
                        {
                            Color pixel = originalImage.GetPixel(newX, newY);
                            rezultat.SetPixel(x, y, pixel);
                        }
                    }
                }
            }

            return rezultat;
        }

        public void AplicaEfectFisheye()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuGlitch = EfectFisheye(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuGlitch;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Fisheye Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap DistorsiuneSferica(Bitmap originalImage)
        {
            int latime = originalImage.Width;
            int inaltime = originalImage.Height;
            int centruX = latime / 2;
            int centruY = inaltime / 2;
            double razaMaxima = Math.Min(centruX, centruY);

            Bitmap rezultat = new Bitmap(latime, inaltime);

            for (int y = 0; y < inaltime; y++)
            {
                for (int x = 0; x < latime; x++)
                {
                    int offsetX = x - centruX;
                    int offsetY = y - centruY;
                    double distanta = Math.Sqrt(offsetX * offsetX + offsetY * offsetY);

                    if (distanta < razaMaxima)
                    {
                        double theta = Math.Atan2(offsetY, offsetX);
                        double r = Math.Pow(distanta / razaMaxima, 2) * razaMaxima;

                        int newX = (int)(r * Math.Cos(theta)) + centruX;
                        int newY = (int)(r * Math.Sin(theta)) + centruY;

                        if (newX >= 0 && newX < latime && newY >= 0 && newY < inaltime)
                        {
                            Color pixel = originalImage.GetPixel(newX, newY);
                            rezultat.SetPixel(x, y, pixel);
                        }
                    }
                }
            }

            return rezultat;
        }

        public void AplicaDistorsiuneSferica()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuGlitch = DistorsiuneSferica(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuGlitch;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Glitch Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Random rand = new Random();

        static Bitmap FiltruOldMovie(Bitmap originalImage)
        {
            int latime = originalImage.Width;
            int inaltime = originalImage.Height;

            Bitmap rezultat = new Bitmap(latime, inaltime);

            // Adăugarea de zgomot/granulație
            for (int y = 0; y < inaltime; y++)
            {
                for (int x = 0; x < latime; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    int zgomot = rand.Next(-30, 30); // Ajustează intensitatea zgomotului

                    int R = Math.Max(0, Math.Min(255, pixel.R + zgomot));
                    int G = Math.Max(0, Math.Min(255, pixel.G + zgomot));
                    int B = Math.Max(0, Math.Min(255, pixel.B + zgomot));

                    rezultat.SetPixel(x, y, Color.FromArgb(R, G, B));
                }
            }

            // Schimbarea culorilor pentru a da un aspect mai învechit
            for (int y = 0; y < inaltime; y++)
            {
                for (int x = 0; x < latime; x++)
                {
                    Color pixel = rezultat.GetPixel(x, y);

                    int grayscale = (int)(0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B); // Convertirea în nuanțe de gri

                    Color newColor = Color.FromArgb(grayscale, grayscale, grayscale);
                    rezultat.SetPixel(x, y, newColor);
                }
            }

            // Adăugarea de efecte specifice filmelor vechi (defecte de film, îmbătrânire etc.)
            // Adăugarea de zgârieturi și linii verticale
            int numZgarieturi = rand.Next(5, 15); // Ajustează numărul de zgârieturi
            for (int i = 0; i < numZgarieturi; i++)
            {
                int zgWidth = rand.Next(1, 3); // Lățimea zgârieturii
                int zgX = rand.Next(0, latime);
                for (int zgY = 0; zgY < inaltime; zgY++)
                {
                    for (int j = 0; j < zgWidth; j++)
                    {
                        int px = zgX + j;
                        if (px < latime)
                        {
                            rezultat.SetPixel(px, zgY, Color.White);
                        }
                    }
                }
            }

            // Adăugarea de puncte negre
            int numPuncte = rand.Next(100, 200); // Ajustează numărul de puncte
            for (int i = 0; i < numPuncte; i++)
            {
                int px = rand.Next(0, latime);
                int py = rand.Next(0, inaltime);
                rezultat.SetPixel(px, py, Color.Black);
            }

            return rezultat;
        }

        public void AplicaFiltruOldMovie()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuGlitch = FiltruOldMovie(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuGlitch;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Old Movie Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        static Bitmap FiltruTechnicolor(Bitmap originalImage)
        {
            int latime = originalImage.Width;
            int inaltime = originalImage.Height;

            Bitmap rezultat = new Bitmap(latime, inaltime);

            for (int y = 0; y < inaltime; y++)
            {
                for (int x = 0; x < latime; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    int valoareGrayscale = (int)(0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B);

                    // Transformarea nuanțelor de gri într-o variație de culori specifice Technicolor
                    int R = (int)(valoareGrayscale * 1.2); // Poți ajusta coeficienții pentru fiecare canal de culoare
                    int G = (int)(valoareGrayscale * 1.1);
                    int B = (int)(valoareGrayscale * 1.3);

                    R = Math.Min(255, R);
                    G = Math.Min(255, G);
                    B = Math.Min(255, B);

                    Color nouaCuloare = Color.FromArgb(R, G, B);

                    rezultat.SetPixel(x, y, nouaCuloare);
                }
            }

            return rezultat;
        }


        public void AplicaFiltruTechnicolor()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuGlitch = FiltruTechnicolor(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuGlitch;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Technicolor Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        static Bitmap FiltruKodachrome(Bitmap originalImage)
        {
            int latime = originalImage.Width;
            int inaltime = originalImage.Height;

            Bitmap rezultat = new Bitmap(latime, inaltime);

            for (int y = 0; y < inaltime; y++)
            {
                for (int x = 0; x < latime; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    // Ajustează culorile pentru a evidenția caracteristicile Kodachrome
                    int R = Math.Min(255, (int)(pixel.R * 1.4)); // Saturare mai mare a roșului
                    int G = Math.Min(255, (int)(pixel.G * 1.3)); // Saturare mai mare a verdeului
                    int B = Math.Min(255, (int)(pixel.B * 1.1)); // Saturare mai mare a albastrului

                    // Asigură-te că culorile nu depășesc valoarea maximă de 255
                    R = Math.Max(0, R);
                    G = Math.Max(0, G);
                    B = Math.Max(0, B);

                    Color nouaCuloare = Color.FromArgb(R, G, B);

                    rezultat.SetPixel(x, y, nouaCuloare);
                }
            }

            return rezultat;
        }

        public void AplicaFiltruKodachrome()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuGlitch = FiltruKodachrome(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuGlitch;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Kodachrome Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruPrewitt(Bitmap originalImage)
        {
            Color color;
            double[,] P = { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } };
            double[,] Q = { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
            double sumaPR = 0, sumaQR = 0;
            double sumaPG = 0, sumaQG = 0;
            double sumaPB = 0, sumaQB = 0;
            double s_R = 0, s_G = 0, s_B = 0;

            int latime = originalImage.Width;
            int inaltime = originalImage.Height;

            Bitmap rezultat = new Bitmap(latime, inaltime);

            for (int y = 1; y < inaltime - 2; y++)
            {
                for (int x = 1; x < latime - 2; x++)
                {
                    sumaPR = 0; sumaQR = 0;
                    sumaPG = 0; sumaQG = 0;
                    sumaPB = 0; sumaQB = 0;

                    for (int r = x - 1; r <= x + 1; r++)
                    {
                        for (int c = y - 1; c <= y + 1; c++)
                        {
                            color = originalImage.GetPixel(r, c);
                            int R = color.R;
                            int G = color.G;
                            int B = color.B;

                            sumaPR = sumaPR + R * P[r - x + 1, c - y + 1];
                            sumaPB = sumaPB + B * P[r - x + 1, c - y + 1];
                            sumaPG = sumaPG + R * P[r - x + 1, c - y + 1];

                            sumaQR = sumaQR + R * Q[r - x + 1, c - y + 1];
                            sumaQB = sumaQB + B * Q[r - x + 1, c - y + 1];
                            sumaQG = sumaQG + G * Q[r - x + 1, c - y + 1];

                        }
                    }
                    s_R = Math.Sqrt(Math.Pow(sumaPR, 2) + Math.Pow(sumaQR, 2));
                    s_G = Math.Sqrt(Math.Pow(sumaPG, 2) + Math.Pow(sumaQG, 2));
                    s_B = Math.Sqrt(Math.Pow(sumaPB, 2) + Math.Pow(sumaQB, 2));
                    s_R = Normalizare(s_R);
                    s_G = Normalizare(s_G);
                    s_B = Normalizare(s_B);


                    color = Color.FromArgb((int)s_R, (int)s_G, (int)s_B);

                    rezultat.SetPixel(x, y, color);
                }
            }
            double Normalizare(double x)
            {
                if (x > 255)
                    return 255;
                else if (x < 0)
                    return 0;
                else
                    return x;
            }


            return rezultat;
        }

        public void AplicaFiltruPrewitt()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuGlitch = FiltruPrewitt(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuGlitch;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Prewitt Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruBlur(Bitmap originalImage)
        {
            Bitmap imagineBlur = new Bitmap(originalImage.Width, originalImage.Height);

            byte[,] pixelR = new byte[originalImage.Width, originalImage.Height];
            byte[,] pixelG = new byte[originalImage.Width, originalImage.Height];
            byte[,] pixelB = new byte[originalImage.Width, originalImage.Height];

            
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    pixelR[x, y] = pixel.R;
                    pixelG[x, y] = pixel.G;
                    pixelB[x, y] = pixel.B;
                }
            }

            for (int y = 1; y < originalImage.Height - 1; y++)
            {
                for (int x = 1; x < originalImage.Width - 1; x++)
                {
                    int sumaR = 0, sumaG = 0, sumaB = 0;

                    
                    for (int offsetY = -1; offsetY <= 1; offsetY++)
                    {
                        for (int offsetX = -1; offsetX <= 1; offsetX++)
                        {
                            sumaR += pixelR[x + offsetX, y + offsetY];
                            sumaG += pixelG[x + offsetX, y + offsetY];
                            sumaB += pixelB[x + offsetX, y + offsetY];
                        }
                    }

                    
                    int avgR = sumaR / 9;
                    int avgG = sumaG / 9;
                    int avgB = sumaB / 9;

                    imagineBlur.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return imagineBlur;
        }


        public void AplicaFiltruBlur()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuBlur= FiltruBlur(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuBlur;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Blur Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        static Bitmap FiltruSharpen(Bitmap originalImage)
        {
            Bitmap imagineAccentuata = new Bitmap(originalImage.Width, originalImage.Height);
            int[,] kernel = { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } }; // Kernel pentru sharpening

            for (int y = 1; y < originalImage.Height - 1; y++)
            {
                for (int x = 1; x < originalImage.Width - 1; x++)
                {
                    int sumR = 0, sumG = 0, sumB = 0;

                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            Color pixel = originalImage.GetPixel(x + i, y + j);
                            sumR += pixel.R * kernel[i + 1, j + 1];
                            sumG += pixel.G * kernel[i + 1, j + 1];
                            sumB += pixel.B * kernel[i + 1, j + 1];
                        }
                    }

                    int newR = Math.Min(255, Math.Max(0, sumR));
                    int newG = Math.Min(255, Math.Max(0, sumG));
                    int newB = Math.Min(255, Math.Max(0, sumB));

                    imagineAccentuata.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
                }
            }

            return imagineAccentuata;
        }

        public void AplicaFiltruSharpen()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuSharpen = FiltruSharpen(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuSharpen;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Sharpen Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static Bitmap FiltruContrast(Bitmap originalImage, float contrastLevel)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);

            float contrast = (100.0f + contrastLevel) / 100.0f;
            contrast *= contrast;

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    float newRed = ((((pixel.R / 255.0f) - 0.5f) * contrast) + 0.5f) * 255.0f;
                    float newGreen = ((((pixel.G / 255.0f) - 0.5f) * contrast) + 0.5f) * 255.0f;
                    float newBlue = ((((pixel.B / 255.0f) - 0.5f) * contrast) + 0.5f) * 255.0f;

                    newRed = Math.Min(255, Math.Max(0, newRed));
                    newGreen = Math.Min(255, Math.Max(0, newGreen));
                    newBlue = Math.Min(255, Math.Max(0, newBlue));

                    adjustedImage.SetPixel(x, y, Color.FromArgb((int)newRed, (int)newGreen, (int)newBlue));
                }
            }

            return adjustedImage;
        }


        public void AplicaFiltruContrast()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {   
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    float contrast = 10;
                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuContrast = FiltruContrast(imagineInitiala,contrast);
                    activeChildForm.pictureBox_photo.Image = imagineCuContrast;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Contrast Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        static Bitmap FiltruPopArt(Bitmap originalImage, int intensitate)
        {
            Bitmap imagineFiltrata = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    int red = (pixel.R + intensitate) / (255 / 3) * (255 / 3);
                    int green = (pixel.G + intensitate) / (255 / 3) * (255 / 3);
                    int blue = (pixel.B + intensitate) / (255 / 3) * (255 / 3);

                    imagineFiltrata.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return imagineFiltrata;
        }

        public void AplicaFiltruPopArt()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    int intensitate = 2 ;
                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuSharpen = FiltruPopArt(imagineInitiala,intensitate);
                    activeChildForm.pictureBox_photo.Image = imagineCuSharpen;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply PopArt Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        public Bitmap FiltruEqualizareHistograma(Bitmap originalImage)
        {
            Bitmap imagineFiltrata = new Bitmap(originalImage.Width, originalImage.Height);

            int[] hist = new int[256];
            int[] histc = new int[256];
            int[] transf = new int[256];

            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    Color color = originalImage.GetPixel(i, j);
                    byte R = color.R;
                    byte G = color.G;
                    byte B = color.B;

                    byte average = (byte)((R + G + B) / 3);
                    hist[average]++;
                }
            }

            histc[0] = hist[0];

            for (int i = 1; i < 255; i++)
            {
                histc[i] = histc[i - 1] + hist[i];
            }

            for (int i = 0; i < 255; i++)
            {
                transf[i] = (histc[i] * 255) / (originalImage.Width * originalImage.Height);
            }

            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    Color color = originalImage.GetPixel(i, j);
                    byte R = color.R;
                    byte G = color.G;
                    byte B = color.B;

                    byte average = (byte)((R + G + B) / 3);

                    color = Color.FromArgb(transf[average], transf[average], transf[average]);
                    imagineFiltrata.SetPixel(i, j, color);
                }
            }

            return imagineFiltrata;
        }

        public void AplicaFiltruEqualizareHistograma()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);


                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuSharpen = FiltruEqualizareHistograma(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuSharpen;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Glitch Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public Bitmap DetectareContururi(Bitmap originalImage)
        {
            Bitmap imagineFinala = new Bitmap(originalImage.Width, originalImage.Height);

            
            byte[,] intensitati = new byte[originalImage.Width, originalImage.Height];
            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    Color pixel = originalImage.GetPixel(x, y);
                    byte intensitate = (byte)((pixel.R + pixel.G + pixel.B) / 3);
                    intensitati[x, y] = intensitate;
                }
            }

            // Aplica filtrul Sobel 
            for (int x = 1; x < originalImage.Width - 1; x++)
            {
                for (int y = 1; y < originalImage.Height - 1; y++)
                {
                    int Gx = (intensitati[x - 1, y + 1] + 2 * intensitati[x, y + 1] + intensitati[x + 1, y + 1]) -
                             (intensitati[x - 1, y - 1] + 2 * intensitati[x, y - 1] + intensitati[x + 1, y - 1]);

                    int Gy = (intensitati[x + 1, y - 1] + 2 * intensitati[x + 1, y] + intensitati[x + 1, y + 1]) -
                             (intensitati[x - 1, y - 1] + 2 * intensitati[x - 1, y] + intensitati[x - 1, y + 1]);

                    int valoareGradient = (int)Math.Sqrt(Gx * Gx + Gy * Gy);
                    valoareGradient = Math.Min(255, Math.Max(0, valoareGradient));

                    if (valoareGradient < 50)
                        imagineFinala.SetPixel(x, y, Color.White);
                    else
                        imagineFinala.SetPixel(x, y, Color.Black);
                }
            }

            return imagineFinala;
        }


        public void AplicaDetectareContururi()//only one press
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);


                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuSharpen = DetectareContururi(imagineInitiala);
                    activeChildForm.pictureBox_photo.Image = imagineCuSharpen;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Drawing Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void FiltruClarendonButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm) 
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruClarendon(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Clarendon Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }          
            }
            else 
            {
                AplicaFiltruClarendon();
            }

        }

        private void FiltruDesenButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = DetectareContururi(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Drawing Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaDetectareContururi();
            }
        }

        private void EfectFisheyeButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = EfectFisheye(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Fisheye Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaEfectFisheye();
            }
        }

        private void FiltruAlbNegruButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruAlbNegru(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Black And White Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruAlbNegru();
            }
        }

        private void FiltruBlurButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo =  FiltruBlur(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Blur Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruBlur();
            }
        }

        private void FiltruContrastButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        float contrast = 10;

                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruContrast(imagineInitiala, contrast);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Contrast Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruContrast();
            }
        }


        private void FiltruCrossButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruCrossProcessing(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Cross Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruCrossProcessing();
            }
        }

        private void FiltruDuotone_Albastru_PortocaliuButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruDuotone_Albastru_Portocaliu(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Duotone_Blue_Orange Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruDuotone_Albastru_Portocaliu();
            }
        }

        private void FiltruDuotone_Gri_AlbButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruDuotone_Gri_Alb(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Duotone_Gray_White Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruDuotone_Gri_Alb();
            }
        }

        private void FiltruDuotone_Maro_VerdeButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruDuotone_Maro_Verde(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Duotone_GreenYellow_Brown Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruDuotone_Maro_Verde();
            }        
        }

        private void FiltruDuotone_Mov_TurcoazButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruDuotone_Mov_Turcoaz(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Duotone_Purple_Turquoise Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruDuotone_Mov_Turcoaz();
            }
        }

        private void FiltruDuotone_Rosu_GalbenButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruDuotone_Rosu_Galben(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Duotone_Red_Yellow Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruDuotone_Rosu_Galben();
            }
        }

        private void FiltruDuotone_Rosu_VerdeButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruDuotone_Rosu_Verde(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Duotone_Red_Green Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruDuotone_Rosu_Verde();
            }
        }

        private void FiltruEqualizareHistogramaButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruEqualizareHistograma(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Equalizare Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruEqualizareHistograma();
            }
        }

        private void FiltruGlitchButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruGlitch(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Glitch Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruGlitch();
            }
        }

        private void FiltruHDRButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruHDR(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply HDR Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruHDR();
            }
        }

        private void FiltruKodachromeButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruKodachrome(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Kodachrome Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruKodachrome();
            }
        }

        private void FiltrulVignetteButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruVignette(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Vignette Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltrulVignette();
            }
        }

        private void FiltruOldMovieButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruOldMovie(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Old Movie Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruOldMovie();
            }
        }

        private void FiltruPopArtButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        int intensitate = 2;
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruPopArt(imagineInitiala,intensitate);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply PopArt Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruPopArt();
            }
        }

        private void FiltruPrewittButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruPrewitt(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Prewitt Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruPrewitt();
            }
        }

        private void FiltruRetroButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruRetro(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Retro Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruRetro();
            }
        }

        private void FiltruSepiaButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruSepia(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Sepia Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruSepia();
            }
        }

        private void FiltruSharpenButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruSharpen(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Sharpen Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruSharpen();
            }
        }

        private void FiltruTechnicolorButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruTechnicolor(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Technicolor Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruTechnicolor();
            }
        }

        private void FiltruToneCaldeReciButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruToneCaldeReci(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Warm_Cool_Tone Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruToneCaldeReci();
            }
        }

        private void FiltruVintageButton_Click(object sender, EventArgs e)
        {
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = FiltruVintage(imagineInitiala);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }


                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply Vintage Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaFiltruVintage();
            }
        }

        /// <summary>
        /// Crop
        /// </summary>

        public Bitmap RotesteImagine_Dreapta(Bitmap imagineInitiala, int numarRotiri)
        {
            for (int i = 0; i < numarRotiri; i++)
            {
                imagineInitiala.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }

            return imagineInitiala;
        }

        public void AplicaRotesteImagine_Dreapta()
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {

                    // Save the original image associated with the active form
                    Bitmap imagineCurenta = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineCurenta);

                    // Rotate with 90 degree
                    imagineCurenta = RotesteImagine_Dreapta(imagineCurenta, 1);

                    activeChildForm.pictureBox_photo.Image = imagineCurenta;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to rotate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        public Bitmap RotesteImagine_Stanga(Bitmap imagineInitiala, int numarRotiri)
        {
            for (int i = 0; i < numarRotiri; i++)
            {
                imagineInitiala.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }

            return imagineInitiala;
        }

        public void AplicaRotesteImagine_Stanga()
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineCurenta = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineCurenta);

                    // Rotate with 90 degree
                    imagineCurenta = RotesteImagine_Stanga(imagineCurenta, 1);


                    activeChildForm.pictureBox_photo.Image = imagineCurenta;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to rotate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public Bitmap OglindirePeOrizontala(Bitmap imagineInitiala)
        {
            imagineInitiala.RotateFlip(RotateFlipType.RotateNoneFlipX); // Flip horizontally
            return imagineInitiala;
        }

        public Bitmap OglindirePeVerticala(Bitmap imagineInitiala)
        {
            imagineInitiala.RotateFlip(RotateFlipType.RotateNoneFlipY); // Flip vertically
            return imagineInitiala;
        }

        public void AplicaOglindireImagineOrizontala()
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineCurenta = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineCurenta);

                    //flip
                    imagineCurenta = OglindirePeOrizontala(imagineCurenta);


                    activeChildForm.pictureBox_photo.Image = imagineCurenta;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to rotate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        public void AplicaOglindireImagineVerticala()
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineCurenta = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineCurenta);

                    //flip
                    imagineCurenta = OglindirePeVerticala(imagineCurenta);


                    activeChildForm.pictureBox_photo.Image = imagineCurenta;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to rotate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RotateDButton_Click(object sender, EventArgs e)
        {
            AplicaRotesteImagine_Dreapta();
        }

        private void RotireSButton_Click(object sender, EventArgs e)
        {
            AplicaRotesteImagine_Stanga();
        }

        private void FlipHButton_Click(object sender, EventArgs e)
        {
            AplicaOglindireImagineOrizontala();
        }

        private void FlipVButton_Click(object sender, EventArgs e)
        {
            AplicaOglindireImagineVerticala();
        }

        /// <summary>
        /// Edit Sidebar
        /// </summary>
        /// 
        public Bitmap AdjustBrightness(Bitmap originalImage, int delta)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);

            // Convert to 32-bit image (supports alpha channel)
            Bitmap image32 = new Bitmap(originalImage.Width, originalImage.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(image32);
            graphics.DrawImage(originalImage, new Rectangle(0, 0, originalImage.Width, originalImage.Height));

            // Get a copy of the 32-bit image pixels
            BitmapData data = image32.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytesPerPixel = 4;
            int stride = data.Stride;
            IntPtr scan0 = data.Scan0;
            byte[] pixelValues = new byte[originalImage.Width * originalImage.Height * bytesPerPixel];
            Marshal.Copy(scan0, pixelValues, 0, pixelValues.Length);

            // Modify the brightness
            for (int i = 0; i < pixelValues.Length; i += bytesPerPixel)
            {
                pixelValues[i] = (byte)Math.Max(0, Math.Min(255, pixelValues[i] + delta)); // Blue
                pixelValues[i + 1] = (byte)Math.Max(0, Math.Min(255, pixelValues[i + 1] + delta)); // Green
                pixelValues[i + 2] = (byte)Math.Max(0, Math.Min(255, pixelValues[i + 2] + delta)); // Red
            }

            // Copy modified pixel values back to image
            Marshal.Copy(pixelValues, 0, scan0, pixelValues.Length);
            image32.UnlockBits(data);

            return image32;
        }
        private void trackBar_Darkness_Scroll(object sender, EventArgs e)
        {
            label_Darkness_val.Text = trackBar_Darkness.Value.ToString();
            int intensitate = trackBar_Darkness.Value;

            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Verificăm dacă avem imaginea originală salvată pentru formular
                    if (!imaginiModificate.ContainsKey(activeChildForm))
                    {
                        imaginiModificate[activeChildForm] = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    }
                    // Save the Trackbar Darkness value
                    darknessValues[activeChildForm] = trackBar_Darkness.Value;

                    label_Exposure_val.Text = exposureValues[activeChildForm].ToString();
                    trackBar_Exposure.Value = exposureValues[activeChildForm];

                    label_Contrast_val.Text = contrastValues[activeChildForm].ToString();
                    trackBar_Contrast.Value = (int)contrastValues[activeChildForm];

                    label_Highlights_val.Text = highlightsValues[activeChildForm].ToString();
                    trackBar_Highlights.Value = (int)highlightsValues[activeChildForm];
                    
                    label_Shadows_val.Text = shadowsValues[activeChildForm].ToString();
                    trackBar_Shadows.Value = (int)shadowsValues[activeChildForm];
                    
                    label_Saturation_val.Text = saturationValues[activeChildForm].ToString();
                    trackBar_Saturation.Value = saturationValues[activeChildForm];

                    label_Temp_val.Text = tempValues[activeChildForm].ToString();
                    trackBar_Temp.Value = (int)tempValues[activeChildForm];
                    // Luăm imaginea curentă asociată formularului din dicționar
                    Bitmap imagineCurenta = new Bitmap(imaginiModificate[activeChildForm]);

                    // Aplicăm ajustarea luminozității (întunecarea) pe imaginea curentă
                    // Aplicăm ajustările în ordine pe imaginea curentă
                    imagineCurenta = AdjustBrightness(imagineCurenta, intensitate);
                    if (exposureValues[activeChildForm] != 0) { imagineCurenta = AdjustExposure(imagineCurenta, exposureValues[activeChildForm]); }
                    if ((int)contrastValues[activeChildForm] != 0) { imagineCurenta = AdjustContrast(imagineCurenta, contrastValues[activeChildForm]); }
                    if ((int)highlightsValues[activeChildForm] != 0) { imagineCurenta = AdjustHighlights(imagineCurenta, highlightsValues[activeChildForm]); }
                    if ((int)shadowsValues[activeChildForm] != 0) { imagineCurenta = AdjustShadows(imagineCurenta, shadowsValues[activeChildForm]); }
                    if (saturationValues[activeChildForm] != 0) { imagineCurenta = AdjustSaturation(imagineCurenta, saturationValues[activeChildForm]); }
                    if ((int)tempValues[activeChildForm] != 0) { imagineCurenta = AdjustTemperature(imagineCurenta, tempValues[activeChildForm]); }


                    // Actualizăm imaginea din formularul copil cu noua imagine modificată
                    activeChildForm.pictureBox_photo.Image = imagineCurenta;

                }
                else
                {
                    MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public unsafe Bitmap AdjustExposure(Bitmap originalImage, int exposureValue)
        {
            float brightness = 1.0f + (exposureValue / 100.0f); // Calculul factorului de luminozitate bazat pe valoarea expunerii

            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);

            // Crearea unui fix-up de regiune
            BitmapData originalData = originalImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData adjustedData = adjustedImage.LockBits(new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // Declararea unor variabile de lucru
            int originalStride = originalData.Stride;
            int adjustedStride = adjustedData.Stride;
            byte* originalScan0 = (byte*)originalData.Scan0.ToPointer();
            byte* adjustedScan0 = (byte*)adjustedData.Scan0.ToPointer();

            // Parcurgerea fiecărui pixel din imagine și aplicarea ajustării de expunere
            Parallel.For(0, originalImage.Height, j =>
            {
                byte* originalPixels = originalScan0 + j * originalStride;
                byte* adjustedPixels = adjustedScan0 + j * adjustedStride;

                for (int i = 0; i < originalData.Width; i++)
                {
                    // Citirea pixelului curent
                    byte red = originalPixels[4 * i];
                    byte green = originalPixels[4 * i + 1];
                    byte blue = originalPixels[4 * i + 2];

                    // Calculul noilor componente de culoare în funcție de expunerea dorită
                    byte newRed = (byte)Math.Max(0, Math.Min(255, red * brightness));
                    byte newGreen = (byte)Math.Max(0, Math.Min(255, green * brightness));
                    byte newBlue = (byte)Math.Max(0, Math.Min(255, blue * brightness));

                    // Setarea pixelului ajustat în imaginea finală
                    adjustedPixels[4 * i] = newRed;
                    adjustedPixels[4 * i + 1] = newGreen;
                    adjustedPixels[4 * i + 2] = newBlue;
                    adjustedPixels[4 * i + 3] = 255; // Alocarea valorii alpha 255 pentru toate pixelii
                }
            });

            // Eliberarea blocurilor de regiune și rezervarea pixelilor ajustați
            originalImage.UnlockBits(originalData);
            adjustedImage.UnlockBits(adjustedData);

            return adjustedImage;
        }

        private void trackBar_Exposure_Scroll(object sender, EventArgs e)
        {
            label_Exposure_val.Text = trackBar_Exposure.Value.ToString();
            int exposureValue = trackBar_Exposure.Value;

            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Verificăm dacă avem imaginea originală salvată pentru formular
                    if (!imaginiModificate.ContainsKey(activeChildForm))
                    {
                        imaginiModificate[activeChildForm] = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    }
                    // Save the Trackbar Exposure value
                    exposureValues[activeChildForm] = trackBar_Exposure.Value;

                    label_Darkness_val.Text = darknessValues[activeChildForm].ToString();
                    trackBar_Darkness.Value = darknessValues[activeChildForm];

                    label_Contrast_val.Text = contrastValues[activeChildForm].ToString();
                    trackBar_Contrast.Value = (int)contrastValues[activeChildForm];

                    label_Highlights_val.Text = highlightsValues[activeChildForm].ToString();
                    trackBar_Highlights.Value = (int)highlightsValues[activeChildForm];

                    label_Shadows_val.Text = shadowsValues[activeChildForm].ToString();
                    trackBar_Shadows.Value = (int)shadowsValues[activeChildForm];

                    label_Saturation_val.Text = saturationValues[activeChildForm].ToString();
                    trackBar_Saturation.Value = saturationValues[activeChildForm];

                    label_Temp_val.Text = tempValues[activeChildForm].ToString();
                    trackBar_Temp.Value = (int)tempValues[activeChildForm];

                    // Luăm imaginea curentă asociată formularului din dicționar
                    Bitmap imagineCurenta = new Bitmap(imaginiModificate[activeChildForm]);

                    // Aplicăm ajustarea expunerii pe imaginea curentă
                    imagineCurenta = AdjustExposure(imagineCurenta, exposureValue);
                    if (darknessValues[activeChildForm] != 0) { imagineCurenta = AdjustBrightness(imagineCurenta, darknessValues[activeChildForm]); }
                    if ((int)contrastValues[activeChildForm] != 0) { imagineCurenta = AdjustContrast(imagineCurenta, contrastValues[activeChildForm]); }
                    if ((int)highlightsValues[activeChildForm] != 0) { imagineCurenta = AdjustHighlights(imagineCurenta, highlightsValues[activeChildForm]); }
                    if ((int)shadowsValues[activeChildForm] != 0) { imagineCurenta = AdjustShadows(imagineCurenta, shadowsValues[activeChildForm]); }
                    if (saturationValues[activeChildForm]  != 0) { imagineCurenta = AdjustSaturation(imagineCurenta, saturationValues[activeChildForm]); }
                    if ((int)tempValues[activeChildForm] != 0) { imagineCurenta = AdjustTemperature(imagineCurenta, tempValues[activeChildForm]); }
                    // Actualizăm imaginea din formularul copil cu noua imagine modificată
                    activeChildForm.pictureBox_photo.Image = imagineCurenta;

                }
                else
                {
                    MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public Bitmap AdjustContrast(Bitmap originalImage, float contrastValue)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);

            float factor = (100.0f + contrastValue) / 100.0f;
            factor *= factor;

            BitmapData originalData = originalImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData adjustedData = adjustedImage.LockBits(new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytesPerPixel = 4;
            int heightInPixels = originalData.Height;
            int widthInBytes = originalData.Width * bytesPerPixel;

            unsafe
            {
                byte* originalPointer = (byte*)originalData.Scan0;
                byte* adjustedPointer = (byte*)adjustedData.Scan0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    byte* originalLine = originalPointer + (y * originalData.Stride);
                    byte* adjustedLine = adjustedPointer + (y * adjustedData.Stride);

                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        float red = originalLine[x + 2] / 255.0f;
                        float green = originalLine[x + 1] / 255.0f;
                        float blue = originalLine[x] / 255.0f;

                        red = (((red - 0.5f) * factor) + 0.5f) * 255.0f;
                        green = (((green - 0.5f) * factor) + 0.5f) * 255.0f;
                        blue = (((blue - 0.5f) * factor) + 0.5f) * 255.0f;

                        red = Math.Max(0, Math.Min(255, red));
                        green = Math.Max(0, Math.Min(255, green));
                        blue = Math.Max(0, Math.Min(255, blue));

                        adjustedLine[x + 2] = (byte)red;
                        adjustedLine[x + 1] = (byte)green;
                        adjustedLine[x] = (byte)blue;
                        adjustedLine[x + 3] = originalLine[x + 3]; // Alpha channel
                    }
                }
            }

            originalImage.UnlockBits(originalData);
            adjustedImage.UnlockBits(adjustedData);

            return adjustedImage;
        }

        private void trackBar_Contrast_Scroll(object sender, EventArgs e)
        {
            label_Contrast_val.Text = trackBar_Contrast.Value.ToString();
            float contrastValue = trackBar_Contrast.Value / 10.0f; 

            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Verificăm dacă avem imaginea originală salvată pentru formular
                    if (!imaginiModificate.ContainsKey(activeChildForm))
                    {
                        imaginiModificate[activeChildForm] = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    }
                    // Save the Trackbar contrast value
                    contrastValues[activeChildForm] = trackBar_Contrast.Value;

                    label_Darkness_val.Text = darknessValues[activeChildForm].ToString();
                    trackBar_Darkness.Value = darknessValues[activeChildForm];

                    label_Exposure_val.Text = exposureValues[activeChildForm].ToString();
                    trackBar_Exposure.Value = exposureValues[activeChildForm];

                    label_Highlights_val.Text = highlightsValues[activeChildForm].ToString();
                    trackBar_Highlights.Value = (int)highlightsValues[activeChildForm];

                    label_Shadows_val.Text = shadowsValues[activeChildForm].ToString();
                    trackBar_Shadows.Value = (int)shadowsValues[activeChildForm];

                    label_Saturation_val.Text = saturationValues[activeChildForm].ToString();
                    trackBar_Saturation.Value = saturationValues[activeChildForm];

                    label_Temp_val.Text = tempValues[activeChildForm].ToString();
                    trackBar_Temp.Value = (int)tempValues[activeChildForm];

                    // Luăm imaginea curentă asociată formularului din dicționar
                    Bitmap imagineCurenta = new Bitmap(imaginiModificate[activeChildForm]);

                    // Aplicăm ajustarea contrastului pe imaginea curentă
                    imagineCurenta = AdjustContrast(imagineCurenta, contrastValue);
                    if (darknessValues[activeChildForm] != 0) { imagineCurenta = AdjustBrightness(imagineCurenta, darknessValues[activeChildForm]); }
                    if (exposureValues[activeChildForm] != 0) { imagineCurenta = AdjustExposure(imagineCurenta, exposureValues[activeChildForm]); }             
                    if ((int)highlightsValues[activeChildForm] != 0) { imagineCurenta = AdjustHighlights(imagineCurenta, highlightsValues[activeChildForm]); }
                    if ((int)shadowsValues[activeChildForm] != 0) { imagineCurenta = AdjustShadows(imagineCurenta, shadowsValues[activeChildForm]); }
                    if (saturationValues[activeChildForm] != 0) { imagineCurenta = AdjustSaturation(imagineCurenta, saturationValues[activeChildForm]); }
                    if ((int)tempValues[activeChildForm] != 0) { imagineCurenta = AdjustTemperature(imagineCurenta, tempValues[activeChildForm]); }
                    // Actualizăm imaginea din formularul copil cu noua imagine modificată
                    activeChildForm.pictureBox_photo.Image = imagineCurenta;

                }
                else
                {
                    MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public Bitmap AdjustHighlights(Bitmap originalImage, float highlightValue)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);
            BitmapData originalData = originalImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData adjustedData = adjustedImage.LockBits(new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytesPerPixel = 4;
            int heightInPixels = originalImage.Height;
            int widthInBytes = originalImage.Width * bytesPerPixel;

            unsafe
            {
                byte* originalPtr = (byte*)originalData.Scan0.ToPointer();
                byte* adjustedPtr = (byte*)adjustedData.Scan0.ToPointer();

                for (int y = 0; y < heightInPixels; y++)
                {
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        float red = originalPtr[x + 2] / 255.0f;
                        float green = originalPtr[x + 1] / 255.0f;
                        float blue = originalPtr[x] / 255.0f;

                        float factor = (100.0f + highlightValue) / 100.0f;

                        red = ((red - 0.5f) * factor + 0.5f) * 255.0f;
                        green = ((green - 0.5f) * factor + 0.5f) * 255.0f;
                        blue = ((blue - 0.5f) * factor + 0.5f) * 255.0f;

                        red = Math.Max(0, Math.Min(255, red));
                        green = Math.Max(0, Math.Min(255, green));
                        blue = Math.Max(0, Math.Min(255, blue));

                        adjustedPtr[x + 2] = (byte)red;
                        adjustedPtr[x + 1] = (byte)green;
                        adjustedPtr[x] = (byte)blue;
                        adjustedPtr[x + 3] = originalPtr[x + 3]; // Alpha channel
                    }

                    originalPtr += originalData.Stride;
                    adjustedPtr += adjustedData.Stride;
                }
            }

            originalImage.UnlockBits(originalData);
            adjustedImage.UnlockBits(adjustedData);

            return adjustedImage;
        }

        private void trackBar_Highlights_Scroll(object sender, EventArgs e)
        {
            label_Highlights_val.Text = trackBar_Highlights.Value.ToString();
            float highlightValue = trackBar_Highlights.Value;

            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Verificăm dacă avem imaginea originală salvată pentru formular
                    if (!imaginiModificate.ContainsKey(activeChildForm))
                    {
                        imaginiModificate[activeChildForm] = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    }
                    // Save the Trackbar Highlights value
                    highlightsValues[activeChildForm] = trackBar_Highlights.Value;
                    label_Darkness_val.Text = darknessValues[activeChildForm].ToString();
                    trackBar_Darkness.Value = darknessValues[activeChildForm];

                    label_Exposure_val.Text = exposureValues[activeChildForm].ToString();
                    trackBar_Exposure.Value = exposureValues[activeChildForm];

                    label_Contrast_val.Text = contrastValues[activeChildForm].ToString();
                    trackBar_Contrast.Value = (int)contrastValues[activeChildForm];

                    label_Shadows_val.Text = shadowsValues[activeChildForm].ToString();
                    trackBar_Shadows.Value = (int)shadowsValues[activeChildForm];

                    label_Saturation_val.Text = saturationValues[activeChildForm].ToString();
                    trackBar_Saturation.Value = saturationValues[activeChildForm];

                    label_Temp_val.Text = tempValues[activeChildForm].ToString();
                    trackBar_Temp.Value = (int)tempValues[activeChildForm];

                    // Luăm imaginea curentă asociată formularului din dicționar
                    Bitmap imagineCurenta = new Bitmap(imaginiModificate[activeChildForm]);

                    // Aplicăm ajustarea luminozității în zonele luminate pe imaginea curentă
                    imagineCurenta = AdjustHighlights(imagineCurenta, highlightValue);
                    if (darknessValues[activeChildForm] != 0) { imagineCurenta = AdjustBrightness(imagineCurenta, darknessValues[activeChildForm]); }
                    if (exposureValues[activeChildForm] != 0) { imagineCurenta = AdjustExposure(imagineCurenta, exposureValues[activeChildForm]); }
                    if ((int)contrastValues[activeChildForm] != 0) { imagineCurenta = AdjustContrast(imagineCurenta, contrastValues[activeChildForm]); }
                    if ((int)shadowsValues[activeChildForm] != 0) { imagineCurenta = AdjustShadows(imagineCurenta, shadowsValues[activeChildForm]); }
                    if (saturationValues[activeChildForm] != 0) { imagineCurenta = AdjustSaturation(imagineCurenta, saturationValues[activeChildForm]); }
                    if ((int)tempValues[activeChildForm] != 0) { imagineCurenta = AdjustTemperature(imagineCurenta, tempValues[activeChildForm]); }

                    // Actualizăm imaginea din formularul copil cu noua imagine modificată
                    activeChildForm.pictureBox_photo.Image = imagineCurenta;

                }
                else
                {
                    MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public Bitmap AdjustShadows(Bitmap originalImage, float shadowValue)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);
            BitmapData originalData = originalImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData adjustedData = adjustedImage.LockBits(new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytesPerPixel = 4;
            int heightInPixels = originalImage.Height;
            int widthInBytes = originalImage.Width * bytesPerPixel;

            unsafe
            {
                byte* originalPtr = (byte*)originalData.Scan0.ToPointer();
                byte* adjustedPtr = (byte*)adjustedData.Scan0.ToPointer();

                for (int y = 0; y < heightInPixels; y++)
                {
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        float red = originalPtr[x + 2] / 255.0f;
                        float green = originalPtr[x + 1] / 255.0f;
                        float blue = originalPtr[x] / 255.0f;

                        float luminosity = 0.2126f * red + 0.7152f * green + 0.0722f * blue; // Calculate luminosity

                        if (luminosity < 0.5f) // Apply only to shadowed areas
                        {
                            float factor = (100.0f + shadowValue) / 100.0f;

                            red *= factor;
                            green *= factor;
                            blue *= factor;

                            red = Math.Max(0, Math.Min(1, red)) * 255.0f;
                            green = Math.Max(0, Math.Min(1, green)) * 255.0f;
                            blue = Math.Max(0, Math.Min(1, blue)) * 255.0f;

                            adjustedPtr[x + 2] = (byte)red;
                            adjustedPtr[x + 1] = (byte)green;
                            adjustedPtr[x] = (byte)blue;
                            adjustedPtr[x + 3] = originalPtr[x + 3]; // Alpha channel
                        }
                        else // Copy original pixel data for non-shadowed areas
                        {
                            adjustedPtr[x + 2] = originalPtr[x + 2];
                            adjustedPtr[x + 1] = originalPtr[x + 1];
                            adjustedPtr[x] = originalPtr[x];
                            adjustedPtr[x + 3] = originalPtr[x + 3]; // Alpha channel
                        }
                    }

                    originalPtr += originalData.Stride;
                    adjustedPtr += adjustedData.Stride;
                }
            }

            originalImage.UnlockBits(originalData);
            adjustedImage.UnlockBits(adjustedData);

            return adjustedImage;
        }

        private void trackBar_Shadows_Scroll(object sender, EventArgs e)
        {
            label_Shadows_val.Text = trackBar_Shadows.Value.ToString();
            float shadowValue = trackBar_Shadows.Value;

            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Verificăm dacă avem imaginea originală salvată pentru formular
                    if (!imaginiModificate.ContainsKey(activeChildForm))
                    {
                        imaginiModificate[activeChildForm] = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    }
                    // Save the Trackbar Shadows value
                    shadowsValues[activeChildForm] = trackBar_Shadows.Value;
                    label_Darkness_val.Text = darknessValues[activeChildForm].ToString();
                    trackBar_Darkness.Value = darknessValues[activeChildForm];

                    label_Exposure_val.Text = exposureValues[activeChildForm].ToString();
                    trackBar_Exposure.Value = exposureValues[activeChildForm];

                    label_Highlights_val.Text = highlightsValues[activeChildForm].ToString();
                    trackBar_Highlights.Value = (int)highlightsValues[activeChildForm];

                    label_Contrast_val.Text = contrastValues[activeChildForm].ToString();
                    trackBar_Contrast.Value = (int)contrastValues[activeChildForm];

                    label_Saturation_val.Text = saturationValues[activeChildForm].ToString();
                    trackBar_Saturation.Value = saturationValues[activeChildForm];

                    label_Temp_val.Text = tempValues[activeChildForm].ToString();
                    trackBar_Temp.Value = (int)tempValues[activeChildForm];

                    // Luăm imaginea curentă asociată formularului din dicționar
                    Bitmap imagineCurenta = new Bitmap(imaginiModificate[activeChildForm]);

                    // Aplicăm ajustarea umbrelor pe imaginea curentă
                    imagineCurenta = AdjustShadows(imagineCurenta, shadowValue);
                    if (darknessValues[activeChildForm] != 0) { imagineCurenta = AdjustBrightness(imagineCurenta, darknessValues[activeChildForm]); }
                    if (exposureValues[activeChildForm] != 0) { imagineCurenta = AdjustExposure(imagineCurenta, exposureValues[activeChildForm]); }
                    if ((int)contrastValues[activeChildForm] != 0) { imagineCurenta = AdjustContrast(imagineCurenta, contrastValues[activeChildForm]); }
                    if ((int)highlightsValues[activeChildForm] != 0) { imagineCurenta = AdjustHighlights(imagineCurenta, highlightsValues[activeChildForm]); }
                    if (saturationValues[activeChildForm] != 0) { imagineCurenta = AdjustSaturation(imagineCurenta, saturationValues[activeChildForm]); }
                    if ((int)tempValues[activeChildForm] != 0) { imagineCurenta = AdjustTemperature(imagineCurenta, tempValues[activeChildForm]); }


                    // Actualizăm imaginea din formularul copil cu noua imagine modificată
                    activeChildForm.pictureBox_photo.Image = imagineCurenta;

                }
                else
                {
                    MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public Bitmap AdjustSaturation(Bitmap originalImage, float saturationValue)
        {
            float saturationFactor = (100.0f + saturationValue) / 100.0f;
            BitmapData originalData = originalImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* originalPtr = (byte*)originalData.Scan0;

                for (int y = 0; y < originalData.Height; y++)
                {
                    for (int x = 0; x < originalData.Width; x++)
                    {
                        int offset = (y * originalData.Stride) + (x * 4);
                        float[] hsv = RGBtoHSV(originalPtr[offset + 2], originalPtr[offset + 1], originalPtr[offset]);

                        hsv[1] *= saturationFactor;
                        if (hsv[1] > 1.0f) hsv[1] = 1.0f; // Clamp saturation value

                        Color adjustedColor = HSVtoRGB(hsv[0], hsv[1], hsv[2]);
                        originalPtr[offset + 2] = adjustedColor.R;
                        originalPtr[offset + 1] = adjustedColor.G;
                        originalPtr[offset] = adjustedColor.B;
                    }
                }
            }

            originalImage.UnlockBits(originalData);
            return originalImage;
        }

        // Convert RGB to HSV color space
        private float[] RGBtoHSV(int red, int green, int blue)
        {
            float r = red / 255.0f;
            float g = green / 255.0f;
            float b = blue / 255.0f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            float hue = 0;
            if (delta != 0)
            {
                if (max == r)
                    hue = (g - b) / delta + (g < b ? 6 : 0);
                else if (max == g)
                    hue = (b - r) / delta + 2;
                else
                    hue = (r - g) / delta + 4;
                hue /= 6;
            }

            float saturation = (max != 0) ? delta / max : 0;
            float value = max;

            return new float[] { hue, saturation, value };
        }

        // Convert HSV to RGB color space
        private Color HSVtoRGB(float hue, float saturation, float value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue * 6)) % 6;
            float f = (float)(hue * 6 - Math.Floor(hue * 6));
            int v = Convert.ToInt32(value * 255);
            int p = Convert.ToInt32(value * (1 - saturation) * 255);
            int q = Convert.ToInt32(value * (1 - f * saturation) * 255);
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation) * 255);

            if (hi == 0) return Color.FromArgb(v, t, p);
            else if (hi == 1) return Color.FromArgb(q, v, p);
            else if (hi == 2) return Color.FromArgb(p, v, t);
            else if (hi == 3) return Color.FromArgb(p, q, v);
            else if (hi == 4) return Color.FromArgb(t, p, v);
            else return Color.FromArgb(v, p, q);
        }

        private void trackBar_Saturation_Scroll(object sender, EventArgs e)
        {
            label_Saturation_val.Text = trackBar_Saturation.Value.ToString();
            int saturationValue = trackBar_Saturation.Value;

            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Verificăm dacă avem imaginea originală salvată pentru formular
                    if (!imaginiModificate.ContainsKey(activeChildForm))
                    {
                        imaginiModificate[activeChildForm] = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    }
                    // Save the Trackbar Saturation value
                    saturationValues[activeChildForm] = trackBar_Saturation.Value;
                    label_Darkness_val.Text = darknessValues[activeChildForm].ToString();
                    trackBar_Darkness.Value = darknessValues[activeChildForm];

                    label_Exposure_val.Text = exposureValues[activeChildForm].ToString();
                    trackBar_Exposure.Value = exposureValues[activeChildForm];

                    label_Highlights_val.Text = highlightsValues[activeChildForm].ToString();
                    trackBar_Highlights.Value = (int)highlightsValues[activeChildForm];

                    label_Shadows_val.Text = shadowsValues[activeChildForm].ToString();
                    trackBar_Shadows.Value = (int)shadowsValues[activeChildForm];

                    label_Contrast_val.Text = contrastValues[activeChildForm].ToString();
                    trackBar_Contrast.Value = (int)contrastValues[activeChildForm];

                    label_Temp_val.Text = tempValues[activeChildForm].ToString();
                    trackBar_Temp.Value = (int)tempValues[activeChildForm];

                    // Luăm imaginea curentă asociată formularului din dicționar
                    Bitmap imagineCurenta = new Bitmap(imaginiModificate[activeChildForm]);

                    // Aplicăm ajustarea saturării pe imaginea curentă
                    imagineCurenta = AdjustSaturation(imagineCurenta, saturationValue);
                    if (darknessValues[activeChildForm] != 0) { imagineCurenta = AdjustBrightness(imagineCurenta, darknessValues[activeChildForm]); }
                    if (exposureValues[activeChildForm] != 0) { imagineCurenta = AdjustExposure(imagineCurenta, exposureValues[activeChildForm]); }
                    if ((int)contrastValues[activeChildForm] != 0) { imagineCurenta = AdjustContrast(imagineCurenta, contrastValues[activeChildForm]); }
                    if ((int)highlightsValues[activeChildForm] != 0) { imagineCurenta = AdjustHighlights(imagineCurenta, highlightsValues[activeChildForm]); }
                    if ((int)shadowsValues[activeChildForm] != 0) { imagineCurenta = AdjustShadows(imagineCurenta, shadowsValues[activeChildForm]); }
                    if ((int)tempValues[activeChildForm] != 0) { imagineCurenta = AdjustTemperature(imagineCurenta, tempValues[activeChildForm]); }


                    // Actualizăm imaginea din formularul copil cu noua imagine modificată
                    activeChildForm.pictureBox_photo.Image = imagineCurenta;

                }
                else
                {
                    MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public Bitmap AdjustTemperature(Bitmap originalImage, float temperatureValue)
        {
            if (Math.Abs(temperatureValue) < float.Epsilon)
                return new Bitmap(originalImage); // Returnează o copie a imaginii originale când temperatura este 0

            float tempFactor = (temperatureValue + 100) / 200.0f;

            float blueFactor = Math.Max(0, 1 - tempFactor);
            float redFactor = Math.Max(0, tempFactor);

            // Aplicarea curbei de transfer pentru a ajusta tranziția culorilor la valori mici
            blueFactor = 1 - (float)Math.Pow(1 - blueFactor, 2);
            redFactor = 1 - (float)Math.Pow(1 - redFactor, 2);

            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);

            BitmapData originalData = originalImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData adjustedData = adjustedImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* originalPtr = (byte*)originalData.Scan0;
                byte* adjustedPtr = (byte*)adjustedData.Scan0;

                for (int y = 0; y < originalData.Height; y++)
                {
                    for (int x = 0; x < originalData.Width; x++)
                    {
                        int offset = (y * originalData.Stride) + (x * 4);

                        float blue = originalPtr[offset];
                        float green = originalPtr[offset + 1];
                        float red = originalPtr[offset + 2];

                        blue *= blueFactor;
                        red *= redFactor;

                        adjustedPtr[offset] = (byte)Math.Min(255, blue);
                        adjustedPtr[offset + 1] = (byte)Math.Min(255, green);
                        adjustedPtr[offset + 2] = (byte)Math.Min(255, red);
                        adjustedPtr[offset + 3] = originalPtr[offset + 3]; // Alpha channel remains unchanged
                    }
                }
            }

            originalImage.UnlockBits(originalData);
            adjustedImage.UnlockBits(adjustedData);

            return adjustedImage;
        }

        private void trackBar_Temp_Scroll(object sender, EventArgs e)
        {
            label_Temp_val.Text = trackBar_Temp.Value.ToString();
            float temperatureValue = trackBar_Temp.Value;

            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Verificăm dacă avem imaginea originală salvată pentru formular
                    if (!imaginiModificate.ContainsKey(activeChildForm))
                    {
                        imaginiModificate[activeChildForm] = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    }
                    // Save the Trackbar Temp value
                    tempValues[activeChildForm] = trackBar_Temp.Value;
                    label_Darkness_val.Text = darknessValues[activeChildForm].ToString();
                    trackBar_Darkness.Value = darknessValues[activeChildForm];

                    label_Exposure_val.Text = exposureValues[activeChildForm].ToString();
                    trackBar_Exposure.Value = exposureValues[activeChildForm];

                    label_Highlights_val.Text = highlightsValues[activeChildForm].ToString();
                    trackBar_Highlights.Value = (int)highlightsValues[activeChildForm];

                    label_Shadows_val.Text = shadowsValues[activeChildForm].ToString();
                    trackBar_Shadows.Value = (int)shadowsValues[activeChildForm];

                    label_Saturation_val.Text = saturationValues[activeChildForm].ToString();
                    trackBar_Saturation.Value = saturationValues[activeChildForm];

                    label_Contrast_val.Text = contrastValues[activeChildForm].ToString();
                    trackBar_Contrast.Value = (int)contrastValues[activeChildForm];

                    // Luăm imaginea curentă asociată formularului din dicționar
                    Bitmap imagineCurenta = new Bitmap(imaginiModificate[activeChildForm]);

                    // Aplicăm ajustarea saturării pe imaginea curentă
                    imagineCurenta = AdjustTemperature(imagineCurenta, temperatureValue);
                    if (darknessValues[activeChildForm] != 0) { imagineCurenta = AdjustBrightness(imagineCurenta, darknessValues[activeChildForm]); }
                    if (exposureValues[activeChildForm] != 0) { imagineCurenta = AdjustExposure(imagineCurenta, exposureValues[activeChildForm]); }
                    if ((int)contrastValues[activeChildForm] != 0) { imagineCurenta = AdjustContrast(imagineCurenta, contrastValues[activeChildForm]); }
                    if ((int)highlightsValues[activeChildForm] != 0) { imagineCurenta = AdjustHighlights(imagineCurenta, highlightsValues[activeChildForm]); }
                    if ((int)shadowsValues[activeChildForm] != 0) { imagineCurenta = AdjustShadows(imagineCurenta, shadowsValues[activeChildForm]); }
                    if (saturationValues[activeChildForm] != 0) { imagineCurenta = AdjustSaturation(imagineCurenta, saturationValues[activeChildForm]); }


                    // Actualizăm imaginea din formularul copil cu noua imagine modificată
                    activeChildForm.pictureBox_photo.Image = imagineCurenta;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void button_pencil_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(true); // Activează evenimentul
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);

                //  Salveaza imaginea originala asociata formului copil activ
                Bitmap imagineCurenta = new Bitmap(activeChildForm.pictureBox_photo.Image);
                imaginiAnterioare[activeChildForm] = new Bitmap(imagineCurenta);

                // Apelează o metodă din formularul copil pentru desenare
                activeChildForm.StartPencilDrawing();

            }
        }

        private void button_high_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.IncreasePenSize();
                float currentPenSize = activeChildForm.GetCurrentPenSize();
                size_label.Text = currentPenSize.ToString(); // Sets the label text with the current pen value
            }
        }

        private void button_low_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.DecreasePenSize();
                float currentPenSize = activeChildForm.GetCurrentPenSize();
                size_label.Text = currentPenSize.ToString(); // Sets the label text with the current pen value
            }
        }

        private void button_eraser_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(true); // Activează evenimentul
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);
                // Apelează o metodă din formularul copil pentru desenare
                activeChildForm.StartEraserDrawing();
            }
        }

        private void button_ellipse_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(true); // Activează evenimentul
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);
                // Apelează o metodă din formularul copil pentru desenare
                activeChildForm.DrawingEllipse();
            }
        }

        private void button_rect_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(true); // Activează evenimentul
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);
                // Apelează o metodă din formularul copil pentru desenare
                activeChildForm.DrawingRectangle();
            }
        }

        private void button_line_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(true); // Activează evenimentul
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);
                // Apelează o metodă din formularul copil pentru desenare
                activeChildForm.DrawingLine();
            }
        }

        private void button_triangle_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(true); // Activează evenimentul
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);
                // Apelează o metodă din formularul copil pentru desenare
                activeChildForm.DrawingBezier();
            }
        }

        private void button_color_palette_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(true); // Activează evenimentul
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);
                // Apelează o metodă din formularul copil pentru desenare
                activeChildForm.DrawingColor();
                pic_color.BackColor = activeChildForm.GetColor();
            }
        }

        static Point set_point(PictureBox pb, Point pt)
        {
            float pX = 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));
        }

        private void color_picker_MouseClick(object sender, MouseEventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(true); // Activează evenimentul
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);
                // Apelează o metodă din formularul copil pentru desenare
                Point point = set_point(color_picker, e.Location);
                pic_color.BackColor = ((Bitmap)color_picker.Image).GetPixel(point.X, point.Y);
                activeChildForm.new_color = pic_color.BackColor;
                activeChildForm.UpdateColor();
            }
        }

        private void button_fill_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.EnablePictureBoxMouseDown(true); // Activează evenimentul
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);
                // Apelează o metodă din formularul copil pentru desenare
                activeChildForm.FillColor();
            }
        }

        private void button_add_txt_Click(object sender, EventArgs e)
        {
            // Obține formularul copil activ
            if (ActiveMdiChild is Photo activeChildForm)
            {

                // Transmite informațiile necesare pentru adăugarea textului
                int width = int.Parse(textBox_width_txt.Text);
                int height = int.Parse(textBox_height_txt.Text);
                string fontName = comboBox_font_f.SelectedItem.ToString();
                float fontSize = float.Parse(textBox_font_s_txt.Text);
                Color textColor = button_color_txt.BackColor;

                // Trimite informațiile către formularul copil pentru adăugarea textului
                activeChildForm.AdaugaTextPeImagine(width, height, fontName, fontSize, textColor, boldActivated, italicActivated);
                activeChildForm.EnablePictureBoxMouseDown(true);
                activeChildForm.EnablePictureBoxMouseMove(true);
                activeChildForm.EnablePictureBoxMouseUp(true);
            }
        }

        private void button_color_txt_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            if (color.ShowDialog() == DialogResult.OK)
            {
                button_color_txt.BackColor = color.Color;              
            }
        }


        private void button_bold_Click(object sender, EventArgs e)
        {
            boldActivated = !boldActivated;
            button_bold.BackColor = boldActivated ? Color.Gray : SystemColors.Control; 
        }

        private void button_italic_Click(object sender, EventArgs e)
        {
            italicActivated = !italicActivated;
            button_italic.BackColor = italicActivated ? Color.Gray : SystemColors.Control;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                // Trimite evenimentul de la fereastra părinte către fereastra copil
                activeChildForm.Photo_KeyPress(sender, e);
            }
        }

        private void button_cancel_add_txt_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                activeChildForm.RemoveLastTextAdded();
            }
        }


        public static Bitmap InpaintingTelea(Bitmap image, int radius)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);
            int width = image.Width;
            int height = image.Height;

            // Parcurgem fiecare pixel din imagine
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Suma și numărul de pixeli vecini pentru a calcula noua valoare a pixelului
                    int sumR = 0, sumG = 0, sumB = 0, count = 0;

                    // Parcurgem o fereastră cu raza data în jurul fiecărui pixel
                    for (int ky = -radius; ky <= radius; ky++)
                    {
                        for (int kx = -radius; kx <= radius; kx++)
                        {
                            // Coordonatele noului pixel
                            int nx = x + kx;
                            int ny = y + ky;

                            // Verificăm dacă noul pixel este în interiorul imaginii
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                Color color = image.GetPixel(nx, ny);
                                sumR += color.R;
                                sumG += color.G;
                                sumB += color.B;
                                count++;
                            }
                        }
                    }

                    // Calculăm noua valoare a pixelului bazată pe media valorilor pixelilor din vecinătate
                    Color newColor = Color.FromArgb(sumR / count, sumG / count, sumB / count);
                    result.SetPixel(x, y, newColor);
                }
            }

            return result;
        }

        public void AplicaInpaintingTelea(int radius)
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);

                    Bitmap imagineCuInpainting = InpaintingTelea(imagineInitiala, radius);
                    activeChildForm.pictureBox_photo.Image = imagineCuInpainting;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Telea's Inpainting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void TeleasAlgorithmButton_Click(object sender, EventArgs e)
        {
            
        }


        // MedianFilter

        public static Bitmap MedianFilter(Bitmap originalImage, int radius)
        {
            Bitmap imagineFiltrata = new Bitmap(originalImage.Width, originalImage.Height);
            int width = originalImage.Width;
            int height = originalImage.Height;

            // Blocăm datele imaginii originale și imaginii filtrate în memorie
            BitmapData originalData = originalImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData filteredData = imagineFiltrata.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = originalData.Stride;
            IntPtr scan0 = originalData.Scan0;
            IntPtr scan1 = filteredData.Scan0;

            unsafe
            {
                byte* ptrOriginal = (byte*)scan0.ToPointer();
                byte* ptrFiltered = (byte*)scan1.ToPointer();

                Parallel.For(0, height, y =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        // List pentru a stoca valorile pixelilor din vecinătate
                        int[] neighborhoodR = new int[(2 * radius + 1) * (2 * radius + 1)];
                        int[] neighborhoodG = new int[(2 * radius + 1) * (2 * radius + 1)];
                        int[] neighborhoodB = new int[(2 * radius + 1) * (2 * radius + 1)];

                        int count = 0;

                        // Parcurgem o fereastră cu raza data în jurul fiecărui pixel
                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                // Coordonatele noului pixel
                                int nx = x + kx;
                                int ny = y + ky;

                                // Verificăm dacă noul pixel este în interiorul imaginii
                                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                                {
                                    byte* pixel = ptrOriginal + ny * stride + nx * 3;
                                    neighborhoodR[count] = pixel[2];
                                    neighborhoodG[count] = pixel[1];
                                    neighborhoodB[count] = pixel[0];
                                    count++;
                                }
                            }
                        }

                        // Calculăm valorile mediane pentru fiecare canal de culoare
                        Array.Sort(neighborhoodR, 0, count);
                        Array.Sort(neighborhoodG, 0, count);
                        Array.Sort(neighborhoodB, 0, count);

                        int medianR = neighborhoodR[count / 2];
                        int medianG = neighborhoodG[count / 2];
                        int medianB = neighborhoodB[count / 2];

                        // Setăm noua valoare a pixelului
                        byte* filteredPixel = ptrFiltered + y * stride + x * 3;
                        filteredPixel[2] = (byte)medianR;
                        filteredPixel[1] = (byte)medianG;
                        filteredPixel[0] = (byte)medianB;
                    }
                });
            }

            // Deblocăm datele imaginii
            originalImage.UnlockBits(originalData);
            imagineFiltrata.UnlockBits(filteredData);

            return imagineFiltrata;
        }

        public void AplicaMedianFilter(int radius)
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);


                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuSharpen = MedianFilter(imagineInitiala, radius);
                    activeChildForm.pictureBox_photo.Image = imagineCuSharpen;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Median Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FiltruMedianButton_Click(object sender, EventArgs e)
        {
            int radius = int.Parse(textBox_radius_median.Text);
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = MedianFilter(imagineInitiala, radius);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply HDR Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaMedianFilter(radius);
            }
        }


        //Lucy-Richardson Deconvolution
        public static Bitmap LucyRichardsonDeconvolution(Bitmap originalImage, double[,] psf, int iterations)
        {
            int width = originalImage.Width;
            int height = originalImage.Height;
            Bitmap result = new Bitmap(width, height);

            BitmapData originalData = originalImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int bytesPerPixel = Image.GetPixelFormatSize(originalImage.PixelFormat) / 8;
            int byteCount = originalData.Stride * originalData.Height;
            byte[] originalPixels = new byte[byteCount];
            byte[] resultPixels = new byte[byteCount];

            Marshal.Copy(originalData.Scan0, originalPixels, 0, byteCount);
            Marshal.Copy(resultData.Scan0, resultPixels, 0, byteCount);

            int stride = originalData.Stride;

            double[,] original = new double[height, width];
            double[,] resultImage = new double[height, width];
            double[,] estimate = new double[height, width];

            // Convertirea imaginii în tonuri de gri
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * stride) + (x * bytesPerPixel);
                    if (pixelIndex + 2 < originalPixels.Length)
                    {
                        double gray = (originalPixels[pixelIndex] + originalPixels[pixelIndex + 1] + originalPixels[pixelIndex + 2]) / 3.0;
                        original[y, x] = gray;
                        estimate[y, x] = gray;
                    }
                }
            }

            for (int iter = 0; iter < iterations; iter++)
            {
                // Convoluția estimării cu PSF
                var blurred = Convolve(estimate, psf);

                // Calcularea raportului
                var ratio = new double[height, width];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        ratio[y, x] = original[y, x] / Math.Max(blurred[y, x], 1e-8);
                    }
                }

                // Convoluția raportului cu PSF-ul inversat
                var correction = Convolve(ratio, FlipPSF(psf));

                // Actualizarea estimării
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        estimate[y, x] *= correction[y, x];
                    }
                }
            }

            // Convertirea rezultatului în array de bytes
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * stride) + (x * bytesPerPixel);
                    byte gray = (byte)Math.Min(255, Math.Max(0, estimate[y, x]));
                    if (pixelIndex + 2 < resultPixels.Length)
                    {
                        resultPixels[pixelIndex] = gray;
                        resultPixels[pixelIndex + 1] = gray;
                        resultPixels[pixelIndex + 2] = gray;
                    }
                }
            }

            Marshal.Copy(resultPixels, 0, resultData.Scan0, byteCount);
            originalImage.UnlockBits(originalData);
            result.UnlockBits(resultData);

            return result;
        }

        private static double[,] Convolve(double[,] image, double[,] kernel)
        {
            int height = image.GetLength(0);
            int width = image.GetLength(1);
            int kHeight = kernel.GetLength(0);
            int kWidth = kernel.GetLength(1);
            double[,] result = new double[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sum = 0.0;
                    for (int ky = 0; ky < kHeight; ky++)
                    {
                        for (int kx = 0; kx < kWidth; kx++)
                        {
                            int ix = x + kx - kWidth / 2;
                            int iy = y + ky - kHeight / 2;
                            if (ix >= 0 && ix < width && iy >= 0 && iy < height)
                            {
                                sum += image[iy, ix] * kernel[ky, kx];
                            }
                        }
                    }
                    result[y, x] = sum;
                }
            }

            return result;
        }

        private static double[,] FlipPSF(double[,] psf)
        {
            int height = psf.GetLength(0);
            int width = psf.GetLength(1);
            double[,] result = new double[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[y, x] = psf[height - y - 1, width - x - 1];
                }
            }

            return result;
        }

        public void AplicaLucyRichardsonDeconvolution(double[,] psf, int iterations)
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);


                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuSharpen = LucyRichardsonDeconvolution(imagineInitiala, psf, iterations);
                    activeChildForm.pictureBox_photo.Image = imagineCuSharpen;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply Lucy Richardson Deconvolution.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FiltruNLMButton_Click(object sender, EventArgs e)
        {
            //PSF Gaussian:
            double[,] psf = new double[,]
                {
                { 1, 4, 6, 4, 1 },
                { 4, 16, 24, 16, 4 },
                { 6, 24, 36, 24, 6 },
                { 4, 16, 24, 16, 4 },
                { 1, 4, 6, 4, 1 }
            };

            //PSF pentru blur de mișcare:
            /*
            double[,] psf = new double[,]
            {
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 }
            };*/
            int iterations = 50;
           
            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = LucyRichardsonDeconvolution(imagineInitiala, psf, iterations);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply HDR Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaLucyRichardsonDeconvolution(psf, iterations);
            }
        }


        // High-Pass Filter

        public static Bitmap HighPassFilter(Bitmap originalImage, int radius)
        {
            Bitmap blurredImage = GaussianBlur(originalImage, radius);
            Bitmap highPassImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color originalColor = originalImage.GetPixel(x, y);
                    Color blurredColor = blurredImage.GetPixel(x, y);

                    // Calculăm diferența dintre pixelul original și cel neclar, cu offset de 128
                    int r = Math.Min(Math.Max(originalColor.R - blurredColor.R + 128, 0), 255);
                    int g = Math.Min(Math.Max(originalColor.G - blurredColor.G + 128, 0), 255);
                    int b = Math.Min(Math.Max(originalColor.B - blurredColor.B + 128, 0), 255);

                    highPassImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            // Combinăm imaginea high-pass cu imaginea originală pentru a menține culorile originale
            Bitmap resultImage = new Bitmap(originalImage.Width, originalImage.Height);
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color originalColor = originalImage.GetPixel(x, y);
                    Color highPassColor = highPassImage.GetPixel(x, y);

                    int r = Math.Min(Math.Max(originalColor.R + highPassColor.R - 128, 0), 255);
                    int g = Math.Min(Math.Max(originalColor.G + highPassColor.G - 128, 0), 255);
                    int b = Math.Min(Math.Max(originalColor.B + highPassColor.B - 128, 0), 255);

                    resultImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return resultImage;
        }

        private static Bitmap GaussianBlur(Bitmap image, int radius)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // Convertim imaginea în byte array pentru procesare mai rapidă
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, blurred.Width, blurred.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = imageData.Stride;
            int bytes = stride * image.Height;
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            Marshal.Copy(imageData.Scan0, pixelBuffer, 0, bytes);
            image.UnlockBits(imageData);

            double[,] kernel = CreateGaussianKernel(radius, out double kernelSum);

            // Aplicăm filtrul Gaussian
            for (int y = radius; y < image.Height - radius; y++)
            {
                for (int x = radius; x < image.Width - radius; x++)
                {
                    double blue = 0.0, green = 0.0, red = 0.0;

                    for (int fy = -radius; fy <= radius; fy++)
                    {
                        for (int fx = -radius; fx <= radius; fx++)
                        {
                            int imageX = (x + fx + image.Width) % image.Width;
                            int imageY = (y + fy + image.Height) % image.Height;

                            int imageIndex = (imageY * stride) + (imageX * 3);

                            blue += (double)(pixelBuffer[imageIndex]) * kernel[fy + radius, fx + radius];
                            green += (double)(pixelBuffer[imageIndex + 1]) * kernel[fy + radius, fx + radius];
                            red += (double)(pixelBuffer[imageIndex + 2]) * kernel[fy + radius, fx + radius];
                        }
                    }

                    int resultIndex = (y * stride) + (x * 3);

                    resultBuffer[resultIndex] = (byte)(blue / kernelSum);
                    resultBuffer[resultIndex + 1] = (byte)(green / kernelSum);
                    resultBuffer[resultIndex + 2] = (byte)(red / kernelSum);
                }
            }

            Marshal.Copy(resultBuffer, 0, blurredData.Scan0, bytes);
            blurred.UnlockBits(blurredData);

            return blurred;
        }

        private static double[,] CreateGaussianKernel(int radius, out double kernelSum)
        {
            int size = 2 * radius + 1;
            double[,] kernel = new double[size, size];
            double sigma = radius / 3.0;
            double sigma2 = sigma * sigma;
            double piSigma2 = 2.0 * Math.PI * sigma2;
            double sqrtPiSigma2 = 1.0 / Math.Sqrt(piSigma2);
            double total = 0.0;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    double distance = (x * x + y * y) / (2 * sigma2);
                    kernel[y + radius, x + radius] = sqrtPiSigma2 * Math.Exp(-distance);
                    total += kernel[y + radius, x + radius];
                }
            }

            kernelSum = total;
            return kernel;
        }

        public void AplicaHighPassFilter(int radius)
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);


                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuSharpen = HighPassFilter(imagineInitiala, radius);
                    activeChildForm.pictureBox_photo.Image = imagineCuSharpen;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply High-Pass Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FiltruHighPassButton_Click(object sender, EventArgs e)
        {
            int radius = 5;

            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = HighPassFilter(imagineInitiala, radius);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply HDR Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaHighPassFilter(radius);
            }
        }


        // Unsharp Masking
        public static Bitmap UnsharpMask(Bitmap originalImage, double amount, int radius, double threshold)
        {
            Bitmap blurredImage = GaussianBlur(originalImage, radius);
            Bitmap resultImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color originalColor = originalImage.GetPixel(x, y);
                    Color blurredColor = blurredImage.GetPixel(x, y);

                    int diffR = originalColor.R - blurredColor.R;
                    int diffG = originalColor.G - blurredColor.G;
                    int diffB = originalColor.B - blurredColor.B;

                    if (Math.Abs(diffR) >= threshold)
                        diffR = (int)(amount * diffR);
                    else
                        diffR = 0;

                    if (Math.Abs(diffG) >= threshold)
                        diffG = (int)(amount * diffG);
                    else
                        diffG = 0;

                    if (Math.Abs(diffB) >= threshold)
                        diffB = (int)(amount * diffB);
                    else
                        diffB = 0;

                    int r = Math.Min(Math.Max(originalColor.R + diffR, 0), 255);
                    int g = Math.Min(Math.Max(originalColor.G + diffG, 0), 255);
                    int b = Math.Min(Math.Max(originalColor.B + diffB, 0), 255);

                    resultImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return resultImage;
        }

        public void AplicaUnsharpMask(double amount, int radius, double threshold)
        {
            if (ActiveMdiChild is Photo activeChildForm)
            {
                if (activeChildForm.pictureBox_photo.Image != null)
                {
                    // Save the original image associated with the active form
                    Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                    imaginiAnterioare[activeChildForm] = new Bitmap(imagineInitiala);


                    // Apply the sepia filter and update the image in the PictureBox
                    Bitmap imagineCuSharpen = UnsharpMask(imagineInitiala, amount,radius,threshold);
                    activeChildForm.pictureBox_photo.Image = imagineCuSharpen;
                }
                else
                {
                    MessageBox.Show("No image in the PictureBox to apply UnsharpMask.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UnsharpMaskButton_Click(object sender, EventArgs e)
        {
            double amount = 0.5; 
            int radius = 1; 
            double threshold = 10;

            if (!workInCurrentForm)
            {
                if (ActiveMdiChild is Photo activeChildForm)
                {
                    if (activeChildForm.pictureBox_photo.Image != null)
                    {
                        Bitmap imagineInitiala = new Bitmap(activeChildForm.pictureBox_photo.Image);
                        Bitmap imagineCuClarendo = UnsharpMask(imagineInitiala, amount, radius, threshold);

                        Photo newChildForm = new Photo();
                        newChildForm.MdiParent = this;
                        newChildForm.Show();

                        newChildForm.pictureBox_photo.Image = imagineCuClarendo;

                        if (!darknessValues.ContainsKey(newChildForm))
                        {
                            darknessValues[newChildForm] = 0;
                        }
                        if (!exposureValues.ContainsKey(newChildForm))
                        {
                            exposureValues[newChildForm] = 0;
                        }
                        if (!contrastValues.ContainsKey(newChildForm))
                        {
                            contrastValues[newChildForm] = 0;
                        }
                        if (!highlightsValues.ContainsKey(newChildForm))
                        {
                            highlightsValues[newChildForm] = 0;
                        }
                        if (!shadowsValues.ContainsKey(newChildForm))
                        {
                            shadowsValues[newChildForm] = 0;
                        }
                        if (!saturationValues.ContainsKey(newChildForm))
                        {
                            saturationValues[newChildForm] = 0;
                        }
                        if (!tempValues.ContainsKey(newChildForm))
                        {
                            tempValues[newChildForm] = 0;
                        }

                        //  Applies image resizing in the new Photo form
                        if (newChildForm.pictureBox_photo.Image != null)
                        {
                            if (newChildForm.pictureBox_photo.Image.Width > newChildForm.ClientSize.Width || newChildForm.pictureBox_photo.Image.Height > newChildForm.ClientSize.Height)
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.Zoom;
                                newChildForm.pictureBox_photo.Width = newChildForm.ClientSize.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.ClientSize.Height;
                            }
                            else
                            {
                                newChildForm.pictureBox_photo.SizeMode = PictureBoxSizeMode.AutoSize;
                                newChildForm.pictureBox_photo.Width = newChildForm.pictureBox_photo.Image.Width;
                                newChildForm.pictureBox_photo.Height = newChildForm.pictureBox_photo.Image.Height;
                            }

                            newChildForm.pictureBox_photo.Left = (newChildForm.ClientSize.Width - newChildForm.pictureBox_photo.Width) / 2;
                            newChildForm.pictureBox_photo.Top = (newChildForm.ClientSize.Height - newChildForm.pictureBox_photo.Height) / 2;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image in the PictureBox to apply HDR Filter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                AplicaUnsharpMask(amount, radius, threshold);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string aboutMessage = "Aplicație pentru prelucrarea de fotografii\n" +
                            "Autor: Filip Felix-Constantin\n" +
                            "Anul: 2024\n\n" +
                            "Caracteristici:\n" +
                            "- Preseturi și Filtre: Aplică rapid efecte artistice sau ajustări predefinite\n" +
                            "- Funcții Avansate de Transformare: Rotire și translație cu control precis\n" +
                            "- Editare Manuală Detaliată: Reglează luminozitatea, saturația și altele pentru fiecare detaliu\n" +
                            "- Interfață Inspirată de Paint: Acces ușor și intuitiv la unelte și funcții\n" +
                            "- Adăugare de Text Personalizat: Integrează mesaje personalizate în imagini\n" +
                            "- Instrumente de Restaurare: Repară imperfecțiunile și restaurează aspectul original al imaginilor";

            MessageBox.Show(aboutMessage, "Despre aplicație", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

}
