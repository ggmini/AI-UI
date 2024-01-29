namespace Data_Structure {

    // Collection of Structs to help with Serializing Data when saving

    /// <summary>
    /// Prompt information for this node
    /// </summary>
    public struct NodeInfo {
        public string prompt { get; set; }
        public string negative_prompt { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    /// <summary>
    /// Other information for this node
    /// </summary>
    public struct NodeStruct {
        public int nodeId { get; set; }
        /// <summary>
        /// Will contain NodeInfo in JsonFormat
        /// </summary>
        public string nodeInfo { get; set; }
        public int[] imgIds { get; set; }
        public int[] childrenIds { get; set; }
    }

    /// <summary>
    /// Information about the current tree
    /// </summary>
    public struct TreeStruct {
        public int CountNodeId {get; set; }
        public int CountImgId { get; set; }
        public string CurrentTreeName { get; set; }
    }
}
