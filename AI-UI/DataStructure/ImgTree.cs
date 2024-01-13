using System.IO;
using System;
using System.Collections.Generic;
using AI_UI;
using System.Text.Json;

namespace Data_Structure {
    public static class ImgTree {

        private static ImgNode start;
        private static int countNodeId = 0;
        private static int countImgId = 0;
        private static ImgNode currentNode;// set via onClick
        private static string currentTreeName;
        private static List<ImgNode> nodes;
        public static List<ImgNode> Nodes { get => nodes; }

        public static void NewTree(string name) {
            Directory.CreateDirectory("\\output\\" + name);
            nodes = new List<ImgNode>();
        }

        public static void LoadTree() {
            nodes = new List<ImgNode>();
            //Read Tree File from Selected Workspace

        }

        public static void InsertItem(ImgNode node) {
            if(start == null) {
                start = node;
                currentNode = node;
            } else {
                currentNode.children.Add(node);
                currentNode = node;
            }
        }
        
        public static void SaveBatch(string[] images, ResponseStruct.batchImgInfo batchInfo, ResponseStruct.imgInfo[] imgInfo) {
            string filePath = $"output\\{currentTreeName}\\";
            //Decide if new Node is needed
            if(batchInfo.prompt != currentNode.nodeInfo.prompt || batchInfo.negative_prompt != currentNode.nodeInfo.negative_prompt ||
                batchInfo.width != currentNode.nodeInfo.width || batchInfo.height != currentNode.nodeInfo.width)  { //Prompt Info not identical -> Create a new Node
                NodeInfo nodeInfo = new NodeInfo() {
                    prompt = batchInfo.prompt,
                    negative_prompt = batchInfo.negative_prompt,
                    width = batchInfo.width,
                    height = batchInfo.height
                };
                ImgNode node = new ImgNode(countNodeId++, nodeInfo);
                InsertItem(node);
                currentNode.AddChild(node);
                currentNode = node;
            }
            //Save Images to node
            for (int i = 0; i < batchInfo.batch_size; i++) {
                File.WriteAllBytes(filePath + countImgId + ".png", Convert.FromBase64String(images[i]));
                File.WriteAllText(filePath + countImgId + ".txt", JsonSerializer.Serialize(imgInfo[i]));
                currentNode.imgId.Add(countImgId);
                countImgId++;
            }        
        }

        public static void SaveTree() {
            TreeStruct treeStruct = new TreeStruct {
                CurrentTreeName = currentTreeName,
                CountImgId = countImgId,
                CountNodeId = countNodeId
            };
            string treeInfo = JsonSerializer.Serialize(treeStruct);
            string[] nodeInfos = new string[countNodeId + 1];
            for (int i = 0; i < nodeInfos.Length; i++) {
                nodeInfos[i] = nodes[i].ToString();
            }
            string[] outputString = new string[nodeInfos.Length];
            for (int i = 1; i < outputString.Length; i++)
                outputString[i] = nodeInfos[i - 1]; 
            File.WriteAllLines($"output\\{currentTreeName}\\tree.txt", outputString);
        }
        
    }
}
