using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Org.BouncyCastle.Cms;

namespace P7MExtractor;

public partial class Form1 : Form
{
    private Label lblStatus;
    private ProgressBar progressBar;
    private LinkLabel linkWebsite;
    private LinkLabel linkSupport;
    private string[] _initialArgs;

    public Form1(string[] args)
    {
        InitializeComponent();
        _initialArgs = args;
        SetupCustomUI();
    }

    private void SetupCustomUI()
    {
        this.Text = "Bave P7M - Lettore file con firma digitale P7M";
        this.Size = new Size(600, 400);
        this.AllowDrop = true;
        this.DragEnter += Form1_DragEnter;
        this.DragDrop += Form1_DragDrop;
        this.Shown += Form1_Shown;

        lblStatus = new Label();
        lblStatus.Text = "Trascina i file .p7m qui \r\n Saranno estratti in cartella non firmati";
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        lblStatus.Dock = DockStyle.Fill;
        lblStatus.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
        this.Controls.Add(lblStatus);

        progressBar = new ProgressBar();
        progressBar.Dock = DockStyle.Bottom;
        progressBar.Height = 20;
        progressBar.Visible = false;
        this.Controls.Add(progressBar);

        // Create a panel to hold the links at the bottom right
        var linkPanel = new FlowLayoutPanel();
        linkPanel.FlowDirection = FlowDirection.RightToLeft;
        linkPanel.Dock = DockStyle.Bottom;
        linkPanel.Height = 25;
        linkPanel.Padding = new Padding(5);
        this.Controls.Add(linkPanel);

        // Support link
        linkSupport = new LinkLabel();
        linkSupport.Text = "Support me ☕";
        linkSupport.AutoSize = true;
        linkSupport.LinkClicked += (s, e) => OpenUrl("https://ko-fi.com/bave_");
        linkPanel.Controls.Add(linkSupport);

        // Website link
        linkWebsite = new LinkLabel();
        linkWebsite.Text = "www.bave.info";
        linkWebsite.AutoSize = true;
        linkWebsite.LinkClicked += (s, e) => OpenUrl("https://www.bave.info");
        linkPanel.Controls.Add(linkWebsite);
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Form1_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void Form1_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[]? files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                ProcessFiles(files);
            }
        }
    }

    private void Form1_Shown(object? sender, EventArgs e)
    {
        if (_initialArgs != null && _initialArgs.Length > 0)
        {
            ProcessFiles(_initialArgs);
        }
    }

    private async void ProcessFiles(string[] files)
    {
        var p7mFiles = files.Where(f => Path.GetExtension(f).Equals(".p7m", StringComparison.OrdinalIgnoreCase)).ToArray();

        if (p7mFiles.Length == 0) return;

        progressBar.Visible = true;
        progressBar.Value = 0;
        progressBar.Maximum = p7mFiles.Length;
        lblStatus.Text = "Processing...";

        await Task.Run(() =>
        {
            int count = 0;
            foreach (var file in p7mFiles)
            {
                try
                {
                    ExtractP7M(file);
                }
                catch (Exception ex)
                {
                    // Ignore errors as per "dont ask confirmation or other" 
                    // but maybe we should log? For now just continue.
                    System.Diagnostics.Debug.WriteLine($"Error extracting {file}: {ex.Message}");
                }

                count++;
                this.Invoke(() =>
                {
                    progressBar.Value = count;
                    lblStatus.Text = $"Processed {count}/{p7mFiles.Length}";
                });
            }
        });

        lblStatus.Text = "Done! Drop files here";
        await Task.Delay(2000);
        progressBar.Visible = false;
        progressBar.Value = 0;
        lblStatus.Text = "Drop .p7m files here";
    }

    private void ExtractP7M(string inputFile)
    {
        try
        {
            byte[] fileData = File.ReadAllBytes(inputFile);
            var cmsSignedData = new Org.BouncyCastle.Cms.CmsSignedData(fileData);

            // Extract content
            var signedContent = cmsSignedData.SignedContent;
            if (signedContent == null) return; // No content

            byte[] content;
            using (var ms = new MemoryStream())
            {
                signedContent.Write(ms);
                content = ms.ToArray();
            }

            // Determine output path
            string inputDir = Path.GetDirectoryName(inputFile) ?? string.Empty;
            string outputDir = Path.Combine(inputDir, "non firmati");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Try to get original filename, otherwise strip .p7m
            string outputFilename = Path.GetFileNameWithoutExtension(inputFile);

            // BouncyCastle doesn't always easily give the original filename from the CMS blob 
            // without parsing the ContentInfo more deeply or if it's not there.
            // The requirement says "save the extracted files...". 
            // Usually p7m wraps the file. If the inner content has no name metadata easily accessible, 
            // we use the outer name minus .p7m.

            string outputPath = Path.Combine(outputDir, outputFilename);
            File.WriteAllBytes(outputPath, content);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
