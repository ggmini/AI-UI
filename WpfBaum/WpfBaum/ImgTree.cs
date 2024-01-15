using System;
using System.Collections.Generic;
using System.Text;

namespace WpfBaum
{
    class ImgTree
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
        private ImgNode currentNode = null;

        public List<ImgNode> nodeList = new List<ImgNode>();

        public ImgNode SelectNode(int pos)// set via onClick
        {
            if (currentNode != null && pos >= 0 && pos < nodeList.Count)
            {
                currentNode = nodeList[pos];
                return currentNode;
            }

            return null;
        }


        public void InsertItem(string PromptInfo, int Id)
        {
            ImgItem neu = new ImgItem() { promptInfo = PromptInfo, id = Id };

            // Erstes Element in Baum
            if (start == null)
            {
                ImgNode newNode = new ImgNode(countNodeId++);
                nodeList.Add(newNode);
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
                nodeList.Add(newNode);
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
    }
}
