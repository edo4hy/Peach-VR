using UnityEngine;
using System.Collections;

public class Effect_Transparency : MonoBehaviour {


// Loops through Parts changing conditions to make it more suited to opacity -
// changes obtained from unity forum
  public void makeOpaque(GameObject[] Parts){
    for(int i = 0; i < Parts.Length; i++){
      Renderer tempRenderer = Parts[i].GetComponent<Renderer>();
      Material material = tempRenderer.material;
      // http://answers.unity3d.com/questions/999594/change-material-rendering-mode-but-dont-update-mat.html
      material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
      material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
      material.SetInt("_ZWrite", 1);
      material.DisableKeyword("_ALPHATEST_ON");
      material.DisableKeyword("_ALPHABLEND_ON");
      material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
      material.renderQueue = -1;
      Color oldColor = material.color;

      material.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1.0f);
    }
  }

  // Overloaded so that it can affect a single part
  public void makeOpaque(GameObject Part){
      Renderer tempRenderer = Part.GetComponent<Renderer>();
      Material material = tempRenderer.material;
      // http://answers.unity3d.com/questions/999594/change-material-rendering-mode-but-dont-update-mat.html
      material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
      material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
      material.SetInt("_ZWrite", 1);
      material.DisableKeyword("_ALPHATEST_ON");
      material.DisableKeyword("_ALPHABLEND_ON");
      material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
      material.renderQueue = -1;
      Color oldColor = material.color;

      material.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1.0f);

  }

  public void makeTransparent(GameObject[] Parts, float a){
    for(int i = 0; i < Parts.Length; i++){
      Renderer tempRenderer = Parts[i].GetComponent<Renderer>();
      Material material = tempRenderer.material;
      // http://answers.unity3d.com/questions/999594/change-material-rendering-mode-but-dont-update-mat.html
      material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
      material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
      material.SetInt("_ZWrite", 0);
      material.DisableKeyword("_ALPHATEST_ON");
      material.DisableKeyword("_ALPHABLEND_ON");
      material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
      material.renderQueue = 3000;
      Color oldColor = material.color;
      material.color = new Color(oldColor.r, oldColor.g, oldColor.b, a);
    }
  }

  // Overloaded so that it can affect a single part
  public void makeTransparent(GameObject Part, float a){
      
      Renderer tempRenderer = Part.GetComponent<Renderer>();
      Material material = tempRenderer.material;
      // http://answers.unity3d.com/questions/999594/change-material-rendering-mode-but-dont-update-mat.html
      material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
      material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
      material.SetInt("_ZWrite", 0);
      material.DisableKeyword("_ALPHATEST_ON");
      material.DisableKeyword("_ALPHABLEND_ON");
      material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
      material.renderQueue = 3000;
      Color oldColor = material.color;
      material.color = new Color(oldColor.r, oldColor.g, oldColor.b, a);

  }
}
