using System.IO;
using System;
using System.Collections.Generic;
using AI_UI;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Data_Structure {
    public static class ImgTree {

        private static ImgNode start; //Root Node
        private static int countNodeId = 0; //ID for next Node
        private static int countImgId = 0; //ID for next Img
        private static ImgNode currentNode; //Currently selected node
        private static string currentTreeName;
        public static string CurrentTreeName { get => currentTreeName; }
        private static List<ImgNode> nodes; //List of current Nodes
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
            foreach(ImgNode node in nodes)
                node.FindChildren(); //Add Children to their parent nodes (right now Nodes only have the children IDs)
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
                InsertItem(node); //Current Node is automatically moved
            }
            //Save Images to node
            for (int i = 0; i < batchInfo.batch_size; i++) {
                File.WriteAllBytes(filePath + countImgId + ".png", Convert.FromBase64String(images[i]));
                File.WriteAllText(filePath + countImgId + ".txt", JsonSerializer.Serialize(imgInfo[i]));
                currentNode.imgId.Add(countImgId);
                countImgId++;
                Controller.WriteToLog("Image Id Increased to " + countImgId);
            }
            PopulateTreeView();
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

        /// <summary>
        /// Populates the visual tree on the UI
        /// </summary>
        public static void PopulateTreeView()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                //Access visual tree from MainWindow
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                TreeView treeView = mainWindow.treeView;

                treeView.Items.Clear();

                List<ImgNode> addedNodes = new List<ImgNode>(); //used to keep track of added nodes
                foreach (var node in Nodes)
                {
                    //If not already existing, creates new Node and adds it to the visual tree
                    if (!addedNodes.Contains(node))
                    {
                        TreeViewItem treeNode = CreateTreeViewItem(node, addedNodes);
                        treeView.Items.Add(treeNode);
                    }
                }
            });
        }

        /// <summary>
        /// Helps PopulateTreeView by creating new Nodes represented by TreeViewItems
        /// </summary>
        /// <param name="node">node currently handled by iteration in PopulateTreeView</param>
        /// <param name="addedNodes">all nodes currently added to the visual tree while iterating Nodes</param>
        /// <returns></returns>
        public static TreeViewItem CreateTreeViewItem(ImgNode node, List<ImgNode> addedNodes)
        {
            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            //Get first image created by prompt of this node to represent node
            Image image = new Image {
                Source = new BitmapImage(new Uri(Path.GetFullPath($"output\\{currentTreeName}\\{node.imgId[0]}.png"))),
                    Height = 100
            };
            stackPanel.Children.Add(image);
            //Show number of further images created by prompt of this node
            if (node.imgId.Count > 1) stackPanel.Children.Add(new TextBlock { Text = $"+{node.imgId.Count - 1}", FontSize = 36,
                VerticalAlignment = VerticalAlignment.Center, Foreground = new SolidColorBrush(Colors.White), Margin = new Thickness(10, 0, 10, 0) });
            TreeViewItem treeNode = new TreeViewItem() { Header = stackPanel, Tag = node.nodeId.ToString(), IsExpanded = true };

            //These Events execute for all parent nodes as well...this element executes last so it's still working as intended
            treeNode.PreviewMouseLeftButtonDown += TreeViewItem_MouseUp;
            treeNode.PreviewMouseRightButtonDown += TreeViewItem_MouseRight;
            treeNode.PreviewMouseDoubleClick += TreeViewItem_MouseDouble;

            addedNodes.Add(node);

            //Create child nodes for each node derived from parent node's prompt
            foreach (var childNode in node.children)
            {
                TreeViewItem childTreeNode = CreateTreeViewItem(childNode, addedNodes);
                treeNode.Items.Add(childTreeNode);
            }

            return treeNode;
        }

        /// <summary>
        /// Click-Event for Node (calls SelectNode)
        /// </summary>
        private static void TreeViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                string headerText = treeViewItem.Tag.ToString();
                SelectNode(treeViewItem);

                // if-else just for logging
                if (headerText != null) {
                    Controller.WriteToLog("Selecting Node: " + headerText);
                }
                else {
                    Controller.WriteToLog("no HeaderText found");
                }
            }
        }

        /// <summary>
        /// Right-Click-Event for Node (calls SelectNodeRight)
        /// </summary>
        private static void TreeViewItem_MouseRight(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                string headerText = treeViewItem.Tag.ToString();
                SelectNodeRight(treeViewItem);

                // if-else just for logging
                if (headerText != null)
                {
                    Controller.WriteToLog("Selecting Node for Merging: " + headerText);
                }
                else
                {
                    Controller.WriteToLog("no HeaderText found");
                }
            }
        }

        /// <summary>
        /// Double-Click-Event for Node (calls SelectNode and OpenNode to view all Images of the node)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TreeViewItem_MouseDouble(object sender, MouseEventArgs e) {
            if (sender is TreeViewItem treeViewItem) {
                SelectNode(treeViewItem);
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.OpenNode(currentNode);
                
            }
        }

        /// <summary>
        /// Selects node when called by one of it's two Click-Events and adds it's nodeInfo to the UI's input elements
        /// </summary>
        /// <param name="treeViewItem">TreeViewItem clicked on</param>
        /// <returns>Selected node</returns>
        public static ImgNode SelectNode(TreeViewItem treeViewItem)
        {
            string searchedIdString = "0";
            if (treeViewItem.Tag is string s)
            {
                searchedIdString = s;
            }
            int searchedId = int.Parse($"{searchedIdString}");

            try{ 
            currentNode = nodes[searchedId];

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                
                mainWindow.PromptBox.Text = $"{currentNode.nodeInfo.prompt}";
                mainWindow.NegativePromptBox.Text = $"{currentNode.nodeInfo.negative_prompt}";
                mainWindow.SeedBox.Value = -1; //During demo users seemed to generate the same image multiple times.
                mainWindow.WidthBox.Text = $"{currentNode.nodeInfo.width}";
                mainWindow.HeightBox.Text = $"{currentNode.nodeInfo.height}";
            });

                        
            return currentNode;
            }
            catch (Exception ex) { return null; }
        }

        /// <summary>
        /// Selects node for merging ("img2" on UI) when called by it's Right-Click-Event and adds it's nodeInfo to the UI's input elements
        /// </summary>
        /// <param name="treeViewItem">TreeViewItem clicked on</param>
        /// <returns>Selected node</returns>
        public static ImgNode SelectNodeRight(TreeViewItem treeViewItem)
        {
            string searchedIdString = "0";
            if (treeViewItem.Tag is string s)
            {
                searchedIdString = s;
            }
            int searchedId = int.Parse($"{searchedIdString}");

            try
            {
                currentNode = nodes[searchedId];

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                    mainWindow.PromptBox2.Text = $"{currentNode.nodeInfo.prompt}";
                    mainWindow.NegativePromptBox2.Text = $"{currentNode.nodeInfo.negative_prompt}";
                    mainWindow.SeedBox.Value = -1; //During demo users seemed to generate the same image multiple times.
                    mainWindow.WidthBox.Text = $"{currentNode.nodeInfo.width}";
                    mainWindow.HeightBox.Text = $"{currentNode.nodeInfo.height}";
                });


                return currentNode;
            }
            catch (Exception ex) { return null; }
        }
    }
}
