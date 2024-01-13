using System.Collections.Generic;
using System.Text.Json;
using AI_UI;

namespace Data_Structure {
    public class ImgNode {
        public int nodeId { get; set; }
        public NodeInfo nodeInfo;
        public List<int> imgId { get; set; }
        public List<ImgNode> children;
        public List<int> childrenIds { get; set; } //Serializing the Objects works better with IDs

        public ImgNode(int nodeId, NodeInfo nodeInfo) {
            this.nodeId = nodeId;
            this.nodeInfo = nodeInfo;
            imgId = new List<int>();
            children = new List<ImgNode>();
            childrenIds = new List<int>();
        }

        public void FindChildren(int[] childrenIds) {
            foreach(int id  in childrenIds)
                children.Add(ImgTree.Nodes[id]);
        }

        public void AddChild(ImgNode child) {
            children.Add(child);
            childrenIds.Add(child.nodeId);
        }

        public override string ToString() {
            string promptInfo = JsonSerializer.Serialize(nodeInfo);
            NodeStruct nodeStruct = new NodeStruct {
                nodeInfo = promptInfo,
                imgIds = imgId.ToArray(),
                childrenIds = childrenIds.ToArray()
            };
            return JsonSerializer.Serialize(nodeStruct);
            
        }

    }

}
