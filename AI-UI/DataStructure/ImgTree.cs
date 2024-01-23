using System.IO;
using System;
using System.Collections.Generic;
using AI_UI;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Data_Structure {
    public static class ImgTree {

        private static ImgNode start;
        private static int countNodeId = 0;
        private static int countImgId = 0;
        private static ImgNode currentNode;// set via onClick
        private static string currentTreeName;
        private static List<ImgNode> nodes;
        public static List<ImgNode> Nodes { get => nodes; }

        /// <summary>
        /// Create New Tree
        /// </summary>
        /// <param name="name">Name of new Tree</param>
        public static void NewTree(string name) {
            Directory.CreateDirectory("\\output\\" + name);
            currentTreeName = name;
            nodes = new();
        }

        /// <summary>
        /// Load existing Tree from file
        /// </summary>
        /// <param name="sr">Stream Reader with File Opened</param>
        public static void LoadTree(StreamReader sr) {
            nodes = new();
            //Read Tree File from Selected Workspace
            string treeInfos = sr.ReadLine();
            //Parse TreeInfo
            TreeStruct treeStruct = JsonSerializer.Deserialize<TreeStruct>(treeInfos);
            countNodeId = treeStruct.CountNodeId;
            countImgId = treeStruct.CountImgId;
            currentTreeName = treeStruct.CurrentTreeName;
            string nodeInfo = sr.ReadLine();
            while(nodeInfo != null)  {
                //Parse NodeInfo
                NodeStruct nodeStruct = JsonSerializer.Deserialize<NodeStruct>(nodeInfo);
                ImgNode node = new(nodeStruct);
                nodes.Add(node);
                nodeInfo = sr.ReadLine();
            }
            sr.Close();
            foreach(ImgNode node in nodes) {
                node.FindChildren();
            }
            start = nodes[0];
            currentNode = start;
        }

        /// <summary>
        /// Insert New Node into Tree
        /// </summary>
        /// <param name="node">Node to be inserted</param>
        public static void InsertItem(ImgNode node) {
            if(start == null) { //If there are no other nodes in the tree make this one the root node
                start = node;
                currentNode = node;
            } else { //Else add this node as a child of it's predecessor
                currentNode.AddChild(node);
                currentNode = node;
            } nodes.Add(currentNode);
        }

        /// <summary>
        /// Select Node in Tree
        /// </summary>
        /// <param name="nodeId"></param>
        //public static void SelectNode(int nodeId) {
        //    currentNode = nodes[nodeId];
        //}
        // ----vorübergehend ersetzt

        
        /// <summary>
        /// Save a New Batch of Images
        /// </summary>
        /// <param name="images">Collection of Images in base64 format</param>
        /// <param name="batchInfo">Prompt Information for this Batch of Images</param>
        /// <param name="imgInfo">Collection of prompt Information for each individual image</param>
        public static void SaveBatch(string[] images, ResponseStruct.batchImgInfo batchInfo, ResponseStruct.imgInfo[] imgInfo) {
            string filePath = $"output\\{currentTreeName}\\";
            Directory.CreateDirectory(filePath);
            //Decide if new Node is needed
            if(nodes.Count == 0 || batchInfo.prompt != currentNode.nodeInfo.prompt || batchInfo.negative_prompt != currentNode.nodeInfo.negative_prompt ||
                batchInfo.width != currentNode.nodeInfo.width || batchInfo.height != currentNode.nodeInfo.width)  { //Prompt Info not identical -> Create a new Node
                NodeInfo nodeInfo = new NodeInfo() { //New Node is needed -> Create new Node with Prompt Info and Add it to tree
                    prompt = batchInfo.prompt,
                    negative_prompt = batchInfo.negative_prompt,
                    width = batchInfo.width,
                    height = batchInfo.height
                };
                ImgNode node = new ImgNode(countNodeId++, nodeInfo);
                Controller.WriteToLog("Node Id Increased to " + countNodeId);
                InsertItem(node);
            }
            //Save Images to node
            for (int i = 0; i < batchInfo.batch_size; i++) {
                File.WriteAllBytes(filePath + countImgId + ".png", Convert.FromBase64String(images[i]));
                File.WriteAllText(filePath + countImgId + ".txt", JsonSerializer.Serialize(imgInfo[i]));
                currentNode.imgId.Add(countImgId);
                countImgId++;
                Controller.WriteToLog("Image Id Increased to " + countImgId);
            }
            //PopulateTreeView();
        }

        /// <summary>
        /// Save Tree Structure to File
        /// </summary>
        public static void SaveTree() {
            if (start == null) return; //Don't Save Tree if nothing has been generated (nothing to save!)
            //Create TreeStruct
            TreeStruct treeStruct = new TreeStruct {
                CurrentTreeName = currentTreeName,
                CountImgId = countImgId,
                CountNodeId = countNodeId
            };
            string treeInfo = JsonSerializer.Serialize(treeStruct); //Serialize TreeStruct
            //Create collection of Node Info
            string[] nodeInfos = new string[countNodeId];
            for (int i = 0; i < nodeInfos.Length; i++)
                nodeInfos[i] = nodes[i].ToString();
            //Write all data to the file
            string outputString = treeInfo;
            for (int i = 0; i < nodeInfos.Length; i++)
                outputString += "\n" + nodeInfos[i];
            File.WriteAllText($"output\\{currentTreeName}\\tree.txt", outputString);
        }


        //public static void PopulateTreeView()
        //{
        //    Application.Current.Dispatcher.Invoke((Action)delegate
        //    {
        //        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        //        TreeView treeView = mainWindow.treeView;

        //        treeView.Items.Clear();

        //        List<ImgNode> addedNodes = new List<ImgNode>();

        //        foreach (var node in ImgTree.Nodes)
        //        {
        //            if (!addedNodes.Contains(node))
        //            {
        //                TreeViewItem treeNode = CreateTreeViewItem(node, addedNodes);
        //                treeView.Items.Add(treeNode);
        //            }
        //        }
        //    });
        //}

        public static TreeViewItem CreateTreeViewItem(ImgNode node, List<ImgNode> addedNodes)
        {
            TreeViewItem treeNode = new TreeViewItem { Header = $"Node {node.nodeId}", Tag = node.nodeId.ToString(), IsExpanded = true };
            treeNode.PreviewMouseLeftButtonDown += TreeViewItem_MouseUp;

            addedNodes.Add(node);

            foreach (var item in node.imgId)
            {
                //// Image:

                //Image image = new Image
                //{
                //    Source = new BitmapImage(new Uri(Path.GetFullPath("output\\zoo\\0.png"))), //(ToDo: Image-path must be retreived)
                //    Height = 20
                //};

                //// StackPanel to hold the TextBlock and Image
                //StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
                //// TextBlock for the header text
                //TextBlock textBlock = new TextBlock { Text = $"Item {item}" };

                //stackPanel.Children.Add(textBlock);
                //stackPanel.Children.Add(image);
                ////
                string stackPanel = $"Item {item}"; // Platzhalter

                treeNode.Items.Add(new TreeViewItem { Header = stackPanel, Tag = item.ToString() });
            }

            foreach (var childNode in node.children)
            {
                TreeViewItem childTreeNode = CreateTreeViewItem(childNode, addedNodes);
                treeNode.Items.Add(childTreeNode);
            }

            return treeNode;
        }

        private static void TreeViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                string headerText = treeViewItem.Header.ToString(); //unvollständig
                // Logik
                SelectNode(treeViewItem);

                // if-else just for testing
                if (headerText != null)
                {
                    Controller.WriteToLog(headerText);
                }
                else
                {
                    Controller.WriteToLog("no HeaderText found");
                }
            }
        }

        // set via onClick
        public static ImgNode SelectNode(TreeViewItem treeViewItem)
        {
            string searchedIdString = "0";
            if (treeViewItem.Tag is string s)
            {
                searchedIdString = s;
            }
            int searchedId = int.Parse($"{searchedIdString}");
            // wohl besser mit try catch
            //if (currentNode != null && nodeId >= 0 && nodeId < nodes.Count)
            //{
            currentNode = nodes[searchedId];

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.PromptBox.Text = $"{currentNode.nodeInfo.prompt}";
                mainWindow.NegativePromptBox.Text = $"{currentNode.nodeInfo.negative_prompt}";
                mainWindow.WidthBox.Text = $"{currentNode.nodeInfo.width}";
                mainWindow.HeightBox.Text = $"{currentNode.nodeInfo.height}";
            });
                        
            return currentNode;
            //}

            //return null;
        }
    }
}
