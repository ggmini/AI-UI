using System.Text.Json;

namespace AI_UI {
    public struct RequestStruct {
        public string prompt { get; set; }
        public string negative_prompt { get; set; }
        public int seed { get; set; }
        public int steps { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int batch_size { get; set; }
        public string[] init_images { get; set; }
        public double denoising_strength { get; set; }
    }

    public struct ResponseStruct {
        public string[] images { get; set; }
        public string info { get; set; }
        public imgInfo[] imgInfos;

        public struct imgInfo {
            public string prompt { get; set; }
            public string negative_prompt { get; set; }
            public int seed { get; set; }
            public int steps { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public double denoising_strength { get; set; }
        }

        public struct batchImgInfo {
            public string prompt { get; set; }
            public string negative_prompt { get; set; }
            public int[] seed { get; set; }
            public int steps { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public double denoising_strength { get; set; }
            public int batch_size { get; set; }

        }

        public batchImgInfo ExtractImgInfo() {
            batchImgInfo batchInfo = JsonSerializer.Deserialize<batchImgInfo>(info);
            imgInfo[] imgInfos = new imgInfo[batchInfo.batch_size];
            for (int i = 0; i > batchInfo.batch_size; i++)
                imgInfos[i] = new imgInfo
                {
                    prompt = batchInfo.prompt,
                    negative_prompt = batchInfo.negative_prompt,
                    seed = batchInfo.seed[i],
                    steps = batchInfo.steps,
                    width = batchInfo.width,
                    height = batchInfo.height,
                    denoising_strength = batchInfo.denoising_strength
                };
            this.imgInfos = imgInfos;
            return batchInfo;
        }
    }

    public struct NodeInfo {
        public string prompt { get; set; }
        public string negative_prompt { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public struct ProgressStruct {
        public double progress { get; set; }
    }
}
