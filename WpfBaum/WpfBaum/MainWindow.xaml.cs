using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace WpfBaum
{
    public partial class MainWindow : Window
    {
        public class TreeViewItemModel
        {
            public string Text { get; set; }
            public ObservableCollection<TreeViewItemModel> Children { get; set; }

            public TreeViewItemModel()
            {
                Children = new ObservableCollection<TreeViewItemModel>();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Hier die Methode einsetzen, um aus Ihrem ImgTree-Objekt ein passendes Datenmodell zu erstellen
            ImgTree imgTree = new ImgTree();
            imgTree.InsertItem("Bär", 1);
            imgTree.InsertItem("Bär", 2);
            imgTree.InsertItem("Bär", 3);
            imgTree.InsertItem("Bär fotorealistisch", 4);
            imgTree.SelectNode(0);
            imgTree.InsertItem("Bär", 5);
            imgTree.SelectNode(1);
            imgTree.InsertItem("Bär", 6);
            imgTree.InsertItem("Bär", 7);
            imgTree.InsertItem("Bär fotorealistisch", 8);

            // Hier wird die Methode PopulateTreeView verwendet, um die TreeView zu füllen
            PopulateTreeView(imgTree);
        }

        //private ImgTree CreateImgTree()
        //{
        //    // Hier den ImgTree mit den entsprechenden Daten erstellen
        //    ImgTree imgTree = new ImgTree();


        //    imgTree.InsertItem("Bär", 1);
        //    imgTree.InsertItem("Bär", 2);
        //    imgTree.InsertItem("Bär", 3);
        //    imgTree.InsertItem("Bär fotorealistisch", 4);
        //    imgTree.SelectNode(0);
        //    imgTree.InsertItem("Bär", 5);
        //    imgTree.SelectNode(1);
        //    imgTree.InsertItem("Bär", 6);
        //    imgTree.InsertItem("Bär", 7);
        //    imgTree.InsertItem("Bär fotorealistisch", 8);

        //    PopulateTreeView();


        //    // Fügen Sie Elemente zum ImgTree hinzu, wie im vorherigen Beispiel gezeigt
        //    return imgTree;
        //}

        private void PopulateTreeView(ImgTree imgTree)
        {
            treeView.Items.Clear();

            List<ImgTree.ImgNode> addedNodes = new List<ImgTree.ImgNode>();

            foreach (var node in imgTree.nodeList)
            {
                if (!addedNodes.Contains(node))
                {
                    TreeViewItem treeNode = CreateTreeViewItem(node, addedNodes);
                    treeView.Items.Add(treeNode);
                }
            }
        }

        private TreeViewItem CreateTreeViewItem(ImgTree.ImgNode node, List<ImgTree.ImgNode> addedNodes)
        {
            TreeViewItem treeNode = new TreeViewItem { Header = $"Node {node.nodeId}" };

            addedNodes.Add(node);

            foreach (var item in node.itemList)
            {
                treeNode.Items.Add(new TreeViewItem { Header = $"Item {item.id}" });
            }

            foreach (var childNode in node.children)
            {
                TreeViewItem childTreeNode = CreateTreeViewItem(childNode, addedNodes);
                treeNode.Items.Add(childTreeNode);
            }

            return treeNode;
        }

        private ObservableCollection<TreeViewItemModel> CreateTreeViewData(ImgTree imgTree)
        {
            ObservableCollection<TreeViewItemModel> treeViewData = new ObservableCollection<TreeViewItemModel>();

            // Hier die Methode einsetzen, um aus Ihrem ImgTree-Objekt ein passendes Datenmodell zu erstellen
            // Sie müssen durch die Hierarchie des ImgTree navigieren und TreeViewItemModel-Objekte erstellen
            // Hier ein Platzhalter-Code:
            foreach (var imgNode in imgTree.nodeList)
            {
                TreeViewItemModel nodeModel = new TreeViewItemModel { Text = $"Node {imgNode.nodeId}" };
                foreach (var item in imgNode.itemList)
                {
                    nodeModel.Children.Add(new TreeViewItemModel { Text = $"Item {item.id}" });
                }
                treeViewData.Add(nodeModel);
            }

            return treeViewData;
        }
    }
}
