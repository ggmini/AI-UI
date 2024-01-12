using System;
using System.Collections.Generic;

namespace Data_Structure
{
    public class ImgTree
    {
        public class ImgItem
        {
            public string promptInfo;
            public int id;
        }

        public class ImgNode
        {
            public int nodeId;
            public string promptInfoOfNode;
            public List<ImgItem> itemList;
            public List<ImgNode> children;

            public ImgNode(int nodeId)
            {
                this.nodeId = nodeId;
                itemList = new List<ImgItem>();
                children = new List<ImgNode>();
            }
        }

        private ImgNode start = null;
        private int countNodeId = 0;
        private ImgNode currentNode = null;// set via onClick

        public void InsertItem(string PromptInfo, int Id)
        {
            ImgItem neu = new ImgItem() { promptInfo = PromptInfo, id = Id };

            // Erstes Element in Baum
            if (start == null)
            {
                ImgNode newNode = new ImgNode(countNodeId++);
                start = newNode;
                currentNode = newNode;
                currentNode.promptInfoOfNode = PromptInfo;
                currentNode.itemList.Add(neu);
            }
            else if (currentNode.promptInfoOfNode == PromptInfo) // von aktueller node
            {
                currentNode.itemList.Add(neu);
            }
            else
            {
                ImgNode newNode = new ImgNode(countNodeId++) { promptInfoOfNode = PromptInfo };
                currentNode.children.Add(newNode);

                currentNode = newNode;
                currentNode.itemList.Add(neu);
            }
        }

        public void PrintTree()
        {
            PrintNode(start, "");
        }

        private void PrintNode(ImgNode node, string indent, bool last = true)
        {
            Console.Write(indent);

            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }

            Console.WriteLine($"Node {node.nodeId}");

            foreach (var item in node.itemList)
            {
                Console.WriteLine($"{indent}  Item {item.id}");
            }

            for (int i = 0; i < node.children.Count; i++)
            {
                bool isLastChild = i == node.children.Count - 1;
                PrintNode(node.children[i], indent, isLastChild);
            }
        }

        static void Main()
        {
            ImgTree it = new ImgTree();
            it.InsertItem("Bär", 1);
            it.InsertItem("Bär", 2);
            it.InsertItem("Bär", 3);
            it.InsertItem("Bär fotorealistisch", 4);
            it.InsertItem("Bär isst Fisch", 5);
            it.InsertItem("Bär isst Fisch", 6);
            it.InsertItem("Gans", 7);

            it.PrintTree();
        }
    }

}
