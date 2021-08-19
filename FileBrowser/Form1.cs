using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using EmbeddedFS;

namespace FileBrowser
{
    public partial class Form1 : Form
    {
        public List<EmFileInfo> Files { get; set; }
        public List<EmDirectoryInfo> Directories { get; set; }

        private EmDriveInfo efs = new EmDriveInfo(@"c:\temp\drive2.efs");
        private EmDirectoryInfo directory;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            directory = efs.RootDirectory;
            EmFileInfo[] l = directory.GetFiles();
            this.FileDGV.Columns["SizeColumn"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            UpdateData();
            PopulateTreeView();
            lblCurrentFS.Text = this.efs.FullName;
        }


        private void PopulateTreeView()
        {
            TreeNode rootNode;
            EmDirectoryInfo info = efs.RootDirectory;
            if (info.Exists)
            {
                treeView1.Nodes.Clear();

                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }



        private void UpdateData()
        {
            UrlTextBox.Text = directory.FullName;

            Files = directory.GetFiles().ToList(); // efs.GetAllFiles();// 
            Directories = directory.GetDirectories().ToList();

            // remove all current items
            FileDGV.BeginInvoke((Action)(() =>
            {
                FileDGV.Rows.Clear();
            }));

            // put all the directories in first
            foreach (EmDirectoryInfo d in Directories)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(FileDGV);

                row.Cells[0].Value = imageList1.Images["folder"];

                // add values to each column
                row.Cells[1].Value = d.Name;
                row.Cells[2].Value = d.LastWriteTimeUtc.ToString();
                row.Cells[3].Value = "File Folder";

                row.Tag = d;

                FileDGV.BeginInvoke((Action)(() =>
                {
                    FileDGV.Rows.Add(row);
                }));

            }

            // put in all files next
            foreach (EmFileInfo f in Files)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(FileDGV);

                row.Cells[0].Value = imageList1.Images["file"];

                // add values to each column
                row.Cells[1].Value = f.Name;
                row.Cells[2].Value = f.LastWriteTimeUtc.ToString();
                row.Cells[3].Value = f.Extension.ToString();
                row.Cells[4].Value = f.Length;

                row.Tag = f;

                FileDGV.BeginInvoke((Action)(() =>
                {
                    FileDGV.Rows.Add(row);
                }));
            }

            // update the dgv
            FileDGV.BeginInvoke((Action)(() =>
            {
                FileDGV.Refresh();
            }));

        }


        //
        // Source: https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/creating-an-explorer-style-interface-with-the-listview-and-treeview?view=netframeworkdesktop-4.8
        //
        private void GetDirectories(EmDirectoryInfo[] subDirs,
            TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            EmDirectoryInfo[] subSubDirs;
            foreach (EmDirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }



        private void NewFolderButton_Click(object sender, EventArgs e)
        {
            // create the new folder and update the interface
            directory.CreateSubdirectory(txtNewFolder.Text);
            UpdateData();
            PopulateTreeView();
        }



        private void PasteButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            
            if (dialog.ShowDialog().Equals(DialogResult.OK))
            {
                string copiedItemPath = dialog.FileName;

                string fileName = Path.GetFileName(copiedItemPath);

                EmFileInfo file = new EmFileInfo(efs, Path.Combine(directory.FullName, fileName));
                using (Stream s = file.OpenWrite())
                {
                    byte[] data = File.ReadAllBytes(copiedItemPath);
                    s.Write(data, 0, data.Length);
                }

                UpdateData();
            }
        }



        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (FileDGV.RowCount <= 0)
                return;

            // get the name of the selected item
            if (FileDGV.SelectedRows[0].Tag is EmFileInfo)
            {
                EmFileInfo file = FileDGV.SelectedRows[0].Tag as EmFileInfo;
                file.Delete();
            }
            else
            {
                EmDirectoryInfo dir = FileDGV.SelectedRows[0].Tag as EmDirectoryInfo;
                dir.Delete();
            }

            UpdateData();
            PopulateTreeView();
        }

        private void FileDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FileDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (FileDGV.RowCount <= 0)
                return;

            // get the name of the double clicked item
            string itemName;
            try
            {
                itemName = FileDGV.SelectedRows[0].Cells[1].Value.ToString();
            }
            catch
            {
                return;
            }

            if (itemName.EndsWith(@"\"))
            {
                directory = new EmDirectoryInfo(efs, Path.Combine(UrlTextBox.Text, itemName));
                UpdateData();
            }

        }

        private void UpDirectoryButton_Click(object sender, EventArgs e)
        {
            if (directory.Parent != null)
            { 
                directory = directory.Parent;

                UpdateData();
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            this.directory = new EmDirectoryInfo(efs, newSelected.Tag.ToString());

            UpdateData();
        }

        private void tbnExportFile_Click(object sender, EventArgs e)
        {
            // don't export folders
            if (FileDGV.SelectedRows[0].Tag is EmDirectoryInfo)
            {
                return;
            }

            string itemName = (FileDGV.SelectedRows[0].Tag as EmFileInfo).FullName;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = itemName;

            if (dialog.ShowDialog().Equals(DialogResult.OK))
            {
                string fileName = dialog.FileName;
                using (Stream fOut = File.OpenWrite(fileName))
                {
                    byte[] data = EmFile.ReadAllBytes(this.directory.Drive, Path.Combine(directory.FullName, itemName));
                    fOut.Write(data, 0, data.Length);
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://didisoft.com/embeddedfs/");
        }

        private void btnInMemory_Click(object sender, EventArgs e)
        {
            this.efs = new EmDriveInfo();
            this.directory = this.efs.RootDirectory;

            UpdateData();
            PopulateTreeView();
            lblCurrentFS.Text = this.efs.FullName;
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = false;
            dialog.Title = "Open or Create Single file Embedded File System";

            if (dialog.ShowDialog().Equals(DialogResult.OK))
            {
                string fileName = dialog.FileName;

                this.efs = new EmDriveInfo(fileName);
                this.directory = this.efs.RootDirectory;

                UpdateData();
                PopulateTreeView();
                lblCurrentFS.Text = this.efs.FullName;
            }

        }

    }
}
