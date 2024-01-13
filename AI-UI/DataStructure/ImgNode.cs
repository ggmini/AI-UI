using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AI_UI;

namespace Data_Structure {
    public class ImgNode {
        public int nodeId { get; set; }
        public NodeInfo nodeInfo;
        public List<int> imgId { get; set; }
        public List<ImgNode> children;
        public List<int> childrenIds { get; set; } //Serializing the Objects works better with IDs

        /// <summary>
        /// Create New ImgNode
        /// </summary>
        /// <param name="nodeId">Id to be assigned to node</param>
        /// <param name="nodeInfo">Node Info to be assigned to Node</param>
        public ImgNode(int nodeId, NodeInfo nodeInfo) {
            this.nodeId = nodeId;
            this.nodeInfo = nodeInfo;
            imgId = new List<int>();
            children = new List<ImgNode>();
            childrenIds = new List<int>();

            Controller.WriteToLog($"Node {nodeId} created");
        }

        /// <summary>
        /// Create Node from existing NodeStruct
        /// </summary>
        /// <param name="nodeStruct">NodeStruct to use for creation</param>
        public ImgNode(NodeStruct nodeStruct) {
            nodeId = nodeStruct.nodeId;
            imgId = nodeStruct.imgIds.ToList();
            childrenIds = nodeStruct.childrenIds.ToList();
            nodeInfo = JsonSerializer.Deserialize<NodeInfo>(nodeStruct.nodeInfo);
            children = new();
        }

        /// <summary>
        /// Find all Children
        /// </summary>
        public void FindChildren() {
            foreach(int id  in childrenIds)
                children.Add(ImgTree.Nodes[id]);
        }

        /// <summary>
        /// Add Child
        /// </summary>
        /// <param name="child">Child Node to Add</param>
        public void AddChild(ImgNode child) {
            children.Add(child);
            childrenIds.Add(child.nodeId);
        }

        /// <summary>
        /// Convert Node to String to Save in Tree File
        /// </summary>
        /// <returns>Returns Node in Json String</returns>
        public override string ToString() {
            Controller.WriteToLog($"Serializing Node {nodeId} with prompt {nodeInfo.prompt}");
            string promptInfo = JsonSerializer.Serialize(nodeInfo); //Stringify all Prompt Info
            NodeStruct nodeStruct = new NodeStruct {
                nodeInfo = promptInfo,
                nodeId = nodeId,
                imgIds = imgId.ToArray(),
                childrenIds = childrenIds.ToArray()
            };
            return JsonSerializer.Serialize(nodeStruct);
            
        }

    }

}
