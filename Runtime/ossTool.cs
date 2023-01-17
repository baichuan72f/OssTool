using System.IO;
using Aliyun.OSS;
using UnityEngine; // using UniRx;

namespace OssTool.Runtime
{
    public class OssConfig
    {
        public string AccessKeyId;
        public string AccessKeySecret;
        public string Endpoint;
        public string BucketName;
    }

    public class OssTool
    {
        private OssConfig _config;

        public OssTool(string endpoint, string accessKeyId, string accessKeySecret, string bucketName)
        {
            _config = new OssConfig
            {
                Endpoint = endpoint,
                AccessKeyId = accessKeyId,
                AccessKeySecret = accessKeySecret,
                BucketName = bucketName
            };
        }

        /// <summary>
        /// oss上传
        /// </summary>
        /// <param name="bucketName">桶名称</param>
        /// <param name="ossFileName">oss存储名称(需自定义为不重复的名称，且存储以供下载)</param>
        /// <param name="uploadDir">上传路径(/结尾)</param>
        /// <param name="fileName">上传文件名</param>
        public void OssUpload(string bucketName, string ossFileName, string uploadDir, string fileName)
        {
            if (!File.Exists(uploadDir + fileName))
            {
                Debug.Log("No File");
                return;
            }

            OssClient client = new OssClient(_config.Endpoint, _config.AccessKeyId, _config.AccessKeySecret);
            bucketName = string.IsNullOrEmpty(bucketName) ? _config.BucketName : bucketName;
            client.PutObject(bucketName, ossFileName, uploadDir + fileName);
            Debug.Log("PutObject");
        }

        /// <summary>
        /// oss下载
        /// </summary>
        /// <param name="bucketName">桶名称</param>
        /// <param name="ossFileName">oss存储名称</param>
        /// <param name="downloadDir">下载的路径</param>
        /// <param name="fileName">下载文件名</param>
        public void OssDownload(string bucketName, string ossFileName, string downloadDir, string fileName)
        {
            bucketName = string.IsNullOrEmpty(bucketName) ? _config.BucketName : bucketName;
            OssClient client = new OssClient(_config.Endpoint, _config.AccessKeyId, _config.AccessKeySecret);
            var result = client.GetObject(bucketName, ossFileName);

            using (var requestStream = result.Content)
            {
                using (var fs = File.Open(downloadDir + fileName, FileMode.OpenOrCreate))
                {
                    int length = 4 * 1024;
                    var buf = new byte[length];
                    do
                    {
                        length = requestStream.Read(buf, 0, length);
                        fs.Write(buf, 0, length);
                    } while (length != 0);
                }
            }

            Debug.Log("Get object succeeded");
        }
    }
}