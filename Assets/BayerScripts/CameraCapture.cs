using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;
using BestHTTP;
using BestHTTP.Authentication;
using BestHTTP.Examples;
using System;
using SimpleJSON;
using UnityEngine.SceneManagement;

public class CameraCapture : MonoBehaviour
{

	private const string API= "https://api.plant.id/v2/identify";

	public Texture2D texture2D;

	public RawImage capturedImage;

	public Text resultJSON;

	public string sampleJSON;

	public Text PlantName;
	public Text PlantHealth;
	public Text DiseaseCause;

	public GameObject Recommendation;
	public Text CaptureText;
	public Text JSONText;
	
    // Start is called before the first frame update
    void Start()
    {
		
		
    }

	public void CaptureClicked()
    {

		texture2D = null;

		if (NativeCamera.IsCameraBusy())
			return;

		//TakePicture(1024);

		StartCoroutine(HTTPRequests());


		
	}

	void Update()
	{
		
	}

	public void LoadMainScene()
    {
		SceneManager.LoadScene(0);
    }
	

	private IEnumerator HTTPRequests()
    {
		//yield return new WaitForEndOfFrame();

		string auth = "Basic Y2xpZW50OlZCdG5zMkJiVVhOVUE0cXJsWXM2SnpEMlJSZ2FHclRPbU9MNnh3VDY3QUdzWEd5NEYy";





		TakePicture(1024);

		CaptureText.text = "waiting for pic...!";

		yield return new WaitUntil(isTexture2DFilled);



		//capturedImage.texture = texture2D;
		//capturedImage.transform.localScale = new Vector3(1f, texture2D.height / (float)texture2D.width, 1f);

		//Debug.Log(texture2D.isReadable);
		//CaptureText.text = texture2D.isReadable.ToString();

		var bytes = texture2D.EncodeToJPG();
		var form = new WWWForm();

		FormData formData = new FormData();

		formData.api_key = "VBtns2BbUXNUA4qrlYs6JzD2RRgaGrTOmOL6xwT67AGsXGy4F2";
		formData.modifiers.Add("health_auto");

		Debug.Log(formData.ToString());

		string data = JsonUtility.ToJson(formData);


		CaptureText.text = "pic taken";


		form.AddField("data", data);
		form.AddBinaryData("image", bytes, $"{texture2D.name}.png", "image/png");


		using (var unityWebRequest = UnityWebRequest.Post(API, form))
		{
			unityWebRequest.SetRequestHeader("Authorization", auth);

			CaptureText.text = "pic uploaded";

			yield return unityWebRequest.SendWebRequest();
			

			if (unityWebRequest.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(unityWebRequest.result);

				print($"Failed to upload {texture2D.name}: {unityWebRequest.result} - {unityWebRequest.error}");


				CaptureText.text = "Error!!";
			}
			else
			{
				Debug.Log(unityWebRequest.result);
				print($"Finished Uploading {texture2D.name}");

				Debug.Log(unityWebRequest.downloadHandler.text);

				resultJSON.text = unityWebRequest.downloadHandler.text;

				PopulateData();

				CaptureText.text = "Loaded";
			}
		}

		


	}

	private bool isTexture2DFilled()
    {
		if (texture2D != null)
			return true;
		else
			return false;
    }

	private void PopulateData()
    {
		JSONNode result = SimpleJSON.JSON.Parse(resultJSON.text);

		//JSONText.text = resultJSON.text;

		Debug.Log(result["id"]);

		Debug.Log("plant name : " + result["suggestions"][0]["plant_name"]);

		Debug.Log("Health Status : " + result["health_assessment"]["is_healthy"]);

		Debug.Log("Disease cause" + result["health_assessment"]["diseases_simple"][0]["name"]);

		PlantName.text = "plant name : " + result["suggestions"][0]["plant_name"];

		if (result["health_assessment"]["is_healthy"] == true)
		{
			PlantHealth.text = "Health Status : " + "Healthy";
		}
		else if (result["health_assessment"]["is_healthy"] == false)
		{
			PlantHealth.text = "Health Status : " + "Not Healthy";
			DiseaseCause.text = "Disease cause : " + result["health_assessment"]["diseases_simple"][0]["name"];

		}

		Recommendation.gameObject.SetActive(true);
	}


	private void TakePicture(int maxSize)
	{
		NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				// Create a Texture2D from the captured image

				//Destroy(texture2D);

				texture2D = NativeCamera.LoadImageAtPath(path, maxSize);
				if (texture2D == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}



				// Assign texture to a temporary quad and destroy it after 5 seconds
				//GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				//quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
				//quad.transform.forward = Camera.main.transform.forward;
				//quad.transform.localScale = new Vector3(1f, texture2D.height / (float)texture2D.width, 1f);

				//Material material = quad.GetComponent<Renderer>().material;
				//if (!material.shader.isSupported) // happens when Standard shader is not included in the build
				//	material.shader = Shader.Find("Legacy Shaders/Diffuse");

				//material.mainTexture = texture2D;

				//Destroy(quad, 5f);

				// If a procedural texture is not destroyed manually, 
				// it will only be freed after a scene change
				//Destroy(texture2D, 5f);

				capturedImage.texture = texture2D;
				capturedImage.transform.localScale = new Vector3(1f, texture2D.height / (float)texture2D.width, 1f);

				Texture2D copy = duplicateTexture(texture2D);

				

				texture2D = copy;

			}
		}, maxSize);

		Debug.Log("Permission result: " + permission);
	}


	Texture2D duplicateTexture(Texture2D source)
	{
		RenderTexture renderTex = RenderTexture.GetTemporary(
					source.width,
					source.height,
					0,
					RenderTextureFormat.Default,
					RenderTextureReadWrite.Linear);

		Graphics.Blit(source, renderTex);
		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = renderTex;
		Texture2D readableText = new Texture2D(source.width, source.height);
		readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
		readableText.Apply();
		RenderTexture.active = previous;
		RenderTexture.ReleaseTemporary(renderTex);
		return readableText;
	}

	private void RecordVideo()
	{
		NativeCamera.Permission permission = NativeCamera.RecordVideo((path) =>
		{
			Debug.Log("Video path: " + path);
			if (path != null)
			{
				// Play the recorded video
				Handheld.PlayFullScreenMovie("file://" + path);
			}
		});

		Debug.Log("Permission result: " + permission);
	}

}


[Serializable]
public class FormData
{
	
	public string api_key;
	public List<string> modifiers = new List<string>();
}


