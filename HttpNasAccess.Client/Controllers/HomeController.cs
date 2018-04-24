using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using HttpNasAccess.Client.Model;
using Newtonsoft.Json;

namespace HttpNasAccess.Client.Controllers
{
    public class HomeController : Controller
    {
        private string userId = "userid";
        private string userPassword = "userpwd";
        public ActionResult Index()
        {
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var appPath = Server.MapPath(@"~\Temp");
            if (!Directory.Exists(appPath))
                Directory.CreateDirectory(appPath);

            return View();
        }

        public async Task<ActionResult> Download()
        {
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/download";

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };
            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);

                //Sample file to download from NAS
                var fullFileNasPath = "\\\\nasv0034\\WAPFileTest\\GoodNas\\SQL Server date types.xlsx";
                var content = new MultipartFormDataContent
                {
                    {
                        new StringContent(fullFileNasPath), "FullFilePath"
                    }
                };
                var response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var fileName = response.Content.Headers.ContentDisposition.FileName;
                    var stream = await response.Content.ReadAsStreamAsync();
                    //You can save it to local Temp folder in web worker for further processing
                    //var appPath = Server.MapPath("~\\Temp");
                    //var localFullPath = string.Format("{0}\\{1}", appPath, fileName);
                    // vnd.ms - excel
                    //const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Clear();
                    Response.ContentType = "application/octet-stream";//contentType;
                    //Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName); 
                    stream.CopyTo(Response.OutputStream);
                    //stream.Close();
                    Response.Flush();
                    Response.End();
                    
                    //using (var fileStream = System.IO.File.Create(localFullPath))
                    //{
                    //    stream.CopyTo(fileStream);
                    //}
                    responseMessage = "yay!";
                }
                else
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    var message = response.Content.ReadAsStringAsync().Result;
                    responseMessage = message;
                }
            }

            return new ContentResult
            {
                Content = responseMessage
            };

        }

        public async Task<ActionResult> Delete()
        {
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/delete";

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };
            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);

                //Sample file to delete from NAS
                var fullFileNasPath = "\\\\nasv0034\\WAPFileTest\\GoodNas\\test.txt";
                var content = new MultipartFormDataContent
                {
                    {
                        new StringContent(fullFileNasPath), "FullFilePath"
                    }
                };
                var response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    responseMessage = "yay!";
                }
                else
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    var message = response.Content.ReadAsStringAsync().Result;
                    responseMessage = message;
                }
            }
            return new ContentResult
            {
                Content = responseMessage
            };

        }

        public async Task<ActionResult> RenameFile()
        {
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/renamefile";

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };
            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);

                //Sample file to delete from NAS
                var oldFileName = "\\\\nasv0034\\WAPFileTest\\GoodNas\\test.html";
                var newFileName = "\\\\nasv0034\\WAPFileTest\\GoodNas\\testNewName.html";
                var content = new MultipartFormDataContent
                {
                    {
                        new StringContent(oldFileName), "OldFileNameFullPath"
                    },
                    {
                        new StringContent(newFileName), "NewFileNameFullPath"
                    }
                };
                var response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    responseMessage = "yay!";
                }
                else
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    var message = response.Content.ReadAsStringAsync().Result;
                    responseMessage = message;
                }
            }
            return new ContentResult
            {
                Content = responseMessage
            };
        }

        public async Task<ActionResult> Upload()
        {
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/upload";

            //Get it from local
            var localFileToUpload = Server.MapPath("~\\Temp\\test.txt");

            var fileName = Path.GetFileName(localFileToUpload);
            FileStream fs = System.IO.File.OpenRead(localFileToUpload);

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };

            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);
                //Can upload more than one file in one request
                var content = new MultipartFormDataContent
                {
                    {
                        new StringContent("\\\\nasv0034\\WAPFileTest\\GoodNas"), "NASPath"
                    },
                    {
                        new StreamContent(fs), "txtFile", fileName
                    }
                };
                var response = await client.PostAsync(uri, content);
                if (!response.IsSuccessStatusCode)
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    var message = response.Content.ReadAsStringAsync().Result;
                    responseMessage = message;
                }
            }
            return new ContentResult
            {
                Content = responseMessage
            };
        }

        public async Task<ActionResult> DownloadMultiple()
        {
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/downloadmultiple";

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };
            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);

                //Sample file to download from NAS
                //var fullFileNasPath = "\\\\nasv0034\\WAPFileTest\\GoodNas\\test.txt";
                var content = new MultipartFormDataContent
                {
                    {new StringContent(@"\\nasv0034\WAPFileTest\GoodNas\web.config"), "FullFilePath"},
                    {new StringContent(@"\\nasv0034\WAPFileTest\GoodNas\test.html"), "FullFilePath"},
                    {new StringContent(@"\\nasv0034\WAPFileTest\GoodNas\IST databases 042415.xlsx"), "FullFilePath"},
                    {new StringContent(@"\\nasv0034\WAPFileTest\GoodNas\Background.png"), "FullFilePath"}
                };
               
                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var fileName = response.Content.Headers.ContentDisposition.FileName;
                    var stream = await response.Content.ReadAsStreamAsync();
                    //You can save it to local Temp folder in web worker for further processing
                    var appPath = Server.MapPath("~\\Temp");
                    var localFullPath = string.Format("{0}\\{1}", appPath, fileName);
                    using (var fileStream = System.IO.File.Create(localFullPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                    responseMessage = "yay!";
                }
                else
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    var message = response.Content.ReadAsStringAsync().Result;
                    responseMessage = message;
                }
            }

            return new ContentResult
            {
                Content = responseMessage
            };

        }

        public async Task<ActionResult> ListFileNames()
        {
            
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/listfilenames";
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };

            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);
                var folderInNASToQuery = "\\\\nasv0034\\WAPFileTest\\GoodNas";
                var content = new MultipartFormDataContent
                {
                    {
                        new StringContent(folderInNASToQuery), "NASPath"
                    }
                };

                var response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    //check  the filenames collection for all the files in NAS
                    var fileNames = JsonConvert.DeserializeObject<IEnumerable<string>>(result);
                    responseMessage = fileNames.Aggregate(responseMessage, (current, fileName) => $"{current},{fileName}");
                }
                else
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    var message = response.Content.ReadAsStringAsync().Result;
                    responseMessage = message;
                }
            }
            return new ContentResult
            {
                Content = responseMessage
            };
        }

        public async Task<ActionResult> ListAll()
        {
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/listall";
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };

            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);
                var folderInNASToQuery = "\\\\nasv0034\\WAPFileTest\\GoodNas";
                var content = new MultipartFormDataContent
                {
                    {
                        new StringContent(folderInNASToQuery), "NASPath"
                    }
                };

                var response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    //check  the filenames collection for all the files in NAS
                    var fileNames = JsonConvert.DeserializeObject<IEnumerable<string>>(result);
                    responseMessage = fileNames.Aggregate(responseMessage, (current, fileName) => $"{current},{fileName}");
                }
                else
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    var message = response.Content.ReadAsStringAsync().Result;
                    responseMessage = message;
                }
            }
            return new ContentResult
            {
                Content = responseMessage
            };
        }

        public async Task<ActionResult> CreateFolder()
        {
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/listall";
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };

            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);
                var folderInNASToQuery = "\\\\nasv0034\\WAPFileTest\\GoodNas\\NewFolder";
                var content = new MultipartFormDataContent
                {
                    {
                        new StringContent(folderInNASToQuery), "NASPath"
                    }
                };

                var response = await client.PostAsync(uri, content);
                if (!response.IsSuccessStatusCode)
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    var message = response.Content.ReadAsStringAsync().Result;
                    responseMessage = message;
                }
            }
            return new ContentResult
            {
                Content = responseMessage
            };
        }

        public async Task<ActionResult> GetInfo()
        {
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/getinfo";
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };

            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);
                var folderInNASToQuery = "\\\\nasv0034\\WAPFileTest\\GoodNas";
                var content = new MultipartFormDataContent
                {
                    {
                        new StringContent(folderInNASToQuery), "NASPath"
                    }
                };

                var response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    //check  the filedirectoryinfo collection for all the files and folders in NAS
                    var objects = JsonConvert.DeserializeObject<IEnumerable<FileDirectoryInfo>>(result);
                    responseMessage = "completed";
                }
                else
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    responseMessage = response.Content.ReadAsStringAsync().Result;
                   
                }
                return new ContentResult
                {
                    Content = responseMessage
                };
            }
            
        }

        public async Task<ActionResult> CopyToAnotherNas()
        {
            string responseMessage = string.Empty;
            var endpointUrl = "https://nas.domain.com/nas/copytoanothernasdrive";
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userId, userPassword)
            };

            using (var client = new HttpClient(handler))
            {
                var uri = new Uri(endpointUrl);
                var targetFolder = "\\\\nasv0034\\WAPFileTest\\GoodNas\\NewFolder";
                var content = new MultipartFormDataContent
                {
                    //first item must be the target folder in NAS
                    {new StringContent(targetFolder), "targetFolderInNas"},
                    //second item and onward are files or folders to copy
                    {new StringContent(@"\\nasv0034\WAPFileTest\GoodNas\web.config"), "FullFilePath"},
                    {new StringContent(@"\\nasv0034\WAPFileTest\GoodNas\test.html"), "FullFilePath"},
                    {new StringContent(@"\\nasv0034\WAPFileTest\GoodNas\IST databases 042415.xlsx"), "FullFilePath"},
                    {new StringContent(@"\\nasv0034\WAPFileTest\GoodNas\Background.png"), "FullFilePath"},
                    {new StringContent(@"\\nasv0034\WAPFileTest\GoodNas\JimmyFolder1"), "FullFilePath"}
                };

                var response = await client.PostAsync(uri, content);
                if (!response.IsSuccessStatusCode)
                {
                    //check status code. usually it is access denied
                    var statusCode = response.StatusCode;
                    var message = response.Content.ReadAsStringAsync().Result;
                    responseMessage = message;
                }
            }
            return new ContentResult
            {
                Content = responseMessage
            };
        }
    }
}