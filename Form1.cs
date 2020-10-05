using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FileExplorer
{
    public partial class Form1 : Form
    {
        DirectoryInfo root;
        FileSystemWatcher watcher;
        int operation;
        TreeNode currentNode;

        delegate void D();
        public Form1()
        {
            InitializeComponent();
            root = new DirectoryInfo(@"D:\code\c#\FileExplorer\Папка");
            //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            treeView1.ImageList = imageList1;
            SetTreeNode();
            SetWatcher();
            ResetButtons();


        }
        void SetWatcher()
        {
            watcher = new FileSystemWatcher(root.FullName);
            watcher.Changed += Handler;
            watcher.Renamed += Handler;
            watcher.Deleted += Handler;
            watcher.Changed += Handler;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        void Handler(object o, FileSystemEventArgs e)
        {
            treeView1.Invoke(new D(SetTreeNode));
        }
        void SetTreeNode()
        {
            treeView1.Nodes.Clear();
            var rootNode = new TreeNode("", 0, 0);
            rootNode.Tag = root.FullName;
            AddBranch(rootNode, root);
            treeView1.Nodes.Add(rootNode);
            treeView1.ExpandAll();
            treeView1.SelectedNode = null;
            
        }
        void AddBranch(TreeNode node, DirectoryInfo dir)
        {
            var dirNode = new TreeNode(dir.Name, 0, 0);
            dirNode.Tag = dir.FullName;
            var dirs = dir.GetDirectories();
            foreach(var d in dirs)
            {
                AddBranch(dirNode, d);
            }
            var files = dir.GetFiles();
            foreach(var f in files)
            {
                var fileNode = new TreeNode(f.Name, 1, 1);
                fileNode.Tag = f.FullName;
                if(Path.GetExtension(f.Name) == ".zip")
                {
                    fileNode.ImageIndex = 2;
                    fileNode.SelectedImageIndex = 2;
                }
                    
                dirNode.Nodes.Add(fileNode);
            }
            node.Nodes.Add(dirNode);
        }
     
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = treeView1.SelectedNode;
            buttonOK.Enabled = false;
            buttonRename.Enabled = true;
            buttonDelete.Enabled = true;
            buttonArchive.Enabled = true;
            if (node.ImageIndex == 0)
            {
                buttonOpen.Enabled = false;
                buttonDearchive.Enabled = false;
                buttonCopy.Enabled = false;
                buttonCreateFile.Enabled = true;
                buttonCreateFolder.Enabled = true;
            }
            if (node.ImageIndex == 1)
            {
                buttonOpen.Enabled = true;
                buttonDearchive.Enabled = false;
                buttonCopy.Enabled = true;
                buttonCreateFile.Enabled = false;
                buttonCreateFolder.Enabled = false;
            }
            if (node.ImageIndex == 2)
            {
                buttonOpen.Enabled = false;
                buttonDearchive.Enabled = true;
                buttonCopy.Enabled = true;
                buttonCreateFile.Enabled = false;
                buttonCreateFolder.Enabled = false;
            }
           // currentNode = node;
        }
        void ResetButtons()
        {
            buttonOK.Enabled = false;
            buttonRename.Enabled = false;
            buttonDelete.Enabled = false;
            buttonArchive.Enabled = false;
            buttonOpen.Enabled = false;
            buttonDearchive.Enabled = false;
            buttonCopy.Enabled = false;
            buttonCreateFile.Enabled = false;
            buttonCreateFolder.Enabled = false;
            treeView1.SelectedNode = null;
            operation = 0;
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            //if (node == null)
              //  return;
            var fullName = (string)node.Tag;
            var name = Path.GetFileName(fullName);
            var extension = Path.GetExtension(name);
            var dir = Path.GetDirectoryName(fullName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(name);
            //var fullNameWithoutExtension = Path.GetFileNameWithoutExtension(fullName);
            File.Copy(fullName, Path.Combine(dir,nameWithoutExtension + "(1)" + extension), true);
            ResetButtons();

        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            currentNode = node;
            if(node.ImageIndex == 0)
            {
                textBox1.Text = node.Text;
            }
            else
            {
                textBox1.Text = Path.GetFileNameWithoutExtension(node.Text);
            }
            ResetButtons();
            operation = 1;
            buttonOK.Enabled = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if(operation == 1)
            {
                var oldName = (string)currentNode.Tag;
                var newName = textBox1.Text;
                var dir = Path.GetDirectoryName(oldName);
                try
                {
                    if (currentNode.ImageIndex == 0)
                    {
                        var fullName = Path.Combine(dir, newName);
                        //if (ValidFullName(fullName))
                            Directory.Move(oldName, fullName);
                    }
                    else
                    {
                        var extension = Path.GetExtension(oldName);
                        var fullName = Path.Combine(dir, newName + extension);
                        while (File.Exists(fullName))
                        {
                            newName += "(1)";
                            fullName = Path.Combine(dir, newName + extension);
                        }
                        //if (ValidFullName(fullName))
                            File.Move(oldName, fullName);

                    }
                }
                catch(Exception exeption)
                { }
                
            }
            ResetButtons();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var node = tree
        }
        //private bool ValidFullName(string name)
        //{
        //    if (name.Length >= 256)
        //        return false;
        //    return true;
        //}
        //private bool ValidName(string name)
        //{
        //    if (name.Length == 0)
        //        return false;
        //    if (name == "CON" || name == "PRN" || name == "AUX" || name == "NUL" || name == "COM1" || name == "COM2" || name == "COM3" ||
        //        name == "COM4" || name == "COM5" || name == "COM6" || name == "COM7" || name == "COM8" || name == "COM9" || name == "LPT1" ||
        //        name == "LPT2" || name == "LPT3" || name == "LPT4" || name == "LPT5" || name == "LPT6" || name == "LPT7" || name == "LPT8" || name == "LPT9")
        //        return false;
        //    if (name.Contains(">") || name.Contains("<") || name.Contains(":") || name.Contains("\"") || name.Contains("\\") || name.Contains("/")
        //        || name.Contains("|") || name.Contains("?") || name.Contains("*") || name.Contains("\n") || name.Contains("\t"))
        //        return false;
        //    if(name[name.Length-1] == ' ' || name[name.Length - 1] == '.')
        //        return false;
        //    return true;
        //}
    }
}
