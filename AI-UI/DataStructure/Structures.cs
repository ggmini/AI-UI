namespace Data_Structure {
    public struct NodeInfo {
        public string prompt { get; set; }
        public string negative_prompt { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string nodeInfo { get; set; }
    }

    public struct NodeStruct {
        public string nodeInfo { get; set; }
        public int[] imgIds { get; set; }
        public int[] childrenIds { get; set; }
    }

    public struct TreeStruct {
        public int CountNodeId {get; set; }
        public int CountImgId { get; set; }
        public string CurrentTreeName { get; set; }
    }
}
