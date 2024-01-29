using System.Text.Json;

namespace AI_UI {

    //Most of these variables need use { get; set; } in order for the JsonSerializer to work
    //This is a collection of structs used to assist with Serialization for the Web Requests to the API

    /// <summary>
    /// Struct used to serialize information to send to API with the txt2img or img2img POST Request
    /// </summary>
    public struct RequestStruct {
        public string prompt { get; set; }
        public string negative_prompt { get; set; }
        public long seed { get; set; }
        public int steps { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int batch_size { get; set; }
        public string[] init_images { get; set; }
        public double denoising_strength { get; set; }
    }

    /// <summary>
    /// Struct used to deserialize the response from the txt2img Post Request
    /// </summary>
    public struct ResponseStruct {
        public string[] images { get; set; }
        public string info { get; set; }
        public imgInfo[] imgInfos;

        /// <summary>
        /// Struct used to serialize the Information for a specific image
        /// </summary>
        public struct imgInfo {
            public string prompt { get; set; }
            public string negative_prompt { get; set; }
            public long seed { get; set; }
            public int steps { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public double denoising_strength { get; set; }
        }

        /// <summary>
        /// Struct used to serialize the Information for a batch of images
        /// </summary>
        public struct batchImgInfo {
            public string prompt { get; set; }
            public string negative_prompt { get; set; }
            public long[] all_seeds { get; set; }
            public int steps { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public double denoising_strength { get; set; }
            public int batch_size { get; set; }

        }

        /// <summary>
        /// Extracts the batch and individual image information out of the response struct
        /// </summary>
        /// <returns>Returns BatchImgInfo with all the ImgInfos</returns>
        public batchImgInfo ExtractImgInfo() {
            batchImgInfo batchInfo = JsonSerializer.Deserialize<batchImgInfo>(info);
            imgInfo[] imgInfos = new imgInfo[batchInfo.batch_size];
            for (int i = 0; i < batchInfo.batch_size; i++)
                imgInfos[i] = new imgInfo
                {
                    prompt = batchInfo.prompt,
                    negative_prompt = batchInfo.negative_prompt,
                    seed = batchInfo.all_seeds[i],
                    steps = batchInfo.steps,
                    width = batchInfo.width,
                    height = batchInfo.height,
                    denoising_strength = batchInfo.denoising_strength
                };
            this.imgInfos = imgInfos;
            return batchInfo;
        }
    }

    /// <summary>
    /// Simpe Struct used to deserialize the response from the "/progress" GET Request
    /// </summary>
    public struct ProgressStruct {
        public double progress { get; set; }
    }
}
