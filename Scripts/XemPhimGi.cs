using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.SceneManagement;
using System.IO;
using System.IO.Compression;

public class XemPhimGi : MonoBehaviour
{
    //Link xem phim
    private string gitLink = "https://raw.githubusercontent.com/duongan2001/Testing-Cocos/main/MyJSON.json";

    //Link không xem được phim
    //private string gitLink = "https://raw.githubusercontent.com/duongan2001/Testing-Cocos/refs/heads/main/MyNullJSON.json";

    private string taiPhimVeDay;
    private string projectJsonPath;

    private OiDoiOi layLinkDiEm;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DocFileJSON());
    }

    IEnumerator DocFileJSON()
    {
        var request = UnityWebRequest.Get(gitLink);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Lỗi tải JSON: " + request.error);
        }
        else
        {
            string jsonText = request.downloadHandler.text;
            Debug.Log("Dữ liệu JSON tải về: " + jsonText);

            TimLinkPhimNao(jsonText);
        }
    }

    void TimLinkPhimNao(string linkDau)
    {
        layLinkDiEm = JsonUtility.FromJson<OiDoiOi>(linkDau);

        if (layLinkDiEm == null || string.IsNullOrEmpty(layLinkDiEm.phim))
        {
            Debug.Log("Không tìm thấy link phim");
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.Log("Phim gì: " + layLinkDiEm.phim);
            Debug.Log("Link đâu: " + layLinkDiEm.link);

            TaiPhim();
        }
    }

    void TaoThuMucLuuPhim()
    {
        string dataFolderPath = Application.persistentDataPath;

        taiPhimVeDay = dataFolderPath;
        Debug.Log("Đường dẫn lưu dữ liệu: " + taiPhimVeDay);
    }

    void TaiPhim()
    {
        TaoThuMucLuuPhim();

        projectJsonPath = Path.Combine(taiPhimVeDay, "data/project.json");

        if (File.Exists(projectJsonPath))
        {
            Debug.Log("Có phim rồi, xem thôi...");
            BatPhimLenVaXem();
        }
        else
        {
            Debug.Log("Tải phim về");

            string fileUrl = layLinkDiEm.link; // Thay thế bằng link thực tế
            StartCoroutine(DownloadZip(fileUrl, "downloaded.zip"));
        }
    }

    IEnumerator DownloadZip(string url, string fileName)
    {
        string filePath = Path.Combine(taiPhimVeDay, fileName);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Lưu file ZIP vào thư mục
                File.WriteAllBytes(filePath, request.downloadHandler.data);
                Debug.Log("Tải file ZIP thành công: " + filePath);

                // Giải nén file sau khi tải xong
                ExtractZip(filePath, taiPhimVeDay);

                // Xóa file ZIP sau khi giải nén
                DeleteZip(filePath);

                BatPhimLenVaXem();
            }
            else
            {
                Debug.LogError("Lỗi tải file ZIP: " + request.error);
            }
        }
    }

    void ExtractZip(string zipPath, string extractPath)
    {
        try
        {
            // Giải nén file vào thư mục đích
            ZipFile.ExtractToDirectory(zipPath, extractPath);
            Debug.Log("Giải nén thành công vào thư mục: " + extractPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi khi giải nén: " + e.Message);
        }
    }

    void DeleteZip(string zipPath)
    {
        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
            Debug.Log("Đã xóa file ZIP: " + zipPath);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy file ZIP để xóa.");
        }
    }

    private void BatPhimLenVaXem()
    {
#if UNITY_ANDROID
        ///Huh
#elif UNITY_IOS || UNITY_TVOS
        PhimNhatHayKhong.PhimNhatRatHay();
#endif
    }
}



//_____________________________________________________________________________

public class OiDoiOi
{
    public string phim = "";
    public string link = "";
}